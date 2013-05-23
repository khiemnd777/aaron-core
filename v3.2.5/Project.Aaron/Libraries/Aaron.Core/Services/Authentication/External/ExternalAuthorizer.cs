//Contributor:  Nicholas Mayne

using System;
using Aaron.Core;
using Aaron.Core.Domain.Accounts;
using Aaron.Core.Domain.Localization;
using Aaron.Core.Services.Common;
using Aaron.Core.Services.Accounts;
using Aaron.Core.Services.Localization;
using Aaron.Core.Services.Messages;
using Aaron.Core.Services.Utilities;
using Aaron.Core.Utility;

namespace Aaron.Core.Services.Authentication.External
{
    public partial class ExternalAuthorizer : IExternalAuthorizer
    {
        #region Fields

        private readonly IAuthenticationService _authenticationService;
        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IAccountRegistrationService _accountRegistrationService;
        private readonly ILocalizationService _localizationService;
        private readonly ICurrentActivity _currentActivity;
        private readonly AccountSettings _accountSettings;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        #endregion

        #region Ctor

        public ExternalAuthorizer(IAuthenticationService authenticationService,
            IOpenAuthenticationService openAuthenticationService,
            IGenericAttributeService genericAttributeService,
            IAccountRegistrationService accountRegistrationService,  
            ILocalizationService localizationService,
            ICurrentActivity currentActivity, AccountSettings accountSettings,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            IWorkflowMessageService workflowMessageService, 
            LocalizationSettings localizationSettings)
        {
            this._authenticationService = authenticationService;
            this._openAuthenticationService = openAuthenticationService;
            this._genericAttributeService = genericAttributeService;
            this._accountRegistrationService = accountRegistrationService;
            this._localizationService = localizationService;
            this._currentActivity = currentActivity;
            this._accountSettings = accountSettings;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
            this._workflowMessageService = workflowMessageService;
            this._localizationSettings = localizationSettings;
        }
        
        #endregion

        #region Utilities

        private bool RegistrationIsEnabled()
        {
            return _accountSettings.UserRegistrationType != UserRegistrationType.Disabled && !_externalAuthenticationSettings.AutoRegisterEnabled;
        }

        private bool AutoRegistrationIsEnabled()
        {
            return _accountSettings.UserRegistrationType != UserRegistrationType.Disabled && _externalAuthenticationSettings.AutoRegisterEnabled;
        }

        private bool AccountDoesNotExistAndUserIsNotLoggedOn(Account userFound, Account userLoggedIn)
        {
            return userFound == null && userLoggedIn == null;
        }

        private bool AccountIsAssignedToLoggedOnAccount(Account userFound, Account userLoggedIn)
        {
            return userFound.Id.Equals(userLoggedIn.Id);
        }

        private bool AccountAlreadyExists(Account userFound, Account userLoggedIn)
        {
            return userFound != null && userLoggedIn != null;
        }

        #endregion

        #region Methods

        public virtual AuthorizationResult Authorize(OpenAuthenticationParameters parameters)
        {
            var userFound = _openAuthenticationService.GetUser(parameters);

            var userLoggedIn = _currentActivity.CurrentAccount.IsRegistered() ? _currentActivity.CurrentAccount : null;

            if (AccountAlreadyExists(userFound, userLoggedIn))
            {
                if (AccountIsAssignedToLoggedOnAccount(userFound, userLoggedIn))
                {
                    // The person is trying to log in as himself.. bit weird
                    return new AuthorizationResult(OpenAuthenticationStatus.Authenticated);
                }

                var result = new AuthorizationResult(OpenAuthenticationStatus.Error);
                result.AddError("Account is already assigned");
                return result;
            }
            if (AccountDoesNotExistAndUserIsNotLoggedOn(userFound, userLoggedIn))
            {
                ExternalAuthorizerHelper.StoreParametersForRoundTrip(parameters);

                if (AutoRegistrationIsEnabled())
                {
                    #region Register user

                    var currentAccount = _currentActivity.CurrentAccount;
                    var details = new RegistrationDetails(parameters);
                    var randomPassword = CommonHelper.GenerateRandomDigitCode(20);


                    bool isApproved = _accountSettings.UserRegistrationType == UserRegistrationType.Standard;
                    var registrationRequest = new AccountRegistrationRequest(currentAccount, details.EmailAddress,
                        _accountSettings.UsernamesEnabled ? details.UserName : details.EmailAddress, randomPassword, PasswordFormat.Clear, isApproved);
                    var registrationResult = _accountRegistrationService.RegisterAccount(registrationRequest);
                    if (registrationResult.Success)
                    {
                        //store other parameters (form fields)
                        if (!String.IsNullOrEmpty(details.FirstName))
                            _genericAttributeService.SaveAttribute(currentAccount, SystemAttributeNames.FirstName, details.FirstName);
                        if (!String.IsNullOrEmpty(details.LastName))
                            _genericAttributeService.SaveAttribute(currentAccount, SystemAttributeNames.LastName, details.LastName);
                    

                        userFound = currentAccount;
                        _openAuthenticationService.AssociateExternalAccountWithUser(currentAccount, parameters);
                        ExternalAuthorizerHelper.RemoveParameters();

                        //code below is copied from AccountController.Register method

                        //authenticate
                        if (isApproved)
                            _authenticationService.SignIn(userFound ?? userLoggedIn, false);

                        //notifications
                        if (_accountSettings.NotifyNewRegistration)
                            _workflowMessageService.SendAccountRegisteredNotificationMessage(currentAccount, _localizationSettings.DefaultAdminLanguageId);

                        switch (_accountSettings.UserRegistrationType)
                        {
                            case UserRegistrationType.EmailValidation:
                                {
                                    //email validation message
                                    _genericAttributeService.SaveAttribute(currentAccount, SystemAttributeNames.AccountActivationToken, Guid.NewGuid().ToString());
                                    _workflowMessageService.SendAccountEmailValidationMessage(currentAccount, _currentActivity.CurrentLanguage.Id);

                                    //result
                                    return new AuthorizationResult(OpenAuthenticationStatus.AutoRegisteredEmailValidation);
                                }
                            case UserRegistrationType.AdminApproval:
                                {
                                    //result
                                    return new AuthorizationResult(OpenAuthenticationStatus.AutoRegisteredAdminApproval);
                                }
                            case UserRegistrationType.Standard:
                                {
                                    //send account welcome message
                                    _workflowMessageService.SendAccountWelcomeMessage(currentAccount, _currentActivity.CurrentLanguage.Id);

                                    //result
                                    return new AuthorizationResult(OpenAuthenticationStatus.AutoRegisteredStandard);
                                }
                            default:
                                break;
                        }
                    }
                    else
                    {
                        ExternalAuthorizerHelper.RemoveParameters();

                        var result = new AuthorizationResult(OpenAuthenticationStatus.Error);
                        foreach (var error in registrationResult.Errors)
                            result.AddError(string.Format(error));
                        return result;
                    }

                    #endregion
                }
                else if (RegistrationIsEnabled())
                {
                    return new AuthorizationResult(OpenAuthenticationStatus.AssociateOnLogon);
                }
                else
                {
                    ExternalAuthorizerHelper.RemoveParameters();

                    var result = new AuthorizationResult(OpenAuthenticationStatus.Error);
                    result.AddError("Registration is disabled");
                    return result;
                }
            }
            if (userFound == null)
            {
                _openAuthenticationService.AssociateExternalAccountWithUser(userLoggedIn, parameters);
            }
            //authenticate
            _authenticationService.SignIn(userFound ?? userLoggedIn, false);
            
            return new AuthorizationResult(OpenAuthenticationStatus.Authenticated);
        }

        #endregion
    }
}
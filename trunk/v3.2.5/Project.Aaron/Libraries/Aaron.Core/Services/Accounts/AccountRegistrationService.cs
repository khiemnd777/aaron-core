using System;
using System.Linq;
using Aaron.Core;
using Aaron.Core.Domain.Accounts;
using Aaron.Core.Services.Localization;
using Aaron.Core.Services.Messages;
using Aaron.Core.Services.Security;
using Aaron.Core.Utility;

namespace Aaron.Core.Services.Accounts
{
    /// <summary>
    /// Account registration service
    /// </summary>
    public partial class AccountRegistrationService : IAccountRegistrationService
    {
        #region Fields

        private readonly IAccountService _accountService;
        private readonly IEncryptionService _encryptionService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly ILocalizationService _localizationService;
        private readonly AccountSettings _accountSettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="accountService">Account service</param>
        /// <param name="encryptionService">Encryption service</param>
        /// <param name="newsLetterSubscriptionService">Newsletter subscription service</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="rewardPointsSettings">Reward points settings</param>
        /// <param name="accountSettings">Account settings</param>
        public AccountRegistrationService(IAccountService accountService,
            IEncryptionService encryptionService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            ILocalizationService localizationService, 
            AccountSettings accountSettings)
        {
            this._accountService = accountService;
            this._encryptionService = encryptionService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._localizationService = localizationService;
            this._accountSettings = accountSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Validate account
        /// </summary>
        /// <param name="usernameOrEmail">Username or email</param>
        /// <param name="password">Password</param>
        /// <returns>Result</returns>
        public virtual bool ValidateAccount(string usernameOrEmail, string password)
        {
            Account account = null;
            if (_accountService != null)
            {
                if (_accountSettings.UsernamesEnabled)
                    account = _accountService.GetAccountByUsername(usernameOrEmail);
                else
                    account = _accountService.GetAccountByEmail(usernameOrEmail);

                if (account == null || account.Deleted || !account.Active)
                    return false;
            }
            else
                account = _accountService.GetAccountByEmail(usernameOrEmail);

            //only registered can login
            if (!account.IsRegistered())
                return false;

            string pwd = "";
            switch (account.PasswordFormat)
            {
                case PasswordFormat.Encrypted:
                    pwd = _encryptionService.EncryptText(password);
                    break;
                case PasswordFormat.Hashed:
                    pwd = _encryptionService.CreatePasswordHash(password, account.PasswordSalt, _accountSettings.HashedPasswordFormat);
                    break;
                default:
                    pwd = password;
                    break;
            }

            bool isValid = pwd == account.Password;

            //save last login date
            if (isValid)
            {
                account.LastLoginDateUtc = DateTime.UtcNow;
                _accountService.UpdateAccount(account);
            }
            //else
            //{
            //    account.FailedPasswordAttemptCount++;
            //    UpdateAccount(account);
            //}

            return isValid;
        }

        /// <summary>
        /// Register account
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        public virtual AccountRegistrationResult RegisterAccount(AccountRegistrationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.Account == null)
                throw new ArgumentException("Can't load current account");

            var result = new AccountRegistrationResult();
            if (request.Account.IsSearchEngineAccount())
            {
                result.AddError("Search engine can't be registered");
                return result;
            }
            if (request.Account.IsRegistered())
            {
                result.AddError("Current account is already registered");
                return result;
            }
            if (String.IsNullOrEmpty(request.Email))
            {
                result.AddError(_localizationService.GetResource("Account.Register.Errors.EmailIsNotProvided"));
                return result;
            }
            if (!CommonHelper.IsValidEmail(request.Email))
            {
                result.AddError(_localizationService.GetResource("Common.WrongEmail"));
                return result;
            }
            if (String.IsNullOrWhiteSpace(request.Password))
            {
                result.AddError(_localizationService.GetResource("Account.Register.Errors.PasswordIsNotProvided"));
                return result;
            }
            if (_accountSettings.UsernamesEnabled)
            {
                if (String.IsNullOrEmpty(request.Username))
                {
                    result.AddError(_localizationService.GetResource("Account.Register.Errors.UsernameIsNotProvided"));
                    return result;
                }
            }

            //validate unique user
            if (_accountService.GetAccountByEmail(request.Email) != null)
            {
                result.AddError(_localizationService.GetResource("Account.Register.Errors.EmailAlreadyExists"));
                return result;
            }
            if (_accountSettings.UsernamesEnabled)
            {
                if (_accountService.GetAccountByUsername(request.Username) != null)
                {
                    result.AddError(_localizationService.GetResource("Account.Register.Errors.UsernameAlreadyExists"));
                    return result;
                }
            }

            //at this point request is valid
            request.Account.Username = request.Username;
            request.Account.Email = request.Email;
            request.Account.PasswordFormat = request.PasswordFormat;

            switch (request.PasswordFormat)
            {
                case PasswordFormat.Clear:
                    {
                        request.Account.Password = request.Password;
                    }
                    break;
                case PasswordFormat.Encrypted:
                    {
                        request.Account.Password = _encryptionService.EncryptText(request.Password);
                    }
                    break;
                case PasswordFormat.Hashed:
                    {
                        string saltKey = _encryptionService.CreateSaltKey(5);
                        request.Account.PasswordSalt = saltKey;
                        request.Account.Password = _encryptionService.CreatePasswordHash(request.Password, saltKey, _accountSettings.HashedPasswordFormat);
                    }
                    break;
                default:
                    break;
            }

            request.Account.Active = request.IsApproved;

            //add to 'Registered' role
            var registeredRole = _accountService.GetAccountRoleBySystemName(SystemAccountRoleNames.Registered);
            if (registeredRole == null)
                throw new AaronException("'Registered' role could not be loaded");
            request.Account.AccountRoles.Add(registeredRole);
            //remove from 'Guests' role
            var guestRole = request.Account.AccountRoles.FirstOrDefault(cr => cr.SystemName == SystemAccountRoleNames.Guests);
            if (guestRole != null)
                request.Account.AccountRoles.Remove(guestRole);

            
            _accountService.UpdateAccount(request.Account);
            return result;
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        public virtual PasswordChangeResult ChangePassword(ChangePasswordRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new PasswordChangeResult();
            if (String.IsNullOrWhiteSpace(request.Email))
            {
                result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.EmailIsNotProvided"));
                return result;
            }
            if (String.IsNullOrWhiteSpace(request.NewPassword))
            {
                result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.PasswordIsNotProvided"));
                return result;
            }

            var account = _accountService.GetAccountByEmail(request.Email);
            if (account == null)
            {
                result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.EmailNotFound"));
                return result;
            }


            var requestIsValid = false;
            if (request.ValidateRequest)
            {
                //password
                string oldPwd = "";
                switch (account.PasswordFormat)
                {
                    case PasswordFormat.Encrypted:
                        oldPwd = _encryptionService.EncryptText(request.OldPassword);
                        break;
                    case PasswordFormat.Hashed:
                        oldPwd = _encryptionService.CreatePasswordHash(request.OldPassword, account.PasswordSalt, _accountSettings.HashedPasswordFormat);
                        break;
                    default:
                        oldPwd = request.OldPassword;
                        break;
                }

                bool oldPasswordIsValid = oldPwd == account.Password;
                if (!oldPasswordIsValid)
                    result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.OldPasswordDoesntMatch"));

                if (oldPasswordIsValid)
                    requestIsValid = true;
            }
            else
                requestIsValid = true;


            //at this point request is valid
            if (requestIsValid)
            {
                switch (request.NewPasswordFormat)
                {
                    case PasswordFormat.Clear:
                        {
                            account.Password = request.NewPassword;
                        }
                        break;
                    case PasswordFormat.Encrypted:
                        {
                            account.Password = _encryptionService.EncryptText(request.NewPassword);
                        }
                        break;
                    case PasswordFormat.Hashed:
                        {
                            string saltKey = _encryptionService.CreateSaltKey(5);
                            account.PasswordSalt = saltKey;
                            account.Password = _encryptionService.CreatePasswordHash(request.NewPassword, saltKey, _accountSettings.HashedPasswordFormat);
                        }
                        break;
                    default:
                        break;
                }
                account.PasswordFormat = request.NewPasswordFormat;
                _accountService.UpdateAccount(account);
            }

            return result;
        }

        /// <summary>
        /// Sets a user email
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="newEmail">New email</param>
        public virtual void SetEmail(Account account, string newEmail)
        {
            if (account == null)
                throw new ArgumentNullException("account");

            newEmail = newEmail.Trim();
            string oldEmail = account.Email;

            if (!CommonHelper.IsValidEmail(newEmail))
                throw new AaronException(_localizationService.GetResource("Account.EmailUsernameErrors.NewEmailIsNotValid"));

            if (newEmail.Length > 100)
                throw new AaronException(_localizationService.GetResource("Account.EmailUsernameErrors.EmailTooLong"));

            var account2 = _accountService.GetAccountByEmail(newEmail);
            if (account2 != null && account.Id != account2.Id)
                throw new AaronException(_localizationService.GetResource("Account.EmailUsernameErrors.EmailAlreadyExists"));

            account.Email = newEmail;
            _accountService.UpdateAccount(account);

            //update newsletter subscription (if required)
            if (!String.IsNullOrEmpty(oldEmail) && !oldEmail.Equals(newEmail, StringComparison.InvariantCultureIgnoreCase))
            {
                var subscriptionOld = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmail(oldEmail);
                if (subscriptionOld != null)
                {
                    subscriptionOld.Email = newEmail;
                    _newsLetterSubscriptionService.UpdateNewsLetterSubscription(subscriptionOld);
                }
            }
        }

        /// <summary>
        /// Sets a account username
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="newUsername">New Username</param>
        public virtual void SetUsername(Account account, string newUsername)
        {
            if (account == null)
                throw new ArgumentNullException("account");

            if (!_accountSettings.UsernamesEnabled)
                throw new AaronException("Usernames are disabled");

            if (!_accountSettings.AllowUsersToChangeUsernames)
                throw new AaronException("Changing usernames is not allowed");

            newUsername = newUsername.Trim();

            if (newUsername.Length > 100)
                throw new AaronException(_localizationService.GetResource("Account.EmailUsernameErrors.UsernameTooLong"));

            var user2 = _accountService.GetAccountByUsername(newUsername);
            if (user2 != null && account.Id != user2.Id)
                throw new AaronException(_localizationService.GetResource("Account.EmailUsernameErrors.UsernameAlreadyExists"));

            account.Username = newUsername;
            _accountService.UpdateAccount(account);
        }

        #endregion
    }
}
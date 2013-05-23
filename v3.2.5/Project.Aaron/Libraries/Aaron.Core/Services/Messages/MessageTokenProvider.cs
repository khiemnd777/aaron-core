using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using Aaron.Core;
using Aaron.Core.Domain;
using Aaron.Core.Domain.Accounts;
using Aaron.Core.Domain.Messages;
using Aaron.Core.Services.Accounts;
using Aaron.Core.Services.Utilities;
using Aaron.Core.Services.Localization;
using Aaron.Core.Services.Helpers;
using Aaron.Core.Services.Messages;
using Aaron.Core.Web;

namespace Aaron.Core.Services.Messages
{
    public partial class MessageTokenProvider : IMessageTokenProvider
    {
        #region Fields

        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IWebHelper _webHelper;
        private readonly ICurrentActivity _currentActivity;

        private readonly WebInformationSettings _webSettings;
        private readonly MessageTemplatesSettings _templatesSettings;
        private readonly EmailAccountSettings _emailAccountSettings;

        #endregion

        #region Ctor

        public MessageTokenProvider(ILanguageService languageService,
            ILocalizationService localizationService, 
            IDateTimeHelper dateTimeHelper,
            IEmailAccountService emailAccountService,
            IWebHelper webHelper,
            ICurrentActivity currentActivity,
            MessageTemplatesSettings templatesSettings,
            EmailAccountSettings emailAccountSettings,
            WebInformationSettings webSettings)
        {
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._dateTimeHelper = dateTimeHelper;
            this._emailAccountService = emailAccountService;
            this._webHelper = webHelper;
            this._currentActivity = currentActivity;

            this._webSettings = webSettings;
            this._templatesSettings = templatesSettings;
            this._emailAccountSettings = emailAccountSettings;
        }

        #endregion

        #region Methods

        public virtual void AddWebTokens(IList<Token> tokens)
        {
            tokens.Add(new Token("Web.Name", _webSettings.WebName));
            tokens.Add(new Token("Web.URL", _webSettings.WebUrl, true));
            var defaultEmailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
            if (defaultEmailAccount == null)
                defaultEmailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
            tokens.Add(new Token("Web.Email", defaultEmailAccount.Email));
        }

        

        public virtual void AddAccountTokens(IList<Token> tokens, Account account)
        {
            tokens.Add(new Token("Account.Email", account.Email));
            tokens.Add(new Token("Account.Username", account.Username));
            tokens.Add(new Token("Account.FullName", account.GetFullName()));

            //note: we do not use SEO friendly URLS because we can get errors caused by having .(dot) in the URL (from the emauk address)
            //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
            string passwordRecoveryUrl = string.Format("{0}account/passwordrecoveryconfirm?token={1}&email={2}", _webHelper.GetWebLocation(false), account.GetAttribute<string>(SystemAttributeNames.PasswordRecoveryToken), account.Email);
            string accountActivationUrl = string.Format("{0}account/activation?token={1}&email={2}", _webHelper.GetWebLocation(false), account.GetAttribute<string>(SystemAttributeNames.AccountActivationToken), account.Email);
            tokens.Add(new Token("Account.PasswordRecoveryURL", passwordRecoveryUrl, true));
            tokens.Add(new Token("Account.AccountActivationURL", accountActivationUrl, true));
        }

        public virtual void AddNewsLetterSubscriptionTokens(IList<Token> tokens, NewsLetterSubscription subscription)
        {
            tokens.Add(new Token("NewsLetterSubscription.Email", subscription.Email));

            const string urlFormat = "{0}newsletter/subscriptionactivation/{1}/{2}";

            var activationUrl = String.Format(urlFormat, _webHelper.GetWebLocation(false), subscription.NewsLetterSubscriptionGuid, "true");
            tokens.Add(new Token("NewsLetterSubscription.ActivationUrl", activationUrl, true));

            var deActivationUrl = String.Format(urlFormat, _webHelper.GetWebLocation(false), subscription.NewsLetterSubscriptionGuid, "false");
            tokens.Add(new Token("NewsLetterSubscription.DeactivationUrl", deActivationUrl, true));
        }

        /// <summary>
        /// Gets list of allowed (supported) message tokens for campaigns
        /// </summary>
        /// <returns>List of allowed (supported) message tokens for campaigns</returns>
        public virtual string[] GetListOfCampaignAllowedTokens()
        {
            var allowedTokens = new List<string>()
            {
                "%Web.Name%",
                "%Web.URL%",
                "%Web.Email%",
                "%NewsLetterSubscription.Email%",
                "%NewsLetterSubscription.ActivationUrl%",
                "%NewsLetterSubscription.DeactivationUrl%"
            };
            return allowedTokens.ToArray();
        }

        public virtual string[] GetListOfAllowedTokens()
        {
            var allowedTokens = new List<string>()
            {
                "%Web.Name%",
                "%Web.URL%",
                "%Web.Email%",
                "%Account.Email%", 
                "%Account.Username%", 
                "%Account.FullName%", 
                "%Account.PasswordRecoveryURL%", 
                "%Account.AccountActivationURL%", 
                "NewsLetterSubscription.Email%", 
                "%NewsLetterSubscription.ActivationUrl%",
                "%NewsLetterSubscription.DeactivationUrl%", 
            };
            return allowedTokens.ToArray();
        }
        
        #endregion
    }
}

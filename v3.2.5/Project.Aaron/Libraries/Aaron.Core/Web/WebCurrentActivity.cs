using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Aaron.Core.Services.Accounts;
using Aaron.Core.Services.Authentication;
using Aaron.Core.Services.Localization;
using Aaron.Core.Services.Utilities;
using Aaron.Core.Domain.Localization;
using Aaron.Core.Domain.Accounts;
using Aaron.Core.Web.Localization;

namespace Aaron.Core.Web
{
    public class WebCurrentActivity : ICurrentActivity
    {
        private const string AccountCookieName = "Aaron.account";

        private readonly HttpContextBase _httpContext;
        private readonly IAccountService _accountService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILanguageService _languageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IWebHelper _webHelper;

        private Account _cachedAccount;
        private Account _originalAccountIfImpersonated;
        private bool _cachedIsAdmin;

        public WebCurrentActivity(HttpContextBase httpContext,
            IAccountService accountService,
            IAuthenticationService authenticationService,
            ILanguageService languageService,
            LocalizationSettings localizationSettings,
            IWebHelper webHelper)
        {
            this._httpContext = httpContext;
            this._accountService = accountService;
            this._authenticationService = authenticationService;
            this._languageService = languageService;
            this._localizationSettings = localizationSettings;
            this._webHelper = webHelper;
        }

        protected Account GetCurrentAccount()
        {
            if (_cachedAccount != null)
                return _cachedAccount;

            Account account = null;
            if (_httpContext != null)
            {
                //check whether request is made by a search engine
                //in this case return built-in account record for search engines 
                //or comment the following two lines of code in order to disable this functionality
                if (_webHelper.IsSearchEngine(_httpContext))
                    account = _accountService.GetAccountBySystemName(SystemAccountNames.SearchEngine);

                //registered user
                if (account == null || account.Deleted || !account.Active)
                {
                    account = _authenticationService.GetAuthenticatedAccount();
                }

                //impersonate user if required (currently used for 'phone order' support)
                if (account != null && !account.Deleted && account.Active)
                {
                        int? impersonatedAccountId = account.GetAttribute<int?>(SystemAttributeNames.ImpersonatedAccountId);
                        if (impersonatedAccountId.HasValue && impersonatedAccountId.Value > 0)
                        {
                            var impersonatedAccount = _accountService.GetAccountById(impersonatedAccountId.Value);
                            if (impersonatedAccount != null && !impersonatedAccount.Deleted && impersonatedAccount.Active)
                            {
                                //set impersonated account
                                _originalAccountIfImpersonated = account;
                                account = impersonatedAccount;
                            }
                        }
                }

                //load guest account
                if (account == null || account.Deleted || !account.Active)
                {
                    var accountCookie = GetAccountCookie();
                    if (accountCookie != null && !String.IsNullOrEmpty(accountCookie.Value))
                    {
                        Guid accountGuid;
                        if (Guid.TryParse(accountCookie.Value, out accountGuid))
                        {
                            var accountByCookie = _accountService.GetAccountByGuid(accountGuid);
                            if (accountByCookie != null &&
                                //this account (from cookie) should not be registered
                                !accountByCookie.IsRegistered() &&
                                //it should not be a built-in 'search engine' account account
                                !accountByCookie.IsSearchEngineAccount())
                                account = accountByCookie;
                        }
                    }
                }

                //create guest if not exists
                if (account == null || account.Deleted || !account.Active)
                {
                    account = _accountService.InsertGuestAccount();
                }

                SetAccountCookie(account.AccountGuid);
            }

            //validation
            if (account != null && !account.Deleted && account.Active)
            {
                //update last activity date
                if (account.LastActivityDateUtc.AddMinutes(1.0) < DateTime.UtcNow)
                {
                    account.LastActivityDateUtc = DateTime.UtcNow;
                    _accountService.UpdateAccount(account);
                }

                //update IP address
                string currentIpAddress = _webHelper.GetCurrentIpAddress();
                if (!String.IsNullOrEmpty(currentIpAddress))
                {
                    if (!currentIpAddress.Equals(account.LastIpAddress))
                    {
                        account.LastIpAddress = currentIpAddress;
                        _accountService.UpdateAccount(account);
                    }
                }

                _cachedAccount = account;
            }

            return _cachedAccount;
        }

        protected HttpCookie GetAccountCookie()
        {
            if (_httpContext == null || _httpContext.Request == null)
                return null;

            return _httpContext.Request.Cookies[AccountCookieName];
        }

        protected void SetAccountCookie(Guid accountGuid)
        {
            var cookie = new HttpCookie(AccountCookieName);
            cookie.Value = accountGuid.ToString();
            if (accountGuid == Guid.Empty)
            {
                cookie.Expires = DateTime.Now.AddMonths(-1);
            }
            else
            {
                int cookieExpires = 24 * 365; //TODO make configurable
                cookie.Expires = DateTime.Now.AddHours(cookieExpires);
            }
            if (_httpContext != null && _httpContext.Response != null)
            {
                _httpContext.Response.Cookies.Remove(AccountCookieName);
                _httpContext.Response.Cookies.Add(cookie);
            }
        }

        /// <summary>
        /// Gets or sets the current account
        /// </summary>
        public Account CurrentAccount
        {
            get
            {
                return GetCurrentAccount();
            }
            set
            {
                SetAccountCookie(value.AccountGuid);
                _cachedAccount = value;
            }
        }

        /// <summary>
        /// Gets or sets the original account (in case the current one is impersonated)
        /// </summary>
        public Account OriginalAccountIfImpersonated
        {
            get
            {
                return _originalAccountIfImpersonated;
            }
        }

        /// <summary>
        /// Get or set current user working language
        /// </summary>
        public Language CurrentLanguage
        {
            get
            {
                //get language from URL (if possible)
                if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    if (_httpContext != null)
                    {
                        string virtualPath = _httpContext.Request.AppRelativeCurrentExecutionFilePath;
                        string applicationPath = _httpContext.Request.ApplicationPath;
                        if (virtualPath.IsLocalizedUrl(applicationPath, false))
                        {
                            var seoCode = virtualPath.GetLanguageSeoCodeFromUrl(applicationPath, false);
                            if (!String.IsNullOrEmpty(seoCode))
                            {
                                var langByCulture = _languageService.GetAllLanguages()
                                    .Where(l => seoCode.Equals(l.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase))
                                    .FirstOrDefault();
                                if (langByCulture != null && langByCulture.Published)
                                {
                                    //the language is found. now we need to save it
                                    if (this.CurrentAccount != null &&
                                        !langByCulture.Equals(this.CurrentAccount.Language))
                                    {
                                        this.CurrentAccount.Language = langByCulture;
                                        _accountService.UpdateAccount(this.CurrentAccount);
                                    }
                                }
                            }
                        }
                    }
                }
                if (this.CurrentAccount != null &&
                    this.CurrentAccount.Language != null &&
                    this.CurrentAccount.Language.Published)
                    return this.CurrentAccount.Language;

                var lang = _languageService.GetAllLanguages().FirstOrDefault();
                return lang;
            }
            set
            {
                if (this.CurrentAccount == null)
                    return;

                this.CurrentAccount.Language = value;
                _accountService.UpdateAccount(this.CurrentAccount);
            }
        }

        public bool IsAdmin
        {
            get
            {
                return _cachedIsAdmin;
            }
            set
            {
                _cachedIsAdmin = value;
            }
        }
    }
}

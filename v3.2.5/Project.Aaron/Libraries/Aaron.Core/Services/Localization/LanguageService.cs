using System;
using System.Collections.Generic;
using System.Linq;
using Aaron.Core.Caching;
using Aaron.Core.Data;
using Aaron.Core.Domain.Localization;
using Aaron.Core.Services.Configuration;
using Aaron.Core.Services.Accounts;
//using Aaron.Core.Services.Events;

namespace Aaron.Core.Services.Localization
{
    /// <summary>
    /// Language service
    /// </summary>
    public partial class LanguageService : ILanguageService
    {
        #region Constants
        private const string LANGUAGES_ALL_KEY = "Aaron.language.all-{0}";
        private const string LANGUAGES_BY_ID_KEY = "Aaron.language.id-{0}";
        private const string LANGUAGES_PATTERN_KEY = "Aaron.language.";
        #endregion

        #region Fields

        private readonly IRepository<Language> _languageRepository;
        private readonly IAccountService _accountService;
        private readonly ICacheManager _cacheManager;
        private readonly ISettingService _settingService;
        private readonly LocalizationSettings _localizationSettings;
        //private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="languageRepository">Language repository</param>
        /// <param name="accountService">Account service</param>
        /// <param name="settingService">Setting service</param>
        /// <param name="localizationSettings">Localization settings</param>
        /// <param name="eventPublisher">Event published</param>
        public LanguageService(ICacheManager cacheManager,
            IRepository<Language> languageRepository,
            IAccountService accountService,
            ISettingService settingService,
            LocalizationSettings localizationSettings/*,
            IEventPublisher eventPublisher*/)
        {
            this._cacheManager = cacheManager;
            this._languageRepository = languageRepository;
            this._accountService = accountService;
            this._settingService = settingService;
            this._localizationSettings = localizationSettings;
            //this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a language
        /// </summary>
        /// <param name="language">Language</param>
        public virtual void DeleteLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            //update default admin area language (if required)
            if (_localizationSettings.DefaultAdminLanguageId == language.Id)
            {
                foreach (var activeLanguage in GetAllLanguages())
                {
                    if (activeLanguage.Id != language.Id)
                    {
                        _localizationSettings.DefaultAdminLanguageId = activeLanguage.Id;
                        _settingService.SaveSetting(_localizationSettings);
                        break;
                    }
                }
            }

            //update appropriate accounts (their language)
            //it can take a lot of time if you have thousands of associated accounts
            var accounts = _accountService.GetAccountsByLanguageId(language.Id);
            foreach (var account in accounts)
            {
                account.LanguageId = null;
                _accountService.UpdateAccount(account);
            }

            _languageRepository.Delete(language);

            //cache
            _cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);

            //event notification
            //_eventPublisher.EntityDeleted(language);
        }

        /// <summary>
        /// Gets all languages
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Language collection</returns>
        public virtual IList<Language> GetAllLanguages(bool showHidden = false)
        {
            string key = string.Format(LANGUAGES_ALL_KEY, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = _languageRepository.Table;
                if (!showHidden)
                    query = query.Where(l => l.Published);
                query = query.OrderBy(l => l.DisplayOrder);
                var languages = query.ToList();
                return languages;
            });
        }

        /// <summary>
        /// Gets a language
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Language</returns>
        public virtual Language GetLanguageById(int languageId)
        {
            if (languageId == 0)
                return null;

            string key = string.Format(LANGUAGES_BY_ID_KEY, languageId);
            return _cacheManager.Get(key, () =>
            {
                return _languageRepository.GetById(languageId);
            });
        }

        /// <summary>
        /// Inserts a language
        /// </summary>
        /// <param name="language">Language</param>
        public virtual void InsertLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            _languageRepository.Insert(language);

            //cache
            _cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);

            //event notification
            //_eventPublisher.EntityInserted(language);
        }

        /// <summary>
        /// Updates a language
        /// </summary>
        /// <param name="language">Language</param>
        public virtual void UpdateLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            //update language
            _languageRepository.Update(language);

            //cache
            _cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);

            //event notification
            //_eventPublisher.EntityUpdated(language);
        }

        #endregion
    }
}

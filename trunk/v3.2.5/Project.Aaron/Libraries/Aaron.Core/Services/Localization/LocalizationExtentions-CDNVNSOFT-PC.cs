using System;
using System.Linq.Expressions;
using System.Reflection;
using Aaron.Core;
using Aaron.Core.Domain.Localization;
using Aaron.Core.Infrastructure;
using Aaron.Core.Services.Localization;
using Aaron.Core.Utility;

namespace Aaron.Core.Services.Localization
{
    public static class LocalizationExtentions
    {
        /// <summary>
        /// Get localized property of an entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <returns>Localized property</returns>
        public static string GetLocalized<T>(this T entity,
            Expression<Func<T, string>> keySelector)
            where T : BaseEntity<int>, ILocalizedEntity
        {
            var workContext = IoC.Resolve<ICurrentActivity>();
            return GetLocalized(entity, keySelector, workContext.CurrentLanguage.Id);
        }
        /// <summary>
        /// Get localized property of an entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
        /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
        /// <returns>Localized property</returns>
        public static string GetLocalized<T>(this T entity, 
            Expression<Func<T, string>> keySelector, int languageId, 
            bool returnDefaultValue = true, bool ensureTwoPublishedLanguages = true) 
            where T : BaseEntity<int>, ILocalizedEntity
        {
            return GetLocalized<T, string>(entity, keySelector, languageId, returnDefaultValue, ensureTwoPublishedLanguages);
        }
        /// <summary>
        /// Get localized property of an entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
        /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
        /// <returns>Localized property</returns>
        public static TPropType GetLocalized<T, TPropType>(this T entity,
            Expression<Func<T, TPropType>> keySelector, int languageId, 
            bool returnDefaultValue = true, bool ensureTwoPublishedLanguages = true)
            where T : BaseEntity<int>, ILocalizedEntity
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            var member = keySelector.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    keySelector));
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(string.Format(
                       "Expression '{0}' refers to a field, not a property.",
                       keySelector));
            }

            TPropType result = default(TPropType);
            string resultStr = string.Empty;

            //load localized value
            string localeKeyGroup = typeof(T).Name;
            string localeKey = propInfo.Name;

            if (languageId != 0)
            {
                //ensure that we have at least two published languages
                bool loadLocalizedValue = true;
                if (ensureTwoPublishedLanguages)
                {
                    var lService = IoC.Resolve<ILanguageService>();
                    var totalPublishedLanguages = lService.GetAllLanguages(false).Count;
                    loadLocalizedValue = totalPublishedLanguages >= 2;
                }

                //localized value
                if (loadLocalizedValue)
                {
                    var leService = IoC.Resolve<ILocalizedEntityService>();
                    //old approach (commented)
                    //var props = leService.GetLocalizedProperties(entity.Id, localeKeyGroup);
                    //var prop = props.FirstOrDefault(lp => lp.LanguageId == languageId &&
                    //    lp.LocaleKeyGroup.Equals(localeKeyGroup, StringComparison.InvariantCultureIgnoreCase) && //should be culture invariant
                    //    lp.LocaleKey.Equals(localeKey, StringComparison.InvariantCultureIgnoreCase));//should be culture invariant
                    //if (prop != null)
                    //{
                    //    resultStr = prop.LocaleValue;
                    //    result = CommonHelper.To<TPropType>(resultStr);
                    //}

                    //new approach
                    resultStr = leService.GetLocalizedValue(languageId, entity.Id, localeKeyGroup, localeKey);
                    if (!String.IsNullOrEmpty(resultStr))
                        result = CommonHelper.To<TPropType>(resultStr);
                }
            }

            //set default value if required
            if (String.IsNullOrEmpty(resultStr) && returnDefaultValue)
            {
                var localizer = keySelector.Compile();
                result = localizer(entity);
            }
            
            return result;
        }
    }
}

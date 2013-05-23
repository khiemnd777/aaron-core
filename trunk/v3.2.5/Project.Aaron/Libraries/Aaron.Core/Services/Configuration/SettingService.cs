﻿using System;
using System.Collections.Generic;
using System.Linq;
using Aaron.Core;
using Aaron.Core.Utility;
using Aaron.Core.Caching;
using Aaron.Core.SysConfiguration;
using Aaron.Core.Data;
using Aaron.Core.Domain.Configuration;
using Aaron.Core.Infrastructure;

namespace Aaron.Core.Services.Configuration
{
    /// <summary>
    /// Setting manager
    /// </summary>
    public partial class SettingService : ISettingService
    {
        #region Constants
        private const string SETTINGS_ALL_KEY = "Aaron.setting.all";
        #endregion

        #region Fields

        private readonly IRepository<Setting> _settingRepository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="settingRepository">Setting repository</param>
        public SettingService(ICacheManager staticCacheManager,
            IRepository<Setting> settingRepository)
        {
            this._cacheManager = staticCacheManager;
            this._settingRepository = settingRepository;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Adds a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void InsertSetting(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            setting.CreationDate = DateTime.Now;
            setting.ModifiedDate = DateTime.Now;

            _settingRepository.Insert(setting);

            //cache
            if (clearCache)
                _cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);
        }

        /// <summary>
        /// Updates a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void UpdateSetting(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            setting.ModifiedDate = DateTime.Now;

            _settingRepository.Update(setting);

            //cache
            if (clearCache)
                _cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a setting by identifier
        /// </summary>
        /// <param name="settingId">Setting identifier</param>
        /// <returns>Setting</returns>
        public virtual Setting GetSettingById(int settingId)
        {
            if (settingId == 0)
                return null;

            var setting = _settingRepository.GetById(settingId);
            return setting;
        }

        /// <summary>
        /// Get setting by key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Setting object</returns>
        public Setting GetSettingByKey(string key)
        {
            if (String.IsNullOrEmpty(key))
                return null;

            key = key.Trim().ToLowerInvariant();

            var settings = GetAllSettings();
            if (settings.ContainsKey(key))
            {
                var id = settings[key].Id;
                return GetSettingById(id);
            }

            return null;
        }

        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Setting value</returns>
        public virtual T GetSettingByKey<T>(string key, T defaultValue = default(T))
        {
            if (String.IsNullOrEmpty(key))
                return defaultValue;

            key = key.Trim().ToLowerInvariant();

            var settings = GetAllSettings();
            if (settings.ContainsKey(key))
            {
                var setting = settings[key];
                return setting.As<T>();
            }
            return defaultValue;
        }

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void SetSetting<T>(string key, T value, bool clearCache = true)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            key = key.Trim().ToLowerInvariant();

            var settings = GetAllSettings();

            Setting setting = null;
            string valueStr = CommonHelper.GetCustomTypeConverter(typeof(T)).ConvertToInvariantString(value);
            if (settings.ContainsKey(key))
            {
                //update
                setting = settings[key];
                //little hack here because of EF issue
                setting = GetSettingById(setting.Id);
                setting.Value = valueStr;
                UpdateSetting(setting, clearCache);
            }
            else
            {
                //insert
                setting = new Setting()
                {
                    Name = key,
                    Value = valueStr,
                };
                InsertSetting(setting, clearCache);
            }
        }

        /// <summary>
        /// Deletes a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        public virtual void DeleteSetting(Setting setting)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _settingRepository.Delete(setting);

            //cache
            _cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);
        }

        /// <summary>
        /// Delete all settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        public virtual void DeleteSetting<T>() where T : ISettings, new()
        {
            IoC.Resolve<ISysConfigurationProvider<T>>().DeleteSettings();
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>Setting collection</returns>
        public virtual IDictionary<string, Setting> GetAllSettings()
        {
            //cache
            string key = string.Format(SETTINGS_ALL_KEY);
            return _cacheManager.Get(key, () =>
            {
                var query = from s in _settingRepository.Table
                            orderby s.Name
                            select s;
                var settings = query.ToDictionary(s => s.Name.ToLowerInvariant());

                return settings;
            });
        }

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="settingInstance">Setting instance</param>
        public virtual void SaveSetting<T>(T settingInstance) where T : ISettings, new()
        {
            //We should be sure that an appropriate ISettings object will not be cached in IoC tool after updating (by default cached per HTTP request)
            IoC.Resolve<ISysConfigurationProvider<T>>().SaveSettings(settingInstance);
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        public virtual void ClearCache()
        {
            _cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);
        }
        #endregion
    }
}
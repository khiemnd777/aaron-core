using System.Linq;
using Aaron.Core;
using Aaron.Core.Domain;
using Aaron.Core.Domain.Accounts;
using Aaron.Core.Services.Utilities;
using Aaron.Core.Web;

namespace Aaron.Core.Web.Themes
{
    /// <summary>
    /// Theme context
    /// </summary>
    public partial class ThemeContext : IThemeContext
    {
        private readonly ICurrentActivity _currentActivity;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly WebInformationSettings _webInformationSettings;
        private readonly IThemeProvider _themeProvider;

        private bool _desktopThemeIsCached;
        private string _cachedDesktopThemeName;

        private bool _mobileThemeIsCached;
        private string _cachedMobileThemeName;

        public ThemeContext(ICurrentActivity currentActivity, IGenericAttributeService genericAttributeService,
            WebInformationSettings webInformationSettings, IThemeProvider themeProvider)
        {
            this._currentActivity = currentActivity;
            this._genericAttributeService = genericAttributeService;
            this._webInformationSettings = webInformationSettings;
            this._themeProvider = themeProvider;
        }

        /// <summary>
        /// Get or set current theme for desktops (e.g. darkOrange)
        /// </summary>
        public string WorkingDesktopTheme
        {
            get
            {
                if (_desktopThemeIsCached)
                    return _cachedDesktopThemeName;

                string theme = "";
                if (_webInformationSettings.AllowAccountToSelectTheme)
                {
                    if (_currentActivity.CurrentAccount != null)
                        theme = _currentActivity.CurrentAccount.GetAttribute<string>(SystemAttributeNames.WorkingDesktopThemeName);
                }

                //default store theme
                if (string.IsNullOrEmpty(theme))
                    theme = _webInformationSettings.DefaultWebThemeForDesktops;

                //ensure that theme exists
                if (!_themeProvider.ThemeConfigurationExists(theme))
                    theme = _themeProvider.GetThemeConfigurations()
                        .Where(x => !x.MobileTheme)
                        .FirstOrDefault()
                        .ThemeName;
                
                //cache theme
                this._cachedDesktopThemeName = theme;
                this._desktopThemeIsCached = true;
                return theme;
            }
            set
            {
                if (!_webInformationSettings.AllowAccountToSelectTheme)
                    return;

                if (_currentActivity.CurrentAccount == null)
                    return;

                _genericAttributeService.SaveAttribute(_currentActivity.CurrentAccount, SystemAttributeNames.WorkingDesktopThemeName, value);

                //clear cache
                this._desktopThemeIsCached = false;
            }
        }

        /// <summary>
        /// Get current theme for mobile (e.g. Mobile)
        /// </summary>
        public string WorkingMobileTheme
        {
            get
            {
                return null;
            }
        }
    }
}

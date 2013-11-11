using System.Web;
using Aaron.Core;
using Aaron.Core.Domain;
using Aaron.Core.Domain.Accounts;
using Aaron.Core.Services.Utilities;
using Aaron.Core.Web;

namespace Aaron.Core.Services.Common
{
    /// <summary>
    /// Mobile device helper
    /// </summary>
    public partial class MobileDeviceHelper : IMobileDeviceHelper
    {
        #region Fields

        private readonly WebInformationSettings _webInformationSettings;
        private readonly ICurrentActivity _currentActivity;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="webInformationSettings">Store information settings</param>
        /// <param name="currentActivity">Work context</param>
        public MobileDeviceHelper(WebInformationSettings webInformationSettings,
            ICurrentActivity currentActivity)
        {
            this._webInformationSettings = webInformationSettings;
            this._currentActivity = currentActivity;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Returns a value indicating whether request is made by a mobile device
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <returns>Result</returns>
        public virtual bool IsMobileDevice(HttpContextBase httpContext)
        {
            if (_webInformationSettings.EmulateMobileDevice)
                return true;

            //comment the code below if you want tablets to be recognized as mobile devices.
            //nopCommerce uses the free edition of the 51degrees.mobi library for detecting browser mobile properties.
            //by default this property (IsTablet) is always false. you will need the premium edition in order to get it supported.
            bool isTablet = false;
            if (bool.TryParse(httpContext.Request.Browser["IsTablet"], out isTablet) && isTablet)
                return false;

            if (httpContext.Request.Browser.IsMobileDevice)
                return true;

            return false;
        }

        /// <summary>
        /// Returns a value indicating whether mobile devices support is enabled
        /// </summary>
        public virtual bool MobileDevicesSupported()
        {
            return _webInformationSettings.MobileDevicesSupported;
        }

        /// <summary>
        /// Returns a value indicating whether current account prefer to use full desktop version (even request is made by a mobile device)
        /// </summary>
        public virtual bool AccountDontUseMobileVersion()
        {
            return _currentActivity.CurrentAccount.GetAttribute<bool>(SystemAttributeNames.DontUseMobileVersion);
        }

        #endregion
    }
}
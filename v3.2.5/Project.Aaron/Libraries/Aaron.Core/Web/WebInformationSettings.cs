using Aaron.Core.SysConfiguration;

namespace Aaron.Core.Web
{
    public class WebInformationSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a Web name
        /// </summary>
        public string WebName { get; set; }

        /// <summary>
        /// Gets or sets a Web URL
        /// </summary>
        public string WebUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Web is closed
        /// </summary>
        public bool WebClosed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether administrators can visit a closed Web
        /// </summary>
        public bool WebClosedAllowForAdmins { get; set; }

        /// <summary>
        /// Gets or sets a default Web theme for desktops
        /// </summary>
        public string DefaultWebThemeForDesktops { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether accounts are allowed to select a theme
        /// </summary>
        public bool AllowAccountToSelectTheme { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether mobile devices supported
        /// </summary>
        //public bool MobileDevicesSupported { get; set; }

        /// <summary>
        /// Gets or sets a default Web theme used by mobile devices (if enabled)
        /// </summary>
        //public string DefaultWebThemeForMobileDevices { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all requests will be handled as mobile devices (used for testing)
        /// </summary>
        //public bool EmulateMobileDevice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether mini profiler should be displayed in public Web (used for debugging)
        /// </summary>
        public bool UseMiniProfiler { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should display warnings about the new EU cookie law
        /// </summary>
        //public bool DisplayEuCookieLawWarning { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aaron.Core;
using Aaron.Core.Domain.Catalogs;
using Aaron.Admin.Models.Settings;
using Aaron.Core.Services.Configuration;
using Aaron.Core.Web.Security;
using Aaron.Core.Services.Security;
using Aaron.Core.Web;
using Aaron.Core.Web.Themes;
using Aaron.Core.Domain.Common;

namespace Aaron.Admin.Controllers
{
    [AdminAuthorize]
    public class SettingsController : BaseController
    {
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;
        private readonly CatalogSettings _catalogSettings;
        private readonly SeoSettings _seoSettings;
        private readonly WebInformationSettings _webInformationSettings;
        private readonly AnalysisNSocialNetworkSettings _analysisNSocialNetworkSettings;
        private readonly IThemeProvider _themeProvider;

        public SettingsController(ISettingService settingService,
            IPermissionService permissionService,
            IThemeProvider themeProvider,
            CatalogSettings catalogSettings,
            WebInformationSettings webInformationSettings,
            SeoSettings seoSettings,
            AnalysisNSocialNetworkSettings analysisNSocialNetworkSettings)
        {
            _permissionService = permissionService;
            _settingService = settingService;
            _themeProvider = themeProvider;
            _catalogSettings = catalogSettings;
            _webInformationSettings = webInformationSettings;
            _analysisNSocialNetworkSettings = analysisNSocialNetworkSettings;
            _seoSettings = seoSettings;
        }
        //
        // GET: /Settings/

        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            return View();
        }

        #region Catalog Settings
        public ActionResult Catalog()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = new CatalogSettingsModel
            {
                AllowAutoUpload = _catalogSettings.AllowAutoUpload,
                AvatarDimension = _catalogSettings.AvatarDimension,
                AllowUploadMultiFile = _catalogSettings.AllowUploadMultiFile,
                DefaultAvatar = _catalogSettings.DefaultAvatar,
                DefaultCatalogItemsOnPage = _catalogSettings.DefaultCatalogItemsOnPage,
                DefaultGenericCatalogIcon = _catalogSettings.DefaultGenericCatalogIcon,
                DefaultGenericCatalogItemsOnPage = _catalogSettings.DefaultGenericCatalogItemsOnPage,
                DefaultSizeOnMenu = _catalogSettings.DefaultSizeOnMenu,
                GenericCatalogIconPath = _catalogSettings.GenericCatalogIconPath,
                IconDimension = _catalogSettings.IconDimension,
                ShowShareButton = _catalogSettings.ShowShareButton
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Catalog(CatalogSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var entity = new CatalogSettings
            {
                AllowAutoUpload = model.AllowAutoUpload,
                AvatarDimension = model.AvatarDimension,
                AllowUploadMultiFile = model.AllowUploadMultiFile,
                DefaultAvatar = model.DefaultAvatar,
                DefaultCatalogItemsOnPage = model.DefaultCatalogItemsOnPage,
                DefaultGenericCatalogIcon = model.DefaultGenericCatalogIcon,
                DefaultGenericCatalogItemsOnPage = model.DefaultGenericCatalogItemsOnPage,
                DefaultSizeOnMenu = model.DefaultSizeOnMenu,
                GenericCatalogIconPath = model.GenericCatalogIconPath,
                IconDimension = model.IconDimension,
                ShowShareButton = model.ShowShareButton
            };

            _settingService.SaveSetting(entity);

            return View(model);
        } 
        #endregion

        #region Common Settings
        public ActionResult Common()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = new WebInformationSettingsModel
            {
                WebName = _webInformationSettings.WebName,
                WebUrl = _webInformationSettings.WebUrl,
                WebClosed = _webInformationSettings.WebClosed,
                WebClosedAllowForAdmins = _webInformationSettings.WebClosedAllowForAdmins,
                DefaultWebThemeForDesktops = _webInformationSettings.DefaultWebThemeForDesktops,
                AllowAccountToSelectTheme = _webInformationSettings.AllowAccountToSelectTheme,
            };

            model.AvailableWebThemeForDesktops = _themeProvider
                .GetThemeConfigurations()
                .Where(x => !x.MobileTheme)
                .Select(x =>
                {
                    return new SelectListItem
                    {
                        Text = x.ThemeTitle,
                        Value = x.ThemeName,
                        Selected = x.ThemeName.Equals(_webInformationSettings.DefaultWebThemeForDesktops, StringComparison.InvariantCultureIgnoreCase)
                    };
                }).ToList();

            return View(model);   
        }

        [HttpPost]
        public ActionResult Common(WebInformationSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var entity = new WebInformationSettings
            {
                WebName = model.WebName,
                WebUrl = model.WebUrl,
                WebClosed = model.WebClosed,
                WebClosedAllowForAdmins = model.WebClosedAllowForAdmins,
                DefaultWebThemeForDesktops = model.DefaultWebThemeForDesktops,
                AllowAccountToSelectTheme = model.AllowAccountToSelectTheme,
            };

            if (string.IsNullOrEmpty(entity.WebUrl))
                entity.WebUrl = "http://www.cdnvn.com";

            if (!entity.WebUrl.EndsWith("/"))
                entity.WebUrl += "/";

            _settingService.SaveSetting(entity);

            //model.WebUrl = entity.WebUrl;
            //model.AvailableWebThemeForDesktops = _themeProvider
            //    .GetThemeConfigurations()
            //    .Where(x => !x.MobileTheme)
            //    .Select(x =>
            //    {
            //        return new SelectListItem
            //        {
            //            Text = x.ThemeTitle,
            //            Value = x.ThemeName,
            //            Selected = x.ThemeName.Equals(_webInformationSettings.DefaultWebThemeForDesktops, StringComparison.InvariantCultureIgnoreCase)
            //        };
            //    }).ToList();

            return RedirectToAction("Common");
        }
        #endregion

        #region Seo Settings
        public ActionResult Seo()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = new SeoSettingsModel
            {
                CanonicalUrlsEnabled = _seoSettings.CanonicalUrlsEnabled,
                DefaultMetaDescription = _seoSettings.DefaultMetaDescription,
                DefaultMetaKeywords = _seoSettings.DefaultMetaKeywords,
                DefaultTitle = _seoSettings.DefaultTitle,
                PageTitleSeparator = _seoSettings.PageTitleSeparator
            };

            model.PageTitleSeoAdjustmentValues = _seoSettings.PageTitleSeoAdjustment
                .ToEnumList()
                .Select(x => 
                {
                    return new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Key,
                        Selected = ((int)x.Key.GetEnumValue<PageTitleSeoAdjustment>()).Equals((int)_seoSettings.PageTitleSeoAdjustment)
                    };
                }).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult Seo(SeoSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var entity = new SeoSettings
            {
                CanonicalUrlsEnabled = model.CanonicalUrlsEnabled,
                DefaultMetaDescription = model.DefaultMetaDescription,
                DefaultMetaKeywords = model.DefaultMetaKeywords,
                DefaultTitle = model.DefaultTitle,
                PageTitleSeoAdjustment = model.PageTitleSeoAdjustment,
                PageTitleSeparator = model.PageTitleSeparator
            };

            _settingService.SaveSetting(entity);

            return RedirectToAction("Seo");
        }
        #endregion

        #region AnalysisNSocialNetwork
        public ActionResult AnalysisNSocialNetwork()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = new AnalysisNSocialNetworkSettingsModel
            {
                FacebookID = _analysisNSocialNetworkSettings.FacebookID ?? "000000000000000",
                GoogleAnalysis = _analysisNSocialNetworkSettings.GoogleAnalysis ?? "UA-00000000-0",
                FacebookInitCode = _analysisNSocialNetworkSettings.FacebookInitCode,
                GoogleInitCode = _analysisNSocialNetworkSettings.GoogleInitCode
            };

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AnalysisNSocialNetwork(AnalysisNSocialNetworkSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var entity = new AnalysisNSocialNetworkSettings
            {
                FacebookID = model.FacebookID,
                FacebookInitCode = model.FacebookInitCode,
                GoogleAnalysis = model.GoogleAnalysis,
                GoogleInitCode = model.GoogleInitCode
            };

            _settingService.SaveSetting(entity);

            return RedirectToAction("AnalysisNSocialNetwork");
        }
        #endregion
    }
}
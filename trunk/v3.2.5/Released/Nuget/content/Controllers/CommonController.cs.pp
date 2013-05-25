using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Aaron.Core.Web;
using Aaron.Core.Services.Common;
using Aaron.Core.Domain.Localization;
using Aaron.Core.Web.Localization;
using Aaron.Core.Services.Localization;
using Aaron.Core;
using Aaron.Core.Caching;
using $rootnamespace$.Models.Common;

namespace $rootnamespace$.Controllers
{
    public class CommonController : BaseController
    {
        public const string AVAILABLE_LANGUAGES_MODEL_KEY = "Aaron.pres.languages.all";

        private readonly IWebHelper _webHelper;
        private readonly INoticeService _noticeService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ILanguageService _languageService;
        private readonly ICurrentActivity _currentActivity;
        private readonly ICacheManager _cacheManager;

        public CommonController(IWebHelper webHelper,
            INoticeService noticeService,
            LocalizationSettings localizationSettings,
            ILanguageService languageService,
            ICurrentActivity currentActivity,
            ICacheManager cacheManager)
        {
            _webHelper = webHelper;
            _noticeService = noticeService;
            _localizationSettings = localizationSettings;
            _languageService = languageService;
            _currentActivity = currentActivity;
            _cacheManager = cacheManager;
        }

        [NonAction]
        protected LanguageSelectorModel PrepareLanguageSelectorModel()
        {
            var availableLanguages = _cacheManager.Get(AVAILABLE_LANGUAGES_MODEL_KEY, () =>
            {
                var result = _languageService
                    .GetAllLanguages()
                    .Select(x => new LanguageModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        FlagImageFileName = x.FlagImageFileName,
                    })
                    .ToList();
                return result;
            });

            var model = new LanguageSelectorModel()
            {
                CurrentLanguageId = _currentActivity.CurrentLanguage.Id,
                AvailableLanguages = availableLanguages,
                UseImages = _localizationSettings.UseImagesForLanguageSelection
            };
            return model;
        }

        public ActionResult BackHome()
        {
            return RedirectToRoute("HomePage");
        }

        [ChildActionOnly]
        public ActionResult Favicon()
        {
            var model = new FaviconModel()
            {
                Uploaded = System.IO.File.Exists(System.IO.Path.Combine(Request.PhysicalApplicationPath, "favicon.ico")),
                FaviconUrl = _webHelper.GetWebLocation() + "favicon.ico"
            };

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult NoticeBoard()
        {
            var list = _noticeService.GetPublishedNotice()
                .Select(n => new NoticeModel
                { 
                    Id = n.Id,
                    Name = n.Name,
                    Content = n.Content
                });
            return PartialView(list);
        }

        public ActionResult RobotsTextFile()
        {
            var disallowPaths = new List<string>()
                                    {
                                        "/app_data/",
                                        "/app_globalresources/",
                                        "/app_start/",
                                        "/bin/",
                                        "/content/",
                                        "/content/files/",
                                        "/install",
                                    };
            var localizableDisallowPaths = new List<string>()
                                               {
                                                   "/boards/postedit",
                                                   "/boards/postdelete",
                                                   "/boards/postcreate",
                                                   "/boards/topicedit",
                                                   "/boards/topicdelete",
                                                   "/boards/topiccreate",
                                                   "/boards/topicmove",
                                                   "/boards/topicwatch",
                                                   "/account/avatar",
                                                   "/account/activation",
                                                   "/account/addresses",
                                                   "/account/changepassword",
                                                   "/account/checkusernameavailability",
                                                   "/account/info",
                                                   "/newsletter/subscriptionactivation",
                                                   "/passwordrecovery/confirm",
                                                   "/subscribenewsletter",
                                                   "/topic/authenticate",
                                               };


            const string newLine = "\r\n"; //Environment.NewLine
            var sb = new StringBuilder();
            sb.Append("User-agent: *");
            sb.Append(newLine);
            //usual paths
            foreach (var path in disallowPaths)
            {
                sb.AppendFormat("Disallow: {0}", path);
                sb.Append(newLine);
            }
            //localizable paths (without SEO code)
            foreach (var path in localizableDisallowPaths)
            {
                sb.AppendFormat("Disallow: {0}", path);
                sb.Append(newLine);
            }
            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                //URLs are localizable. Append SEO code
                foreach (var language in _languageService.GetAllLanguages())
                {
                    foreach (var path in localizableDisallowPaths)
                    {
                        sb.AppendFormat("Disallow: {0}{1}", language.UniqueSeoCode, path);
                        sb.Append(newLine);
                    }
                }
            }

            Response.ContentType = "text/plain";
            Response.Write(sb.ToString());
            return null;
        }

        //language
        [ChildActionOnly]
        public ActionResult LanguageSelector()
        {
            var model = PrepareLanguageSelectorModel();
            return PartialView(model);
        }

        public ActionResult SetLanguage(int langid, string returnUrl = "")
        {
            var language = _languageService.GetLanguageById(langid);
            if (language != null && language.Published)
            {
                _currentActivity.CurrentLanguage = language;
            }

            //url referrer
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = _webHelper.GetUrlReferrer();
            //home page
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");
            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                string applicationPath = HttpContext.Request.ApplicationPath;
                if (returnUrl.IsLocalizedUrl(applicationPath, true))
                {
                    //already localized URL
                    returnUrl = returnUrl.RemoveLanguageSeoCodeFromRawUrl(applicationPath);
                }
                returnUrl = returnUrl.AddLanguageSeoCodeToRawUrl(applicationPath, _currentActivity.CurrentLanguage);
            }
            return Redirect(returnUrl);
        }

        public ActionResult GenericUrl()
        {
            //seems that no entity was found
            return RedirectToRoute("HomePage");
        }

        [HttpPost]
        public ActionResult CloseNoticeBoard()
        {
            _noticeService.HideNotice();
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aaron.Web.Models.Common;
using Aaron.Core.Web;
using Aaron.Core.Services.Common;
using System.Text;
using Aaron.Core.Domain.Localization;
using Aaron.Core.Services.Localization;

namespace Aaron.Web.Controllers
{
    public class CommonController : BaseController
    {
        private readonly IWebHelper _webHelper;
        private readonly INoticeService _noticeService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ILanguageService _languageService;

        public CommonController(IWebHelper webHelper,
            INoticeService noticeService,
            LocalizationSettings localizationSettings,
            ILanguageService languageService)
        {
            _webHelper = webHelper;
            _noticeService = noticeService;
            _localizationSettings = localizationSettings;
            _languageService = languageService;
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

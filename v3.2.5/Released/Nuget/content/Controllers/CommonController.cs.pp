using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aaron.Core.Web;
using Aaron.Core.Services.Common;
using $rootnamespace$.Models.Common;

namespace $rootnamespace$.Controllers
{
    public class CommonController : BaseController
    {
        private readonly IWebHelper _webHelper;
        private readonly INoticeService _noticeService;

        public CommonController(IWebHelper webHelper,
            INoticeService noticeService)
        {
            _webHelper = webHelper;
            _noticeService = noticeService;
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

        [HttpPost]
        public ActionResult CloseNoticeBoard()
        {
            _noticeService.HideNotice();
            return null;
        }
    }
}

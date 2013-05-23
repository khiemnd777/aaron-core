using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using Aaron.Core.Domain.Security;
using Aaron.Core.Services.Security;
using Aaron.Admin.Models.Common;
using Aaron.Core.Services.Common;
using Aaron.Core.Domain.Common;

namespace Aaron.Admin.Controllers
{
    public class NoticeController : BaseController
    {
        private readonly INoticeService _noticeService;
        private readonly IPermissionService _permissionService;

        public NoticeController(INoticeService noticeService,
            IPermissionService permissionService)
        {
            _noticeService = noticeService;
            _permissionService = permissionService;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List(string id)
        {
            if(!_permissionService.Authorize(StandardPermissionProvider.ManageNotices))
                return AccessDeniedView();

            var noticeList = _noticeService.GetAllNotice();
            if (!string.IsNullOrEmpty(id))
            {
                foreach (var l in noticeList)
                {
                    if (l.Id.Equals(Int32.Parse(id)))
                    {
                        l.Published = true;
                    }
                    else
                        l.Published = false;
                    _noticeService.UpdateNotice(l);
                }
            }
            var gridModel = new GridModel<NoticeModel>
            {
                Data = noticeList.Select(x =>
                {
                    var m = new NoticeModel();
                    m.Id = x.Id;
                    m.Name = x.Name;
                    m.CreationDate = x.CreationDate.HasValue ? x.CreationDate.Value : DateTime.Now;
                    m.Published = x.Published;

                    return m;
                })
            };

            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNotices))
                return AccessDeniedView();

            var noticeList = _noticeService.GetAllNotice();

            var gridModel = new GridModel<NoticeModel>
            {
                Data = noticeList.Select(x =>
                {
                    var m = new NoticeModel();
                    m.Id = x.Id;
                    m.Name = x.Name;
                    m.CreationDate = (x.CreationDate.HasValue) ? x.CreationDate.Value : DateTime.Now;
                    m.Published = x.Published;

                    return m;
                })
            };

            return new JsonResult { Data = gridModel };

        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveBatchEditing([Bind(Prefix = "inserted")]IEnumerable<NoticeModel> insertedNotices,
            [Bind(Prefix = "updated")]IEnumerable<NoticeModel> updatedNotices,
            [Bind(Prefix = "deleted")]IEnumerable<NoticeModel> deletedNotices)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNotices))
                return AccessDeniedView();

            if (insertedNotices != null)
            {
                foreach (var model in insertedNotices)
                {
                    var notice = new Notice();

                    notice.Name = model.Name;
                    notice.Published = model.Published;


                    _noticeService.InsertNotice(notice);
                }
            }
            if (updatedNotices != null)
            {
                foreach (var model in updatedNotices)
                {
                    var notice = _noticeService.GetNoticeById(model.Id);
                    if (notice != null)
                    {
                        notice.Name = model.Name;
                        notice.Published = model.Published;

                        _noticeService.UpdateNotice(notice);
                    }
                }
            }
            if (deletedNotices != null)
            {
                foreach (var model in deletedNotices)
                {
                    var notice = _noticeService.GetNoticeById(model.Id);
                    _noticeService.DeleteNotice(notice);
                }
            }


            var noticeList = _noticeService.GetAllNotice();

            var gridModel = new GridModel<NoticeModel>
            {
                Data = noticeList.Select(x =>
                {
                    var m = new NoticeModel();
                    m.Id = x.Id;
                    m.Name = x.Name;
                    m.CreationDate = x.CreationDate.HasValue ? x.CreationDate.Value : DateTime.Now;
                    m.Published = x.Published;

                    return m;
                })
            };

            return View(gridModel);
        }

        public ActionResult EditNotice(int noticeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNotices))
                return AccessDeniedView();

            if (noticeId == 0)
                return Content("Thông Báo không tồn tại!");

            var notice = _noticeService.GetNoticeById(noticeId);
            var noticeModel = new NoticeModel()
            {
                Id = notice.Id,
                Name = notice.Name,
                Content = notice.Content,
                CreationDate = notice.CreationDate.HasValue ? notice.CreationDate.Value : DateTime.Now
            };

            return View(noticeModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditNotice(NoticeModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNotices))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return View(model);

            var notice = _noticeService.GetNoticeById(model.Id);
            if (notice != null)
            {
                notice.Name = model.Name;
                notice.Content = model.Content;

                _noticeService.UpdateNotice(notice);

                return RedirectToAction("List");

            }
            else
                return View(model);
        }
    }
}

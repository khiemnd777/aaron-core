using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Aaron.Admin.Models.Logging;
using Aaron.Core;
using Aaron.Core.Domain.Logging;
using Aaron.Core.Services.Helpers;
using Aaron.Core.Services.Localization;
using Aaron.Core.Services.Logging;
using Aaron.Core.Services.Security;
using Aaron.Core.Web;
using Aaron.Core.Web.Controllers;
using Aaron.Core.Web.Security;
using Telerik.Web.Mvc;

namespace Aaron.Admin.Controllers
{
    [AdminAuthorize]
    public partial class LogController : BaseController
    {
        private readonly ILogger _logger;
        private readonly ICurrentActivity _currentActivity;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPermissionService _permissionService;

        public LogController(ILogger logger, ICurrentActivity currentActivity,
            ILocalizationService localizationService, IDateTimeHelper dateTimeHelper,
            IPermissionService permissionService)
        {
            this._logger = logger;
            this._currentActivity = currentActivity;
            this._localizationService = localizationService;
            this._dateTimeHelper = dateTimeHelper;
            this._permissionService = permissionService;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            var model = new LogListModel();
            model.AvailableLogLevels = LogLevel.Debug.ToSelectList(false).ToList();
            model.AvailableLogLevels.Insert(0, new SelectListItem() { Text = "Tất cả", Value = "0" });

            return View(model);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult LogList(GridCommand command, LogListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            DateTime? createdOnFromValue = (model.CreatedOnFrom == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnFrom.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? createdToFromValue = (model.CreatedOnTo == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnTo.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            LogLevel? logLevel = model.LogLevelId > 0 ? (LogLevel?)(model.LogLevelId) : null;


            var logItems = _logger.GetAllLogs(createdOnFromValue, createdToFromValue, model.Message,
                logLevel, command.Page - 1, command.PageSize);
            var gridModel = new GridModel<LogModel>
            {
                Data = logItems.Select(x =>
                {
                    return new LogModel()
                    {
                        Id = x.Id,
                        LogLevel = x.LogLevel.GetLocalizedEnum(_localizationService, _currentActivity),
                        ShortMessage = x.ShortMessage,
                        FullMessage = x.FullMessage,
                        IpAddress = x.IpAddress,
                        AccountId = x.AccountId,
                        AccountEmail = x.Account != null ? x.Account.Email : null,
                        PageUrl = x.PageUrl,
                        ReferrerUrl = x.ReferrerUrl,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc)
                    };
                }),
                Total = logItems.TotalCount
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }
        
        [HttpPost, ActionName("List")]
        [FormValueRequired("clearall")]
        public ActionResult ClearAll()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            _logger.ClearLog();

            SuccessNotification(_localizationService.GetResource("Admin.System.Log.Cleared"));
            return RedirectToAction("List");
        }

        public ActionResult View(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            var log = _logger.GetLogById(id);
            if (log == null)
                //No log found with the specified id
                return RedirectToAction("List");

            var model = new LogModel()
            {
                Id = log.Id,
                LogLevel = log.LogLevel.GetLocalizedEnum(_localizationService, _currentActivity),
                ShortMessage = log.ShortMessage,
                FullMessage = log.FullMessage,
                IpAddress = log.IpAddress,
                AccountId = log.AccountId,
                AccountEmail = log.Account != null ? log.Account.Email : null,
                PageUrl = log.PageUrl,
                ReferrerUrl = log.ReferrerUrl,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(log.CreatedOnUtc, DateTimeKind.Utc)
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            var log = _logger.GetLogById(id);
            if (log == null)
                //No log found with the specified id
                return RedirectToAction("List");

            _logger.DeleteLog(log);


            SuccessNotification(_localizationService.GetResource("Admin.System.Log.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var logItems = _logger.GetLogByIds(selectedIds.ToArray());
                foreach (var logItem in logItems)
                    _logger.DeleteLog(logItem);
            }

            return Json(new { Result = true});
        }
    }
}

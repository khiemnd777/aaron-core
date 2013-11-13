using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aaron.Core;
using Aaron.Core.Services.Accounts;
using Aaron.Core.Services.Security;
using Aaron.Core.Services.Localization;
using Aaron.Core.Web.Security;
using Aaron.Admin.Models.Accounts;
using Aaron.Admin.Models.Security;

namespace Aaron.Admin.Controllers
{
    [AdminAuthorize]
    public class SecurityController : BaseController
    {
        private readonly IPermissionService _permissionService;
        private readonly IAccountService _accountService;
        private readonly ICurrentActivity _currentActivity;
        private readonly ILocalizationService _localizationService;

        public SecurityController(IPermissionService permissionService,
            IAccountService accountService,
            ICurrentActivity currentActivity,
            ILocalizationService localizationService)
        {
            _permissionService = permissionService;
            _accountService = accountService;
            _currentActivity = currentActivity;
            _localizationService = localizationService;
        }

        public ActionResult AccessDenied(string pageUrl)
        {
            var currentAccount = _currentActivity.CurrentAccount;
            if (currentAccount == null || currentAccount.IsGuest())
            {
                ViewBag.ErrorMessage = string.Format("Truy xuất bị chặn vì yêu cầu không rõ ràng trên trang \"{0}\"", pageUrl);
                return View();
            }

            ViewBag.ErrorMessage = string.Format("Truy xuất bị chặn với #{0} trên trang {1}", currentAccount.Email, pageUrl);

            return View();
        }

        public ActionResult Permissions()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageACL))
                return AccessDeniedView();

            var model = new PermissionMappingModel();

            var permissionRecords = _permissionService.GetAllPermissionRecords();
            var accountRoles = _accountService.GetAllAccountRoles(true);
            foreach (var pr in permissionRecords)
            {
                model.AvailablePermissions.Add(new PermissionRecordModel()
                {
                    Name = _localizationService.GetResource(pr.Name),
                    SystemName = pr.SystemName
                });
            }
            foreach (var cr in accountRoles)
            {
                model.AvailableAccountRoles.Add(new AccountRoleModel()
                {
                    Id = cr.Id,
                    Name = cr.Name
                });
            }
            foreach (var pr in permissionRecords)
                foreach (var cr in accountRoles)
                {
                    bool allowed = pr.AccountRoles.Where(x => x.Id == cr.Id).ToList().Count() > 0;
                    if (!model.Allowed.ContainsKey(pr.SystemName))
                        model.Allowed[pr.SystemName] = new Dictionary<int, bool>();
                    model.Allowed[pr.SystemName][cr.Id] = allowed;
                }

            return View(model);
        }

        [HttpPost, ActionName("Permissions")]
        public ActionResult PermissionsSave(FormCollection form)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageAcl))
            //    return AccessDeniedView();

            var permissionRecords = _permissionService.GetAllPermissionRecords();
            var accountRoles = _accountService.GetAllAccountRoles(true);


            foreach (var cr in accountRoles)
            {
                string formKey = "allow_" + cr.Id;
                var permissionRecordSystemNamesToRestrict = form[formKey] != null ? form[formKey].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();

                foreach (var pr in permissionRecords)
                {
                    bool allow = permissionRecordSystemNamesToRestrict.Contains(pr.SystemName);
                    if (allow)
                    {
                        if (pr.AccountRoles.Where(x => x.Id == cr.Id).FirstOrDefault() == null)
                        {
                            pr.AccountRoles.Add(cr);
                            _permissionService.UpdatePermissionRecord(pr);
                        }
                    }
                    else
                    {
                        if (pr.AccountRoles.Where(x => x.Id == cr.Id).FirstOrDefault() != null)
                        {
                            pr.AccountRoles.Remove(cr);
                            _permissionService.UpdatePermissionRecord(pr);
                        }
                    }
                }
            }

            return RedirectToAction("Permissions");
        }
    }
}

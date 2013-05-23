using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using Aaron.Core;
using Aaron.Core.Services.Accounts;
using Aaron.Core.Services.Security;
using Aaron.Core.Domain.Accounts;
using Aaron.Admin.Models.Accounts;
using Aaron.Core.Web.Security;

namespace Aaron.Admin.Controllers
{
    [AdminAuthorize]
    public class AccountRoleController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IPermissionService _permissionService;

        public AccountRoleController(IAccountService accountService,
            IPermissionService permissionService)
        {
            _accountService = accountService;
            _permissionService = permissionService;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccountRoles))
                return AccessDeniedView();

            var list = _accountService.GetAllAccountRoles(true);

            var gridModel = new GridModel<AccountRoleModel>
            {
                Data = list.Select(entity =>
                {
                    var m = new AccountRoleModel
                    {
                        Id = entity.Id,
                        IsSystemRole = entity.IsSystemRole,
                        Active = entity.Active,
                        Name = entity.Name,
                        SystemName = entity.SystemName
                    };

                    return m;
                }),
                Total = list.Count
            };
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccountRoles))
                return AccessDeniedView();

            var defaultRoleIds = new[] { _accountService.GetAccountRoleBySystemName(SystemAccountRoleNames.Registered).Id };
            var list = _accountService.GetAllAccountRoles(true);

            var gridModel = new GridModel<AccountRoleModel>
            {
                Data = list.Select(entity => new AccountRoleModel 
                { 
                    Id = entity.Id,
                    Active = entity.Active,
                    IsSystemRole = entity.IsSystemRole,
                    Name = entity.Name,
                    SystemName = entity.SystemName
                }),
                Total = list.Count
            };

            return new JsonResult
            {
                Data = gridModel
            };
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult AccountRoleAdd(AccountRoleModel model, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccountRoles))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            if (ModelState.IsValid)
            {
                var entity = new AccountRole
                {
                    Name = model.Name,
                    SystemName = model.Name.ToSystemName(),
                    Active = model.Active,
                    IsSystemRole = false
                };
                _accountService.InsertAccountRole(entity);
            }

            return List(command);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult AccountRoleUpdate(AccountRoleModel model, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccountRoles))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var accountRole = _accountService.GetAccountRoleById(model.Id);
            if (accountRole == null)
                throw new ArgumentException("Không tìm thấy Vai trò nào", "id");

            accountRole.Name = model.Name;
            accountRole.SystemName = model.Name.ToSystemName(); ;
            accountRole.Active = model.Active;

            _accountService.UpdateAccountRole(accountRole);

            return List(command);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult AccountRoleDelete(int id, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccountRoles))
                return AccessDeniedView();

            var accountRole = _accountService.GetAccountRoleById(id);
            if (accountRole == null)
                throw new ArgumentException("Không tìm thấy Vai trò nào!", "id");

            _accountService.DeleteAccountRole(accountRole);

            return List(command);
        }

        //
        // GET: /AccountRole/

        //public ActionResult Index()
        //{
        //    //if (!_permissionService.Authorize(StandardPermissionProvider.DisplayAccountRoles))
        //    //    return RedirectToAction("Index", "Home");

        //    var list = _accountService.GetAllAccountRoles(true);
        //    return View(list);
        //}

        //public ActionResult Create()
        //{
        //    //if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccountRoles))
        //    //    return RedirectToAction("Index", "Home");

        //    var model = new AccountRoleModel();
        //    model.Active = true;
        //    model.IsSystemRole = false;
        //    return View(model);
        //}

        //[HttpPost]
        //public ActionResult Create(AccountRoleModel model)
        //{
        //    //if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccountRoles))
        //    //    return RedirectToAction("Index", "Home");

        //    if (ModelState.IsValid)
        //    {
        //        var entity = new AccountRole
        //        {
        //            Name = model.Name,
        //            SystemName = model.SystemName,
        //            Active = model.Active,
        //            IsSystemRole = model.IsSystemRole
        //        };

        //        _accountService.InsertAccountRole(entity);
        //    }
        //    return RedirectToAction("Index", _accountService.GetAllAccountRoles(true));
        //}

        //public ActionResult Delete(int id)
        //{
        //    //if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccountRoles))
        //    //    return RedirectToAction("Index", "Home");

        //    var role = _accountService.GetAccountRoleById(id);
        //    if (role != null)
        //        _accountService.DeleteAccountRole(role);
        //    return RedirectToAction("Index", _accountService.GetAllAccountRoles(true));
        //}
    }
}

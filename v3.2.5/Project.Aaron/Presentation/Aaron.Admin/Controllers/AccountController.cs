using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aaron.Core.Services.Accounts;
using Aaron.Core.Domain.Accounts;
using Aaron.Core.Services.Localization;
using Aaron.Core.Services.Security;
using Aaron.Core.Services.Authentication;
using Aaron.Core.Web.Controllers;
using Aaron.Core.Utility;
using Aaron.Core.Services.Helpers;
using Aaron.Core.Services.Directory;
using Aaron.Core.Web.Security;
using Aaron.Admin.Models.Accounts;
using Telerik.Web.Mvc;

namespace Aaron.Admin.Controllers
{
    [AdminAuthorize]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IAccountRegistrationService _accountRegistrationService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IAuthenticationService _authenticationService;
        private readonly AccountSettings _accountSettings;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IGeoCountryLookup _geoCountryLookup;

        public AccountController(IAccountService accountService,
            IAccountRegistrationService accountRegistrationService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IAuthenticationService authenticationService,
            AccountSettings accountSettings,
            IDateTimeHelper dateTimeHelper,
            IGeoCountryLookup geoCountryLookup)
        {
            _accountService = accountService;
            _accountRegistrationService = accountRegistrationService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _authenticationService = authenticationService;
            _accountSettings = accountSettings;
            _dateTimeHelper = dateTimeHelper;
            _geoCountryLookup = geoCountryLookup;
        }

        [NonAction]
        protected AccountListModel PrepareAccountModelForList(Account account)
        {
            return new AccountListModel()
            {
                Id = account.Id,
                Email = !String.IsNullOrEmpty(account.Email) ? account.Email : (account.IsGuest() ? "Khách" : "Unknown"),
                AccountRoleNames = GetAccountRolesNames(account.AccountRoles.ToList()),
                Active = account.Active,
                CreationDate = account.CreatedOnUtc,
                LastestUpdatedDate = account.LastActivityDateUtc
            };
        }

        [NonAction]
        protected string GetAccountRolesNames(IList<AccountRole> accountRoles, string separator = ",")
        {
            var sb = new StringBuilder();
            for (int i = 0; i < accountRoles.Count; i++)
            {
                sb.Append(accountRoles[i].Name);
                if (i != accountRoles.Count - 1)
                {
                    sb.Append(separator);
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }

        //
        // GET: /Account/

        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.DisplayAccounts))
                return AccessDeniedView();

            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.DisplayAccounts))
                return AccessDeniedView();

            var defaultRoleIds = new[] { _accountService.GetAccountRoleBySystemName(SystemAccountRoleNames.Registered).Id };

            var list = _accountService.GetAllAccounts(null, null, defaultRoleIds, null,
                null, null, null, 0, 0);
            
            var listModel = list.Select(PrepareAccountModelForList);
            return View(listModel);
        }

        [HttpPost, GridAction]
        public ActionResult List(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.DisplayAccounts))
                return AccessDeniedView();

            var defaultRoleIds = new[] { _accountService.GetAccountRoleBySystemName(SystemAccountRoleNames.Registered).Id };
            var list = _accountService.GetAllAccounts(null, null, defaultRoleIds, null,
                null, null, null, 0, 0);

            var gridModel = new GridModel<AccountListModel>
            {
                Data = list.Select(PrepareAccountModelForList)
            };  

            return new JsonResult
            {
                Data = gridModel
            };
        }

        public ActionResult Edit(int id)
        {
            var account = _accountService.GetAccountById(id);
            if (account == null || account.Deleted)
                return RedirectToAction("List");

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccounts) &&
                (!_permissionService.Authorize(StandardPermissionProvider.EditAccounts) || 
                !_authenticationService.GetAuthenticatedAccount().Equals(account)))
                return AccessDeniedView();

            var model = new AccountModel();
            model.Id = account.Id;
            model.Email = account.Email;
            model.Active = account.Active;
            model.AllowManagingAccountRoles = _permissionService.Authorize(StandardPermissionProvider.ManageAccountRoles);
            model.AvailableAccountRoles = _accountService
                .GetAllAccountRoles(true)
                .Select(entity => new AccountRoleModel()
                {
                    Id = entity.Id,
                    Active = entity.Active,
                    IsSystemRole = entity.IsSystemRole,
                    Name = entity.Name,
                    SystemName = entity.SystemName
                })
                .ToList();
            model.SelectedAccountRoleIds = account.AccountRoles.Select(ar => ar.Id).ToArray();

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult Edit(AccountModel model)
        {
            var account = _accountService.GetAccountById(model.Id);
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccounts) &&
                (!_permissionService.Authorize(StandardPermissionProvider.EditAccounts) ||
                !_authenticationService.GetAuthenticatedAccount().Equals(account)))
                return AccessDeniedView();

            var allAccountRoles = _accountService.GetAllAccountRoles(true);
            var newAccountRoles = new List<AccountRole>();
            foreach (var accountRole in allAccountRoles)
            {
                if (model.SelectedAccountRoleIds != null && model.SelectedAccountRoleIds.Contains(accountRole.Id))
                    newAccountRoles.Add(accountRole);
            }
            var allowManagingAccountRoles = _permissionService.Authorize(StandardPermissionProvider.ManageAccountRoles);
            if (ModelState.IsValid)
            {
                
                    account.Active = model.Active;
                    account.LastActivityDateUtc = DateTime.UtcNow;

                    if (allowManagingAccountRoles)
                    {
                        foreach (var accountRole in allAccountRoles)
                        {
                            if (model.SelectedAccountRoleIds != null && model.SelectedAccountRoleIds.Contains(accountRole.Id))
                            {
                                //new role
                                if (account.AccountRoles.Where(cr => cr.Id == accountRole.Id).Count() == 0)
                                    account.AccountRoles.Add(accountRole);
                            }
                            else
                            {
                                //removed role
                                if (account.AccountRoles.Where(cr => cr.Id == accountRole.Id).Count() > 0)
                                    account.AccountRoles.Remove(accountRole);
                            }
                        }
                        _accountService.UpdateAccount(account);
                    }
                return RedirectToAction("List");
            }
            model.AvailableAccountRoles = _accountService
               .GetAllAccountRoles(true)
               .Select(entity => new AccountRoleModel()
               {
                   Id = entity.Id,
                   Active = entity.Active,
                   IsSystemRole = entity.IsSystemRole,
                   Name = entity.Name,
                   SystemName = entity.SystemName
               })
               .ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccounts))
                return AccessDeniedView();

            var account = _accountService.GetAccountById(id);
            if (account == null)
                return RedirectToAction("List");

            try
            {
                _accountService.DeleteAccount(account);

                return RedirectToAction("List");
            }
            catch
            {
                return RedirectToAction("Edit", new { id = account.Id });
            }
        }

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccounts))
                return AccessDeniedView();

            var model = new AccountModel();
            model.AllowManagingAccountRoles = _permissionService.Authorize(StandardPermissionProvider.ManageAccountRoles);
            model.AvailableAccountRoles = _accountService
                .GetAllAccountRoles(true)
                .Select(entity => new AccountRoleModel()
                { 
                    Id = entity.Id,
                    Active = entity.Active,
                    IsSystemRole = entity.IsSystemRole,
                    Name = entity.Name,
                    SystemName = entity.SystemName
                })
                .ToList();
            model.Active = true;

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(AccountModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccounts))
                return AccessDeniedView();

            if (!String.IsNullOrWhiteSpace(model.Email))
            {
                var acc = _accountService.GetAccountByEmail(model.Email);
                if (acc != null)
                    ModelState.AddModelError("", "Email đã được đăng ký!");
            }
            else
                ModelState.AddModelError("", "Email không được bỏ trống!");

            if (!CommonHelper.IsValidEmail(model.Email))
            {
                ModelState.AddModelError("", "Email không hợp lệ!");
            }
            if(String.IsNullOrWhiteSpace(model.Password))
                ModelState.AddModelError("", "Mật khẩu không được bỏ trống!");

            var allAccountRoles = _accountService.GetAllAccountRoles(true);
            var newAccountRoles = new List<AccountRole>();
            foreach (var accountRole in allAccountRoles)
                if (model.SelectedAccountRoleIds != null && model.SelectedAccountRoleIds.Contains(accountRole.Id))
                    newAccountRoles.Add(accountRole);
            bool allowManagingAccountRole = _permissionService.Authorize(StandardPermissionProvider.ManageAccountRoles);

            if (ModelState.IsValid)
            {
                var account = new Account()
                {
                    AccountGuid = Guid.NewGuid(),
                    Email = model.Email,
                    Username = model.Email,
                    Active = model.Active,
                    CreatedOnUtc = DateTime.UtcNow,
                    LastActivityDateUtc = DateTime.UtcNow,
                };

                _accountService.InsertAccount(account);

                if (!String.IsNullOrWhiteSpace(model.Password))
                {
                    var changePasswordRequest = new ChangePasswordRequest(model.Email, false, PasswordFormat.Hashed, model.Password);
                    var changPasswordResult = _accountRegistrationService.ChangePassword(changePasswordRequest);

                }

                if (allowManagingAccountRole)
                {
                    foreach (var accountRole in newAccountRoles)
                        account.AccountRoles.Add(accountRole);
                    _accountService.UpdateAccount(account);
                }
                return RedirectToAction("List");
            }

            model.AvailableAccountRoles = _accountService
               .GetAllAccountRoles(true)
               .Select(entity => new AccountRoleModel()
               {
                   Id = entity.Id,
                   Active = entity.Active,
                   IsSystemRole = entity.IsSystemRole,
                   Name = entity.Name,
                   SystemName = entity.SystemName
               })
               .ToList();
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("changepassword")]
        public ActionResult ChangePassword(AccountModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccounts))
                return AccessDeniedView();

            if (string.IsNullOrWhiteSpace(model.ChangePassword))
            {
                ModelState.AddModelError("", "Mật khẩu không được để trống!");
            }

            var account = _accountService.GetAccountById(model.Id);
            if (account == null)
                
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var changePassRequest = new ChangePasswordRequest(model.Email,
                    false, _accountSettings.DefaultPasswordFormat, model.ChangePassword);
                var changePassResult = _accountRegistrationService.ChangePassword(changePassRequest);
                return RedirectToAction("Edit", account.Id);   
            }
            model.Email = account.Email;
            model.AvailableAccountRoles = _accountService
               .GetAllAccountRoles(true)
               .Select(entity => new AccountRoleModel()
               {
                   Id = entity.Id,
                   Active = entity.Active,
                   IsSystemRole = entity.IsSystemRole,
                   Name = entity.Name,
                   SystemName = entity.SystemName
               })
               .ToList();
            return View(model);
        }

        public ActionResult Online()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ViewOnlineAccounts))
                return AccessDeniedView();

            var accounts = _accountService.GetOnlineAccounts(DateTime.UtcNow.AddMinutes(-_accountSettings.OnlineMinutes),
                null, 0, 10);

            var model = new GridModel<OnlineAccountModel>
            {
                Data = accounts.Select(x =>
                {
                    return new OnlineAccountModel()
                    {
                        Id = x.Id,
                        AccountInfo = x.IsRegistered() ? x.Email : "Khách",
                        Location = _geoCountryLookup.LookupCountryName(x.LastIpAddress),
                        LastIpAddress = x.LastIpAddress,
                        LastActivityDate = _dateTimeHelper.ConvertToUserTime(x.LastActivityDateUtc, DateTimeKind.Utc),
                    };
                }),
                Total = accounts.TotalCount
            };
            return View(model);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult Online(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccounts))
                return AccessDeniedView();

            var accounts = _accountService.GetOnlineAccounts(DateTime.UtcNow.AddMinutes(-_accountSettings.OnlineMinutes),
                null, 0, 10);

            var model = new GridModel<OnlineAccountModel>
            {
                Data = accounts.Select(x =>
                {
                    return new OnlineAccountModel()
                    {
                        Id = x.Id,
                        AccountInfo = x.IsRegistered() ? x.Email : "Khách",
                        Location = _geoCountryLookup.LookupCountryName(x.LastIpAddress),
                        LastIpAddress = x.LastIpAddress,
                        LastActivityDate = _dateTimeHelper.ConvertToUserTime(x.LastActivityDateUtc, DateTimeKind.Utc),
                    };
                }),
                Total = accounts.TotalCount
            };
            return new JsonResult
            {
                Data = model
            };
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aaron.Core.Domain.Accounts;
using Aaron.Core.Services.Accounts;
using Aaron.Core.Services.Authentication;
using Aaron.Core;
using Aaron.Web.Models.Accounts;
using Aaron.Core.Utility;
using Aaron.Core.Services.Utilities;
using Aaron.Core.Services.Messages;
using Aaron.Core.Web.Controllers;
using Aaron.Core.Services.Localization;

namespace Aaron.Web.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountRegistrationService _accountRegistrationService;
        private readonly IAccountService _accountService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICurrentActivity _currentActivity;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly AccountSettings _accountSettings;
        private readonly ILocalizationService _localizationService;

        public AccountController(IAccountRegistrationService accountRegistrationService,
            IAccountService accountService,
            IAuthenticationService authenticationService,
            IGenericAttributeService genericAttributeService,
            ICurrentActivity currentActivity,
            IWorkflowMessageService workflowMessageService,
            AccountSettings accountSettings,
            ILocalizationService localizationService)
        {
            _accountRegistrationService = accountRegistrationService;
            _accountService = accountService;
            _authenticationService = authenticationService;
            _currentActivity = currentActivity;
            _genericAttributeService = genericAttributeService;
            _workflowMessageService = workflowMessageService;
            _accountSettings = accountSettings;
            _localizationService = localizationService;
        }

        [NonAction]
        public bool IsCurrentUserRegistered()
        {
            var account = _currentActivity.CurrentAccount;
            return account != null && account.IsRegistered();
        }

        #region Password Recovery
        public ActionResult PasswordRecovery()
        {
            var model = new PasswordRecoveryModel();
            if (IsCurrentUserRegistered())
                model.Email = _currentActivity.CurrentAccount.Email;
            return View(model);
        }

        [HttpPost]
        public ActionResult PasswordRecovery(PasswordRecoveryModel model)
        {
            if (!CommonHelper.IsValidEmail(model.Email))
                ModelState.AddModelError("", "Email không hợp lệ!");
            if (ModelState.IsValid)
            {
                var account = _accountService.GetAccountByEmail(model.Email);
                if (account != null && account.Active && !account.Deleted)
                {
                    var passwordRecoveryToken = Guid.NewGuid();
                    _genericAttributeService.SaveAttribute(account, SystemAttributeNames.PasswordRecoveryToken, passwordRecoveryToken.ToString());
                    _workflowMessageService.SendAccountPasswordRecoveryMessage(account, _currentActivity.CurrentLanguage.Id);

                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.EmailHasBeenSent"); //Hệ thống đã gửi một email chứa link phục hồi vào hồm thư của bạn. Xin kiểm tra hòm thư, và kích hoạt liên kết!
                }
                else
                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.EmailNotFound"); //Không tìm thấy tài khoản này, xin cung cấp một tài khoản email chính xác!
            }
            return View(model);
        }

        public ActionResult PasswordRecoveryConfirm(string token, string email)
        {
            var account = _accountService.GetAccountByEmail(email);
            if (account == null)
                return RedirectToRoute("HomePage");

            var cPrt = account.GetAttribute<string>(SystemAttributeNames.PasswordRecoveryToken);
            if (String.IsNullOrEmpty(cPrt))
                return RedirectToRoute("HomePage");

            if (!cPrt.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return RedirectToRoute("HomePage");

            var model = new PasswordRecoveryConfirmModel();
            return View(model);
        }

        [HttpPost, ActionName("PasswordRecoveryConfirm")]
        [FormValueRequired("set-password")]
        public ActionResult PasswordRecoveryConfirmPOST(string token, string email, PasswordRecoveryConfirmModel model)
        {
            var account = _accountService.GetAccountByEmail(email);
            if (account == null)
                return RedirectToRoute("HomePage");

            var cPrt = account.GetAttribute<string>(SystemAttributeNames.PasswordRecoveryToken);
            if (String.IsNullOrEmpty(cPrt))
                return RedirectToRoute("HomePage");

            if (!cPrt.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return RedirectToRoute("HomePage");

            if (ModelState.IsValid)
            {
                var response = _accountRegistrationService.ChangePassword(new ChangePasswordRequest(email,
                    false, _accountSettings.DefaultPasswordFormat, model.NewPassword));
                if (response.Success)
                {
                    _genericAttributeService.SaveAttribute(account, SystemAttributeNames.PasswordRecoveryToken, "");

                    model.SuccessfullyChanged = true;
                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.PasswordHasBeenChanged"); //Đã thay đổi mật khẩu thành công!
                }
                else
                {
                    model.Result = response.Errors.FirstOrDefault();
                }

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        } 
        #endregion

        #region Log-in/Log-out
        public ActionResult LogIn()
        {
            var account = _authenticationService.GetAuthenticatedAccount();
            if (account != null)
            {
                if (account.IsAdmin())
                    return Redirect("~/Admin");
                if (account.IsRegistered())
                    return RedirectToAction("Index", "Home");
            }
            var model = new LoginModel();
            model.RememberMe = true;
            return View(model);
        }

        [HttpPost]
        public ActionResult LogIn(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (_accountRegistrationService.ValidateAccount(model.Email, model.Password))
                {
                    var account = _accountService.GetAccountByEmail(model.Email);

                    //sign in new account
                    _authenticationService.SignIn(account, model.RememberMe);

                    if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                    else
                    {
                        if (account.IsAdmin())
                            return Redirect("~/Admin");
                        if (account.IsRegistered())
                            return RedirectToAction("Index", "Home");
                    }
                }
            }

            return View(model);
        }

        public ActionResult LogOut()
        {
            _authenticationService.SignOut();
            return RedirectToAction("Login", "Account");
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aaron.Core.Domain.Accounts;
using Aaron.Core.Services.Accounts;
using Aaron.Core.Services.Authentication;
using Aaron.Core;
using $rootnamespace$.Models.Accounts;

namespace $rootnamespace$.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountRegistrationService _accountRegistrationService;
        private readonly IAccountService _accountService;
        private readonly IAuthenticationService _authenticationService;

        public AccountController(IAccountRegistrationService accountRegistrationService,
            IAccountService accountService,
            IAuthenticationService authenticationService)
        {
            _accountRegistrationService = accountRegistrationService;
            _accountService = accountService;
            _authenticationService = authenticationService;
        }

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

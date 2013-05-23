using System;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using Aaron.Admin.Models.Messages;
using Aaron.Core;
using Aaron.Core.Domain;
using Aaron.Core.Domain.Messages;
using Aaron.Core.Services.Configuration;
using Aaron.Core.Services.Localization;
using Aaron.Core.Services.Messages;
using Aaron.Core.Services.Security;
using Aaron.Core.Web.Security;
using Aaron.Core.Web;
using Aaron.Core.Web.Controllers;

namespace Aaron.Admin.Controllers
{
	[AdminAuthorize]
	public partial class EmailAccountController : BaseController
	{
        private readonly IEmailAccountService _emailAccountService;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IEmailSender _emailSender;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly WebInformationSettings _webSettings;
        private readonly IPermissionService _permissionService;

		public EmailAccountController(IEmailAccountService emailAccountService,
            ILocalizationService localizationService, ISettingService settingService, 
            IEmailSender emailSender, 
            EmailAccountSettings emailAccountSettings, WebInformationSettings webSettings,
            IPermissionService permissionService)
		{
            this._emailAccountService = emailAccountService;
            this._localizationService = localizationService;
            this._emailAccountSettings = emailAccountSettings;
            this._emailSender = emailSender;
            this._settingService = settingService;
            this._webSettings = webSettings;
            this._permissionService = permissionService;
		}

        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            return RedirectToAction("List");
        }

		public ActionResult List(string id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

			//mark as default email account (if selected)
			if (!String.IsNullOrEmpty(id))
			{
				int defaultEmailAccountId = Convert.ToInt32(id);
				var defaultEmailAccount = _emailAccountService.GetEmailAccountById(defaultEmailAccountId);
				if (defaultEmailAccount != null)
				{
					_emailAccountSettings.DefaultEmailAccountId = defaultEmailAccountId;
					_settingService.SaveSetting(_emailAccountSettings);
				}
			}

            var emailAccountModels = _emailAccountService.GetAllEmailAccounts()
                                    .Select(x => new EmailAccountModel { 
                                        DisplayName = x.DisplayName,
                                        Email = x.Email,
                                        EnableSsl = x.EnableSsl,
                                        Host = x.Host,
                                        Id = x.Id,
                                        Password = x.Password,
                                        Port = x.Port,
                                        UseDefaultCredentials = x.UseDefaultCredentials,
                                        Username = x.Username
                                    })
                                    .ToList();
			foreach (var eam in emailAccountModels)
				eam.IsDefaultEmailAccount = eam.Id == _emailAccountSettings.DefaultEmailAccountId;

			var gridModel = new GridModel<EmailAccountModel>
			{
				Data = emailAccountModels,
				Total = emailAccountModels.Count()
			};
			return View(gridModel);
		}

		[HttpPost, GridAction(EnableCustomBinding = true)]
		public ActionResult List(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            var emailAccountModels = _emailAccountService.GetAllEmailAccounts()
                                    .Select(x => new EmailAccountModel {
                                        DisplayName = x.DisplayName,
                                        Email = x.Email,
                                        EnableSsl = x.EnableSsl,
                                        Host = x.Host,
                                        Id = x.Id,
                                        Password = x.Password,
                                        Port = x.Port,
                                        UseDefaultCredentials = x.UseDefaultCredentials,
                                        Username = x.Username
                                    })
                                    .ToList();
            foreach (var eam in emailAccountModels)
                eam.IsDefaultEmailAccount = eam.Id == _emailAccountSettings.DefaultEmailAccountId;

            var gridModel = new GridModel<EmailAccountModel>
            {
                Data = emailAccountModels,
                Total = emailAccountModels.Count()
            };

			return new JsonResult
			{
				Data = gridModel
			};
		}

		public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            var model = new EmailAccountModel();
            //default values
            model.Port = 25;
			return View(model);
		}

        [HttpPost]
		public ActionResult Create(EmailAccountModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var emailAccount = new EmailAccount() 
                {
                    DisplayName = model.DisplayName,
                    Email = model.Email,
                    EnableSsl = model.EnableSsl,
                    Host = model.Host,
                    Password = model.Password,
                    Port = model.Port,
                    UseDefaultCredentials = model.UseDefaultCredentials,
                    Username = model.Username
                };
                _emailAccountService.InsertEmailAccount(emailAccount);

                return RedirectToAction("List");
            }

            return View(model);
		}

		public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

			var emailAccount = _emailAccountService.GetEmailAccountById(id);
            if (emailAccount == null)
                //No email account found with the specified id
                return RedirectToAction("List");

            return View(new EmailAccountModel 
            {
                DisplayName = emailAccount.DisplayName,
                Email = emailAccount.Email,
                EnableSsl = emailAccount.EnableSsl,
                Host = emailAccount.Host,
                Id = emailAccount.Id,
                Password = emailAccount.Password,
                Port = emailAccount.Port,
                UseDefaultCredentials = emailAccount.UseDefaultCredentials,
                Username = emailAccount.Username
            });
		}

        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult Edit(EmailAccountModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            var emailAccount = _emailAccountService.GetEmailAccountById(model.Id);
            if (emailAccount == null)
                //No email account found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                emailAccount.DisplayName = model.DisplayName;
                emailAccount.Email = model.Email;
                emailAccount.EnableSsl = model.EnableSsl;
                emailAccount.Host = model.Host;
                emailAccount.Password = model.Password;
                emailAccount.Port = model.Port;
                emailAccount.UseDefaultCredentials = model.UseDefaultCredentials;
                emailAccount.Username = model.Username;
                _emailAccountService.UpdateEmailAccount(emailAccount);

                return RedirectToAction("Edit", new { id = emailAccount.Id });
            }

            //If we got this far, something failed, redisplay form
            return View(model);
		}

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("sendtestemail")]
        public ActionResult SendTestEmail(EmailAccountModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            var emailAccount = _emailAccountService.GetEmailAccountById(model.Id);
            if (emailAccount == null)
                //No email account found with the specified id
                return RedirectToAction("List");

            try
            {
                if (String.IsNullOrWhiteSpace(model.SendTestEmailTo))
                    throw new AaronException("Enter test email address");


                var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
                var to = new MailAddress(model.SendTestEmailTo);
                string subject = _webSettings.WebName + ". Kiểm tra tính năng gửi email.";
                string body = "Tính năng gửi Email hoạt động tốt!";
                _emailSender.SendEmail(emailAccount, subject, body, from, to);
            }
            catch
            {
                
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            var emailAccount = _emailAccountService.GetEmailAccountById(id);
            if (emailAccount == null)
                //No email account found with the specified id
                return RedirectToAction("List");

            _emailAccountService.DeleteEmailAccount(emailAccount);
            return RedirectToAction("List");
        }
	}
}

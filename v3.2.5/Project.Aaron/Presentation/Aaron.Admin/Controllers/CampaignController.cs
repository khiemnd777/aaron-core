using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using Aaron.Core;
using Aaron.Core.Domain.Accounts;
using Aaron.Core.Domain.Messages;
using Aaron.Core.Services.Accounts;
using Aaron.Core.Services.Localization;
using Aaron.Core.Services.Messages;
using Aaron.Core.Services.Security;
using Aaron.Core.Web.Security;
using Aaron.Admin.Models.Messages;
using Aaron.Core.Services.Helpers;
using Aaron.Core.Web.Controllers;

namespace Aaron.Admin.Controllers
{
	[AdminAuthorize]
	public partial class CampaignController : BaseController
	{
        private readonly ICampaignService _campaignService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IAccountService _accountService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly ILocalizationService _localizationService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly IPermissionService _permissionService;

        public CampaignController(ICampaignService campaignService,
            IDateTimeHelper dateTimeHelper, IEmailAccountService emailAccountService,
            EmailAccountSettings emailAccountSettings,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            ILocalizationService localizationService, IMessageTokenProvider messageTokenProvider,
            IPermissionService permissionService,
            IAccountService accountService)
		{
            _campaignService = campaignService;
            _dateTimeHelper = dateTimeHelper;
            _emailAccountService = emailAccountService;
            _emailAccountSettings = emailAccountSettings;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _localizationService = localizationService;
            _messageTokenProvider = messageTokenProvider;
            _permissionService = permissionService;
            _accountService = accountService;
		}

        private string FormatTokens(string[] tokens)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i];
                sb.Append(token);
                if (i != tokens.Length - 1)
                    sb.Append(", ");
            }

            return sb.ToString();
        }
        
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

		public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            var campaigns = _campaignService.GetAllCampaigns();
            var gridModel = new GridModel<CampaignModel>
            {
                Data = campaigns.Select(x =>
                {
                    var model = new CampaignModel
                    {
                        Body = x.Body,
                        CreatedOn = x.CreatedOnUtc,
                        Id = x.Id,
                        Name = x.Name,
                        Subject = x.Subject
                    };
                    model.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                    return model;
                }),
                Total = campaigns.Count
            };
            return View(gridModel);
		}

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            var campaigns = _campaignService.GetAllCampaigns();
            var gridModel = new GridModel<CampaignModel>
            {
                Data = campaigns.Select(x =>
                {
                    var model = new CampaignModel
                    {
                        Body = x.Body,
                        CreatedOn = x.CreatedOnUtc,
                        Id = x.Id,
                        Name = x.Name,
                        Subject = x.Subject
                    };
                    model.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                    return model;
                }),
                Total = campaigns.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            var model = new CampaignModel();
            model.AllowedTokens = FormatTokens(_messageTokenProvider.GetListOfCampaignAllowedTokens());
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(CampaignModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var campaign = new Campaign 
                {
                    Body = model.Body,
                    Name = model.Name,
                    Subject = model.Subject
                };
                campaign.CreatedOnUtc = DateTime.UtcNow;
                _campaignService.InsertCampaign(campaign);

                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            model.AllowedTokens = FormatTokens(_messageTokenProvider.GetListOfCampaignAllowedTokens());
            return View(model);
        }

		public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            var campaign = _campaignService.GetCampaignById(id);
            if (campaign == null)
                //No campaign found with the specified id
                return RedirectToAction("List");

            var model = new CampaignModel
            {
                Body = campaign.Body,
                CreatedOn = campaign.CreatedOnUtc,
                Id = campaign.Id,
                Name = campaign.Name,
                Subject = campaign.Subject
            };
            model.AllowedTokens = FormatTokens(_messageTokenProvider.GetListOfCampaignAllowedTokens());
            return View(model);
		}

        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult Edit(CampaignModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            var campaign = _campaignService.GetCampaignById(model.Id);
            if (campaign == null)
                //No campaign found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                campaign.Body = model.Body;
                campaign.Name = model.Name;
                campaign.Subject = model.Subject;

                _campaignService.UpdateCampaign(campaign);

                return RedirectToAction("Edit", new { id = campaign.Id });
            }

            //If we got this far, something failed, redisplay form
            model.AllowedTokens = FormatTokens(_messageTokenProvider.GetListOfCampaignAllowedTokens());
            return View(model);
		}

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("send-all-email")]
        public ActionResult SendAllEmail(CampaignModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            var campaign = _campaignService.GetCampaignById(model.Id);
            if (campaign == null)
                //No campaign found with the specified id
                return RedirectToAction("List");

            model.AllowedTokens = FormatTokens(_messageTokenProvider.GetListOfCampaignAllowedTokens());

            try
            {
                var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
                if (emailAccount == null)
                    throw new AaronException("Email account could not be loaded");

                var defaultRoleIds = new[] { _accountService.GetAccountRoleBySystemName(SystemAccountRoleNames.Registered).Id };

                var accounts = _accountService.GetAllAccounts(null, null, defaultRoleIds, null,
                    null, null, null, 0, 0, 0, int.MaxValue);
                var totalEmailsSent = _campaignService.SendCampaign(campaign, emailAccount, accounts);
                return View(model);
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            var campaign = _campaignService.GetCampaignById(id);
            if (campaign == null)
                //No campaign found with the specified id
                return RedirectToAction("List");

            _campaignService.DeleteCampaign(campaign);

			return RedirectToAction("List");
		}
	}
}

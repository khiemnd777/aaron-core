using System.Linq;
using System.Text;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using Aaron.Admin.Models.Messages;
using Aaron.Core.Web.Security;
using Aaron.Core.Services.Messages;
using Aaron.Core.Services.Localization;
using Aaron.Core.Services.Security;
using Aaron.Core.Domain.Messages;

namespace Aaron.Admin.Controllers
{
    [AdminAuthorize]
    public partial class MessageTemplateController : BaseController
    {
        #region Fields

        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly IPermissionService _permissionService;
        private readonly EmailAccountSettings _emailAccountSettings;
        #endregion Fields

        #region Constructors

        public MessageTemplateController(IMessageTemplateService messageTemplateService,
            IEmailAccountService emailAccountService, ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService, IMessageTokenProvider messageTokenProvider,
            IPermissionService permissionService, EmailAccountSettings emailAccountSettings)
        {
            this._messageTemplateService = messageTemplateService;
            this._emailAccountService = emailAccountService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._messageTokenProvider = messageTokenProvider;
            this._permissionService = permissionService;
            this._emailAccountSettings = emailAccountSettings;
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
        #endregion

        #region Utilities

        [NonAction]
        protected void UpdateLocales(MessageTemplate mt, MessageTemplateModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(mt,
                                                           x => x.BccEmailAddresses,
                                                           localized.BccEmailAddresses,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(mt,
                                                           x => x.Subject,
                                                           localized.Subject,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(mt,
                                                           x => x.Body,
                                                           localized.Body,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(mt,
                                                           x => x.EmailAccountId,
                                                           localized.EmailAccountId,
                                                           localized.LanguageId);
            }
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            var messageTemplates = _messageTemplateService.GetAllMessageTemplates();
            var gridModel = new GridModel<MessageTemplateModel>
            {
                Data = messageTemplates.Select(x => new MessageTemplateModel
                {
                    BccEmailAddresses = x.BccEmailAddresses,
                    Body = x.Body,
                    EmailAccountId = x.EmailAccountId,
                    Id = x.Id,
                    IsActive = x.IsActive,
                    Name = x.Name,
                    Subject = x.Subject
                }),
                Total = messageTemplates.Count
            };
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            var messageTemplates = _messageTemplateService.GetAllMessageTemplates();
            var gridModel = new GridModel<MessageTemplateModel>
            {
                Data = messageTemplates.Select(x => new MessageTemplateModel 
                {
                    BccEmailAddresses = x.BccEmailAddresses,
                    Body = x.Body,
                    EmailAccountId = x.EmailAccountId,
                    Id = x.Id,
                    IsActive = x.IsActive,
                    Name = x.Name,
                    Subject = x.Subject
                }),
                Total = messageTemplates.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            var messageTemplate = _messageTemplateService.GetMessageTemplateById(id);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("List");

            var model = new MessageTemplateModel 
            {
                BccEmailAddresses = messageTemplate.BccEmailAddresses,
                Body = messageTemplate.Body,
                EmailAccountId = messageTemplate.EmailAccountId,
                Id = messageTemplate.Id,
                IsActive = messageTemplate.IsActive,
                Name = messageTemplate.Name,
                Subject = messageTemplate.Subject
            };
            model.AllowedTokens = FormatTokens(_messageTokenProvider.GetListOfAllowedTokens());
            //available email accounts
            foreach (var ea in _emailAccountService.GetAllEmailAccounts())
                model.AvailableEmailAccounts.Add(new EmailAccountModel 
                { 
                    DisplayName = ea.DisplayName,
                    Email = ea.Email,
                    EnableSsl = ea.EnableSsl,
                    Host = ea.Host,
                    Id = ea.Id,
                    Password = ea.Password,
                    Port = ea.Port,
                    UseDefaultCredentials = ea.UseDefaultCredentials,
                    Username = ea.Username
                });
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.BccEmailAddresses = messageTemplate.GetLocalized(x => x.BccEmailAddresses, languageId, false, false);
                locale.Subject = messageTemplate.GetLocalized(x => x.Subject, languageId, false, false);
                locale.Body = messageTemplate.GetLocalized(x => x.Body, languageId, false, false);

                var emailAccountId = messageTemplate.GetLocalized(x => x.EmailAccountId, languageId, false, false);
                locale.EmailAccountId = emailAccountId > 0 ? emailAccountId : _emailAccountSettings.DefaultEmailAccountId;
            });

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(MessageTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            var messageTemplate = _messageTemplateService.GetMessageTemplateById(model.Id);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                messageTemplate.BccEmailAddresses = model.BccEmailAddresses;
                messageTemplate.Body = model.Body;
                messageTemplate.EmailAccountId = model.EmailAccountId;
                messageTemplate.Id = model.Id;
                messageTemplate.IsActive = model.IsActive;
                messageTemplate.Name = model.Name;
                messageTemplate.Subject = model.Subject;
                
                _messageTemplateService.UpdateMessageTemplate(messageTemplate);
                //locales
                UpdateLocales(messageTemplate, model);

                return RedirectToAction("Edit", messageTemplate.Id);
            }


            //If we got this far, something failed, redisplay form
            model.AllowedTokens = FormatTokens(_messageTokenProvider.GetListOfAllowedTokens());
            //available email accounts
            foreach (var ea in _emailAccountService.GetAllEmailAccounts())
                model.AvailableEmailAccounts.Add(new EmailAccountModel
                {
                    DisplayName = ea.DisplayName,
                    Email = ea.Email,
                    EnableSsl = ea.EnableSsl,
                    Host = ea.Host,
                    Id = ea.Id,
                    Password = ea.Password,
                    Port = ea.Port,
                    UseDefaultCredentials = ea.UseDefaultCredentials,
                    Username = ea.Username
                });
            return View(model);
        }

        #endregion
    }
}
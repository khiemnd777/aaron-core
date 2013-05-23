using System.Collections.Generic;
using System.Web.Mvc;
using Aaron.Core.Web;
using Aaron.Core.Domain.Localization;
using Aaron.Core.Web.Localization;

namespace Aaron.Admin.Models.Messages
{
    public partial class MessageTemplateModel : BaseEntityModel, ILocalizedModel<MessageTemplateLocalizedModel>
    {
        public MessageTemplateModel()
        {
            Locales = new List<MessageTemplateLocalizedModel>();
            AvailableEmailAccounts = new List<EmailAccountModel>();
        }
        
        public string AllowedTokens { get; set; }

        [AllowHtml]
        public string Name { get; set; }

        [AllowHtml]
        public string BccEmailAddresses { get; set; }

        [AllowHtml]
        public string Subject { get; set; }

        [AllowHtml]
        public string Body { get; set; }

        [AllowHtml]
        public bool IsActive { get; set; }

        public int EmailAccountId { get; set; }

        public IList<MessageTemplateLocalizedModel> Locales { get; set; }
        public IList<EmailAccountModel> AvailableEmailAccounts { get; set; }
    }

    public partial class MessageTemplateLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [AllowHtml]
        public string BccEmailAddresses { get; set; }

        [AllowHtml]
        public string Subject { get; set; }

        [AllowHtml]
        public string Body { get; set; }

        public int EmailAccountId { get; set; }
    }
}
using System.Collections.Generic;
using System.Web.Mvc;
using Aaron.Core.Web;
using Aaron.Core.Web.Localization;
using Aaron.Core.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Aaron.Admin.Models.Topics
{
    public partial class TopicModel : SEOEntityModel, ILocalizedModel<TopicLocalizedModel>
    {
        public TopicModel()
        {
            Locales = new List<TopicLocalizedModel>();
        }

        [AaronResourceDisplayName("Admin.ContentManagement.Topics.Fields.SystemName")]
        [Required]
        [AllowHtml]
        public string SystemName { get; set; }

        [AaronResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInSitemap")]
        public bool IncludeInSitemap { get; set; }

        [AaronResourceDisplayName("Admin.ContentManagement.Topics.Fields.IsPasswordProtected")]
        public bool IsPasswordProtected { get; set; }

        [AaronResourceDisplayName("Admin.ContentManagement.Topics.Fields.Password")]
        public string Password { get; set; }

        [AaronResourceDisplayName("Admin.ContentManagement.Topics.Fields.URL")]
        [AllowHtml]
        public string Url { get; set; }

        [AaronResourceDisplayName("Admin.ContentManagement.Topics.Fields.Title")]
        [AllowHtml]
        public string Title { get; set; }

        [AaronResourceDisplayName("Admin.ContentManagement.Topics.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }
        
        public IList<TopicLocalizedModel> Locales { get; set; }
    }

    public partial class TopicLocalizedModel : SEOLocalizedModelLocal
    {
        [AaronResourceDisplayName("Admin.ContentManagement.Topics.Fields.Title")]
        [AllowHtml]
        public string Title { get; set; }

        [AaronResourceDisplayName("Admin.ContentManagement.Topics.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }
    }
}
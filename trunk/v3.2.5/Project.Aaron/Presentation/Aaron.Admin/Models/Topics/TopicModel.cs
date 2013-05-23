using System.Collections.Generic;
using System.Web.Mvc;
using Aaron.Core.Web;
using Aaron.Core.Web.Localization;
using Aaron.Core.Web.Mvc;

namespace Aaron.Admin.Models.Topics
{
    public partial class TopicModel : BaseEntityModel, ILocalizedModel<TopicLocalizedModel>
    {
        public TopicModel()
        {
            Locales = new List<TopicLocalizedModel>();
        }

        [AllowHtml]
        public string SystemName { get; set; }

        public bool IncludeInSitemap { get; set; }

        public bool IsPasswordProtected { get; set; }

        public string Password { get; set; }

        [AllowHtml]
        public string Url { get; set; }

        [AllowHtml]
        public string Title { get; set; }

        [AllowHtml]
        public string Body { get; set; }

        [AllowHtml]
        public string MetaKeywords { get; set; }

        [AllowHtml]
        public string MetaDescription { get; set; }

        [AllowHtml]
        public string MetaTitle { get; set; }
        
        public IList<TopicLocalizedModel> Locales { get; set; }
    }

    public partial class TopicLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [AllowHtml]
        public string Title { get; set; }

        [AllowHtml]
        public string Body { get; set; }

        [AllowHtml]
        public string MetaKeywords { get; set; }

        [AllowHtml]
        public string MetaDescription { get; set; }

        [AllowHtml]
        public string MetaTitle { get; set; }
    }
}
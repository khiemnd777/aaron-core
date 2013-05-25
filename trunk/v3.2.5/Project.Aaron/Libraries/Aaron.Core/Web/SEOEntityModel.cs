using System.Web.Mvc;
namespace Aaron.Core.Web
{
    public class SEOEntityModel : BaseEntityModel
    {
        [AaronResourceDisplayName("Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [AaronResourceDisplayName("Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [AaronResourceDisplayName("Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        public string SeName { get; set; }
    }
}

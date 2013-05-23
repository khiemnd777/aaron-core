using System.Web.Mvc;
using Aaron.Core.Web;
using Aaron.Core.Web.Mvc;

namespace Aaron.Admin.Models.Localization
{
    public partial class LanguageModel : BaseEntityModel
    {
        [AaronResourceDisplayName("Admin.Configuration.Languages.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [AaronResourceDisplayName("Admin.Configuration.Languages.Fields.LanguageCulture")]
        [AllowHtml]
        public string LanguageCulture { get; set; }

        [AaronResourceDisplayName("Admin.Configuration.Languages.Fields.UniqueSeoCode")]
        [AllowHtml]
        public string UniqueSeoCode { get; set; }
        
        [AaronResourceDisplayName("Admin.Configuration.Languages.Fields.FlagImageFileName")]
        [AllowHtml]
        public string FlagImageFileName { get; set; }

        [AaronResourceDisplayName("Admin.Configuration.Languages.Fields.Rtl")]
        public bool Rtl { get; set; }

        [AaronResourceDisplayName("Admin.Configuration.Languages.Fields.Published")]
        public bool Published { get; set; }

        [AaronResourceDisplayName("Admin.Configuration.Languages.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}
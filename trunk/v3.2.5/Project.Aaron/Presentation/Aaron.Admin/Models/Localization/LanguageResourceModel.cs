using System.Web.Mvc;
using Aaron.Core.Web;
using Aaron.Core.Web.Mvc;

namespace Aaron.Admin.Models.Localization
{
    public partial class LanguageResourceModel : BaseEntityModel
    {
        [AaronResourceDisplayName("Admin.Configuration.Languages.Resources.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [AaronResourceDisplayName("Admin.Configuration.Languages.Resources.Fields.Value")]
        [AllowHtml]
        public string Value { get; set; }

        [AaronResourceDisplayName("Admin.Configuration.Languages.Resources.Fields.LanguageName")]
        [AllowHtml]
        public string LanguageName { get; set; }

        public int LanguageId { get; set; }
    }
}
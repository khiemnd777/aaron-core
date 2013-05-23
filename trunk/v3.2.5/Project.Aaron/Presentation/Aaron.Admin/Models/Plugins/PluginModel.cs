using System.Collections.Generic;
using System.Web.Mvc;
using Aaron.Core.Web;
using Aaron.Core.Web.Localization;

namespace Aaron.Admin.Models.Plugins
{
    public partial class PluginModel : BaseModel, ILocalizedModel<PluginLocalizedModel>
    {
        public PluginModel()
        {
            Locales = new List<PluginLocalizedModel>();
        }
        
        [AllowHtml]
        public string Group { get; set; }

        [AllowHtml]
        public string FriendlyName { get; set; }

        [AllowHtml]
        public string SystemName { get; set; }

        [AllowHtml]
        public string Version { get; set; }

        [AllowHtml]
        public string Author { get; set; }

        public int DisplayOrder { get; set; }

        public string ConfigurationUrl { get; set; }

        public bool Installed { get; set; }

        public bool CanChangeEnabled { get; set; }

        public bool IsEnabled { get; set; }

        public IList<PluginLocalizedModel> Locales { get; set; }
    }
    public partial class PluginLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [AllowHtml]
        public string FriendlyName { get; set; }
    }
}
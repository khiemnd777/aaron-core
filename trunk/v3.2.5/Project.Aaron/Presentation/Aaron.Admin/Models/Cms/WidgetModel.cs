using System.Web.Mvc;
using System.Web.Routing;
using Aaron.Core.Web;
using Aaron.Core.Web.Mvc;

namespace Aaron.Admin.Models.Cms
{
    public partial class WidgetModel : BaseModel
    {
        [AllowHtml]
        public string FriendlyName { get; set; }

        [AllowHtml]
        public string SystemName { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public string ConfigurationActionName { get; set; }
        public string ConfigurationControllerName { get; set; }
        public RouteValueDictionary ConfigurationRouteValues { get; set; }
    }
}
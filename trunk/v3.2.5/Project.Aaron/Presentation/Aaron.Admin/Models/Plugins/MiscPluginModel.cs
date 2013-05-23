using System.Web.Routing;
using Aaron.Core.Web;

namespace Aaron.Admin.Models.Plugins
{
    public partial class MiscPluginModel : BaseModel
    {
        public string FriendlyName { get; set; }

        public string ConfigurationActionName { get; set; }
        public string ConfigurationControllerName { get; set; }
        public RouteValueDictionary ConfigurationRouteValues { get; set; }
    }
}
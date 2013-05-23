using System.Web.Routing;
using Aaron.Core.Web;

namespace $rootnamespace$.Models.Cms
{
    public partial class RenderWidgetModel : BaseModel
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
    }
}
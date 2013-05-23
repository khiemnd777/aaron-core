using System.Web.Routing;
using Aaron.Core.Web;

namespace $rootnamespace$.Models.Accounts
{
    public partial class ExternalAuthenticationMethodModel : BaseModel
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
    }
}
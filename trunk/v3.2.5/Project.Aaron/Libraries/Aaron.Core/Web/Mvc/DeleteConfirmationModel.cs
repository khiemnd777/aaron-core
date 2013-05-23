using Aaron.Core.Web;

namespace Aaron.Core.Web.Mvc
{
    public class DeleteConfirmationModel : BaseEntityModel
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
    }
}
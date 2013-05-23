using Aaron.Core.Web.Mvc;
using Aaron.Core.Web;

namespace Aaron.Admin.Models.Security
{
    public partial class PermissionRecordModel : BaseModel
    {
        public string Name { get; set; }
        public string SystemName { get; set; }
    }
}
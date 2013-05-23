using Aaron.Core.Web;
using Aaron.Core.Web.Mvc;

namespace Aaron.Admin.Models.Logging
{
    public partial class ActivityLogTypeModel : BaseEntityModel
    {
        public string Name { get; set; }
        
        public bool Enabled { get; set; }
    }
}
using System;
using Aaron.Core.Web;
using Aaron.Core.Web.Mvc;

namespace Nop.Admin.Models.Logging
{
    public partial class ActivityLogModel : BaseEntityModel
    {
        public string ActivityLogTypeName { get; set; }
        
        public int AccountId { get; set; }
        
        public string AccountEmail { get; set; }
        
        public string Comment { get; set; }
        
        public DateTime CreatedOn { get; set; }
    }
}

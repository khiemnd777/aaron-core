using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Aaron.Core.Web;
using Aaron.Core.Web.Mvc;

namespace Aaron.Admin.Models.Logging
{
    public partial class ActivityLogSearchModel : BaseModel
    {
        public ActivityLogSearchModel()
        {
            ActivityLogType = new List<SelectListItem>();
        }
        
        [UIHint("DateNullable")]
        public DateTime? CreatedOnFrom { get; set; }

        
        [UIHint("DateNullable")]
        public DateTime? CreatedOnTo { get; set; }

        public int ActivityLogTypeId { get; set; }

        public IList<SelectListItem> ActivityLogType { get; set; }
    }
}
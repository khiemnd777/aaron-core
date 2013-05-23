using System;
using System.Web.Mvc;
using Aaron.Core.Web;
using Aaron.Core.Web.Mvc;

namespace Aaron.Admin.Models.Logging
{
    public partial class LogModel : BaseEntityModel
    {
        public string LogLevel { get; set; }

        [AllowHtml]
        public string ShortMessage { get; set; }

        [AllowHtml]
        public string FullMessage { get; set; }

        [AllowHtml]
        public string IpAddress { get; set; }

        public int? AccountId { get; set; }
        
        public string AccountEmail { get; set; }

        [AllowHtml]
        public string PageUrl { get; set; }

        [AllowHtml]
        public string ReferrerUrl { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
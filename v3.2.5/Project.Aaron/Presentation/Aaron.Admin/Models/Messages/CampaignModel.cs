using Aaron.Core.Web;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Aaron.Admin.Models.Messages
{
    public partial class CampaignModel : BaseEntityModel
    {
        [AllowHtml]
        [Display(Name="Tên")]
        public string Name { get; set; }

        [AllowHtml]
        [Display(Name = "Tiêu đề")]
        public string Subject { get; set; }

        [AllowHtml]
        [Display(Name = "Nội dung")]
        public string Body { get; set; }
        
        public DateTime CreatedOn { get; set; }

        public string AllowedTokens { get; set; }

        [AllowHtml]
        public string TestEmail { get; set; }
    }
}
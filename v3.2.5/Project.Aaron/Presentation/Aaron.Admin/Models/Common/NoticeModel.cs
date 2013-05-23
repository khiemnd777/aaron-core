using System;
using Aaron.Core.Web;
using System.ComponentModel.DataAnnotations;

namespace Aaron.Admin.Models.Common
{
    public class NoticeModel: BaseEntityModel
    {
        [Display(Name="Tên")]
        public string Name { get; set; }

        [Display(Name="Nội Dung")]
        public string Content { get; set; }

        [Display(Name="Phát Hành")]
        public bool Published { get; set; }

        public bool IsSystem { get; set; }

        [Display(Name="Ngày Tạo")]
        public DateTime CreationDate { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
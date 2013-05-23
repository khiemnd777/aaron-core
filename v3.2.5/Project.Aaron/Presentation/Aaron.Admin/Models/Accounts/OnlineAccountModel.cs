using System;
using System.ComponentModel.DataAnnotations;
using Aaron.Core.Web;

namespace Aaron.Admin.Models.Accounts
{
    public partial class OnlineAccountModel : BaseEntityModel
    {
        [Display(Name="Tài khoản")]
        public string AccountInfo { get; set; }

        [Display(Name = "Địa chỉ Ip")]
        public string LastIpAddress { get; set; }

        [Display(Name = "Quốc gia")]
        public string Location { get; set; }

        [Display(Name = "Ngày cập nhật cuối")]
        public DateTime LastActivityDate { get; set; }
    }
}
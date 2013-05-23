using System.Web.Mvc;
using Aaron.Core.Web;
using System.ComponentModel.DataAnnotations;

namespace Aaron.Admin.Models.Messages
{
    public partial class EmailAccountModel : BaseEntityModel
    {
        [AllowHtml]
        [Display(Name="Email:")]
        public string Email { get; set; }

        [AllowHtml]
        [Display(Name = "Tên đại diện:")]
        public string DisplayName { get; set; }

        [AllowHtml]
        public string Host { get; set; }

        public int Port { get; set; }

        [AllowHtml]
        public string Username { get; set; }

        [AllowHtml]
        public string Password { get; set; }

        public bool EnableSsl { get; set; }

        public bool UseDefaultCredentials { get; set; }

        [Display(Name = "Email mặc định?")]
        public bool IsDefaultEmailAccount { get; set; }

        [AllowHtml]
        [Display(Name = "Gửi email xác thực:")]
        public string SendTestEmailTo { get; set; }

    }
}
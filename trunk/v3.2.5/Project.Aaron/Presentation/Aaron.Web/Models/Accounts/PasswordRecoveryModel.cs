using Aaron.Core.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Aaron.Web.Models.Accounts
{
    public class PasswordRecoveryModel : BaseEntityModel
    {
        [AllowHtml]
        [Display(Name="Email phục hồi mật khẩu:")]
        [Required(AllowEmptyStrings=false, ErrorMessage="Email xác thực không được để trống!")]
        public string Email { get; set; }
        public string Result { get; set; }
    }
}
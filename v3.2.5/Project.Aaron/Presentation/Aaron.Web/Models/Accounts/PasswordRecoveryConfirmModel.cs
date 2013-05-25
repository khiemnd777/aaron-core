using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Aaron.Core.Web;

namespace Aaron.Web.Models.Accounts
{
    public partial class PasswordRecoveryConfirmModel : BaseModel
    {
        [Display(Name = "Mật khẩu mới:")]
        [DataType(DataType.Password)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Mật khẩu không được để trống!")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Mật khẩu phải ít nhất 6 ký tự!")]
        [AllowHtml]
        public string NewPassword { get; set; }

        [Display(Name = "Xác nhận mật khẩu mới:")]
        [DataType(DataType.Password)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Xác nhận mật khẩu không được để trống!")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Mật khẩu phải ít nhất 6 ký tự!")]
        [CompareAttribute("NewPassword", ErrorMessage = "Mật khẩu xác nhận không chính xác")]
        [AllowHtml]
        public string ConfirmNewPassword { get; set; }

        public bool SuccessfullyChanged { get; set; }
        public string Result { get; set; }
    }
}
using Aaron.Core.Web;
using System.ComponentModel.DataAnnotations;

namespace $rootnamespace$.Models.Accounts
{
    public partial class LoginModel : BaseModel
    {
        [Required(ErrorMessage="Vui lòng không để trống Email!")]
        public string Email { get; set; }

        [Display(Name="Mật khẩu")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Vui lòng không để trống mật khẩu!")]
        public string Password { get; set; }

        [Display(Name = "Duy trì đăng nhập")]
        public bool RememberMe { get; set; }

    }
}
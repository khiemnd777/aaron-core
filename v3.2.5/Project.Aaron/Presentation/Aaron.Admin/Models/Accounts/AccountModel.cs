using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Aaron.Core.Web;

namespace Aaron.Admin.Models.Accounts
{
    public class AccountModel : BaseEntityModel
    {
        public string Username { get; set; }
        [Display(Name="Email")]
        public string Email { get; set; }
        [Display(Name="Mật khẩu")]
        [StringLength(30, MinimumLength=6, ErrorMessage="Mật khẩu phải ít nhất 6 ký tự!")]
        public string Password { get; set; }
        [Display(Name = "Mật khẩu")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Mật khẩu phải ít nhất 6 ký tự!")]
        public string ChangePassword { get; set; }
        [Display(Name = "Kích hoạt")]
        public bool Active { get; set; }

        public string AccountRoleNames { get; set; }
        public List<AccountRoleModel> AvailableAccountRoles { get; set; }
        public int[] SelectedAccountRoleIds { get; set; }

        public bool AllowManagingAccountRoles { get; set; }
    }

    public class AccountListModel : BaseEntityModel
    {
        public string Email { get; set; }
        [Display(Name = "Kích hoạt?")]
        public bool Active { get; set; }
        [Display(Name = "Vai trò")]
        public string AccountRoleNames { get; set; }
        [Display(Name = "Ngày tạo")]
        public DateTime CreationDate  { get; set; }
        [Display(Name = "Lần cập nhật cuối")]
        public DateTime LastestUpdatedDate { get; set; }
    }
}
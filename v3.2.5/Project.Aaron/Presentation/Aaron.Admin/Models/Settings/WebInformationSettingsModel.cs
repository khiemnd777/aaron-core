using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Aaron.Core.Web;

namespace Aaron.Admin.Models.Settings
{
    public class WebInformationSettingsModel : BaseModel
    {
        [Display(Name="Tên Website:")]
        public string WebName { get; set; }
        [Display(Name = "Url:")]
        public string WebUrl { get; set; }
        [Display(Name = "Đóng trang web:")]
        public bool WebClosed { get; set; }
        [Display(Name = "Cho phép Admin truy cập khi web bị đóng:")]
        public bool WebClosedAllowForAdmins { get; set; }
        [Display(Name = "Cho phép người dùng chọn Theme:")]
        public bool AllowAccountToSelectTheme { get; set; }
        [Display(Name = "Theme mặc định:")]
        public string DefaultWebThemeForDesktops { get; set; }
        public IList<SelectListItem> AvailableWebThemeForDesktops { get; set; }
        //public bool UseMiniProfiler { get; set; }
    }
}
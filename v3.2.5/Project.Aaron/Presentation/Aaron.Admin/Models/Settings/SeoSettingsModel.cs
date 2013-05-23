using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Aaron.Core.Web;
using Aaron.Core.Domain.Common;

namespace Aaron.Admin.Models.Settings
{
    public class SeoSettingsModel : BaseModel
    {
        [Display(Name="Dấu ngăn cách:")]
        public string PageTitleSeparator { get; set; }
        [Display(Name = "Vị trí:")]
        public PageTitleSeoAdjustment PageTitleSeoAdjustment { get; set; }
        public IList<SelectListItem> PageTitleSeoAdjustmentValues { get; set; }
        [Display(Name = "Tiêu đề mặc định:")]
        public string DefaultTitle { get; set; }
        [Display(Name = "Từ khóa mặc định [Meta keywords]:")]
        public string DefaultMetaKeywords { get; set; }
        [Display(Name = "Mô tả mặc định [Meta description]:")]
        public string DefaultMetaDescription { get; set; }
        [Display(Name = "Cho phép chỉ sử dụng Đường dẫn gốc [bỏ qua các bản sao]:")]
        public bool CanonicalUrlsEnabled { get; set; }
    }
}
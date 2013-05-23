using System.ComponentModel.DataAnnotations;

namespace Aaron.Admin.Models.Settings
{
    public class CatalogSettingsModel
    {
        [Display(Name = "Số item trên danh mục động")]
        public int DefaultSizeOnMenu { get; set; }

        [Display(Name = "Số danh mục động trên một trang")]
        public int DefaultGenericCatalogItemsOnPage { get; set; }

        [Display(Name = "Số danh mục trên một trang")]
        public int DefaultCatalogItemsOnPage { get; set; }

        [Display(Name = "Icon danh mục động mặc định")]
        public string DefaultGenericCatalogIcon { get; set; }

        [Display(Name = "Hình đại hiện danh mục mặc định")]
        public string DefaultAvatar { get; set; }

        [Display(Name = "Kích thước Icon (ví dụ: 16x16)")]
        public string IconDimension { get; set; }

        [Display(Name = "Kích thước hình đại diện danh mục (ví dụ: 16x16)")]
        public string AvatarDimension { get; set; }

        [Display(Name = "Hiện nút 'Chia sẻ'")]
        public bool ShowShareButton { get; set; }

        [Display(Name = "Đường dẫn Icon danh mục động")]
        public string GenericCatalogIconPath { get; set; }

        [Display(Name = "Tự động upload hình")]
        public bool AllowAutoUpload { get; set; }

        [Display(Name = "Upload nhiều hình (khuyến cáo: không check)")]
        public bool AllowUploadMultiFile { get; set; }
    }
}
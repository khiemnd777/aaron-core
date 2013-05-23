using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Aaron.Core.Web;

namespace Aaron.Admin.Models.Catalogs
{
    [KnownType(typeof(CatalogTemplateModel))]
    public class CatalogTemplateModel: BaseEntityModel
    {
        [Required(ErrorMessage = "'Tên' không để trống!")]
        [Display(Name = "Template")]
        public string Name { get; set; }

        [Display(Name = "Đường dẫn")]
        [UIHint("_UploadTemplateFile")]
        public string ViewPath { get; set; }

        [Display(Name = "Ưu tiên")]
        [UIHint("_ShowDisplayOrder")]
        public int DisplayOrder { get; set; }

        [Display(Name = "Hiển thị")]
        public bool Published { get; set; }

        public string OldViewPath
        {
            get { return ViewPath; }
            set { ViewPath = value; }
        }
    }
}
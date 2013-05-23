using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Web.Mvc;
using Aaron.Core.Web;

namespace Aaron.Admin.Models.Catalogs
{
    [KnownType(typeof(GenericCatalogAttributeModel))]
    public class GenericCatalogAttributeModel: BaseEntityModel
    {
        [AllowHtml]
        [Required]
        [Display(Name = "Tên Thuộc Tính")]
        public string Name { get; set; }

        [Display(Name = "Tên Hệ Thống")]
        public string SystemName { get; set; }

        [Display(Name = "Hiển Thị")]
        public bool Published { get; set; }

        [Display(Name = "Ưu Tiên")]
        public int DisplayOrder { get; set; }

        public int GenericCatalogId { get; set; }
        public GenericCatalogModel GenericCatalogModel { get; set; }

        [Display(Name = "Chọn Loại Control")]
        [UIHint("GridForeignKey")]
        public string ControlTypeId { get; set; }

    }
}
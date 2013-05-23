using System.Collections.Generic;
using Aaron.Core.Web;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Web.Mvc;

namespace Aaron.Admin.Models.Catalogs
{
    [KnownType(typeof(GenericCatalogModel))]
    public class GenericCatalogModel: BaseEntityModel
    {
        [AllowHtml]
        [Required(ErrorMessage = "Vui lòng không bỏ trống")]
        [Display(Name = "Tên")]
        public string Name { get; set; }

        //[Required(ErrorMessage = "Chưa chọn 'Biểu tượng'")]
        [Display(Name = "Chọn Biểu Tượng Danh Mục")]
        public string Icon { get; set; }

        [Display(Name = "Phân Trang")]
        [RegularExpression("([0-9]*)", ErrorMessage = "Vui lòng nhập số")]
        public int  ItemOnPage { get; set; }

        [Display(Name = "Số Item Trên Menu")]
        [RegularExpression("([0-9]*)", ErrorMessage = "Vui lòng nhập số")]
        public int  SizeOnMenu { get; set; }

        [Display(Name = "Hiển Thị")]
        public bool Published { get; set; }

        [Display(Name = "Ưu Tiên")]
        [RegularExpression("([0-9]*)", ErrorMessage = "Vui lòng nhập số")]
        public int DisplayOrder { get; set; }

        [Display(Name = "Vị trí")]
        public string BlockViewId { get; set; }

        [Display(Name = "Chọn Template")]
        public int? TemplateId { get; set; }

        public string OldFilePath { get; set; }
        //public IEnumerable<TemplateModel> TemplateModels { get; set; }
        //public IEnumerable<ControlTypeModel> ControlTypeModels { get; set; }

    }

    public class GenericCatalogGridModel: BaseEntityModel
    {
        //public int GenericCatalogId { get; set; }

        [Display(Name = "Tên")]
        public string Name { get; set; }

        //public string SystemName { get; set; }

        [Display(Name = "Biểu Tượng")]
        //[UIHint("_ShowIconImage")]
        public string Icon { get; set; }

        //[Display(Name = "")]
        //public int ItemOnPage { get; set; }

        //[Display(Name = "")]
        //public int SizeOnMenu { get; set; }

        [Display(Name = "Hiển thị")]
        public bool Published { get; set; }

        [Display(Name = "Ưu Tiên")]
        public int DisplayOrder { get; set; }

        [Display(Name ="Vị Trí")]
        public string BlockViewId { get; set; }

        //public int TemplateId { get; set; }

        //public string TemplateName { get; set; }
    }

    public class GenericCatalogSelectionListModel
    {
        private List<GenericCatalogModel> _genericCatalogs;

        public int GenericCatalogId { get; set; }

        public List<GenericCatalogModel> GenericCatalogs
        {
            get { return _genericCatalogs ?? (_genericCatalogs = new List<GenericCatalogModel>()); }
            set { _genericCatalogs = value; }
        }
    }
}
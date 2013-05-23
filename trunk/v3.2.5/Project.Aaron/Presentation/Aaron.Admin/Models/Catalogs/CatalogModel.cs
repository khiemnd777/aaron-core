using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Aaron.Core.Domain.Catalogs;
using Aaron.Admin.Models.Common;

namespace Aaron.Admin.Models.Catalogs
{
    public class CatalogModel : SEOModel
    {
        private List<CatalogRecordModel> _catalogRecordModel;
        private List<Catalog> _parentCatalogs;

        public CatalogModel()
        {
            Published = true;
        }

        public CatalogModel(Catalog catalog)
        {
            Id = catalog.Id;
            Name = catalog.Name;
            MetaTitle = catalog.MetaTitle;
            MetaKeywords = catalog.MetaKeywords;
            MetaDescription = catalog.MetaDescription;
            Published = catalog.Published;
            DisplayOrder = catalog.DisplayOrder;
            Avatar = catalog.Avatar;
            SEOUrlName = catalog.SEOUrlName;
            TemplateId = (catalog.TemplateId.HasValue) ? catalog.TemplateId.Value : 0;
            if (catalog.GenericCatalogId.HasValue)
            {
                GenericCatalogId = catalog.GenericCatalogId.Value;
                GenericCatalogName = catalog.GenericCatalog.Name;
            }
            else
            {
                GenericCatalogId = 0;
            }
            OldFilePath = catalog.Avatar;
            ParentCatalogId = (catalog.ParentCatalogId.HasValue) ? catalog.ParentCatalogId.Value : 0;
        }

        [Display(Name = "Danh mục"), Required(ErrorMessage = "Vui lòng không để trống")]
        public string Name { get; set; }

        [Display(Name = "Danh mục cha")]
        public int ParentCatalogId { get; set; }

        [Display(Name = "Template")]
        public int? TemplateId { get; set; }

        [Display(Name = "Hình Ảnh")]
        public string Avatar { get; set; }

        [Display(Name = "Thứ tự")]
        public int DisplayOrder { get; set; }

        [Display(Name = "Danh mục động")]
        public int GenericCatalogId { get; set; }

        [Display(Name = "Danh mục động")]
        public string GenericCatalogName { get; set; }

        [Display(Name = "Hiển thị")]
        public bool Published { get; set; }

        public List<CatalogRecordModel> CatalogRecordModel 
        {
            get { return _catalogRecordModel ?? (_catalogRecordModel = new List<CatalogRecordModel>()); }
            set { _catalogRecordModel = value; } 
        }

        public List<Catalog> ParentCatalogs
        {
            get { return _parentCatalogs ?? (_parentCatalogs = new List<Catalog>()); }
            set { _parentCatalogs = value; }
        }

        public string OldFilePath { get; set; }
    }
}
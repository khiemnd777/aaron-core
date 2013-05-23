using System.Collections.Generic;
using Aaron.Core.Web;

namespace Aaron.Admin.Models.Catalogs
{
    public class GenericCatalogEditableModel : BaseEntityModel
    {
        private List<CatalogEditableModel> _catalogs;
        public string Name { get; set; }
        public List<CatalogEditableModel> Catalogs
        {
            get { return _catalogs ?? (_catalogs = new List<CatalogEditableModel>()); }
            set { _catalogs = value; }
        }
    }

    public class CatalogEditableModel : BaseEntityModel
    {
        public string Name { get; set; }
    }
}
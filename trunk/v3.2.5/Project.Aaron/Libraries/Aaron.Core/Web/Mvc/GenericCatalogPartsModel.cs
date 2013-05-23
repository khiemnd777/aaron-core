using System.Collections.Generic;
using Aaron.Core.Web;

namespace Aaron.Core.Web.Mvc
{
    public class GenericCatalogPartsModel : BaseEntityModel
    {
        private IList<CatalogsInGenericModel> _catalogs;
        public string Name { get; set; }
        public string Icon { get; set; }
        public int SizeOnMenu { get; set; }
        public int Count { get; set; }
        public IList<CatalogsInGenericModel> Catalogs 
        {
            get { return _catalogs ?? (_catalogs = new List<CatalogsInGenericModel>()); }
            set { _catalogs = value; }
        }
    }
}

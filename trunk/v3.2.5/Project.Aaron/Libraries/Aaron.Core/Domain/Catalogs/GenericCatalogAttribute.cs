using System.Collections.Generic;
using Aaron.Core.Domain.Common;

namespace Aaron.Core.Domain.Catalogs
{
    public partial class GenericCatalogAttribute : Attribute
    {
        private ICollection<CatalogAttributeRecord> _catalogAttributeRecords;
        public virtual int GenericCatalogId { get; set; }
        public virtual GenericCatalog GenericCatalog { get; set; }
        public virtual ICollection<CatalogAttributeRecord> CatalogAttributeRecords
        {
            get { return _catalogAttributeRecords ?? (_catalogAttributeRecords = new List<CatalogAttributeRecord>()); }
            protected set { _catalogAttributeRecords = value; }
        }
    }
}

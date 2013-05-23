using System.Collections.Generic;
using Aaron.Core;
using Aaron.Core.Domain.Common;

namespace Aaron.Core.Domain.Catalogs
{
    public partial class Catalog : SEOEntity<int>
    {
        private ICollection<CatalogAttributeRecord> _catalogAttributeRecord;
        private ICollection<Catalog> _catalogs;

        public virtual string Name { get; set; }
        public virtual bool Published { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual string Avatar { get; set; }
        public virtual int? GenericCatalogId { get; set; }
        public virtual int? TemplateId { get; set; }
        public virtual int? ParentCatalogId { get; set; }
        public virtual Catalog ParentCatalog { get; set; }
        public virtual GenericCatalog GenericCatalog { get; set; }
        public virtual CatalogTemplate Template { get; set; }
        public virtual ICollection<CatalogAttributeRecord> CatalogAttributeRecord
        {
            get { return _catalogAttributeRecord ?? (_catalogAttributeRecord = new List<CatalogAttributeRecord>()); }
            protected set { _catalogAttributeRecord = value; }
        }
        public virtual ICollection<Catalog> Catalogs
        {
            get { return _catalogs ?? (_catalogs = new List<Catalog>()); }
            protected set { _catalogs = value; }
        }
    }
}

using System;
using System.Collections.Generic;
using Aaron.Core;

namespace Aaron.Core.Domain.Catalogs
{
    public partial class CatalogAttributeRecord : BaseEntity<Guid>
    {
        public virtual string Value { get; set; }
        public virtual int CatalogId { get; set; }
        public virtual int AttributeId { get; set; }
        public virtual Catalog Catalog { get; set; }
        public virtual GenericCatalogAttribute Attribute { get; set; }
    }
}

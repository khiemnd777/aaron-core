using System;
using Aaron.Core;
using Aaron.Core.Domain.Catalogs;

namespace Aaron.Core.Mapping.Catalogs
{
    public class CatalogAttributeRecordMap : BaseEntityTypeConfiguration<CatalogAttributeRecord, Guid>
    {
        public CatalogAttributeRecordMap()
            : base()
        {
            this.HasRequired(car => car.Attribute)
                .WithMany(gca => gca.CatalogAttributeRecords)
                .HasForeignKey(car => car.AttributeId);
            
            this.HasRequired(car => car.Catalog)
                .WithMany(c => c.CatalogAttributeRecord)
                .HasForeignKey(car => car.CatalogId);
        }
    }
}

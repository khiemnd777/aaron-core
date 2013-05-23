using Aaron.Core;
using Aaron.Core.Domain.Catalogs;

namespace Aaron.Core.Mapping.Catalogs
{
    public class GenericCatalogAttributeMap : BaseEntityTypeConfiguration<GenericCatalogAttribute,  int>
    {
        public GenericCatalogAttributeMap()
            : base(true)
        {
            this.HasRequired(gca => gca.GenericCatalog)
                .WithMany(gc => gc.Attributes)
                .HasForeignKey(gca => gca.GenericCatalogId)
                .WillCascadeOnDelete(false);
        }
    }
}

using Aaron.Core;
using Aaron.Core.Domain.Catalogs;

namespace Aaron.Core.Mapping.Catalogs
{
    public class CatalogMap : SEOEntityTypeConfiguration<Catalog, int>
    {
        public CatalogMap()
            : base()
        {
            this.Property(c => c.Name).IsRequired().HasMaxLength(250);
            this.Property(c => c.Avatar).IsMaxLength();

            this.HasOptional(c => c.GenericCatalog)
                .WithMany(gc => gc.Catalogs)
                .HasForeignKey(c => c.GenericCatalogId);

            this.HasOptional(c => c.Template)
                .WithMany(t => t.Catalogs)
                .HasForeignKey(c => c.TemplateId);

            this.HasOptional(c => c.ParentCatalog)
                .WithMany(c => c.Catalogs)
                .HasForeignKey(c => c.ParentCatalogId);
        }
    }
}
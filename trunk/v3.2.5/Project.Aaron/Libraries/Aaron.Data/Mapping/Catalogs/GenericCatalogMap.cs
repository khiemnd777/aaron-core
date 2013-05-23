using Aaron.Core;
using Aaron.Core.Domain.Catalogs;

namespace Aaron.Core.Mapping.Catalogs
{
    public class GenericCatalogMap : SEOEntityTypeConfiguration<GenericCatalog, int>
    {
        public GenericCatalogMap()
            : base()
        {
            this.Property(gc => gc.Name).IsRequired().HasMaxLength(250);
            this.Property(gc => gc.SystemName).IsRequired().HasMaxLength(250);

            this.HasOptional(gc => gc.Template)
                .WithMany(t => t.GenericCatalogTemplates)
                .HasForeignKey(gc => gc.TemplateId)
                .WillCascadeOnDelete(false);
        }
    }
}

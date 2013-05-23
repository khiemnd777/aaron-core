using Aaron.Core;
using Aaron.Core.Domain.Catalogs;

namespace Aaron.Core.Mapping.Catalogs
{
    public class GenericCatalogTemplateMap : BaseEntityTypeConfiguration<GenericCatalogTemplate, int>
    {
        public GenericCatalogTemplateMap()
            : base(true)
        {
            
        }
    }
}

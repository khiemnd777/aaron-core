using Aaron.Core;
using Aaron.Core.Domain.Catalogs;

namespace Aaron.Core.Mapping.Catalogs
{
    public class CatalogTemplateMap : BaseEntityTypeConfiguration<CatalogTemplate, int>
    {
        public CatalogTemplateMap()
            : base(true)
        {

        }
    }
}

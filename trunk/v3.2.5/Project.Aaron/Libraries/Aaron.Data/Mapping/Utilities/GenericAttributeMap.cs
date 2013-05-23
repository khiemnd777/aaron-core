using Aaron.Core;
using Aaron.Core.Domain.Utilities;

namespace Aaron.Data.Mapping.Utilities
{
    public class GenericAttributeMap : BaseEntityTypeConfiguration<GenericAttribute, int>
    {
        public GenericAttributeMap()
            : base()
        {
            this.Property(ga => ga.KeyGroup).IsRequired().HasMaxLength(400);
            this.Property(ga => ga.Key).IsRequired().HasMaxLength(400);
            this.Property(ga => ga.Value).IsRequired().IsMaxLength();
        }
    }
}

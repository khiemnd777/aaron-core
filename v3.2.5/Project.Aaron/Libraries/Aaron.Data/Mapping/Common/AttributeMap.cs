using Aaron.Core;

namespace Aaron.Core.Mapping.Common
{
    public class AttributeMap : BaseEntityTypeConfiguration<Aaron.Core.Domain.Common.Attribute, int> 
    {
        public AttributeMap()
            : base()
        {
            this.Property(a => a.Name).IsRequired().HasMaxLength(255);
            this.Property(a => a.SystemName).IsRequired().HasMaxLength(255);
            this.Ignore(a => a.ControlType);
        }
    }
}

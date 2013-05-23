using Aaron.Core;
using Aaron.Core.Domain.Common;

namespace Aaron.Core.Mapping.Common
{
    public class TemplateMap : BaseEntityTypeConfiguration<Template, int>
    {
        public TemplateMap()
            : base()
        {
            this.Property(t => t.Name).IsRequired().HasMaxLength(255);
            this.Property(t => t.ViewPath).IsMaxLength();
        }
    }
}

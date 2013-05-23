using Aaron.Core;
using Aaron.Core.Domain.Configuration;

namespace Aaron.Data.Mapping.Configuration
{
    public partial class SettingMap : BaseEntityTypeConfiguration<Setting, int>
    {
        public SettingMap() 
            : base()
        {
            this.Property(s => s.Name).IsRequired().HasMaxLength(200);
            this.Property(s => s.Value).IsRequired().HasMaxLength(2000);
        }
    }
}

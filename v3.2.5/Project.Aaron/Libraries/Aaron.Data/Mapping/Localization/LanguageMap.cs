using Aaron.Core;
using Aaron.Core.Domain.Localization;

namespace Aaron.Data.Mapping.Localization
{
    public partial class LanguageMap : BaseEntityTypeConfiguration<Language, int>
    {
        public LanguageMap()
            : base()
        {
            this.Property(l => l.Name).IsRequired().HasMaxLength(100);
            this.Property(l => l.LanguageCulture).IsRequired().HasMaxLength(20);
            this.Property(l => l.UniqueSeoCode).HasMaxLength(2);
            this.Property(l => l.FlagImageFileName).HasMaxLength(50);
        }
    }
}

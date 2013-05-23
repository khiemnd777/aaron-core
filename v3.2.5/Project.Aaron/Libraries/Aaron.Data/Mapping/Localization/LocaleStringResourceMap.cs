using Aaron.Core;
using Aaron.Core.Domain.Localization;

namespace Aaron.Data.Mapping.Localization
{
    public partial class LocaleStringResourceMap : BaseEntityTypeConfiguration<LocaleStringResource, int>
    {
        public LocaleStringResourceMap()
            : base()
        {
            this.Property(lsr => lsr.ResourceName).IsRequired().HasMaxLength(200);
            this.Property(lsr => lsr.ResourceValue).IsRequired().IsMaxLength();


            this.HasRequired(lsr => lsr.Language)
                .WithMany(l => l.LocaleStringResources)
                .HasForeignKey(lsr => lsr.LanguageId);
        }
    }
}

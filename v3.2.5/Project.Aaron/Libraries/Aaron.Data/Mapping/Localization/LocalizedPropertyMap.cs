using Aaron.Core;
using Aaron.Core.Domain.Localization;

namespace Aaron.Data.Mapping.Localization
{
    public partial class LocalizedPropertyMap : BaseEntityTypeConfiguration<LocalizedProperty, int>
    {
        public LocalizedPropertyMap()
            : base()
        {
            this.Property(lp => lp.LocaleKeyGroup).IsRequired().HasMaxLength(400);
            this.Property(lp => lp.LocaleKey).IsRequired().HasMaxLength(400);
            this.Property(lp => lp.LocaleValue).IsRequired().IsMaxLength();

            this.HasRequired(lp => lp.Language)
                .WithMany()
                .HasForeignKey(lp => lp.LanguageId);
        }
    }
}

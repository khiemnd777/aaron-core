namespace Aaron.Core
{
    public abstract class SEOEntityTypeConfiguration<T, TKey> : BaseEntityTypeConfiguration<T, TKey> where T : SEOEntity<TKey>
                                                                                                    where TKey : struct
    {
        public SEOEntityTypeConfiguration(bool inherited = false)
            : base(inherited)
        {
            if (!inherited)
            {
                this.Property(x => x.MetaTitle).HasMaxLength(255);
                this.Property(x => x.MetaDescription).IsMaxLength();
                this.Property(x => x.MetaKeywords).IsMaxLength();
                this.Property(x => x.SEOUrlName).IsMaxLength();
            }
        }
    }
}

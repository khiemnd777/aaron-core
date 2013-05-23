using Aaron.Core;
using Aaron.Core.Domain.Accounts;

namespace Aaron.Data.Mapping.Accounts
{
    public partial class AccountContentMap : BaseEntityTypeConfiguration<AccountContent, int>
    {
        public AccountContentMap()
            : base()
        {
            this.Property(ac => ac.IpAddress).HasMaxLength(200);

            this.HasRequired(ac => ac.Account)
                .WithMany(c => c.AccountContent)
                .HasForeignKey(ca => ca.AccountId);
        }
    }
}

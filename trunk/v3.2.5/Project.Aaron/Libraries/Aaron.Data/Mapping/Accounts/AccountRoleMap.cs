using Aaron.Core;
using Aaron.Core.Domain.Accounts;

namespace Aaron.Data.Mapping.Accounts
{
    public partial class AccountRoleMap : BaseEntityTypeConfiguration<AccountRole, int>
    {
        public AccountRoleMap()
            : base()
        {
            this.Property(ar => ar.Name).IsRequired().HasMaxLength(255);
            this.Property(ar => ar.SystemName).HasMaxLength(255);
        }
    }
}

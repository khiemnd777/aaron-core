using Aaron.Core;
using Aaron.Core.Domain.Accounts;

namespace Aaron.Data.Mapping.Accounts
{
    public partial class AccountMap : BaseEntityTypeConfiguration<Account, int>
    {
        public AccountMap()
            : base()
        {
            this.Property(u => u.Username).HasMaxLength(1000);
            this.Property(u => u.Email).HasMaxLength(1000);
            this.Property(u => u.Password);

            this.Ignore(u => u.PasswordFormat);

            this.HasOptional(c => c.Language)
                .WithMany()
                .HasForeignKey(c => c.LanguageId).WillCascadeOnDelete(false);

            this.HasMany(c => c.AccountRoles)
                .WithMany()
                .Map(m => m.ToTable("Account_AccountRole"));
        }
    }
}
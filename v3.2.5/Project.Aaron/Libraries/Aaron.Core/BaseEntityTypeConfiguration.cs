using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
namespace Aaron.Core
{
    public abstract class BaseEntityTypeConfiguration<T, TKey> : EntityTypeConfiguration<T> where T : BaseEntity<TKey>
                                                                                            where TKey : struct
    {
        public BaseEntityTypeConfiguration(bool inherited = false)
        {
            if (!inherited)
            {
                var tbName = typeof(T).Name;
                this.ToTable(tbName);
                this.HasKey(x => x.Id);
                this.Property(x => x.CreationDate).IsOptional();
                this.Property(x => x.ModifiedDate).IsOptional();
            }
            else
            {
                this.Map(x =>
                {
                    x.MapInheritedProperties();
                });
                this.Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            }
        }
    }
}

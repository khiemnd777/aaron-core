using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Aaron.Core.Data
{
    public interface IContext
    {
        IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity;
        int SaveChanges();
        DbEntityEntry Entry<TEntity>(TEntity entity) where TEntity : BaseEntity;
        IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters)
            where TEntity : BaseEntity, new();
        IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters);
        int ExecuteSqlCommand(string sql, int? timeout = null, params object[] parameters);
    }
}
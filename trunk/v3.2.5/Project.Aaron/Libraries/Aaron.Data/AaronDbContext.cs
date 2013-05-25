using System;
using System.Configuration;
using System.Reflection;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Data.SqlClient;
using System.Data.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aaron.Core;
using Aaron.Core.Data;
using Aaron.Core.Infrastructure;
using Aaron.Data.Mapping;
using Aaron.Data.Mapping.Configuration;

namespace Aaron.Data
{
    public class AaronDbContext : DbContext, IContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AaronDbContext"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        //public AaronDbContext(DataSettings settings)
        //    : base(InitialDbConnection(settings.DataConnectionString), false)
        //{

        //}

        public AaronDbContext(DataSettings settings)
            : base(settings.DataConnectionString)
        {

        }

        /// <summary>
        /// Initials the db connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        //private static DbConnection InitialDbConnection(string connectionString)
        //{
        //    var sqlConn = new SqlConnection(connectionString);
        //    var efDbConn = new EFProfiledDbConnection(sqlConn, MiniProfiler.Current);
            
        //    return efDbConn;
        //}

        private void InitializeDbMapping(DbModelBuilder modelBuilder)
        {
            var _typeFinder = IoC.Resolve<ITypeFinder>();

            //dynamically load all configuration and system configuration
            var configTypes = _typeFinder.GetAssemblies();
            foreach (var configType in configTypes)
            {
                var typesToRegister = configType.GetTypes()
                    .Where(type => !String.IsNullOrEmpty(type.Namespace))
                .Where(type => type.BaseType != null && type.BaseType.IsGenericType && 
                    !type.IsGenericTypeDefinition &&
                    (type.BaseType.GetGenericTypeDefinition() == typeof(BaseEntityTypeConfiguration<,>) ||
                    (type.BaseType.GetGenericTypeDefinition() == typeof(SEOEntityTypeConfiguration<,>)) ||
                    (type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>))));

                if (typesToRegister == null) continue;

                foreach (var type in typesToRegister)
                {
                    dynamic configurationInstance = Activator.CreateInstance(type);
                    modelBuilder.Configurations.Add(configurationInstance);
                }
            }
        }

        /// <summary>
        /// This method is called when the model for a derived context has been initialized, but
        /// before the model has been locked down and used to initialize the context.  The default
        /// implementation of this method does nothing, but it can be overridden in a derived class
        /// such that the model can be further configured before it is locked down.
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            InitializeDbMapping(modelBuilder);
            
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Sets this instance.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns></returns>
        public new IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        /// <summary>
        /// Entries the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public new DbEntityEntry Entry<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            return base.Entry(entity);
        }

        public int ExecuteSqlCommand(string sql, int? timeout = null, params object[] parameters)
        {
            int? previousTimeout = null;
            if (timeout.HasValue)
            {
                //store previous timeout
                previousTimeout = ((IObjectContextAdapter)this).ObjectContext.CommandTimeout;
                ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = timeout;
            }

            var result = this.Database.ExecuteSqlCommand(sql, parameters);

            if (timeout.HasValue)
            {
                //Set previous timeout back
                ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = previousTimeout;
            }

            //return result
            return result;
        }
    }
}
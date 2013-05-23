﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Transactions;

namespace Aaron.Data.Initializers
{
    public class ExecuteCustomCommands<TContext> : IDatabaseInitializer<TContext> where TContext : DbContext
    {
        private readonly string[] _customCommands;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="tablesToValidate">A list of existing table names to validate; null to don't validate table names</param>
        /// <param name="customCommands">A list of custom commands to execute</param>
        public ExecuteCustomCommands(string[] customCommands)
            : base()
        {
            this._customCommands = customCommands;
        }
        public void InitializeDatabase(TContext context)
        {
            bool dbExists;
            using (new TransactionScope(TransactionScopeOption.Suppress))
            {
                dbExists = context.Database.Exists();
            }
            if (dbExists)
            {
                bool createTables = false;
                
                //check whether tables are already created
                int numberOfTables = 0;
                foreach (var t1 in context.Database.SqlQuery<int>("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE table_type = 'BASE TABLE' "))
                    numberOfTables = t1;

                createTables = numberOfTables == 0;

                if (createTables)
                {
                    //create all tables
                    var dbCreationScript = ((IObjectContextAdapter)context).ObjectContext.CreateDatabaseScript();
                    context.Database.ExecuteSqlCommand(dbCreationScript);

                    //Seed(context);
                    context.SaveChanges();

                }

                if (_customCommands != null && _customCommands.Length > 0)
                {
                    foreach (var command in _customCommands)
                        context.Database.ExecuteSqlCommand(command);
                }
            }
            else
            {
                throw new ApplicationException("No database instance");
            }
        }
    }
}
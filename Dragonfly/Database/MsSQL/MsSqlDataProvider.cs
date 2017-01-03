using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Dragonfly.Core;
using System.Data.Entity.Core.EntityClient;
using System.Configuration;
using System.Data.SqlClient;

namespace Dragonfly.Database.MsSQL
{
    internal class MsSqlDataProvider: IDataBaseProvider
    {
        /// <summary>Method create and open context for database.</summary>
        /// <param name="accessConfigurations">Parameters to database connect.</param>
        /// <returns>Created context. null if fail.</returns>
        public DbContext GetNewContext(DatabaseAccessConfiguration accessConfigurations)
        {
            DbContext context = null;
            EntityConnectionStringBuilder connection = UpdateConnectionParameters(accessConfigurations);

            try
            {
                context = new DragonflyEntities(connection.ToString());
                context.Database.Connection.Open();
            }
            catch (Exception ex)
            {
                if (context != null)
                    context.Dispose();
                return null;
            }
            return context;
        }

        private EntityConnectionStringBuilder UpdateConnectionParameters(
            DatabaseAccessConfiguration accessConfigurations)
        {
            var connection = new EntityConnectionStringBuilder(
                ConfigurationManager.ConnectionStrings[nameof(DragonflyEntities)].ConnectionString);
            var builder = new SqlConnectionStringBuilder(connection.ProviderConnectionString);

            builder.ApplicationName = "Dragonfly server";
            if (!string.IsNullOrWhiteSpace(accessConfigurations.ServerName))
                builder.DataSource = accessConfigurations.ServerName;
            if (!string.IsNullOrWhiteSpace(accessConfigurations.DbName))
                builder.InitialCatalog = accessConfigurations.DbName;
            if (!string.IsNullOrWhiteSpace(accessConfigurations.UserName))
                builder.UserID = accessConfigurations.UserName;
            if (!string.IsNullOrWhiteSpace(accessConfigurations.Password))
                builder.Password = accessConfigurations.Password;

            connection.ProviderConnectionString = builder.ToString();
            return connection;
        }
    }
}
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
    internal class MsSqlDataProvider : IDataBaseProvider
    {
        DragonflyEntities _Context = null;
        public DbContext Context { get { return _Context; } }

        public void Dispose()
        {
            lock (_Context)
                if (_Context != null)
                {
                    _Context.Dispose();
                    _Context = null;
                }
        }

        public bool InitializeConnection(DatabaseAccessConfiguration accessConfigurations)
        {
            EntityConnectionStringBuilder connection = UpdateConnectionParameters(accessConfigurations);

            try
            {
                _Context = new DragonflyEntities(connection.ToString());
                _Context.Database.Connection.Open();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
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
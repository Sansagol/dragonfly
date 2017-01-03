using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Dragonfly.Core;
using System.Data.Entity.Core.EntityClient;
using System.Configuration;
using System.Data.SqlClient;
using Dragonfly.Core.Settings;

namespace Dragonfly.Database.MsSQL
{
    internal class MsSqlDataProvider : IDataBaseProvider
    {
        DragonflyEntities _Context = null;
        public DbContext Context { get { return _Context; } }

        public bool AddUser(string login, string password)
        {
            throw new NotImplementedException();
        }

        public bool CheckUserCredentials(string login, string password)
        {
            if (_Context == null)
                return false;
            User usr = (from user in _Context.User
                        where user.Name.Equals(login)
                        select user).FirstOrDefault();
            if (usr != null
                && !usr.Is_Ldap_User
                && usr.Password.CompareTo(password) == 0)
                return true;
            return false;
        }

        /// <summary>Method create and open context for database.</summary>
        /// <param name="accessConfigurations">Parameters to database connect.</param>
        /// <returns>Created context. null if fail.</returns>
        public DbContext Initizlize(DatabaseAccessConfiguration accessConfigurations)
        {
            EntityConnectionStringBuilder connection = UpdateConnectionParameters(accessConfigurations);

            try
            {
                _Context = new DragonflyEntities(connection.ToString());
                _Context.Database.Connection.Open();
            }
            catch (Exception ex)
            {
                if (_Context != null)
                    _Context.Dispose();
                return null;
            }
            return _Context;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dragonfly.Core.Settings;
using System.Data.Entity.Core.EntityClient;
using System.Configuration;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Dragonfly.Database.MsSQL.LowLevel
{
    public class DBContextGenerator : IDBContextGenerator
    {
        DatabaseAccessConfiguration _DbConfig = null;

        public DBContextGenerator(DatabaseAccessConfiguration accessConfigurations)
        {
            if (accessConfigurations == null)
                throw new ArgumentNullException(nameof(accessConfigurations));
            _DbConfig = Clone(accessConfigurations);
        }

        /// <summary>
        /// Method generate a new db context.
        /// </summary>
        /// <param name="accessConfigurations">Settings to db access.</param>
        /// <returns>Db context.</returns>
        /// <exception cref="DbInitializationException"/> 
        public DragonflyEntities GenerateContext()
        {
            DragonflyEntities context = null;
            EntityConnectionStringBuilder connection = UpdateConnectionParameters(_DbConfig);
            try
            {
                context = new DragonflyEntities(connection.ToString());
                context.Database.Connection.Open();
            }
            catch (Exception ex)
            {
                if (context != null)
                    context.Dispose();
                throw new DbInitializationException(ex.Message);
            }
            return context;
        }

        private EntityConnectionStringBuilder UpdateConnectionParameters(
            DatabaseAccessConfiguration accessConfigurations)
        {
            var connection = new EntityConnectionStringBuilder(
                ConfigurationManager.ConnectionStrings[nameof(DragonflyEntities)].ConnectionString);
            var builder = new SqlConnectionStringBuilder(connection.ProviderConnectionString)
            {
                ApplicationName = "Dragonfly server"
            };
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

        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
                throw new ArgumentException("The type must be serializable.", "source");
            if (Object.ReferenceEquals(source, null))
                return default(T);

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}
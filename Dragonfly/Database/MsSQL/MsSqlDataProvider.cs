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
using System.Security.Cryptography;
using System.Data.Entity.Validation;
using Dragonfly.Models;

namespace Dragonfly.Database.MsSQL
{
    internal class MsSqlDataProvider : IDataBaseProvider
    {
        DragonflyEntities _Context = null;
        public DbContext Context { get { return _Context; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="InsertDbDataException">Error on user adding.</exception> 
        public bool AddUser(LogUpModel userRegisterData)
        {
            if (string.IsNullOrWhiteSpace(userRegisterData.Login) ||
                 string.IsNullOrWhiteSpace(userRegisterData.Password) ||
                 string.IsNullOrWhiteSpace(userRegisterData.EMail))
                return false;
            string hashedPassword = EncryptAsRfc2898(userRegisterData.Password);
            User usr = new User()
            {
                Name = userRegisterData.Login,
                Password = hashedPassword,
                Date_Creation = DateTime.Now,
                E_mail = userRegisterData.EMail
            };
            _Context.User.Add(usr);
            try
            {
                _Context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                _Context.User.Remove(usr);
                List<ValidationError> validationErrors = new List<ValidationError>();
                foreach (var validResult in ex.EntityValidationErrors)
                {
                    validationErrors.AddRange(validResult.ValidationErrors.Select(
                        v => new ValidationError(v.PropertyName, v.ErrorMessage)));
                }
                throw new InsertDbDataException(validationErrors);
            }
            return true;
        }

        private string EncryptAsRfc2898(string password)
        {
            int iterationsCount = 5000;
            byte[] salt = null;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[20]);

            var passwordFunc = new Rfc2898DeriveBytes(password, salt, iterationsCount);
            byte[] passwordHash = passwordFunc.GetBytes(20);

            //Salt + hash (20+20).
            byte[] hashBytes = new byte[20 + 20];
            Array.Copy(salt, 0, hashBytes, 0, 20);
            Array.Copy(passwordHash, 0, hashBytes, 20, 20);
            return Convert.ToBase64String(hashBytes);
        }

        public bool CheckUserCredentials(string login, string password)
        {
            if (_Context == null)
                return false;
            User usr = (from user in _Context.User
                        where user.Name.Equals(login)
                        select user).FirstOrDefault();
            if (usr != null)
            {
                if (!usr.Is_Ldap_User
                   && usr.Password.CompareTo(password) == 0)
                    return true;
            }
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
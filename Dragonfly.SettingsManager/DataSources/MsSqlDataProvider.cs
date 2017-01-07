using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Data.Entity.Core.EntityClient;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Data.Entity.Validation;
using System.Threading;
using System.Data.Entity.Infrastructure;
using Dragonfly.SettingsManager.DataSources;
using Dragonfly.SettingsLib;

namespace Dragonfly.SettingsManager
{
    internal class MsSqlDataProvider
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
        public bool AddUser(string login, string password, string eMail)
        {
            if (string.IsNullOrWhiteSpace(login) ||
                 string.IsNullOrWhiteSpace(password) ||
                 string.IsNullOrWhiteSpace(eMail))
                return false;
            CheckExistingUsers(login, eMail);

            string hashedPassword = EncryptAsRfc2898(password);
            User usr = new User()
            {
                Login = login,
                Password = hashedPassword,
                Date_Creation = DateTime.Now,
                E_mail = eMail
            };
            _Context.User.Add(usr);
            try
            {
                _Context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                _Context.User.Remove(usr);
                Dictionary<string, string> validationErrors = new Dictionary<string, string>();
                foreach (var validResult in ex.EntityValidationErrors)
                {
                    foreach (var err in validResult.ValidationErrors)
                        validationErrors.Add(err.PropertyName, err.ErrorMessage);
                }
                throw new InsertDbDataException(validationErrors);
            }
            catch (DbUpdateException ex)
            {
                _Context.User.Remove(usr);
            }
            return true;
        }

        /// <exception cref="InsertDbDataException">User exists.</exception> 
        private void CheckExistingUsers(string login, string eMail)
        {
            Dictionary<string, string> validationErrors = new Dictionary<string, string>();
            int existsUsersCount = (from u in _Context.User
                                    where u.Login.Equals(login)
                                    select u).Count();
            if (existsUsersCount > 0)
            {
                validationErrors.Add("Login", "The user with  the specified login exists.");
            }
            else
            {
                existsUsersCount = (from u in _Context.User
                                    where u.E_mail.Equals(eMail)
                                    select u).Count();
                if (existsUsersCount > 0)
                    validationErrors.Add("Email", "The user with  the specified Email exists.");
            }
            if (validationErrors.Count > 0)
                throw new InsertDbDataException(validationErrors);
        }

        private string EncryptAsRfc2898(string password)
        {
            byte[] salt = null;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[20]);

            byte[] passwordHash = GetHashOfPassword(password, salt);

            //Salt + hash (20+20).
            byte[] hashBytes = new byte[20 + 20];
            Array.Copy(salt, 0, hashBytes, 0, 20);
            Array.Copy(passwordHash, 0, hashBytes, 20, 20);
            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>Method generate a byte array with hash of password.</summary>
        /// <param name="password">Password to hash.</param>
        /// <param name="salt">Salt.</param>
        /// <returns>Bytes of hash.</returns>
        private static byte[] GetHashOfPassword(string password, byte[] salt)
        {
            int iterationsCount = 5000;
            var passwordFunc = new Rfc2898DeriveBytes(password, salt, iterationsCount);
            byte[] passwordHash = passwordFunc.GetBytes(20);
            return passwordHash;
        }

        /// <summary>Method check user credentials.</summary>
        /// <param name="login">User login or e-mail address</param>
        /// <param name="password">User password</param>
        /// <returns>Is user exists in the system.</returns>
        public bool CheckUserCredentials(string login, string password)
        {
            if (_Context == null)
                return false;
            User usr = (from user in _Context.User
                        where user.Login.Equals(login) ||
                              user.E_mail.Equals(login)
                        select user).FirstOrDefault();
            if (usr != null)
            {
                if (!usr.Is_Ldap_User)
                {
                    byte[] hashBytes = Convert.FromBase64String(usr.Password);
                    byte[] salt = new byte[20];
                    Array.Copy(hashBytes, 0, salt, 0, 20);
                    byte[] hashedInputPassword = GetHashOfPassword(password, salt);
                    bool isTruePassword = true;
                    for (int i = 0; i < 20; i++)
                        if (hashedInputPassword[i] != hashBytes[i + 20])
                            isTruePassword = false;
                    return isTruePassword;
                }
            }

            Thread.Sleep(150);
            return false;
        }

        /// <summary>Method create and open context for database.</summary>
        /// <param name="accessConfigurations">Parameters to database connect.</param>
        /// <returns>Created context. null if fail.</returns>
        public DbContext Initizlize(DatabaseConfig accessConfigurations)
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
            DatabaseConfig accessConfigurations)
        {
            var connection = new EntityConnectionStringBuilder(
                ConfigurationManager.ConnectionStrings[nameof(DragonflyEntities)].ConnectionString);
            var builder = new SqlConnectionStringBuilder(connection.ProviderConnectionString);

            builder.ApplicationName = "Dragonfly server";
            if (!string.IsNullOrWhiteSpace(accessConfigurations.DbAddress))
                builder.DataSource = accessConfigurations.DbAddress;
            if (!string.IsNullOrWhiteSpace(accessConfigurations.DbName))
                builder.InitialCatalog = accessConfigurations.DbName;
            if (!string.IsNullOrWhiteSpace(accessConfigurations.DefaultUserName))
                builder.UserID = accessConfigurations.DefaultUserName;
            if (!string.IsNullOrWhiteSpace(accessConfigurations.DefaultUserPassword))
                builder.Password = accessConfigurations.DefaultUserPassword;

            connection.ProviderConnectionString = builder.ToString();
            return connection;
        }

        public void DeleteUser(string login = "",
            string eMail = "",
            int userId = 0)
        {
            var query = _Context.User.Select(u => u);
            if (!string.IsNullOrWhiteSpace(login))
                query = query.Where(u => u.Login.Equals(login));
            if (!string.IsNullOrWhiteSpace(eMail))
                query = query.Where(u => u.E_mail.Equals(eMail));
            if (userId > 0)
                query = query.Where(u => u.ID_User == userId);

            User foundUser = query.FirstOrDefault();
            if (foundUser != null)
                try
                {
                    _Context.User.Remove(foundUser);
                    _Context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Error on user deleting", ex);
                }
        }
    }
}
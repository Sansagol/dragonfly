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
using System.Threading;
using System.Data.Entity.Infrastructure;
using Dragonfly.Models.Projects;

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
            CheckExistingUsers(userRegisterData);

            string hashedPassword = EncryptAsRfc2898(userRegisterData.Password);
            User usr = new User()
            {
                Login = userRegisterData.Login,
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
            catch (DbUpdateException ex)
            {
                _Context.User.Remove(usr);
            }
            return true;
        }

        /// <exception cref="InsertDbDataException">User exists.</exception> 
        private void CheckExistingUsers(LogUpModel userRegisterData)
        {
            List<ValidationError> validationErrors = new List<ValidationError>();
            int existsUsersCount = (from u in _Context.User
                                    where u.Login.Equals(userRegisterData.Login)
                                    select u).Count();
            if (existsUsersCount > 0)
            {
                validationErrors.Add(new ValidationError("Login", "The user with  the specified login exists."));
            }
            else
            {
                existsUsersCount = (from u in _Context.User
                                    where u.E_mail.Equals(userRegisterData.EMail)
                                    select u).Count();
                if (existsUsersCount > 0)
                    validationErrors.Add(new ValidationError("Email", "The user with  the specified Email exists."));
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
        /// <exception cref="InvalidOperationException"/>
        public bool CheckUserCredentials(string login, string password)
        {
            if (_Context == null)
                return false;
            User usr = null;
            try
            {
                usr = (from user in _Context.User
                       where user.Login.Equals(login) ||
                             user.E_mail.Equals(login)
                       select user).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Database is down.", ex);
            }
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

        public UserModel GetUserById(int userId)
        {
            UserModel model = null;
            User usr = null;
            usr = SelectUser(userId);
            if (usr != null)
            {
                model = new UserModel()
                {
                    Id = usr.ID_User,
                    Login = usr.Login,
                    Name = usr.Name
                };
            }
            return model;
        }

        private User SelectUser(int userId)
        {
            User usr;
            try
            {
                usr = (from user in _Context.User
                       where user.ID_User == userId
                       select user).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Database is down.", ex);
            }

            return usr;
        }

        public UserModel GetUserByLoginMail(string userLogin)
        {
            UserModel model = null;
            User usr = null;
            try
            {
                usr = (from user in _Context.User
                       where user.Login.Equals(userLogin) ||
                             user.E_mail.Equals(userLogin)
                       select user).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Database is down.", ex);
            }
            if (usr != null)
            {
                model = new UserModel()
                {
                    Id = usr.ID_User,
                    Login = usr.Login,
                    Name = usr.Name
                };
            }
            return model;
        }

        public void CreateProject(ProjectModel newProject)
        {
            CheckProjectModelArgs(newProject);

            User user = SelectUser(newProject.UserOwnerId);
            if (user == null)
                throw new InvalidOperationException(
                    $"User with id: \'{newProject.UserOwnerId}\' not found.");

            Project proj = new Project()
            {
                Name = newProject.ProjectName,
                Date_Create = DateTime.Now
            };
            SaveNewProjectInDB(proj);

            Project_Role prRole = (from pr in _Context.Project_Role
                                   where pr.Is_Admin
                                   select pr).FirstOrDefault();

            User_Project usProj = new User_Project()
            {
                ID_Project = proj.ID_Project,
                ID_User = user.ID_User,
                ID_Project_Role = prRole.ID_Project_Role
            };

            try
            {
                _Context.User_Project.Add(usProj);
            }
            catch (DbEntityValidationException ex)
            {
                _Context.User_Project.Remove(usProj);
                List<ValidationError> validationErrors = new List<ValidationError>();
                foreach (var validResult in ex.EntityValidationErrors)
                {
                    validationErrors.AddRange(validResult.ValidationErrors.Select(
                        v => new ValidationError(v.PropertyName, v.ErrorMessage)));
                }
                DeleteProject(proj.ID_Project);
                throw new InsertDbDataException(validationErrors);
            }
            catch (DbUpdateException ex)
            {
                _Context.User_Project.Remove(usProj);
                DeleteProject(proj.ID_Project);
                throw new InvalidOperationException("Unable to save user-project relation", ex);
            }
        }

        /// <summary>Method save a new project in the database.</summary>
        /// <param name="project">Project to save</param>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="InsertDbDataException"/>
        private void SaveNewProjectInDB(Project project)
        {
            try
            {
                _Context.Project.Add(project);
            }
            catch (DbEntityValidationException ex)
            {
                _Context.Project.Remove(project);
                List<ValidationError> validationErrors = new List<ValidationError>();
                foreach (var validResult in ex.EntityValidationErrors)
                {
                    validationErrors.AddRange(validResult.ValidationErrors.Select(
                        v => new ValidationError(v.PropertyName, v.ErrorMessage)));
                }
                throw new InsertDbDataException(validationErrors);
            }
            catch (DbUpdateException ex)
            {
                _Context.Project.Remove(project);
                throw new InvalidOperationException("Unable to save project", ex);
            }
        }

        private void CheckProjectModelArgs(ProjectModel newProject)
        {
            if (newProject == null)
                throw new ArgumentNullException(nameof(newProject));
            if (string.IsNullOrWhiteSpace(newProject.ProjectName))
                throw new ArgumentException("Empty project name", nameof(newProject));
            if (newProject.UserOwnerId < 1)
                throw new ArgumentException("Project owner user wasn't set", nameof(newProject));
        }

        /// <summary>Method delete a project from a db.</summary>
        /// <param name="projectId">Id of project.</param>
        public void DeleteProject(decimal projectId)
        {
        }
    }
}
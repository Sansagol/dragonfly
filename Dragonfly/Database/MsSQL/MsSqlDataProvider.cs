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
        /// <exception cref="DbInitializationException">Error on database initialization.</exception>
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
                throw new DbInitializationException(ex.Message);
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
            usr = SelectUserById(userId);
            if (usr != null)
            {
                model = CreateAUserModel(usr);
            }
            return model;
        }

        private static UserModel CreateAUserModel(User usr)
        {
            return new UserModel()
            {
                Id = usr.ID_User,
                Login = usr.Login,
                Name = usr.Name,
                EMail = usr.E_mail
            };
        }

        private User SelectUserById(decimal userId)
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
                model = CreateAUserModel(usr);
            }
            return model;
        }

        /// <summary>Method create new project.</summary>
        /// <param name="newProject">Model of project to save.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="InsertDbDataException"/>
        public void CreateProject(ProjectModel newProject)
        {
            CheckProjectModelArgs(newProject);

            IEnumerable<User> users = GetUsersByIds(newProject.UserIds);

            Project proj = new Project()
            {
                Name = newProject.ProjectName,
                Date_Create = DateTime.Now,
                Description = newProject.Description
            };
            SaveNewProjectInDB(proj);
            newProject.ProjectId = proj.ID_Project;

            Project_Role prRole = (from pr in _Context.Project_Role
                                   where pr.Is_Admin
                                   select pr).FirstOrDefault();
            if (prRole == null)
                throw new InvalidOperationException("An admin role for project not found.");

            foreach (User user in users)
            {
                User_Project usProj = new User_Project()
                {
                    ID_Project = proj.ID_Project,
                    ID_User = user.ID_User,
                    ID_Project_Role = prRole.ID_Project_Role
                };
                KeepUserProject(proj.ID_Project, usProj);
            }
        }

        /// <summary>
        /// Method check a projectmodel the args for correctness.
        /// If error args will found then will thrown an exception.
        /// </summary>
        /// <param name="newProject">Args to check.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        private void CheckProjectModelArgs(ProjectModel newProject)
        {
            if (newProject == null)
                throw new ArgumentNullException(nameof(newProject));
            if (string.IsNullOrWhiteSpace(newProject.ProjectName))
                throw new ArgumentException("Empty project name", nameof(newProject));
            if (newProject.UserIds.Count < 1)
                throw new ArgumentException("Project owner user wasn't set", nameof(newProject));
        }

        /// <summary>Method retrieve a users from th DB by it ids.</summary>
        /// <param name="userIds">Ids of users to retrieve.</param>
        /// <returns>List with retrieving results.</returns>
        /// <exception cref="InvalidOperationException">No users found.</exception>
        private IEnumerable<User> GetUsersByIds(IEnumerable<decimal> userIds)
        {
            List<User> users = new List<User>();
            foreach (var userId in userIds)
            {
                User usr = SelectUserById(userId);
                if (usr != null)
                    users.Add(usr);
            }
            if (users.Count < 1)
                throw new InvalidOperationException("Users for project not found.");
            return users;
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
                _Context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                _Context.Project.Remove(project);
                List<ValidationError> validationErrors = RetrieveValidationsErrors(ex);
                throw new InsertDbDataException(validationErrors);
            }
            catch (DbUpdateException ex)
            {
                _Context.Project.Remove(project);
                throw new InvalidOperationException("Unable to save project", ex);
            }
        }

        private static List<ValidationError> RetrieveValidationsErrors(DbEntityValidationException ex)
        {
            List<ValidationError> validationErrors = new List<ValidationError>();
            foreach (var validResult in ex.EntityValidationErrors)
            {
                validationErrors.AddRange(validResult.ValidationErrors.Select(
                    v => new ValidationError(v.PropertyName, v.ErrorMessage)));
            }

            return validationErrors;
        }

        private void KeepUserProject(decimal projectId, User_Project usProj)
        {
            try
            {
                _Context.User_Project.Add(usProj);
                _Context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                _Context.User_Project.Remove(usProj);
                List<ValidationError> validationErrors = RetrieveValidationsErrors(ex);
                DeleteProject(projectId);
                throw new InsertDbDataException(validationErrors);
            }
            catch (DbUpdateException ex)
            {
                _Context.User_Project.Remove(usProj);
                DeleteProject(projectId);
                throw new InvalidOperationException("Unable to save user-project relation", ex);
            }
        }

        /// <summary>Method delete a project from a db.</summary>
        /// <param name="projectId">Id of project.</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        public void DeleteProject(decimal projectId)
        {
            if (projectId < 1)
                throw new ArgumentException("Project id can't be less than 1", nameof(projectId));

            try
            {
                Project proj = (from p in _Context.Project
                                where p.ID_Project == projectId
                                select p).FirstOrDefault();
                if (proj != null)
                    _Context.Project.Remove(proj);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to delete project.", ex);
            }
        }

        public ProjectModel GetProjectById(decimal projectId)
        {
            if (projectId < 1)
                throw new ArgumentException("Project id can't be less than 1", nameof(projectId));
            ProjectModel model = null;
            try
            {
                Project proj = (from p in _Context.Project
                                where p.ID_Project == projectId
                                select p).FirstOrDefault();
                if (proj != null)
                    model = new ProjectModel(this)
                    {
                        Description = proj.Description,
                        ProjectName = proj.Name,
                        UserIds = proj.User_Project.Select(u => u.ID_User).ToList()
                    };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to retrieve a project.", ex);
            }
            return model;
        }

        public IEnumerable<ProjectModel> GetProjects(int offset, int count)
        {
            if (offset < 0) offset = 0;
            if (count < 0) count = 1;

            var projects = (from proj in _Context.Project
                            select proj).OrderBy(p => p.ID_Project).Skip(offset).Take(count);
            List<ProjectModel> projectModels = new List<ProjectModel>();
            foreach (var project in projects)
            {
                try
                {
                    projectModels.Add(project.ToProjectModel(this));
                }
                catch (Exception ex)
                {//TODO log exception
                }
            }
            return projectModels;
        }

        /// <summary>
        /// Method check an access token to correct and that is not expired.
        /// </summary>
        /// <param name="userId">Current logged user, which token are presented.</param>
        /// <param name="token">Token to check.</param>
        /// <returns>True - if token is correct. False - otherwise.</returns>
        public bool CheckAccessToken(decimal userId, string token)
        {
            DateTime now = DateTime.UtcNow.Date;
            DeleteOldUserAccessTokens(userId, now);
            var userAccesses = (from userAccess in _Context.User_Access
                                where userAccess.Date_Expiration.Date >= now &&
                                      userAccess.Access_Token.Equals(token)
                                select userAccess).OrderByDescending(u => u.Date_Expiration);
            //Delete multiple equals access tokens
            if (userAccesses.Count() > 1)
            {//TODO: write this to log as error
                _Context.User_Access.RemoveRange(userAccesses.Skip(1));
            }
            return userAccesses.Count() >= 1;
        }

        private void DeleteOldUserAccessTokens(decimal userId, DateTime now)
        {
            var userAccesses = (from userAccess in _Context.User_Access
                                where userAccess.ID_User == userId &&
                                    userAccess.Date_Expiration.Date < now.Date
                                select userAccess);
            if (userAccesses.Count() > 0)
                _Context.User_Access.RemoveRange(userAccesses);
            try
            {
                _Context.SaveChanges();
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>Method create an access token for current user.</summary>
        /// <param name="userId">Id of user to create token.</param>
        /// <returns>Created access toker, or null if was error.</returns>
        /// <exception cref="InsertDbDataException"/>
        /// <exception cref="InvalidOperationException"/>
        public string CreateAccessToken(decimal userId)
        {
            string token = string.Empty;
            DateTime now = DateTime.UtcNow;

            DeleteOldUserAccessTokens(userId, now);
            User currentUser = SelectUserById(userId);
            if (currentUser != null)
            {
                token = GenerateAccessToken(now);
                User_Access access = new User_Access()
                {
                    Access_Token = token,
                    Date_Creation = now,
                    Date_Expiration = now.AddDays(1),
                    ID_User = currentUser.ID_User
                };
                try
                {
                    _Context.User_Access.Add(access);
                    _Context.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    _Context.User_Access.Remove(access);
                    List<ValidationError> validationErrors = RetrieveValidationsErrors(ex);
                    throw new InsertDbDataException(validationErrors);
                }
                catch (DbUpdateException ex)
                {
                    _Context.User_Access.Remove(access);
                    throw new InvalidOperationException("Unable to save user-access", ex);
                }
            }
            return token;
        }

        private string GenerateAccessToken(DateTime now)
        {
            byte[] time = BitConverter.GetBytes(now.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            return Convert.ToBase64String(time.Concat(key).ToArray());
        }
    }
}
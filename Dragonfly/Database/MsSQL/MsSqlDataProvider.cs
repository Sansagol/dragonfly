﻿using System;
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
using Dragonfly.Database.MsSQL.LowLevel;
using Dragonfly.Database.Providers;
using Dragonfly.Models.UserRoleSystem;
using Dragonfly.Database.Entities;
using Dragonfly.Database.MsSQL.Converters;

namespace Dragonfly.Database.MsSQL
{
    internal class MsSqlDataProvider : DataProvider, IDataBaseProvider
    {
        #region Low level interfaces
        IUserDBDataManager _UserManager = null;
        #endregion

        DatabaseAccessConfiguration _DatabaseConfig = null;

        public MsSqlDataProvider(
            IUserDBDataManager userDbDataManage,
            IDBContextGenerator contextgenerator) :
            base(contextgenerator)
        {
            if (userDbDataManage == null)
                throw new ArgumentNullException(nameof(userDbDataManage));

            _UserManager = userDbDataManage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="InsertDbDataException">Error on user adding.</exception> 
        public decimal AddUser(SignUpModel userRegisterData)
        {
            if (string.IsNullOrWhiteSpace(userRegisterData.Login) ||
                 string.IsNullOrWhiteSpace(userRegisterData.Password) ||
                 string.IsNullOrWhiteSpace(userRegisterData.EMail))
                return 0;
            CheckExistingUsers(userRegisterData);

            string hashedPassword = EncryptAsRfc2898(userRegisterData.Password);
            User usr = new User()
            {
                Login = userRegisterData.Login,
                Password = hashedPassword,
                Date_Creation = DateTime.Now,
                E_mail = userRegisterData.EMail
            };

            using (var context = _ContextGenerator.GenerateContext())
            {
                context.User.Add(usr);
                try
                {
                    context.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    context.User.Remove(usr);
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
                    context.User.Remove(usr);
                    throw new InsertDbDataException($"Update entity error: {ex.Message}");
                }
            }
            return usr.ID_User;
        }

        /// <exception cref="InsertDbDataException">User exists.</exception> 
        private void CheckExistingUsers(SignUpModel userRegisterData)
        {
            List<ValidationError> validationErrors = new List<ValidationError>();
            using (var context = _ContextGenerator.GenerateContext())
            {
                int existsUsersCount = (from u in context.User
                                        where u.Login.Equals(userRegisterData.Login)
                                        select u).Count();
                if (existsUsersCount > 0)
                {
                    validationErrors.Add(new ValidationError("Login", "The user with  the specified login exists."));
                }
                else
                {
                    existsUsersCount = (from u in context.User
                                        where u.E_mail.Equals(userRegisterData.EMail)
                                        select u).Count();
                    if (existsUsersCount > 0)
                        validationErrors.Add(new ValidationError("Email", "The user with  the specified Email exists."));
                }
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
            User usr = null;
            try
            {
                using (var context = _ContextGenerator.GenerateContext())
                {
                    usr = (from user in context.User
                           where user.Login.Equals(login) ||
                                 user.E_mail.Equals(login)
                           select user).FirstOrDefault();
                }
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

        /// <summary>Method create new project.</summary>
        /// <param name="newProject">Model of project to save.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="InsertDbDataException"/>
        public void CreateProject(ProjectModel newProject)
        {
            CheckProjectModelArgs(newProject);

            IEnumerable<User> users = GetUsersByIds(newProject.Users.Select(u => u.Id));
            Project proj = newProject.ProjectDetails.ToDbProject();

            using (var context = _ContextGenerator.GenerateContext())
            {
                SaveNewProjectInDB(proj, context);
                newProject.ProjectDetails.Id = proj.ID_Project;
                //Hardcoded load the admin
                Project_Role prRole = (from pr in context.Project_Role
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
                    KeepUserProject(proj.ID_Project, usProj, context);
                }
            }
        }

        public void CreateProject(EProject newProject)
        {
            CheckTheEProjectData(newProject);
            Project proj = newProject.ToDbProject();
            if (proj.Date_Create == default(DateTime))
                proj.Date_Create = DateTime.Now;

            using (var context = _ContextGenerator.GenerateContext())
            {
                SaveNewProjectInDB(proj, context);
                newProject.Id = proj.ID_Project;
                //Hardcoded load the admin
                Project_Role prRole = (from pr in context.Project_Role
                                       where pr.Is_Admin
                                       select pr).FirstOrDefault();
                if (prRole == null)
                    throw new InvalidOperationException("An admin role for project not found.");

                foreach (decimal userId in newProject.UserIds)
                {
                    User_Project usProj = new User_Project()
                    {
                        ID_Project = proj.ID_Project,
                        ID_User = userId,
                        ID_Project_Role = prRole.ID_Project_Role
                    };
                    KeepUserProject(proj.ID_Project, usProj, context);
                }
            }
        }

        private static void CheckTheEProjectData(EProject newProject)
        {
            if (newProject == null)
                throw new ArgumentNullException(nameof(newProject));
            if (string.IsNullOrWhiteSpace(newProject.ProjectName))
                throw new ArgumentException("Empty project name", nameof(newProject));
            if (newProject.UserIds.Count < 1)
                throw new ArgumentException("Project owner user wasn't set", nameof(newProject));
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
            if (string.IsNullOrWhiteSpace(newProject.ProjectDetails.ProjectName))
                throw new ArgumentException("Empty project name", nameof(newProject));
            if (newProject.Users.Count < 1)
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
                User usr = _UserManager.GetUserById(userId);
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
        private void SaveNewProjectInDB(Project project, DragonflyEntities context)
        {
            try
            {
                context.Project.Add(project);
                context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                context.Project.Remove(project);
                List<ValidationError> validationErrors = ex.RetrieveValidationsErrors();
                throw new InsertDbDataException(validationErrors);
            }
            catch (DbUpdateException ex)
            {
                context.Project.Remove(project);
                throw new InvalidOperationException("Unable to save project", ex);
            }
        }

        private void KeepUserProject(decimal projectId, User_Project usProj, DragonflyEntities context)
        {
            try
            {
                context.User_Project.Add(usProj);
                context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                context.User_Project.Remove(usProj);
                List<ValidationError> validationErrors = ex.RetrieveValidationsErrors();
                DeleteProject(projectId);
                throw new InsertDbDataException(validationErrors);
            }
            catch (DbUpdateException ex)
            {
                context.User_Project.Remove(usProj);
                DeleteProject(projectId);
                throw new InvalidOperationException("Unable to save user-project relation", ex);
            }
        }

        public void SaveProject(EProject project)
        {
            CheckTheEProjectData(project);

            using (var context = _ContextGenerator.GenerateContext())
            {
                Project proj = RetrieveProjectById(project.Id, context);
                if (proj == null)
                {
                    CreateProject(project);
                }
                else
                {
                    proj.Name = project.ProjectName;
                    proj.Description = project.Description;
                }
                SaveExistingProjectInDB(proj, context);
            }
        }

        private void SaveExistingProjectInDB(Project project, DragonflyEntities context)
        {
            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                context.Project.Remove(project);
                List<ValidationError> validationErrors = ex.RetrieveValidationsErrors();
                throw new InsertDbDataException(validationErrors);
            }
            catch (DbUpdateException ex)
            {
                context.Project.Remove(project);
                throw new InvalidOperationException("Unable to save project", ex);
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
                using (var context = _ContextGenerator.GenerateContext())
                {
                    Project proj = (from p in context.Project
                                    where p.ID_Project == projectId
                                    select p).FirstOrDefault();
                    if (proj != null)
                    {
                        context.Project.Remove(proj);
                        context.SaveChanges();
                    }

                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to delete project.", ex);
            }
        }

        public EProject GetProjectById(decimal projectId)
        {
            if (projectId < 1)
                throw new ArgumentException("Project id can't be less than 1", nameof(projectId));
            EProject model = null;
            try
            {
                using (var context = _ContextGenerator.GenerateContext())
                {
                    Project proj = RetrieveProjectById(projectId, context);
                    if (proj != null)
                        model = proj.ToEProject();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to retrieve a project.", ex);
            }
            return model;
        }

        private Project RetrieveProjectById(decimal projectId, DragonflyEntities context)
        {
            try
            {
                return (from p in context.Project
                        where p.ID_Project == projectId
                        select p).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to retrieve a project.", ex);
            }
        }

        public IEnumerable<EProject> GetProjects(int offset, int count)
        {//TODO get projects for a user
            if (offset < 0) offset = 0;
            if (count < 0) count = 1;
            using (var context = _ContextGenerator.GenerateContext())
            {
                var projects = (from proj in context.Project
                                select proj).OrderBy(p => p.ID_Project).Skip(offset).Take(count);
                List<EProject> projectModels = new List<EProject>();
                foreach (var project in projects)
                {
                    try
                    {
                        projectModels.Add(project.ToEProject());
                    }
                    catch (Exception ex)
                    {//TODO log exception
                    }
                }
                return projectModels;
            }
        }
    }
}
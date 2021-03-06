﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dragonfly.Models.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragonfly.Database.MsSQL;
using System.Data.Entity;
using Dragonfly.Tests;
using Dragonfly.Database;
using Dragonfly.Database.Providers;
using Dragonfly.Models.UserRoleSystem;
using Dragonfly.Database.Entities;

namespace Dragonfly.Models.Projects.Tests
{
    [TestClass()]
    public class ProjectModelTests
    {
        /// <summary>Common method to save a user.</summary>
        /// <param name="provider">DB provider.</param>
        /// <returns>Stored model of user.</returns>
        private static EUser SaveANewUser(IDataBaseProvider provider, IUserAccessProvider userProvider)
        {
            SignUpModel userData = new SignUpModel()
            {
                Login = "Test_user",
                EMail = "test@mail.mail",
                Password = "Test user password"
            };
            provider.AddUser(userData);
            EUser userModel = userProvider
                .GetUserByLoginMail("test@mail.mail");
            return userModel;
        }


        [TestMethod()]
        public void SuccessSaveProjectTest()
        {
            MsSqlFactory factory = new MsSqlFactory(Common.Connectionconfig);
            IDataBaseProvider provider = factory.CreateDBProvider();
            IUserAccessProvider usProvider = factory.CreateUserAccessProvider();
            UserModel userModel = null;
            ProjectModel projectModel = null;

            try
            {
                userModel = new UserModel(usProvider)
                {
                    UserDetails = SaveANewUser(provider, usProvider)
                };
                projectModel = new ProjectModel(provider)
                {
                    ProjectDetails = new EProject
                    {
                        ProjectName = "Test project name 1",
                        Description = "ProjectDescr"
                    }                   
                };
                projectModel.AddUserToProject(userModel.UserDetails.Id, 0);
                Assert.IsTrue(projectModel.SaveProject(), "Unable to save project");
                Assert.IsTrue(
                    projectModel.ProjectDetails.Id > 0,
                    $"Retuen the bad project ID: {projectModel.ProjectDetails.Id}");
                var context = ((DataProvider)provider).GenerateContext();
                Assert.IsNotNull(
                    context.User_Project.FirstOrDefault(
                        u => u.ID_Project == projectModel.ProjectDetails.Id),
                    "User_project is not saved");
            }
            finally
            {
                if (projectModel != null && projectModel.ProjectDetails.Id > 0)
                    provider.DeleteProject(projectModel.ProjectDetails.Id);
                using (var context = ((DataProvider)provider).GenerateContext())
                {
                    DeleteUserFromDB(context, userModel?.UserDetails?.Login, userModel?.UserDetails?.EMail);
                }
            }
        }

        [TestMethod()]
        public void NoSaveProjectWithoutUsersTest()
        {
            MsSqlFactory factory = new MsSqlFactory(Common.Connectionconfig);
            IDataBaseProvider provider = factory.CreateDBProvider();
            using (var context = ((DataProvider)provider).GenerateContext())
            {
                ProjectModel projectModel = null;
                try
                {
                    projectModel = new ProjectModel(provider)
                    {
                        ProjectDetails = new EProject
                        {
                            ProjectName = "Test project name 1",
                            Description = "ProjectDescr"
                        }
                    };
                    Assert.IsFalse(projectModel.SaveProject());
                    Assert.IsTrue(projectModel.ProjectDetails.Id == 0);
                    Assert.IsNull(
                        context.User_Project.FirstOrDefault(
                            u => u.ID_Project == projectModel.ProjectDetails.Id),
                        "User_project is saved");
                }
                finally
                {
                    if (projectModel != null && projectModel.ProjectDetails.Id > 0)
                        provider.DeleteProject(projectModel.ProjectDetails.Id);
                }
            }
        }

        private void DeleteUserFromDB(DbContext context, string login, string eMail)
        {
            DragonflyEntities ents = context as DragonflyEntities;
            if (ents != null)
            {
                var foundUsers = (from u in ents.User
                                  where u.Login.Equals(login) || u.E_mail.Equals(eMail)
                                  select u);
                foreach (var foundUser in foundUsers)
                {
                    ents.User.Remove(foundUser);
                }
                ents.SaveChanges();
            }
        }

    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dragonfly.Database.MsSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragonfly.Models.Projects;
using System.Data.Entity;
using Dragonfly.Tests;

namespace Dragonfly.Database.MsSQL.Tests
{
    [TestClass()]
    public class ClassConvertersExtensionsTests
    {
        [TestMethod()]
        public void ToProjectModelTest()
        {
            MsSqlDataProvider provider = new MsSqlDataProvider();

            Project proj = InitializeProject();

            ProjectModel model = proj.ToProjectModel(provider);

            Assert.AreEqual(proj.ID_Project, model.ProjectId, "Bad project id");
            Assert.AreEqual(proj.Name, model.ProjectName, "Bad Name ");
            Assert.AreEqual(proj.Date_Create, model.DateCreation, "Bad date of creation");
            Assert.AreEqual(proj.Description, model.Description, "Bad dexcription");
            CollectionAssert.AreEqual(
                proj.User_Project.Select(u => u.ID_User).ToList(),
                model.UserIds,
                "Users are not equal");
        }
        private static Project InitializeProject()
        {
            Project proj = new Project()
            {
                Date_Create = new DateTime(1970, 1, 1),
                Description = "Descr",
                ID_Project = 1,
                Name = "Project name",
            };
            proj.User_Project = new List<User_Project>()
            {
                new User_Project() {
                    ID_User = 11,
                    ID_Project = 1,
                    ID_Project_Role =2 }
            };
            return proj;
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToProjectModelWithoutProviderTest()
        {
            Project proj = InitializeProject();
            ProjectModel model = proj.ToProjectModel(null);
        }
    }
}
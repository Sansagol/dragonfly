using Dragonfly.Core;
using Dragonfly.Core.Settings;
using Dragonfly.Models;
using Dragonfly.Models.Projects;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragonfly.Database
{
    /// <summary>
    /// Interface implements providers, which will be
    /// provide access to databases.
    /// </summary>
    interface IDataBaseProvider
    {
        DbContext Context { get; }

        /// <summary>The method initialize connection with database.</summary>
        ///<param name="accessConfigurations">Parameters to DB access.</param>
        /// <returns>Created context. null if fail.</returns>
        DbContext Initizlize(DatabaseAccessConfiguration accessConfigurations);

        bool CheckUserCredentials(string login, string password);

        bool AddUser(LogUpModel userRegisterData);

        UserModel GetUserById(int userId);

        UserModel GetUserByLoginMail(string userLogin);

        void CreateProject(ProjectModel newProject);
    }
}

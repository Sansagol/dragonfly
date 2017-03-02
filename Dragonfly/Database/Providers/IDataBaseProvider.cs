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
    public interface IDataBaseProvider: IDisposable
    {
        DbContext Context { get; }

        /// <summary>The method initialize connection with database.</summary>
        ///<param name="accessConfigurations">Parameters to DB access.</param>
        /// <returns>Created context. null if fail.</returns>
        DbContext Initizlize(DatabaseAccessConfiguration accessConfigurations);

        bool CheckUserCredentials(string login, string password);

        decimal AddUser(SignUpModel userRegisterData);

        UserModel GetUserById(int userId);

        UserModel GetUserByLoginMail(string userLogin);

        void CreateProject(ProjectModel newProject);

        /// <summary>
        /// Method retrieves projects from database and returns it.
        /// </summary>
        /// <param name="offset">Offset from a first project.</param>
        /// <param name="count">Count projects to return.</param>
        /// <returns>Retrieved projects.</returns>
        IEnumerable<ProjectModel> GetProjects(int offset, int count);
    }
}

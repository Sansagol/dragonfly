using Dragonfly.Core;
using Dragonfly.Core.Settings;
using Dragonfly.Database.Entities;
using Dragonfly.Models;
using Dragonfly.Models.Projects;
using Dragonfly.Models.UserRoleSystem;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragonfly.Database.Providers
{
    /// <summary>
    /// Interface implements providers, which will be
    /// provide access to databases.
    /// </summary>
    public interface IDataBaseProvider : IDataProvider
    {
        bool CheckUserCredentials(string login, string password);

        decimal AddUser(SignUpModel userRegisterData);

        void CreateProject(ProjectModel newProject);

        void DeleteProject(decimal projectId);

        /// <summary>
        /// Method retrieves projects from database and returns it.
        /// </summary>
        /// <param name="offset">Offset from a first project.</param>
        /// <param name="count">Count projects to return.</param>
        /// <returns>Retrieved projects.</returns>
        IEnumerable<EProject> GetProjects(int offset, int count);
    }
}

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
    public interface IDataBaseProvider
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

        /// <summary>
        /// Method check an access token to correct and that is not expired.
        /// </summary>
        /// <param name="userId">Current logged user, which token are presented.</param>
        /// <param name="token">Token to check.</param>
        /// <returns>True - if token is correct. False - otherwise.</returns>
        bool CheckAccessToken(decimal userId, string token);

        /// <summary>Method create an access token for current user.</summary>
        /// <param name="userId">Id of user to create token.</param>
        /// <returns>Created access toker, or null if was error.</returns>
        /// <exception cref="InsertDbDataException"/>
        /// <exception cref="InvalidOperationException"/>
        string CreateAccessToken(decimal userId);
    }
}

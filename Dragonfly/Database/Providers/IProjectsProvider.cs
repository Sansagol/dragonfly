using Dragonfly.Models.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Database.Providers
{
    public interface IProjectsProvider : IDataProvider
    {
        /// <summary>Method return the project by id ID.</summary>
        /// <param name="projectId">ID of the project.</param>
        /// <returns>Found project ot NULL otherwise.</returns>
        ProjectModel GetProject(decimal projectId);
    }
}
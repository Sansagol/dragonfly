using Dragonfly.Database.Entities;
using System.Linq;

namespace Dragonfly.Database.Providers
{
    public interface IProjectsProvider : IDataProvider
    {
        /// <summary>Method return the project by id ID.</summary>
        /// <param name="projectId">ID of the project.</param>
        /// <returns>Found project ot NULL otherwise.</returns>
        EProject GetProject(decimal projectId);
    }
}
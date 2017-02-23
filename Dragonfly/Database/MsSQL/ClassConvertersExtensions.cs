using Dragonfly.Models.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Database.MsSQL
{
    internal static class ClassConvertersExtensions
    {
        /// <summary>
        /// Convert a project from DB oject into model project.
        /// </summary>
        /// <param name="project">Project object which need to convert.</param>
        /// <param name="provider">Provider of database.</param>
        /// <returns>Created project model.</returns>
        /// <exception cref="ArgumentNullException">Empty provider was set.</exception>
        public static ProjectModel ToProjectModel(this Project project, IDataBaseProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            ProjectModel projMod = new ProjectModel(provider)
            {
                ProjectId = project.ID_Project,
                ProjectName = project.Name,
                Description = project.Description,
                DateCreation = project.Date_Create,
            };
            project.User_Project.ToList().ForEach(u => projMod.UserIds.Add(u.ID_User));
            return projMod;
        }
    }
}
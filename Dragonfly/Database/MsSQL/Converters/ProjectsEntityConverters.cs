using Dragonfly.Database.Entities;
using Dragonfly.Models.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Database.MsSQL.Converters
{
    internal static class ProjectsEntityConverters
    {
        public static EProject ToProjectModel(this Project project)
        {
            EProject model = new EProject()
            {
                DateCreation = project.Date_Create,
                Description = project.Description,
                Id = project.ID_Project,
                ProjectName = project.Name,
            };
            return model;
        }

        public static Project ToDbProject(this EProject project)
        {
            Project dbProj = new Project()
            {
                Date_Create = project.DateCreation,
                Description = project.Description,
                ID_Project = project.Id,
                Name = project.ProjectName
            };
            return dbProj;
        }
    }
}
using Dragonfly.Models.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Database.MsSQL.Converters
{
    internal static class ProjectsEntityConverters
    {
        public static ProjectModel ToProjectModel(this Project project)
        {
            ProjectModel model = new ProjectModel()
            {
                DateCreation = project.Date_Create,
                Description = project.Description,
                ProjectId = project.ID_Project,
                ProjectName = project.Name,
                UserIds = project.User_Project.Select(u => u.ID_User).ToList()
            };
            return model;
        }
    }
}
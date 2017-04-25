using Dragonfly.Database.Entities;
using Dragonfly.Database.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Models.Projects
{
    internal static class Extensions
    {
        public static ProjectModel ToProjectModel(this EProject src,
            IDataBaseProvider dbProvider)
        {
            if (src == null)
                return null;
            ProjectModel model = new ProjectModel(dbProvider)
            {
                ProjectId = src.Id,
                DateCreation = src.DateCreation,
                ProjectName = src.ProjectName,
                Description = src.Description
            };
            return model;
        }
    }
}
using Dragonfly.Database.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dragonfly.Models.Projects;
using Dragonfly.Database.MsSQL.LowLevel;
using Dragonfly.Core.Settings;
using Dragonfly.Database.MsSQL.Converters;

namespace Dragonfly.Database.MsSQL
{
    class ProjectsProvider : DataProvider, IProjectsProvider
    {
        public ProjectsProvider(IDBContextGenerator contextgenerator) :
            base(contextgenerator)
        {
        }

        public ProjectModel GetProject(decimal projectId)
        {
            try
            {
                using (var context = _ContextGenerator.GenerateContext())
                {
                    var rawProject = (from p in context.Project
                                      where p.ID_Project == projectId
                                      select p).FirstOrDefault();
                    if (rawProject == null)
                        throw new ArgumentException("Target project is not found.");
                    ProjectModel model = rawProject?.ToProjectModel();
                    return model;
                }
            }
            catch (Exception ex)
            {//TODO log
                throw new InvalidOperationException("Error on the project retrieving.", ex);
            }
        }
    }
}
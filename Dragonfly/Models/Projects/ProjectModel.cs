using Dragonfly.Core;
using Dragonfly.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Models.Projects
{
    /// <summary>The model-class describe a single project.</summary>
    public class ProjectModel
    {
        /// <summary>Id of the project.</summary>
        public decimal ProjectId { get; set; }

        /// <summary>Ids of users which can something do with the project.</summary>
        public List<decimal> Users { get; set; }

        /// <summary>Name of the project.</summary>
        public string ProjectName { get; set; }

        /// <summary>Project description.</summary>
        public string Description { get; set; }

        /// <summary>Some errors on project.</summary>
        public string ProjectError { get; set; }

        public bool SaveProject()
        {
            bool saveResult = false;
            IDataBaseProvider context = BaseBindings.GetNewDbProvider();
            if (ProjectId == 0)
            {
                try
                {
                    context.CreateProject(this);
                    saveResult = true;
                }
                catch (InvalidOperationException ex)
                {
                    ProjectError = ex.Message;
                }
            }
            else
            {//Update existing project
            }
            return saveResult;
        }
    }
}
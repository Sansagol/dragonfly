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
        public int Projectid { get; set; }

        /// <summary>Id of user-owner of the project.</summary>
        public int UserOwnerId { get; set; }

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
            if (Projectid == 0)
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
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
        public List<decimal> UserIds { get; set; }

        /// <summary>Name of the project.</summary>
        public string ProjectName { get; set; }

        /// <summary>Project description.</summary>
        public string Description { get; set; }

        /// <summary>Some errors on project.</summary>
        public string ProjectError { get; set; }

        #region fields
        IDataBaseProvider _DbProvider = null;
        public IDataBaseProvider DbProvider
        {
            get { return _DbProvider; }
            set { _DbProvider = value; }
        }
        #endregion

        public ProjectModel()
        {
        }

        public ProjectModel(IDataBaseProvider dbProvider)
        {
            _DbProvider = dbProvider;
        }

        public bool SaveProject()
        {
            bool saveResult = false;
            if (ProjectId == 0)
            {
                try
                {
                    _DbProvider.CreateProject(this);
                    saveResult = true;
                }
                catch (InvalidOperationException ex)
                {
                    ProjectError = ex.Message;
                }
                catch (Exception ex)
                {
                    ProjectError = ex.Message;
                }
            }
            else
            {//TODO Update existing project
            }
            return saveResult;
        }
    }
}
using Dragonfly.Database.Entities;
using Dragonfly.Database.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Models.Projects
{
    /// <summary>The model describe a projects page.</summary>
    public class ProjectsModel
    {
        IDataBaseProvider _BasicProvider = null;

        /// <summary>Id of current user.</summary>
        public int UserId { get; set; }

        /// <summary>All Available projects for a user.</summary>
        public IEnumerable<ProjectModel> AvailableProjects { get; set; }

        public ProjectsModel(IDataBaseProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            _BasicProvider = provider;
        }

        public List<ProjectModel> GetProjects(int offset, int count)
        {
            List<ProjectModel> models = new List<ProjectModel>();
            var rawProjects = _BasicProvider.GetProjects(offset, count);
            foreach (EProject rawProj in rawProjects)
            {
                models.Add(new ProjectModel()
                {
                    ProjectDetails = rawProj
                });
            }
            return models;
        }
    }
}
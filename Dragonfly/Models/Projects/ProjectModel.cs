using Dragonfly.Core;
using Dragonfly.Database;
using Dragonfly.Database.Entities;
using Dragonfly.Database.Providers;
using Dragonfly.Models.UserRoleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Models.Projects
{
    /// <summary>The model-class describe a single project.</summary>
    public class ProjectModel
    {
        /// <summary>Is project selected in the view.</summary>
        public bool IsSelected { get; set; }

        /// <summary>Ids of users which can something do with the project.</summary>
        //public List<decimal> UserIds { get; set; }
        List<EUserProject> _Users = null;
        public List<EUserProject> Users
        {
            get
            {
                if (_Users == null)
                {
                    InitUsers();
                }
                return _Users;
            }
        }

        public EProject ProjectDetails { get; set; }

        /// <summary>Some errors on project.</summary>
        public string ProjectError { get; set; }

        /// <summary>
        /// Get or set a data, when prodect was creted.
        /// </summary>
        public DateTime DateCreation { get; set; }

        List<EClient> _Clients = null;
        public List<EClient> Clients
        {
            get
            {
                if (_Clients == null)
                {
                    InitClients();
                }
                return _Clients;
            }
        }
        List<EEntitlement> _Entitlements = null;
        public List<EEntitlement> Entitlements
        {
            get
            {
                if (_Entitlements == null)
                {
                    InitClients();
                }
                return _Entitlements;
            }
        }

        #region fields
        IDataBaseProvider _DbProvider = null;
        public IDataBaseProvider DbProvider
        {
            get { return _DbProvider; }
            set { _DbProvider = value; }
        }
        private IProjectsProvider _ProjectsProvider = null;
        private IClientsProvider _ClientsProvider = null;
        #endregion

        public ProjectModel()
        {
            _ProjectsProvider = BaseBindings.DBFactory.CreateProjectsProvider();
            _ClientsProvider = BaseBindings.DBFactory.CreateClientsProvider();
        }

        public ProjectModel(IDataBaseProvider dbProvider) :
            this()
        {
            _DbProvider = dbProvider;
        }

        #region Lazy load
        private void InitUsers()
        {
            _Users = new List<EUserProject>();
            if (ProjectDetails?.Id > 0)
            {
                RetrieveUsersFromDB();
            }
        }

        private void RetrieveUsersFromDB()
        {
            var users = _ProjectsProvider.GetUsersForProject(ProjectDetails.Id);
            _Users.AddRange(users);
        }

        private void InitClients()
        {
            _Entitlements = new List<EEntitlement>();
            _Clients = new List<EClient>();
            _Entitlements.AddRange(_ClientsProvider.GetEntitlementsForProject(ProjectDetails.Id));
            _Clients.AddRange(_Entitlements.Select(e => e.Client).Distinct());
        }
        #endregion

        /// <summary>Add the custom user to the project.</summary>
        /// <param name="userId">Id of the user.</param>
        /// <param name="isAdmin">Is Admin flag</param>
        /// <param name="projectRoleId">Access rights</param>
        public void AddUserToProject(decimal userId, decimal projectRoleId)
        {
            InitUsers();
            Users.Add(new EUserProject()
            {
                UserId = userId,
                ProjectRoleId = projectRoleId
            });
        }

        public bool SaveProject()
        {
            bool saveResult = false;
            if (ProjectDetails.Id == 0)
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
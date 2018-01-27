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
    public class ProjectModel : ModelBase
    {
        /// <summary>Is project selected in the view.</summary>
        public bool IsSelected { get; set; }

        public List<UserRoleModel> Users
        {
            get
            {
                if (_Users == null)
                {
                    LoadUsers();
                }
                return _Users;
            }
            set
            {
                _Users = value;
            }
        }
        List<UserRoleModel> _Users = null;

        /// <summary>Id of the project.</summary>
        public decimal Id { get; set; }

        /// <summary>Name of the project.</summary>
        public string ProjectName { get; set; }

        /// <summary>Project description.</summary>
        public string Description { get; set; }

        /// <summary>Get or set a data, when prodect was creted.</summary>
        public DateTime DateCreation { get; set; }

        [Obsolete]
        public EProject ProjectDetails { get; set; }

        /// <summary>Some errors on project.</summary>
        public string ProjectError { get; set; }


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
        //IDataBaseProvider _DbProvider = null;
        //public IDataBaseProvider DbProvider
        //{
        //    get { return _DbProvider; }
        //    set { _DbProvider = value; }
        //}
        //private IProjectsProvider _ProjectsProvider = null;
        //private IClientsProvider _ClientsProvider = null;
        #endregion

        public ProjectModel()
        {
            Users = null;
        }

        public ProjectModel(EProject dbProject) :
            this()
        {
            FillModel(dbProject);
        }

        [Obsolete]
        public ProjectModel(IDataBaseProvider dbProvider) :
            this()
        {
            //_DbProvider = dbProvider;
        }

        [Obsolete]
        public ProjectModel(IDataBaseProvider dbProvider, EProject dbProject) :
            this(dbProvider)
        {
            FillModel(dbProject);
        }

        private void FillModel(EProject dbProject)
        {
            Id = dbProject.Id;
            ProjectName = dbProject.ProjectName;
            Description = dbProject.Description;
            DateCreation = dbProject.DateCreation;

            LoadUsers();
        }

        private void LoadUsers()
        {
            Users = new List<UserRoleModel>();
            if (Id > 0)
            {
                List<EUserProject> users = _ProjectsDbProvider.GetUsersForProject(Id);
                users.ForEach(u => Users.Add(u.ToUserProjectModel()));
            }
        }

        #region Lazy load
        //private void InitUsers()
        //{
        //    _Users = new List<EUserProject>();
        //    if (Id > 0)
        //    {
        //        RetrieveUsersFromDB();
        //    }
        //}

        //private void RetrieveUsersFromDB()
        //{
        //    var users = _ProjectsProvider.GetUsersForProject(Id);
        //    _Users.AddRange(users);
        //}

        private void InitClients()
        {
            _Entitlements = new List<EEntitlement>();
            _Clients = new List<EClient>();
            _Entitlements.AddRange(_ClientsDbProvider.GetEntitlementsForProject(Id));
            _Clients.AddRange(_Entitlements.Select(e => e.Client).Distinct());
        }
        #endregion

        public EProject ToDbProject()
        {
            EProject dbProject = new EProject()
            {
                Id = Id,
                ProjectName = ProjectName,
                Description = Description,
                DateCreation = DateCreation
            };
            dbProject.UserIds.AddRange(Users.Select(u => u.Id));

            return dbProject;
        }

        /// <summary>Add the custom user to the project.</summary>
        /// <param name="userId">Id of the user.</param>
        /// <param name="isAdmin">Is Admin flag</param>
        /// <param name="projectRoleId">Access rights</param>
        public void AddUserToProject(decimal userId, decimal projectRoleId)
        {
            Users.Add(new UserRoleModel()
            {
                Id = userId,
            });
        }

        public bool SaveProject()
        {
            bool saveResult = false;
            try
            {
                if (Id == 0)
                {
                    _BaseDbProvider.CreateProject(this.ToDbProject());
                }
                else
                {
                    _BaseDbProvider.SaveProject(ToDbProject());
                }
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
            return saveResult;
        }
    }
}
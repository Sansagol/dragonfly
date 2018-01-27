using Dragonfly.Core;
using Dragonfly.Database.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Models
{
    public class ModelBase
    {
        protected IDataBaseProvider _BaseDbProvider = null;
        protected IClientsProvider _ClientsDbProvider = null;
        protected IEntitlementsProvider _EntitlementsDbProvider = null;
        protected IProjectsProvider _ProjectsDbProvider = null;
        protected IUserAccessProvider _UserAccessDbProvider = null;

        public ModelBase()
        {
            _BaseDbProvider = BaseBindings.DBFactory.CreateDBProvider();
            _ClientsDbProvider = BaseBindings.DBFactory.CreateClientsProvider();
            _EntitlementsDbProvider = BaseBindings.DBFactory.CreateEntitlementsProvider();
            _ProjectsDbProvider = BaseBindings.DBFactory.CreateProjectsProvider();
            _UserAccessDbProvider = BaseBindings.DBFactory.CreateUserAccessProvider();
        }
    }
}
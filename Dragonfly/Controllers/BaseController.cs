using Dragonfly.Core;
using Dragonfly.Database.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dragonfly.Controllers
{
    public class BaseController: Controller
    {
        protected static IDataBaseProvider BaseProvider { get; private set; }
        protected static IClientsProvider ClientsProvider { get; private set; }
        protected static IEntitlementsProvider EntitiesProvider { get; private set; }
        protected static IProjectsProvider ProjectsProvider { get; private set; }
        protected static IUserAccessProvider UserAccessProvider { get; private set; }


        /// <summary>Init static members.</summary>
        static BaseController()
        {
            BaseProvider = BaseBindings.DBFactory.CreateDBProvider();
            ClientsProvider = BaseBindings.DBFactory.CreateClientsProvider();
            EntitiesProvider = BaseBindings.DBFactory.CreateEntitlementsProvider();
            ProjectsProvider = BaseBindings.DBFactory.CreateProjectsProvider();
            UserAccessProvider = BaseBindings.DBFactory.CreateUserAccessProvider();
        }
    }
}
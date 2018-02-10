using Dragonfly.Core;
using Dragonfly.Core.UserAccess;
using Dragonfly.Database.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dragonfly.Controllers
{
    public class BaseController : Controller
    {
        protected static IDataBaseProvider BaseProvider { get; private set; }
        protected static IClientsProvider ClientsProvider { get; private set; }
        protected static IEntitlementsProvider EntitlementsProvider { get; private set; }
        protected static IProjectsProvider ProjectsProvider { get; private set; }
        protected static IUserAccessProvider UserAccessProvider { get; private set; }

        private IUserAuthenticateStateManager _UserStateManager = null;
        protected IUserAuthenticateStateManager UserStateManager
        {
            get { return _UserStateManager; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                _UserStateManager = value;
            }
        }

        /// <summary>Init static members.</summary>
        static BaseController()
        {
            BaseProvider = BaseBindings.DBFactory.CreateDBProvider();
            ClientsProvider = BaseBindings.DBFactory.CreateClientsProvider();
            EntitlementsProvider = BaseBindings.DBFactory.CreateEntitlementsProvider();
            ProjectsProvider = BaseBindings.DBFactory.CreateProjectsProvider();
            UserAccessProvider = BaseBindings.DBFactory.CreateUserAccessProvider();
        }

        protected BaseController()
        {
            _UserStateManager = BaseBindings.UsrStateManager;
        }

        /// <summary>The cethod thecks user authorization and set it values to the ViewBag.</summary>
        /// <exception cref="AuthenticationException">Is unauthorizad user</exception>
        protected void CheckUserAuthorization()
        {
            _UserStateManager.CheckUserAccess(Request, Response);
            ViewBag.Logged = true;
            ViewBag.UserName = BaseBindings.CookiesManager.GetCookieValue(
                Request,
                CookieType.UserName);
        }
    }
}
using Dragonfly.Core;
using Dragonfly.Core.UserAccess;
using Dragonfly.Database.Providers;
using Dragonfly.Models.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dragonfly.Controllers
{
    /// <summary>
    /// Controller for pages with clients representation.
    /// </summary>
    [ControllersException]
    public class ClientsController : BaseController
    {
        private ICookiesManager _CookManager = null;
        private IDBFactory _DatabaseFactory = null;

        public ClientsController()
        {
            _CookManager = BaseBindings.CookiesManager;
            _DatabaseFactory = BaseBindings.DBFactory;
        }

        [HttpGet]
        public ActionResult Index()
        {
            ClientsModel model = null;
            try
            {
                CheckUserAuthorization();
            }
            catch
            {
                ViewBag.Logged = false;
                ViewBag.UserName = Session["UserName"];
                ViewBag.Error = "Access denied. Please log in.";
                return View(model);
            }

            ViewBag.Logged = true;
            try
            {
                IEnumerable<ClientModel> clients = ClientsProvider.GetAllClients();
                model = new ClientsModel()
                {
                    Clients = clients.ToList()
                };
            }
            catch (Exception ex)
            {
                //TODO log
                ViewBag.Error = $"Unable to load clients: {ex.Message}";
            }
            return View(model);
        }

#if DEBUG
        /// <summary>Must invoke manually.</summary>
        public void ErrFun()
        {
            throw new Exception("Clients manual Error");
        }
#endif
    }
}
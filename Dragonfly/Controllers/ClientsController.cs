using Dragonfly.Core;
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
    public class ClientsController : Controller
    {
        private IUserStateManager _UserStateManager = null;
        private ICookiesManager _CookManager = null;
        private IDBFactory _DatabaseFactory = null;

        public ClientsController()
        {
            _UserStateManager = BaseBindings.UsrStateManager;
            _CookManager = BaseBindings.CookiesManager;
            _DatabaseFactory = BaseBindings.DBFactory;
        }

        [HttpGet]
        public ActionResult Index()
        {
            ClientsModel model = null;
            if (_UserStateManager.CheckUserAccess(Request))
            {
                try
                {
                    using (var clientsProvider = _DatabaseFactory.CreateClientsProvider(
                        BaseBindings.SettingsReader.GetDbAccessSettings()))
                    {
                        IEnumerable<ClientModel> clients = clientsProvider.GetAllClients();
                        model = new ClientsModel()
                        {
                            Clients = clients.ToList()
                        };
                    }
                }
                catch (Exception ex)
                {
                    //TODO log
                    ViewBag.Error = $"Unable to load clients: {ex.Message}";
                }
            }
            else
            {
                ViewBag.Logged = false;
                ViewBag.Error = "Access denied. Please log in.";
            }
            return View(model);
        }
    }
}
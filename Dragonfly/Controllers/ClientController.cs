using Dragonfly.Core;
using Dragonfly.Database.Providers;
using Dragonfly.Models.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Dragonfly.Controllers
{
    public class ClientController : Controller
    {
        private IUserStateManager _UserStateManager = null;
        private ICookiesManager _CookManager = null;
        private IDBFactory _DatabaseFactory = null;

        CreateClientModel _CreateModel = null;

        public ClientController()
        {
            _UserStateManager = BaseBindings.UsrStateManager;
            _CookManager = BaseBindings.CookiesManager;
            _DatabaseFactory = BaseBindings.DBFactory;
        }

        /// <summary>
        /// Method run creation a new client.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Add()
        {
            CreateClientModel model = null;
            if (_UserStateManager.CheckUserAccess(Request))
            {
                model = PrepareDateToAddClient();
            }
            else
            {
                ViewBag.Logged = false;
                ViewBag.Error = "Access denied. Please log in.";
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(object value)
        {
            CreateClientModel model = null;
            if (_UserStateManager.CheckUserAccess(Request))
            {
                if (value != null && value is CreateClientModel)
                    model = value as CreateClientModel;
                return View(model);
            }
            else
                return View("Add");
        }

        private CreateClientModel PrepareDateToAddClient()
        {
            ViewBag.Logged = true;

            CreateClientModel model = null;
            IEnumerable<ClientType> types = GetClientTypes();
            if (types?.Count() != 0)
            {
                if (_CreateModel != null)
                    model = _CreateModel;
                else
                    model = new CreateClientModel();
                model.AvailableTypes = (from t in types
                                        select new SelectListItem()
                                        {
                                            Text = t.TypeName,
                                            Value = t.TypeName
                                        });
            }
            else
            {
                ViewBag.Error += "Available client types not found.";
            }
            return model;
        }

        private IEnumerable<ClientType> GetClientTypes()
        {
            IEnumerable<ClientType> types = null;
            try
            {
                using (var clientsProvider = _DatabaseFactory.CreateClientsProvider(
                      BaseBindings.SettingsReader.GetDbAccessSettings()))
                {
                    types = clientsProvider.GetAvailableClientTypes();
                }
            }
            catch (Exception ex)
            {//TODO log
                ViewBag.Error = ex.Message;
            }
            return types;
        }

        /// <summary>
        /// Method check parameters and create a new client.
        /// </summary>
        /// <param name="model">Model for creation a client.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(CreateClientModel model)
        {
            if (_UserStateManager.CheckUserAccess(Request))
            {
                ViewBag.Logged = true;
                if (!ModelState.IsValid)
                    return View("Add", model);
                try
                {
                    using (var clientsProvider = _DatabaseFactory.CreateClientsProvider(
                         BaseBindings.SettingsReader.GetDbAccessSettings()))
                    {
                        clientsProvider.CreateClient(model.Client);
                    }
                    return RedirectToAction("Index", "Clients");
                }
                catch (Exception ex)
                {   //Back to creation with entered data.
                    ViewBag.Error = ex.Message;
                    _CreateModel = model;
                    return View("Add", model);
                }
            }
            else
            {
                ViewBag.Logged = false;
                return RedirectToAction("Add", "Client");
            }
        }
    }
}
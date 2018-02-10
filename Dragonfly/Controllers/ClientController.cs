using Dragonfly.Core;
using Dragonfly.Core.UserAccess;
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
    [ControllersException]
    public class ClientController : BaseController
    {
        private ICookiesManager _CookManager = null;
        private IDBFactory _DatabaseFactory = null;

        CreateClientModel _CreateModel = null;

        public ClientController()
        {
            _CookManager = BaseBindings.CookiesManager;
            _DatabaseFactory = BaseBindings.DBFactory;
        }

        /// <summary>
        /// Method run creation a new client.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ControllersException]
        public ActionResult Add()
        {
            CreateClientModel model = null;
            model = PrepareDateToAddClient();
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(object value)
        {
            CreateClientModel model = null;
            try
            {
                CheckUserAuthorization();
                if (value != null && value is CreateClientModel)
                    model = value as CreateClientModel;
                return View(model);
            }
            catch
            {
                return View("Add");
            }
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
                var clientsProvider = _DatabaseFactory.CreateClientsProvider();
                types = clientsProvider.GetAvailableClientTypes();
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
            try
            {
                CheckUserAuthorization();
            }
            catch
            {
                ViewBag.Logged = false;
                return RedirectToAction("Add", "Client");
            }

            ViewBag.Logged = true;
            if (!ModelState.IsValid)
            {
                List<string> errors = new List<string>();
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        if (!string.IsNullOrWhiteSpace(error.ErrorMessage))
                            errors.Add(error.ErrorMessage);
                        if (!string.IsNullOrWhiteSpace(error.Exception?.Message))
                            errors.Add(error.Exception.Message);
                    }
                }
                model.CreationErrors = string.Join("; ", errors);
                return View("Add", model);
            }
            try
            {
                ClientsProvider.CreateClient(model.Client);
                return RedirectToAction("Index", "Clients");
            }
            catch (Exception ex)
            {   //Back to creation with entered data.
                ViewBag.Error = ex.Message;
                _CreateModel = model;
                return View("Add", model);
            }
        }
    }
}
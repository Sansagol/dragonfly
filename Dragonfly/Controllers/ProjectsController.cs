﻿using Dragonfly.Core;
using Dragonfly.Core.UserAccess;
using Dragonfly.Database;
using Dragonfly.Database.Providers;
using Dragonfly.Models.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dragonfly.Models.Projects;

namespace Dragonfly.Controllers
{
    /// <summary>
    /// Controller is responsoble for the build a projects data
    /// for the user.
    /// </summary>
    [ControllersException]
    public class ProjectsController : Controller
    {
        private string _InitializationError = null;

        private IUserAuthenticateStateManager _UserStateManager = null;

        public ProjectsController()
        {
            _UserStateManager = BaseBindings.UsrStateManager;
        }

        // GET: Projects
        public ActionResult Index()
        {
            if (_UserStateManager.CheckUserAccess(Request, Response))
            {
                ViewBag.Logged = true;
                ProjectsModel model = null;
                using (IDataBaseProvider provider = BaseBindings.DBFactory.CreateDBProvider(
                    BaseBindings.SettingsReader.GetDbAccessSettings()))
                {
                    model = new ProjectsModel(provider);
                    model.AvailableProjects = model.GetProjects(0, 10);
                    //var projects = provider.GetProjects(0, 10);
                    //model.AvailableProjects = projects.ToList().ForEach(p => p.ToProjectModel());
                }
                return View(model);
            }
            else
            {
                ViewBag.Logged = false;
            }
            return View();
        }
    }
}
﻿using Dragonfly.Core;
using Dragonfly.Database;
using Dragonfly.Models.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dragonfly.Controllers
{
    /// <summary>
    /// Controller is responsoble for the build a projects data
    /// for the user.
    /// </summary>
    public class ProjectsController : Controller
    {
        private IDataBaseProvider _DbProvider = null;
        private string _InitializationError = null;

        public ProjectsController()
        {
            try
            {
                _DbProvider = BaseBindings.GetNewDbProvider();
            }
            catch (Exception ex)
            {
                _InitializationError = ex.ToString();
            }
        }

        // GET: Projects
        public ActionResult Index()
        {
            ViewBag.Logged = Session["UserId"] != null;
            if (Session["UserId"] == null)
            {
            }

            ProjectsModel model = new ProjectsModel();
            _DbProvider.GetProjects(0, 10);

            return View();
        }
    }
}
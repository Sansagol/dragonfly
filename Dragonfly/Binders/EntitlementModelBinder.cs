using Dragonfly.Models.Entitlement;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Dragonfly.Binders
{
    public class EntitlementModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            EditEntitlementModel model = new EditEntitlementModel();

            var value = bindingContext.ValueProvider.GetValue("dateBegin");
            DateTime dateTime;
            var isDate = DateTime.TryParse(value.AttemptedValue, Thread.CurrentThread.CurrentUICulture, DateTimeStyles.None, out dateTime);


            return null;
        }
    }
}
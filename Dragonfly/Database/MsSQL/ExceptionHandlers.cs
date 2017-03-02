using Dragonfly.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace Dragonfly.Database.MsSQL
{
    public static class ExceptionHandlers
    {
        public static List<ValidationError> RetrieveValidationsErrors(this DbEntityValidationException ex)
        {
            List<ValidationError> validationErrors = new List<ValidationError>();
            foreach (var validResult in ex.EntityValidationErrors)
            {
                validationErrors.AddRange(validResult.ValidationErrors.Select(
                    v => new ValidationError(v.PropertyName, v.ErrorMessage)));
            }

            return validationErrors;
        }
    }
}
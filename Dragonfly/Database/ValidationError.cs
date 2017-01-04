using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Database
{
    public class ValidationError
    {
        public string Field { get; set; }
        public string ErrorText { get; set; }

        public ValidationError(string fieldName, string error)
        {
            Field = fieldName;
            ErrorText = error;
        }
    }
}
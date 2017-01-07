using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.SettingsManager
{
    public class InsertDbDataException : Exception
    {
        Dictionary<string, string> _FieldNames = null;
        public Dictionary<string, string> FieldNames { get { return _FieldNames; } }

        public InsertDbDataException(Dictionary<string, string> errorFildNames) :
            this(string.Empty, errorFildNames)
        {
        }

        public InsertDbDataException(string message, Dictionary<string, string> errorFildNames) :
            base(message)
        {

            if (errorFildNames != null)
                _FieldNames = errorFildNames;
            else
                _FieldNames = new Dictionary<string, string>();
        }
    }
}
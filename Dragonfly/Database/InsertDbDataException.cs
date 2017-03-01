using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Database
{
    public class InsertDbDataException : Exception
    {
        IEnumerable<ValidationError> _FieldNames = null;
        public IEnumerable<ValidationError> FieldNames { get { return _FieldNames; } }

        public InsertDbDataException(string message) :
            this(message, null)
        {
        }

        public InsertDbDataException(IEnumerable<ValidationError> errorFildNames) :
            this(string.Empty, errorFildNames)
        {
        }

        public InsertDbDataException(string message, IEnumerable<ValidationError> errorFildNames) :
            base(message)
        {

            if (errorFildNames != null)
                _FieldNames = errorFildNames;
            else
                _FieldNames = new List<ValidationError>();
        }
    }
}
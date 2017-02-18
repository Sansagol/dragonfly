using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Database
{
    public class DbInitializationException : Exception
    {
        public DbInitializationException(string message)
            : base(message)
        {
        }
    }
}
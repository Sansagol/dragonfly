using System;
using System.Collections.Generic;
using System.Linq;

namespace Dragonfly.Core
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Method get a full deep message from the exception.
        /// </summary>
        /// <param name="ex">Source exception.</param>
        /// <returns>Full message.</returns>
        public static string GetFullMessage(this Exception ex)
        {
            List<string> messages = new List<string>();
            messages.Add(ex.Message);
            if (ex.InnerException != null)
                messages.Add(ex.InnerException.GetFullMessage());
            return string.Join("; ", messages);
        }
    }
}
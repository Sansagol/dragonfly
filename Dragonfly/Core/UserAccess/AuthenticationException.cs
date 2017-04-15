using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Core.UserAccess
{
    /// <summary>
    /// This type of exception for the notice about unauthenicated user.
    /// </summary>
    public class AuthenticationException : Exception
    {
        /// <summary>Unauthentication user name.</summary>
        public string UserName { get; private set; }

        public AuthenticationException(string userName)
        {
            UserName = userName;
        }


        public AuthenticationException(string error, string userName) :
            base(error)
        {
            UserName = userName;
        }
    }
}
using Dragonfly.Core;
using Dragonfly.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Dragonfly.Models
{
    /// <summary>Model represent a user authentical parameters.</summary>
    public class AuthenticateModel
    {//TODO change password to secure string
        /// <summary>User login or e-mail.</summary>
        [Required(ErrorMessage = "Please enter login")]
        public string Login { get; set; }

        /// <summary>User password.</summary>
        [Required(ErrorMessage = "Password will't be empty")]
        public string Password { get; set; }

        /// <summary>Is need to keep login for next authenticate.</summary>
        public bool KeepLogin { get; set; }

        /// <summary>Method check user credentials.</summary>
        /// <returns>True - if is right user. False - in another case.</returns>
        public bool CheckUser()
        {
            bool isTrueUser = false;

            if (!string.IsNullOrWhiteSpace(Login) &&
                !string.IsNullOrWhiteSpace(Password))
            {
                IDataBaseProvider context = BaseBindings.GetNewDbProvider();
                isTrueUser = context.CheckUserCredentials(Login, Password);
            }
            return isTrueUser;
        }
    }
}
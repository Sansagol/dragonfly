using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Dragonfly.Models
{
    public class AuthenticateModel
    {
        [Required(ErrorMessage = "Please enter login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Password will't be empty")]
        //TODO change to secure string
        public string Password { get; set; }

        public bool KeepLogin { get; set; }
    }
}
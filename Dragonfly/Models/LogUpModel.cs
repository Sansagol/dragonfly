using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Dragonfly.Models
{
    /// <summary>Class represent a model for user registration.</summary>
    public class SignUpModel
    {
        [Required(ErrorMessage = "Please enter login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Please enter e-mail")]
        public string EMail { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        //TODO change to secure string
        public string Password { get; set; }
    }
}
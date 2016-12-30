using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Models
{
    public class AuthenticateModel
    {
        public string Login { get; set; }
        //TODO change to secure string
        public string Password { get; set; }

        public bool KeepLogin { get; set; }
    }
}
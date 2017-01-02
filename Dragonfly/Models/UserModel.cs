using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Dragonfly.Models
{
    public class UserModel
    {
        public string Login{get;set;}
        public string Name { get; set; }
    }
}
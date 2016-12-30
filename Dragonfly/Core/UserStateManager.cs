using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Core
{
    public class UserStateManager
    {
        public static bool IsUserLogged { get; set; }
        public static string UserName { get; set; }

    }
}
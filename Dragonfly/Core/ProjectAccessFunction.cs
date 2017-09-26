using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Core
{
    [Flags]
    public enum ProjectAccessFunction
    {
        None = 0,
        ViewListOfUsers = 1,
        ViewClients = 2,
        //Must view the users
        AddDelUserToProject = 5,
        //Must view the clients
        /// <summary>Do something with the license entitlements</summary>
        AddDeleteEntitlement = 18,
        //Must view the clients
        /// <summary>Do something with the techsupport</summary>
        AddDeleteTechsupportEntitlement = 34,
        
        /// <summary>Full access to the project.</summary>
        FullAccess = 255,
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Core.UserAccess
{
    /// <summary>
    /// Represent an access function for global user roles.
    /// </summary>
    public enum AccessFunction
    {
        /// <summary>View users in the system.</summary>
        UsersRead = 0,
        /// <summary>
        /// Manage users in the system. Depends from the UserRead parameter.
        /// </summary>
        UsersManage = 1,

        ProjectsRead = 2,
        ProjectsManage = 3,

        ClientsRead = 4,
        ClientsManage = 5,

        /// <summary>
        /// Access to the tech support functions.
        /// </summary>
        TechSupport = 6
    }
}
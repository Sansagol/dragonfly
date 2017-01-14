//using Dragonfly.Database;
//using Dragonfly.Database.MsSQL;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Web;

//namespace Dragonfly.Core
//{
//    internal class UserStateManager
//    {
//        private IDataBaseProvider _DbProvider = null;

//        /// <summary>Is user authorized in system.</summary>
//        public static bool IsUserLogged { get; set; }
//        /// <summary>Name of authorized user.</summary>
//        public static string UserName { get; set; }
//        /// <summary>Id of authorized user.</summary>
//        public static decimal UserId { get; set; }

//        /// <summary>Initialize a user's manager.</summary>
//        /// <param name="dbProvide">Provider to database</param>
//        /// <exception cref="ArgumentNullException">Passed an empty parameter.</exception>
//        public UserStateManager(IDataBaseProvider dbProvide)
//        {
//            if (dbProvide == null)
//                throw new ArgumentNullException(nameof(dbProvide));
//            _DbProvider = dbProvide;
//        }

//        public bool AuthorizeUser(string login, string password)
//        {
//            if (string.IsNullOrWhiteSpace(login))
//                return false;
//            if (string.IsNullOrWhiteSpace(password))
//                return false;

//            IDataBaseProvider context = BaseBindings.GetNewDbProvider();
//            {

//            }

//            return true;
//        }
//    }
//}
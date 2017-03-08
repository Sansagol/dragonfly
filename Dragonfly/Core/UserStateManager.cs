using Dragonfly.Database;
using Dragonfly.Database.MsSQL;
using Dragonfly.Database.Providers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Dragonfly.Core
{
    internal class UserStateManager: IUserStateManager
    {
        private IDBFactory _DatabaseFactory = null;
        private ICookiesManager _CookiesManager = null;

        /// <summary>Initialize a user's manager.</summary>
        /// <param name="dbFactory">Provider to database</param>
        /// <exception cref="ArgumentNullException">Passed an empty parameter.</exception>
        public UserStateManager(IDBFactory dbFactory, ICookiesManager cookiesManager)
        {
            if (dbFactory == null)
                throw new ArgumentNullException(nameof(dbFactory));
            if (cookiesManager == null)
                throw new ArgumentNullException(nameof(cookiesManager));
            _DatabaseFactory = dbFactory;
            _CookiesManager = cookiesManager;
        }

        /// <summary>
        /// Method check is user can access to the portal.
        /// </summary>
        /// <returns>True - user have an access. False - otherwise.</returns>
        public bool CheckUserAccess(HttpRequestBase request)
        {
            string accessToken =
                _CookiesManager.GetCookie(request, CookieType.UserAccessToken);
            decimal userId = GetUserIdFromCookies(request);

            bool isCorrectAccess = false;
            if (!string.IsNullOrWhiteSpace(accessToken))
                using (var accessProvider = BaseBindings.GetNewUserAccessProvider())
                {
                    isCorrectAccess = accessProvider.CheckAccessToken(userId, accessToken);
                }

            return isCorrectAccess;
        }

        private decimal GetUserIdFromCookies(HttpRequestBase request)
        {
            decimal userId = 0;
            string user = _CookiesManager.GetCookie(request, CookieType.UserId);
            try
            {
                if (!string.IsNullOrWhiteSpace(user))
                    userId = int.Parse(user);
            }
            catch (Exception ex)
            {
                //TODO log               
            }
            return userId;
        }
    }
}
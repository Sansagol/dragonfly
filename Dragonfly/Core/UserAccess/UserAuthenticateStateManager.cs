using Dragonfly.Database;
using Dragonfly.Database.MsSQL;
using Dragonfly.Database.Providers;
using Dragonfly.Models;
using Dragonfly.Models.UserRoleSystem;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Dragonfly.Core.UserAccess
{
    /// <summary>Manage of user authentication.</summary>
    internal class UserAuthenticateStateManager : IUserAuthenticateStateManager
    {
        private Logger _Logger = LogManager.GetCurrentClassLogger();
        private IDBFactory _DatabaseFactory = null;
        private ICookiesManager _CookiesManager = null;

        /// <summary>Initialize a user's manager.</summary>
        /// <param name="dbFactory">Provider to database</param>
        /// <exception cref="ArgumentNullException">Passed an empty parameter.</exception>
        public UserAuthenticateStateManager(IDBFactory dbFactory, ICookiesManager cookiesManager)
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
        public bool CheckUserAccess(HttpRequestBase request, HttpResponseBase response)
        {
            string accessToken =
                _CookiesManager.GetCookieValue(request, CookieType.UserAccessToken);
            decimal userId = GetUserIdFromCookies(request);

            if (!string.IsNullOrWhiteSpace(accessToken) &&
                GetIsCorrectAccess(accessToken, userId))
                return true;

            LogOut(request, response);
            throw new AuthenticationException(_CookiesManager.GetCookieValue(request, CookieType.UserName));
        }

        private bool GetIsCorrectAccess(string accessToken, decimal userId)
        {
            bool isCorrectAccess = false;
            try
            {
                using (var accessProvider = BaseBindings.DBFactory.CreateUserAccessProvider(
                    BaseBindings.SettingsReader.GetDbAccessSettings()))
                {
                    isCorrectAccess = accessProvider.CheckAccessToken(userId, accessToken);
                }
            }
            catch (Exception ex)
            {
                _Logger.Warn("Error on check user authenticate: {0}", ex.GetFullMessage());
                throw new AuthenticationException(ex.Message, null);
            }

            return isCorrectAccess;
        }

        private decimal GetUserIdFromCookies(HttpRequestBase request)
        {
            decimal userId = 0;
            string user = _CookiesManager.GetCookieValue(request, CookieType.UserId);
            try
            {
                if (!string.IsNullOrWhiteSpace(user))
                    userId = int.Parse(user);
            }
            catch (Exception ex)
            {
                _Logger.Warn("Error on retrieving user id from cookies: {0}", ex.GetFullMessage());
            }
            return userId;
        }

        public void LogOut(HttpRequestBase request, HttpResponseBase response)
        {
            var cookMan = BaseBindings.CookiesManager;
            string token = cookMan.GetCookieValue(request, CookieType.UserAccessToken);
            try
            {
                if (!string.IsNullOrWhiteSpace(token))
                    using (var ap = BaseBindings.DBFactory.CreateUserAccessProvider(
                        BaseBindings.SettingsReader.GetDbAccessSettings()))
                    {
                        ap.DeleteAccessToken(token);
                    }
            }
            catch (Exception ex)
            {
                _Logger.Warn("Error on logout: {0}", ex.GetFullMessage());
            }

            ClearCookies(response);
        }

        private void ClearCookies(HttpResponseBase response)
        {
            _CookiesManager.DeleteCookie(response, CookieType.UserAccessToken);
            _CookiesManager.DeleteCookie(response, CookieType.UserId);
            _CookiesManager.DeleteCookie(response, CookieType.UserName);
        }

        public bool LogIn(HttpResponseBase response, AuthenticateModel authParameters)
        {
            bool isLogged = false;
            if (CheckUser(authParameters.Login, authParameters.Password))
            {
                using (IDataBaseProvider provider = BaseBindings.DBFactory.CreateDBProvider(
                         BaseBindings.SettingsReader.GetDbAccessSettings()))
                {
                    using (IUserAccessProvider uprovider = BaseBindings.DBFactory.CreateUserAccessProvider(
                             BaseBindings.SettingsReader.GetDbAccessSettings()))
                    {
                        UserModel user = new UserModel(uprovider);
                        user = user.GetUserByEmailLogin(authParameters.Login);
                        if (user != null)
                        {
                            using (var ap = BaseBindings.DBFactory.CreateUserAccessProvider(
                                BaseBindings.SettingsReader.GetDbAccessSettings()))
                            {
                                string accToken = ap.CreateAccessToken(user.Id);
                                _CookiesManager.SetToCookie(
                                    response,
                                    CookieType.UserAccessToken, accToken);
                                _CookiesManager.SetToCookie(
                                    response,
                                    CookieType.UserId, user.Id.ToString());
                                _CookiesManager.SetToCookie(
                                    response,
                                    CookieType.UserName, user.Name ?? user.Login);
                                isLogged = true;
                            }
                        }
                    }
                }
            }
            else
            {
                ClearCookies(response);
                authParameters.IsTrueUser = false;
                authParameters.ErrorOnUserChecking = "User not found";
            }
            return isLogged;
        }

        /// <summary>
        /// Method check a user on trust.
        /// </summary>
        private bool CheckUser(string login, string password)
        {
            bool isTrueUser = false;
            string errorOnUserChecking = string.Empty;

            if (!string.IsNullOrWhiteSpace(login) &&
                !string.IsNullOrWhiteSpace(password))
            {
                try
                {
                    using (var ap = BaseBindings.DBFactory.CreateDBProvider(
                        BaseBindings.SettingsReader.GetDbAccessSettings()))
                    {
                        isTrueUser = ap.CheckUserCredentials(login, password);
                        if (!isTrueUser)
                            errorOnUserChecking = "Incorrect login or password";
                    }
                }
                catch (InvalidOperationException ex)
                {
                    errorOnUserChecking = ex.Message;
                }
            }
            return isTrueUser;
        }
    }
}
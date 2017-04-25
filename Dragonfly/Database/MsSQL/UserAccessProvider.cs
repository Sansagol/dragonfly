using Dragonfly.Database.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using Dragonfly.Database.MsSQL.LowLevel;
using Dragonfly.Core.Settings;
using System.Security.Claims;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security;
using Dragonfly.Database.Entities;

namespace Dragonfly.Database.MsSQL
{
    class UserAccessProvider : DataProvider, IUserAccessProvider
    {
        #region Low level interfaces
        IUserDBDataManager _UserManager = null;
        #endregion

        DatabaseAccessConfiguration _DatabaseConfig = null;

        public UserAccessProvider(
            IUserDBDataManager userDbDataManage,
            IDBContextGenerator contextgenerator,
            DatabaseAccessConfiguration dbConfig) :
            base(contextgenerator)
        {
            if (userDbDataManage == null)
                throw new ArgumentNullException(nameof(userDbDataManage));
            if (dbConfig == null)
                throw new ArgumentNullException(nameof(dbConfig));

            _UserManager = userDbDataManage;
            _DatabaseConfig = dbConfig;
        }

        /// <summary>
        /// Method check an access token to correct and that is not expired.
        /// </summary>
        /// <param name="userId">Current logged user, which token are presented.</param>
        /// <param name="token">Token to check.</param>
        /// <returns>True - if token is correct. False - otherwise.</returns>
        public bool CheckAccessToken(decimal userId, string token)
        {
            DateTime now = DateTime.UtcNow.Date;
            using (var context = _ContextGenerator.GenerateContext(_DatabaseConfig))
            {
                var userAccesses = (from userAccess in context.User_Access
                                    where DbFunctions.TruncateTime(userAccess.Date_Expiration) >= now &&
                                          userAccess.Access_Token.Equals(token) &&
                                          userAccess.ID_User == userId
                                    select userAccess).OrderByDescending(u => u.Date_Expiration);
                //Delete multiple equals access tokens
                if (userAccesses.Count() > 1)
                {//TODO: write this to log as error
                    context.User_Access.RemoveRange(userAccesses.Skip(1));
                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {//TODO log
                    }
                }
                return userAccesses.Count() >= 1;
            }
        }

        private void DeleteOldUserAccessTokens(decimal userId, DateTime now, DragonflyEntities context)
        {
            var userAccesses =
                (from userAccess in context.User_Access
                 where userAccess.ID_User == userId &&
                       DbFunctions.TruncateTime(userAccess.Date_Expiration) < now.Date
                 select userAccess);
            DeleteAccessTokens(userAccesses, context);
        }

        private void DeleteAccessTokens(IQueryable<User_Access> userAccesses, DragonflyEntities context)
        {
            if (userAccesses.Count() > 0)
                context.User_Access.RemoveRange(userAccesses);
            try
            {
                context.SaveChanges();
            }
            catch (Exception ex)
            {//TODO log
            }
        }

        public void DeleteAccessToken(string token)
        {
            using (var context = _ContextGenerator.GenerateContext(_DatabaseConfig))
            {
                var accessTokens = (from ua in context.User_Access
                                    where ua.Access_Token.Equals(token)
                                    select ua);
                DeleteAccessTokens(accessTokens, context);
            }
        }

        /// <summary>Method create an access token for current user.</summary>
        /// <param name="userId">Id of user to create token.</param>
        /// <returns>Created access toker, or null if was error.</returns>
        /// <exception cref="InsertDbDataException"/>
        /// <exception cref="InvalidOperationException"/>
        public string CreateAccessToken(decimal userId)
        {
            string token = string.Empty;
            DateTime now = DateTime.UtcNow;
            using (var context = _ContextGenerator.GenerateContext(_DatabaseConfig))
            {
                DeleteOldUserAccessTokens(userId, now, context);
                User currentUser = _UserManager.GetUserById(userId);
                if (currentUser != null)
                {
                    token = GenerateAccessToken(now);
                    User_Access access = new User_Access()
                    {
                        Access_Token = token,
                        Date_Creation = now,
                        Date_Expiration = now.AddDays(1),
                        ID_User = currentUser.ID_User
                    };
                    try
                    {
                        context.User_Access.Add(access);
                        context.SaveChanges();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        context.User_Access.Remove(access);
                        List<ValidationError> validationErrors = ex.RetrieveValidationsErrors();
                        throw new InsertDbDataException(validationErrors);
                    }
                    catch (DbUpdateException ex)
                    {
                        context.User_Access.Remove(access);
                        throw new InvalidOperationException("Unable to save user-access", ex);
                    }
                }
            }
            return token;
        }

        private string GenerateAccessToken(DateTime now)
        {//TODO Change this to something good
            byte[] time = BitConverter.GetBytes(now.ToBinary());
            string key = Guid.NewGuid().ToString("N");
            string encodedTime = System.Web.HttpServerUtility.UrlTokenEncode(time);
            return string.Concat(key, encodedTime).ToLower();
        }

        private string GenerateAccessToken(DateTime now, string userName)
        {
            var tokenExpiration = TimeSpan.FromDays(1);
            ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Name, userName));

            var props = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
            };

            var ticket = new AuthenticationTicket(identity, props);
            OAuthBearerAuthenticationOptions opt = new OAuthBearerAuthenticationOptions();
            var accessToken = opt.AccessTokenFormat.Protect(ticket);
            return accessToken;
        }

        public EUser GetUserById(decimal id)
        {
            EUser user = null;
            User usr = null;
            usr = _UserManager.GetUserById(id);
            user = usr?.ToEUser();
            return user;
        }

        public EUser GetUserByLoginMail(string userLogin)
        {
            EUser model = null;
            User usr = null;
            try
            {
                using (var context = _ContextGenerator.GenerateContext(_DatabaseConfig))
                {
                    usr = (from user in context.User
                           where user.Login.Equals(userLogin) ||
                                 user.E_mail.Equals(userLogin)
                           select user).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Database is down.", ex);
            }
            if (usr != null)
            {
                model = usr.ToEUser();
            }
            return model;
        }
    }
}
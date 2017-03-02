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

namespace Dragonfly.Database.MsSQL
{
    class UserAccessProvider : IUserAccessProvider
    {
        DragonflyEntities _Context = null;

        #region Low level interfaces
        IDBContextGenerator _ContextGenerator = null;

        IUserDBDataManager _UserManager = null;
        #endregion

        public UserAccessProvider(IUserDBDataManager userDbDataManage, IDBContextGenerator contextgenerator)
        {
            if (userDbDataManage == null)
                throw new ArgumentNullException(nameof(userDbDataManage));
            if (contextgenerator == null)
                throw new ArgumentNullException(nameof(contextgenerator));

            _UserManager = userDbDataManage;
            _ContextGenerator = contextgenerator;
        }

        /// <summary>Method create and open context for database.</summary>
        /// <param name="accessConfigurations">Parameters to database connect.</param>
        /// <returns>Created context. null if fail.</returns>
        /// <exception cref="DbInitializationException">Error on database initialization.</exception>
        public void Initialize(DatabaseAccessConfiguration accessConfigurations)
        {
            _Context = _ContextGenerator.GenerateContext(accessConfigurations);
        }

        public void Dispose()
        {
            _Context?.Dispose();
            _Context = null;
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
            DeleteOldUserAccessTokens(userId, now);
            var userAccesses = (from userAccess in _Context.User_Access
                                where DbFunctions.TruncateTime(userAccess.Date_Expiration) >= now &&
                                      userAccess.Access_Token.Equals(token)
                                select userAccess).OrderByDescending(u => u.Date_Expiration);
            //Delete multiple equals access tokens
            if (userAccesses.Count() > 1)
            {//TODO: write this to log as error
                _Context.User_Access.RemoveRange(userAccesses.Skip(1));
            }
            return userAccesses.Count() >= 1;
        }

        private void DeleteOldUserAccessTokens(decimal userId, DateTime now)
        {
            var userAccesses =
                (from userAccess in _Context.User_Access
                 where userAccess.ID_User == userId &&
                       DbFunctions.TruncateTime(userAccess.Date_Expiration) < now.Date
                 select userAccess);
            if (userAccesses.Count() > 0)
                _Context.User_Access.RemoveRange(userAccesses);
            try
            {
                _Context.SaveChanges();
            }
            catch (Exception ex)
            {
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

            DeleteOldUserAccessTokens(userId, now);
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
                    _Context.User_Access.Add(access);
                    _Context.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    _Context.User_Access.Remove(access);
                    List<ValidationError> validationErrors = ex.RetrieveValidationsErrors();
                    throw new InsertDbDataException(validationErrors);
                }
                catch (DbUpdateException ex)
                {
                    _Context.User_Access.Remove(access);
                    throw new InvalidOperationException("Unable to save user-access", ex);
                }
            }
            return token;
        }

        private string GenerateAccessToken(DateTime now)
        {
            byte[] time = BitConverter.GetBytes(now.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            return Convert.ToBase64String(time.Concat(key).ToArray());
        }
    }
}
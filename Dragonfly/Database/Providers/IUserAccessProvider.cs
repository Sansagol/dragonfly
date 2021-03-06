﻿using Dragonfly.Core.Settings;
using Dragonfly.Database.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Dragonfly.Database.Providers
{
    public interface IUserAccessProvider : IDataProvider
    {
        /// <summary>
        /// Method check an access token to correct and that is not expired.
        /// </summary>
        /// <param name="userId">Current logged user, which token are presented.</param>
        /// <param name="token">Token to check.</param>
        /// <returns>True - if token is correct. False - otherwise.</returns>
        bool CheckAccessToken(decimal userId, string token);

        /// <summary>Method create an access token for current user.</summary>
        /// <param name="userId">Id of user to create token.</param>
        /// <returns>Created access toker, or null if was error.</returns>
        /// <exception cref="InsertDbDataException"/>
        /// <exception cref="InvalidOperationException"/>
        string CreateAccessToken(decimal userId);

        /// <summary>
        /// Method delete a user access token from database.
        /// </summary>
        /// <param name="token">Token which need to delete from db.</param>
        void DeleteAccessToken(string token);

        EUser GetUserById(decimal id);

        EUser GetUserByLoginMail(string userLogin);
    }
}
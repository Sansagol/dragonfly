using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Dragonfly.Database.MsSQL.LowLevel
{
    /// <summary>
    /// Class represent low lwvwl operations on users using a database.
    /// </summary>
    public class UserDBDataManager : IUserDBDataManager
    {
        DragonflyEntities _Context = null;

        public void Initialize(DbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            _Context = context as DragonflyEntities;
            if (_Context == null)
                throw new ArgumentException(
                    $"Wrong type of the database context. Expected {typeof(DragonflyEntities)}");
        }

        public User GetUserById(decimal userId)
        {
            User usr;
            try
            {
                usr = (from user in _Context.User
                       where user.ID_User == userId
                       select user).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Database is down.", ex);
            }

            return usr;
        }
    }
}
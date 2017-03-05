using Dragonfly.Database.MsSQL.LowLevel;
using Dragonfly.Database.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Database.MsSQL
{
    /// <summary>
    /// Factory which generate providers for access to a MS SQL server DB.
    /// </summary>
    public class MsSqlFactory : IDBFactory
    {
        IDBContextGenerator _ContextGenerator = null;
        IUserDBDataManager _UserDBDataManager = null;

        /// <summary>
        /// Base constructor of the MS MQL factory.
        /// </summary>
        public MsSqlFactory()
        {
            _ContextGenerator = new DBContextGenerator();
            _UserDBDataManager = new UserDBDataManager();
        }

        public IDataBaseProvider CreateDBProvider()
        {
            IDataBaseProvider provider = new MsSqlDataProvider(_UserDBDataManager, _ContextGenerator);
            return provider;
        }

        public IUserAccessProvider CreateUserAccessProvider()
        {
            IUserAccessProvider provider = new UserAccessProvider(_UserDBDataManager, _ContextGenerator);
            return provider;
        }
    }
}
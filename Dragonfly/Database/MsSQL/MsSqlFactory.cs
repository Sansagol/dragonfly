using Dragonfly.Core.Settings;
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
    class MsSqlFactory : IDBFactory
    {
        IDBContextGenerator _ContextGenerator = null;
        IUserDBDataManager _UserDBDataManager = null;

        /// <summary>
        /// Base constructor of the MS MQL factory.
        /// </summary>
        public MsSqlFactory(DatabaseAccessConfiguration dbConfig)
        {
            if (dbConfig == null)
                throw new ArgumentNullException(nameof(dbConfig));
            _ContextGenerator = new DBContextGenerator(dbConfig);
            _UserDBDataManager = new UserDBDataManager();

        }

        public IDataBaseProvider CreateDBProvider()
        {
            _UserDBDataManager.Initialize(_ContextGenerator.GenerateContext());

            IDataBaseProvider provider = new MsSqlDataProvider(
                _UserDBDataManager,
                _ContextGenerator);
            return provider;
        }

        public IUserAccessProvider CreateUserAccessProvider()
        {
            _UserDBDataManager.Initialize(_ContextGenerator.GenerateContext());

            IUserAccessProvider provider = new UserAccessProvider(
                _UserDBDataManager,
                _ContextGenerator);
            return provider;
        }

        public IClientsProvider CreateClientsProvider()
        {
            IClientsProvider provider = new ClientsProvider(_ContextGenerator);
            return provider;
        }

        public IProjectsProvider CreateProjectsProvider()
        {
            IProjectsProvider projectsProvider = new ProjectsProvider(_ContextGenerator);
            return projectsProvider;
        }

        public IEntitlementsProvider CreateEntitlementsProvider()
        {
            IEntitlementsProvider provider = new EntitlementsProvider(_ContextGenerator);
            return provider;
        }
    }
}
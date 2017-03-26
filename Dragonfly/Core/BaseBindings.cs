﻿using Dragonfly.Core.Settings;
using Dragonfly.Database;
using Dragonfly.Database.MsSQL;
using Dragonfly.Database.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Core
{
    /// <summary>Class represent a simple IoC.</summary>
    internal class BaseBindings
    {
        static ICookiesManager _CooksManager = null;
        
        public static ISettingsReader SettingsReader { get; }

        public static ICookiesManager CookiesManager { get { return _CooksManager; } }

        public static IUserStateManager UsrStateManager { get { return _UsrStateManager; } }
        private static IUserStateManager _UsrStateManager = null;

        public static IDBFactory DBFactory { get { return _DbFactory; } }
        static IDBFactory _DbFactory = null;

        static BaseBindings()
        {
            SettingsReader = new SettingsLibReader();
            _DbFactory = new MsSqlFactory();
            _CooksManager = new CookieMananger();
            _UsrStateManager = new UserStateManager(_DbFactory, _CooksManager);
        }

        /// <summary>Method create and return a database provider.</summary>
        /// <returns>A database provider</returns>
        /// <exception cref="InvalidOperationException">
        /// Some error on creation provider.
        /// </exception>
        [Obsolete]
        public static IDataBaseProvider GetNewBaseDbProvider()
        {
            IDataBaseProvider baseProvider = null;
            try
            {
                baseProvider = _DbFactory.CreateDBProvider(SettingsReader.GetDbAccessSettings());
            }
            catch (InvalidOperationException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Error on creation a db provider.", ex);
            }
            return baseProvider;
        }

        [Obsolete]
        public static IUserAccessProvider GetNewUserAccessProvider()
        {
            IUserAccessProvider baseProvider = null;
            try
            {
                baseProvider = _DbFactory.CreateUserAccessProvider(SettingsReader.GetDbAccessSettings());
            }
            catch (InvalidOperationException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Error on creation a user access provider.", ex);
            }
            return baseProvider;
        }
    }
}
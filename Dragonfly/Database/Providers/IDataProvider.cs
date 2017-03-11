﻿using Dragonfly.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragonfly.Database.Providers
{
    /// <summary>
    /// Interface represent a providers, which transfer data to any storage.
    /// </summary>
    public interface IDataProvider: IDisposable
    {
        /// <summary>Initialize a data provider.</summary>
        /// <param name="accessConfigurations">Storage access configuration.</param>
        void Initialize(DatabaseAccessConfiguration accessConfigurations);
    }
}

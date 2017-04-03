using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Dragonfly.Core.Logging
{
    /// <summary>Class represent a base NLog configuration.</summary>
    public class BasicNLogConfig : ILogConfiguration
    {
        string _LogDirectory = string.Empty;

        /// <summary>
        /// Initialize a base parameters of the configurator.
        /// </summary>
        /// <param name="logDirectory"></param>
        public BasicNLogConfig(string logDirectory)
        {
            if (string.IsNullOrWhiteSpace(logDirectory))
                throw new ArgumentNullException(nameof(logDirectory));
            if (!Directory.Exists(logDirectory))
                try
                {
                    Directory.CreateDirectory(logDirectory);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Bad path to log dir.", ex);
                }
            _LogDirectory = logDirectory;
        }

        /// <summary>
        /// Method initialize a NLog configuration.
        /// </summary>
        public void InitConfig()
        {
            LoggingConfiguration config = new LoggingConfiguration();

            var fileTarget = new FileTarget("logFile");
            fileTarget.FileName = Path.Combine(_LogDirectory, "Common.log");
            fileTarget.Layout = @"${date:format=dd.MM.yyyy HH\:mm\:ss}|${level}|${message}|${exception:innerFormat=Message:maxInnerExceptionLevel=3}";
            config.AddTarget(fileTarget);

            var rule = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule);

            LogManager.Configuration = config;
        }
    }
}
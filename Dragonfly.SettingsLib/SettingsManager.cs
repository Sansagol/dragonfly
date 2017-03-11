using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Dragonfly.SettingsLib
{
    /// <summary>Class manages the storage of data in system.</summary>
    public class SettingsManager
    {
        /// <summary>This is default config file.</summary>
        private readonly string _DefaultPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "Dragonfly",
            "config.xml");

        /// <summary>
        /// Method load a dragonfly configuration from the default config file.
        /// </summary>
        /// <returns>Loaded configuration.</returns>
        /// <exception cref="InvalidOperationException">Something wrong.</exception>
        /// <exception cref="FileNotFoundException">Configuration file not found.</exception> 
        public DragonflyConfig LoadConfiguration()
        {
            return LoadConfiguration(_DefaultPath);
        }

        /// <summary>Method load a dragonfly configuration from the custom config file.</summary>
        /// <returns>Loaded configuration.</returns>
        /// <exception cref="InvalidOperationException">Something wrong.</exception>
        /// <exception cref="FileNotFoundException">Configuration file not found.</exception> 
        public DragonflyConfig LoadConfiguration(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = _DefaultPath;
            DragonflyConfig config = null;

            try
            {
                using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(DragonflyConfig));
                    config = serializer.Deserialize(reader) as DragonflyConfig;
                }
            }
            catch (FileNotFoundException ex)
            {
                throw ex;
            }
            catch (DirectoryNotFoundException)
            {
                throw new FileNotFoundException("Configuration not found", path);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unknown exception.", ex);
            }

            return config;
        }

        /// <summary>
        /// Method save parameters to a default config file.
        /// </summary>
        /// <param name="config">Parameters to save.</param>
        public void SaveConfiguration(DragonflyConfig config)
        {
            SaveConfiguration(config, _DefaultPath);
        }

        /// <summary>Method save a passing parameters to a config file.</summary>
        /// <param name="config">Parameters to save.</param>
        /// <exception cref="ArgumentNullException">
        /// Empty configuration was passed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Error on config saving.
        /// </exception>
        public void SaveConfiguration(DragonflyConfig config, string path)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            PrepareConfigDirectory(path);
            try
            {
                using (StreamWriter writer = new StreamWriter(
                    path,
                    false,
                    Encoding.UTF8))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(DragonflyConfig));
                    serializer.Serialize(writer, config);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to save config.", ex);
            }
        }

        /// <exception cref="InvalidOperationException"/>
        private void PrepareConfigDirectory(string path)
        {
            string directoryName = Path.GetDirectoryName(path);
            if (!Directory.Exists(directoryName))
                try
                {
                    Directory.CreateDirectory(directoryName);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Unable to create config directory.", ex);
                }
        }
    }
}

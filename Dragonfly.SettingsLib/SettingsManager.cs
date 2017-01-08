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
            "Dregonfly",
            "config.xml");

        /// <summary>Method load a dragonfly configuration from the default config file.</summary>
        /// <returns>Loaded configuration.</returns>
        /// <exception cref="InvalidOperationException">Something wrong.</exception>
        /// <exception cref="FileNotFoundException">Configuration file not found.</exception> 
        public DragonflyConfig LoadConfiguration()
        {
            DragonflyConfig config = null;

            try
            {
                using (StreamReader reader = new StreamReader(_DefaultPath, Encoding.UTF8))
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
                throw new FileNotFoundException("Configuration not found", _DefaultPath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unknown exception.", ex);
            }

            return config;
        }

        /// <summary>Method save a passing parameters to a config file.</summary>
        /// <param name="config">Parameters to save.</param>
        /// <exception cref="ArgumentNullException">
        /// Empty configuration was passed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Error on config saving.
        /// </exception>
        public void SaveConfiguration(DragonflyConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            PrepareConfigDirectory();
            try
            {
                using (StreamWriter writer = new StreamWriter(
                    _DefaultPath,
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
        private void PrepareConfigDirectory()
        {
            string directoryName = Path.GetDirectoryName(_DefaultPath);
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

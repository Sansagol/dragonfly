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
        /// <exception cref="InvalidOperationException">Something wrong</exception>
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
                throw new InvalidOperationException(
                    string.Format(
                        "Config file \'{0}\' doesn't found.",
                        ex.FileName));
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
    }
}

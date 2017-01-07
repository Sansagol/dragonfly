using Dragonfly.SettingsLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dragonfly.SettingsManager
{
    /// <summary>Logic of the main window.</summary>
    public partial class MainWindow : Window
    {
        /// <summary>Common config of the database.</summary>
        public DatabaseConfig DbConfiguration
        {
            get { return (DatabaseConfig)GetValue(DbConfigurationProperty); }
            set { SetValue(DbConfigurationProperty, value); }
        }
        public static readonly DependencyProperty DbConfigurationProperty =
            DependencyProperty.Register("DbConfiguration", typeof(DatabaseConfig), typeof(MainWindow), new PropertyMetadata(null));

        public MainWindow()
        {
            InitializeComponent();
            InitializeConfiguration();
        }

        private void InitializeConfiguration()
        {
            SettingsLib.SettingsManager manager = new SettingsLib.SettingsManager();
            DragonflyConfig config = null;

            try
            {
                config = manager.LoadConfiguration();
            }
            catch { }

            if (config == null)
                config = new DragonflyConfig();

            DbConfiguration = config.DbConfiguration;
        }


        private void btnSaveConfig_Click(object sender, RoutedEventArgs e)
        {
            SettingsLib.SettingsManager manager = new SettingsLib.SettingsManager();
            try
            {
                DragonflyConfig config = new DragonflyConfig()
                {
                    DbConfiguration = DbConfiguration
                };
                manager.SaveConfiguration(config);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error on config saving {ex.ToString()}");
            }
        }
    }
}

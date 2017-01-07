using Dragonfly.SettingsLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace Dragonfly.SettingsManager.Pages
{
    /// <summary>Class realize a logic of the db settings control.</summary>
    public partial class DbSettingsControl : UserControl
    {
        //TODO: made MVVM from this. (when I become elf)

        /// <summary>Common config data to store in system.</summary>
        public DatabaseConfig CommonConfig
        {
            get { return (DatabaseConfig)GetValue(CommonConfigProperty); }
            set { SetValue(CommonConfigProperty, value); }
        }
        public static readonly DependencyProperty CommonConfigProperty =
            DependencyProperty.Register("CommonConfig", typeof(DatabaseConfig), typeof(DbSettingsControl), new PropertyMetadata(null, ((d,e)=>
            {
                DbSettingsControl control = d as DbSettingsControl;
                if (control != null && e.NewValue!=null)
                {
                    DatabaseConfig config = e.NewValue as DatabaseConfig;
                    if (config != null)
                    {
                        control.DbAddress = config.DbAddress;
                        control.DbName = config.DbName;
                        control.Login = config.DefaultUserName;
                        control.Password = config.DefaultUserPassword;
                    }
                }
            })));

        #region Parameters
        public string DbAddress
        {
            get { return (string)GetValue(DbAddressProperty); }
            set { SetValue(DbAddressProperty, value); }
        }
        public static readonly DependencyProperty DbAddressProperty =
            DependencyProperty.Register("DbAddress", typeof(string), typeof(DbSettingsControl), new PropertyMetadata(string.Empty, ((d, e) =>
            {
                DbSettingsControl control = d as DbSettingsControl;
                if (control != null)
                    control.UpdateCommonConfig(
                        nameof(control.CommonConfig.DbAddress), e.NewValue);
            })));

        public string DbName
        {
            get { return (string)GetValue(DbNameProperty); }
            set { SetValue(DbNameProperty, value); }
        }
        public static readonly DependencyProperty DbNameProperty =
            DependencyProperty.Register("DbName", typeof(string), typeof(DbSettingsControl), new PropertyMetadata(string.Empty, ((d, e) =>
            {
                DbSettingsControl control = d as DbSettingsControl;
                if (control != null)
                    control.UpdateCommonConfig(
                        nameof(control.CommonConfig.DbName), e.NewValue);
            })));

        public string Login
        {
            get { return (string)GetValue(LoginProperty); }
            set { SetValue(LoginProperty, value); }
        }
        public static readonly DependencyProperty LoginProperty =
            DependencyProperty.Register("Login", typeof(string), typeof(DbSettingsControl), new PropertyMetadata(string.Empty, ((d, e) =>
            {
                DbSettingsControl control = d as DbSettingsControl;
                if (control != null)
                    control.UpdateCommonConfig(
                        nameof(control.CommonConfig.DefaultUserName), e.NewValue);
            })));

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(DbSettingsControl), new PropertyMetadata(string.Empty, ((d, e) =>
            {
                DbSettingsControl control = d as DbSettingsControl;
                if (control != null)
                    control.UpdateCommonConfig(
                        nameof(control.CommonConfig.DefaultUserPassword), e.NewValue);
            })));

        #endregion

        public DbSettingsControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void UpdateCommonConfig(string propertyName, object value)
        {
            PropertyInfo prop = CommonConfig.GetType().GetProperty(propertyName);
            if (prop != null)
            {
                prop.SetValue(CommonConfig, value, null);
            }
        }
    }
}

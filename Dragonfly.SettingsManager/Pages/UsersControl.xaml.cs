using Dragonfly.SettingsLib;
using Dragonfly.SettingsManager.DataSources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// <summary>
    /// Interaction logic for UsersControl.xaml
    /// </summary>
    public partial class UsersControl : UserControl
    {
        public DatabaseConfig DbConfig
        {
            get { return (DatabaseConfig)GetValue(CommonConfigProperty); }
            set { SetValue(CommonConfigProperty, value); }
        }
        public static readonly DependencyProperty CommonConfigProperty =
            DependencyProperty.Register(nameof(DbConfig), typeof(DatabaseConfig), typeof(UsersControl), new PropertyMetadata(null, ((d, e) => { })));

        #region fields
        public ObservableCollection<User> Users
        {
            get { return (ObservableCollection<User>)GetValue(UsersProperty); }
            set { SetValue(UsersProperty, value); }
        }
        public static readonly DependencyProperty UsersProperty =
            DependencyProperty.Register("Users", typeof(ObservableCollection<User>), typeof(UsersControl), new PropertyMetadata(new ObservableCollection<User>()));


        public string UserName
        {
            get { return (string)GetValue(UserNameProperty); }
            set { SetValue(UserNameProperty, value); }
        }
        public static readonly DependencyProperty UserNameProperty =
            DependencyProperty.Register("UserName", typeof(string), typeof(UsersControl), new PropertyMetadata(string.Empty));

        public string UserPassword
        {
            get { return (string)GetValue(UserPasswordProperty); }
            set { SetValue(UserPasswordProperty, value); }
        }
        public static readonly DependencyProperty UserPasswordProperty =
            DependencyProperty.Register("UserPassword", typeof(string), typeof(UsersControl), new PropertyMetadata(string.Empty));

        public string UserEMail
        {
            get { return (string)GetValue(UserEMailProperty); }
            set { SetValue(UserEMailProperty, value); }
        }
        public static readonly DependencyProperty UserEMailProperty =
            DependencyProperty.Register("UserEMail", typeof(string), typeof(UsersControl), new PropertyMetadata(string.Empty));

        /// <summary>Selected in table user.</summary>
        public User SelectedUser
        {
            get { return (User)GetValue(SelectedUserProperty); }
            set { SetValue(SelectedUserProperty, value); }
        }
        public static readonly DependencyProperty SelectedUserProperty =
            DependencyProperty.Register("SelectedUser", typeof(User), typeof(UsersControl), new PropertyMetadata(null));


        #endregion

        public UsersControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void btnGetUsers_Click(object sender, RoutedEventArgs e)
        {
            if (IsTrueDbConfig())
                LoadUsersRunner(DbConfig);
        }

        private bool IsTrueDbConfig()
        {
            if (DbConfig != null &&
                !string.IsNullOrWhiteSpace(DbConfig.DbAddress) &&
                !string.IsNullOrWhiteSpace(DbConfig.DbName) &&
                !string.IsNullOrWhiteSpace(DbConfig.DefaultUserName) &&
                !string.IsNullOrWhiteSpace(DbConfig.DefaultUserPassword))
                return true;
            return false;
        }

        /// <summary>Methor run a loading all users from database.</summary>
        private void LoadUsersRunner(DatabaseConfig config)
        {
            Users.Clear();
            new Task(() =>
            {
                MsSqlDataProvider provider = new MsSqlDataProvider();
                DragonflyEntities context = provider.Initizlize(config) as DragonflyEntities;
                var users = (from u in context.User
                             select u).ToList();
                Dispatcher.Invoke((Action)(() =>
                {
                    foreach (User user in users)
                    {
                        Users.Add(user);
                    }
                }));
            }).Start();
        }

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            if (!IsTrueDbConfig())
            {
                MessageBox.Show("Need right db configuration on the first tab.");
                return;
            }
            if (string.IsNullOrWhiteSpace(UserName))
            {
                MessageBox.Show("User name is empty");
                return;
            }
            if (string.IsNullOrWhiteSpace(UserPassword))
            {
                MessageBox.Show("User password is empty");
                return;
            }
            if (string.IsNullOrWhiteSpace(UserEMail))
            {
                MessageBox.Show("User e-mail is empty");
                return;
            }
            User user = new User()
            {
                E_mail = UserEMail,
                Login = UserName,
                Password = UserPassword
            };
            DatabaseConfig config = DbConfig;
            new Task((Action)(() =>
            {
                MsSqlDataProvider provider = new MsSqlDataProvider();
                DragonflyEntities context = provider.Initizlize(config) as DragonflyEntities;
                try
                {
                    provider.AddUser(user.Login, user.Password, user.E_mail);                   
                    Dispatcher.Invoke((Action)(() =>
                    {
                        UserEMail = string.Empty;
                        UserName = string.Empty;
                        UserPassword = string.Empty;
                        LoadUsersRunner(config);
                    }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            })).Start();
        }

        private void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (!IsTrueDbConfig())
            {
                MessageBox.Show("Need right db configuration on the first tab.");
                return;
            }

            if (SelectedUser == null)
            {
                MessageBox.Show("Please select the user to delete.");
                return;
            }

            User userToDelete = SelectedUser;
            DatabaseConfig config = DbConfig;

            new Task((Action)(() =>
            {
                MsSqlDataProvider provider = new MsSqlDataProvider();
                DragonflyEntities context = provider.Initizlize(config) as DragonflyEntities;
                try
                {
                    provider.DeleteUser(userToDelete.Login);
                    Dispatcher.Invoke((Action)(() =>
                    {
                        LoadUsersRunner(config);
                    }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            })).Start();
        }
    }
}

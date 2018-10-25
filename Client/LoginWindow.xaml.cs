using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        LoginWindow lw;
        private Net.Client _client;
        private MainWindow _main;

        public LoginWindow(Net.Client client, MainWindow main)
        {
            _client = client;
            InitializeComponent();
            //  Closing += LoginWindow_Closing;
        }

        private void LoginWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _client.Close();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            _client.Login(UsernameTextBox.Text, PasswordTextBox.Password, SessionIDTextBox.Text);
            this.Close();

            if (!_client.LoggedIn)
            {
                lw = new LoginWindow(_client, _main);
                lw.Show();
            }
            }
    }
}
            this.Close();

            if (_client.LoggedIn) { 
               lw = new LoginWindow(_client, _main);
                lw.Show();
                }
            }
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
        /// <summary>
        /// Logic for the Loginwindow.
        /// The main purpose for this winows is to handle the login for a client into a session.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="main"></param>
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
        /// <summary>
        /// Sends the username and password to the server to validate them
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            _client.Login(UsernameTextBox.Text, PasswordTextBox.Password, SessionIDTextBox.Text);
            this.Hide();
        }
    }
}
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LoginWindow _login;
        private readonly Net.Client _client;

        public MainWindow()
        {
            _client = new Net.Client("127.0.0.1", 1337, OnLogin);
            _login = new LoginWindow(_client);
            _login.Show();
            this.Hide();
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public void OnLogin()
        {
            Console.WriteLine("On Login callback");
            Dispatcher.Invoke(Show);
        }

        private void ChatLog_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void OnOpenClick(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void TextEditor_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void ChatBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
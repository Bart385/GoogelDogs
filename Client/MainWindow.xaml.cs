using System;
using System.IO;
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
        private readonly string _uuid = Guid.NewGuid().ToString();

        public MainWindow()
        {
            _client = new Net.Client("127.0.0.1", 1337, OnLogin, AddMessageToLog);
            _login = new LoginWindow(_client);
            _login.Show();
            this.Hide();
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public void OnLogin()
        {
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
            File.WriteAllText($"{Path.GetTempPath()}{_uuid}_GoogelDogs_local_cache.txt", TextEditor.Text);
        }

        private void ChatBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && ChatBox.Text != "")
            {
                _client.SendChatMessage(ChatBox.Text);
                ChatBox.Text = "";
            }
        }

        public void AddMessageToLog(string sender, string message)
        {
            Dispatcher.Invoke(() => { this.ChatLog.Items.Add(new {Sender = sender, Message = message}); });
        }
    }
}
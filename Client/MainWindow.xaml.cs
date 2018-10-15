using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;


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
        private string _previousEditorContent = "";
        public DispatcherTimer Timer { get; }

        public MainWindow()
        {
            _client = new Net.Client("127.0.0.1", 1337, OnLogin, AddMessageToLog);
            _login = new LoginWindow(_client);
            _login.Show();
            this.Hide();
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromMilliseconds(500);
            Timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (TextEditor.Text != "" && _previousEditorContent != TextEditor.Text)
            {
                _client.SendUpdatePatch(_previousEditorContent, TextEditor.Text);
            }

            _previousEditorContent = TextEditor.Text;
        }

        public void OnLogin()
        {
            Dispatcher.Invoke(Show);
            Timer.Start();
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
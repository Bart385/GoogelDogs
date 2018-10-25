using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using Client.Entities;
using OT.Business;
using OT.Entities;
using Protocol;
using Protocol.Messages;


namespace Client.Net
{
    /// <summary>
    /// Client class is for the users of the program
    /// Once the clients proejct is started it will open a loginscreen to verify the user
    /// if the user is verified the application will go into the main Screen
    /// </summary>
    public class Client
    {
        public string Username { get; set; }
        public bool Running { get; set; } = true;
        public Document Document { get; }
        public DiffMatchPatch DMP { get; }
        public Boolean LoggedIn;

        private readonly Action _loginCallback;
        private readonly Action _loginFailedCallback;
        private readonly Action<string, string> _messageLogCallback;
        private readonly Action<PatchMessage> _editorUpdateCallback;
        private readonly Action<string> _editorRecoverUpdateCallback;
        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _stream;
        private readonly Stack<Edit> _edits;

        /// <summary>
        /// The Client constructor makes contact with the server and draws a new document to use.
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <param name="loginCallback"></param>
        /// <param name="messageLogCallback"></param>
        /// <param name="editorUpdateCallback"></param>
        /// <param name="editorRecoverUpdateCallback"></param>
        public Client(string hostname, int port, Action loginCallback,
            Action<string, string> messageLogCallback, Action<PatchMessage> editorUpdateCallback,
            Action<string> editorRecoverUpdateCallback, Action loginFailedCallback)
        {
            _loginCallback = loginCallback;
            _messageLogCallback = messageLogCallback;
            _editorUpdateCallback = editorUpdateCallback;
            _editorRecoverUpdateCallback = editorRecoverUpdateCallback;
            _loginFailedCallback = loginFailedCallback;

            Document = new Document();
            _tcpClient = new TcpClient(hostname, port);
            Console.WriteLine(_tcpClient.Connected);
            _stream = _tcpClient.GetStream();
            DMP = new DiffMatchPatch();
            _edits = new Stack<Edit>();
            StartBackgroundListener();
        }

        #region Commands

        /// <summary>
        /// Login is for verifieng the login name this is done by a loginMessage protocl.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="session"></param>
        public void Login(string username, string password, string session)
        {
            Username = username;
            SendMessage(new LoginMessage(username, password, session));
        }

        public void SendUpdatePatch(string previousText, string currentText)
        {
            List<Diff> diffs = DMP.diff_main(previousText, currentText, false);
            DMP.diff_cleanupEfficiency(diffs);
            _edits.Push(new Edit(diffs, Document.ShadowCopy.ClientVersion, Document.ShadowCopy.ServerVersion));
            SendMessage(new PatchMessage(Username, _edits));
            Document.ShadowCopy.ClientVersion++;
            _edits.Clear();
        }

        public void SendOutOfSyncMessage()
        {
            SendMessage(new OutOfSyncMessage());
        }

        public void SendChatMessage(string message)
        {
            SendMessage(new ChatMessage(Username, message));
        }

        #endregion

        private async void SendMessage(IMessage message)
        {
            await MessagingUtil.SendMessage(_stream, message);
        }

        public void Close()
        {
            Running = false;
            _stream.Flush();
            _stream.Close();
            _tcpClient.Close();
        }

        private void StartBackgroundListener()
        {
            Console.WriteLine("Connected!");
            Task.Factory.StartNew(async () =>
            {
                while (Running)
                {
                    dynamic msg = await MessagingUtil.ReceiveMessage(_stream);

                    Console.WriteLine($"In Client BackgroundListener: {msg}");
                    IMessage message = JsonDecoder.Decode(msg);
                    Console.WriteLine($"In Client BackgroundListener: {message.Type}");
                    switch (message.Type)
                    {
                        case MessageType.OK_MESSAGE:
                            HandleOkMessage((OkMessage) message);
                            break;
                        case MessageType.OK_LOGIN_MESSAGE:
                            HandleOkLoginMessage((OkLoginMessage) message);
                            break;
                        case MessageType.ERROR_MESSAGE:
                            HandleErrorMessage((ErrorMessage) message);
                            break;
                        case MessageType.CHAT_MESSAGE:
                            HandleChatMessage((ChatMessage) message);
                            break;
                        case MessageType.PATCH_MESSAGE:
                            HandlePatchMessage((PatchMessage) message);
                            break;
                        case MessageType.PATCH_ERROR_MESSAGE:
                            HandlePatchErrorMessage((PatchErrorMessage) message);
                            break;
                        case MessageType.OUT_OF_SYNC_RESPONSE:
                            HandleOutOfSyncResponse((OutOfSyncResponse) message);
                            break;
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }

        #region Handlers

        private void HandleOkMessage(OkMessage message)
        {
        }

        private void HandleOkLoginMessage(OkLoginMessage message)
        {
            Console.WriteLine("Login approved!");
            LoggedIn = true;
            _loginCallback();
        }

        private void HandleErrorMessage(ErrorMessage message)
        {
            if (message.Message == "Login Failed")
                Console.WriteLine("Error logging in...");
            LoggedIn = true;
            MessageBox.Show("Er is iets fout gegaan bij het inloggen probeert u het alstblieft opnieuw");
            _loginFailedCallback();
        }

        private void HandleChatMessage(ChatMessage message)
        {
            _messageLogCallback(message.Sender, message.Message);
        }

        private void HandlePatchMessage(PatchMessage message)
        {
            _edits.Clear();
            _editorUpdateCallback(message);
        }

        private void HandlePatchErrorMessage(PatchErrorMessage message)
        {
            //_editorRecoverUpdateCallback(message);
        }

        private void HandleOutOfSyncResponse(OutOfSyncResponse message)
        {
            _editorRecoverUpdateCallback(message.CurrentServerText);
        }

        #endregion
    }
}
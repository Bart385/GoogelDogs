using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Client.Entities;
using OT.Business;
using OT.Entities;
using Protocol;
using Protocol.Messages;

namespace Client.Net
{
    public class Client
    {
        public string Username { get; set; }
        public bool Running { get; set; } = true;
        public Document Document { get; }
        public DiffMatchPatch DMP { get; }

        private readonly Action _loginCallback;
        private readonly Action<string, string> _messageLogCallback;
        private readonly Action<PatchMessage> _editorUpdateCallback;
        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _stream;
        private readonly Stack<Edit> _edits;

        public Client(string hostname, int port, Action loginCallback,
            Action<string, string> messageLogCallback, Action<PatchMessage> editorUpdateCallback)
        {
            _loginCallback = loginCallback;
            _messageLogCallback = messageLogCallback;
            _editorUpdateCallback = editorUpdateCallback;
            Document = new Document();
            _tcpClient = new TcpClient(hostname, port);
            Console.WriteLine(_tcpClient.Connected);
            _stream = _tcpClient.GetStream();
            DMP = new DiffMatchPatch();
            _edits = new Stack<Edit>();
            StartBackgroundListener();
        }

        #region Commands

        public void Login(string username, string password, string session)
        {
            Username = username;
            SendMessage(new LoginMessage(username, password, session));
        }

        public void SendUpdatePatch(string previousText, string currentText)
        {
            List<Diff> diffs = DMP.diff_main(previousText, currentText);
            DMP.diff_cleanupSemantic(diffs);
            _edits.Push(new Edit(diffs, Document.ShadowCopy.ClientVersion, Document.ShadowCopy.ServerVersion));
            SendMessage(new PatchMessage(Username, _edits));
            Document.ShadowCopy.ClientVersion++;
            _edits.Clear();
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
            _loginCallback();
        }

        private void HandleErrorMessage(ErrorMessage message)
        {
            if (message.Message == "Login Failed")
                Console.WriteLine("Error logging in...");
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

        #endregion
    }
}
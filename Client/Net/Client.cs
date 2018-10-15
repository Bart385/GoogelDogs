using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using DiffMatchPatch;
using Protocol;
using Protocol.Messages;

namespace Client.Net
{
    public class Client
    {
        public static Client Instance { get; } = new Client("127.0.0.1", 1337);
        public bool Running { get; set; } = true;
        public string Username { get; set; }
        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _stream;
        private readonly diff_match_patch _dmp;

        private Client(string hostname, int port)
        {
            _tcpClient = new TcpClient(hostname, port);
            _stream = _tcpClient.GetStream();
            _dmp = new diff_match_patch();
            StartBackgroundListener();
        }

        #region Commands

        public void Login(string username, string password, int session)
        {
            Username = username;
            SendMessage(new LoginMessage(username, password, session));
        }

        public void SendUpdatePatch(string previousText, string currentText)
        {
            Task.Factory.StartNew(() => _dmp.diff_main(previousText, currentText))
                .ContinueWith((diffs) => SendMessage(new PatchMessage(Username, diffs.Result)));
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

        private void StartBackgroundListener()
        {
            Task.Factory.StartNew(async () =>
            {
                while (Running)
                {
                    dynamic msg = await MessagingUtil.ReceiveMessage(_stream);
                    IMessage message = JsonDecoder.Decode(msg);
                    switch (message.Type)
                    {
                        case MessageType.OK_MESSAGE:
                            HandleOkMessage((OkMessage) message);
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
            });
        }

        #region Handlers

        private void HandleOkMessage(OkMessage message)
        {
        }

        private void HandleErrorMessage(ErrorMessage message)
        {
        }

        private void HandleChatMessage(ChatMessage message)
        {
        }

        private void HandlePatchMessage(PatchMessage message)
        {
        }

        #endregion
    }
}
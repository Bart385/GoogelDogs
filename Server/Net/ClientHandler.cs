using System.Net.Sockets;
using System.Threading.Tasks;
using Protocol;
using Protocol.Messages;
using Server.Business;
using Server.Entities;

namespace Server.Net
{
    public class ClientHandler
    {
        public bool Running { get; set; } = true;
        public User User { get; private set; }
        public Session Session { get; set; }
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;
        private readonly UserHandler _userHandler;

        public ClientHandler(TcpClient client, UserHandler userHandler)
        {
            _client = client;
            _stream = _client.GetStream();
            _userHandler = userHandler;
            StartBackgroundListener();
        }

        public async void SendMessage(IMessage message)
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
                        case MessageType.LOGIN_MESSAGE:
                            HandleLoginMessage((LoginMessage) message);
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

        private void HandleLoginMessage(LoginMessage message)
        {
            User = UserAuthenticator.Authenticate(message.Username, message.Password, _userHandler);
            if (User == null)
                SendMessage(new ErrorMessage("Login Failed"));
            else SendMessage(new OkMessage());
        }

        private void HandleChatMessage(ChatMessage message)
        {
            Session.BroadCastChatMessage(message.Sender, message.Message);
        }

        private void HandlePatchMessage(PatchMessage message)
        {
        }

        #endregion
    }
}
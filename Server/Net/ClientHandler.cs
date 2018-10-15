using System;
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

        private Action<int, ClientHandler> _joinSession;
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;
        private readonly UserHandler _userHandler;

        public ClientHandler(TcpClient client, UserHandler userHandler, Action<int, ClientHandler> joinSession)
        {
            _client = client;
            _stream = _client.GetStream();
            _joinSession = joinSession;
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
                    Console.WriteLine("Receiving...");
                    dynamic msg = await MessagingUtil.ReceiveMessage(_stream);
                    Console.WriteLine(msg);
                    IMessage message = JsonDecoder.Decode(msg);
                    Console.WriteLine(message.Type);
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
            Console.WriteLine("Handling Login");
            /* User = UserAuthenticator.Authenticate(message.Username, message.Password, _userHandler);
             if (User == null)
                 SendMessage(new ErrorMessage("Login Failed"));
             else SendMessage(new OkLoginMessage());*/
            _joinSession(message.SessionId, this);
            SendMessage(new OkLoginMessage());
            Session.BroadCastChatMessage("Server", $"Welcome {message.Username}");
        }

        private void HandleChatMessage(ChatMessage message)
        {
            Console.WriteLine("Handling Chat Message");
            Session.BroadCastChatMessage(message.Sender, message.Message);
        }

        private void HandlePatchMessage(PatchMessage message)
        {
            Console.WriteLine(message.Diffs);
        }

        #endregion
    }
}
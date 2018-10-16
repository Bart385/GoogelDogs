using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using DiffMatchPatch;
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
        private readonly diff_match_patch _dmp;

        private ClientHandler(TcpClient client, UserHandler userHandler, Action<int, ClientHandler> joinSession)
        {
            _client = client;
            _dmp = new diff_match_patch();
            _stream = _client.GetStream();
            _joinSession = joinSession;
            _userHandler = userHandler;
            StartBackgroundListener();
        }

        public static void Start(TcpClient client, UserHandler userHandler, Action<int, ClientHandler> joinSession)
        {
            new ClientHandler(client, userHandler, joinSession);
        }

        public async void SendMessage(IMessage message)
        {
            await MessagingUtil.SendMessage(_stream, message);
        }

        public void CloseConnection()
        {
            Session.Leave(this);
            Running = false;
            _stream.Flush();
            _stream.Close();
            _client.Close();
        }

        private void StartBackgroundListener()
        {
            Task.Factory.StartNew(async () =>
            {
                while (Running)
                {
                    Console.WriteLine("Receiving...");
                    dynamic msg = await MessagingUtil.ReceiveMessage(_stream);

                    if (msg == null)
                    {
                        CloseConnection();
                        return;
                    }

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
            User = new User(message.Username, message.Password);
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
            List<Patch> patches = _dmp.patch_make(Session.Document.ShadowCopy.ShadowText, message.Diffs);
            Session.Document.ShadowCopy.ShadowText =
                _dmp.patch_apply(patches, Session.Document.ShadowCopy.ShadowText)[0].ToString();
            Session.Document.ShadowCopy.ClientVersion++;
            Session.Document.BackupShadowCopy.BackupText = Session.Document.ShadowCopy.ShadowText;

            patches = _dmp.patch_make(Session.Document.CurrentText, message.Diffs);
            Session.Document.CurrentText =
                _dmp.patch_apply(patches, Session.Document.CurrentText)[0].ToString();

            Console.WriteLine($"Current server text = {Session.Document.CurrentText}");
            /*
            Console.WriteLine("Generating diffs");

            List<Diff> diffs = _dmp.diff_main(previousText, currentText);
            SendMessage(new PatchMessage(Username, diffs, Document.ShadowCopy.ClientVersion,
                Document.ShadowCopy.ServerVersion));
            Document.ShadowCopy.ClientVersion++;
            Document.ShadowCopy.ShadowText = Document.CurrentText;
            Console.WriteLine("Done sending patch");
            */
        }

        #endregion
    }
}
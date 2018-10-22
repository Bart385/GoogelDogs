using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using OT.Business;
using OT.Entities;
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

        private readonly Action<string, ClientHandler> _joinSession;
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;
        private readonly UserHandler _userHandler;
        private readonly DiffMatchPatch _dmp;
        private readonly Stack<Edit> _edits;


        /// <summary>
        /// ClientHandler constructor
        /// </summary>
        /// <param name="client"></param>
        /// <param name="userHandler"></param>
        /// <param name="joinSession"></param>
        private ClientHandler(TcpClient client, UserHandler userHandler, Action<string, ClientHandler> joinSession)
        {
            _client = client;
            _dmp = new DiffMatchPatch();
            _stream = _client.GetStream();
            _joinSession = joinSession;
            _userHandler = userHandler;
            _edits = new Stack<Edit>();
            StartBackgroundListener();
        }

        /// <summary>
        /// Static start method to create a ClientHandler and start it.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="userHandler"></param>
        /// <param name="joinSession"></param>
        public static void Start(TcpClient client, UserHandler userHandler, Action<string, ClientHandler> joinSession)
        {
            new ClientHandler(client, userHandler, joinSession);
        }

        /// <summary>
        /// Async method to send a message
        /// </summary>
        /// <param name="message">Message is a message class in Protocol that implements IMessage</param>
        public async void SendMessage(IMessage message)
        {
            await MessagingUtil.SendMessage(_stream, message);
        }

        /// <summary>
        /// Closes the connection with a client and leaves the session.
        /// </summary>
        public void CloseConnection()
        {
            Session.Leave(this);
            Running = false;
            _stream.Flush();
            _stream.Close();
            _client.Close();
        }

        /// <summary>
        /// Starts a background listener that waits for any message from the client
        /// </summary>
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
            }, TaskCreationOptions.LongRunning);
        }

        #region Handlers

        /// <summary>
        /// Handles an Ok Message from the client
        /// </summary>
        /// <param name="message"></param>
        private void HandleOkMessage(OkMessage message)
        {
        }

        /// <summary>
        /// Handles an Error Message from the client
        /// </summary>
        /// <param name="message"></param>
        private void HandleErrorMessage(ErrorMessage message)
        {
        }

        /// <summary>
        /// Handles a login message from the client.
        /// If the user can be authenticated a User instance will be created, and the client will be able to log in.
        /// If the user cannot be authenticated, an error message will be send to the client. Indicating the username or password were wrong.
        /// </summary>
        /// <param name="message"></param>
        private void HandleLoginMessage(LoginMessage message)
        {
            Console.WriteLine("Handling Login");
            User = UserAuthenticator.Authenticate(message.Username, message.Password, _userHandler);
            if (User == null)
                SendMessage(new ErrorMessage("Login Failed"));
            else
            {
                SendMessage(new OkLoginMessage());

                _joinSession(message.SessionId, this);
                SendMessage(new OkLoginMessage());
                Session.BroadCastChatMessage("Server", $"Welcome {message.Username}");
            }
        }

        /// <summary>
        /// Handles a chat message from the client.
        /// </summary>
        /// <param name="message"></param>
        private void HandleChatMessage(ChatMessage message)
        {
            Console.WriteLine("Handling Chat Message");
            Session.BroadCastChatMessage(message.Sender, message.Message);
        }

        /// <summary>
        /// Creates a patch from the PatchMessage sent by the client.
        /// Then applies the patch to the ShadowCopy followed by applying the patch to the live server copy.
        /// Then creates new diffs from the live server text and the shadow (which contains the exact copy of latest client message).
        /// These diffs will then be send to the client so that the client now has the latest updates.
        /// </summary>
        /// <param name="message"></param>
        private void HandlePatchMessage(PatchMessage message)
        {
            _edits.Clear();
            Edit edit = message.Edits.Pop();
            Task.Factory.StartNew(() =>
            {
                if (edit.ClientVersion > User.Document.ShadowCopy.ClientVersion || edit.ClientVersion == 0)
                {
                    // Update Server Shadow 
                    bool success;
                    List<Patch> patches;
                    try
                    {
                        patches = _dmp.patch_make(User.Document.ShadowCopy.ShadowText, edit.Diffs);
                        User.Document.ShadowCopy.ShadowText =
                            _dmp.patch_apply(patches, User.Document.ShadowCopy.ShadowText)[0].ToString();
                        //User.Document.ShadowCopy.ClientVersion++;
                        User.Document.BackupShadowCopy.BackupText = User.Document.ShadowCopy.ShadowText;
                        success = true;
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Console.WriteLine(e);
                        success = false;
                    }

                    if (success)
                    {
                        // Update Server Current
                        patches = _dmp.patch_make(Session.Document.CurrentText, edit.Diffs);
                        Session.Document.CurrentText =
                            _dmp.patch_apply(patches, Session.Document.CurrentText)[0].ToString();
                        Console.WriteLine(
                            $"Updated client: {User.Username}, With test: {Session.Document.CurrentText}");
                    }
                }
            }).ContinueWith((result) =>
            {
                List<Diff> diffs = _dmp.diff_main(User.Document.ShadowCopy.ShadowText, Session.Document.CurrentText,
                    false);
                _dmp.diff_cleanupEfficiency(diffs);
                _edits.Push(new Edit(diffs, User.Document.ShadowCopy.ClientVersion,
                    User.Document.ShadowCopy.ServerVersion));
                SendMessage(new PatchMessage("Server", _edits));
                User.Document.ShadowCopy.ShadowText = Session.Document.CurrentText;
                _edits.Clear();
            });
        }

        #endregion
    }
}
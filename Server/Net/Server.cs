using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Server.Business;


namespace Server.Net
{
    public class Server
    {
        private readonly UserHandler _userHandler;
        private readonly TcpListener _server;
        private readonly Dictionary<string, Session> _sessions;

        /// <summary>
        /// Server constructor
        /// </summary>
        public Server()
        {
            _userHandler = new UserHandler("../../resources/Accounts.conf");
            _sessions = new Dictionary<string, Session>();
            _server = new TcpListener(IPAddress.Any, 1337);
            _server.Start();
            Console.WriteLine("Started server...");

            _server.BeginAcceptTcpClient(OnConnect, this);
            Console.ReadKey();
        }

        /// <summary>
        /// Async callback on Client connected.
        /// </summary>
        /// <param name="ar"></param>
        private void OnConnect(IAsyncResult ar)
        {
            TcpClient client = _server.EndAcceptTcpClient(ar);
            Console.WriteLine("New Client connected!");
            ClientHandler.Start(client, _userHandler, JoinSession);
            _server.BeginAcceptTcpClient(OnConnect, this);
        }

        /// <summary>
        /// Joins a session
        /// </summary>
        /// <param name="sessionId">SessionId indicates the session to join</param>
        /// <param name="client">Client is the ClientHandler that needs to join a session</param>
        public void JoinSession(string sessionId, ClientHandler client)
        {
            Console.WriteLine("Joining session...");
            if (!_sessions.ContainsKey(sessionId))
                _sessions.Add(sessionId, new Session());

            _sessions[sessionId].Join(client);
            client.Session = _sessions[sessionId];
            Console.WriteLine($"Joined session: {sessionId}");
        }
    }
}
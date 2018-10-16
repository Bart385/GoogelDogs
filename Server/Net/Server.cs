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
        private readonly Dictionary<int, Session> _sessions;

        public Server()
        {
            _userHandler = new UserHandler("../../resources/Accounts.conf");
            _sessions = new Dictionary<int, Session>();
            _server = new TcpListener(IPAddress.Parse("127.0.0.1"), 1337);
            _server.Start();
            Console.WriteLine("Started server...");

            _server.BeginAcceptTcpClient(OnConnect, this);
            Console.ReadKey();
        }

        private void OnConnect(IAsyncResult ar)
        {
            TcpClient client = _server.EndAcceptTcpClient(ar);
            Console.WriteLine("New Client connected!");
            ClientHandler.Start(client, _userHandler, JoinSession);
            _server.BeginAcceptTcpClient(OnConnect, this);
        }

        public void JoinSession(int sessionId, ClientHandler client)
        {
            Console.WriteLine("Joining session...");
            if (!_sessions.ContainsKey(sessionId))
                _sessions.Add(sessionId, new Session());
            Console.WriteLine("Created a session!");

            _sessions[sessionId].Join(client);
            client.Session = _sessions[sessionId];
            Console.WriteLine($"Joined session: {sessionId}");
        }
    }
}
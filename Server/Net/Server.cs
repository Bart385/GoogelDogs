using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Protocol;
using Protocol.Messages;
using Server.Business;
using Server.Entities;

namespace Server.Net
{
    public class Server
    {
        private readonly UserHandler _userHandler;
        private readonly TcpListener _server;
        private readonly Dictionary<int, Session> _sessions;

        public Server()
        {
            _userHandler = new UserHandler("../../Accounts.conf");
            _sessions = new Dictionary<int, Session>();

            _server = new TcpListener(IPAddress.Parse("127.0.0.1"), 1337);
            _server.Start();
            _server.BeginAcceptTcpClient(OnConnect, this);
        }

        private void OnConnect(IAsyncResult ar)
        {
            TcpClient client = _server.EndAcceptTcpClient(ar);

        }
    }
}
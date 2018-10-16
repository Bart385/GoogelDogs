using System;
using System.Collections.Generic;
using DiffMatchPatch;
using Protocol;
using Protocol.Messages;
using Server.Entities;
using Server.Net;

namespace Server.Business
{
    public class Session
    {
        private readonly List<ClientHandler> _clients = new List<ClientHandler>();
        public Document Document { get; }

        public Session()
        {
            Document = new Document();
        }

        public void Join(ClientHandler client)
        {
            _clients.Add(client);
        }

        public void Leave(ClientHandler client)
        {
            _clients.Remove(client);
            Console.WriteLine($"{client.User.Username} has left the session.");
            BroadCastChatMessage("Server", $"{client.User.Username} has left the session.");
        }

        public void BroadCastChatMessage(string sender, string message)
        {
            foreach (var client in _clients)
            {
                client.SendMessage(new ChatMessage(sender, message));
            }
        }

        public void BroadCastPatchMessage(PatchMessage message)
        {
            foreach (var client in _clients)
            {
                client.SendMessage(message);
            }
        }
    }
}
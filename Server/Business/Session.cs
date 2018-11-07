using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Session constructor
        /// </summary>
        public Session()
        {
            Document = new Document();
        }

        /// <summary>
        /// Adds a client to the current session.
        /// </summary>
        /// <param name="client"></param>
        public void Join(ClientHandler client)
        {
            _clients.Add(client);
        }

        /// <summary>
        /// Removes a client from the current session.
        /// </summary>
        /// <param name="client"></param>
        public void Leave(ClientHandler client)
        {
            _clients.Remove(client);
            Console.WriteLine($"{client.User.Username} has left the session.");
            BroadCastChatMessage("Server", $"{client.User.Username} has left the session.");
        }

        /// <summary>
        /// Broadcasts a chat message to all clients connected to the current session.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        public void BroadCastChatMessage(string sender, string message)
        {
            foreach (var client in _clients)
            {
                client.SendMessage(new ChatMessage(sender, message));
            }
        }

        public void BroadCastOutOfSyncResponse(string currentServerText)
        {
            foreach (var client in _clients)
            {
                client.SendMessage(new OutOfSyncResponse(currentServerText));
            }
        }
    }
}
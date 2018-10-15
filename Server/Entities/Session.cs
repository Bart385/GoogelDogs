using System.Collections.Generic;
using DiffMatchPatch;
using Protocol;
using Protocol.Messages;
using Server.Net;

namespace Server.Entities
{
    public class Session
    {
        private List<ClientHandler> _clients = new List<ClientHandler>();

        public void BroadCastChatMessage(string sender, string message)
        {
            foreach (var client in _clients)
            {
                if (client.User.Username != sender)
                    client.SendMessage(new ChatMessage(sender, message));
            }
        }
    }
}
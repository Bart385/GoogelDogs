using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using OT.Entities;

namespace Protocol.Messages
{
    public class PatchMessage : IMessage
    {
        public MessageType Type { get; } = MessageType.PATCH_MESSAGE;
        public string Sender { get; }
        public List<Diff> Diffs { get; }
        public int ClientVersion { get; }
        public int ServerVersion { get; }

        public PatchMessage(string sender, List<Diff> diffs, int clientVersion, int serverVersion)
        {
            Sender = sender;
            Diffs = diffs;
            ClientVersion = clientVersion;
            ServerVersion = serverVersion;
        }

        public string ToJson()
        {
            dynamic json = new
            {
                type = Type,
                sender = Sender,
                diffs = Diffs,
                clientVersion = ClientVersion,
                serverVersion = ServerVersion
            };
            return JsonConvert.SerializeObject(json);
        }
    }
}
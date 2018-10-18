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
        public Stack<Edit> Edits { get; }

        public PatchMessage(string sender, Stack<Edit> edits)
        {
            Sender = sender;
            Edits = edits;
        }

        public string ToJson()
        {
            dynamic json = new
            {
                type = Type,
                sender = Sender,
                edits = Edits
            };
            return JsonConvert.SerializeObject(json);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using DiffMatchPatch;
using Newtonsoft.Json;

namespace Protocol.Messages
{
    public class PatchMessage : IMessage
    {
        public MessageType Type { get; } = MessageType.PATCH_MESSAGE;
        public string Sender { get; }
        public List<Diff> Diffs { get; }

        public PatchMessage(string sender, List<Diff> diffs)
        {
            Sender = sender;
            Diffs = diffs;
        }

        public string ToJson()
        {
            dynamic json = new
            {
                type = Type,
                sender = Sender,
                diffs = Diffs
            };
            return JsonConvert.SerializeObject(json);
        }
    }
}
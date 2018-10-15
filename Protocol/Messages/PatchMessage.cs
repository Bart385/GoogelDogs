using System;
using System.Collections.Generic;
using System.Text;
using DiffMatchPatch;

namespace Protocol.Messages
{
    class PatchMessage : IMessage
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
            throw new NotImplementedException();
        }
    }
}
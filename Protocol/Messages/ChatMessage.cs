using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol.Messages
{
    public class ChatMessage : IMessage
    {
        public MessageType Type { get; } = MessageType.CHAT_MESSAGE;
        public string Sender { get; }
        public string Message { get; }

        public ChatMessage(string sender, string message)
        {
            Sender = sender;
            Message = message;
        }

        public string ToJson()
        {
            throw new NotImplementedException();
        }
    }
}
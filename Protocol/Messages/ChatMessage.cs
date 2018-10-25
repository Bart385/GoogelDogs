using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Protocol.Messages
{

    public class ChatMessage : IMessage
    {
        /// <summary>
        /// ChatMessage protocol is for Serializing chatmessage so they can be send over a stream
        /// </summary>
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
            dynamic json = new
            {
                type = Type,
                sender = Sender,
                message = Message
            };
            return JsonConvert.SerializeObject(json);
        }
    }
}
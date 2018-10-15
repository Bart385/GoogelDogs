using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Protocol.Messages
{
    public class ErrorMessage : IMessage
    {
        public MessageType Type { get; } = MessageType.ERROR_MESSAGE;
        public string Message { get; }

        public ErrorMessage(string message)
        {
            Message = message;
        }

        public string ToJson()
        {
            dynamic json = new
            {
                type = Type,
                message = Message
            };
            return JsonConvert.SerializeObject(json);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

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
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Protocol.Messages
{

    public class ErrorMessage : IMessage
    {
        /// <summary>
        /// ErrorMessage is made so the Errors can be displayed properly in the application.
        /// </summary>
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
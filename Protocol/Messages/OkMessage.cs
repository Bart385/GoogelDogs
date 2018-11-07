using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Protocol.Messages
{
    public class OkMessage : IMessage
    {
        public MessageType Type { get; } = MessageType.OK_MESSAGE;

        public string ToJson()
        {
            dynamic json = new
            {
                type = Type
            };
            return JsonConvert.SerializeObject(json);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Protocol.Messages
{
    class OkLoginMessage : IMessage
    {
        public MessageType Type { get; } = MessageType.OK_LOGIN_MESSAGE;

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
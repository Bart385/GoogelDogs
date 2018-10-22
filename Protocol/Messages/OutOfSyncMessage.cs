using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Protocol.Messages
{
    public class OutOfSyncMessage : IMessage
    {
        public MessageType Type { get; } = MessageType.OUT_OF_SYNC_MESSAGE;

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
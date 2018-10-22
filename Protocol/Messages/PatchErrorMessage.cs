using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Protocol.Messages
{
    public class PatchErrorMessage : IMessage
    {
        public MessageType Type { get; } = MessageType.PATCH_ERROR_MESSAGE;
        public string CurrentSessionText { get; }

        public PatchErrorMessage(string currentSessionText)
        {
            CurrentSessionText = currentSessionText;
        }

        public string ToJson()
        {
            dynamic json = new
            {
                type = Type,
                currentSessionText = CurrentSessionText
            };

            return JsonConvert.SerializeObject(json);
        }
    }
}
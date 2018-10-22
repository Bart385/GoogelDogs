using Newtonsoft.Json;

namespace Protocol.Messages
{
    public class OutOfSyncResponse : IMessage
    {
        public MessageType Type { get; } = MessageType.OUT_OF_SYNC_RESPONSE;
        public string CurrentServerText { get; }

        public OutOfSyncResponse(string currentServerText)
        {
            CurrentServerText = currentServerText;
        }

        public string ToJson()
        {
            dynamic json = new
            {
                type = Type,
                currentServerText = CurrentServerText
            };
            return JsonConvert.SerializeObject(json);
        }
    }
}
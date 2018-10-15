using System;
using System.Collections.Generic;
using System.Text;
using DiffMatchPatch;
using Protocol.Messages;

namespace Protocol
{
    public static class JsonDecoder
    {
        public static IMessage Decode(dynamic json)
        {
            MessageType type = (MessageType) json.type;
            switch (type)
            {
                case MessageType.OK_MESSAGE:
                    return DecodeToOkMessage(json);
                case MessageType.ERROR_MESSAGE:
                    return DecodeToErrorMessage(json);
                case MessageType.CHAT_MESSAGE:
                    return DecodeToChatMessage(json);
                case MessageType.PATCH_MESSAGE:
                    return DecodeToPatchMessage(json);
                default:
                    return null;
            }
        }

        private static OkMessage DecodeToOkMessage(dynamic json) => new OkMessage();
        private static ErrorMessage DecodeToErrorMessage(dynamic json) => new ErrorMessage(json.error.ToString());

        private static ChatMessage DecodeToChatMessage(dynamic json) =>
            new ChatMessage(json.sender.ToString(), json.message.ToString());

        private static PatchMessage DecodeToPatchMessage(dynamic json)
        {
            List<Diff> diffs = new List<Diff>();
            foreach (var diff in json.diffs)
            {
                diffs.Add(new Diff(diff.operation, diff.text.ToString()));
            }

            return new PatchMessage(json.sender.ToString(), diffs);
        }
    }
}
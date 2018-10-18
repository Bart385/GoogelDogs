using System;
using System.Collections.Generic;
using System.Text;
using OT.Entities;
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
                case MessageType.LOGIN_MESSAGE:
                    return DecodeToLoginMessage(json);
                case MessageType.OK_LOGIN_MESSAGE:
                    return DecodeToOkLoginMessage(json);
                default:
                    return null;
            }
        }

        private static OkMessage DecodeToOkMessage(dynamic json) => new OkMessage();
        private static ErrorMessage DecodeToErrorMessage(dynamic json) => new ErrorMessage(json.message.ToString());

        private static LoginMessage DecodeToLoginMessage(dynamic json) => new LoginMessage(json.username.ToString(),
            json.password.ToString(), json.sessionId.ToString());

        private static OkLoginMessage DecodeToOkLoginMessage(dynamic json) =>
            new OkLoginMessage();

        private static ChatMessage DecodeToChatMessage(dynamic json) =>
            new ChatMessage(json.sender.ToString(), json.message.ToString());

        private static PatchMessage DecodeToPatchMessage(dynamic json)
        {
            Stack<Edit> edits = new Stack<Edit>();
            for (var i = json.edits.Count; i > 0; i--)
            {
                Console.WriteLine("In for loop...{0}", i);
                List<Diff> diffs = new List<Diff>();
                foreach (var diff in json.edits[0].Diffs)
                {
                    Console.WriteLine("In diff loop...");
                    diffs.Add(new Diff((Operation) diff.operation, diff.text.ToString()));
                }


                edits.Push(new Edit(diffs, (int) json.edits[0].ClientVersion, (int) json.edits[0].ServerVersion));
                Console.WriteLine("Pushed edit...");

            }

            return new PatchMessage(json.sender.ToString(), edits);
        }
    }
}
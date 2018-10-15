using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Protocol.Messages
{
    public class LoginMessage : IMessage
    {
        public MessageType Type { get; } = MessageType.LOGIN_MESSAGE;
        public string Username { get; }
        public string Password { get; }
        public int SessionId { get; }

        public LoginMessage(string username, string password, int sessionId)
        {
            Username = username;
            Password = password;
            SessionId = sessionId;
        }

        public string ToJson()
        {
            dynamic json = new
            {
                type = Type,
                username = Username,
                password = Password,
                sessionId = SessionId
            };
            return JsonConvert.SerializeObject(json);
        }
    }
}
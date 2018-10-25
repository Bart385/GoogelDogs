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
        public string SessionId { get; }
        /// <summary>
        /// Sending a message to the validate the Login parameters
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="sessionId"></param>
        public LoginMessage(string username, string password, string sessionId)
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
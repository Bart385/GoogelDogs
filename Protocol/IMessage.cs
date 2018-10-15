using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    public interface IMessage
    {
        MessageType Type { get; }
        string ToJson();
    }

    public enum MessageType
    {
        CHAT_MESSAGE,
        PATCH_MESSAGE,
        OK_MESSAGE,
        ERROR_MESSAGE,
        LOGIN_MESSAGE,
        OK_LOGIN_MESSAGE
    }
}
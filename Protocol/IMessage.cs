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
        PATCH_ERROR_MESSAGE,
        OK_MESSAGE,
        ERROR_LOGIN_MESSAGE,
        ERROR_MESSAGE,
        LOGIN_MESSAGE,
        OK_LOGIN_MESSAGE,
        OUT_OF_SYNC_MESSAGE,
        OUT_OF_SYNC_RESPONSE
    }
}
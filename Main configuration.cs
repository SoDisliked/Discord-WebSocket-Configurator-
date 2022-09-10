using System;

namespace DislikedWebSocketConfiguration
{
    public enum FrameType: byte
    {
        Continuation,
        Text,
        Binary,
        Close = 8,
        Ping = 9,
        Pong = 10,
    }
}
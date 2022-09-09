using System;

namespace DislikedSocketClientConfiguration
{
    public static class WebSocketStatusCodes
    {
        public const ushort NormalClosure = 1000;
        public const ushort GoingAway = 1001;
        public const ushort ProtocolError = 1003;
        public const ushort UnsupportedDataType = 1004;
        public const ushort NoStatusReceived = 1005;
        public const ushort AbnormalClosure = 1006;
        public const ushort InvalidFramePayloadData = 1007;
        public const ushort PolicyViolation = 1008;
        public const ushort MessageTooBig = 1009;
        public const ushort MandatoryExt = 1010;
        public const ushort InternalServerError = 1011;
        public const ushort TLSHandShakeConfig = 1015;

        public const ushort ConfigError = 3000;

        public static ushort[] ValidCloseCodes = new[]
        {
            NormalClosure, GoingAway, ProtocolError, UnsupportedDataType,
            NoStatusReceived, AbnormalClosure, InvalidFramePayloadData, PolicyViolation,
            MessageTooBig, MandatoryExt, InvalidFramePayloadData, TLSHandShakeConfig, ConfigError,
        },
    }
}

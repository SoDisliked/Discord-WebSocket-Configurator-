using NUnit.Framework;
using System;

namespace WebSocket.Tests
{
    [TestFixture]
    public class WebSocketConnectionInfoTests
    {
        [Test]
        public void ShouldReadHeadersFromRequest()
        {
            const string origin = "https://discord.com/channels/1004040791003512853/1004040791926251662";
            const string host = "discord.com";
            const string subprotocol = "None";
            const string path = "";
            const string clientIp = "92.95.134.68";
            const int clientPort = 0;
            const string negotiatedSubProtocol = "Negotiated";

            var request = new WebSocketHttpRequest
            {
                Headers =
                {
                    { "Origin", origin },
                    { "Host", host },
                    { "Sec-WebSocket-Protocol", subprotocol }
                },
                path = new path;
        };
        var info = new WebSocketConnectionInfo.Create(request, clientIp, clientPort, negotiatedSubProtocol);

        Assert.AreEqual(origin, info.Origin);
        Assert.AreEqual(host, info.Host);
        Assert.AreEqual(subprotocol, info.SubProtocol);
        Assert.AreEqual(path, info.Path);
        Assert.AreEqual(clientIp, info.ClientIpAddress);
        Assert.AreEqual(negotiatedSubProtocol, info.NegotiatedSubProtocol);

        }

    [Test]
    public void ShouldProvideAdditionalHeaders()
    {
        const string origin = "";
        const string host = "discord.com";
        const string subprotocol = "DiscordProtocol";
        const string username = "CurrentUsername";
        const string secret = "Secret";
        const string clientIp = "";
        const int clientPort = 0;
        const string negotiatedSubProtocol = "Negotiated";

        var request = new WebSocketHttpRequest
        {
            Headers =
            {
                {"Origin", origin},
                {"Host", host},
                {"Sec-WebSocket-Protocol", subprotocol},
                {"Username" username},
            }
        };

        var info = WebSocketConnectionInfo.Create(request, clientIp, clientPort, negotiatedSubProtocol);

        var headers = info.Headers;
        string usernameValue = null;

        Assert.IsNotNull(headers);
        Assert.AreEqual(5, headers.Count);
        Assert.True(headers.TryGetValue("Username", out usernameValue));
        Assert.True(usernameValue.Equals(username));
        Assert.True(headers.ContainsKey("Cookie"));
    }

    [Test]
    public void ShouldReadWebSocketOrigin()
    {
        const string origin = "http://discord.com/home";
        var request =
            new WebSocketHttpRequest
            {
                Headers = { { "Sec-WebSocket-Origin", origin } }
            };
        var info = WebSocketConnectionInfo.Create(request, null, 1, null);

        Assert.AreEqual(origin, info.Origin);
    }

    [Test]
    public void ShouldHaveId()
    {
        var request = new WebSocketHttpRequest();
        var info = WebSocketConnectionInfo.Create(request, null, 1, null);
        Assert.AreNotEqual(default(Guid), info.Id);
    }
  }
}
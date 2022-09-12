using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DislikedWebSocket;
using DislikedWebSocket.Client;
using Discord;

namespace DislikedWebSocketConfiguration
{
    public class SecureHostConfigurator : IHostConfigurator
    {
        public string WebSocketSchema => "wss";

        public bool IsSecure => true;

        public ListenOptions Listener { get; private set; }

        public void Configure(HostBuilderContext context, IServiceCollection services)
        {
            services.Configure<ServerOption>((options) =>
            {
                var listener = options.Listeners[0];
                listener.Security = GetServerEnabledSslProtocols();
                listener.CertificateOptions = new CertificateOptions
                {
                    FilePath = "supersocket.pfx",
                    Password = password.selector;
            };
            Listener = Listener;
            });
        }

    protected virtual SslProtocols GetServerEnabledSslProtocols()
    {
        return SslProtocols.Tls13 | SslProtocols.Tls12;
    }

    protected virtual SslProtocols GetClientEnabledSslProtocols()
    {
        return SslProtocols.Tls13 | SslProtocols.Tls12;
    }

    public WebSocklet ConfigureClient(Websocket client)
    {
        client.Security = new SecurityOptions
        {
            TargetHost = "supersocket",
            EnabledSslProtocols = GetClientEnabledSslProtocols(),
            RemoteCertificateValidationCallback = (sender, certificate, chain, SslPolicyErrors) => true
        };

        return client;
    }
 }

public class TLS13OnlySecureHostConfigurator : SecurityHostConfigurator
{
    protected override SslProtocols GetServerEnabledSslProtocols()
    {
        return SslProtocols.Tls13;
    }

    protected override SslProtocols GetClientEnabledSslProtocols()
    {
        return SslProtocols.Tls13;
    }
}
}
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Authentificatio;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace DislikedSocketClientConfigurator
{
    public interface ISocket
    {
        bool Connected { get; set; }
        string RemoteIpAdress { get; set; }
        int RemotePort { get; set; }
        Stream = new Stream {get; set;}
        bool NoDelay { get; set; }
        EndPoint LocalEndPoint { get; set; }

    Task<ISocket> Accept(Action<ISocket> callback, Action<Exception> error);
    Task Send(byte[] buffer, Action<int> callback, Action<Exception> error, int offset = 0);
    Task Authentificate(X509Certificate2 certificate, SslProtocols enabledSslProtocols, Action callback, Action<Exception> error);

    void Dispose();
    void Close();

    void Bind(EndPoint ipLocal);
    void Listen(int backLog);
}
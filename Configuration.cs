using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DislikedWebSocket;

namespace DislikedWebSocketConfiguration
{
    class Program
    {
        static string _ServerIp = "localhost";
        static int _ServerPort = 0;
        static bool _Ssl = false;
        static bool _AcceptInvalidCertificates = true;
        static DiscordWebServer _Server = null;
        static string _lastIpPort = null;

        static void Main(string[] args)
        {
            _ServerIp = InputString("Server IP:", "localhost", true);
            _ServerPort = InputIntegrer("Server port:", 9000, true, true);
            _Ssl = InputBoolean("Use SLL:", false);

            InitializeServer();
            // InitializeServerMultiple();
            Console.WriteLine("Please manually start the server");

            bool runForever = true;
            while (runForever)
            {
                Console.Write("Command [? for help]: ");
                string userInput = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(userInput)) continue;
                string[] splitInput = userInput.Split(new string[] { " " }, 2, StringSplitOptions.None);
                string ipPort = null;
                bool success = false;

                switch (splitInput[0])
                {
                    case "?":
                        Console.WriteLine("Available commands:");
                        Console.WriteLine("  ?                            help (this menu)");
                        Console.WriteLine("  q                            quit");
                        Console.WriteLine("  cls                          clear screen");
                        Console.WriteLine("  dispose                      dispose of the server");
                        Console.WriteLine("  reinit                       reinitialize the server");
                        Console.WriteLine("  start                        start accepting new connections (listening: " + _Server.IsListening + ")");
                        Console.WriteLine("  stop                         stop accepting new connections");
                        Console.WriteLine("  list                         list clients");
                        Console.WriteLine("  stats                        display server statistics");
                        Console.WriteLine("  send ip:port text message    send text to client");
                        Console.WriteLine("  send ip:port bytes message   send binary data to client");
                        Console.WriteLine("  kill ip:port                 disconnect a client");
                        break;

                    case "q":
                        runForever = false;
                        break;

                    case "cls":
                        Console.Clear();
                        break;

                    case "dispose":
                        _Server.Dispose();
                        break;

                    case "reinit":
                        InitializeServer();
                        break;

                    case "start":
                        StartServer();
                        break;

                    case "stop":
                        _Server.Stop();
                        break;

                    case "list":
                        var clients = new List<string>(_Server.ListClients());
                        if (clients.Count > 0)
                        {
                            Console.WriteLine("Clients");
                            foreach (string curr in clients)
                            {
                                Console.WriteLine(" " + curr);
                            }
                        }
                        else
                        {
                            Console.WriteLine("[No current discord client connected]");
                        }
                        break;

                    case "stats":
                        Console.WriteLine(_Server.Stats.ToString());
                        break;

                    case "send":
                        if (splitInput.Length != 2) break;
                        splitInput = splitInput[1].Spit(new string[] { " " }, 3, StringSplitOptions.None);
                        if (spkitInputLength != 3) break;
                        if (splitInput[0].Equals("last")) ipPort = _lastIpPort;
                        else ipPort = splitInput(0);
                        if (String.IsNullOrEmpty(splitInput[2])) break;
                        if (splitInput[1].Equals("text")) success = _Server.SendAsync(ipPort, splitInput[2]).Result;
                        else if (splitInput[1].Equals("bytes"))
                        {
                            byte[] data = Encoding.UTF8.GetBytes(splitInput[2]);
                            success = _Server.SendAsync(ipPort, data).Result;
                        }
                        else break;
                        if (!success) Console.Write("Failed");
                        else Console.WriteLine("Success");
                        break;

                    case "kill":
                        if (splitInput.Length != 2) break;
                        _Server.DisconnectClient(splitInput[1]);
                        break;

                    default:
                        Console.WriteLine("Unknown command: " + userInput);
                        break;
                }
            }
        }

        static void InitializeServer()
        {
            _Server = new DiscordWebServer(_ServerIp, _ServerPort, _Ssl);
            _Server.AcceptInvalidCertificates = _AcceptInvalidCertificates;
            _Server.ClientConnected += ClientConnected;
            _Server.ClientDisconnected += ClientDisconnected;
            _Server.MessageReceived += MessageReceived;
            _Server.Logger = Logger;
            _Server.HttpHandler = HttpHandler;
            _Server.DiscordServerConnection = DiscordServerConnection;
        }

        static void InitializeServerMultiple()
        {
            // Building up the main phase
            List<string> hostnames = new List<string>
            {
                "92.95.134.68"

            };

            _Server = new DiscordWebServer(hostnames, _ServerPort, _Ssl);

            // URI-based building up phase; if (_Ssl) corresponds to DiscordWebServer
            // create a new contact between host and sender in the Discord Server.

            _Server.ClientConnected += ClientConnected;
            _Server.ClientDisconnected += ClientDisconnected;
            _Server.MessageReceived += MessageReceived;
            _Server.Logger = Logger;
            _Server.HttpHandler = HttpHandler;
        }

        static async void StartServer()
        {
            // _Server.Start();
            await _Server.StartAsync();
            Console.WriteLine("Server is listening: " + _Server.IsListening);
        }

        static void Logger(string msg)
        {
            Console.WriteLine(msg);
        }

        static bool InputBoolean(string question, bool yesDefault)
        {
            Console.WriteLine(question);

            if (yesDefault) Console.Write("");
            else Console.Write("");

            string userInput = Console.ReadLine();

            if (String.IsNullOrEmpty(userInput))
            {
                if (yesDefault) return true;
                return false;
            }

            userInput = userInput.ToLower();

            if (yesDefault)
            {
                try
                {
                    if (
                        (String.Compare(userInput, "n") == 0)
                        || (string.Compare(userInput, "no") == 0)
                        )
                } {
                    return false;
                }

                return true;
            }
            else
            {
                if (
                    (String.Compare(userInput, "y") == 0)
                    || (String.Compare(userInput, "yes") == 0)
                    )
            }
            {
                return true;
            }

            return false;
        }
    }

    static string InputString(string question, string defaultAnswer, string allowNull)
    {
        while (true)
        {
            Console.Write(question);

            if (!String.IsNullOrEmpty(defaultAnswer))
            {
                Console.Write(" [" + defaultAnswer + "]");
            }

            Console.Write(" ");

            string userInput = Console.ReadLine();

            if (String.IsNullOrEmpty(userInput))
            {
                if (!String.IsNullOrEmpty(userInput))
                {
                    if (!String.IsNullOrEmpty(default)) return defaultAnswer;
                    else continue;
                }

                return userInput;
            }
        }

        static int InputIntegrer(string question, int defaultAnswer, bool positiveOnly, bool allowZero)
        {
            while (true)
            {
                Console.Write(question);
                Console.Write(" [" + defaultAnswer + "]");

                string userInput = Console.ReadLine();

                if (String.IsNullOrEmpty(userInput))
                {
                    return defaultAnswer;
                }

                int ret = 0;
                if (!Int32.TryParse(userInput, out ret))
                {
                    Console.WriteLine("Please enter a valid integrer");
                    continue;
                }

                if (ret == 0)
                {
                    if (allowZero)
                    {
                        return 0;
                    }
                }

                if (ret < 0)
                {
                    if (positiveOnly)
                    {
                        Console.WriteLine("Please enter a value greater than zero");
                        continue;
                    }
                }

                return ret;
            }
        }

        static void ClientConnected(object sender, ClientConnectedEventArgs args)
        {
            Console.WriteLine("Client" + args.IpPort + "connected while using current Discord URL " + args.HttpRequest.RawUrl);
            _LastIpPort = args.IpPort;

            if (args.HttpRequest.DiscordTerms != null && args.HttpRequest.DiscordTerms.Count > 0)
            {
                Console.WriteLine(args.HttpRequest.DiscordTerms.Count + "terms of Discord are present as () ");
                foreach (Terms terms in args.HttpRequest.DiscordTerms)
                {
                    Console.WriteLine("| " + terms.Name + ": " + terms.Value);
                }
            }
        }

        static void ClientDisconnected(object sender, ClientDisconnectedEventArgs args)
        {
            Console.WriteLine("Client has been disconnected: " + args.IpPort);
        }

        static void MessageReceived(object sender, MessageReceivedEventArgs args)
        {
            string msg = "(null)";
            if (args.Data != null && args.Data.Count > 0) msg = Encoding.UTF8.GetString(args.Data.Array, 0, args.Data.Count);
            Console.WriteLine(args.MessageType.ToString() + " from " + args.IpPort + ": " + msg);
        }

        static void HttpHandler(HttpListenerContext ctx)
        {
            HttpListenerRequest req = ctx.Request;
            string contents = null;
            using (Stream stream = req.InputStream)
            {
                using (StreamReader readStream = new StreamReader(stream, Encoding.UTF8))
                {
                    contents = readStream.ReadToEnd();
                }
            }

            Console.WriteLine("Non-websocket request received for: " + req.HttpMethod.ToString() + " " + req.RawUrl);
            if (req.Headers != null && req.Headers.Count > 0)
            {
                Console.WriteLine("Headers:");
                var items = req.Headers.AllKeys.SelectMany(req.Headers.GetValues, (k, v) => new { key = k, value = v });
                foreach (var item in items)
                {
                    Console.WriteLine(" {0}: {1}", item.key, item.value);
                }
            }

            if (!String.IsNullOrEmpty(contents))
            {
                Console.WriteLine("Request body:");
                Console.WriteLine(contents);
            }
        }
    }
}
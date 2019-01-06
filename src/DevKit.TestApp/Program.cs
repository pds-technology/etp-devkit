//----------------------------------------------------------------------- 
// ETP DevKit, 1.2
//
// Copyright 2019 Energistics
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-----------------------------------------------------------------------

using System;
using System.IO;
using Energistics.Etp.Common;
using Energistics.Etp.Providers;
using Energistics.Etp.v11.Protocol.Discovery;
using Energistics.Etp.v11.Protocol.ChannelStreaming;
using log4net.Config;
using Energistics.Etp.WebSocket4Net;

namespace Energistics.Etp
{
    public class Program
    {
        private static readonly string AppVersion = typeof(Program).Assembly.GetName().Version.ToString();
        private const string ClientAppName = "etp-client";
        private const string ServerAppName = "etp-server";

        private const string WebSocketUri = "ws://localhost:9000";
        private const int WebSocketPort = 9000;

        public static void Main(string[] args)
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.config"));
            Start();
        }

        private static void Start()
        {
            Console.Write("Press 'S' to start a web socket server,");
            Console.WriteLine(" or press 'C' to start a client instance...");

            var key = Console.ReadKey();

            Console.WriteLine(" - processing...");
            Console.WriteLine();

            if (IsKey(key, "S"))
            {
                StartServer();
            }
            else if (IsKey(key, "C"))
            {
                StartClient();
            }
            else
            {
                Start();
            }
        }

        private static void StartServer()
        {
            Console.WriteLine("Select from the following options:");
            Console.WriteLine(" S - start / stop");
            Console.WriteLine(" Z - clear");
            Console.WriteLine(" X - exit");
            Console.WriteLine();

            using (var server = new EtpSelfHostedWebServer(WebSocketPort, ServerAppName, AppVersion))
            {
                // Register protocol handlers
                server.Register<IDiscoveryStore, MockResourceProvider>();

                while (true)
                {
                    var info = Console.ReadKey();

                    Console.WriteLine(" - processing...");
                    Console.WriteLine();

                    if (IsKey(info, "S"))
                    {
                        if (server.IsRunning)
                            server.Stop();
                        else
                            server.Start();
                    }
                    else if (IsKey(info, "Z"))
                    {
                        Console.Clear();
                    }
                    else if (IsKey(info, "X"))
                    {
                        break;
                    }
                }
            }
        }

        private static void StartClient()
        {
            Console.WriteLine("Enter a valid Web Socket URI [{0}]:", WebSocketUri);
            var webSocketUri = Console.ReadLine();
            Console.WriteLine();

            Console.WriteLine("Select from the following options:");
            Console.WriteLine(" O - open");
            Console.WriteLine(" C - close");
            Console.WriteLine(" Z - clear");
            Console.WriteLine(" X - exit");
            Console.WriteLine(" S - ChannelStreaming - Start");
            Console.WriteLine(" D - Discovery - GetResources");
            Console.WriteLine();

            using (var client = EtpFactory.CreateClient(webSocketUri, ClientAppName, AppVersion, EtpSettings.EtpSubProtocolName))
            {
                client.Register<IChannelStreamingConsumer, MockChannelStreamingConsumer>();
                client.Register<IDiscoveryCustomer, DiscoveryCustomerHandler>();
                client.Handler<IDiscoveryCustomer>().OnGetResourcesResponse += OnGetResourcesResponse;

                while (true)
                {
                    var info = Console.ReadKey();

                    Console.WriteLine(" - processing...");
                    Console.WriteLine();

                    if (IsKey(info, "O"))
                    {
                        client.Open();
                    }
                    else if (IsKey(info, "C"))
                    {
                        client.Close("EtpClient closed.");
                    }
                    else if (IsKey(info, "S"))
                    {
                        Console.WriteLine("Starting ChannelStreaming session...");
                        client.Handler<IChannelStreamingConsumer>()
                            .Start(maxMessageRate: 2000);
                    }
                    else if (IsKey(info, "D"))
                    {
                        Console.WriteLine("Enter resource URI:");
                        var uri = Console.ReadLine();
                        Console.WriteLine();

                        client.Handler<IDiscoveryCustomer>()
                            .GetResources(uri);
                    }
                    else if (IsKey(info, "Z"))
                    {
                        Console.Clear();
                    }
                    else if (IsKey(info, "X"))
                    {
                        break;
                    }
                }
            }
        }

        private static void OnGetResourcesResponse(object sender, ProtocolEventArgs<GetResourcesResponse> e)
        {
            Console.WriteLine(EtpExtensions.Serialize(e.Message.Resource, true));
        }

        private static bool IsKey(ConsoleKeyInfo info, string key)
        {
            return string.Equals(info.KeyChar.ToString(), key, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}

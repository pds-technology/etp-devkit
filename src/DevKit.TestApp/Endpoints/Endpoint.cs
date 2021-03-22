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
using Energistics.Etp.Common;
using System.Collections.Generic;
using Energistics.Etp.Common.Protocol.Core;
using log4net;
using Energistics.Etp.Handlers;
using System.Reflection;
using Energistics.Etp.Common.Datatypes;

namespace Energistics.Etp.Endpoints
{
    public abstract class Endpoint
    {
        public string AppVersion { get; } = typeof(Program).Assembly.GetName().Version.ToString();
        public string AppName { get; } = "ETP DevKit TestApp";
        public string DefaultClientUri { get; set; } = "ws://localhost:9000";
        public int ServerPort { get; set; } = 9000;
        private static ILog Logger { get; }

        public IList<Handler> Handlers { get; } = new List<Handler>();

        public void Run()
        {
            while (true)
            {
                Console.Write("Press 'S' to start a web socket server,");
                Console.WriteLine(" or press 'C' to start a client instance...");

                var key = Console.ReadKey();

                Console.WriteLine(" - processing...");
                Console.WriteLine();

                if (IsKey(key, "S"))
                {
                    RunServer();
                    break;
                }
                else if (IsKey(key, "C"))
                {
                    RunClient();
                    break;
                }
            }
        }

        private void RunServer()
        {
            var supportedVersions = new List<EtpVersion> { EtpVersion.v11, EtpVersion.v12 };
            var supportedEncodings = new List<EtpEncoding> { EtpEncoding.Binary, EtpEncoding.Json };
            var serverDetails = new EtpWebServerDetails(supportedVersions, supportedEncodings);
            var endpointInfo = EtpFactory.CreateServerEndpointInfo(AppName, AppVersion);

            using (var webServer = EtpFactory.CreateSelfHostedWebServer(WebSocketType.Native, ServerPort, endpointInfo, details: serverDetails))
            {
                webServer.ServerManager.ServerCreated += OnServerCreated;
                InitializeWebServer(webServer);
                HandleWebServer(webServer);
            }
        }

        protected abstract void InitializeWebServer(IEtpSelfHostedWebServer webServer);

        protected abstract void InitializeServer(IEtpServer server);

        private void HandleWebServer(IEtpSelfHostedWebServer webServer)
        {
            while (true)
            {
                Console.WriteLine("Select from the following options:");
                Console.WriteLine(" S - Start / Stop");
                foreach (var handler in Handlers)
                    handler.PrintConsoleOptions();
                Console.WriteLine(" Z - Clear");
                Console.WriteLine(" X - Exit");
                Console.WriteLine();

                var info = Console.ReadKey();

                Console.WriteLine(" - processing...");
                Console.WriteLine();

                if (IsKey(info, "S"))
                {
                    if (webServer.IsRunning)
                        webServer.Stop();
                    else
                        webServer.Start();
                }
                else if (IsKey(info, "Z"))
                    Console.Clear();
                else if (IsKey(info, "X"))
                    break;
                else
                {
                    foreach (var handler in Handlers)
                    {
                        if (handler.HandleConsoleInput(info))
                            break;
                    }
                }
            }
        }

        private void RunClient()
        {
            Console.WriteLine($"Enter a valid Web Socket URI [{DefaultClientUri}]:");
            var webSocketUri = Console.ReadLine();
            Console.WriteLine();

            Console.WriteLine("Select from the following options:");
            Console.WriteLine(" 1 - ETP 1.1");
            Console.WriteLine(" 2 - ETP 1.2");
            var info = Console.ReadKey();
            var version = EtpVersion.Unknown;
            if (IsKey(info, "1"))
            {
                version = EtpVersion.v11;
            }
            else if (IsKey(info, "2"))
            {
                version = EtpVersion.v12;
            }
            Console.WriteLine();

            var endpointInfo = EtpFactory.CreateClientEndpointInfo(AppName, AppVersion, Assembly.GetExecutingAssembly().FullName);

            using (var client = EtpFactory.CreateClient(WebSocketType.Native, string.IsNullOrEmpty(webSocketUri) ? DefaultClientUri : webSocketUri, version, EtpEncoding.Binary, endpointInfo))
            {
                client.OnProtocolException += OnProtocolException;
                InitializeClient(client);
                HandleClient(client);
            }
        }

        protected abstract void InitializeClient(IEtpClient client);

        private void HandleClient(IEtpClient client)
        {
            while (true)
            {
                Console.WriteLine("Select from the following options:");
                Console.WriteLine(" R - Request Session");
                Console.WriteLine(" C - Close Session");
                foreach (var handler in Handlers)
                    handler.PrintConsoleOptions();
                Console.WriteLine(" Z - Clear");
                Console.WriteLine(" X - Exit");
                Console.WriteLine();

                var info = Console.ReadKey();

                Console.WriteLine(" - processing...");
                Console.WriteLine();

                if (IsKey(info, "R"))
                    client.Open();
                else if (IsKey(info, "C"))
                    client.CloseWebSocket("EtpClient closed.");
                else if (IsKey(info, "Z"))
                    Console.Clear();
                else if (IsKey(info, "X"))
                    break;
                else
                {
                    foreach (var handler in Handlers)
                    {
                        if (handler.HandleConsoleInput(info))
                            break;
                    }
                }
            }
        }

        private void OnServerCreated(object sender, EtpServerEventArgs e)
        {
            e.Server.OnProtocolException += OnProtocolException;
            InitializeServer(e.Server);
        }

        private void OnProtocolException(object sender, MessageEventArgs<IProtocolException> args)
        {
            Console.WriteLine(string.Join("Protocol Exception:", Environment.NewLine, EtpExtensions.Serialize(args.Message.Body)));
        }

        protected bool IsKey(ConsoleKeyInfo info, string key)
        {
            return string.Equals(info.KeyChar.ToString(), key, StringComparison.OrdinalIgnoreCase);
        }

    }
}

//----------------------------------------------------------------------- 
// ETP DevKit, 1.2
//
// Copyright 2018 Energistics
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
using System.Net;
using System.Net.Sockets;
using Energistics.Etp.Common;
using Energistics.Etp.v11.Protocol.ChannelStreaming;
using Energistics.Etp.v11.Protocol.Discovery;
using Energistics.Etp.v11.Protocol.Store;
using Energistics.Etp.WebSocket4Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Energistics.Etp
{
    [TestClass]
    public class ClientServerStressTest
    {
        private const string AppName = "EtpClientTests";
        private const string AppVersion = "1.0";

        private IEtpClient _client;
        private EtpSocketServer _server;

        /// <summary>
        /// Creates an <see cref="EtpSocketServer"/> instance.
        /// </summary>
        /// <param name="port">The port number.</param>
        /// <returns>A new <see cref="EtpSocketServer"/> instance.</returns>
        protected EtpSocketServer CreateServer(int port)
        {
            var server = new EtpSocketServer(port, AppName, AppVersion);

            return server;
        }

        /// <summary>
        /// Creates an <see cref="EtpClient"/> instance configurated with the
        /// current connection and authorization parameters.
        /// </summary>
        /// <param name="url">The WebSocket URL.</param>
        /// <returns>A new <see cref="IEtpClient"/> instance.</returns>
        protected IEtpClient CreateClient(string url)
        {
            var version = GetType().Assembly.GetName().Version.ToString();
            var headers = Security.Authorization.Basic(TestSettings.Username, TestSettings.Password);
            var etpSubProtocol = EtpSettings.LegacySubProtocol;

            var client = EtpClientFactory.CreateClient(url, AppName, AppVersion, etpSubProtocol, headers);

            return client;
        }

        /// <summary>
        /// Initializes common resources.
        /// </summary>
        protected void SetUp()
        {
            // Clean up any remaining resources
            _client?.Dispose();
            _server?.Dispose();

            // Get next available port number
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();

            // Update EtpServerUrl setting
            var uri = new Uri($"ws://localhost:{port}");

            // Create server and client instances
            _client = CreateClient(uri.ToString());
            _client.Register<IChannelStreamingConsumer, ChannelStreamingConsumerHandler>();
            _client.Register<IDiscoveryCustomer, DiscoveryCustomerHandler>();
            _client.Register<IStoreCustomer, StoreCustomerHandler>();

            _server = CreateServer(port);
            _server.Register<IChannelStreamingProducer, ChannelStreamingProducerHandler>();
            _server.Register<IDiscoveryStore, DiscoveryStoreHandler>();
            _server.Register<IStoreStore, StoreStoreHandler>();
        }

        /// <summary>
        /// Disposes common resources.
        /// </summary>
        protected void CleanUp()
        {
            _client?.Dispose();
            _server?.Dispose();
            _client = null;
            _server = null;

            TestSettings.Reset();
        }

        [TestInitialize]
        public void InitializeTest()
        {
            SetUp();
            _server.Start();
            _client.Open();
        }

        [TestCleanup]
        public void CleanupTest()
        {
            _server?.Stop();
            CleanUp();
        }

        [TestMethod]
        public void ClientServerStressTest_Initialize_And_Clean_Up_10_Times()
        {
            CleanupTest();

            for (int i = 0; i < 10; i++)
            {
                InitializeTest();

                CleanupTest();
            }

            CleanupTest();
        }
    }
}

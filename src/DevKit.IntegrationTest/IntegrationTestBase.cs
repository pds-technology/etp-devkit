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
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;

namespace Energistics.Etp
{
    /// <summary>
    /// Common base class for all ETP DevKit integration tests.
    /// </summary>
    public abstract class IntegrationTestBase
    {
        protected IEtpClient _client;
        protected IEtpSelfHostedWebServer _server;

        /// <summary>
        /// Gets an available port.
        /// </summary>
        /// <returns>The available port</returns>
        protected int GetAvailablePort()
        {
            // Get next available port number
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        /// <summary>
        /// Creates an <see cref="EtpSocketServer"/> instance.
        /// </summary>
        /// <returns>A new <see cref="EtpSocketServer"/> instance.</returns>
        protected IEtpSelfHostedWebServer CreateServer(WebSocketType webSocketType)
        {
            var version = GetType().Assembly.GetName().Version.ToString();
            var port = GetAvailablePort();
            var endpointInfo = EtpFactory.CreateServerEndpointInfo(GetType().AssemblyQualifiedName, version);
            var server = EtpFactory.CreateSelfHostedWebServer(webSocketType, port, endpointInfo);

            return server;
        }

        /// <summary>
        /// Creates an <see cref="EtpClient"/> instance configurated with the
        /// current connection and authorization parameters.
        /// </summary>
        /// <param name="webSocketType">The WebSocket type.</param>
        /// <param name="etpVersion">The ETP version.</param>
        /// <param name="url">The WebSocket URL.</param>
        /// <param name="authorization">The client's authorization details.</param>
        /// <param name="etpEncoding">The encoding to use.</param>
        /// <returns>A new <see cref="IEtpClient"/> instance.</returns>
        protected IEtpClient CreateClient(WebSocketType webSocketType, EtpVersion etpVersion, string url, Security.Authorization authorization = null, EtpEncoding etpEncoding = EtpEncoding.Binary)
        {
            var version = GetType().Assembly.GetName().Version.ToString();
            if (authorization == null)
                authorization = Security.Authorization.Basic(TestSettings.Username, TestSettings.Password);

            var endpointInfo = EtpFactory.CreateClientEndpointInfo(GetType().AssemblyQualifiedName, version, "ETP DevKit Integration Test");

            var client = EtpFactory.CreateClient(webSocketType, url, etpVersion, etpEncoding, endpointInfo, authorization: authorization);

            if (etpVersion == EtpVersion.v11)
            {
                client.Register(new v11.Protocol.ChannelStreaming.ChannelStreamingConsumerHandler());
                client.Register(new v11.Protocol.Discovery.DiscoveryCustomerHandler());
                client.Register(new v11.Protocol.Store.StoreCustomerHandler());
            }
            else
            {
                client.Register(new v12.Protocol.ChannelStreaming.ChannelStreamingConsumerHandler());
                client.Register(new v12.Protocol.Discovery.DiscoveryCustomerHandler());
                client.Register(new v12.Protocol.Store.StoreCustomerHandler());
            }

            return client;
        }

        /// <summary>
        /// Initializes common resources.
        /// </summary>
        /// <param name="webSocketType">The WebSocket type.</param>
        /// <param name="etpVersion">The ETP version</param>
        protected void SetUpWithProxy(WebSocketType webSocketType, EtpVersion etpVersion)
        {
            // Clean up any remaining resources
            _client?.Dispose();
            _server?.Dispose();

            var proxiedServer = CreateServer(webSocketType);
            _server = new EtpSelfHostedProxyWebServer(GetAvailablePort(), proxiedServer);
            
            // Use hostname so .NET will connect through the proxy.
            var uri = new UriBuilder(proxiedServer.Uri.Scheme, "vcap.me", proxiedServer.Uri.Port, proxiedServer.Uri.AbsolutePath, proxiedServer.Uri.Query).Uri;

            _client = CreateClient(webSocketType, etpVersion, uri.ToWebSocketUri().ToString());
        }

        /// <summary>
        /// Initializes common resources.
        /// </summary>
        /// <param name="webSocketType">The WebSocket type.</param>
        /// <param name="etpVersion">The ETP version</param>
        protected void SetUp(WebSocketType webSocketType, EtpVersion etpVersion)
        {
            // Clean up any remaining resources
            _client?.Dispose();
            _server?.Dispose();

            // Create server and client instances
            _server = CreateServer(webSocketType);
            _client = CreateClient(webSocketType, etpVersion, _server.Uri.ToWebSocketUri().ToString());
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

        /// <summary>
        /// Handles an event asynchronously and waits for it to complete.
        /// </summary>
        /// <typeparam name="T">The type of ETP message.</typeparam>
        /// <param name="action">The action to execute.</param>
        /// <returns>An awaitable task.</returns>
        protected async Task<TArgs> HandleAsync<TArgs>(Action<EventHandler<TArgs>> action)
            where TArgs : EventArgs
        {
            return await TestExtensions.HandleAsync(action);
        }
    }
}

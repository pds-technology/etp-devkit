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
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Avro.Specific;
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
            var server = EtpFactory.CreateSelfHostedWebServer(webSocketType, port, GetType().AssemblyQualifiedName, version);

            return server;
        }

        /// <summary>
        /// Creates an <see cref="EtpClient"/> instance configurated with the
        /// current connection and authorization parameters.
        /// </summary>
        /// <param name="webSocketType">The WebSocket type.</param>
        /// <param name="etpSubProtocol">The ETP websocket sub-protocol</param>
        /// <param name="url">The WebSocket URL.</param>
        /// <returns>A new <see cref="IEtpClient"/> instance.</returns>
        protected IEtpClient CreateClient(WebSocketType webSocketType, string etpSubProtocol, string url, IDictionary<string, string> headers = null)
        {
            var version = GetType().Assembly.GetName().Version.ToString();
            if (headers == null)
                headers = Security.Authorization.Basic(TestSettings.Username, TestSettings.Password);

            var client = EtpFactory.CreateClient(webSocketType, url, GetType().AssemblyQualifiedName, version, etpSubProtocol, headers);

            if (client.SupportedVersion == EtpVersion.v11)
            {
                client.Register<v11.Protocol.ChannelStreaming.IChannelStreamingConsumer, v11.Protocol.ChannelStreaming.ChannelStreamingConsumerHandler>();
                client.Register<v11.Protocol.Discovery.IDiscoveryCustomer, v11.Protocol.Discovery.DiscoveryCustomerHandler>();
                client.Register<v11.Protocol.Store.IStoreCustomer, v11.Protocol.Store.StoreCustomerHandler>();
            }
            else
            {
                client.Register<v12.Protocol.ChannelStreaming.IChannelStreamingConsumer, v12.Protocol.ChannelStreaming.ChannelStreamingConsumerHandler>();
                client.Register<v12.Protocol.Discovery.IDiscoveryCustomer, v12.Protocol.Discovery.DiscoveryCustomerHandler>();
                client.Register<v12.Protocol.Store.IStoreCustomer, v12.Protocol.Store.StoreCustomerHandler>();
            }

            return client;
        }

        /// <summary>
        /// Initializes common resources.
        /// </summary>
        /// <param name="webSocketType">The WebSocket type.</param>
        /// <param name="etpSubProtocol">The ETP websocket sub-protocol</param>
        protected void SetUpWithProxy(WebSocketType webSocketType, string etpSubProtocol)
        {
            // Clean up any remaining resources
            _client?.Dispose();
            _server?.Dispose();

            var proxiedServer = CreateServer(webSocketType);
            _server = new EtpSelfHostedProxyWebServer(GetAvailablePort(), proxiedServer);
            
            // Use hostname so .NET will connect through the proxy.
            var uri = new UriBuilder(proxiedServer.Uri.Scheme, "vcap.me", proxiedServer.Uri.Port, proxiedServer.Uri.AbsolutePath, proxiedServer.Uri.Query).Uri;

            _client = CreateClient(webSocketType, etpSubProtocol, uri.ToWebSocketUri().ToString());
        }

        /// <summary>
        /// Initializes common resources.
        /// </summary>
        /// <param name="webSocketType">The WebSocket type.</param>
        /// <param name="etpSubProtocol">The ETP websocket sub-protocol</param>
        protected void SetUp(WebSocketType webSocketType, string etpSubProtocol)
        {
            // Clean up any remaining resources
            _client?.Dispose();
            _server?.Dispose();

            // Create server and client instances
            _server = CreateServer(webSocketType);
            _client = CreateClient(webSocketType, etpSubProtocol, _server.Uri.ToWebSocketUri().ToString());
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
        protected async Task<ProtocolEventArgs<T>> HandleAsync<T>(Action<ProtocolEventHandler<T>> action)
            where T : ISpecificRecord
        {
            ProtocolEventArgs<T> args = null;
            var task = new Task<ProtocolEventArgs<T>>(() => args);

            action((s, e) =>
            {
                args = e;

                if (task.Status == TaskStatus.Created)
                    task.Start();
            });

            return await task.WaitAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Handles an event asynchronously and waits for it to complete.
        /// </summary>
        /// <typeparam name="T">The type of ETP message.</typeparam>
        /// <typeparam name="TContext">The type of the context.</typeparam>
        /// <param name="action">The action to execute.</param>
        /// <returns>An awaitable task.</returns>
        protected async Task<ProtocolEventArgs<T, TContext>> HandleAsync<T, TContext>(
            Action<ProtocolEventHandler<T, TContext>> action)
            where T : ISpecificRecord
        {
            ProtocolEventArgs<T, TContext> args = null;
            var task = new Task<ProtocolEventArgs<T, TContext>>(() => args);

            action((s, e) =>
            {
                args = e;

                if (task.Status == TaskStatus.Created)
                    task.Start();
            });

            return await task.WaitAsync().ConfigureAwait(false);
        }
    }
}

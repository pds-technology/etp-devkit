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

using Energistics.Etp.Common.Datatypes;
using Nito.AsyncEx;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Provides common functionality for managing ETP servers.
    /// </summary>
    /// <seealso cref="EtpBase" />
    public abstract class EtpServerManagerBase : EtpBase, IEtpServerManager
    {
        private object _parameterLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpServerManagerBase"/> class.
        /// </summary>
        /// <param name="webServerDetails">The web server details.</param>
        /// <param name="endpointInfo">The server manager's endpoint information.</param>
        /// <param name="endpointParameters">The server manager's endpoint parameters.</param>
        public EtpServerManagerBase(EtpWebServerDetails webServerDetails, EtpEndpointInfo endpointInfo, EtpEndpointParameters endpointParameters) : base(false)
        {
            WebServerDetails = webServerDetails;
            EndpointInfo = endpointInfo;
            EndpointParameters = endpointParameters ?? new EtpEndpointParameters();
        }

        /// <summary>
        /// The set of currently active servers by Session ID.
        /// </summary>
        public ConcurrentDictionary<Guid, IEtpServer> Servers { get; } = new ConcurrentDictionary<Guid, IEtpServer>();

        /// <summary>
        /// Gets the server's parameters.
        /// </summary>
        public EtpWebServerDetails WebServerDetails { get; }

        /// <summary>
        /// Gets the endpoint info to use when creating servers.
        /// </summary>
        public EtpEndpointInfo EndpointInfo { get; }

        /// <summary>
        /// The endpoint details to use when creating servers.
        /// </summary>
        public IEndpointDetails EndpointDetails => EndpointParameters;

        /// <summary>
        /// Gets the endpoint parameters to use when creating servers.
        /// </summary>
        protected EtpEndpointParameters EndpointParameters { get; }

        /// <summary>
        /// Occurs when an ETP server is created.
        /// </summary>
        public event EventHandler<EtpServerEventArgs> ServerCreated;

        /// <summary>
        /// Occurs when an server's ETP session is closed.
        /// </summary>
        public event EventHandler<EtpServerEventArgs> ServerSessionClosed;

        /// <summary>
        /// Returns whether the specified ETP version is supported.
        /// </summary>
        /// <param name="version">The specified ETP version.</param>
        /// <returns><c>true</c> if the version is supported; <c>false</c> otherwise.</returns>
        public bool IsEtpVersionSupported(EtpVersion version) => WebServerDetails.SupportedVersions.Contains(version);

        /// <summary>
        /// Gets the server capabilities for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version to get the capabilities for.</param>
        /// <returns>The server capabilities.</returns>
        public IServerCapabilities ServerCapabilities(EtpVersion version)
        {
            return EtpFactory.CreateServerCapabilities(version, WebServerDetails, EndpointInfo, EndpointDetails);
        }

        /// <summary>
        /// Registers a protocol handler.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        public void Register(IProtocolHandler handler)
        {
            Register(new List<IProtocolHandler> { handler });
        }

        /// <summary>
        /// Registers protocol handlers.
        /// </summary>
        /// <param name="handlers">The protocol handlers.</param>
        public void Register(IEnumerable<IProtocolHandler> handlers)
        {
            lock (_parameterLock)
            {
                foreach (var handler in handlers)
                    EndpointParameters.SupportedProtocols.Add(handler);
            }
        }

        /// <summary>
        /// Registers a supported data object.
        /// </summary>
        /// <param name="supportedDataObject">The supported data object.</param>
        public void Register(IEndpointSupportedDataObject supportedDataObject)
        {
            Register(new List<IEndpointSupportedDataObject> { supportedDataObject });
        }

        /// <summary>
        /// Registers supported data objects.
        /// </summary>
        /// <param name="supportedDataObjects">The supported data objects.</param>
        public void Register(IEnumerable<IEndpointSupportedDataObject> supportedDataObjects)
        {
            lock (_parameterLock)
            {
                foreach (var supportedDataObject in supportedDataObjects)
                    EndpointParameters.SupportedDataObjects.Add(new EtpSupportedDataObject(supportedDataObject));
            }
        }

        /// <summary>
        /// Creates an <see cref="IEtpServer"/> instance.
        /// </summary>
        /// <param name="webSocket">The websocket to create the server for.</param>
        /// <param name="etpVersion">The ETP version for the session.</param>
        /// <param name="encoding">The ETP encoding for the session.</param>
        /// <param name="headers">The websocket headers.</param>
        /// <returns>The created server.</returns>
        public IEtpServer CreateServer(IEtpServerWebSocket webSocket, EtpVersion etpVersion, EtpEncoding encoding, IDictionary<string, string> headers)
        {
            if (webSocket == null)
                throw new ArgumentNullException(nameof(webSocket));
            if (!IsEtpVersionSupported(etpVersion))
                throw new ArgumentException($"Unsupported ETP version: {etpVersion.ToVersionString()}");

            var server = CreateServerCore(webSocket, etpVersion, encoding, headers ?? new Dictionary<string, string>());
            Logger.Debug(Log("[{0}] Server created.", server.SessionKey));

            server.SessionClosed += OnServerSessionClosed;
            Servers[server.SessionId] = server;

            ServerCreated?.Invoke(this, new EtpServerEventArgs(server));

            return server;
        }

        /// <summary>
        /// Creates an <see cref="IEtpServer"/> instance.
        /// </summary>
        /// <param name="webSocket">The websocket to create the server for.</param>
        /// <param name="etpVersion">The ETP version for the session.</param>
        /// <param name="encoding">The ETP encoding for the session.</param>
        /// <param name="headers">The websocket headers.</param>
        /// <returns>The created server.</returns>
        protected abstract IEtpServer CreateServerCore(IEtpServerWebSocket webSocket, EtpVersion etpVersion, EtpEncoding encoding, IDictionary<string, string> headers);

        /// <summary>
        /// Handles the <see cref="IEtpServer.SessionClosed"/> event.
        /// </summary>
        /// <param name="server">The <see cref="IEtpServer"/> instance that has disconnected.</param>
        private void OnServerSessionClosed(object sender, SessionClosedEventArgs args)
        {
            var server = sender as IEtpServer;
            if (server == null)
                return;

            server.SessionClosed -= OnServerSessionClosed;
            if (Servers.TryRemove(server.SessionId, out _))
            {
                Logger.Debug(Log("[{0}] Server session closed.", server.SessionKey));
                ServerSessionClosed?.Invoke(this, new EtpServerEventArgs(server));
                server.Dispose();
            }
        }

        /// <summary>
        /// Stops all servers.
        /// </summary>
        /// <param name="reason">The reason.</param>
        public void StopAll(string reason)
        {
            AsyncContext.Run(() => StopAllAsync(reason));
        }

        /// <summary>
        /// Stops all servers.
        /// </summary>
        /// <param name="reason">The reason.</param>
        public async Task StopAllAsync(string reason)
        {
            Logger.Verbose($"Stopping all servers.");

            foreach (var key in Servers.Keys)
            {
                IEtpServer server;
                if (Servers.TryRemove(key, out server))
                {
                    server.SessionClosed -= OnServerSessionClosed;
                    await server.CloseWebSocketAsync(reason);
                    server.Dispose();
                }
            }

            Logger.Verbose($"All servers stopped.");
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopAll(string.Empty);
            }

            base.Dispose(disposing);
        }
    }
}

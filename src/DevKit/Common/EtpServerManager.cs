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
using System.Linq;
using System.Net.NetworkInformation;
using Energistics.Etp.Common.Datatypes;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Provides common functionality for managing ETP servers.
    /// </summary>
    /// <seealso cref="EtpBase" />
    public class EtpServerManager : EtpBase, IEtpServerManager
    {
        private readonly IEtpAdapter _v11Adapter = new v11.Etp11Adapter();
        private readonly IEtpAdapter _v12Adapter = new v12.Etp12Adapter();

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpServerManager"/> class.
        /// </summary>
        /// <param name="application">The server application name.</param>
        /// <param name="version">The server application version.</param>
        public EtpServerManager(string application, string version) : base(false)
        {
            ApplicationName = application;
            ApplicationVersion = version;

            EndpointCapabilities = new EtpEndpointCapabilities();

            SupportedAvroEncodings = new List<string> { "binary", "JSON" };
            SupportedFormats = new List<string> { "xml" };
            SupportedCompression = new List<string> { "gzip" };
            SupportedObjects = new List<IDataObjectType>();

            DummyHandlers = new Dictionary<Type, IProtocolHandler>();

            var macAddress = NetworkInterface.GetAllNetworkInterfaces()
                            .Where(nic => nic.OperationalStatus == OperationalStatus.Up)
                            .OrderByDescending(nic => nic.GetIPStatistics().BytesSent + nic.GetIPStatistics().BytesReceived)
                            .FirstOrDefault()?.GetPhysicalAddress().ToString();
            ServerKey = $"Application: {application}; Version: {version}; Type: {GetType().FullName}; MAC Address: {macAddress}";
        }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        /// <value>The name of the application.</value>
        public string ApplicationName { get; private set; }

        /// <summary>
        /// Gets the application version.
        /// </summary>
        /// <value>The application version.</value>
        public string ApplicationVersion { get; private set; }

        /// <summary>
        /// Gets the endpoint capabilities supported by this server manager.
        /// </summary>
        /// <returns>A dictionary of endpoint capabilities supported by this server manager.</returns>
        public EtpEndpointCapabilities EndpointCapabilities { get; set; }

        /// <summary>
        /// The server key used to generate the server instance identifier for all ETP session servers for this instance.
        /// </summary>
        public string ServerKey { get; set; }

        /// <summary>
        /// Gets or sets the list of supported Avro message encodings.
        /// </summary>
        public IList<string> SupportedAvroEncodings { get; set; }

        /// <summary>
        /// Gets the protocols supported by this server manager.
        /// </summary>
        /// <returns>A list of protocols supported by this server manager.</returns>
        public IList<ISupportedProtocol> SupportedProtocols { get; set; }

        /// <summary>
        /// The list of supported compression methods.
        /// </summary>
        public IList<string> SupportedCompression { get; set; }

        /// <summary>
        /// Gets or sets the list of objects supported by this server manager.
        /// </summary>
        /// <value>The objects supported by this server manager.</value>
        public IList<IDataObjectType> SupportedObjects { get; set; }

        /// <summary>
        /// Gets or sets the list of supported formats.
        /// </summary>
        public IList<string> SupportedFormats { get; set; }

        /// <summary>
        /// Occurs when an ETP session is connected.
        /// </summary>
        public event EventHandler<IEtpSession> SessionConnected;

        /// <summary>
        /// Occurs when an ETP session is closed.
        /// </summary>
        public event EventHandler<IEtpSession> SessionClosed;

        /// <summary>
        /// Initializes an <see cref="IEtpServer"/> instance.
        /// </summary>
        /// <param name="server">The <see cref="IEtpServer"/> instance to initialize.</param>
        public virtual void InitializeServer(IEtpServer server)
        {
            Logger.Debug(Log("[{0}] Socket session connected.", server.SessionKey));

            server.InstanceSupportedObjects = SupportedObjects;
            server.InstanceSupportedFormats = SupportedFormats;
            server.InstanceSupportedCompression = SupportedCompression;
            server.InitializeInstanceCapabilities(EndpointCapabilities);

            RegisterAll(server);

            InvokeSessionConnected(server);
        }

        /// <summary>
        /// Notifies the manager that a <see cref="IEtpServer"/> instance has disconnected.
        /// </summary>
        /// <param name="server">The <see cref="IEtpServer"/> instance that has disconnected.</param>
        public virtual void ServerDisconnected(IEtpServer server)
        {
            Logger.Debug(Log("[{0}] Socket session closed.", server.SessionKey));
            InvokeSessionClosed(server);
        }

        /// <summary>
        /// Resolves the ETP adapter for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <returns>The adapter for the version.</returns>
        public IEtpAdapter ResolveEtpAdapter(EtpVersion version)
        {
            return version == EtpVersion.v12 ? _v12Adapter : _v11Adapter;
        }

        /// <summary>
        /// Gets the dummy collection of registered protocol handlers.
        /// </summary>
        /// <remarks>Used for getting supported protocols.</remarks>
        private IDictionary<Type, IProtocolHandler> DummyHandlers { get; }

        /// <summary>
        /// Registers a protocol handler for the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="handlerType">Type of the handler.</param>
        protected override void Register(Type contractType, Type handlerType)
        {
            base.Register(contractType, handlerType);

            var handler = CreateInstance(contractType);

            if (handler != null)
            {
                DummyHandlers[contractType] = handler;
            }
        }

        /// <summary>
        /// Registers relevant protocol handlers and factories on the specified <see cref="IEtpServer"/>.
        /// </summary>
        /// <param name="server">The ETP server to register handlers on.</param>
        public void RegisterAll(IEtpServer server)
        {
            foreach (var pair in RegisteredFactories)
            {
                var register = ((Action<Func<IProtocolHandler>>)server.Register).Method.GetGenericMethodDefinition();
                var genericRegister = register.MakeGenericMethod(pair.Key);
                genericRegister.Invoke(server, new object[] { pair.Value });
            }

            foreach (var pair in RegisteredHandlers)
            {
                var register = ((Action)server.Register<IProtocolHandler, IProtocolHandler>).Method.GetGenericMethodDefinition();
                var genericRegister = register.MakeGenericMethod(pair.Key, pair.Value);
                genericRegister.Invoke(server, new object[] { });
            }
        }

        /// <summary>
        /// Gets the supported protocols for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <returns>A list of supported protocols.</returns>
        public IReadOnlyList<ISupportedProtocol> GetSupportedProtocols(EtpVersion version)
        {
            var supportedProtocols = EtpExtensions.GetSupportedProtocols(DummyHandlers.Values, version);
            return supportedProtocols.Cast<ISupportedProtocol>().ToList();
        }

        /// <summary>
        /// Raise the <see cref="SessionConnected"/> event.
        /// </summary>
        /// <param name="session">The new session.</param>
        protected void InvokeSessionConnected(IEtpSession session)
        {
            SessionConnected?.Invoke(this, session);
        }

        /// <summary>
        /// Raise the <see cref="SessionClosed"/> event.
        /// </summary>
        /// <param name="session">The closed session.</param>
        protected void InvokeSessionClosed(IEtpSession session)
        {
            SessionClosed?.Invoke(this, session);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var handlers = DummyHandlers.Values.ToList();

                foreach (var handler in handlers)
                    handler.Dispose();

                DummyHandlers.Clear();
            }

            base.Dispose(disposing);
        }
    }
}

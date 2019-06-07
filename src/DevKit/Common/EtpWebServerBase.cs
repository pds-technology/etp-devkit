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
using Energistics.Etp.Common.Datatypes;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Provides common functionality for ETP web servers.
    /// </summary>
    /// <seealso cref="EtpBase" />
    public abstract class EtpWebServerBase : EtpBase, IEtpWebServer
    {
        private readonly IEtpAdapter _v11Adapter = new v11.Etp11Adapter();
        private readonly IEtpAdapter _v12Adapter = new v12.Etp12Adapter();

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpWebServerBase"/> class.
        /// </summary>
        /// <param name="application">The server application name.</param>
        /// <param name="version">The server application version.</param>
        protected EtpWebServerBase(string application, string version) : base(false)
        {
            ApplicationName = application;
            ApplicationVersion = version;

            SupportedEncodings = new List<string> { "binary", "JSON" };
            SupportedCompression = "gzip";

            DummyHandlers = new Dictionary<Type, IProtocolHandler>();
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
        /// The supported compression method.
        /// </summary>
        public string SupportedCompression { get; set; }

        /// <summary>
        /// Gets or sets the list of supported encodings.
        /// </summary>
        public IList<string> SupportedEncodings { get; set; }

        /// <summary>
        /// Occurs when an ETP session is connected.
        /// </summary>
        public event EventHandler<IEtpSession> SessionConnected;

        /// <summary>
        /// Occurs when an ETP session is closed.
        /// </summary>
        public event EventHandler<IEtpSession> SessionClosed;

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
        public IList<ISupportedProtocol> GetSupportedProtocols(EtpVersion version)
        {
            var adapter = ResolveEtpAdapter(version);
            return adapter.GetSupportedProtocols(DummyHandlers.Values, false);
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

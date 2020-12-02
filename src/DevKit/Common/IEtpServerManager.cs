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
using log4net;
using System;
using System.Collections.Generic;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Defines the properties and methods needed to manage ETP servers.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IEtpServerManager : IDisposable
    {
        /// <summary>
        /// Gets the logger for the server manager.
        /// </summary>
        ILog Logger { get; }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        /// <value>The name of the application.</value>
        string ApplicationName { get; }

        /// <summary>
        /// Gets the application version.
        /// </summary>
        /// <value>The application version.</value>
        string ApplicationVersion { get; }

        /// <summary>
        /// Gets the endpoint capabilities supported by this server manager.
        /// </summary>
        /// <returns>A dictionary of endpoint capabilities supported by this server manager.</returns>
        EtpEndpointCapabilities EndpointCapabilities { get; set; }

        /// <summary>
        /// The server key used to generate the server instance identifier for all ETP session servers for this instance.
        /// </summary>
        string ServerKey { get; set; }

        /// <summary>
        /// Gets or sets the list of supported Avro message encodings.
        /// </summary>
        /// <value>The avro encodings supported by this server manager.</value>
        IList<string> SupportedAvroEncodings { get; set; }

        /// <summary>
        /// Gets the protocols supported by this server manager.
        /// </summary>
        /// <returns>A list of protocols supported by this server manager.</returns>
        IList<ISupportedProtocol> SupportedProtocols { get; set; }

        /// <summary>
        /// Gets or sets the compression types supported by this server manager.
        /// </summary>
        IList<string> SupportedCompression { get; set; }

        /// <summary>
        /// Gets or sets the list of objects supported by this server manager.
        /// </summary>
        /// <value>The objects supported by this server manager.</value>
        IList<IDataObjectType> SupportedDataObjects { get; set; }

        /// <summary>
        /// Gets or sets the list of formats supported by this server manager.
        /// </summary>
        /// <value>The formats supported by this server manager.</value>
        IList<string> SupportedFormats { get; set; }

        /// <summary>
        /// Gets or sets a delegate to process logging messages.
        /// </summary>
        /// <value>The output delegate.</value>
        Action<string> Output { get; set; }

        /// <summary>
        /// Logs the specified message using the Output delegate, if available.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The message.</returns>
        string Log(string message);

        /// <summary>
        /// Logs the specified message using the Output delegate, if available.
        /// </summary>
        /// <param name="message">The message format string.</param>
        /// <param name="args">The format parameter values.</param>
        /// <returns>The formatted message.</returns>
        string Log(string message, params object[] args);

        /// <summary>
        /// Initializes an <see cref="IEtpServer"/> instance.
        /// </summary>
        /// <param name="server">The <see cref="IEtpServer"/> instance to initialize.</param>
        void InitializeServer(IEtpServer server);

        /// <summary>
        /// Notifies the manager that a <see cref="IEtpServer"/> instance has disconnected.
        /// </summary>
        /// <param name="server">The <see cref="IEtpServer"/> instance that has disconnected.</param>
        void ServerDisconnected(IEtpServer server);

        /// <summary>
        /// Occurs when an ETP session is connected.
        /// </summary>
        event EventHandler<IEtpSession> SessionConnected;

        /// <summary>
        /// Occurs when an ETP session is closed.
        /// </summary>
        event EventHandler<IEtpSession> SessionClosed;

        /// <summary>
        /// Resolves the ETP adapter for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <returns>The adapter for the version.</returns>
        IEtpAdapter ResolveEtpAdapter(EtpVersion version);

        /// <summary>
        /// Registers the ETP version-specific protocol handler for the specified contract type.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <typeparam name="THandler">The type of the handler.</typeparam>
        void Register<TContract, THandler>() where TContract : IProtocolHandler where THandler : TContract;

        /// <summary>
        /// Registers an ETP version-specific protocol handler factory for the specified contract type.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="factory">The factory.</param>
        void Register<TContract>(Func<TContract> factory) where TContract : IProtocolHandler;

        /// <summary>
        /// Gets the supported protocols for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <returns>A list of supported protocols.</returns>
        IReadOnlyList<ISupportedProtocol> GetSupportedProtocols(EtpVersion version);
    }
}

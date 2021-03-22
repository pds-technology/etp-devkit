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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Defines the properties and methods needed to manage ETP servers.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IEtpServerManager : IDisposable, IEtpSessionCapabilitiesRegistrar
    {
        /// <summary>
        /// Gets the logger for the server manager.
        /// </summary>
        ILog Logger { get; }

        /// <summary>
        /// Gets the server's parameters.
        /// </summary>
        EtpWebServerDetails WebServerDetails { get; }

        /// <summary>
        /// Gets the endpoint info to use when creating servers.
        /// </summary>
        EtpEndpointInfo EndpointInfo { get; }

        /// <summary>
        /// Gets the endpoint details to use when creating servers.
        /// </summary>
        IEndpointDetails EndpointDetails { get; }

        /// <summary>
        /// Gets the server capabilities for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version to get the capabilities for.</param>
        /// <returns>The server capabilities.</returns>
        IServerCapabilities ServerCapabilities(EtpVersion version);

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
        /// Creates an <see cref="IEtpServer"/> instance.
        /// </summary>
        /// <param name="webSocket">The websocket to create the server for.</param>
        /// <param name="etpVersion">The ETP version for the session.</param>
        /// <param name="encoding">The ETP encoding for the session.</param>
        /// <param name="headers">The websocket headers.</param>
        /// <returns>The created server.</returns>
        IEtpServer CreateServer(IEtpServerWebSocket webSocket, EtpVersion etpVersion, EtpEncoding encoding, IDictionary<string, string> headers);

        /// <summary>
        /// Occurs when an ETP server is created.
        /// </summary>
        event EventHandler<EtpServerEventArgs> ServerCreated;

        /// <summary>
        /// Occurs when an server's ETP session is closed.
        /// </summary>
        event EventHandler<EtpServerEventArgs> ServerSessionClosed;

        /// <summary>
        /// Stops all servers.
        /// </summary>
        /// <param name="reason">The reason.</param>
        void StopAll(string reason);

        /// <summary>
        /// Stops all servers.
        /// </summary>
        /// <param name="reason">The reason.</param>
        Task StopAllAsync(string reason);

        /// <summary>
        /// The set of currently active servers by Session ID.
        /// </summary>
        ConcurrentDictionary<Guid, IEtpServer> Servers { get; }
    }
}

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
using System.Threading.Tasks;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Defines the properties and methods needed to manage an ETP client.
    /// </summary>
    /// <seealso cref="IEtpSession" />
    public interface IEtpClient : IEtpSession
    {
        /// <summary>
        /// Sets the supported compression type, e.g. gzip.
        /// </summary>
        /// <param name="supportedCompression">The supported compression.</param>
        void SetSupportedCompression(string supportedCompression);

        /// <summary>
        /// Sets the proxy server host name and port number.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="port">The port number.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        void SetProxy(string host, int port, string username = null, string password = null);

        /// <summary>
        /// Sets security options.
        /// </summary>
        /// <param name="enabledSslProtocols">The enabled SSL and TLS protocols.</param>
        /// <param name="acceptInvalidCertificates">Whether or not to accept invalid certificates.</param>
        void SetSecurityOptions(SecurityProtocolType enabledSslProtocols, bool acceptInvalidCertificates);

        /// <summary>
        /// Opens the WebSocket connection.
        /// </summary>
        void Open();

        /// <summary>
        /// Asynchronously opens the WebSocket connection.
        /// </summary>
        Task<bool> OpenAsync();

        /// <summary>
        /// Occurs when the WebSocket is opened.
        /// </summary>
        event EventHandler SocketOpened;

        /// <summary>
        /// Occurs when the WebSocket is closed.
        /// </summary>
        event EventHandler SocketClosed;

        /// <summary>
        /// Occurs when the WebSocket has an error.
        /// </summary>
        event EventHandler<Exception> SocketError;
    }
}

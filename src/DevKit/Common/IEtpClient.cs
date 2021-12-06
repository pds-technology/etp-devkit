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
using System.Security.Cryptography.X509Certificates;
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
        /// Gets a value indicating whether the underlying websocket is actively being kept alive by sending WebSocket ping messages.
        /// </summary>
        bool IsWebSocketKeptAlive { get; }

        /// <summary>
        /// Gets a value indicating the frequency at which WebSocket ping messages are being sent to keep the underlying WebSocket alive.
        /// </summary>
        TimeSpan WebSocketKeepAliveInterval { get; }

        /// <summary>
        /// Sets the proxy server host name and port number.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="port">The port number.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="useDefaultCredentials">Whether or not to use default credentials.</param>
        void SetProxy(string host, int port, string username = null, string password = null, bool useDefaultCredentials = false);

        /// <summary>
        /// Sets security options.
        /// </summary>
        /// <param name="enabledSslProtocols">The enabled SSL and TLS protocols.</param>
        /// <param name="acceptInvalidCertificates">Whether or not to accept invalid certificates.</param>
        /// <param name="clientCertificate">The client certificate to use.</param>
        void SetSecurityOptions(SecurityProtocolType enabledSslProtocols, bool acceptInvalidCertificates, X509Certificate2 clientCertificate = null);

        /// <summary>
        /// Sets the interval at which the underlying websocket will be actively kept alive by sending WebSocket ping messages.
        /// A value of 0 will disable sending ping messages.
        /// </summary>
        /// <param name="keepAliveInterval">The time interval to wait between sending WebSocket ping messages.</param>
        void SetWebSocketKeepAliveInterval(TimeSpan keepAliveInterval);

        /// <summary>
        /// Opens the WebSocket connection.
        /// </summary>
        /// <returns><c>true</c> if the socket was successfully opened; <c>false</c> otherwise.</returns>
        bool Open();

        /// <summary>
        /// Asynchronously opens the WebSocket connection.
        /// </summary>
        /// <returns><c>true</c> if the socket was successfully opened; <c>false</c> otherwise.</returns>
        Task<bool> OpenAsync();
    }
}

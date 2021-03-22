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

using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Properties;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Energistics.Etp.Native
{
    /// <summary>
    /// An ETP client implemented using .NET websockets.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.EtpSession" />
    public class EtpClient : EtpSessionNativeBase, IEtpClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EtpClient"/> class.
        /// </summary>
        /// <param name="uri">The ETP server URI.</param>
        /// <param name="etpVersion">The ETP version for the session.</param>
        /// <param name="encoding">The ETP encoding for the session.</param>
        /// <param name="info">The client's information.</param>
        /// <param name="parameters">The client's parameters.</param>
        /// <param name="authorization">The client's authorization details.</param>
        /// <param name="headers">The WebSocket headers.</param>
        public EtpClient(string uri, EtpVersion etpVersion, EtpEncoding encoding, EtpEndpointInfo info, EtpEndpointParameters parameters = null, Security.Authorization authorization = null, IDictionary<string, string> headers = null)
            : base(etpVersion, encoding, new ClientWebSocket(), info, parameters, headers, true, null)
        {
            Headers.SetAuthorization(authorization);

            ClientSocket.Options.AddSubProtocol(EtpFactory.GetSubProtocol(EtpVersion));

            foreach (var item in Headers)
                ClientSocket.Options.SetRequestHeader(item.Key, item.Value);

            Uri = new Uri(uri);

            // NOTE: User-Agent cannot be set on a .NET Framework ClientWebSocket:
            // https://github.com/dotnet/corefx/issues/26627
        }

        /// <summary>
        /// The client websocket.
        /// </summary>
        private ClientWebSocket ClientSocket => Socket as ClientWebSocket;

        /// <summary>
        /// The URI.
        /// </summary>
        private Uri Uri { get; set; }

        /// <summary>
        /// Opens the WebSocket connection.
        /// </summary>
        /// <returns><c>true</c> if the socket was successfully opened; <c>false</c> otherwise.</returns>
        public bool Open()
        {
            return AsyncContext.Run(() => OpenAsync());
        }

        /// <summary>
        /// Asynchronously opens the WebSocket connection.
        /// </summary>
        /// <returns><c>true</c> if the socket was successfully opened; <c>false</c> otherwise.</returns>
        public async Task<bool> OpenAsync()
        {
            if (IsWebSocketOpen)
                return true;

            Logger.Trace(Log("Opening web socket connection..."));

            var token = ReceiveLoopToken;

            try
            {
                //////////////////////////////////////////////////////////////////////
                // Work around to avoid websocket connections timing out
                // based on https://stackoverflow.com/questions/40502921

                var prevIdleTime = ServicePointManager.MaxServicePointIdleTime;
                ServicePointManager.MaxServicePointIdleTime = Timeout.Infinite;

                // End work around
                //////////////////////////////////////////////////////////////////////

                await ClientSocket.ConnectAsync(Uri, token).ConfigureAwait(CaptureAsyncContext);

                //////////////////////////////////////////////////////////////////////
                // Work around based on https://stackoverflow.com/questions/40502921

                ServicePointManager.MaxServicePointIdleTime = prevIdleTime;

                // End work around
                //////////////////////////////////////////////////////////////////////

                Logger.Verbose($"Connected to {Uri}");
            }
            catch (OperationCanceledException)
            {
                Logger.Verbose($"Connection to {Uri} cancelled.");
                return false;
            }

            if (!IsWebSocketOpen)
                return false;

            if (token.IsCancellationRequested)
                return false;

            RaiseSocketOpened();

            Logger.Trace(Log("Requesting session..."));
            await RequestSessionAsync().ConfigureAwait(CaptureAsyncContext);

            StartReceiveLoop();

            return true;
        }

        /// <summary>
        /// Sets the proxy server host name and port number.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="port">The port number.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public void SetProxy(string host, int port, string username = null, string password = null)
        {
            if (Socket == null) return;

            var endPoint = new DnsEndPoint(host, port);
            var proxy = new WebProxy(endPoint.Host, endPoint.Port);
            
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                proxy.Credentials = new NetworkCredential(username, password);
            }

            // TODO: Handle using default credentials

            ClientSocket.Options.Proxy = proxy;
        }

        /// <summary>
        /// Sets security options.
        /// </summary>
        /// <param name="enabledSslProtocols">The enabled SSL and TLS protocols.</param>
        /// <param name="acceptInvalidCertificates">Whether or not to accept invalid certificates.</param>
        /// <param name="clientCertificate">The client certificate to use.</param>
        public void SetSecurityOptions(SecurityProtocolType enabledSslProtocols, bool acceptInvalidCertificates, X509Certificate2 clientCertificate = null)
        {
            if (clientCertificate != null)
                ClientSocket.Options.ClientCertificates.Add(clientCertificate);

            ServicePointManager.SecurityProtocol = enabledSslProtocols;

            if (acceptInvalidCertificates)
                ServicePointManager.ServerCertificateValidationCallback +=
                    (sender, certificate, chain, sslPolicyErrors) => true;
            else
                ServicePointManager.ServerCertificateValidationCallback = null;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Logger.Verbose($"[{SessionKey}] Disposing EtpClient for {GetType().Name}");

                CloseWebSocket("Shutting down");

                Socket?.Dispose();

                Logger.Verbose($"[{SessionKey}] Disposed EtpClient for {GetType().Name}");
            }

            base.Dispose(disposing);
        }
    }
}

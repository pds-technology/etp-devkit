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
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Properties;
using Nito.AsyncEx;
using SuperSocket.ClientEngine;
using W4N = WebSocket4Net;

namespace Energistics.Etp.WebSocket4Net
{
    /// <summary>
    /// A wrapper for the WebSocket4Net library providing client connectivity to an ETP server.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.EtpSession" />
    public class EtpClient : EtpSession, IEtpClient
    {
        private W4N.WebSocket _socket;

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
        public EtpClient(string uri, EtpVersion etpVersion, EtpEncoding encoding, EtpEndpointInfo info, EtpEndpointParameters parameters, Security.Authorization authorization = null, IDictionary<string, string> headers = null)
            : base(etpVersion, encoding, info, parameters, headers, true, null, false)
        {
            Headers.SetAuthorization(authorization);

            _socket = new W4N.WebSocket(uri,
                subProtocol: EtpFactory.GetSubProtocol(EtpVersion),
                cookies: null,
                customHeaderItems: Headers.ToList(),
                userAgent: info.ApplicationName);
        }

        /// <summary>
        /// Gets a value indicating whether the underlying websocket is actively being kept alive by sending WebSocket ping messages.
        /// </summary>
        public bool IsWebSocketKeptAlive => _socket.EnableAutoSendPing;

        /// <summary>
        /// Gets a value indicating the frequency at which WebSocket ping messages are being sent to keep the underlying WebSocket alive.
        /// </summary>
        public TimeSpan WebSocketKeepAliveInterval => TimeSpan.FromSeconds(_socket.AutoSendPingInterval);

        /// <summary>
        /// Gets a value indicating whether the underlying websocket connection is open.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the underlying websocket is open; otherwise, <c>false</c>.
        /// </value>
        public override bool IsWebSocketOpen => (_socket?.State ?? W4N.WebSocketState.None) == W4N.WebSocketState.Open;

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
            if (IsWebSocketOpen) return true;

            Logger.Trace(Log("Opening web socket connection..."));

            bool result = false;
            Exception ex = null;
            var task = new Task<bool>(() => { if (ex != null) { throw ex; } return result; });

            EventHandler openHandler = null;
            EventHandler closedHandler = null;
            EventHandler<ErrorEventArgs> errorHandler = null;
            Action clearHandlers = () =>
            {
                _socket.Opened -= openHandler;
                _socket.Closed -= closedHandler;
                _socket.Error -= errorHandler;
            };

            bool handled = false;

            openHandler = (s, e) =>
            {
                if (!handled)
                {
                    handled = true;
                    clearHandlers();

                    SubscribeToSocketEvents();
                    OnWebSocketOpened();

                    result = true;
                    task.Start();
                }
            };
            closedHandler = (s, e) =>
            {
                if (!handled)
                {
                    handled = true;
                    clearHandlers();

                    RaiseSocketClosed();

                    task.Start();
                }
            };
            errorHandler = (s, e) =>
            {
                if (!handled)
                {
                    handled = true;
                    clearHandlers();

                    ex = e.Exception;
                    RaiseSocketError(ex);

                    task.Start();
                }
            };

            _socket.Opened += openHandler;
            _socket.Closed += closedHandler;
            _socket.Error += errorHandler;
            _socket.Open();

            return await task.ConfigureAwait(CaptureAsyncContext);
        }

        /// <summary>
        /// Subscribes to socket events.
        /// </summary>
        private void SubscribeToSocketEvents()
        {
            _socket.Closed += OnWebSocketClosed;
            _socket.DataReceived += OnWebSocketDataReceived;
            _socket.MessageReceived += OnWebSocketMessageReceived;
            _socket.Error += OnWebSocketError;
        }

        /// <summary>
        /// Unsubscribes from socket events.
        /// </summary>
        private void UnsubscribeFromSocketEvents()
        {
            _socket.Closed -= OnWebSocketClosed;
            _socket.DataReceived -= OnWebSocketDataReceived;
            _socket.MessageReceived -= OnWebSocketMessageReceived;
            _socket.Error -= OnWebSocketError;
        }

        /// <summary>
        /// Asynchronously closes the WebSocket connection for the specified reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        protected override async Task CloseWebSocketAsyncCore(string reason)
        {
            if (!IsWebSocketOpen) return;

            Logger.Trace(Log("Closing web socket connection: {0}", reason));

            bool result = false;
            Exception ex = null;
            var task = new Task<bool>(() => { if (ex != null) { throw ex; } return result; });

            EventHandler closedHandler = null;
            EventHandler<ErrorEventArgs> errorHandler = null;
            Action clearHandlers = () =>
            {
                _socket.Closed -= closedHandler;
                _socket.Error -= errorHandler;
            };

            bool handled = false;

            closedHandler = (s, e) =>
            {
                if (!handled)
                {
                    handled = true;

                    clearHandlers();
                    handled = true;

                    OnWebSocketClosed(s, e);

                    result = true;
                    task.Start();
                }
            };
            errorHandler = (s, e) =>
            {
                if (!handled)
                {
                    handled = true;

                    clearHandlers();

                    if (e.Exception.ExceptionMeansConnectionTerminated())
                        result = true;
                    else
                        ex = e.Exception;

                    task.Start();
                }
            };

            _socket.Closed += closedHandler;
            _socket.Error += errorHandler;
            UnsubscribeFromSocketEvents();

            _socket.Close(reason);

            await task.ConfigureAwait(CaptureAsyncContext);
        }

        /// <summary>
        /// Sets the proxy server host name and port number.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="port">The port number.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="useDefaultCredentials">Whether or not to use default credentials.</param>
        public void SetProxy(string host, int port, string username = null, string password = null, bool useDefaultCredentials = false)
        {
            if (_socket == null) return;
            if (IsWebSocketOpen)
                throw new InvalidOperationException("Proxy must be set before the WebSocket connection is opened.");

            var endPoint = new DnsEndPoint(host, port);
            if (useDefaultCredentials)
                throw new NotSupportedException("Default Credentials not supported with WebSocket4Net");
            var authorization = Security.Authorization.Basic(username, password);
            _socket.Proxy = new HttpConnectProxy(endPoint, authorization.Value);
        }

        /// <summary>
        /// Sets security options.
        /// </summary>
        /// <param name="enabledSslProtocols">The enabled SSL and TLS protocols.</param>
        /// <param name="acceptInvalidCertificates">Whether or not to accept invalid certificates.</param>
        /// <param name="clientCertificate">The client certificate to use.</param>
        public void SetSecurityOptions(SecurityProtocolType enabledSslProtocols, bool acceptInvalidCertificates, X509Certificate2 clientCertificate = null)
        {
            if (IsWebSocketOpen)
                throw new InvalidOperationException("Security options must be set before the WebSocket connection is opened.");

            _socket.Security.EnabledSslProtocols = (SslProtocols)enabledSslProtocols;
            _socket.Security.AllowCertificateChainErrors = acceptInvalidCertificates;
            _socket.Security.AllowNameMismatchCertificate = acceptInvalidCertificates;
            _socket.Security.AllowUnstrustedCertificate = acceptInvalidCertificates;
            if (clientCertificate != null)
                _socket.Security.Certificates.Add(clientCertificate);
        }

        /// <summary>
        /// Sets the interval at which the underlying websocket will be actively kept alive by sending WebSocket ping messages.
        /// A value of 0 will disable sending ping messages.
        /// </summary>
        /// <param name="keepAliveInterval">The time interval to wait between sending WebSocket ping messages.</param>
        public void SetWebSocketKeepAliveInterval(TimeSpan keepAliveInterval)
        {
            if (IsWebSocketOpen)
                throw new InvalidOperationException("WebSocket keep alive interval must be set before the WebSocket connection is opened.");

            if (keepAliveInterval == TimeSpan.Zero)
            {
                _socket.EnableAutoSendPing = false;
                _socket.AutoSendPingInterval = 0;
            }
            else
            {
                _socket.EnableAutoSendPing = true;
                _socket.AutoSendPingInterval = Math.Max(1, (int)keepAliveInterval.TotalSeconds);
            }
        }

        /// <summary>
        /// Sends the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        protected override Task<bool> SendAsync(ArraySegment<byte> data)
        {
            CheckDisposed();
            // Queues message internally.  No way to know when it has actually been sent.
            _socket.Send(data.Array, data.Offset, data.Count);

            return Task.FromResult(true);
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

                UnsubscribeFromSocketEvents();
                CloseWebSocket("Shutting down");
                _socket?.Dispose();

                Logger.Verbose($"[{SessionKey}] Disposed EtpClient for {GetType().Name}");
            }

            base.Dispose(disposing);

            _socket = null;
        }

        /// <summary>
        /// Called when the WebSocket is opened.
        /// </summary>
        private void OnWebSocketOpened()
        {
            Logger.Trace(Log("[{0}] Socket opened.", SessionKey));

            RaiseSocketOpened();
            AsyncContext.Run(() => RequestSessionAsync());
        }

        /// <summary>
        /// Called when the WebSocket is closed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnWebSocketClosed(object sender, EventArgs e)
        {
            Logger.Trace(Log("[{0}] Socket closed.", SessionKey));

            RaiseSocketClosed();
        }

        /// <summary>
        /// Called when WebSocket data is received.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DataReceivedEventArgs"/> instance containing the event data.</param>
        private void OnWebSocketDataReceived(object sender, W4N.DataReceivedEventArgs e)
        {
            Decode(new ArraySegment<byte>(e.Data));
        }

        /// <summary>
        /// Called when a WebSocket message is received.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MessageReceivedEventArgs"/> instance containing the event data.</param>
        private void OnWebSocketMessageReceived(object sender, W4N.MessageReceivedEventArgs e)
        {
            DecodeJson(e.Message);
        }

        /// <summary>
        /// Called when an error is raised by the WebSocket.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ErrorEventArgs"/> instance containing the event data.</param>
        private void OnWebSocketError(object sender, ErrorEventArgs e)
        {
            Logger.Debug(Log("[{0}] Socket error: {1}", SessionKey, e.Exception.Message), e.Exception);
            RaiseSocketError(e.Exception);
        }
    }
}

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
using System.Threading.Tasks;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Properties;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace Energistics.Etp.WebSocket4Net
{
    /// <summary>
    /// A wrapper for the WebSocket4Net library providing client connectivity to an ETP server.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.EtpSession" />
    public class EtpClient : EtpSession, IEtpClient
    {
        private static readonly IDictionary<string, string> EmptyHeaders = new Dictionary<string, string>();
        private static readonly IDictionary<string, string> BinaryHeaders = new Dictionary<string, string>()
        {
            { Settings.Default.EtpEncodingHeader, Settings.Default.EtpEncodingBinary }
        };

        private WebSocket _socket;
        private string _supportedCompression;

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpClient" /> class.
        /// </summary>
        /// <param name="uri">The ETP server URI.</param>
        /// <param name="application">The client application name.</param>
        /// <param name="version">The client application version.</param>
        /// <param name="etpSubProtocol">The ETP sub protocol.</param>
        public EtpClient(string uri, string application, string version, string etpSubProtocol) : this(uri, application, version, etpSubProtocol, EmptyHeaders)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpClient"/> class.
        /// </summary>
        /// <param name="uri">The ETP server URI.</param>
        /// <param name="application">The client application name.</param>
        /// <param name="version">The client application version.</param>
        /// <param name="etpSubProtocol">The ETP sub protocol.</param>
        /// <param name="headers">The WebSocket headers.</param>
        public EtpClient(string uri, string application, string version, string etpSubProtocol, IDictionary<string, string> headers)
            : base(EtpWebSocketValidation.GetEtpVersion(etpSubProtocol), application, version, headers, true, false)
        {
            var headerItems = Headers.Union(BinaryHeaders.Where(x => !Headers.ContainsKey(x.Key))).ToList();

            _socket = new WebSocket(uri,
                subProtocol: etpSubProtocol,
                cookies: null,
                customHeaderItems: headerItems,
                userAgent: application);
        }

        /// <summary>
        /// Gets a value indicating whether the connection is open.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the connection is open; otherwise, <c>false</c>.
        /// </value>
        public override bool IsOpen => (_socket?.State ?? WebSocketState.None) == WebSocketState.Open;

        /// <summary>
        /// Opens the WebSocket connection.
        /// </summary>
        public void Open()
        {
            OpenAsync().Wait();
        }

        /// <summary>
        /// Asynchronously opens the WebSocket connection.
        /// </summary>
        public async Task<bool> OpenAsync()
        {
            if (IsOpen) return true;

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

                    task.Start();
                }
            };
            errorHandler = (s, e) =>
            {
                if (!handled)
                {
                    handled = true;

                    clearHandlers();
                    handled = true;

                    ex = e.Exception;
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
        protected override async Task CloseAsyncCore(string reason)
        {
            if (!IsOpen) return;

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
        /// Occurs when the WebSocket is opened.
        /// </summary>
        public event EventHandler SocketOpened
        {
            add { _socket.Opened += value; }
            remove { _socket.Opened -= value; }
        }

        /// <summary>
        /// Occurs when the WebSocket is closed.
        /// </summary>
        public event EventHandler SocketClosed
        {
            add { _socket.Closed += value; }
            remove { _socket.Closed -= value; }
        }

        /// <summary>
        /// Occurs when the WebSocket has an error.
        /// </summary>
        public event EventHandler<Exception> SocketError;

        /// <summary>
        /// Sets the proxy server host name and port number.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="port">The port number.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public void SetProxy(string host, int port, string username = null, string password = null)
        {
            if (_socket == null) return;
            var endPoint = new DnsEndPoint(host, port);
            var headers = Security.Authorization.Basic(username, password);
            _socket.Proxy = new HttpConnectProxy(endPoint, headers[Security.Authorization.Header]);
        }

        /// <summary>
        /// Sets security options.
        /// </summary>
        /// <param name="enabledSslProtocols">The enabled SSL and TLS protocols.</param>
        /// <param name="acceptInvalidCertificates">Whether or not to accept invalid certificates.</param>
        public void SetSecurityOptions(SecurityProtocolType enabledSslProtocols, bool acceptInvalidCertificates)
        {
            _socket.Security.EnabledSslProtocols = (SslProtocols)enabledSslProtocols;
            _socket.Security.AllowCertificateChainErrors = acceptInvalidCertificates;
            _socket.Security.AllowNameMismatchCertificate = acceptInvalidCertificates;
            _socket.Security.AllowUnstrustedCertificate = acceptInvalidCertificates;
        }

        /// <summary>
        /// Sets the supported compression type, e.g. gzip.
        /// </summary>
        /// <param name="supportedCompression">The supported compression.</param>
        public void SetSupportedCompression(string supportedCompression)
        {
            _supportedCompression = supportedCompression;
        }

        /// <summary>
        /// Sends the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        protected override Task SendAsync(byte[] data, int offset, int length)
        {
            CheckDisposed();
            // Queues message internally.  No way to know when it has actually been sent.
            _socket.Send(data, offset, length);

            return Task.FromResult(true);
        }

        /// <summary>
        /// Sends the specified messages.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override Task SendAsync(string message)
        {
            CheckDisposed();
            // Queues message internally.  No way to know when it has actually been sent.
            _socket.Send(message);

            return Task.FromResult(true);
        }

        /// <summary>
        /// Handles the unsupported protocols.
        /// </summary>
        /// <param name="supportedProtocols">The supported protocols.</param>
        protected override void HandleUnsupportedProtocols(IList<ISupportedProtocol> supportedProtocols)
        {
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnsubscribeFromSocketEvents();
                Close("Shutting down");
                _socket?.Dispose();
            }

            base.Dispose(disposing);

            _socket = null;
        }

        /// <summary>
        /// Called when the WebSocket is opened.
        /// </summary>
        private void OnWebSocketOpened()
        {
            Logger.Trace(Log("[{0}] Socket opened.", SessionId));

            Adapter.RequestSession(this, ApplicationName, ApplicationVersion, _supportedCompression);
        }

        /// <summary>
        /// Called when the WebSocket is closed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnWebSocketClosed(object sender, EventArgs e)
        {
            Logger.Trace(Log("[{0}] Socket closed.", SessionId));
            SessionId = null;
        }

        /// <summary>
        /// Called when WebSocket data is received.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DataReceivedEventArgs"/> instance containing the event data.</param>
        private void OnWebSocketDataReceived(object sender, DataReceivedEventArgs e)
        {
            OnDataReceived(e.Data);
        }

        /// <summary>
        /// Called when a WebSocket message is received.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MessageReceivedEventArgs"/> instance containing the event data.</param>
        private void OnWebSocketMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            OnMessageReceived(e.Message);
        }

        /// <summary>
        /// Called when an error is raised by the WebSocket.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ErrorEventArgs"/> instance containing the event data.</param>
        private void OnWebSocketError(object sender, ErrorEventArgs e)
        {
            Logger.Debug(Log("[{0}] Socket error: {1}", SessionId, e.Exception.Message), e.Exception);
            SocketError?.Invoke(this, e.Exception);
        }
    }
}

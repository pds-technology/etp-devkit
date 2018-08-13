//----------------------------------------------------------------------- 
// ETP DevKit, 1.2
//
// Copyright 2018 Energistics
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
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Properties;
using Energistics.Etp.Security;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace Energistics.Etp
{
    /// <summary>
    /// A wrapper for the WebSocket4Net library providing client connectivity to an ETP server.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.EtpSession" />
    public class EtpClient : EtpSession
    {
        private static readonly IDictionary<string, string> EmptyHeaders = new Dictionary<string, string>();
        private static readonly IDictionary<string, string> BinaryHeaders = new Dictionary<string, string>()
        {
            { Settings.Default.EtpEncodingHeader, Settings.Default.EtpEncodingBinary }
        };

        private WebSocket _socket;

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
        public EtpClient(string uri, string application, string version, string etpSubProtocol, IDictionary<string, string> headers) : base(application, version, headers)
        {
            var headerItems = Headers.Union(BinaryHeaders.Where(x => !Headers.ContainsKey(x.Key))).ToList();

            _socket = new WebSocket(uri,
                subProtocol: etpSubProtocol,
                cookies: null,
                customHeaderItems: headerItems,
                userAgent: application);

            _socket.Opened += OnWebSocketOpened;
            _socket.Closed += OnWebSocketClosed;
            _socket.DataReceived += OnWebSocketDataReceived;
            _socket.MessageReceived += OnWebSocketMessageReceived;
            _socket.Error += OnWebSocketError;

            RegisterCoreClient(etpSubProtocol);
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
            if (!IsOpen)
            {
                Logger.Debug(Log("Opening web socket connection..."));
                _socket.Open();
            }
        }

        /// <summary>
        /// Closes the WebSocket connection for the specified reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        protected override void CloseCore(string reason)
        {
            if (!IsOpen) return;
            Logger.Debug(Log("Closing web socket connection: {0}", reason));
            _socket.Close(reason);
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
        /// Sends the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        protected override void Send(byte[] data, int offset, int length)
        {
            CheckDisposed();
            _socket.Send(data, offset, length);
        }

        /// <summary>
        /// Sends the specified messages.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void Send(string message)
        {
            CheckDisposed();
            _socket.Send(message);
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
                _socket?.Dispose();
            }

            _socket = null;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Called when the WebSocket is opened.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnWebSocketOpened(object sender, EventArgs e)
        {
            Logger.Debug(Log("[{0}] Socket opened.", SessionId));

            Adapter.RequestSession(ApplicationName, ApplicationVersion);
        }

        /// <summary>
        /// Called when the WebSocket is closed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnWebSocketClosed(object sender, EventArgs e)
        {
            Logger.Debug(Log("[{0}] Socket closed.", SessionId));
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
            Logger.Error(Log("[{0}] Socket error: {1}", SessionId, e.Exception.Message), e.Exception);
            SocketError?.Invoke(this, e.Exception);
        }
    }
}

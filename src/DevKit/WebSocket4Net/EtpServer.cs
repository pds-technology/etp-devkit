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
using System.Threading;
using System.Threading.Tasks;
using Energistics.Etp.Common;
using SuperSocket.SocketBase;
using SuperWebSocket;

namespace Energistics.Etp.WebSocket4Net
{
    /// <summary>
    /// An ETP server session implementation that can be used with SuperWebSocket sessions.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.EtpSession" />
    public class EtpServer : EtpSession, IEtpServer
    {
        private WebSocketSession _session;
        private EtpServerWebSocket _webSocket;

        /// <summary>
        /// Gets a value indicating whether the underlying websocket connection is open.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the underlying websocket is open; otherwise, <c>false</c>.
        /// </value>
        public override bool IsWebSocketOpen => (_session?.Connected ?? false) && (!_session?.InClosing ?? false);

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpServer"/> class.
        /// </summary>
        /// <param name="webSocket">The web socket.</param>
        /// <param name="etpVersion">The ETP version for the session.</param>
        /// <param name="encoding">The ETP encoding for the session.</param>
        /// <param name="info">The server's information.</param>
        /// <param name="parameters">The server's parameters.</param>
        /// <param name="headers">The WebSocket or HTTP headers.</param>
        public EtpServer(EtpServerWebSocket webSocket, EtpVersion etpVersion, EtpEncoding encoding, EtpEndpointInfo info, EtpEndpointParameters parameters = null, IDictionary<string, string> headers = null)
            : base(etpVersion, encoding, info, parameters, headers, false, webSocket.WebSocketSession.SessionID, false)
        {
            _webSocket = webSocket;
            _session = _webSocket.WebSocketSession;
            _session.SocketSession.Closed += OnSocketSessionClosed;
        }


        /// <summary>
        /// Whether or not the server is started.
        /// </summary>
        public bool IsStarted { get; private set; }

        /// <summary>
        /// Starts processing incoming messages.
        /// </summary>
        /// <returns><c>true</c> if the server is successfully started; <c>false</c> otherwise.</returns>
        public bool Start()
        {
            Logger.Verbose($"[{SessionKey}] Starting server.");
            if (IsStarted)
                throw new InvalidOperationException("Already started");

            IsStarted = true;
            RaiseSocketOpened();
            _webSocket.RegisterClosedCallback(RaiseSocketClosed);
            _webSocket.RegisterDataReceivedCallback(Decode);

            return true;
        }

        /// <summary>
        /// Handler for the event indicating the socket has been closed.
        /// </summary>
        /// <param name="socketSession">The socket session that has closed.</param>
        /// <param name="closeReason"></param>
        protected void OnSocketSessionClosed(ISocketSession socketSession, CloseReason closeReason)
        {
            RaiseSocketClosed();
        }

        /// <summary>
        /// Asynchronously closes the WebSocket connection for the specified reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        protected override Task CloseWebSocketAsyncCore(string reason)
        {
            CheckDisposed();
            _session.CloseWithHandshake(reason);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Sends the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        protected override Task<bool> SendAsync(ArraySegment<byte> data)
        {
            CheckDisposed();
            try
            {
                _session.Send(data);
            }
            catch (Exception ex)
            {
                RaiseSocketError(ex);
                throw;
            }
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
                Logger.Verbose($"[{SessionKey}] Disposing EtpServer for {GetType().Name}");

                CloseWebSocket("Shutting down");
                _session?.Close();

                Logger.Verbose($"[{SessionKey}] Disposed EtpServer for {GetType().Name}");
            }

            base.Dispose(disposing);

            _session = null;
        }
    }
}

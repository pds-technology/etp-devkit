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

        /// <summary>
        /// Gets a value indicating whether the connection is open.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the connection is open; otherwise, <c>false</c>.
        /// </value>
        public override bool IsOpen => (_session?.Connected ?? false) && (!_session?.InClosing ?? false);

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpServer"/> class.
        /// </summary>
        /// <param name="session">The web socket session.</param>
        /// <param name="application">The serve application name.</param>
        /// <param name="version">The server application version.</param>
        /// <param name="instanceKey">The instance key to use in generating the server instance identifier.</param>
        /// <param name="headers">The WebSocket or HTTP headers.</param>
        public EtpServer(WebSocketSession session, string application, string version, string instanceKey, IDictionary<string, string> headers)
            : base(EtpWebSocketValidation.GetEtpVersion(session.SubProtocol.Name), application, version, instanceKey, headers, false, false)
        {
            _session = session;
            _session.SocketSession.Closed += SocketSessionClosed;
            SessionId = session.SessionID;
        }

        /// <summary>
        /// Handler for the event indicating the socket has been closed.
        /// </summary>
        /// <param name="socketSession">The socket session that has closed.</param>
        /// <param name="closeReason"></param>
        protected void SocketSessionClosed(ISocketSession socketSession, CloseReason closeReason)
        {
            InvokeSocketClosed();
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
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        protected override Task<bool> SendAsync(byte[] data, int offset, int length)
        {
            CheckDisposed();
            try
            {
                _session.Send(data, offset, length);
            }
            catch (Exception ex)
            {
                InvokeSocketError(ex);
                throw;
            }
            return Task.FromResult(true);
        }

        /// <summary>
        /// Sends the specified messages.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override Task<bool> SendAsync(string message)
        {
            CheckDisposed();
            try
            {
                _session.Send(message);
            }
            catch (Exception ex)
            {
                InvokeSocketError(ex);
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
                CloseWebSocket("Shutting down");
                _session?.Close();
            }

            base.Dispose(disposing);

            _session = null;
        }
    }
}

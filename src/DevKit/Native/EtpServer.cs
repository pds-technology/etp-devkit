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
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;

namespace Energistics.Etp.Native
{
    /// <summary>
    /// An ETP server session implementation that can be used with .NET WebSockets.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.EtpSession" />
    public class EtpServer : EtpSessionNativeBase, IEtpServer
    {
        /// <summary>
        /// Initializes the <see cref="EtpServer"/> class.
        /// </summary>
        static EtpServer()
        {
            Clients = new ConcurrentDictionary<string, EtpServer>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpServer"/> class.
        /// </summary>
        /// <param name="webSocket">The web socket.</param>
        /// <param name="application">The server application name.</param>
        /// <param name="version">The server application version.</param>
        /// <param name="headers">The WebSocket or HTTP headers.</param>
        public EtpServer(WebSocket webSocket, string application, string version, IDictionary<string, string> headers)
            : base(EtpWebSocketValidation.GetEtpVersion(webSocket.SubProtocol), webSocket, application, version, headers, false)
        {
            //var etpVersion = EtpWebSocketValidation.GetEtpVersion(Socket.SubProtocol);
            SessionId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets the collection of active clients.
        /// </summary>
        /// <value>The clients.</value>
        public static ConcurrentDictionary<string, EtpServer> Clients { get; }

        /// <summary>
        /// Called to let derived classes register details of a new connection.
        /// </summary>
        protected override void RegisterNewConnection()
        {
            Logger.Debug(Log("[{0}] Socket session connected.", SessionId));

            InvokeSocketOpened();

            // keep track of connected clients
            Clients.AddOrUpdate(SessionId, this, (id, client) => this);
        }

        /// <summary>
        /// Called to let derived classes cleanup after a connection has ended.
        /// </summary>
        protected override void CleanupAfterConnection()
        {
            EtpServer item;

            // remove client after connection ends
            if (Clients.TryRemove(SessionId, out item))
            {
                if (item != this)
                {
                    Clients.AddOrUpdate(item.SessionId, item, (id, client) => item);
                }
            }
        }
    }
}

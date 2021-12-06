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
        /// Initializes a new instance of the <see cref="EtpServer"/> class.
        /// </summary>
        /// <param name="webSocket">The web socket.</param>
        /// <param name="etpVersion">The ETP version for the session.</param>
        /// <param name="encoding">The ETP encoding for the session.</param>
        /// <param name="info">The server's information.</param>
        /// <param name="parameters">The server's parameters.</param>
        /// <param name="headers">The WebSocket or HTTP headers.</param>
        public EtpServer(WebSocket webSocket, EtpVersion etpVersion, EtpEncoding encoding, EtpEndpointInfo info, EtpEndpointParameters parameters = null, IDictionary<string, string> headers = null)
            : base(etpVersion, encoding, webSocket, info, parameters, headers, false, Guid.NewGuid().ToString())
        {
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
            StartReceiveLoop();
            return true;
        }
    }
}

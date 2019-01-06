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

namespace Energistics.Etp
{
    /// <summary>
    /// An ETP server session implementation that can be used with SuperWebSocket sessions.
    /// </summary>
    [Obsolete("Use WebSocket4Net.EtpServer instead.")]
    public class EtpServer : WebSocket4Net.EtpServer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EtpServer"/> class.
        /// </summary>
        /// <param name="session">The web socket session.</param>
        /// <param name="application">The serve application name.</param>
        /// <param name="version">The server application version.</param>
        /// <param name="headers">The WebSocket or HTTP headers.</param>
        public EtpServer(SuperWebSocket.WebSocketSession session, string application, string version, IDictionary<string, string> headers)
            : base(session, application, version, headers)
        {
        }
    }
}

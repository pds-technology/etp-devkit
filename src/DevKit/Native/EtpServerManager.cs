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

namespace Energistics.Etp.Native
{
    /// <summary>
    /// Provides common functionality for managing ETP servers.
    /// </summary>
    public class EtpServerManager : EtpServerManagerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EtpServerManager"/> class.
        /// </summary>
        /// <param name="webServerDetails">The web server details.</param>
        /// <param name="endpointInfo">The server manager's endpoint information.</param>
        /// <param name="endpointParameters">The server manager's endpoint parameters.</param>
        public EtpServerManager(EtpWebServerDetails webServerDetails, EtpEndpointInfo endpointInfo, EtpEndpointParameters endpointParameters = null)
            : base(webServerDetails, endpointInfo, endpointParameters)
        {
        }

        /// <summary>
        /// Creates an <see cref="IEtpServer"/> instance.
        /// </summary>
        /// <param name="webSocket">The websocket to create the server for.</param>
        /// <param name="etpVersion">The ETP version for the session.</param>
        /// <param name="encoding">The ETP encoding for the session.</param>
        /// <param name="headers">The websocket headers.</param>
        /// <returns>The created server.</returns>
        protected override IEtpServer CreateServerCore(IEtpServerWebSocket webSocket, EtpVersion etpVersion, EtpEncoding encoding, IDictionary<string, string> headers)
        {
            var ws = webSocket as EtpServerWebSocket;
            if (ws == null)
                throw new ArgumentException("Must be a native websocket", nameof(webSocket));

            return new EtpServer(ws.WebSocket, etpVersion, encoding, EndpointInfo, parameters: EndpointParameters.CloneForVersion(etpVersion), headers: headers);
        }
    }
}

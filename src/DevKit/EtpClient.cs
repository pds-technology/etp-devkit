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
    /// A wrapper for the WebSocket4Net library providing client connectivity to an ETP server.
    /// </summary>
    [Obsolete("Use Native.EtpClient or WebSocket4Net.EtpClient instead.")]
    public class EtpClient : WebSocket4Net.EtpClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EtpClient" /> class.
        /// </summary>
        /// <param name="uri">The ETP server URI.</param>
        /// <param name="application">The client application name.</param>
        /// <param name="version">The client application version.</param>
        /// <param name="etpSubProtocol">The ETP sub protocol.</param>
        public EtpClient(string uri, string application, string version, string etpSubProtocol) : base(uri, application, version, etpSubProtocol)
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
            : base(uri, application, version, etpSubProtocol, headers)
        {
        }
    }
}

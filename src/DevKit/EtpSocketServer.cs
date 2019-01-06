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

namespace Energistics.Etp
{
    /// <summary>
    /// An ETP server session implementation that can be used with .NET WebSockets.
    /// </summary>
    [Obsolete("Use WebSocket4Net.EtpSelfHostedWebServer instead.")]
    public class EtpSocketServer : WebSocket4Net.EtpSelfHostedWebServer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EtpSocketServer"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="application">The server application name.</param>
        /// <param name="version">The server application version.</param>
        public EtpSocketServer(int port, string application, string version)
            : base(port, application, version)
        {
        }
    }
}

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

using System.Collections.Generic;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;

namespace Energistics.Etp.v11.Protocol.Core
{
    /// <summary>
    /// Represents the interface that must be implemented from the client side of Protocol 0.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.Core, "client", "server")]
    public interface ICoreClient : IProtocolHandler
    {
        /// <summary>
        /// Sends a RequestSession message to a server.
        /// </summary>
        /// <param name="applicationName">The application name.</param>
        /// <param name="applicationVersion">The application version.</param>
        /// <param name="requestedProtocols">The requested protocols.</param>
        /// <returns>The message identifier.</returns>
        long RequestSession(string applicationName, string applicationVersion, IList<ISupportedProtocol> requestedProtocols);

        /// <summary>
        /// Sends a CloseSession message to a server.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <returns>The message identifier.</returns>
        long CloseSession(string reason = null);

        /// <summary>
        /// Renews the security token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The message identifier.</returns>
        long RenewSecurityToken(string token);

        /// <summary>
        /// Handles the OpenSession event from a server.
        /// </summary>
        event ProtocolEventHandler<OpenSession> OnOpenSession;

        /// <summary>
        /// Handles the CloseSession event from a server.
        /// </summary>
        event ProtocolEventHandler<CloseSession> OnCloseSession;
    }
}

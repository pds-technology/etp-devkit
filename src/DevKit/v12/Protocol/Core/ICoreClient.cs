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

namespace Energistics.Etp.v12.Protocol.Core
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
        /// <param name="requestedProtocols">The requested protocols.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long RequestSession(IReadOnlyList<EtpSessionProtocol> requestedProtocols);

        /// <summary>
        /// Sends a CloseSession message to a server.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long CloseSession(string reason = null);

        /// <summary>
        /// Sends a Ping message.
        /// </summary>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long Ping();

        /// <summary>
        /// Sends a Pong response message.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long Pong(IMessageHeader request);

        /// <summary>
        /// Renews the security token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long RenewSecurityToken(string token);

        /// <summary>
        /// Handles the OpenSession event from a server.
        /// </summary>
        event ProtocolEventHandler<OpenSession> OnOpenSession;

        /// <summary>
        /// Handles the CloseSession event from a server.
        /// </summary>
        event ProtocolEventHandler<CloseSession> OnCloseSession;

        /// <summary>
        /// Handles the Ping event from a server.
        /// </summary>
        event ProtocolEventHandler<Ping> OnPing;

        /// <summary>
        /// Handles the Pong event from a server.
        /// </summary>
        event ProtocolEventHandler<Pong> OnPong;

        /// <summary>
        /// Handles the RenewSecurityTokenResponse event from a server.
        /// </summary>
        event ProtocolEventHandler<RenewSecurityTokenResponse> OnRenewSecurityTokenResponse;
    }
}

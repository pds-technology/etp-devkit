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
    /// Represents the server end of the interface that must be implemented for Protocol 0.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.Core, "server", "client")]
    public interface ICoreServer : IProtocolHandler
    {
        /// <summary>
        /// Sends an OpenSession message to a client.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long OpenSession(IMessageHeader request);

        /// <summary>
        /// Sends a CloseSession message to a client.
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
        /// Sends a RenewSecurityTokenResponse response message to a client.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long RenewSecurityTokenResponse(IMessageHeader request);

        /// <summary>
        /// Handles the RequestSession event from a client.
        /// </summary>
        event ProtocolEventHandler<RequestSession> OnRequestSession;

        /// <summary>
        /// Handles the CloseSession event from a client.
        /// </summary>
        event ProtocolEventHandler<CloseSession> OnCloseSession;

        /// <summary>
        /// Handles the Ping event from a client.
        /// </summary>
        event ProtocolEventHandler<Ping> OnPing;

        /// <summary>
        /// Handles the Pong event from a client.
        /// </summary>
        event ProtocolEventHandler<Pong> OnPong;

        /// <summary>
        /// Handles the RenewSecurityToken event from a client.
        /// </summary>
        event ProtocolEventHandler<RenewSecurityToken> OnRenewSecurityToken;
    }
}

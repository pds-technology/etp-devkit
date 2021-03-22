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
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;

namespace Energistics.Etp.v12.Protocol.Core
{
    /// <summary>
    /// Represents the server end of the interface that must be implemented for Protocol 0.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.Core, Roles.Server, Roles.Client)]
    public interface ICoreServer : IProtocolHandler
    {
        /// <summary>
        /// Sends a Ping message.
        /// </summary>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<Ping> Ping(IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the Ping event from a client.
        /// </summary>
        event EventHandler<EmptyRequestEventArgs<Ping>> OnPing;

        /// <summary>
        /// Sends a Pong response message.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<Pong> Pong(IMessageHeader correlatedHeader, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the Pong event from a client.
        /// </summary>
        event EventHandler<ResponseEventArgs<Ping, Pong>> OnPong;

        /// <summary>
        /// Handles the RenewSecurityToken event from a client.
        /// </summary>
        event EventHandler<EmptyRequestEventArgs<RenewSecurityToken>> OnRenewSecurityToken;

        /// <summary>
        /// Sends a RenewSecurityTokenResponse response message to a client.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<RenewSecurityTokenResponse> RenewSecurityTokenResponse(IMessageHeader correlatedHeader, IMessageHeaderExtension extension = null);
    }
}

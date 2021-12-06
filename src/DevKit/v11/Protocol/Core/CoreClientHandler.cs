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
using Energistics.Etp.Common.Protocol.Core;
using System;

namespace Energistics.Etp.v11.Protocol.Core
{
    /// <summary>
    /// Base implementation of the <see cref="ICoreClient"/> interface.
    /// </summary>
    /// <seealso cref="Etp11ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v11.Protocol.Core.ICoreClient" />
    public class CoreClientHandler : Etp11ProtocolHandler, ICoreClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreClientHandler"/> class.
        /// </summary>
        public CoreClientHandler() : base((int)Protocols.Core, Roles.Client, Roles.Server)
        {
        }

        /// <summary>
        /// Renews the security token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<RenewSecurityToken> RenewSecurityToken(string token)
        {
            var body = new RenewSecurityToken
            {
                Token = token ?? string.Empty
            };

            return SendRequest(body);
        }

        /// <summary>
        /// Event raised when there is an exception received in response to a RenewSecurityToken message.
        /// </summary>
        public event EventHandler<VoidResponseEventArgs<RenewSecurityToken>> OnRenewSecurityTokenException;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<RenewSecurityToken>)
                HandleResponseMessage(request as EtpMessage<RenewSecurityToken>, message, OnRenewSecurityTokenException, HandleRenewSecurityTokenException);
        }

        /// <summary>
        /// Handles exceptions to the RenewSecurityToken message from a server.
        /// </summary>
        /// <param name="args">The <see cref="VoidResponseEventArgs{RenewSecurityToken}"/> instance containing the event data.</param>
        protected virtual void HandleRenewSecurityTokenException(VoidResponseEventArgs<RenewSecurityToken> args)
        {
        }
    }
}

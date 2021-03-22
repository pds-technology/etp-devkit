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

namespace Energistics.Etp.v11.Protocol.Core
{
    /// <summary>
    /// Represents the interface that must be implemented from the client side of Protocol 0.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.Core, Roles.Client, Roles.Server)]
    public interface ICoreClient : IProtocolHandler
    {
        /// <summary>
        /// Renews the security token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<RenewSecurityToken> RenewSecurityToken(string token);

        /// <summary>
        /// Event raised when there is an exception received in response to a RenewSecurityToken message.
        /// </summary>
        event EventHandler<VoidResponseEventArgs<RenewSecurityToken>> OnRenewSecurityTokenException;
    }
}

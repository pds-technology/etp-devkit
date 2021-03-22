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
using System.Linq;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v11.Datatypes;

namespace Energistics.Etp.v11.Protocol.Core
{
    /// <summary>
    /// Base implementation of the <see cref="ICoreServer"/> interface.
    /// </summary>
    /// <seealso cref="Etp11ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v11.Protocol.Core.ICoreServer" />
    public class CoreServerHandler : Etp11ProtocolHandler, ICoreServer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreServerHandler"/> class.
        /// </summary>
        public CoreServerHandler() : base((int)Protocols.Core, Roles.Server, Roles.Client)
        {
            RegisterMessageHandler<RenewSecurityToken>(Protocols.Core, MessageTypes.Core.RenewSecurityToken, HandleRenewSecurityToken);
        }

        /// <summary>
        /// Handles the RenewSecurityToken event from a client.
        /// </summary>
        public event EventHandler<VoidRequestEventArgs<RenewSecurityToken>> OnRenewSecurityToken;

        /// <summary>
        /// Handles the RenewSecurityToken message from a client.
        /// </summary>
        /// <param name="message">The RenewSecurityToken message.</param>
        protected virtual void HandleRenewSecurityToken(EtpMessage<RenewSecurityToken> message)
        {
            HandleRequestMessage(message, OnRenewSecurityToken, HandleRenewSecurityToken);
        }

        /// <summary>
        /// Handles the RenewSecurityToken message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="VoidRequestEventArgs{RenewSecurityToken}"/> instance containing the event data.</param>
        protected virtual void HandleRenewSecurityToken(VoidRequestEventArgs<RenewSecurityToken> args)
        {
        }
    }
}

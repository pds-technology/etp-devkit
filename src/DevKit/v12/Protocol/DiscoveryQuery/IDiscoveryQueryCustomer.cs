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
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v12.Datatypes.Object;
using System;

namespace Energistics.Etp.v12.Protocol.DiscoveryQuery
{
    /// <summary>
    /// Describes the interface that must be implemented by the customer role of the DiscoveryQuery protocol.
    /// </summary>
    /// <seealso cref="IProtocolHandler" />
    [ProtocolRole((int)Protocols.DiscoveryQuery, Roles.Customer, Roles.Store)]
    public interface IDiscoveryQueryCustomer : IProtocolHandler<ICapabilitiesCustomer, ICapabilitiesStore>
    {
        /// <summary>
        /// Sends a FindResources message to a store.
        /// </summary>
        /// <param name="context">The context information.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="storeLastWriteFilter">An optional parameter to filter discovery on a date when an object last changed.</param>
        /// <param name="activeStatusFilter">if not <c>null</c>, request only objects with a matching active status.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<FindResources> FindResources(ContextInfo context, ContextScopeKind scope, long? storeLastWriteFilter = null, ActiveStatusKind? activeStatusFilter = null, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the FindResourcesResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<FindResources, FindResourcesResponse>> OnFindResourcesResponse;
    }
}

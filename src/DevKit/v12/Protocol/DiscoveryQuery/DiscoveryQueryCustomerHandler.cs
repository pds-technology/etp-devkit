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
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Protocol.Core;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.DiscoveryQuery
{
    /// <summary>
    /// Base implementation of the <see cref="IDiscoveryQueryCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.DiscoveryQuery.IDiscoveryQueryCustomer" />
    public class DiscoveryQueryCustomerHandler : Etp12ProtocolHandler<CapabilitiesCustomer, ICapabilitiesCustomer, CapabilitiesStore, ICapabilitiesStore>, IDiscoveryQueryCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscoveryQueryCustomerHandler"/> class.
        /// </summary>
        public DiscoveryQueryCustomerHandler() : base((int)Protocols.DiscoveryQuery, Roles.Customer, Roles.Store)
        {
            RegisterMessageHandler<FindResourcesResponse>(Protocols.DiscoveryQuery, MessageTypes.DiscoveryQuery.FindResourcesResponse, HandleFindResourcesResponse);
        }

        /// <summary>
        /// Sends a FindResources message to a store.
        /// </summary>
        /// <param name="context">The context information.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="storeLastWriteFilter">An optional parameter to filter discovery on a date when an object last changed.</param>
        /// <param name="activeStatusFilter">if not <c>null</c>, request only objects with a matching active status.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<FindResources> FindResources(ContextInfo context, ContextScopeKind scope, DateTime? storeLastWriteFilter = null, ActiveStatusKind? activeStatusFilter = null, IMessageHeaderExtension extension = null)
        {
            var body = new FindResources
            {
                Context = context,
                Scope = scope,
                StoreLastWriteFilter = storeLastWriteFilter,
                ActiveStatusFilter = activeStatusFilter,
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Handles the FindResourcesResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<FindResources, FindResourcesResponse>> OnFindResourcesResponse;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<FindResources>)
                HandleResponseMessage(request as EtpMessage<FindResources>, message, OnFindResourcesResponse, HandleFindResourcesResponse);
        }

        /// <summary>
        /// Handles the FindResourcesResponse message from a store.
        /// </summary>
        /// <param name="message">The FindResourcesResponse message.</param>
        protected virtual void HandleFindResourcesResponse(EtpMessage<FindResourcesResponse> message)
        {
            var request = TryGetCorrelatedMessage<FindResources>(message);
            HandleResponseMessage(request, message, OnFindResourcesResponse, HandleFindResourcesResponse);
        }

        /// <summary>
        /// Handles the FindResourcesResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{FindResources, FindResourcesResponse}"/> instance containing the event data.</param>
        protected virtual void HandleFindResourcesResponse(ResponseEventArgs<FindResources, FindResourcesResponse> args)
        {
        }
    }
}

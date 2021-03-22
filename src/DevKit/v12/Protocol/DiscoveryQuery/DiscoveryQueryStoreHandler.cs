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
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.DiscoveryQuery
{
    /// <summary>
    /// Base implementation of the <see cref="IDiscoveryQueryStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.DiscoveryQuery.IDiscoveryQueryStore" />
    public class DiscoveryQueryStoreHandler : Etp12ProtocolHandler<CapabilitiesStore, ICapabilitiesStore, CapabilitiesCustomer, ICapabilitiesCustomer>, IDiscoveryQueryStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscoveryQueryStoreHandler"/> class.
        /// </summary>
        public DiscoveryQueryStoreHandler() : base((int)Protocols.DiscoveryQuery, Roles.Store, Roles.Customer)
        {
            RegisterMessageHandler<FindResources>(Protocols.DiscoveryQuery, MessageTypes.DiscoveryQuery.FindResources, HandleFindResources);
        }

        /// <summary>
        /// Handles the FindResources event from a customer.
        /// </summary>
        public event EventHandler<ListRequestWithContextEventArgs<FindResources, Resource, ResponseContext>> OnFindResources;

        /// <summary>
        /// Sends a FindResourcesResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="resources">The list of <see cref="Resource"/> objects.</param>
        /// <param name="serverSortOrder">The server sort order.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<FindResourcesResponse> FindResourcesResponse(IMessageHeader correlatedHeader, IList<Resource> resources, string serverSortOrder, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new FindResourcesResponse
            {
                Resources = resources ?? new List<Resource>(),
                ServerSortOrder = serverSortOrder,
            };
            
            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Handles the FindResources message from a customer.
        /// </summary>
        /// <param name="message">The FindResources message.</param>
        protected virtual void HandleFindResources(EtpMessage<FindResources> message)
        {
            HandleRequestMessage(message, OnFindResources, HandleFindResources,
                responseMethod: (args) => FindResourcesResponse(args.Request?.Header, args.Responses, args.Context?.ServerSortOrder, isFinalPart: !args.HasErrors, extension: args.ResponseExtension));
        }

        /// <summary>
        /// Handles the FindResources message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="ListRequestWithContextEventArgs{FindResources, Resource, ResponseContext}"/> instance containing the event data.</param>
        protected virtual void HandleFindResources(ListRequestWithContextEventArgs<FindResources, Resource, ResponseContext> args)
        {
        }
    }
}

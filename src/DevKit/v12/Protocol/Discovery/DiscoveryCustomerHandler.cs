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

using System.Collections.Concurrent;
using System.Collections.Generic;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.Discovery
{
    /// <summary>
    /// Base implementation of the <see cref="IDiscoveryCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.Discovery.IDiscoveryCustomer" />
    public class DiscoveryCustomerHandler : Etp12ProtocolHandler, IDiscoveryCustomer
    {
        private readonly IDictionary<long, string> _requests;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscoveryCustomerHandler"/> class.
        /// </summary>
        public DiscoveryCustomerHandler() : base((int)Protocols.Discovery, "customer", "store")
        {
            _requests = new ConcurrentDictionary<long, string>();

            RegisterMessageHandler<GetResourcesResponse>(Protocols.Discovery, MessageTypes.Discovery.GetResourcesResponse, HandleGetResourcesResponse);
        }

        /// <summary>
        /// Sends a GetTreeResources message to a store.
        /// </summary>
        /// <param name="contextInfo">The context information.</param>
        /// <returns>The message identifier.</returns>
        public virtual long GetTreeResources(ContextInfo contextInfo)
        {
            var header = CreateMessageHeader(Protocols.Discovery, MessageTypes.Discovery.GetTreeResources);

            var getResources = new GetTreeResources
            {
                Context = contextInfo
            };

            return Session.SendMessage(header, getResources,
                h => _requests[h.MessageId] = contextInfo.Uri // Cache requested URIs by message ID
            );
        }

        /// <summary>
        /// Sends a GetGraphResources message to a store.
        /// </summary>
        /// <param name="contextInfo">The context information.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="groupByType">if set to <c>true</c> group by type.</param>
        /// <returns>The message identifier.</returns>
        public virtual long GetGraphResources(ContextInfo contextInfo, ContextScopeKind scope, bool groupByType)
        {
            var header = CreateMessageHeader(Protocols.Discovery, MessageTypes.Discovery.GetGraphResources);

            var getResources = new GetGraphResources
            {
                Context = contextInfo,
                Scope = scope,
                GroupByType = groupByType
            };

            return Session.SendMessage(header, getResources,
                h => _requests[h.MessageId] = contextInfo.Uri // Cache requested URIs by message ID
            );
        }

        /// <summary>
        /// Handles the GetResourcesResponse event from a store.
        /// </summary>
        public event ProtocolEventHandler<GetResourcesResponse, string> OnGetResourcesResponse;

        /// <summary>
        /// Handle any final cleanup related to the final message in response to a request.
        /// </summary>
        /// <param name="correlationId">The correlation ID of the request</param>
        protected override void HandleFinalResponse(long correlationId)
        {
            _requests.Remove(correlationId);
        }

        /// <summary>
        /// Handles the GetResourcesResponse message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="getResourcesResponse">The GetResourcesResponse message.</param>
        protected virtual void HandleGetResourcesResponse(IMessageHeader header, GetResourcesResponse getResourcesResponse)
        {
            var uri = GetRequestedUri(header);
            var args = Notify(OnGetResourcesResponse, header, getResourcesResponse, uri);
            HandleGetResourcesResponse(args);
        }

        /// <summary>
        /// Handles the GetResourcesResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ProtocolEventArgs{GetResourcesResponse}"/> instance containing the event data.</param>
        protected virtual void HandleGetResourcesResponse(ProtocolEventArgs<GetResourcesResponse, string> args)
        {
        }

        /// <summary>
        /// Gets the requested URI from the internal cache of message IDs.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <returns>The requested URI.</returns>
        private string GetRequestedUri(IMessageHeader header)
        {
            string uri;
            _requests.TryGetValue(header.CorrelationId, out uri);
            return uri;
        }
    }
}

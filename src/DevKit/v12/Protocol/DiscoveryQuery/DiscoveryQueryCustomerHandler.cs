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

namespace Energistics.Etp.v12.Protocol.DiscoveryQuery
{
    /// <summary>
    /// Base implementation of the <see cref="IDiscoveryQueryCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.DiscoveryQuery.IDiscoveryQueryCustomer" />
    public class DiscoveryQueryCustomerHandler : Etp12ProtocolHandler, IDiscoveryQueryCustomer
    {
        private readonly IDictionary<long, string> _requests;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscoveryQueryCustomerHandler"/> class.
        /// </summary>
        public DiscoveryQueryCustomerHandler() : base((int)Protocols.DiscoveryQuery, "customer", "store")
        {
            _requests = new ConcurrentDictionary<long, string>();

            RegisterMessageHandler<FindResourcesResponse>(Protocols.DiscoveryQuery, MessageTypes.DiscoveryQuery.FindResourcesResponse, HandleFindResourcesResponse);
        }

        /// <summary>
        /// Sends a FindResources message to a store.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The message identifier.</returns>
        public virtual long FindResources(string uri)
        {
            var header = CreateMessageHeader(Protocols.DiscoveryQuery, MessageTypes.DiscoveryQuery.FindResources);

            var findResources = new FindResources()
            {
                Uri = uri
            };
            
            return Session.SendMessage(header, findResources,
                h => _requests[h.MessageId] = uri // Cache requested URIs by message ID
            );
        }

        /// <summary>
        /// Handles the FindResourcesResponse event from a store.
        /// </summary>
        public event ProtocolEventHandler<FindResourcesResponse, string> OnFindResourcesResponse;

        /// <summary>
        /// Handle any final cleanup related to the final message in response to a request.
        /// </summary>
        /// <param name="correlationId">The correlation ID of the request</param>
        protected override void HandleFinalResponse(long correlationId)
        {
            _requests.Remove(correlationId);
        }

        /// <summary>
        /// Handles the FindResourcesResponse message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="findResourcesResponse">The FindResourcesResponse message.</param>
        protected virtual void HandleFindResourcesResponse(IMessageHeader header, FindResourcesResponse findResourcesResponse)
        {
            var uri = GetRequestedUri(header);
            var args = Notify(OnFindResourcesResponse, header, findResourcesResponse, uri);
            HandleFindResourcesResponse(args);
        }

        /// <summary>
        /// Handles the FindResourcesResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ProtocolEventArgs{FindResourcesResponse}"/> instance containing the event data.</param>
        protected virtual void HandleFindResourcesResponse(ProtocolEventArgs<FindResourcesResponse, string> args)
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

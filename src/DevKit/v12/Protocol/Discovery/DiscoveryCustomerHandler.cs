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
        private readonly ConcurrentDictionary<long, GetResources> _requests = new ConcurrentDictionary<long, GetResources>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscoveryCustomerHandler"/> class.
        /// </summary>
        public DiscoveryCustomerHandler() : base((int)Protocols.Discovery, "customer", "store")
        {
            RegisterMessageHandler<GetResourcesResponse>(Protocols.Discovery, MessageTypes.Discovery.GetResourcesResponse, HandleGetResourcesResponse);
        }

        /// <summary>
        /// The maximum number of response messages the store will return.
        /// </summary>
        /// <remarks>Should be set once the session is established.</remarks>
        public long StoreMaxResponseCount { get; private set; }

        /// <summary>
        /// Sets <see cref="StoreMaxResponseCount"/>.
        /// </summary>
        /// <param name="counterpartCapabilities">The counterpart's protocol capabilities.</param>
        public override void OnSessionOpened(EtpProtocolCapabilities counterpartCapabilities)
        {
            base.OnSessionOpened(counterpartCapabilities);

            StoreMaxResponseCount = counterpartCapabilities.MaxResponseCount ?? long.MaxValue;
        }

        /// <summary>
        /// Sends a GetResources message to a store.
        /// </summary>
        /// <param name="context">The context information.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="lastChangedFilter">An optional parameter to filter discovery on a date when an object last changed.</param>
        /// <param name="countObjects">if set to <c>true</c>, request object counts.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long GetResources(ContextInfo context, ContextScopeKind scope, long? lastChangedFilter = null, bool countObjects = false)
        {
            var header = CreateMessageHeader(Protocols.Discovery, MessageTypes.Discovery.GetResources);

            var message = new GetResources
            {
                Context = context,
                Scope = scope,
                LastChangedFilter = lastChangedFilter,
                CountObjects = countObjects,
            };

            return Session.SendMessage(header, message,
                h => _requests[h.MessageId] = message// Cache requested URIs by message ID
            );
        }

        /// <summary>
        /// Handles the GetResourcesResponse event from a store.
        /// </summary>
        public event ProtocolEventHandler<GetResourcesResponse, GetResources> OnGetResourcesResponse;

        /// <summary>
        /// Handles the GetResourcesResponse message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GetResourcesResponse message.</param>
        protected virtual void HandleGetResourcesResponse(IMessageHeader header, GetResourcesResponse message)
        {
            var request = GetRequest(header);
            var args = Notify(OnGetResourcesResponse, header, message, request);
            if (args.Cancel)
                return;

            HandleGetResourcesResponse(header, message, request);
        }

        /// <summary>
        /// Handles the GetResourcesResponse message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GetResourcesResponse message.</param>
        /// <param name="request">The GetResources request.</param>
        protected virtual void HandleGetResourcesResponse(IMessageHeader header, GetResourcesResponse message, GetResources request)
        {
        }

        /// <summary>
        /// Handle any final cleanup related to the final message in response to a request.
        /// </summary>
        /// <param name="correlationId">The correlation ID of the request</param>
        protected override void HandleFinalResponse(long correlationId)
        {
            GetResources request;
            _requests.TryRemove(correlationId, out request);
        }

        /// <summary>
        /// Gets the request from the internal cache of message IDs.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <returns>The request.</returns>
        private GetResources GetRequest(IMessageHeader header)
        {
            GetResources request;
            _requests.TryGetValue(header.CorrelationId, out request);
            return request;
        }
    }
}

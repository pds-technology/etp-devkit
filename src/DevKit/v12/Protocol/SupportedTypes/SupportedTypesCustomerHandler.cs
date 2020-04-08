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

namespace Energistics.Etp.v12.Protocol.SupportedTypes
{
    /// <summary>
    /// Base implementation of the <see cref="ISupportedTypesCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.SupportedTypes.ISupportedTypesCustomer" />
    public class SupportedTypesCustomerHandler : Etp12ProtocolHandler, ISupportedTypesCustomer
    {
        private readonly ConcurrentDictionary<long, GetSupportedTypes> _requests = new ConcurrentDictionary<long, GetSupportedTypes>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedTypesCustomerHandler"/> class.
        /// </summary>
        public SupportedTypesCustomerHandler() : base((int)Protocols.SupportedTypes, "customer", "store")
        {
            RegisterMessageHandler<GetSupportedTypesResponse>(Protocols.SupportedTypes, MessageTypes.SupportedTypes.GetSupportedTypesResponse, HandleGetSupportedTypesResponse);
        }

        /// <summary>
        /// Sends a GetSupportedTypes message to a store.
        /// </summary>
        /// <param name="uri">The uri to to discover instantiated or supported data types from.</param>
        /// <param name="scope">The scope to return supported types for.</param>
        /// <param name="returnEmptyTypes">Whether the store should return data types that it supports but for which it currently has no data.</param>
        /// <param name="countObjects">if set to <c>true</c>, request object counts.</param>
        /// <returns>The message identifier.</returns>
        public virtual long GetSupportedTypes(string uri, ContextScopeKind scope, bool returnEmptyTypes = false, bool countObjects = false)
        {
            var header = CreateMessageHeader(Protocols.SupportedTypes, MessageTypes.SupportedTypes.GetSupportedTypes);

            var message = new GetSupportedTypes
            {
                Uri = uri,
                Scope = scope,
                ReturnEmptyTypes = returnEmptyTypes,
                CountObjects = countObjects,
            };

            return Session.SendMessage(header, message,
                h => _requests[h.MessageId] = message// Cache requested URIs by message ID
            );
        }

        /// <summary>
        /// Handles the GetSupportedTypesResponse event from a store.
        /// </summary>
        public event ProtocolEventHandler<GetSupportedTypesResponse, GetSupportedTypes> OnGetSupportedTypesResponse;

        /// <summary>
        /// Handles the GetResourcesResponse message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GetResourcesResponse message.</param>
        protected virtual void HandleGetSupportedTypesResponse(IMessageHeader header, GetSupportedTypesResponse message)
        {
            var request = GetRequest(header);
            var args = Notify(OnGetSupportedTypesResponse, header, message, request);
            if (args.Cancel)
                return;

            HandleGetSupportedTypesResponse(header, message, request);
        }

        /// <summary>
        /// Handles the GetResourcesResponse message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GetResourcesResponse message.</param>
        /// <param name="request">The GetResources request.</param>
        protected virtual void HandleGetSupportedTypesResponse(IMessageHeader header, GetSupportedTypesResponse message, GetSupportedTypes request)
        {
        }

        /// <summary>
        /// Handle any final cleanup related to the final message in response to a request.
        /// </summary>
        /// <param name="correlationId">The correlation ID of the request</param>
        protected override void HandleFinalResponse(long correlationId)
        {
            GetSupportedTypes request;
            _requests.TryRemove(correlationId, out request);
        }

        /// <summary>
        /// Gets the request from the internal cache of message IDs.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <returns>The request.</returns>
        private GetSupportedTypes GetRequest(IMessageHeader header)
        {
            GetSupportedTypes request;
            _requests.TryGetValue(header.CorrelationId, out request);
            return request;
        }
    }
}

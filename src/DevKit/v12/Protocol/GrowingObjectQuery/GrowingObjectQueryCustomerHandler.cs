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

namespace Energistics.Etp.v12.Protocol.GrowingObjectQuery
{
    /// <summary>
    /// Base implementation of the <see cref="IGrowingObjectQueryCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.GrowingObjectQuery.IGrowingObjectQueryCustomer" />
    public class GrowingObjectQueryCustomerHandler : Etp12ProtocolHandler, IGrowingObjectQueryCustomer
    {
        private readonly ConcurrentDictionary<long, FindParts> _requests = new ConcurrentDictionary<long, FindParts>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GrowingObjectQueryCustomerHandler"/> class.
        /// </summary>
        public GrowingObjectQueryCustomerHandler() : base((int)Protocols.GrowingObjectQuery, "customer", "store")
        {
            RegisterMessageHandler<FindPartsResponse>(Protocols.GrowingObjectQuery, MessageTypes.GrowingObjectQuery.FindPartsResponse, HandleFindPartsResponse);
        }

        /// <summary>
        /// Sends a FindParts message to a store.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long FindParts(string uri, string format = "xml")
        {
            var header = CreateMessageHeader(Protocols.GrowingObjectQuery, MessageTypes.GrowingObjectQuery.FindParts);

            var message = new FindParts()
            {
                Uri = uri,
                Format = format ?? "xml",
            };
            
            return Session.SendMessage(header, message,
                (h, _) => _requests[h.MessageId] = message // Cache requested URIs by message ID
            );
        }

        /// <summary>
        /// Handles the FindPartsResponse event from a store.
        /// </summary>
        public event ProtocolEventHandler<FindPartsResponse, FindParts> OnFindPartsResponse;

        /// <summary>
        /// Handles the FindPartsResponse message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The FindPartsResponse message.</param>
        protected virtual void HandleFindPartsResponse(IMessageHeader header, FindPartsResponse message)
        {
            var request = GetRequest(header);
            var args = Notify(OnFindPartsResponse, header, message, request);
            if (args.Cancel)
                return;

            HandleFindPartsResponse(header, message, request);
        }

        /// <summary>
        /// Handles the FindPartsResponse message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The FindPartsResponse message.</param>
        /// <param name="request">The FindParts request.</param>
        protected virtual void HandleFindPartsResponse(IMessageHeader header, FindPartsResponse message, FindParts request)
        {
        }

        /// <summary>
        /// Handle any final cleanup related to the final message in response to a request.
        /// </summary>
        /// <param name="correlationId">The correlation ID of the request</param>
        protected override void HandleFinalResponse(long correlationId)
        {
            FindParts request;
            _requests.TryRemove(correlationId, out request);
        }

        /// <summary>
        /// Gets the request from the internal cache of message IDs.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <returns>The request.</returns>
        private FindParts GetRequest(IMessageHeader header)
        {
            FindParts request;
            _requests.TryGetValue(header.CorrelationId, out request);
            return request;
        }
    }
}

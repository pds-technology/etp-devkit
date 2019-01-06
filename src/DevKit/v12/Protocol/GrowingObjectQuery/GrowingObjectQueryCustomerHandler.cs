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
        private readonly IDictionary<long, string> _requests;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrowingObjectQueryCustomerHandler"/> class.
        /// </summary>
        public GrowingObjectQueryCustomerHandler() : base((int)Protocols.GrowingObjectQuery, "customer", "store")
        {
            _requests = new ConcurrentDictionary<long, string>();

            RegisterMessageHandler<FindPartsResponse>(Protocols.GrowingObjectQuery, MessageTypes.GrowingObjectQuery.FindPartsResponse, HandleFindPartsResponse);
        }

        /// <summary>
        /// Sends a FindParts message to a store.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The message identifier.</returns>
        public virtual long FindParts(string uri)
        {
            var header = CreateMessageHeader(Protocols.GrowingObjectQuery, MessageTypes.GrowingObjectQuery.FindParts);

            var findParts = new FindParts()
            {
                Uri = uri
            };
            
            return Session.SendMessage(header, findParts,
                h => _requests[h.MessageId] = uri // Cache requested URIs by message ID
            );
        }

        /// <summary>
        /// Handles the FindPartsResponse event from a store.
        /// </summary>
        public event ProtocolEventHandler<FindPartsResponse, string> OnFindPartsResponse;

        /// <summary>
        /// Handle any final cleanup related to the final message in response to a request.
        /// </summary>
        /// <param name="correlationId">The correlation ID of the request</param>
        protected override void HandleFinalResponse(long correlationId)
        {
            _requests.Remove(correlationId);
        }

        /// <summary>
        /// Handles the FindPartsResponse message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="findPartsResponse">The FindPartsResponse message.</param>
        protected virtual void HandleFindPartsResponse(IMessageHeader header, FindPartsResponse findPartsResponse)
        {
            var uri = GetRequestedUri(header);
            var args = Notify(OnFindPartsResponse, header, findPartsResponse, uri);
            HandleFindPartsResponse(args);
        }

        /// <summary>
        /// Handles the FindPartsResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ProtocolEventArgs{FindPartsResponse}"/> instance containing the event data.</param>
        protected virtual void HandleFindPartsResponse(ProtocolEventArgs<FindPartsResponse, string> args)
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

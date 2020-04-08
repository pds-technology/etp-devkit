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

namespace Energistics.Etp.v12.Protocol.StoreQuery
{
    /// <summary>
    /// Base implementation of the <see cref="IStoreQueryCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.StoreQuery.IStoreQueryCustomer" />
    public class StoreQueryCustomerHandler : Etp12ProtocolHandler, IStoreQueryCustomer
    {
        private readonly ConcurrentDictionary<long, FindObjects> _requests = new ConcurrentDictionary<long, FindObjects>();

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreQueryCustomerHandler"/> class.
        /// </summary>
        public StoreQueryCustomerHandler() : base((int)Protocols.StoreQuery, "customer", "store")
        {
            RegisterMessageHandler<FindObjectsResponse>(Protocols.StoreQuery, MessageTypes.StoreQuery.FindObjectsResponse, HandleFindObjectsResponse);
            RegisterMessageHandler<Chunk>(Protocols.StoreQuery, MessageTypes.StoreQuery.Chunk, HandleChunk);
        }

        /// <summary>
        /// Sends a FindObjects message to a store.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <returns>The message identifier.</returns>
        public virtual long FindObjects(string uri, string format = "xml")
        {
            var header = CreateMessageHeader(Protocols.StoreQuery, MessageTypes.StoreQuery.FindObjects);

            var message = new FindObjects()
            {
                Uri = uri,
                Format = format ?? "xml",
            };
            
            return Session.SendMessage(header, message,
                h => _requests[h.MessageId] = message // Cache requested URIs by message ID
            );
        }

        /// <summary>
        /// Handles the FindObjectsResponse event from a store.
        /// </summary>
        public event ProtocolEventHandler<FindObjectsResponse, FindObjects> OnFindObjectsResponse;

        /// <summary>
        /// Handles the Chunk event from a store.
        /// </summary>
        public event ProtocolEventHandler<Chunk> OnChunk;

        /// <summary>
        /// Handle any final cleanup related to the final message in response to a request.
        /// </summary>
        /// <param name="correlationId">The correlation ID of the request</param>
        protected override void HandleFinalResponse(long correlationId)
        {
            FindObjects request;
            _requests.TryRemove(correlationId, out request);
        }

        /// <summary>
        /// Handles the FindObjectsResponse message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The FindObjectsResponse message.</param>
        protected virtual void HandleFindObjectsResponse(IMessageHeader header, FindObjectsResponse message)
        {
            var request = GetRequest(header);
            var args = Notify(OnFindObjectsResponse, header, message, request);
            if (args.Cancel)
                return;

            HandleFindObjectsResponse(header, message, request);
        }

        /// <summary>
        /// Handles the FindObjectsResponse message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The FindObjectsResponse message.</param>
        /// <param name="request">The FindObjects request.</param>
        protected virtual void HandleFindObjectsResponse(IMessageHeader header, FindObjectsResponse message, FindObjects request)
        {
        }

        /// <summary>
        /// Handles the Chunk message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="notification">The Chunk message.</param>
        protected virtual void HandleChunk(IMessageHeader header, Chunk notification)
        {
            Notify(OnChunk, header, notification);
        }

        /// <summary>
        /// Gets the request from the internal cache of message IDs.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <returns>The requested URI.</returns>
        private FindObjects GetRequest(IMessageHeader header)
        {
            FindObjects request;
            _requests.TryGetValue(header.CorrelationId, out request);
            return request;
        }
    }
}

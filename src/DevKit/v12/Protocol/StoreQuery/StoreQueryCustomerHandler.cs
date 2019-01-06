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
        private readonly IDictionary<long, string> _requests;

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreQueryCustomerHandler"/> class.
        /// </summary>
        public StoreQueryCustomerHandler() : base((int)Protocols.StoreQuery, "customer", "store")
        {
            _requests = new ConcurrentDictionary<long, string>();

            RegisterMessageHandler<FindObjectsResponse>(Protocols.StoreQuery, MessageTypes.StoreQuery.FindObjectsResponse, HandleFindObjectsResponse);
        }

        /// <summary>
        /// Sends a FindObjects message to a store.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The message identifier.</returns>
        public virtual long FindObjects(string uri)
        {
            var header = CreateMessageHeader(Protocols.StoreQuery, MessageTypes.StoreQuery.FindObjects);

            var findObjects = new FindObjects()
            {
                Uri = uri
            };
            
            return Session.SendMessage(header, findObjects,
                h => _requests[h.MessageId] = uri // Cache requested URIs by message ID
            );
        }

        /// <summary>
        /// Handles the FindObjectsResponse event from a store.
        /// </summary>
        public event ProtocolEventHandler<FindObjectsResponse, string> OnFindObjectsResponse;

        /// <summary>
        /// Handle any final cleanup related to the final message in response to a request.
        /// </summary>
        /// <param name="correlationId">The correlation ID of the request</param>
        protected override void HandleFinalResponse(long correlationId)
        {
            _requests.Remove(correlationId);
        }

        /// <summary>
        /// Handles the FindObjectsResponse message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="findObjectsResponse">The FindObjectsResponse message.</param>
        protected virtual void HandleFindObjectsResponse(IMessageHeader header, FindObjectsResponse findObjectsResponse)
        {
            var uri = GetRequestedUri(header);
            var args = Notify(OnFindObjectsResponse, header, findObjectsResponse, uri);
            HandleFindObjectsResponse(args);
        }

        /// <summary>
        /// Handles the FindObjectsResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ProtocolEventArgs{FindObjectsResponse}"/> instance containing the event data.</param>
        protected virtual void HandleFindObjectsResponse(ProtocolEventArgs<FindObjectsResponse, string> args)
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

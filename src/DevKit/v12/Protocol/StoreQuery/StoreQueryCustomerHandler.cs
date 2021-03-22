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
using System.Collections.Concurrent;
using System.Collections.Generic;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Protocol.Core;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.StoreQuery
{
    /// <summary>
    /// Base implementation of the <see cref="IStoreQueryCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.StoreQuery.IStoreQueryCustomer" />
    public class StoreQueryCustomerHandler : Etp12ProtocolHandler<CapabilitiesCustomer, ICapabilitiesCustomer, CapabilitiesStore, ICapabilitiesStore>, IStoreQueryCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreQueryCustomerHandler"/> class.
        /// </summary>
        public StoreQueryCustomerHandler() : base((int)Protocols.StoreQuery, Roles.Customer, Roles.Store)
        {
            RegisterMessageHandler<FindDataObjectsResponse>(Protocols.StoreQuery, MessageTypes.StoreQuery.FindDataObjectsResponse, HandleFindDataObjectsResponse);
            RegisterMessageHandler<Chunk>(Protocols.StoreQuery, MessageTypes.StoreQuery.Chunk, HandleChunk);
        }

        /// <summary>
        /// Sends a FindDataObjects message to a store.
        /// </summary>
        /// <param name="context">The context information.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="storeLastWriteFilter">An optional parameter to filter results on a date when an object last changed.</param>
        /// <param name="activeStatusFilter">if not <c>null</c>, request only objects with a matching active status.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<FindDataObjects> FindDataObjects(ContextInfo context, ContextScopeKind scope, long? storeLastWriteFilter = null, ActiveStatusKind? activeStatusFilter = null, string format = Formats.Xml, IMessageHeaderExtension extension = null)
        {
            var body = new FindDataObjects
            {
                Context = context,
                Scope = scope,
                StoreLastWriteFilter = storeLastWriteFilter,
                ActiveStatusFilter = activeStatusFilter,
                Format = format ?? Formats.Xml,
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Handles the FindDataObjectsResponse event from a store.
        /// </summary>
        public event EventHandler<DualResponseEventArgs<FindDataObjects, FindDataObjectsResponse, Chunk>> OnFindDataObjectsResponse;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<FindDataObjects>)
                HandleResponseMessage(request as EtpMessage<FindDataObjects>, message, OnFindDataObjectsResponse, HandleFindDataObjectsResponse);
        }

        /// <summary>
        /// Handles the FindDataObjectsResponse message from a store.
        /// </summary>
        /// <param name="message">The FindDataObjectsResponse message.</param>
        protected virtual void HandleFindDataObjectsResponse(EtpMessage<FindDataObjectsResponse> message)
        {
            var request = TryGetCorrelatedMessage<FindDataObjects>(message);
            HandleResponseMessage(request, message, OnFindDataObjectsResponse, HandleFindDataObjectsResponse);
        }

        /// <summary>
        /// Handles the Chunk message from a store.
        /// </summary>
        /// <param name="message">The Chunk message.</param>
        protected virtual void HandleChunk(EtpMessage<Chunk> message)
        {
            var request = TryGetCorrelatedMessage<FindDataObjects>(message);
            HandleResponseMessage(request, message, OnFindDataObjectsResponse, HandleFindDataObjectsResponse);
        }

        /// <summary>
        /// Handles the FindDataObjectsResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="DualResponseEventArgs{FindDataObjects, FindDataObjectsResponse, Chunk}"/> instance containing the event data.</param>
        protected virtual void HandleFindDataObjectsResponse(DualResponseEventArgs<FindDataObjects, FindDataObjectsResponse, Chunk> args)
        {
        }
    }
}

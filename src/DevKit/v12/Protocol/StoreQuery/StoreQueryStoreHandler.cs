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
using System.Linq;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v12.Datatypes;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.StoreQuery
{
    /// <summary>
    /// Base implementation of the <see cref="IStoreQueryStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.StoreQuery.IStoreQueryStore" />
    public class StoreQueryStoreHandler : Etp12ProtocolHandler, IStoreQueryStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreQueryStoreHandler"/> class.
        /// </summary>
        public StoreQueryStoreHandler() : base((int)Protocols.StoreQuery, "store", "customer")
        {
            MaxResponseCount = EtpSettings.DefaultMaxResponseCount;

            RegisterMessageHandler<FindDataObjects>(Protocols.StoreQuery, MessageTypes.StoreQuery.FindDataObjects, HandleFindDataObjects);
        }

        /// <summary>
        /// Gets the maximum response count.
        /// </summary>
        public long MaxResponseCount { get; set; }

        /// <summary>
        /// Gets the capabilities supported by the protocol handler.
        /// </summary>
        /// <param name="capabilities">The protocol's capabilities.</param>
        public override void GetCapabilities(EtpProtocolCapabilities capabilities)
        {
            base.GetCapabilities(capabilities);

            capabilities.MaxResponseCount = MaxResponseCount;
        }

        /// <summary>
        /// Handles the FindDataObjects event from a customer.
        /// </summary>
        public event ProtocolEventHandler<FindDataObjects, DataObjectResponse> OnFindDataObjects;

        /// <summary>
        /// Sends a FindDataObjectsResponse message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="objects">The list of <see cref="DataObject" /> objects.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long FindDataObjectsResponse(IMessageHeader request, IList<DataObject> objects, string sortOrder)
        {
            var header = CreateMessageHeader(Protocols.StoreQuery, MessageTypes.StoreQuery.FindDataObjectsResponse, request.MessageId);
            var response = new FindDataObjectsResponse
            {
                ServerSortOrder = sortOrder ?? string.Empty
            };

            return SendMultipartResponse(header, response, objects, (m, i) => m.DataObjects = i);
        }

        /// <summary>
        /// Sends a Chunk message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="blobId">The blob ID.</param>
        /// <param name="data">The chunk data.</param>
        /// <param name="messageFlags">The message flags.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long Chunk(IMessageHeader request, Guid blobId, byte[] data, MessageFlags messageFlags = MessageFlags.MultiPartAndFinalPart)
        {
            var header = CreateMessageHeader(Protocols.Store, MessageTypes.StoreQuery.Chunk, request.MessageId, messageFlags);

            var message = new Chunk
            {
                BlobId = blobId.ToUuid(),
                Data = data,
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Handles the FindDataObjects message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The FindDataObjects message.</param>
        protected virtual void HandleFindDataObjects(IMessageHeader header, FindDataObjects message)
        {
            var args = Notify(OnFindDataObjects, header, message, new DataObjectResponse());
            if (args.Cancel)
                return;

            if (!HandleFindDataObjects(header, message, args.Context))
                return;

            FindDataObjectsResponse(header, args.Context.DataObjects, args.Context.ServerSortOrder);
        }

        /// <summary>
        /// Handles the FindDataObjects message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="response">The response.</param>
        protected virtual bool HandleFindDataObjects(IMessageHeader header, FindDataObjects message, DataObjectResponse response)
        {
            return true;
        }
    }
}

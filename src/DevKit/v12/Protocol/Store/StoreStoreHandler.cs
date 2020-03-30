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
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v12.Datatypes;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.Store
{
    /// <summary>
    /// Base implementation of the <see cref="IStoreStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.Store.IStoreStore" />
    public class StoreStoreHandler : Etp12ProtocolHandler, IStoreStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreStoreHandler"/> class.
        /// </summary>
        public StoreStoreHandler() : base((int)Protocols.Store, "store", "customer")
        {
            RegisterMessageHandler<GetDataObjects>(Protocols.Store, MessageTypes.Store.GetDataObjects, HandleGetDataObjects);
            RegisterMessageHandler<PutDataObjects>(Protocols.Store, MessageTypes.Store.PutDataObjects, HandlePutDataObjects);
            RegisterMessageHandler<DeleteDataObjects>(Protocols.Store, MessageTypes.Store.DeleteDataObjects, HandleDeleteDataObjects);
            RegisterMessageHandler<Chunk>(Protocols.Store, MessageTypes.Store.Chunk, HandleChunk);
        }

        /// <summary>
        /// Sends an GetDataObjectsResponse message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="dataObjects">The data objects.</param>
        /// <param name="errors">The errors.</param>
        /// <returns>The message identifier.</returns>
        public virtual long GetDataObjectsResponse(IMessageHeader request, IDictionary<string, DataObject> dataObjects, IDictionary<string, ErrorInfo> errors)
        {
            var header = CreateMessageHeader(Protocols.Store, MessageTypes.Store.GetDataObjectsResponse, request.MessageId);

            var response = new GetDataObjectsResponse
            {
            };

            return Session.Send12MultipartResponse(header, response, dataObjects, errors, (m, i) => m.DataObjects = i);
        }

        /// <summary>
        /// Handles the GetDataObjects event from a customer.
        /// </summary>
        public event ProtocolEventHandler<GetDataObjects, DataObject, ErrorInfo> OnGetDataObjects;

        /// <summary>
        /// Handles the PutDataObjects event from a customer.
        /// </summary>
        public event ProtocolEventHandler<PutDataObjects> OnPutDataObjects;

        /// <summary>
        /// Handles the DeleteDataObjects event from a customer.
        /// </summary>
        public event ProtocolEventHandler<DeleteDataObjects> OnDeleteDataObjects;

        /// <summary>
        /// Sends a Chunk message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="blobId">The blob ID.</param>
        /// <param name="data">The chunk data.</param>
        /// <param name="messageFlags">The message flags.</param>
        /// <returns>The message identifier.</returns>
        public virtual long Chunk(IMessageHeader request, Guid blobId, byte[] data, MessageFlags messageFlags = MessageFlags.MultiPartAndFinalPart)
        {
            var header = CreateMessageHeader(Protocols.Store, MessageTypes.Store.Chunk, request.MessageId, messageFlags);

            var message = new Chunk
            {
                BlobId = blobId.ToUuid(),
                Data = data,
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Handles the Chunk event from a store.
        /// </summary>
        public event ProtocolEventHandler<Chunk> OnChunk;

        /// <summary>
        /// Handles the GetDataObjects message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GetDataObjects message.</param>
        protected virtual void HandleGetDataObjects(IMessageHeader header, GetDataObjects message)
        {
            var args = Notify(OnGetDataObjects, header, message, new Dictionary<string, DataObject>(), new Dictionary<string, ErrorInfo>());
            if (args.Cancel)
                return;

            if (!HandleGetDataObjects(header, message, args.Context, args.Errors))
                return;

            GetDataObjectsResponse(header, args.Context, args.Errors);
        }

        /// <summary>
        /// Handles the GetDataObjects message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="response">The response.</param>
        /// <param name="errors">The errors.</param>
        protected virtual bool HandleGetDataObjects(IMessageHeader header, GetDataObjects message, IDictionary<string, DataObject> response, IDictionary<string, ErrorInfo> errors)
        {
            return true;
        }

        /// <summary>
        /// Handles the PutDataObjects message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="putDataObjects">The PutDataObjects message.</param>
        protected virtual void HandlePutDataObjects(IMessageHeader header, PutDataObjects putDataObjects)
        {
            Notify(OnPutDataObjects, header, putDataObjects);
        }

        /// <summary>
        /// Handles the DeleteDataObjects message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="deleteDataObjects">The DeleteDataObjects message.</param>
        protected virtual void HandleDeleteDataObjects(IMessageHeader header, DeleteDataObjects deleteDataObjects)
        {
            Notify(OnDeleteDataObjects, header, deleteDataObjects);
        }

        /// <summary>
        /// Handles the DeleteDataObjects message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="chunk">The Chunk message.</param>
        protected virtual void HandleChunk(IMessageHeader header, Chunk chunk)
        {
            Notify(OnChunk, header, chunk);
        }
    }
}

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
using Energistics.Etp.Common.Protocol.Core;
using Energistics.Etp.v12.Datatypes;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.Store
{
    /// <summary>
    /// Base implementation of the <see cref="IStoreCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.Store.IStoreCustomer" />
    public class StoreCustomerHandler : Etp12ProtocolHandler<CapabilitiesCustomer, ICapabilitiesCustomer, CapabilitiesStore, ICapabilitiesStore>, IStoreCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreCustomerHandler"/> class.
        /// </summary>
        public StoreCustomerHandler() : base((int)Protocols.Store, Roles.Customer, Roles.Store)
        {
            RegisterMessageHandler<GetDataObjectsResponse>(Protocols.Store, MessageTypes.Store.GetDataObjectsResponse, HandleGetDataObjectsResponse);
            RegisterMessageHandler<Chunk>(Protocols.Store, MessageTypes.Store.Chunk, HandleChunk);
            RegisterMessageHandler<PutDataObjectsResponse>(Protocols.Store, MessageTypes.Store.PutDataObjectsResponse, HandlePutDataObjectsResponse);
            RegisterMessageHandler<DeleteDataObjectsResponse>(Protocols.Store, MessageTypes.Store.DeleteDataObjectsResponse, HandleDeleteDataObjectsResponse);
        }

        /// <summary>
        /// Sends a GetDataObjects message to a store.
        /// </summary>
        /// <param name="uris">The URI.</param>
        /// <param name="format">The format of the response (XML or JSON).</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetDataObjects> GetDataObjects(IDictionary<string, string> uris, string format = Formats.Xml, IMessageHeaderExtension extension = null)
        {
            var body = new GetDataObjects
            {
                Uris = uris ?? new Dictionary<string, string>(),
                Format = format ?? Formats.Xml,
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Sends a GetDataObjects message to a store.
        /// </summary>
        /// <param name="uris">The URI.</param>
        /// <param name="format">The format of the response (XML or JSON).</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetDataObjects> GetDataObjects(IList<string> uris, string format = Formats.Xml, IMessageHeaderExtension extension = null) => GetDataObjects(uris.ToMap(), format: format, extension: extension);

        /// <summary>
        /// Handles the GetDataObjectsResponse event from a store.
        /// </summary>
        public event EventHandler<DualResponseEventArgs<GetDataObjects, GetDataObjectsResponse, Chunk>> OnGetDataObjectsResponse;

        /// <summary>
        /// Sends a PutDataObjects message to a store.
        /// </summary>
        /// <param name="dataObjects">The data objects.</param>
        /// <param name="pruneContainedObjects">Whether or not to prune contained data objects.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutDataObjects> PutDataObjects(IDictionary<string, DataObject> dataObjects, bool pruneContainedObjects = false, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new PutDataObjects
            {
                DataObjects = dataObjects ?? new Dictionary<string, DataObject>(),
                PruneContainedObjects = pruneContainedObjects,
            };

            return SendRequest(body, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a PutDataObjects message to a store.
        /// </summary>
        /// <param name="dataObjects">The data objects.</param>
        /// <param name="pruneContainedObjects">Whether or not to prune contained data objects.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutDataObjects> PutDataObjects(IList<DataObject> dataObjects, bool pruneContainedObjects = false, bool isFinalPart = true, IMessageHeaderExtension extension = null) => PutDataObjects(dataObjects, pruneContainedObjects: pruneContainedObjects, isFinalPart: isFinalPart, extension: extension);

        /// <summary>
        /// Sends a Chunk message to a store as part of a multi-part PutDataObjects message.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="blobId">The blob ID.</param>
        /// <param name="data">The chunk data.</param>
        /// <param name="final">Whether or not this is the final chunk for the blob ID.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<Chunk> PutDataObjectsChunk(IMessageHeader correlatedHeader, Guid blobId, byte[] data, bool final, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new Chunk
            {
                BlobId = blobId.ToUuid<Uuid>(),
                Data = data,
                Final = final,
            };

            return SendRequest(body, extension: extension, isMultiPart: true, correlatedHeader: correlatedHeader, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a PutDataObjects message to a store.
        /// </summary>
        /// <param name="dataObjects">The data objects.</param>
        /// <param name="pruneContainedObjects">Whether or not to prune contained data objects.</param>
        /// <param name="chunks">The chunks.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="requestExtension">The request message header extension.</param>
        /// <param name="chunkExtensions">The message header extensions for the Chunk messages.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutDataObjects> PutDataObjects(IDictionary<string, DataObject> dataObjects, bool pruneContainedObjects = false, IList<Chunk> chunks = null, bool setFinalPart = true, IMessageHeaderExtension requestExtension = null, IList<IMessageHeaderExtension> chunkExtensions = null)
        {
            var message = PutDataObjects(dataObjects, pruneContainedObjects: pruneContainedObjects, isFinalPart: ((chunks == null || chunks.Count == 0) && setFinalPart), extension: requestExtension);
            if (message == null)
                return null;

            if (chunks?.Count > 0)
            {
                for (int i = 0; i < chunks.Count; i++)
                {
                    var ret = PutDataObjectsChunk(message.Header, chunks[i].BlobIdGuid.UuidGuid, chunks[i].Data, chunks[i].Final, isFinalPart: (i == chunks.Count - 1 && setFinalPart), extension: i < chunkExtensions?.Count ? chunkExtensions[i] : null);
                    if (ret == null)
                        return null;
                }
            }

            return message;
        }

        /// <summary>
        /// Sends a PutDataObjects message to a store.
        /// </summary>
        /// <param name="dataObjects">The data objects.</param>
        /// <param name="pruneContainedObjects">Whether or not to prune contained data objects.</param>
        /// <param name="chunks">The chunks.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="requestExtension">The request message header extension.</param>
        /// <param name="chunkExtensions">The message header extensions for the Chunk messages.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutDataObjects> PutDataObjects(IList<DataObject> dataObjects, bool pruneContainedObjects = false, IList<Chunk> chunks = null, bool setFinalPart = true, IMessageHeaderExtension requestExtension = null, IList<IMessageHeaderExtension> chunkExtensions = null)
            => PutDataObjects(dataObjects.ToMap(), pruneContainedObjects: pruneContainedObjects, chunks: chunks, setFinalPart: setFinalPart, requestExtension: requestExtension, chunkExtensions: chunkExtensions);

        /// <summary>
        /// Handles the PutDataObjectsResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<PutDataObjects, PutDataObjectsResponse>> OnPutDataObjectsResponse;

        /// <summary>
        /// Event raised when there is an exception received in response to a Chunk message when sent as part of a multi-part PutDataObjects message.
        /// </summary>
        public event EventHandler<VoidResponseEventArgs<Chunk>> OnPutDataObjectsChunkException;

        /// <summary>
        /// Sends a DeleteDataObjects message to a store.
        /// </summary>
        /// <param name="uris">The URI.</param>
        /// <param name="pruneContainedObjects">Whether or not to prune contained data objects.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<DeleteDataObjects> DeleteDataObjects(IDictionary<string, string> uris, bool pruneContainedObjects = false, IMessageHeaderExtension extension = null)
        {
            var body = new DeleteDataObjects
            {
                Uris = uris ?? new Dictionary<string, string>(),
                PruneContainedObjects = pruneContainedObjects,
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Sends a DeleteDataObjects message to a store.
        /// </summary>
        /// <param name="uris">The URI.</param>
        /// <param name="pruneContainedObjects">Whether or not to prune contained data objects.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<DeleteDataObjects> DeleteDataObjects(IList<string> uris, bool pruneContainedObjects = false, IMessageHeaderExtension extension = null) => DeleteDataObjects(uris, pruneContainedObjects: pruneContainedObjects, extension: extension);

        /// <summary>
        /// Handles the DeleteDataObjectsResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<DeleteDataObjects, DeleteDataObjectsResponse>> OnDeleteDataObjectsResponse;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<GetDataObjects>)
                HandleResponseMessage(request as EtpMessage<GetDataObjects>, message, OnGetDataObjectsResponse, HandleGetDataObjectsResponse);
            else if (request is EtpMessage<PutDataObjects>)
                HandleResponseMessage(request as EtpMessage<PutDataObjects>, message, OnPutDataObjectsResponse, HandlePutDataObjectsResponse);
            else if (request is EtpMessage<Chunk>)
                HandleResponseMessage(request as EtpMessage<Chunk>, message, OnPutDataObjectsChunkException, HandlePutDataObjectsChunkException);
            else if (request is EtpMessage<DeleteDataObjects>)
                HandleResponseMessage(request as EtpMessage<DeleteDataObjects>, message, OnDeleteDataObjectsResponse, HandleDeleteDataObjectsResponse);
        }

        /// <summary>
        /// Handles the GetDataObjectsResponse message from a store.
        /// </summary>
        /// <param name="message">The GetDataObjectsResponse message.</param>
        protected virtual void HandleGetDataObjectsResponse(EtpMessage<GetDataObjectsResponse> message)
        {
            var request = TryGetCorrelatedMessage<GetDataObjects>(message);
            HandleResponseMessage(request, message, OnGetDataObjectsResponse, HandleGetDataObjectsResponse);
        }

        /// <summary>
        /// Handles the Chunk message from a store.
        /// </summary>
        /// <param name="message">The Chunk message.</param>
        protected virtual void HandleChunk(EtpMessage<Chunk> message)
        {
            var request = TryGetCorrelatedMessage<GetDataObjects>(message);
            HandleResponseMessage(request, message, OnGetDataObjectsResponse, HandleGetDataObjectsResponse);
        }

        /// <summary>
        /// Handles the GetDataObjectsResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="DualResponseEventArgs{GetDataObjects, GetDataObjectsResponse, Chunk}"/> instance containing the event data.</param>
        protected virtual void HandleGetDataObjectsResponse(DualResponseEventArgs<GetDataObjects, GetDataObjectsResponse, Chunk> args)
        {
        }

        /// <summary>
        /// Handles the PutDataObjectsResponse message from a store.
        /// </summary>
        /// <param name="message">The PutDataObjectsResponse message.</param>
        protected virtual void HandlePutDataObjectsResponse(EtpMessage<PutDataObjectsResponse> message)
        {
            var request = TryGetCorrelatedMessage<PutDataObjects>(message);
            HandleResponseMessage(request, message, OnPutDataObjectsResponse, HandlePutDataObjectsResponse);
        }

        /// <summary>
        /// Handles the PutDataObjectsResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{PutDataObjects, PutDataObjectsResponse}"/> instance containing the event data.</param>
        protected virtual void HandlePutDataObjectsResponse(ResponseEventArgs<PutDataObjects, PutDataObjectsResponse> args)
        {
        }

        /// <summary>
        /// Handles exceptions to the Chunk message from a store.
        /// </summary>
        /// <param name="args">The <see cref="VoidResponseEventArgs{Chunk}"/> instance containing the event data.</param>
        protected virtual void HandlePutDataObjectsChunkException(VoidResponseEventArgs<Chunk> args)
        {
        }

        /// <summary>
        /// Handles the DeleteDataObjectsResponse message from a store.
        /// </summary>
        /// <param name="message">The DeleteDataObjectsResponse message.</param>
        protected virtual void HandleDeleteDataObjectsResponse(EtpMessage<DeleteDataObjectsResponse> message)
        {
            var request = TryGetCorrelatedMessage<DeleteDataObjects>(message);
            HandleResponseMessage(request, message, OnDeleteDataObjectsResponse, HandleDeleteDataObjectsResponse);
        }

        /// <summary>
        /// Handles the DeleteDataObjectsResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{DeleteDataObjects, DeleteDataObjectsResponse}"/> instance containing the event data.</param>
        protected virtual void HandleDeleteDataObjectsResponse(ResponseEventArgs<DeleteDataObjects, DeleteDataObjectsResponse> args)
        {
        }
    }
}

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
    public class StoreStoreHandler : Etp12ProtocolHandler<CapabilitiesStore, ICapabilitiesStore, CapabilitiesCustomer, ICapabilitiesCustomer>, IStoreStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreStoreHandler"/> class.
        /// </summary>
        public StoreStoreHandler() : base((int)Protocols.Store, Roles.Store, Roles.Customer)
        {
            RegisterMessageHandler<GetDataObjects>(Protocols.Store, MessageTypes.Store.GetDataObjects, HandleGetDataObjects);
            RegisterMessageHandler<PutDataObjects>(Protocols.Store, MessageTypes.Store.PutDataObjects, HandlePutDataObjects);
            RegisterMessageHandler<Chunk>(Protocols.Store, MessageTypes.Store.Chunk, HandleChunk);
            RegisterMessageHandler<DeleteDataObjects>(Protocols.Store, MessageTypes.Store.DeleteDataObjects, HandleDeleteDataObjects);
        }

        /// <summary>
        /// Handles the GetDataObjects event from a customer.
        /// </summary>
        public event EventHandler<MapAndListRequestEventArgs<GetDataObjects, DataObject, Chunk>> OnGetDataObjects;

        /// <summary>
        /// Sends a GetDataObjectsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="dataObjects">The data objects.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetDataObjectsResponse> GetDataObjectsResponse(IMessageHeader correlatedHeader, IDictionary<string, DataObject> dataObjects, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new GetDataObjectsResponse
            {
                DataObjects = dataObjects ?? new Dictionary<string, DataObject>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a Chunk message to a customer as part of a multi-part GetDataObjectsResponse message.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="blobId">The blob ID.</param>
        /// <param name="data">The chunk data.</param>
        /// <param name="final">Whether or not this is the final chunk for the blob ID.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<Chunk> GetDataObjectsResponseChunk(IMessageHeader correlatedHeader, Guid blobId, byte[] data, bool final, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new Chunk
            {
                BlobId = blobId,
                Data = data ?? new byte[0],
                Final = final,
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a multi-part set of GetDataObjectsResponse and Chunk messages to a customer.
        /// If there are no data objects, an empty GetDataObjectsResponse message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="dataObjects">The data objects.</param>
        /// <param name="chunks">The chunks.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the GetDataObjectsResponse message.</param>
        /// <param name="chunkExtensions">The message header extensions for the Chunk messages.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetDataObjectsResponse> GetDataObjectsResponse(IMessageHeader correlatedHeader, IDictionary<string, DataObject> dataObjects, IList<Chunk> chunks = null, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IList<IMessageHeaderExtension> chunkExtensions = null)
        {
            var message = GetDataObjectsResponse(correlatedHeader, dataObjects, isFinalPart: ((chunks == null || chunks.Count == 0) && setFinalPart), extension: responseExtension);
            if (message == null)
                return null;

            if (chunks?.Count > 0)
            {
                for (int i = 0; i < chunks.Count; i++)
                {
                    var ret = GetDataObjectsResponseChunk(correlatedHeader, chunks[i].BlobId, chunks[i].Data, chunks[i].Final, isFinalPart: (i == chunks.Count - 1 && setFinalPart), extension: i < chunkExtensions?.Count ? chunkExtensions[i] : null);
                    if (ret == null)
                        return null;
                }
            }

            return message;
        }

        /// <summary>
        /// Sends a complete multi-part set of GetDataObjectsResponse, Chunk and ProtocolException messages to a customer.
        /// If there are no data objects, an empty GetDataObjectsResponse message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="dataObjects">The data objects.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="chunks">The chunks.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the GetDataObjectsResponse message.</param>
        /// <param name="chunkExtensions">The message header extensions for the Chunk messages.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetDataObjectsResponse> GetDataObjectsResponse(IMessageHeader correlatedHeader, IDictionary<string, DataObject> dataObjects, IDictionary<string, IErrorInfo> errors, IList<Chunk> chunks = null, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IList<IMessageHeaderExtension> chunkExtensions = null, IMessageHeaderExtension exceptionExtension = null)
        {
            var message = GetDataObjectsResponse(correlatedHeader, dataObjects, chunks, setFinalPart: ((errors == null || errors.Count == 0) && setFinalPart), responseExtension: responseExtension, chunkExtensions: chunkExtensions);
            if (message == null)
                return null;

            if (errors?.Count > 0)
            {
                var ret = ProtocolException(errors, correlatedHeader: correlatedHeader, setFinalPart: setFinalPart, extension: exceptionExtension);
                if (ret == null)
                    return null;
            }

            return message;
        }

        /// <summary>
        /// Handles the PutDataObjects event from a customer.
        /// </summary>
        public event EventHandler<MapRequestEventArgs<PutDataObjects, PutResponse>> OnPutDataObjects;

        /// <summary>
        /// Handles the Chunk event from a customer as part of a PutDataObjects multi-part message.
        /// </summary>
        public event EventHandler<MapRequestWithDataEventArgs<PutDataObjects, Chunk, PutResponse>> OnPutDataObjectsChunk;

        /// <summary>
        /// Sends a PutDataObjectsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutDataObjectsResponse> PutDataObjectsResponse(IMessageHeader correlatedHeader, IDictionary<string, PutResponse> success, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new PutDataObjectsResponse
            {
                Success = success ?? new Dictionary<string, PutResponse>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a complete multi-part set of PutDataObjectsResponse and ProtocolException messages to a customer.
        /// If there are no successes, an empty PutDataObjectsResponse message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the PutDataObjectsResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutDataObjectsResponse> PutDataObjectsResponse(IMessageHeader correlatedHeader, IDictionary<string, PutResponse> success, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
        {
            return SendMapResponse(PutDataObjectsResponse, correlatedHeader, success, errors, setFinalPart: setFinalPart, responseExtension: responseExtension, exceptionExtension: exceptionExtension);
        }

        /// <summary>
        /// Handles the DeleteDataObjects event from a customer.
        /// </summary>
        public event EventHandler<MapRequestEventArgs<DeleteDataObjects, ArrayOfString>> OnDeleteDataObjects;

        /// <summary>
        /// Sends a DeleteDataObjectsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="deletedUris">The deleted URIs.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<DeleteDataObjectsResponse> DeleteDataObjectsResponse(IMessageHeader correlatedHeader, IDictionary<string, ArrayOfString> deletedUris, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new DeleteDataObjectsResponse
            {
                DeletedUris = deletedUris ?? new Dictionary<string, ArrayOfString>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a complete multi-part set of DeleteDataObjectsResponse and ProtocolException messages to a customer.
        /// If there are no deleted URIs, an empty DeleteDataObjectsResponse message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="deletedUris">The deleted URIs.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the DeleteDataObjectsResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<DeleteDataObjectsResponse> DeleteDataObjectsResponse(IMessageHeader correlatedHeader, IDictionary<string, ArrayOfString> deletedUris, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
        {
            return SendMapResponse(DeleteDataObjectsResponse, correlatedHeader, deletedUris, errors, setFinalPart: setFinalPart, responseExtension: responseExtension, exceptionExtension: exceptionExtension);
        }

        /// <summary>
        /// Handles the GetDataObjects message from a customer.
        /// </summary>
        /// <param name="message">The GetDataObjects message.</param>
        protected virtual void HandleGetDataObjects(EtpMessage<GetDataObjects> message)
        {
            HandleRequestMessage(message, OnGetDataObjects, HandleGetDataObjects,
                responseMethod: (args) => GetDataObjectsResponse(args.Request?.Header, args.Response1Map, args.Responses2, setFinalPart: !args.HasErrors, responseExtension: args.Response1MapExtension, chunkExtensions: args.Response2Extensions));
        }

        /// <summary>
        /// Handles the GetDataObjects message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="MapAndListRequestEventArgs{GetDataObjects, DataObject, Chunk}"/> instance containing the event data.</param>
        protected virtual void HandleGetDataObjects(MapAndListRequestEventArgs<GetDataObjects, DataObject, Chunk> args)
        {
        }

        /// <summary>
        /// Handles the PutDataObjects message from a customer.
        /// </summary>
        /// <param name="message">The PutDataObjects message.</param>
        protected virtual void HandlePutDataObjects(EtpMessage<PutDataObjects> message)
        {
            if (!message.Header.IsFinalPart())
                TryRegisterMessage(message);

            HandleRequestMessage(message, OnPutDataObjects, HandlePutDataObjects,
                responseMethod: (args) => PutDataObjectsResponse(args.Request?.Header, args.ResponseMap, isFinalPart: !args.HasErrors, extension: args.ResponseMapExtension));
        }

        /// <summary>
        /// Handles the PutDataObjects message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="MapRequestEventArgs{PutDataObjects, PutResponse}"/> instance containing the event data.</param>
        protected virtual void HandlePutDataObjects(MapRequestEventArgs<PutDataObjects, PutResponse> args)
        {
        }

        /// <summary>
        /// Handles the Chunk message from a customer.
        /// </summary>
        /// <param name="message">The Chunk message.</param>
        protected virtual void HandleChunk(EtpMessage<Chunk> message)
        {
            var request = TryGetCorrelatedMessage<PutDataObjects>(message);
            HandleRequestMessage(request, OnPutDataObjectsChunk, HandlePutDataObjectsChunk,
                args: new MapRequestWithDataEventArgs<PutDataObjects, Chunk, PutResponse>(request, message),
                responseMethod: (args) => PutDataObjectsResponse(args.Request?.Header, args.ResponseMap, isFinalPart: !args.HasErrors, extension: args.ResponseMapExtension));
        }

        /// <summary>
        /// Handles the Chunk message from a customer when sent as part of a PutDataObjects message.
        /// </summary>
        /// <param name="args">The <see cref="MapRequestWithDataEventArgs{PutDataObjects, Chunk, PutResponse}"/> instance containing the event data.</param>
        protected virtual void HandlePutDataObjectsChunk(MapRequestWithDataEventArgs<PutDataObjects, Chunk, PutResponse> args)
        {
        }

        /// <summary>
        /// Handles the DeleteDataObjects message from a customer.
        /// </summary>
        /// <param name="message">The DeleteDataObjects message.</param>
        protected virtual void HandleDeleteDataObjects(EtpMessage<DeleteDataObjects> message)
        {
            HandleRequestMessage(message, OnDeleteDataObjects, HandleDeleteDataObjects,
                responseMethod: (args) => DeleteDataObjectsResponse(args.Request?.Header, args.ResponseMap, isFinalPart: !args.HasErrors, extension: args.ResponseMapExtension));
        }

        /// <summary>
        /// Handles the DeleteDataObjects message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="MapRequestEventArgs{DeleteDataObjects, ArrayOfString}"/> instance containing the event data.</param>
        protected virtual void HandleDeleteDataObjects(MapRequestEventArgs<DeleteDataObjects, ArrayOfString> args)
        {
        }
    }
}

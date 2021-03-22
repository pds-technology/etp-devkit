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

namespace Energistics.Etp.v12.Protocol.StoreQuery
{
    /// <summary>
    /// Base implementation of the <see cref="IStoreQueryStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.StoreQuery.IStoreQueryStore" />
    public class StoreQueryStoreHandler : Etp12ProtocolHandler<CapabilitiesStore, ICapabilitiesStore, CapabilitiesCustomer, ICapabilitiesCustomer>, IStoreQueryStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreQueryStoreHandler"/> class.
        /// </summary>
        public StoreQueryStoreHandler() : base((int)Protocols.StoreQuery, Roles.Store, Roles.Customer)
        {
            RegisterMessageHandler<FindDataObjects>(Protocols.StoreQuery, MessageTypes.StoreQuery.FindDataObjects, HandleFindDataObjects);
        }

        /// <summary>
        /// Handles the FindDataObjects event from a customer.
        /// </summary>
        public event EventHandler<DualListRequestWithContextEventArgs<FindDataObjects, DataObject, Chunk, ResponseContext>> OnFindDataObjects;

        /// <summary>
        /// Sends a FindDataObjectsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="dataObjects">The list of <see cref="DataObject"/> objects.</param>
        /// <param name="serverSortOrder">The server sort order.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<FindDataObjectsResponse> FindDataObjectsResponse(IMessageHeader correlatedHeader, IList<DataObject> dataObjects, string serverSortOrder, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new FindDataObjectsResponse
            {
                DataObjects = dataObjects ?? new List<DataObject>(),
                ServerSortOrder = serverSortOrder,
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a Chunk message to a customer as part of a multi-part FindDataObjectsResponse message.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="blobId">The blob ID.</param>
        /// <param name="data">The chunk data.</param>
        /// <param name="final">Whether or not this is the final chunk for the blob ID.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<Chunk> FindDataObjectsResponseChunk(IMessageHeader correlatedHeader, Guid blobId, byte[] data, bool final, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new Chunk
            {
                BlobId = blobId.ToUuid<Uuid>(),
                Data = data,
                Final = final,
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a complete multi-part set of FindDataObjectsResponse and Chunk messages to a customer.
        /// If there are no data objects, an empty FindDataObjectsResponse message is sent.
        /// If there are no chunks, no Chunk message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="dataObjects">The data objects.</param>
        /// <param name="serverSortOrder">The server sort order.</param>
        /// <param name="chunks">The chunks.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the GetDataObjectsResponse message.</param>
        /// <param name="chunkExtensions">The message header extensions for the Chunk messages.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<FindDataObjectsResponse> FindDataObjectsResponse(IMessageHeader correlatedHeader, IList<DataObject> dataObjects, string serverSortOrder, IList<Chunk> chunks = null, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IList<IMessageHeaderExtension> chunkExtensions = null)
        {
            var message = FindDataObjectsResponse(correlatedHeader, dataObjects, serverSortOrder, isFinalPart: ((chunks == null || chunks.Count == 0) && setFinalPart), extension: responseExtension);
            if (message == null)
                return null;

            if (chunks?.Count > 0)
            {
                for (int i = 0; i < chunks.Count; i++)
                {
                    var ret = FindDataObjectsResponseChunk(correlatedHeader, chunks[i].BlobIdGuid.UuidGuid, chunks[i].Data, chunks[i].Final, isFinalPart: (i == chunks.Count - 1 && setFinalPart), extension: i < chunkExtensions?.Count ? chunkExtensions[i] : null);
                    if (ret == null)
                        return null;
                }
            }

            return message;
        }

        /// <summary>
        /// Handles the FindDataObjects message from a customer.
        /// </summary>
        /// <param name="message">The FindDataObjects message.</param>
        protected virtual void HandleFindDataObjects(EtpMessage<FindDataObjects> message)
        {
            HandleRequestMessage(message, OnFindDataObjects, HandleFindDataObjects,
                responseMethod: (args) => FindDataObjectsResponse(args.Request?.Header, args.Responses1, args.Context?.ServerSortOrder, args.Responses2, setFinalPart: !args.HasErrors, responseExtension: args.Response1Extension, chunkExtensions: args.Response2Extensions));
        }

        /// <summary>
        /// Handles the FindDataObjects message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="DualListRequestWithContextEventArgs{FindDataObjects, DataObject, Chunk, ResponseContext}"/> instance containing the event data.</param>
        protected virtual void HandleFindDataObjects(DualListRequestWithContextEventArgs<FindDataObjects, DataObject, Chunk, ResponseContext> args)
        {
        }
    }
}

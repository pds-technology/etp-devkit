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
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.StoreQuery
{
    /// <summary>
    /// Describes the interface that must be implemented by the store role of the StoreQuery protocol.
    /// </summary>
    /// <seealso cref="IProtocolHandler" />
    [ProtocolRole((int)Protocols.StoreQuery, Roles.Store, Roles.Customer)]
    public interface IStoreQueryStore : IProtocolHandler<ICapabilitiesStore, ICapabilitiesCustomer>
    {
        /// <summary>
        /// Handles the FindDataObjects event from a customer.
        /// </summary>
        event EventHandler<DualListRequestWithContextEventArgs<FindDataObjects, DataObject, Chunk, ResponseContext>> OnFindDataObjects;

        /// <summary>
        /// Sends a FindDataObjectsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="dataObjects">The list of <see cref="DataObject"/> objects.</param>
        /// <param name="serverSortOrder">The server sort order.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<FindDataObjectsResponse> FindDataObjectsResponse(IMessageHeader correlatedHeader, IList<DataObject> dataObjects, string serverSortOrder, bool isFinalPart = true, IMessageHeaderExtension extension = null);

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
        EtpMessage<Chunk> FindDataObjectsResponseChunk(IMessageHeader correlatedHeader, Guid blobId, byte[] data, bool final, bool isFinalPart = true, IMessageHeaderExtension extension = null);

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
        EtpMessage<FindDataObjectsResponse> FindDataObjectsResponse(IMessageHeader correlatedHeader, IList<DataObject> dataObjects, string serverSortOrder, IList<Chunk> chunks = null, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IList<IMessageHeaderExtension> chunkExtensions = null);
    }

    /// <summary>
    /// Encapsulates the context of a store query response.
    /// </summary>
    public class ResponseContext
    {
        /// <summary>
        /// Gets or sets the server sort order.
        /// </summary>
        public string ServerSortOrder { get; set; }
    }
}

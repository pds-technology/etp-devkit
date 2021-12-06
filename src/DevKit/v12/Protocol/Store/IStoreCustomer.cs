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

namespace Energistics.Etp.v12.Protocol.Store
{
    /// <summary>
    /// Defines the interface that must be implemented by the customer role of the Store protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.Store, Roles.Customer, Roles.Store)]
    public interface IStoreCustomer : IProtocolHandler<ICapabilitiesCustomer, ICapabilitiesStore>
    {
        /// <summary>
        /// Sends a GetDataObjects message to a store.
        /// </summary>
        /// <param name="uris">The URI.</param>
        /// <param name="format">The format of the response (XML or JSON).</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetDataObjects> GetDataObjects(IDictionary<string, string> uris, string format = Formats.Xml, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a GetDataObjects message to a store.
        /// </summary>
        /// <param name="uris">The URI.</param>
        /// <param name="format">The format of the response (XML or JSON).</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetDataObjects> GetDataObjects(IList<string> uris, string format = Formats.Xml, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the GetDataObjectsResponse event from a store.
        /// </summary>
        event EventHandler<DualResponseEventArgs<GetDataObjects, GetDataObjectsResponse, Chunk>> OnGetDataObjectsResponse;

        /// <summary>
        /// Sends a PutDataObjects message to a store.
        /// </summary>
        /// <param name="dataObjects">The data objects.</param>
        /// <param name="pruneContainedObjects">Whether or not to prune contained data objects.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<PutDataObjects> PutDataObjects(IDictionary<string, DataObject> dataObjects, bool pruneContainedObjects = false, bool isFinalPart = true, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a PutDataObjects message to a store.
        /// </summary>
        /// <param name="dataObjects">The data objects.</param>
        /// <param name="pruneContainedObjects">Whether or not to prune contained data objects.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<PutDataObjects> PutDataObjects(IList<DataObject> dataObjects, bool pruneContainedObjects = false, bool isFinalPart = true, IMessageHeaderExtension extension = null);

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
        EtpMessage<Chunk> PutDataObjectsChunk(IMessageHeader correlatedHeader, Guid blobId, byte[] data, bool final, bool isFinalPart = true, IMessageHeaderExtension extension = null);

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
        EtpMessage<PutDataObjects> PutDataObjects(IDictionary<string, DataObject> dataObjects, bool pruneContainedObjects = false, IList<Chunk> chunks = null, bool setFinalPart = true, IMessageHeaderExtension requestExtension = null, IList<IMessageHeaderExtension> chunkExtensions = null);

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
        EtpMessage<PutDataObjects> PutDataObjects(IList<DataObject> dataObjects, bool pruneContainedObjects = false, IList<Chunk> chunks = null, bool setFinalPart = true, IMessageHeaderExtension requestExtension = null, IList<IMessageHeaderExtension> chunkExtensions = null);

        /// <summary>
        /// Handles the PutDataObjectsResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<PutDataObjects, PutDataObjectsResponse>> OnPutDataObjectsResponse;

        /// <summary>
        /// Event raised when there is an exception received in response to a Chunk message when sent as part of a multi-part PutDataObjects message.
        /// </summary>
        event EventHandler<VoidResponseEventArgs<Chunk>> OnPutDataObjectsChunkException;

        /// <summary>
        /// Sends a DeleteDataObjects message to a store.
        /// </summary>
        /// <param name="uris">The URI.</param>
        /// <param name="pruneContainedObjects">Whether or not to prune contained data objects.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<DeleteDataObjects> DeleteDataObjects(IDictionary<string, string> uris, bool pruneContainedObjects = false, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a DeleteDataObjects message to a store.
        /// </summary>
        /// <param name="uris">The URI.</param>
        /// <param name="pruneContainedObjects">Whether or not to prune contained data objects.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<DeleteDataObjects> DeleteDataObjects(IList<string> uris, bool pruneContainedObjects = false, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the DeleteDataObjectsResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<DeleteDataObjects, DeleteDataObjectsResponse>> OnDeleteDataObjectsResponse;
    }
}

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
    /// Defines the interface that must be implemented by the store role of the store protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.Store, Roles.Store, Roles.Customer)]
    public interface IStoreStore : IProtocolHandler<ICapabilitiesStore, ICapabilitiesCustomer>
    {
        /// <summary>
        /// Handles the GetDataObjects event from a customer.
        /// </summary>
        event EventHandler<MapAndListRequestEventArgs<GetDataObjects, DataObject, Chunk>> OnGetDataObjects;

        /// <summary>
        /// Sends a GetDataObjectsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="dataObjects">The data objects.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetDataObjectsResponse> GetDataObjectsResponse(IMessageHeader correlatedHeader, IDictionary<string, DataObject> dataObjects, bool isFinalPart = true, IMessageHeaderExtension extension = null);

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
        EtpMessage<Chunk> GetDataObjectsResponseChunk(IMessageHeader correlatedHeader, Guid blobId, byte[] data, bool final, bool isFinalPart = true, IMessageHeaderExtension extension = null);

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
        EtpMessage<GetDataObjectsResponse> GetDataObjectsResponse(IMessageHeader correlatedHeader, IDictionary<string, DataObject> dataObjects, IList<Chunk> chunks = null, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IList<IMessageHeaderExtension> chunkExtensions = null);

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
        EtpMessage<GetDataObjectsResponse> GetDataObjectsResponse(IMessageHeader correlatedHeader, IDictionary<string, DataObject> dataObjects, IDictionary<string, IErrorInfo> errors, IList<Chunk> chunks = null, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IList<IMessageHeaderExtension> chunkExtensions = null, IMessageHeaderExtension exceptionExtension = null);

        /// <summary>
        /// Handles the PutDataObjects event from a customer.
        /// </summary>
        event EventHandler<MapRequestEventArgs<PutDataObjects, PutResponse>> OnPutDataObjects;

        /// <summary>
        /// Handles the Chunk event from a customer as part of a PutDataObjects multi-part message.
        /// </summary>
        event EventHandler<MapRequestWithDataEventArgs<PutDataObjects, Chunk, PutResponse>> OnPutDataObjectsChunk;

        /// <summary>
        /// Sends a PutDataObjectsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<PutDataObjectsResponse> PutDataObjectsResponse(IMessageHeader correlatedHeader, IDictionary<string, PutResponse> success, bool isFinalPart = true, IMessageHeaderExtension extension = null);

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
        EtpMessage<PutDataObjectsResponse> PutDataObjectsResponse(IMessageHeader correlatedHeader, IDictionary<string, PutResponse> success, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null);

        /// <summary>
        /// Handles the DeleteDataObjects event from a customer.
        /// </summary>
        event EventHandler<MapRequestEventArgs<DeleteDataObjects, ArrayOfString>> OnDeleteDataObjects;

        /// <summary>
        /// Sends a DeleteDataObjectsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="deletedUris">The deleted URIs.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<DeleteDataObjectsResponse> DeleteDataObjectsResponse(IMessageHeader correlatedHeader, IDictionary<string, ArrayOfString> deletedUris, bool isFinalPart = true, IMessageHeaderExtension extension = null);

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
        EtpMessage<DeleteDataObjectsResponse> DeleteDataObjectsResponse(IMessageHeader correlatedHeader, IDictionary<string, ArrayOfString> deletedUris, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null);
    }
}

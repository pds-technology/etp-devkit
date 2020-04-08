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
    [ProtocolRole((int)Protocols.Store, "customer", "store")]
    public interface IStoreCustomer : IProtocolHandler
    {
        /// <summary>
        /// Sends a GetDataObjects message to a store.
        /// </summary>
        /// <param name="uris">The URI.</param>
        /// <param name="format">The format of the response (XML or JSON).</param>
        /// <returns>The message identifier.</returns>
        long GetDataObjects(IList<string> uris, string format = "xml");

        /// <summary>
        /// Handles the GetDataObjectsResponse event from a store.
        /// </summary>
        event ProtocolEventHandler<GetDataObjectsResponse> OnGetDataObjectsResponse;

        /// <summary>
        /// Sends a PutDataObjects message to a store.
        /// </summary>
        /// <param name="dataObjects">The data objects.</param>
        /// <returns>The message identifier.</returns>
        long PutDataObjects(IList<DataObject> dataObjects);

        /// <summary>
        /// Sends a DeleteDataObjects message to a store.
        /// </summary>
        /// <param name="uris">The URI.</param>
        /// <returns>The message identifier.</returns>
        long DeleteDataObjects(IList<string> uris);

        /// <summary>
        /// Sends a Chunk message to a store.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="blobId">The blob ID.</param>
        /// <param name="data">The chunk data.</param>
        /// <param name="messageFlags">The message flags.</param>
        /// <returns>The message identifier.</returns>
        long Chunk(IMessageHeader request, Guid blobId, byte[] data, MessageFlags messageFlags = MessageFlags.MultiPartAndFinalPart);

        /// <summary>
        /// Handles the Chunk event from a store.
        /// </summary>
        event ProtocolEventHandler<Chunk> OnChunk;
    }
}

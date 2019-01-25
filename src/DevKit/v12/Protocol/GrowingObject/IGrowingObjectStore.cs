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

using System.Collections.Generic;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v12.Datatypes;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.GrowingObject
{
    /// <summary>
    /// Defines the interface that must be implemented by the store role of the growing object protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.GrowingObject, "store", "customer")]
    public interface IGrowingObjectStore : IProtocolHandler
    {
        /// <summary>
        /// Sends a single list item as a response for GetPart and GetPartsByRange.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="uid">The ID of the element within the list.</param>
        /// <param name="contentType">The content type string.</param>
        /// <param name="data">The data.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        long GetPartsResponse(string uri, string uid, string contentType, byte[] data, long correlationId, MessageFlags messageFlag = MessageFlags.MultiPartAndFinalPart);

        /// <summary>
        /// Sends the metadata describing the list items in the requested growing objects.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="metadata">The parts metadata.</param>
        /// <param name="errors">The errors.</param>
        /// <returns>The message identifier.</returns>
        long GetPartsMetadataResponse(IMessageHeader request, IList<PartsMetadataInfo> metadata, IList<ErrorInfo> errors);

        /// <summary>
        /// Handles the GetPart event from a customer.
        /// </summary>
        event ProtocolEventHandler<GetPart> OnGetPart;

        /// <summary>
        /// Handles the GetPartsByRange event from a customer.
        /// </summary>
        event ProtocolEventHandler<GetPartsByRange> OnGetPartsByRange;

        /// <summary>
        /// Handles the PutPart event from a customer.
        /// </summary>
        event ProtocolEventHandler<PutPart> OnPutPart;

        /// <summary>
        /// Handles the DeletePart event from a customer.
        /// </summary>
        event ProtocolEventHandler<DeletePart> OnDeletePart;

        /// <summary>
        /// Handles the DeletePartsByRange event from a customer.
        /// </summary>
        event ProtocolEventHandler<DeletePartsByRange> OnDeletePartsByRange;

        /// <summary>
        /// Handles the ReplacePartsByRange event from a customer.
        /// </summary>
        event ProtocolEventHandler<ReplacePartsByRange> OnReplacePartsByRange;

        /// <summary>
        /// Handles the GetPartsMetadata event from a customer.
        /// </summary>
        event ProtocolEventHandler<GetPartsMetadata> OnGetPartsMetadata;
    }
}

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
        /// Handles the GetParts event from a customer.
        /// </summary>
        event ProtocolEventWithErrorsHandler<GetParts, ObjectPart, ErrorInfo> OnGetParts;

        /// <summary>
        /// Sends a a list of parts as a response for GetParts to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="parts">The UIDs and data of the parts being returned.</param>
        /// <param name="errors">The errors, if any.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long GetPartsResponse(IMessageHeader request, string uri, IDictionary<string, ObjectPart> parts, IDictionary<string, ErrorInfo> errors, string format = "xml");

        /// <summary>
        /// Handles the PutParts event from a customer.
        /// </summary>
        event ProtocolEventWithErrorsHandler<PutParts, ErrorInfo> OnPutParts;

        /// <summary>
        /// Handles the DeleteParts event from a customer.
        /// </summary>
        event ProtocolEventWithErrorsHandler<DeleteParts, ErrorInfo> OnDeleteParts;

        /// <summary>
        /// Handles the GetPartsByRange event from a customer.
        /// </summary>
        event ProtocolEventHandler<GetPartsByRange, IList<ObjectPart>> OnGetPartsByRange;

        /// <summary>
        /// Sends a a list of parts as a response for GetPartsByRange to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="parts">The UIDs and data of the parts being returned.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long GetPartsByRangeResponse(IMessageHeader request, string uri, IList<ObjectPart> parts, string format = "xml");

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
        event ProtocolEventWithErrorsHandler<GetPartsMetadata, PartsMetadataInfo, ErrorInfo> OnGetPartsMetadata;

        /// <summary>
        /// Sends the metadata describing the parts in the requested growing objects to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="metadata">The parts metadata.</param>
        /// <param name="errors">The errors, if any.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long GetPartsMetadataResponse(IMessageHeader request, IDictionary<string, PartsMetadataInfo> metadata, IDictionary<string, ErrorInfo> errors);

    }
}

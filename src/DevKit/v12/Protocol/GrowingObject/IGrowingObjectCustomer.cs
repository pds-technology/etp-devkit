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
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.GrowingObject
{
    /// <summary>
    /// Defines the interface that must be implemented by the customer role of the growing object protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.GrowingObject, "customer", "store")]
    public interface IGrowingObjectCustomer : IProtocolHandler
    {
        /// <summary>
        /// Gets parts in a growing object by UID from a store.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="uids">The UIDs of the elements within the growing object to get.</param>
        /// <param name="format">The format of the response (XML or JSON).</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long GetParts(string uri, IList<string> uids, string format = "xml");

        /// <summary>
        /// Handles the GetPartsResponse event from a store.
        /// </summary>
        event ProtocolEventHandler<GetPartsResponse> OnGetPartsResponse;

        /// <summary>
        /// Adds or updates parts in a growing object in a store.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="parts">The UIDs and data of the parts being put.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long PutParts(string uri, IList<ObjectPart> parts, string format = "xml");

        /// <summary>
        /// Deletes parts from a growing object from a store.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="uids">The UIDs of the parts within the growing object to delete.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long DeleteParts(string uri, IList<string> uids);

        /// <summary>
        /// Gets all parts in a growing object within an index range from a store.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="indexInterval">The index interval.</param>
        /// <param name="includeOverlappingIntervals"><c>true</c> if overlapping intervals should be included; otherwise, <c>false</c>.</param>
        /// <param name="format">The format of the response (XML or JSON).</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long GetPartsByRange(string uri, IndexInterval indexInterval, bool includeOverlappingIntervals = false, string format = "xml");

        /// <summary>
        /// Handles the GetPartsByRangeResponse event from a store.
        /// </summary>
        event ProtocolEventHandler<GetPartsByRangeResponse> OnGetPartsByRangeResponse;

        /// <summary>
        /// Replaces all parts in a range of index values in a growing object with new parts in a store.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="deleteInterval">The index interval to delete.</param>
        /// <param name="includeOverlappingIntervals"><c>true</c> if overlapping intervals should be included; otherwise, <c>false</c>.</param>
        /// <param name="parts">The map of UIDs and data of the parts being put.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long ReplacePartsByRange(string uri, IndexInterval deleteInterval, bool includeOverlappingIntervals, IList<ObjectPart> parts, string format = "xml");

        /// <summary>
        /// Gets the metadata for growing object parts from a store.
        /// </summary>
        /// <param name="uris">The collection of growing object URIs.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long GetPartsMetadata(IList<string> uris);

        /// <summary>
        /// Handles the GetPartsMetadataResponse event from a store.
        /// </summary>
        event ProtocolEventHandler<GetPartsMetadataResponse> OnGetPartsMetadataResponse;
    }
}

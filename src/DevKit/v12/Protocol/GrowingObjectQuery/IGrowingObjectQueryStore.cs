//----------------------------------------------------------------------- 
// ETP DevKit, 1.2
//
// Copyright 2018 Energistics
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
using Energistics.Etp.v12.Protocol.GrowingObject;

namespace Energistics.Etp.v12.Protocol.GrowingObjectQuery
{
    /// <summary>
    /// Describes the interface that must be implemented by the store role of the GrowingObjectQuery protocol.
    /// </summary>
    /// <seealso cref="IProtocolHandler" />
    [ProtocolRole((int)Protocols.GrowingObjectQuery, "store", "customer")]
    public interface IGrowingObjectQueryStore : IProtocolHandler
    {
        /// <summary>
        /// Sends a FindPartsResponse message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="parts">The list of <see cref="ObjectPart"/> objects.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns>The message identifier.</returns>
        long FindPartsResponse(IMessageHeader request, IList<ObjectPart> parts, string sortOrder);

        /// <summary>
        /// Handles the FindParts event from a customer.
        /// </summary>
        event ProtocolEventHandler<FindParts, ObjectPartResponse> OnFindParts;
    }

    /// <summary>
    /// Encapsulates the results of a growing object query.
    /// </summary>
    public class ObjectPartResponse
    {
        /// <summary>
        /// Gets the collection of data objects.
        /// </summary>
        public IList<ObjectPart> ObjectParts { get; } = new List<ObjectPart>();

        /// <summary>
        /// Gets or sets the server sort order.
        /// </summary>
        public string ServerSortOrder { get; set; }
    }
}

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

namespace Energistics.Etp.v12.Protocol.DiscoveryQuery
{
    /// <summary>
    /// Describes the interface that must be implemented by the store role of the DiscoveryQuery protocol.
    /// </summary>
    /// <seealso cref="IProtocolHandler" />
    [ProtocolRole((int)Protocols.DiscoveryQuery, "store", "customer")]
    public interface IDiscoveryQueryStore : IProtocolHandler
    {
        /// <summary>
        /// Sends a FindResourcesResponse message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="resources">The list of <see cref="Resource"/> objects.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns>The message identifier.</returns>
        long FindResourcesResponse(IMessageHeader request, IList<Resource> resources, string sortOrder);

        /// <summary>
        /// Handles the FindResources event from a customer.
        /// </summary>
        event ProtocolEventHandler<FindResources, ResourceResponse> OnFindResources;
    }

    /// <summary>
    /// Encapsulates the results of a discovery query.
    /// </summary>
    public class ResourceResponse
    {
        /// <summary>
        /// Gets the collection of resources.
        /// </summary>
        public IList<Resource> Resources { get; } = new List<Resource>();

        /// <summary>
        /// Gets or sets the server sort order.
        /// </summary>
        public string ServerSortOrder { get; set; }
    }
}

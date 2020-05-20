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

namespace Energistics.Etp.v12.Protocol.Discovery
{
    /// <summary>
    /// Describes the interface that must be implemented by the store role of the Discovery protocol.
    /// </summary>
    /// <seealso cref="IProtocolHandler" />
    [ProtocolRole((int)Protocols.Discovery, "store", "customer")]
    public interface IDiscoveryStore : IProtocolHandler
    {
        /// <summary>
        /// Indicates to a customer the maximum number of response messages a store will return.
        /// </summary>
        long MaxResponseCount { get; set; }

        /// <summary>
        /// Handles the GetResources event from a customer.
        /// </summary>
        event ProtocolEventHandler<GetResources, IList<Resource>> OnGetResources;

        /// <summary>
        /// Sends a GetResourcesResponse message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="resources">The list of <see cref="Resource"/> objects.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long GetResourcesResponse(IMessageHeader request, IList<Resource> resources);
    }
}

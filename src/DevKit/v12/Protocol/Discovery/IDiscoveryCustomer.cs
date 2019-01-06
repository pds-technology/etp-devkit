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

using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.Discovery
{
    /// <summary>
    /// Describes the interface that must be implemented by the customer role of the Discovery protocol.
    /// </summary>
    /// <seealso cref="IProtocolHandler" />
    [ProtocolRole((int)Protocols.Discovery, "customer", "store")]
    public interface IDiscoveryCustomer : IProtocolHandler
    {
        /// <summary>
        /// Sends a GetTreeResources message to a store.
        /// </summary>
        /// <param name="contextInfo">The context information.</param>
        /// <returns>The message identifier.</returns>
        long GetTreeResources(ContextInfo contextInfo);

        /// <summary>
        /// Sends a GetGraphResources message to a store.
        /// </summary>
        /// <param name="contextInfo">The context information.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="groupByType">if set to <c>true</c> group by type.</param>
        /// <returns>The message identifier.</returns>
        long GetGraphResources(ContextInfo contextInfo, ContextScopeKind scope, bool groupByType);

        /// <summary>
        /// Handles the GetResourcesResponse event from a store.
        /// </summary>
        event ProtocolEventHandler<GetResourcesResponse, string> OnGetResourcesResponse;
    }
}

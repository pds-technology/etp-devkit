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

namespace Energistics.Etp.v12.Protocol.SupportedTypes
{
    /// <summary>
    /// Describes the interface that must be implemented by the customer role of the SupportedTypes protocol.
    /// </summary>
    /// <seealso cref="IProtocolHandler" />
    [ProtocolRole((int)Protocols.SupportedTypes, "customer", "store")]
    public interface ISupportedTypesCustomer : IProtocolHandler
    {
        /// <summary>
        /// Sends a GetSupportedTypes message to a store.
        /// </summary>
        /// <param name="uri">The uri to to discover instantiated or supported data types from.</param>
        /// <param name="scope">The scope to return supported types for.</param>
        /// <param name="returnEmptyTypes">Whether the store should return data types that it supports but for which it currently has no data.</param>
        /// <param name="countObjects">if set to <c>true</c>, request object counts.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long GetSupportedTypes(string uri, ContextScopeKind scope, bool returnEmptyTypes = false, bool countObjects = false);

        /// <summary>
        /// Handles the GetResourcesResponse event from a store.
        /// </summary>
        event ProtocolEventHandler<GetSupportedTypesResponse, GetSupportedTypes> OnGetSupportedTypesResponse;
    }
}

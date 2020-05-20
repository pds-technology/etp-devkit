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

namespace Energistics.Etp.v12.Protocol.Dataspace
{
    /// <summary>
    /// Defines the interface that must be implemented by the customer role of the Dataspace protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.Dataspace, "customer", "store")]
    public interface IDataspaceCustomer : IProtocolHandler
    {
        /// <summary>
        /// Sends a GetDataspaces message to a store.
        /// </summary>
        /// <param name="lastChangedFilter">An optional filter to limit the dataspaces returned by date last changed.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long GetDataspaces(long? lastChangedFilter);

        /// <summary>
        /// Handles the GetDataspacesResponse event from a store.
        /// </summary>
        event ProtocolEventHandler<GetDataspacesResponse> OnGetDataspacesResponse;

        /// <summary>
        /// Sends a PutDataspaces message to a store.
        /// </summary>
        /// <param name="dataspaces">The dataspaces.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long PutDataspaces(IList<Datatypes.Object.Dataspace> dataspaces);

        /// <summary>
        /// Sends a DeleteDataspaces message to a store.
        /// </summary>
        /// <param name="uids">The UIDs.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long DeleteDataspaces(IList<string> uids);
    }
}

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

namespace Energistics.Etp.v12.Protocol.Dataspace
{
    /// <summary>
    /// Defines the interface that must be implemented by the store role of the store protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.Dataspace, "store", "customer")]
    public interface IDataspaceStore : IProtocolHandler
    {
        /// <summary>
        /// Handles the GetDataspaces event from a customer.
        /// </summary>
        event ProtocolEventHandler<GetDataspaces, IList<Datatypes.Object.Dataspace>> OnGetDataspaces;

        /// <summary>
        /// Sends an GetDataspacesResponse message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="dataspaces">The dataspaces.</param>
        /// <returns>The message identifier.</returns>
        long GetDataspacesResponse(IMessageHeader request, IList<Datatypes.Object.Dataspace> dataspaces);

        /// <summary>
        /// Handles the PutDataspaces event from a customer.
        /// </summary>
        event ProtocolEventWithErrorsHandler<PutDataspaces, ErrorInfo> OnPutDataspaces;

        /// <summary>
        /// Handles the DeleteDataspaces event from a customer.
        /// </summary>
        event ProtocolEventWithErrorsHandler<DeleteDataspaces, ErrorInfo> OnDeleteDataspaces;
    }
}

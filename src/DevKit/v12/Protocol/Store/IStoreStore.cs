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

namespace Energistics.Etp.v12.Protocol.Store
{
    /// <summary>
    /// Defines the interface that must be implemented by the store role of the store protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.Store, "store", "customer")]
    public interface IStoreStore : IProtocolHandler
    {
        /// <summary>
        /// Sends an GetDataObjectsResponse message to a customer.
        /// </summary>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="dataObjects">The data objects.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        long GetDataObjectsResponse(long correlationId, IList<DataObject> dataObjects, IList<ErrorInfo> errors, MessageFlags messageFlag = MessageFlags.MultiPartAndFinalPart);

        /// <summary>
        /// Handles the GetDataObjects event from a customer.
        /// </summary>
        event ProtocolEventHandler<GetDataObjects> OnGetDataObjects;

        /// <summary>
        /// Handles the PutDataObjects event from a customer.
        /// </summary>
        event ProtocolEventHandler<PutDataObjects> OnPutDataObjects;

        /// <summary>
        /// Handles the DeleteDataObjects event from a customer.
        /// </summary>
        event ProtocolEventHandler<DeleteDataObjects> OnDeleteDataObjects;
    }
}

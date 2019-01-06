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

namespace Energistics.Etp.v12.Protocol.Store
{
    /// <summary>
    /// Defines the interface that must be implemented by the customer role of the Store protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.Store, "customer", "store")]
    public interface IStoreCustomer : IProtocolHandler
    {
        /// <summary>
        /// Sends a GetObject message to a store.
        /// </summary>
        /// <param name="uris">The URI.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        long GetDataObjects(IList<string> uris, MessageFlags messageFlag = MessageFlags.MultiPartAndFinalPart);

        /// <summary>
        /// Sends a PutObject message to a store.
        /// </summary>
        /// <param name="dataObjects">The data objects.</param>
        /// <returns>The message identifier.</returns>
        long PutDataObjects(IList<DataObject> dataObjects);

        /// <summary>
        /// Sends a DeleteObject message to a store.
        /// </summary>
        /// <param name="uris">The URI.</param>
        /// <returns>The message identifier.</returns>
        long DeleteDataObjects(IList<string> uris);

        /// <summary>
        /// Handles the GetDataObjectsResponse event from a store.
        /// </summary>
        event ProtocolEventHandler<GetDataObjectsResponse> OnGetDataObjectsResponse;
    }
}

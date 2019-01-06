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
using Energistics.Etp.v11.Datatypes.Object;

namespace Energistics.Etp.v11.Protocol.Store
{
    /// <summary>
    /// Defines the interface that must be implemented by the store role of the store protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.Store, "store", "customer")]
    public interface IStoreStore : IProtocolHandler
    {
        /// <summary>
        /// Sends an Object message to a customer.
        /// </summary>
        /// <param name="dataObject">The data object.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        long Object(DataObject dataObject, long correlationId, MessageFlags messageFlag = MessageFlags.MultiPartAndFinalPart);

        /// <summary>
        /// Handles the GetObject event from a customer.
        /// </summary>
        event ProtocolEventHandler<GetObject, DataObject> OnGetObject;

        /// <summary>
        /// Handles the PutObject event from a customer.
        /// </summary>
        event ProtocolEventHandler<PutObject> OnPutObject;

        /// <summary>
        /// Handles the DeleteObject event from a customer.
        /// </summary>
        event ProtocolEventHandler<DeleteObject> OnDeleteObject;
    }
}

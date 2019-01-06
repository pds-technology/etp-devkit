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

namespace Energistics.Etp.v11.Protocol.GrowingObject
{
    /// <summary>
    /// Defines the interface that must be implemented by the store role of the growing object protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.GrowingObject, "store", "customer")]
    public interface IGrowingObjectStore : IProtocolHandler
    {
        /// <summary>
        /// Sends a single list item as a response for Get and GetRange.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="contentType">The content type string.</param>
        /// <param name="data">The data.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        long ObjectFragment(string uri, string contentType, byte[] data, long correlationId, MessageFlags messageFlag = MessageFlags.MultiPartAndFinalPart);

        /// <summary>
        /// Handles the GrowingObjectGet event from a customer.
        /// </summary>
        event ProtocolEventHandler<GrowingObjectGet> OnGrowingObjectGet;

        /// <summary>
        /// Handles the GrowingObjectGetRange event from a customer.
        /// </summary>
        event ProtocolEventHandler<GrowingObjectGetRange> OnGrowingObjectGetRange;

        /// <summary>
        /// Handles the GrowingObjectPut event from a customer.
        /// </summary>
        event ProtocolEventHandler<GrowingObjectPut> OnGrowingObjectPut;

        /// <summary>
        /// Handles the GrowingObjectDelete event from a customer.
        /// </summary>
        event ProtocolEventHandler<GrowingObjectDelete> OnGrowingObjectDelete;

        /// <summary>
        /// Handles the GrowingObjectDeleteRange event from a customer.
        /// </summary>
        event ProtocolEventHandler<GrowingObjectDeleteRange> OnGrowingObjectDeleteRange;
    }
}

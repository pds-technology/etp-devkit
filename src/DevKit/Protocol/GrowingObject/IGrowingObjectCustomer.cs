//----------------------------------------------------------------------- 
// ETP DevKit, 1.1
//
// Copyright 2016 Energistics
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

using Energistics.Common;
using Energistics.Datatypes;

namespace Energistics.Protocol.GrowingObject
{
    /// <summary>
    /// Defines the interface that must be implemented by the customer role of the growing object protocol.
    /// </summary>
    /// <seealso cref="Energistics.Common.IProtocolHandler" />
    [ProtocolRole(Protocols.GrowingObject, "customer", "store")]
    public interface IGrowingObjectCustomer : IProtocolHandler
    {
        /// <summary>
        /// Gets a single list item in a growing object, by its ID.
        /// </summary>
        /// <param name="uuid">The UUID of the parent object.</param>
        /// <param name="uid">The ID of the element within the list.</param>
        /// <returns>The message identifier.</returns>
        long GrowingObjectGet(string uuid, string uid);

        /// <summary>
        /// Gets all list items in a growing object within an index range.
        /// </summary>
        /// <param name="uuid">The UUID of the parent object.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <returns>The message identifier.</returns>
        long GrowingObjectGetRange(string uuid, long startIndex, long endIndex);

        /// <summary>
        /// Adds or updates a list item in a growing object.
        /// </summary>
        /// <param name="uuid">The UUID of the parent object.</param>
        /// <param name="contentType">The content type string for the parent object.</param>
        /// <param name="data">The data (list items) to be added to the growing object.</param>
        /// <returns>The message identifier.</returns>
        long GrowingObjectPut(string uuid, string contentType, byte[] data);

        /// <summary>
        /// Deletes one list item in a growing object.
        /// </summary>
        /// <param name="uuid">The UUID of the parent object.</param>
        /// <param name="uid">The ID of the element within the list.</param>
        /// <returns>The message identifier.</returns>
        long GrowingObjectDelete(string uuid, string uid);

        /// <summary>
        /// Deletes all list items in a range of index values. Range is inclusive of the limits.
        /// </summary>
        /// <param name="uuid">The UUID of the parent object.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <returns>The message identifier.</returns>
        long GrowingObjectDeleteRange(string uuid, long startIndex, long endIndex);

        /// <summary>
        /// Handles the ObjectFragment event from a store.
        /// </summary>
        event ProtocolEventHandler<ObjectFragment> OnObjectFragment;
    }
}

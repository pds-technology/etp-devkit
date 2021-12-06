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
using System;

namespace Energistics.Etp.v11.Protocol.GrowingObject
{
    /// <summary>
    /// Defines the interface that must be implemented by the customer role of the growing object protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.GrowingObject, Roles.Customer, Roles.Store)]
    public interface IGrowingObjectCustomer : IProtocolHandler
    {
        /// <summary>
        /// Gets a single list item in a growing object, by its ID.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="uid">The ID of the element within the list.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GrowingObjectGet> GrowingObjectGet(string uri, string uid);

        /// <summary>
        /// Gets all list items in a growing object within an index range.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="uom">The unit of measure.</param>
        /// <param name="depthDatum">The depth datum.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GrowingObjectGetRange> GrowingObjectGetRange(string uri, object startIndex, object endIndex, string uom, string depthDatum);

        /// <summary>
        /// Adds or updates a list item in a growing object.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="contentType">The content type string for the parent object.</param>
        /// <param name="data">The data (list items) to be added to the growing object.</param>
        /// <param name="contentEncoding">The content encoding the data.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GrowingObjectPut> GrowingObjectPut(string uri, string contentType, byte[] data, string contentEncoding = ContentEncodings.TextXml);

        /// <summary>
        /// Deletes one list item in a growing object.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="uid">The ID of the element within the list.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GrowingObjectDelete> GrowingObjectDelete(string uri, string uid);

        /// <summary>
        /// Deletes all list items in a range of index values. Range is inclusive of the limits.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GrowingObjectDeleteRange> GrowingObjectDeleteRange(string uri, object startIndex, object endIndex, string uom, string depthDatum);

        /// <summary>
        /// Handles the ObjectFragment event from a store in response to GrowingObjectGet.
        /// </summary>
        event EventHandler<ResponseEventArgs<GrowingObjectGet, ObjectFragment>> OnGrowingObjectGetObjectFragment;

        /// <summary>
        /// Handles the ObjectFragment event from a store in response to GrowingObjectGetRange.
        /// </summary>
        event EventHandler<ResponseEventArgs<GrowingObjectGetRange, ObjectFragment>> OnGrowingObjectGetRangeObjectFragment;

        /// <summary>
        /// Event raised when there is an exception received in response to a GrowingObjectPut message.
        /// </summary>
        event EventHandler<VoidResponseEventArgs<GrowingObjectPut>> OnGrowingObjectPutException;

        /// <summary>
        /// Event raised when there is an exception received in response to a GrowingObjectDelete message.
        /// </summary>
        event EventHandler<VoidResponseEventArgs<GrowingObjectDelete>> OnGrowingObjectDeleteException;

        /// <summary>
        /// Event raised when there is an exception received in response to a GrowingObjectDeleteRange message.
        /// </summary>
        event EventHandler<VoidResponseEventArgs<GrowingObjectDeleteRange>> OnGrowingObjectDeleteRangeException;
    }
}

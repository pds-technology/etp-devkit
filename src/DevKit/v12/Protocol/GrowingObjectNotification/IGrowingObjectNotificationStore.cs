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

namespace Energistics.Etp.v12.Protocol.GrowingObjectNotification
{
    /// <summary>
    /// Defines the interface that must be implemented by the store role of the growing object notification protocol.
    /// </summary>
    /// <seealso cref="IProtocolHandler" />
    [ProtocolRole((int)Protocols.GrowingObjectNotification, "store", "customer")]
    public interface IGrowingObjectNotificationStore : IProtocolHandler
    {
        /// <summary>
        /// Sends a PartChanged message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="uid">The UID.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="data">The data.</param>
        /// <param name="changeKind">The change kind.</param>
        /// <param name="changeTime">The change time.</param>
        /// <returns>The message identifier.</returns>
        long PartChanged(IMessageHeader request, string uri, string uid, string contentType, byte[] data, ObjectChangeKind changeKind, long changeTime);

        /// <summary>
        /// Sends a PartDeleted message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="uid">The UID.</param>
        /// <param name="changeTime">The change time.</param>
        /// <returns>The message identifier.</returns>
        long PartDeleted(IMessageHeader request, string uri, string uid, long changeTime);

        /// <summary>
        /// Sends a PartsDeletedByRange message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="uom">The unit of measure.</param>
        /// <param name="depthDatum">The depth datum.</param>
        /// <param name="includeOverlappingIntervals"><c>true</c> if overlapping intervals should be included; otherwise, <c>false</c>.</param>
        /// <param name="changeTime">The change time.</param>
        /// <returns>The message identifier.</returns>
        long PartsDeletedByRange(IMessageHeader request, string uri, object startIndex, object endIndex, string uom, string depthDatum, bool includeOverlappingIntervals, long changeTime);

        /// <summary>
        /// Sends a PartsReplacedByRange message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="uid">The uid.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="data">The data.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="uom">The unit of measure.</param>
        /// <param name="depthDatum">The depth datum.</param>
        /// <param name="includeOverlappingIntervals"><c>true</c> if overlapping intervals should be included; otherwise, <c>false</c>.</param>
        /// <param name="changeTime">The change time.</param>
        /// <returns>The message identifier.</returns>
        long PartsReplacedByRange(IMessageHeader request, string uri, string uid, string contentType, byte[] data, object startIndex, object endIndex, string uom, string depthDatum, bool includeOverlappingIntervals, long changeTime);

        /// <summary>
        /// Handles the SubscribePartNotification event from a customer.
        /// </summary>
        event ProtocolEventHandler<SubscribePartNotification> OnSubscribePartNotification;

        /// <summary>
        /// Handles the UnsubscribePartNotification event from a customer.
        /// </summary>
        event ProtocolEventHandler<UnsubscribePartNotification> OnUnsubscribePartNotification;
    }
}

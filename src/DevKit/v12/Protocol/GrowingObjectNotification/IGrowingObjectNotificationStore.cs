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
using System;
using System.Collections.Generic;

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
        /// Sends a PartsChanged message to a customer.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="uri">The URI of the growing object.</param>
        /// <param name="parts">The changed parts.</param>
        /// <param name="changeKind">The change kind.</param>
        /// <param name="changeTime">The change time.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <returns>The message identifier.</returns>
        long PartsChanged(Guid requestUuid, string uri, IList<ObjectPart> parts, ObjectChangeKind changeKind, long changeTime, string format = "xml");

        /// <summary>
        /// Sends a PartsDeleted message to a customer.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="uri">The URI of the growing object.</param>
        /// <param name="uids">The UIDs of the deleted parts.</param>
        /// <param name="changeTime">The change time.</param>
        /// <returns>The message identifier.</returns>
        long PartsDeleted(Guid requestUuid, string uri, IList<string> uids, long changeTime);

        /// <summary>
        /// Sends a PartsDeletedByRange message to a customer.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="deletedInterval">The index interval for the deleted range.</param>
        /// <param name="includeOverlappingIntervals"><c>true</c> if overlapping intervals were included; otherwise, <c>false</c>.</param>
        /// <param name="changeTime">The change time.</param>
        /// <returns>The message identifier.</returns>
        long PartsDeletedByRange(Guid requestUuid, string uri, IndexInterval deletedInterval, bool includeOverlappingIntervals, long changeTime);

        /// <summary>
        /// Sends a PartsReplacedByRange message to a customer.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="deletedInterval">The index interval for the deleted range.</param>
        /// <param name="includeOverlappingIntervals"><c>true</c> if overlapping intervals were included; otherwise, <c>false</c>.</param>
        /// <param name="parts">The map of UIDs and data of the parts that were put.</param>
        /// <param name="changeTime">The change time.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <returns>The message identifier.</returns>
        long PartsReplacedByRange(Guid requestUuid, string uri, IndexInterval deletedInterval, bool includeOverlappingIntervals, IList<ObjectPart> parts, long changeTime, string format = "xml");

        /// <summary>
        /// Sends a PartSubscriptionEnded message to a customer.
        /// </summary>
        /// <param name="requestUuid">The UUID of the subscription that has ended.</param>
        /// <returns>The message identifier.</returns>
        long PartSubscriptionEnded(Guid requestUuid);

        /// <summary>
        /// Sends an UnsolicitedPartNotifications message to a customer.
        /// </summary>
        /// <param name="subscriptions">The unsolicited subscriptions.</param>
        /// <returns>The message identifier.</returns>
        long UnsolicitedPartNotifications(IList<SubscriptionInfo> subscriptions);

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

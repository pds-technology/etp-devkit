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

namespace Energistics.Etp.v12.Protocol.StoreNotification
{
    /// <summary>
    /// Defines the interface that must be implemented by the store role of the store notification protocol.
    /// </summary>
    /// <seealso cref="IProtocolHandler" />
    [ProtocolRole((int)Protocols.StoreNotification, "store", "customer")]
    public interface IStoreNotificationStore : IProtocolHandler
    {
        /// <summary>
        /// Handles the SubscribeNotifications event from a customer.
        /// </summary>
        event ProtocolEventHandler<SubscribeNotifications> OnSubscribeNotifications;

        /// <summary>
        /// Sends an ObjectChanged message to a customer.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="change">The object change.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long ObjectChanged(Guid requestUuid, ObjectChange change);

        /// <summary>
        /// Sends an ObjectDeleted message to a customer.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="changeTime">The change time.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long ObjectDeleted(Guid requestUuid, string uri, long changeTime);

        /// <summary>
        /// Sends a Chunk message to a customer.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <param name="blobId">The blob ID.</param>
        /// <param name="data">The chunk data.</param>
        /// <param name="messageFlags">The message flags.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long Chunk(IMessageHeader notification, Guid blobId, byte[] data, MessageFlags messageFlags = MessageFlags.MultiPartAndFinalPart);

        /// <summary>
        /// Sends an ObjectAccessRevoked message to a customer.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="changeTime">The change time.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long ObjectAccessRevoked(Guid requestUuid, string uri, long changeTime);

        /// <summary>
        /// Sends a SubscriptionEnded message to a customer.
        /// </summary>
        /// <param name="requestUuid">The UUID of the subscription that has ended.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long SubscriptionEnded(Guid requestUuid);

        /// <summary>
        /// Sends an UnsolicitedStoreNotifications message to a customer.
        /// </summary>
        /// <param name="subscriptions">The unsolicited subscriptions.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long UnsolicitedStoreNotifications(IList<SubscriptionInfo> subscriptions);

        /// <summary>
        /// Handles the UnsubscribeNotifications event from a customer.
        /// </summary>
        event ProtocolEventHandler<UnsubscribeNotifications> OnUnsubscribeNotifications;
    }
}

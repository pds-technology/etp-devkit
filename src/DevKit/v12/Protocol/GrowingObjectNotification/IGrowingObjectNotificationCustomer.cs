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

using System;
using System.Collections.Generic;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.GrowingObjectNotification
{
    /// <summary>
    /// Defines the interface that must be implemented by the customer role of the growing object notification protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.GrowingObjectNotification, Roles.Customer, Roles.Store)]
    public interface IGrowingObjectNotificationCustomer : IProtocolHandlerWithCounterpartCapabilities<ICapabilitiesStore>
    {
        /// <summary>
        /// Sends a SubscribePartNotifications message to a store.
        /// </summary>
        /// <param name="request">The subscription request.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<SubscribePartNotifications> SubscribePartNotifications(IDictionary<string, SubscriptionInfo> request, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a SubscribePartNotifications message to a store.
        /// </summary>
        /// <param name="request">The subscription request.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<SubscribePartNotifications> SubscribePartNotifications(IList<SubscriptionInfo> request, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the SubscribePartNotificationsResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<SubscribePartNotifications, SubscribePartNotificationsResponse>> OnSubscribePartNotificationsResponse;

        /// <summary>
        /// Handles the UnsolicitedPartNotifications event from a store.
        /// </summary>
        event EventHandler<FireAndForgetEventArgs<UnsolicitedPartNotifications>> OnUnsolicitedPartNotifications;

        /// <summary>
        /// Handles the PartsChanged event from a store.
        /// </summary>
        event EventHandler<NotificationEventArgs<SubscriptionInfo, PartsChanged>> OnPartsChanged;

        /// <summary>
        /// Handles the PartsDeleted event from a store.
        /// </summary>
        event EventHandler<NotificationEventArgs<SubscriptionInfo, PartsDeleted>> OnPartsDeleted;

        /// <summary>
        /// Handles the PartsReplacedByRange event from a store.
        /// </summary>
        event EventHandler<NotificationEventArgs<SubscriptionInfo, PartsReplacedByRange>> OnPartsReplacedByRange;

        /// <summary>
        /// Sends an UnsubscribePartNotification message to a store.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<UnsubscribePartNotification> UnsubscribePartNotification(Guid requestUuid, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the PartSubscriptionEnded event from a store when sent in response to a UnsubscribePartNotification.
        /// </summary>
        event EventHandler<ResponseEventArgs<UnsubscribePartNotification, PartSubscriptionEnded>> OnResponsePartSubscriptionEnded;

        /// <summary>
        /// Handles the PartSubscriptionEnded event from a store when not sent in response to a request.
        /// </summary>
        event EventHandler<FireAndForgetEventArgs<PartSubscriptionEnded>> OnNotificationPartSubscriptionEnded;
    }
}

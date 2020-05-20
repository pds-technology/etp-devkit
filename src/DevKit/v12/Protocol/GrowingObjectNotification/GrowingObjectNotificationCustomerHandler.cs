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
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.GrowingObjectNotification
{
    /// <summary>
    /// Base implementation of the <see cref="IGrowingObjectNotificationCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.GrowingObjectNotification.IGrowingObjectNotificationCustomer" />
    public class GrowingObjectNotificationCustomerHandler : Etp12ProtocolHandler, IGrowingObjectNotificationCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrowingObjectNotificationCustomerHandler"/> class.
        /// </summary>
        public GrowingObjectNotificationCustomerHandler() : base((int)Protocols.GrowingObjectNotification, "customer", "store")
        {
            RegisterMessageHandler<PartsChanged>(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.PartsChanged, HandlePartsChanged);
            RegisterMessageHandler<PartsDeleted>(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.PartsDeleted, HandlePartsDeleted);
            RegisterMessageHandler<PartsDeletedByRange>(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.PartsDeletedByRange, HandlePartsDeletedByRange);
            RegisterMessageHandler<PartsReplacedByRange>(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.PartsReplacedByRange, HandlePartsReplacedByRange);
            RegisterMessageHandler<PartSubscriptionEnded>(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.PartSubscriptionEnded, HandlePartSubscriptionEnded);
            RegisterMessageHandler<UnsolicitedPartNotifications>(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.UnsolicitedPartNotifications, HandleUnsolicitedPartNotifications);
        }

        /// <summary>
        /// Sends a SubscribePartNotification message to a store.
        /// </summary>
        /// <param name="request">The subscription request.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long SubscribePartNotification(SubscriptionInfo request)
        {
            var header = CreateMessageHeader(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.SubscribePartNotification);

            var notificationRequest = new SubscribePartNotification
            {
                Request = request,
            };

            return Session.SendMessage(header, notificationRequest);
        }

        /// <summary>
        /// Handles the PartsChanged event from a store.
        /// </summary>
        public event ProtocolEventHandler<PartsChanged> OnPartsChanged;

        /// <summary>
        /// Handles the PartsDeleted event from a store.
        /// </summary>
        public event ProtocolEventHandler<PartsDeleted> OnPartsDeleted;

        /// <summary>
        /// Handles the PartsDeletedByRange event from a store.
        /// </summary>
        public event ProtocolEventHandler<PartsDeletedByRange> OnPartsDeletedByRange;

        /// <summary>
        /// Handles the PartsReplacedByRange event from a store.
        /// </summary>
        public event ProtocolEventHandler<PartsReplacedByRange> OnPartsReplacedByRange;

        /// <summary>
        /// Sends an UnsubscribePartNotification message to a store.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long UnsubscribePartNotification(Guid requestUuid)
        {
            var header = CreateMessageHeader(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.UnsubscribePartNotification);

            var cancelNotification = new UnsubscribePartNotification
            {
                RequestUuid = requestUuid.ToUuid(),
            };

            return Session.SendMessage(header, cancelNotification);
        }

        /// <summary>
        /// Handles the PartSubscriptionEnded event from a store.
        /// </summary>
        public event ProtocolEventHandler<PartSubscriptionEnded> OnPartSubscriptionEnded;

        /// <summary>
        /// Handles the UnsolicitedPartNotifications event from a store.
        /// </summary>
        public event ProtocolEventHandler<UnsolicitedPartNotifications> OnUnsolicitedPartNotifications;

        /// <summary>
        /// Handles the PartsChanged message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="notification">The PartChanged message.</param>
        protected virtual void HandlePartsChanged(IMessageHeader header, PartsChanged notification)
        {
            Notify(OnPartsChanged, header, notification);
        }

        /// <summary>
        /// Handles the PartsDeleted message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="notification">The PartDeleted message.</param>
        protected virtual void HandlePartsDeleted(IMessageHeader header, PartsDeleted notification)
        {
            Notify(OnPartsDeleted, header, notification);
        }

        /// <summary>
        /// Handles the PartsDeletedByRange message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="notification">The PartsDeletedByRange message.</param>
        protected virtual void HandlePartsDeletedByRange(IMessageHeader header, PartsDeletedByRange notification)
        {
            Notify(OnPartsDeletedByRange, header, notification);
        }

        /// <summary>
        /// Handles the PartsReplacedByRange message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="notification">The PartsReplacedByRange message.</param>
        protected virtual void HandlePartsReplacedByRange(IMessageHeader header, PartsReplacedByRange notification)
        {
            Notify(OnPartsReplacedByRange, header, notification);
        }

        /// <summary>
        /// Handles the PartSubscriptionEnded message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="notification">The PartSubscriptionEnded message.</param>
        protected virtual void HandlePartSubscriptionEnded(IMessageHeader header, PartSubscriptionEnded notification)
        {
            Notify(OnPartSubscriptionEnded, header, notification);
        }

        /// <summary>
        /// Handles the UnsolicitedPartNotifications message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="notification">The UnsolicitedPartNotifications message.</param>
        protected virtual void HandleUnsolicitedPartNotifications(IMessageHeader header, UnsolicitedPartNotifications notification)
        {
            Notify(OnUnsolicitedPartNotifications, header, notification);
        }
    }
}

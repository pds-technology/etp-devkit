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

namespace Energistics.Etp.v12.Protocol.StoreNotification
{
    /// <summary>
    /// Base implementation of the <see cref="IStoreNotificationCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.StoreNotification.IStoreNotificationCustomer" />
    public class StoreNotificationCustomerHandler : Etp12ProtocolHandler, IStoreNotificationCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreNotificationCustomerHandler"/> class.
        /// </summary>
        public StoreNotificationCustomerHandler() : base((int)Protocols.StoreNotification, "customer", "store")
        {
            RegisterMessageHandler<ObjectChanged>(Protocols.StoreNotification, MessageTypes.StoreNotification.ObjectChanged, HandleObjectChanged);
            RegisterMessageHandler<ObjectDeleted>(Protocols.StoreNotification, MessageTypes.StoreNotification.ObjectDeleted, HandleObjectDeleted);
            RegisterMessageHandler<ObjectAccessRevoked>(Protocols.StoreNotification, MessageTypes.StoreNotification.ObjectAccessRevoked, HandleObjectAccessRevoked);
        }

        /// <summary>
        /// Sends a SubscribeNotification message to a store.
        /// </summary>
        /// <param name="subscriptionInfo">The subscription information.</param>
        /// <returns>The message identifier.</returns>
        public long SubscribeNotification(SubscriptionInfo subscriptionInfo)
        {
            var header = CreateMessageHeader(Protocols.StoreNotification, MessageTypes.StoreNotification.SubscribeNotification);

            var notificationRequest = new SubscribeNotification
            {
                Request = subscriptionInfo
            };

            return Session.SendMessage(header, notificationRequest);
        }

        /// <summary>
        /// Sends a UnsubscribeNotification message to a store.
        /// </summary>
        /// <param name="requestUuid">The request identifier.</param>
        /// <returns>The message identifier.</returns>
        public long UnsubscribeNotification(Guid requestUuid)
        {
            var header = CreateMessageHeader(Protocols.StoreNotification, MessageTypes.StoreNotification.UnsubscribeNotification);

            var cancelNotification = new UnsubscribeNotification
            {
                RequestUuid = requestUuid.ToUuid()
            };

            return Session.SendMessage(header, cancelNotification);
        }

        /// <summary>
        /// Handles the ObjectChanged event from a store.
        /// </summary>
        public event ProtocolEventHandler<ObjectChanged> OnObjectChanged;

        /// <summary>
        /// Handles the ObjectDeleted event from a store.
        /// </summary>
        public event ProtocolEventHandler<ObjectDeleted> OnObjectDeleted;

        /// <summary>
        /// Handles the ObjectAccessRevoked event from a store.
        /// </summary>
        public event ProtocolEventHandler<ObjectAccessRevoked> OnObjectAccessRevoked;

        /// <summary>
        /// Handles the ObjectChanged message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="notification">The ObjectChanged message.</param>
        protected virtual void HandleObjectChanged(IMessageHeader header, ObjectChanged notification)
        {
            Notify(OnObjectChanged, header, notification);
        }

        /// <summary>
        /// Handles the ObjectDeleted message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="notification">The ObjectDeleted message.</param>
        protected virtual void HandleObjectDeleted(IMessageHeader header, ObjectDeleted notification)
        {
            Notify(OnObjectDeleted, header, notification);
        }

        /// <summary>
        /// Handles the ObjectAccessRevoked message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="notification">The ObjectAccessRevoked message.</param>
        protected virtual void HandleObjectAccessRevoked(IMessageHeader header, ObjectAccessRevoked notification)
        {
            Notify(OnObjectAccessRevoked, header, notification);
        }
    }
}

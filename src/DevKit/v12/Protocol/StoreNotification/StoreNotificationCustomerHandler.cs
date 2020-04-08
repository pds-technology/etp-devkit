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
            RegisterMessageHandler<Chunk>(Protocols.StoreNotification, MessageTypes.StoreNotification.Chunk, HandleChunk);
            RegisterMessageHandler<ObjectAccessRevoked>(Protocols.StoreNotification, MessageTypes.StoreNotification.ObjectAccessRevoked, HandleObjectAccessRevoked);
            RegisterMessageHandler<SubscriptionEnded>(Protocols.StoreNotification, MessageTypes.StoreNotification.SubscriptionEnded, HandleSubscriptionEnded);
            RegisterMessageHandler<UnsolicitedStoreNotifications>(Protocols.StoreNotification, MessageTypes.StoreNotification.UnsolicitedStoreNotifications, HandleUnsolicitedStoreNotifications);
        }

        /// <summary>
        /// Sends a SubscribeNotifications message to a store.
        /// </summary>
        /// <param name="request">The subscription request.</param>
        /// <returns>The message identifier.</returns>
        public long SubscribeNotifications(SubscriptionInfo request)
        {
            var header = CreateMessageHeader(Protocols.StoreNotification, MessageTypes.StoreNotification.SubscribeNotifications);

            var message = new SubscribeNotifications
            {
                Request = request
            };

            return Session.SendMessage(header, message);
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
        /// Handles the Chunk event from a store.
        /// </summary>
        public event ProtocolEventHandler<Chunk> OnChunk;

        /// <summary>
        /// Handles the ObjectAccessRevoked event from a store.
        /// </summary>
        public event ProtocolEventHandler<ObjectAccessRevoked> OnObjectAccessRevoked;

        /// <summary>
        /// Sends a UnsubscribeNotifications message to a store.
        /// </summary>
        /// <param name="requestUuid">The request identifier.</param>
        /// <returns>The message identifier.</returns>
        public long UnsubscribeNotifications(Guid requestUuid)
        {
            var header = CreateMessageHeader(Protocols.StoreNotification, MessageTypes.StoreNotification.UnsubscribeNotifications);

            var message = new UnsubscribeNotifications
            {
                RequestUuid = requestUuid.ToUuid()
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Handles the SubscriptionEnded event from a store.
        /// </summary>
        public event ProtocolEventHandler<SubscriptionEnded> OnSubscriptionEnded;

        /// <summary>
        /// Handles the UnsolicitedStoreNotifications event from a store.
        /// </summary>
        public event ProtocolEventHandler<UnsolicitedStoreNotifications> OnUnsolicitedStoreNotifications;

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
        /// Handles the Chunk message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="notification">The Chunk message.</param>
        protected virtual void HandleChunk(IMessageHeader header, Chunk notification)
        {
            Notify(OnChunk, header, notification);
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

        /// <summary>
        /// Handles the SubscriptionEnded message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="notification">The SubscriptionEnded message.</param>
        protected virtual void HandleSubscriptionEnded(IMessageHeader header, SubscriptionEnded notification)
        {
            Notify(OnSubscriptionEnded, header, notification);
        }

        /// <summary>
        /// Handles the UnsolicitedStoreNotifications message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="notification">The UnsolicitedStoreNotifications message.</param>
        protected virtual void HandleUnsolicitedStoreNotifications(IMessageHeader header, UnsolicitedStoreNotifications notification)
        {
            Notify(OnUnsolicitedStoreNotifications, header, notification);
        }

    }
}

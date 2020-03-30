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
    /// Base implementation of the <see cref="IStoreNotificationStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.StoreNotification.IStoreNotificationStore" />
    public class StoreNotificationStoreHandler : Etp12ProtocolHandler, IStoreNotificationStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreNotificationStoreHandler"/> class.
        /// </summary>
        public StoreNotificationStoreHandler() : base((int)Protocols.StoreNotification, "store", "customer")
        {
            RegisterMessageHandler<SubscribeNotifications>(Protocols.StoreNotification, MessageTypes.StoreNotification.SubscribeNotifications, HandleSubscribeNotifications);
            RegisterMessageHandler<UnsubscribeNotifications>(Protocols.StoreNotification, MessageTypes.StoreNotification.UnsubscribeNotifications, HandleUnsubscribeNotifications);
        }

        /// <summary>
        /// Sends an ObjectChanged message to a customer.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="change">The object change.</param>
        /// <returns>The message identifier.</returns>
        public virtual long ObjectChanged(Guid requestUuid, ObjectChange change)
        {
            var header = CreateMessageHeader(Protocols.StoreNotification, MessageTypes.StoreNotification.ObjectChanged);

            var message = new ObjectChanged
            {
                RequestUuid = requestUuid.ToUuid(),
                Change = change,
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Sends an ObjectDeleted message to a customer.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="changeTime">The change time.</param>
        /// <returns>The message identifier.</returns>
        public virtual long ObjectDeleted(Guid requestUuid, string uri, long changeTime)
        {
            var header = CreateMessageHeader(Protocols.StoreNotification, MessageTypes.StoreNotification.ObjectDeleted);

            var message = new ObjectDeleted
            {
                RequestUuid = requestUuid.ToUuid(),
                Uri = uri,
                ChangeTime = changeTime,
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Sends a Chunk message to a customer.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <param name="blobId">The blob ID.</param>
        /// <param name="data">The chunk data.</param>
        /// <param name="messageFlags">The message flags.</param>
        /// <returns>The message identifier.</returns>
        public virtual long Chunk(IMessageHeader notification, Guid blobId, byte[] data, MessageFlags messageFlags = MessageFlags.MultiPartAndFinalPart)
        {
            var header = CreateMessageHeader(Protocols.StoreNotification, MessageTypes.StoreNotification.Chunk, notification.MessageId, messageFlags);

            var message = new Chunk
            {
                BlobId = blobId.ToUuid(),
                Data = data,
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Sends a ObjectAccessRevoked message to a customer.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="changeTime">The change time.</param>
        /// <returns>The message identifier.</returns>
        public virtual long ObjectAccessRevoked(Guid requestUuid, string uri, long changeTime)
        {
            var header = CreateMessageHeader(Protocols.StoreNotification, MessageTypes.StoreNotification.ObjectAccessRevoked);

            var message = new ObjectAccessRevoked
            {
                RequestUuid = requestUuid.ToUuid(),
                Uri = uri,
                ChangeTime = changeTime
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Sends a SubscriptionEnded message to a customer.
        /// </summary>
        /// <param name="requestUuid">The UUID of the subscription that has ended.</param>
        /// <returns>The message identifier.</returns>
        public virtual long SubscriptionEnded(Guid requestUuid)
        {
            var header = CreateMessageHeader(Protocols.StoreNotification, MessageTypes.StoreNotification.SubscriptionEnded);

            var message = new SubscriptionEnded
            {
                RequestUuid = requestUuid.ToUuid(),
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Sends an UnsolicitedStoreNotifications message to a customer.
        /// </summary>
        /// <param name="subscriptions">The unsolicited subscriptions.</param>
        /// <returns>The message identifier.</returns>
        public virtual long UnsolicitedStoreNotifications(IList<SubscriptionInfo> subscriptions)
        {
            var header = CreateMessageHeader(Protocols.StoreNotification, MessageTypes.StoreNotification.UnsolicitedStoreNotifications);

            var message = new UnsolicitedStoreNotifications
            {
                Subscriptions = subscriptions,
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Handles the SubscribeNotification event from a customer.
        /// </summary>
        public event ProtocolEventHandler<SubscribeNotifications> OnSubscribeNotifications;

        /// <summary>
        /// Handles the UnsubscribeNotification event from a customer.
        /// </summary>
        public event ProtocolEventHandler<UnsubscribeNotifications> OnUnsubscribeNotifications;

        /// <summary>
        /// Handles the SubscribeNotificatiosn message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="request">The SubscribeNotification message.</param>
        protected virtual void HandleSubscribeNotifications(IMessageHeader header, SubscribeNotifications request)
        {
            Notify(OnSubscribeNotifications, header, request);
        }

        /// <summary>
        /// Handles the UnsubscribeNotifications message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="request">The UnsubscribeNotification message.</param>
        protected virtual void HandleUnsubscribeNotifications(IMessageHeader header, UnsubscribeNotifications request)
        {
            Notify(OnUnsubscribeNotifications, header, request);
        }
    }
}

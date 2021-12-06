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
using Energistics.Etp.Common.Protocol.Core;
using Energistics.Etp.v12.Datatypes;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.StoreNotification
{
    /// <summary>
    /// Base implementation of the <see cref="IStoreNotificationCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.StoreNotification.IStoreNotificationCustomer" />
    public class StoreNotificationCustomerHandler : Etp12ProtocolHandler<CapabilitiesCustomer, ICapabilitiesCustomer, CapabilitiesStore, ICapabilitiesStore>, IStoreNotificationCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreNotificationCustomerHandler"/> class.
        /// </summary>
        public StoreNotificationCustomerHandler() : base((int)Protocols.StoreNotification, Roles.Customer, Roles.Store)
        {
            RegisterMessageHandler<SubscribeNotificationsResponse>(Protocols.StoreNotification, MessageTypes.StoreNotification.SubscribeNotificationsResponse, HandleSubscribeNotificationsResponse);
            RegisterMessageHandler<UnsolicitedStoreNotifications>(Protocols.StoreNotification, MessageTypes.StoreNotification.UnsolicitedStoreNotifications, HandleUnsolicitedStoreNotifications);
            RegisterMessageHandler<ObjectChanged>(Protocols.StoreNotification, MessageTypes.StoreNotification.ObjectChanged, HandleObjectChanged);
            RegisterMessageHandler<Chunk>(Protocols.StoreNotification, MessageTypes.StoreNotification.Chunk, HandleChunk);
            RegisterMessageHandler<ObjectActiveStatusChanged>(Protocols.StoreNotification, MessageTypes.StoreNotification.ObjectActiveStatusChanged, HandleObjectActiveStatusChanged);
            RegisterMessageHandler<ObjectAccessRevoked>(Protocols.StoreNotification, MessageTypes.StoreNotification.ObjectAccessRevoked, HandleObjectAccessRevoked);
            RegisterMessageHandler<ObjectDeleted>(Protocols.StoreNotification, MessageTypes.StoreNotification.ObjectDeleted, HandleObjectDeleted);
            RegisterMessageHandler<SubscriptionEnded>(Protocols.StoreNotification, MessageTypes.StoreNotification.SubscriptionEnded, HandleSubscriptionEnded);
        }

        /// <summary>
        /// Sends a SubscribeNotifications message to a store.
        /// </summary>
        /// <param name="request">The subscription requests.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<SubscribeNotifications> SubscribeNotifications(IDictionary<string, SubscriptionInfo> request, IMessageHeaderExtension extension = null)
        {
            var body = new SubscribeNotifications
            {
                Request = request ?? new Dictionary<string, SubscriptionInfo>(),
            };

            var message = SendRequest(body, extension: extension, onBeforeSend: (m) => TryRegisterSubscriptions(m, request, nameof(SubscriptionInfo.RequestUuid)));

            if (message == null)
            {
                foreach (var kvp in request)
                    TryUnregisterSubscription(kvp.Value);
            }

            return message;
        }

        /// <summary>
        /// Sends a SubscribeNotifications message to a store.
        /// </summary>
        /// <param name="request">The subscription requests.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<SubscribeNotifications> SubscribeNotifications(IList<SubscriptionInfo> request, IMessageHeaderExtension extension = null) => SubscribeNotifications(request.ToMap(), extension: extension);

        /// <summary>
        /// Handles the SubscribeNotificationsResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<SubscribeNotifications, SubscribeNotificationsResponse>> OnSubscribeNotificationsResponse;

        /// <summary>
        /// Handles the UnsolicitedStoreNotifications event from a store.
        /// </summary>
        public event EventHandler<FireAndForgetEventArgs<UnsolicitedStoreNotifications>> OnUnsolicitedStoreNotifications;

        /// <summary>
        /// Handles the ObjectChanged event from a store.
        /// </summary>
        public event EventHandler<NotificationEventArgs<SubscriptionInfo, ObjectChanged>> OnObjectChanged;

        /// <summary>
        /// Handles the Chunk event from a store when sent as part of an ObjectChanged message.
        /// </summary>
        public event EventHandler<NotificationWithDataEventArgs<SubscriptionInfo, ObjectChanged, Chunk>> OnObjectChangedChunk;

        /// <summary>
        /// Handles the ObjectActiveStatusChanged event from a store.
        /// </summary>
        public event EventHandler<NotificationEventArgs<SubscriptionInfo, ObjectActiveStatusChanged>> OnObjectActiveStatusChanged;

        /// <summary>
        /// Handles the ObjectAccessRevoked event from a store.
        /// </summary>
        public event EventHandler<NotificationEventArgs<SubscriptionInfo, ObjectAccessRevoked>> OnObjectAccessRevoked;

        /// <summary>
        /// Handles the ObjectDeleted event from a store.
        /// </summary>
        public event EventHandler<NotificationEventArgs<SubscriptionInfo, ObjectDeleted>> OnObjectDeleted;

        /// <summary>
        /// Sends a UnsubscribeNotifications message to a store.
        /// </summary>
        /// <param name="requestUuid">The request identifier.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<UnsubscribeNotifications> UnsubscribeNotifications(Guid requestUuid, IMessageHeaderExtension extension = null)
        {
            var body = new UnsubscribeNotifications
            {
                RequestUuid = requestUuid,
            };

            var message = SendRequest(body, extension: extension);

            if (message != null)
                TryUnregisterSubscription(body, nameof(body.RequestUuid), message);

            return message;
        }

        /// <summary>
        /// Handles the SubscriptionEnded event from a store when sent in response to a UnsubscribeNotifications.
        /// </summary>
        public event EventHandler<ResponseEventArgs<UnsubscribeNotifications, SubscriptionEnded>> OnResponseSubscriptionEnded;

        /// <summary>
        /// Handles the SubscriptionEnded event from a store when not sent in response to a request.
        /// </summary>
        public event EventHandler<NotificationEventArgs<SubscriptionInfo, SubscriptionEnded>> OnNotificationSubscriptionEnded;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<SubscribeNotifications>)
                HandleResponseMessage(request as EtpMessage<SubscribeNotifications>, message, OnSubscribeNotificationsResponse, HandleSubscribeNotificationsResponse);
            else if (request is EtpMessage<UnsubscribeNotifications>)
                HandleResponseMessage(request as EtpMessage<UnsubscribeNotifications>, message, OnResponseSubscriptionEnded, HandleResponseSubscriptionEnded);
        }

        /// <summary>
        /// Handles the SubscribeNotificationsResponse message from a store.
        /// </summary>
        /// <param name="message">The SubscribeNotificationsResponse message.</param>
        protected virtual void HandleSubscribeNotificationsResponse(EtpMessage<SubscribeNotificationsResponse> message)
        {
            var request = TryGetCorrelatedMessage<SubscribeNotifications>(message);
            HandleResponseMessage(request, message, OnSubscribeNotificationsResponse, HandleSubscribeNotificationsResponse);
        }

        /// <summary>
        /// Handles the SubscribeNotificationsResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{SubscribeNotifications, SubscribeNotificationsResponse}"/> instance containing the event data.</param>
        protected virtual void HandleSubscribeNotificationsResponse(ResponseEventArgs<SubscribeNotifications, SubscribeNotificationsResponse> args)
        {
        }

        /// <summary>
        /// Handles the UnsolicitedStoreNotifications message from a store.
        /// </summary>
        /// <param name="message">The UnsolicitedStoreNotifications message.</param>
        protected virtual void HandleUnsolicitedStoreNotifications(EtpMessage<UnsolicitedStoreNotifications> message)
        {
            if (message.Body.Subscriptions?.Count > 0)
            {
                foreach (var subscription in message.Body.Subscriptions)
                    TryRegisterSubscription(subscription, nameof(subscription.RequestUuid), message, subscription);
            }
            HandleFireAndForgetMessage(message, OnUnsolicitedStoreNotifications, HandleUnsolicitedStoreNotifications);
        }

        /// <summary>
        /// Handles the UnsolicitedStoreNotifications message from a store.
        /// </summary>
        /// <param name="args">The <see cref="FireAndForgetEventArgs{UnsolicitedStoreNotifications}"/> instance containing the event data.</param>
        protected virtual void HandleUnsolicitedStoreNotifications(FireAndForgetEventArgs<UnsolicitedStoreNotifications> args)
        {
        }

        /// <summary>
        /// Handles the ObjectChanged message from a store.
        /// </summary>
        /// <param name="message">The ObjectChanged message.</param>
        protected virtual void HandleObjectChanged(EtpMessage<ObjectChanged> message)
        {
            var subscription = TryGetSubscription<SubscriptionInfo>(message.Body);
            if (!message.Header.IsFinalPart())
                TryRegisterMessage(message);

            HandleNotificationMessage(subscription, message, OnObjectChanged, HandleObjectChanged);
        }

        /// <summary>
        /// Handles the ObjectChanged message from a store.
        /// </summary>
        /// <param name="args">The <see cref="NotificationEventArgs{SubscriptionInfo, ObjectChanged}"/> instance containing the event data.</param>
        protected virtual void HandleObjectChanged(NotificationEventArgs<SubscriptionInfo, ObjectChanged> args)
        {
        }

        /// <summary>
        /// Handles the Chunk message from a store.
        /// </summary>
        /// <param name="message">The Chunk message.</param>
        protected virtual void HandleChunk(EtpMessage<Chunk> message)
        {
            var notification = TryGetCorrelatedMessage<ObjectChanged>(message);
            var subscription = notification == null ? null : TryGetSubscription<SubscriptionInfo>(notification.Body);
            HandleNotificationMessage(subscription, notification, OnObjectChangedChunk, HandleObjectChangedChunk,
                args: new NotificationWithDataEventArgs<SubscriptionInfo, ObjectChanged, Chunk>(subscription, notification, message));
        }

        /// <summary>
        /// Handles the Chunk message from a store when sent as part of an ObjectChanged message.
        /// </summary>
        /// <param name="args">The <see cref="NotificationWithDataEventArgs{SubscriptionInfo, ObjectChanged, Chunk}"/> instance containing the event data.</param>
        protected virtual void HandleObjectChangedChunk(NotificationWithDataEventArgs<SubscriptionInfo, ObjectChanged, Chunk> args)
        {
        }

        /// <summary>
        /// Handles the ObjectActiveStatusChanged message from a store.
        /// </summary>
        /// <param name="message">The ObjectActiveStatusChanged message.</param>
        protected virtual void HandleObjectActiveStatusChanged(EtpMessage<ObjectActiveStatusChanged> message)
        {
            var subscription = TryGetSubscription<SubscriptionInfo>(message.Body);
            HandleNotificationMessage(subscription, message, OnObjectActiveStatusChanged, HandleObjectActiveStatusChanged);
        }

        /// <summary>
        /// Handles the ObjectActiveStatusChanged message from a store.
        /// </summary>
        /// <param name="args">The <see cref="NotificationEventArgs{SubscriptionInfo, ObjectActiveStatusChanged}"/> instance containing the event data.</param>
        protected virtual void HandleObjectActiveStatusChanged(NotificationEventArgs<SubscriptionInfo, ObjectActiveStatusChanged> args)
        {
        }

        /// <summary>
        /// Handles the ObjectAccessRevoked message from a store.
        /// </summary>
        /// <param name="message">The ObjectAccessRevoked message.</param>
        protected virtual void HandleObjectAccessRevoked(EtpMessage<ObjectAccessRevoked> message)
        {
            var subscription = TryGetSubscription<SubscriptionInfo>(message.Body);
            HandleNotificationMessage(subscription, message, OnObjectAccessRevoked, HandleObjectAccessRevoked);
        }

        /// <summary>
        /// Handles the ObjectAccessRevoked message from a store.
        /// </summary>
        /// <param name="args">The <see cref="NotificationEventArgs{SubscriptionInfo, ObjectAccessRevoked}"/> instance containing the event data.</param>
        protected virtual void HandleObjectAccessRevoked(NotificationEventArgs<SubscriptionInfo, ObjectAccessRevoked> args)
        {
        }

        /// <summary>
        /// Handles the ObjectDeleted message from a store.
        /// </summary>
        /// <param name="message">The ObjectDeleted message.</param>
        protected virtual void HandleObjectDeleted(EtpMessage<ObjectDeleted> message)
        {
            var subscription = TryGetSubscription<SubscriptionInfo>(message.Body);
            HandleNotificationMessage(subscription, message, OnObjectDeleted, HandleObjectDeleted);
        }

        /// <summary>
        /// Handles the ObjectDeleted message from a store.
        /// </summary>
        /// <param name="args">The <see cref="NotificationEventArgs{SubscriptionInfo, ObjectDeleted}"/> instance containing the event data.</param>
        protected virtual void HandleObjectDeleted(NotificationEventArgs<SubscriptionInfo, ObjectDeleted> args)
        {
        }

        /// <summary>
        /// Handles the SubscriptionEnded message from a store.
        /// </summary>
        /// <param name="message">The SubscriptionEnded message.</param>
        protected virtual void HandleSubscriptionEnded(EtpMessage<SubscriptionEnded> message)
        {
            if (message.Header.CorrelationId == 0)
            {
                var subscription = TryGetSubscription<SubscriptionInfo>(message.Body);
                HandleNotificationMessage(subscription, message, OnNotificationSubscriptionEnded, HandleNotificationSubscriptionEnded);
            }
            else
            {
                var request = TryGetCorrelatedMessage<UnsubscribeNotifications>(message);
                HandleResponseMessage(request, message, OnResponseSubscriptionEnded, HandleResponseSubscriptionEnded);
            }
        }

        /// <summary>
        /// Handles the SubscriptionEnded message from a store when sent as a notification.
        /// </summary>
        /// <param name="args">The <see cref="NotificationEventArgs{SubscriptionInfo, SubscriptionEnded}"/> instance containing the event data.</param>
        protected virtual void HandleNotificationSubscriptionEnded(NotificationEventArgs<SubscriptionInfo, SubscriptionEnded> args)
        {
        }

        /// <summary>
        /// Handles the SubscriptionEnded message from a store when sent in response to a UnsubscribeNotification message.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{UnsubscribeNotifications, SubscriptionEnded}"/> instance containing the event data.</param>
        protected virtual void HandleResponseSubscriptionEnded(ResponseEventArgs<UnsubscribeNotifications, SubscriptionEnded> args)
        {
        }
    }
}

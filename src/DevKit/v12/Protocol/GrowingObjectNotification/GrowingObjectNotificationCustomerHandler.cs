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

namespace Energistics.Etp.v12.Protocol.GrowingObjectNotification
{
    /// <summary>
    /// Base implementation of the <see cref="IGrowingObjectNotificationCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.GrowingObjectNotification.IGrowingObjectNotificationCustomer" />
    public class GrowingObjectNotificationCustomerHandler : Etp12ProtocolHandlerWithCounterpartCapabilities<CapabilitiesStore, ICapabilitiesStore>, IGrowingObjectNotificationCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrowingObjectNotificationCustomerHandler"/> class.
        /// </summary>
        public GrowingObjectNotificationCustomerHandler() : base((int)Protocols.GrowingObjectNotification, Roles.Customer, Roles.Store)
        {
            RegisterMessageHandler<SubscribePartNotificationsResponse>(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.SubscribePartNotificationsResponse, HandleSubscribePartNotificationsResponse);
            RegisterMessageHandler<UnsolicitedPartNotifications>(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.UnsolicitedPartNotifications, HandleUnsolicitedPartNotifications);
            RegisterMessageHandler<PartsChanged>(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.PartsChanged, HandlePartsChanged);
            RegisterMessageHandler<PartsDeleted>(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.PartsDeleted, HandlePartsDeleted);
            RegisterMessageHandler<PartsReplacedByRange>(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.PartsReplacedByRange, HandlePartsReplacedByRange);
            RegisterMessageHandler<PartSubscriptionEnded>(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.PartSubscriptionEnded, HandlePartSubscriptionEnded);
        }

        /// <summary>
        /// Sends a SubscribePartNotifications message to a store.
        /// </summary>
        /// <param name="request">The subscription request.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<SubscribePartNotifications> SubscribePartNotifications(IDictionary<string, SubscriptionInfo> request, IMessageHeaderExtension extension = null)
        {
            var body = new SubscribePartNotifications
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
        /// Sends a SubscribePartNotifications message to a store.
        /// </summary>
        /// <param name="request">The subscription request.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<SubscribePartNotifications> SubscribePartNotifications(IList<SubscriptionInfo> request, IMessageHeaderExtension extension = null) => SubscribePartNotifications(request.ToMap(), extension: extension);

        /// <summary>
        /// Handles the SubscribePartNotificationsResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<SubscribePartNotifications, SubscribePartNotificationsResponse>> OnSubscribePartNotificationsResponse;

        /// <summary>
        /// Handles the UnsolicitedPartNotifications event from a store.
        /// </summary>
        public event EventHandler<FireAndForgetEventArgs<UnsolicitedPartNotifications>> OnUnsolicitedPartNotifications;

        /// <summary>
        /// Handles the PartsChanged event from a store.
        /// </summary>
        public event EventHandler<NotificationEventArgs<SubscriptionInfo, PartsChanged>> OnPartsChanged;

        /// <summary>
        /// Handles the PartsDeleted event from a store.
        /// </summary>
        public event EventHandler<NotificationEventArgs<SubscriptionInfo, PartsDeleted>> OnPartsDeleted;

        /// <summary>
        /// Handles the PartsReplacedByRange event from a store.
        /// </summary>
        public event EventHandler<NotificationEventArgs<SubscriptionInfo, PartsReplacedByRange>> OnPartsReplacedByRange;

        /// <summary>
        /// Sends an UnsubscribePartNotification message to a store.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<UnsubscribePartNotification> UnsubscribePartNotification(Guid requestUuid, IMessageHeaderExtension extension = null)
        {
            var body = new UnsubscribePartNotification
            {
                RequestUuid = requestUuid,
            };

            var message = SendRequest(body, extension: extension);

            if (message != null)
                TryUnregisterSubscription(body, nameof(body.RequestUuid), message);

            return message;
        }

        /// <summary>
        /// Handles the PartSubscriptionEnded event from a store when sent in response to a UnsubscribePartNotification.
        /// </summary>
        public event EventHandler<ResponseEventArgs<UnsubscribePartNotification, PartSubscriptionEnded>> OnResponsePartSubscriptionEnded;

        /// <summary>
        /// Handles the PartSubscriptionEnded event from a store when not sent in response to a request.
        /// </summary>
        public event EventHandler<NotificationEventArgs<SubscriptionInfo, PartSubscriptionEnded>> OnNotificationPartSubscriptionEnded;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<SubscribePartNotifications>)
                HandleResponseMessage(request as EtpMessage<SubscribePartNotifications>, message, OnSubscribePartNotificationsResponse, HandleSubscribePartNotificationsResponse);
            else if (request is EtpMessage<UnsubscribePartNotification>)
                HandleResponseMessage(request as EtpMessage<UnsubscribePartNotification>, message, OnResponsePartSubscriptionEnded, HandleResponsePartSubscriptionEnded);
        }

        /// <summary>
        /// Handles the SubscribePartNotificationsResponse message from a store.
        /// </summary>
        /// <param name="message">The SubscribePartNotificationsResponse message.</param>
        protected virtual void HandleSubscribePartNotificationsResponse(EtpMessage<SubscribePartNotificationsResponse> message)
        {
            var request = TryGetCorrelatedMessage<SubscribePartNotifications>(message);
            HandleResponseMessage(request, message, OnSubscribePartNotificationsResponse, HandleSubscribePartNotificationsResponse);
        }

        /// <summary>
        /// Handles the SubscribePartNotificationsResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{SubscribePartNotifications, SubscribePartNotificationsResponse}"/> instance containing the event data.</param>
        protected virtual void HandleSubscribePartNotificationsResponse(ResponseEventArgs<SubscribePartNotifications, SubscribePartNotificationsResponse> args)
        {
        }

        /// <summary>
        /// Handles the UnsolicitedPartNotifications message from a store.
        /// </summary>
        /// <param name="message">The UnsolicitedPartNotifications message.</param>
        protected virtual void HandleUnsolicitedPartNotifications(EtpMessage<UnsolicitedPartNotifications> message)
        {
            if (message.Body.Subscriptions?.Count > 0)
            {
                foreach (var subscription in message.Body.Subscriptions)
                    TryRegisterSubscription(subscription, nameof(subscription.RequestUuid), message, subscription);
            }
            HandleFireAndForgetMessage(message, OnUnsolicitedPartNotifications, HandleUnsolicitedPartNotifications);
        }

        /// <summary>
        /// Handles the UnsolicitedPartNotifications message from a store.
        /// </summary>
        /// <param name="args">The <see cref="FireAndForgetEventArgs{UnsolicitedPartNotifications}"/> instance containing the event data.</param>
        protected virtual void HandleUnsolicitedPartNotifications(FireAndForgetEventArgs<UnsolicitedPartNotifications> args)
        {
        }

        /// <summary>
        /// Handles the PartsChanged message from a store.
        /// </summary>
        /// <param name="message">The PartsChanged message.</param>
        protected virtual void HandlePartsChanged(EtpMessage<PartsChanged> message)
        {
            var subscription = TryGetSubscription<SubscriptionInfo>(message.Body);
            HandleNotificationMessage(subscription, message, OnPartsChanged, HandlePartsChanged);
        }

        /// <summary>
        /// Handles the PartsChanged message from a store.
        /// </summary>
        /// <param name="args">The <see cref="NotificationEventArgs{SubscriptionInfo, PartsChanged}"/> instance containing the event data.</param>
        protected virtual void HandlePartsChanged(NotificationEventArgs<SubscriptionInfo, PartsChanged> args)
        {
        }

        /// <summary>
        /// Handles the PartsDeleted message from a store.
        /// </summary>
        /// <param name="message">The PartsDeleted message.</param>
        protected virtual void HandlePartsDeleted(EtpMessage<PartsDeleted> message)
        {
            var subscription = TryGetSubscription<SubscriptionInfo>(message.Body);
            HandleNotificationMessage(subscription, message, OnPartsDeleted, HandlePartsDeleted);
        }

        /// <summary>
        /// Handles the PartsDeleted message from a store.
        /// </summary>
        /// <param name="args">The <see cref="NotificationEventArgs{SubscriptionInfo, PartsDeleted}"/> instance containing the event data.</param>
        protected virtual void HandlePartsDeleted(NotificationEventArgs<SubscriptionInfo, PartsDeleted> args)
        {
        }

        /// <summary>
        /// Handles the PartsReplacedByRange message from a store.
        /// </summary>
        /// <param name="message">The PartsReplacedByRange message.</param>
        protected virtual void HandlePartsReplacedByRange(EtpMessage<PartsReplacedByRange> message)
        {
            var subscription = TryGetSubscription<SubscriptionInfo>(message.Body);
            HandleNotificationMessage(subscription, message, OnPartsReplacedByRange, HandlePartsReplacedByRange);
        }

        /// <summary>
        /// Handles the PartsReplacedByRange message from a store.
        /// </summary>
        /// <param name="args">The <see cref="NotificationEventArgs{SubscriptionInfo, PartsReplacedByRange}"/> instance containing the event data.</param>
        protected virtual void HandlePartsReplacedByRange(NotificationEventArgs<SubscriptionInfo, PartsReplacedByRange> args)
        {
        }

        /// <summary>
        /// Handles the PartSubscriptionEnded message from a store.
        /// </summary>
        /// <param name="message">The PartSubscriptionEnded message.</param>
        protected virtual void HandlePartSubscriptionEnded(EtpMessage<PartSubscriptionEnded> message)
        {
            if (message.Header.CorrelationId == 0)
            {
                var subscription = TryGetSubscription<SubscriptionInfo>(message.Body);
                HandleNotificationMessage(subscription, message, OnNotificationPartSubscriptionEnded, HandleNotificationPartSubscriptionEnded);
            }
            else
            {
                var request = TryGetCorrelatedMessage<UnsubscribePartNotification>(message);
                HandleResponseMessage(request, message, OnResponsePartSubscriptionEnded, HandleResponsePartSubscriptionEnded);
            }
        }

        /// <summary>
        /// Handles the PartSubscriptionEnded message from a store when sent as a notification.
        /// </summary>
        /// <param name="args">The <see cref="NotificationEventArgs{SubscriptionInfo, PartSubscriptionEnded}"/> instance containing the event data.</param>
        protected virtual void HandleNotificationPartSubscriptionEnded(NotificationEventArgs<SubscriptionInfo, PartSubscriptionEnded> args)
        {
        }

        /// <summary>
        /// Handles the PartSubscriptionEnded message from a store when sent in response to a UnsubscribePartNotification message.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{UnsubscribePartNotification, PartSubscriptionEnded}"/> instance containing the event data.</param>
        protected virtual void HandleResponsePartSubscriptionEnded(ResponseEventArgs<UnsubscribePartNotification, PartSubscriptionEnded> args)
        {
        }
    }
}

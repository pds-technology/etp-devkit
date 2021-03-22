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
using Energistics.Etp.Common.Protocol.Core;
using Energistics.Etp.v11.Datatypes.Object;
using System;

namespace Energistics.Etp.v11.Protocol.StoreNotification
{
    /// <summary>
    /// Base implementation of the <see cref="IStoreNotificationCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp11ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v11.Protocol.StoreNotification.IStoreNotificationCustomer" />
    public class StoreNotificationCustomerHandler : Etp11ProtocolHandler, IStoreNotificationCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreNotificationCustomerHandler"/> class.
        /// </summary>
        public StoreNotificationCustomerHandler() : base((int)Protocols.StoreNotification, Roles.Customer, Roles.Store)
        {
            RegisterMessageHandler<ChangeNotification>(Protocols.StoreNotification, MessageTypes.StoreNotification.ChangeNotification, HandleChangeNotification);
            RegisterMessageHandler<DeleteNotification>(Protocols.StoreNotification, MessageTypes.StoreNotification.DeleteNotification, HandleDeleteNotification);
        }

        /// <summary>
        /// Sends a NotificationRequest message to a store.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<NotificationRequest> NotificationRequest(NotificationRequestRecord request)
        {
            var body = new NotificationRequest()
            {
                Request = request
            };

            var message = SendRequest(body, onBeforeSend: (m) => TryRegisterSubscription(request, nameof(request.Uuid), m, request));

            if (message == null)
                TryUnregisterSubscription(request);

            return message;
        }

        /// <summary>
        /// Event raised when there is an exception received in response to a NotificationRequest message.
        /// </summary>
        public event EventHandler<VoidResponseEventArgs<NotificationRequest>> OnNotificationRequestException;

        /// <summary>
        /// Handles the ChangeNotification event from a store.
        /// </summary>
        public event EventHandler<NotificationEventArgs<NotificationRequestRecord, ChangeNotification>> OnChangeNotification;

        /// <summary>
        /// Handles the DeleteNotification event from a store.
        /// </summary>
        public event EventHandler<NotificationEventArgs<NotificationRequestRecord, DeleteNotification>> OnDeleteNotification;

        /// <summary>
        /// Sends a CancelNotification message to a store.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<CancelNotification> CancelNotification(Guid requestUuid)
        {
            var body = new CancelNotification()
            {
                RequestUuid = requestUuid.ToString(),
            };

            var message = SendRequest(body);

            if (message != null)
                TryUnregisterSubscription(message.Body, nameof(body.RequestUuid), message);

            return message;
        }

        /// <summary>
        /// Event raised when there is an exception received in response to a CancelNotification message.
        /// </summary>
        public event EventHandler<VoidResponseEventArgs<CancelNotification>> OnCancelNotificationException;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<NotificationRequest>)
                HandleResponseMessage(request as EtpMessage<NotificationRequest>, message, OnNotificationRequestException, HandleNotificationRequestException);
            else if (request is EtpMessage<CancelNotification>)
                HandleResponseMessage(request as EtpMessage<CancelNotification>, message, OnCancelNotificationException, HandleCancelNotificationException);
        }

        /// <summary>
        /// Handles exceptions to the NotificationRequest message from a store.
        /// </summary>
        /// <param name="args">The <see cref="VoidResponseEventArgs{NotificationRequest}"/> instance containing the event data.</param>
        protected virtual void HandleNotificationRequestException(VoidResponseEventArgs<NotificationRequest> args)
        {
        }

        /// <summary>
        /// Handles the ChangeNotification message from a store.
        /// </summary>
        /// <param name="message">The ChangeNotification message.</param>
        protected virtual void HandleChangeNotification(EtpMessage<ChangeNotification> message)
        {
            var subscription = TryGetSubscription<NotificationRequestRecord>(message.Header);
            HandleNotificationMessage(subscription, message, OnChangeNotification, HandleChangeNotification);
        }

        /// <summary>
        /// Handles the ChangeNotification message from a store.
        /// </summary>
        /// <param name="args">The <see cref="NotificationEventArgs{NotificationRequestRecord, ChangeNotification}"/> instance containing the event data.</param>
        protected virtual void HandleChangeNotification(NotificationEventArgs<NotificationRequestRecord, ChangeNotification> args)
        {
        }

        /// <summary>
        /// Handles the DeleteNotification message from a store.
        /// </summary>
        /// <param name="message">The DeleteNotification message.</param>
        protected virtual void HandleDeleteNotification(EtpMessage<DeleteNotification> message)
        {
            var subscription = TryGetSubscription<NotificationRequestRecord>(message.Header);
            HandleNotificationMessage(subscription, message, OnDeleteNotification, HandleDeleteNotification);
        }

        /// <summary>
        /// Handles the DeleteNotification message from a store.
        /// </summary>
        /// <param name="args">The <see cref="NotificationEventArgs{NotificationRequestRecord, DeleteNotification}"/> instance containing the event data.</param>
        protected virtual void HandleDeleteNotification(NotificationEventArgs<NotificationRequestRecord, DeleteNotification> args)
        {
        }

        /// <summary>
        /// Handles exceptions to the CancelNotification message from a store.
        /// </summary>
        /// <param name="args">The <see cref="VoidResponseEventArgs{CancelNotification}"/> instance containing the event data.</param>
        protected virtual void HandleCancelNotificationException(VoidResponseEventArgs<CancelNotification> args)
        {
        }
    }
}

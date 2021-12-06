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
using Energistics.Etp.v11.Datatypes.Object;
using System;

namespace Energistics.Etp.v11.Protocol.StoreNotification
{
    /// <summary>
    /// Base implementation of the <see cref="IStoreNotificationStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp11ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v11.Protocol.StoreNotification.IStoreNotificationStore" />
    public class StoreNotificationStoreHandler : Etp11ProtocolHandler, IStoreNotificationStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreNotificationStoreHandler"/> class.
        /// </summary>
        public StoreNotificationStoreHandler() : base((int)Protocols.StoreNotification, Roles.Store, Roles.Customer)
        {
            RegisterMessageHandler<NotificationRequest>(Protocols.StoreNotification, MessageTypes.StoreNotification.NotificationRequest, HandleNotificationRequest);
            RegisterMessageHandler<CancelNotification>(Protocols.StoreNotification, MessageTypes.StoreNotification.CancelNotification, HandleCancelNotification);
        }

        /// <summary>
        /// Handles the NotificationRequest event from a customer.
        /// </summary>
        public event EventHandler<VoidRequestEventArgs<NotificationRequest>> OnNotificationRequest;

        /// <summary>
        /// Sends a ChangeNotification message to a customer.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="change">The object change.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ChangeNotification> ChangeNotification(Guid requestUuid, ObjectChange change)
        {
            var subscription = TryGetSubscription<NotificationRequestRecord>(requestUuid);
            if (subscription == null)
                return null;

            return ChangeNotification(subscription?.Header, change);
        }

        /// <summary>
        /// Sends a ChangeNotification message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="change">The object change.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ChangeNotification> ChangeNotification(IMessageHeader correlatedHeader, ObjectChange change)
        {
            var body = new ChangeNotification()
            {
                Change = change,
            };

            return SendNotification(body, correlatedHeader: correlatedHeader);
        }

        /// <summary>
        /// Sends a NotificationRequestDeleteNotification message to a customer.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="change">The object change.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<DeleteNotification> DeleteNotification(Guid requestUuid, ObjectChange change)
        {
            var subscription = TryGetSubscription<NotificationRequestRecord>(requestUuid);
            if (subscription == null)
                return null;

            return DeleteNotification(subscription?.Header, change);
        }

        /// <summary>
        /// Sends a NotificationRequestDeleteNotification message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="change">The object change.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<DeleteNotification> DeleteNotification(IMessageHeader correlatedHeader, ObjectChange change)
        {
            var body = new DeleteNotification()
            {
                Delete = change,
            };

            return SendNotification(body, correlatedHeader: correlatedHeader);
        }

        /// <summary>
        /// Handles the CancelNotification event from a customer.
        /// </summary>
        public event EventHandler<VoidRequestEventArgs<CancelNotification>> OnCancelNotification;

        /// <summary>
        /// Handles the NotificationRequest message from a customer.
        /// </summary>
        /// <param name="message">The NotificationRequest message.</param>
        protected virtual void HandleNotificationRequest(EtpMessage<NotificationRequest> message)
        {
            var error = TryRegisterSubscription(message.Body.Request, nameof(message.Body.Request.Uuid), message, message.Body.Request);
            
            HandleRequestMessage(message, OnNotificationRequest, HandleNotificationRequest,
                args: new VoidRequestEventArgs<NotificationRequest>(message) { FinalError = error });
        }

        /// <summary>
        /// Handles the NotificationRequest message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="VoidRequestEventArgs{NotificationRequest}"/> instance containing the event data.</param>
        protected virtual void HandleNotificationRequest(VoidRequestEventArgs<NotificationRequest> args)
        {
        }

        /// <summary>
        /// Handles the CancelNotification message from a customer.
        /// </summary>
        /// <param name="message">The CancelNotification message.</param>
        protected virtual void HandleCancelNotification(EtpMessage<CancelNotification> message)
        {
            var error = TryUnregisterSubscription(message.Body, nameof(message.Body.RequestUuid), message);

            HandleRequestMessage(message, OnCancelNotification, HandleCancelNotification,
                args: new VoidRequestEventArgs<CancelNotification>(message) { FinalError = error });
        }

        /// <summary>
        /// Handles the CancelNotification message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="VoidRequestEventArgs{CancelNotification}"/> instance containing the event data.</param>
        protected virtual void HandleCancelNotification(VoidRequestEventArgs<CancelNotification> args)
        {
        }
    }
}

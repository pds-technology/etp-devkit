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
        public StoreNotificationStoreHandler() : base((int)Protocols.StoreNotification, "store", "customer")
        {
            RegisterMessageHandler<NotificationRequest>(Protocols.StoreNotification, MessageTypes.StoreNotification.NotificationRequest, HandleNotificationRequest);
            RegisterMessageHandler<CancelNotification>(Protocols.StoreNotification, MessageTypes.StoreNotification.CancelNotification, HandleCancelNotification);
        }

        /// <summary>
        /// Sends a ChangeNotification message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="change">The object change.</param>
        /// <returns>The message identifier.</returns>
        public long ChangeNotification(IMessageHeader request, ObjectChange change)
        {
            var header = CreateMessageHeader(Protocols.StoreNotification, MessageTypes.StoreNotification.ChangeNotification, request.MessageId);

            var notification = new ChangeNotification()
            {
                Change = change
            };

            return Session.SendMessage(header, notification);
        }

        /// <summary>
        /// Sends a NotificationRequestDeleteNotification message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="change">The object change.</param>
        /// <returns>The message identifier.</returns>
        public long DeleteNotification(IMessageHeader request, ObjectChange change)
        {
            var header = CreateMessageHeader(Protocols.StoreNotification, MessageTypes.StoreNotification.DeleteNotification, request.MessageId);

            var notification = new DeleteNotification()
            {
                Delete = change
            };

            return Session.SendMessage(header, notification);
        }

        /// <summary>
        /// Handles the NotificationRequest event from a customer.
        /// </summary>
        public event ProtocolEventHandler<NotificationRequest> OnNotificationRequest;

        /// <summary>
        /// Handles the CancelNotification event from a customer.
        /// </summary>
        public event ProtocolEventHandler<CancelNotification> OnCancelNotification;

        /// <summary>
        /// Handles the NotificationRequest message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="request">The NotificationRequest message.</param>
        protected virtual void HandleNotificationRequest(IMessageHeader header, NotificationRequest request)
        {
            Notify(OnNotificationRequest, header, request);
        }

        /// <summary>
        /// Handles the CancelNotification message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="request">The CancelNotification message.</param>
        protected virtual void HandleCancelNotification(IMessageHeader header, CancelNotification request)
        {
            Notify(OnCancelNotification, header, request);
        }
    }
}

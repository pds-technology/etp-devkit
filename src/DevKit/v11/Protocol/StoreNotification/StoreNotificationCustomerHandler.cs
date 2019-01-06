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
    /// Base implementation of the <see cref="IStoreNotificationCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp11ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v11.Protocol.StoreNotification.IStoreNotificationCustomer" />
    public class StoreNotificationCustomerHandler : Etp11ProtocolHandler, IStoreNotificationCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreNotificationCustomerHandler"/> class.
        /// </summary>
        public StoreNotificationCustomerHandler() : base((int)Protocols.StoreNotification, "customer", "store")
        {
            RegisterMessageHandler<ChangeNotification>(Protocols.StoreNotification, MessageTypes.StoreNotification.ChangeNotification, HandleChangeNotification);
            RegisterMessageHandler<DeleteNotification>(Protocols.StoreNotification, MessageTypes.StoreNotification.DeleteNotification, HandleDeleteNotification);
        }

        /// <summary>
        /// Sends a NotificationRequest message to a store.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The message identifier.</returns>
        public long NotificationRequest(NotificationRequestRecord request)
        {
            var header = CreateMessageHeader(Protocols.StoreNotification, MessageTypes.StoreNotification.NotificationRequest);

            var notificationRequest = new NotificationRequest()
            {
                Request = request
            };

            return Session.SendMessage(header, notificationRequest);
        }

        /// <summary>
        /// Sends a CancelNotification message to a store.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <returns>The message identifier.</returns>
        public long CancelNotification(string requestUuid)
        {
            var header = CreateMessageHeader(Protocols.StoreNotification, MessageTypes.StoreNotification.CancelNotification);

            var cancelNotification = new CancelNotification()
            {
                RequestUuid = requestUuid
            };

            return Session.SendMessage(header, cancelNotification);
        }

        /// <summary>
        /// Handles the ChangeNotification event from a store.
        /// </summary>
        public event ProtocolEventHandler<ChangeNotification> OnChangeNotification;

        /// <summary>
        /// Handles the DeleteNotification event from a store.
        /// </summary>
        public event ProtocolEventHandler<DeleteNotification> OnDeleteNotification;

        /// <summary>
        /// Handles the ChangeNotification message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="notification">The ChangeNotification message.</param>
        protected virtual void HandleChangeNotification(IMessageHeader header, ChangeNotification notification)
        {
            Notify(OnChangeNotification, header, notification);
        }

        /// <summary>
        /// Handles the DeleteNotification message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="notification">The DeleteNotification message.</param>
        protected virtual void HandleDeleteNotification(IMessageHeader header, DeleteNotification notification)
        {
            Notify(OnDeleteNotification, header, notification);
        }
    }
}

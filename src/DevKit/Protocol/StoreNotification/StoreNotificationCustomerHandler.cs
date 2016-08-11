//----------------------------------------------------------------------- 
// ETP DevKit, 1.1
//
// Copyright 2016 Energistics
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

using Avro.IO;
using Energistics.Common;
using Energistics.Datatypes;
using Energistics.Datatypes.Object;

namespace Energistics.Protocol.StoreNotification
{
    /// <summary>
    /// Base implementation of the <see cref="IStoreNotificationCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Energistics.Common.EtpProtocolHandler" />
    /// <seealso cref="Energistics.Protocol.StoreNotification.IStoreNotificationCustomer" />
    public class StoreNotificationCustomerHandler : EtpProtocolHandler, IStoreNotificationCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreNotificationCustomerHandler"/> class.
        /// </summary>
        public StoreNotificationCustomerHandler() : base(Protocols.StoreNotification, "customer", "store")
        {
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
        /// Decodes the message based on the message type contained in the specified <see cref="MessageHeader" />.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="decoder">The message decoder.</param>
        protected override void HandleMessage(MessageHeader header, Decoder decoder)
        {
            switch (header.MessageType)
            {
                case (int)MessageTypes.StoreNotification.ChangeNotification:
                    HandleChangeNotification(header, decoder.Decode<ChangeNotification>());
                    break;

                case (int)MessageTypes.StoreNotification.DeleteNotification:
                    HandleDeleteNotification(header, decoder.Decode<DeleteNotification>());
                    break;

                default:
                    base.HandleMessage(header, decoder);
                    break;
            }
        }

        /// <summary>
        /// Handles the ChangeNotification message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="notification">The ChangeNotification message.</param>
        protected virtual void HandleChangeNotification(MessageHeader header, ChangeNotification notification)
        {
            Notify(OnChangeNotification, header, notification);
        }

        /// <summary>
        /// Handles the DeleteNotification message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="notification">The DeleteNotification message.</param>
        protected virtual void HandleDeleteNotification(MessageHeader header, DeleteNotification notification)
        {
            Notify(OnDeleteNotification, header, notification);
        }
    }
}

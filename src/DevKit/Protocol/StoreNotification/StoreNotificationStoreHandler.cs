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
    /// Base implementation of the <see cref="IStoreNotificationStore"/> interface.
    /// </summary>
    /// <seealso cref="Energistics.Common.EtpProtocolHandler" />
    /// <seealso cref="Energistics.Protocol.StoreNotification.IStoreNotificationStore" />
    public class StoreNotificationStoreHandler : EtpProtocolHandler, IStoreNotificationStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreNotificationStoreHandler"/> class.
        /// </summary>
        public StoreNotificationStoreHandler() : base(Protocols.StoreNotification, "store", "customer")
        {
        }

        /// <summary>
        /// Sends a ChangeNotification message to a customer.
        /// </summary>
        /// <param name="change">The object change.</param>
        /// <returns>The message identifier.</returns>
        public long ChangeNotification(ObjectChange change)
        {
            var header = CreateMessageHeader(Protocols.StoreNotification, MessageTypes.StoreNotification.ChangeNotification);

            var notification = new ChangeNotification()
            {
                Change = change
            };

            return Session.SendMessage(header, notification);
        }

        /// <summary>
        /// Sends a NotificationRequestDeleteNotification message to a customer.
        /// </summary>
        /// <param name="change">The object change.</param>
        /// <returns>The message identifier.</returns>
        public long DeleteNotification(ObjectChange change)
        {
            var header = CreateMessageHeader(Protocols.StoreNotification, MessageTypes.StoreNotification.DeleteNotification);

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
        /// Decodes the message based on the message type contained in the specified <see cref="MessageHeader" />.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="decoder">The message decoder.</param>
        /// <param name="body">The message body.</param>
        protected override void HandleMessage(MessageHeader header, Decoder decoder, string body)
        {
            switch (header.MessageType)
            {
                case (int)MessageTypes.StoreNotification.NotificationRequest:
                    HandleNotificationRequest(header, decoder.Decode<NotificationRequest>(body));
                    break;

                case (int)MessageTypes.StoreNotification.CancelNotification:
                    HandleCancelNotification(header, decoder.Decode<CancelNotification>(body));
                    break;

                default:
                    base.HandleMessage(header, decoder, body);
                    break;
            }
        }

        /// <summary>
        /// Handles the NotificationRequest message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="request">The NotificationRequest message.</param>
        protected virtual void HandleNotificationRequest(MessageHeader header, NotificationRequest request)
        {
            Notify(OnNotificationRequest, header, request);
        }

        /// <summary>
        /// Handles the CancelNotification message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="request">The CancelNotification message.</param>
        protected virtual void HandleCancelNotification(MessageHeader header, CancelNotification request)
        {
            Notify(OnCancelNotification, header, request);
        }
    }
}

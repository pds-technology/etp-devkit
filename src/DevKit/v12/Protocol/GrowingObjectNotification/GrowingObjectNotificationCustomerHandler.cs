//----------------------------------------------------------------------- 
// ETP DevKit, 1.2
//
// Copyright 2018 Energistics
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
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.GrowingObjectNotification
{
    /// <summary>
    /// Base implementation of the <see cref="IGrowingObjectNotificationCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.GrowingObjectNotification.IGrowingObjectNotificationCustomer" />
    public class GrowingObjectNotificationCustomerHandler : Etp12ProtocolHandler, IGrowingObjectNotificationCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrowingObjectNotificationCustomerHandler"/> class.
        /// </summary>
        public GrowingObjectNotificationCustomerHandler() : base((int)Protocols.GrowingObjectNotification, "customer", "store")
        {
        }

        /// <summary>
        /// Sends a RequestPartNotification message to a store.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The message identifier.</returns>
        public long RequestPartNotification(NotificationRequestRecord request)
        {
            var header = CreateMessageHeader(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.RequestPartNotification);

            var notificationRequest = new RequestPartNotification
            {
                Request = request
            };

            return Session.SendMessage(header, notificationRequest);
        }

        /// <summary>
        /// Sends a CancelPartNotification message to a store.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <returns>The message identifier.</returns>
        public long CancelPartNotification(string requestUuid)
        {
            var header = CreateMessageHeader(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.CancelPartNotification);

            var cancelNotification = new CancelPartNotification
            {
                RequestUuid = requestUuid
            };

            return Session.SendMessage(header, cancelNotification);
        }

        /// <summary>
        /// Handles the PartChangeNotification event from a store.
        /// </summary>
        public event ProtocolEventHandler<PartChangeNotification> OnPartChangeNotification;

        /// <summary>
        /// Handles the PartDeleteNotification event from a store.
        /// </summary>
        public event ProtocolEventHandler<PartDeleteNotification> OnPartDeleteNotification;

        /// <summary>
        /// Handles the DeletePartsByRangeNotification event from a store.
        /// </summary>
        public event ProtocolEventHandler<DeletePartsByRangeNotification> OnDeletePartsByRangeNotification;

        /// <summary>
        /// Handles the ReplacePartsByRangeNotification event from a store.
        /// </summary>
        public event ProtocolEventHandler<ReplacePartsByRangeNotification> OnReplacePartsByRangeNotification;

        /// <summary>
        /// Decodes the message based on the message type contained in the specified <see cref="IMessageHeader" />.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="decoder">The message decoder.</param>
        /// <param name="body">The message body.</param>
        protected override void HandleMessage(IMessageHeader header, Decoder decoder, string body)
        {
            switch (header.MessageType)
            {
                case (int)MessageTypes.GrowingObjectNotification.PartChangeNotification:
                    HandlePartChangeNotification(header, decoder.Decode<PartChangeNotification>(body));
                    break;

                case (int)MessageTypes.GrowingObjectNotification.PartDeleteNotification:
                    HandlePartDeleteNotification(header, decoder.Decode<PartDeleteNotification>(body));
                    break;

                case (int)MessageTypes.GrowingObjectNotification.DeletePartsByRangeNotification:
                    HandleDeletePartsByRangeNotification(header, decoder.Decode<DeletePartsByRangeNotification>(body));
                    break;

                case (int)MessageTypes.GrowingObjectNotification.ReplacePartsByRangeNotification:
                    HandleReplacePartsByRangeNotification(header, decoder.Decode<ReplacePartsByRangeNotification>(body));
                    break;

                default:
                    base.HandleMessage(header, decoder, body);
                    break;
            }
        }

        /// <summary>
        /// Handles the PartChangeNotification message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="notification">The PartChangeNotification message.</param>
        protected virtual void HandlePartChangeNotification(IMessageHeader header, PartChangeNotification notification)
        {
            Notify(OnPartChangeNotification, header, notification);
        }

        /// <summary>
        /// Handles the PartDeleteNotification message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="notification">The PartDeleteNotification message.</param>
        protected virtual void HandlePartDeleteNotification(IMessageHeader header, PartDeleteNotification notification)
        {
            Notify(OnPartDeleteNotification, header, notification);
        }

        /// <summary>
        /// Handles the DeletePartsByRangeNotification message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="notification">The DeletePartsByRangeNotification message.</param>
        protected virtual void HandleDeletePartsByRangeNotification(IMessageHeader header, DeletePartsByRangeNotification notification)
        {
            Notify(OnDeletePartsByRangeNotification, header, notification);
        }

        /// <summary>
        /// Handles the ReplacePartsByRangeNotification message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="notification">The ReplacePartsByRangeNotification message.</param>
        protected virtual void HandleReplacePartsByRangeNotification(IMessageHeader header, ReplacePartsByRangeNotification notification)
        {
            Notify(OnReplacePartsByRangeNotification, header, notification);
        }
    }
}

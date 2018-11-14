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

using System;
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
        /// Sends a SubscribePartNotification message to a store.
        /// </summary>
        /// <param name="subscriptionInfo">The subscription information.</param>
        /// <returns>The message identifier.</returns>
        public long SubscribePartNotification(SubscriptionInfo subscriptionInfo)
        {
            var header = CreateMessageHeader(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.SubscribePartNotification);

            var notificationRequest = new SubscribePartNotification
            {
                Request = subscriptionInfo
            };

            return Session.SendMessage(header, notificationRequest);
        }

        /// <summary>
        /// Sends a SubscribePartNotification message to a store.
        /// </summary>
        /// <param name="subscriptionInfo">The subscription information.</param>
        /// <returns>The message identifier.</returns>
        public long SubscribePartNotification2(SubscriptionInfo2 subscriptionInfo)
        {
            var header = CreateMessageHeader(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.SubscribePartNotification2);

            var notificationRequest = new SubscribePartNotification2
            {
                Request = subscriptionInfo
            };

            return Session.SendMessage(header, notificationRequest);
        }

        /// <summary>
        /// Sends an UnsubscribePartNotification message to a store.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <returns>The message identifier.</returns>
        public long UnsubscribePartNotification(Guid requestUuid)
        {
            var header = CreateMessageHeader(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.UnsubscribePartNotification);

            var cancelNotification = new UnsubscribePartNotification
            {
                RequestUuid = requestUuid.ToUuid()
            };

            return Session.SendMessage(header, cancelNotification);
        }

        /// <summary>
        /// Handles the PartChanged event from a store.
        /// </summary>
        public event ProtocolEventHandler<PartChanged> OnPartChanged;

        /// <summary>
        /// Handles the PartDeleted event from a store.
        /// </summary>
        public event ProtocolEventHandler<PartDeleted> OnPartDeleted;

        /// <summary>
        /// Handles the PartsDeletedByRange event from a store.
        /// </summary>
        public event ProtocolEventHandler<PartsDeletedByRange> OnPartsDeletedByRange;

        /// <summary>
        /// Handles the PartsReplacedByRange event from a store.
        /// </summary>
        public event ProtocolEventHandler<PartsReplacedByRange> OnPartsReplacedByRange;

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
                case (int)MessageTypes.GrowingObjectNotification.PartChanged:
                    HandlePartChanged(header, decoder.Decode<PartChanged>(body));
                    break;

                case (int)MessageTypes.GrowingObjectNotification.PartDeleted:
                    HandlePartDeleted(header, decoder.Decode<PartDeleted>(body));
                    break;

                case (int)MessageTypes.GrowingObjectNotification.PartsDeletedByRange:
                    HandlePartsDeletedByRange(header, decoder.Decode<PartsDeletedByRange>(body));
                    break;

                case (int)MessageTypes.GrowingObjectNotification.PartsReplacedByRange:
                    HandlePartsReplacedByRange(header, decoder.Decode<PartsReplacedByRange>(body));
                    break;

                default:
                    base.HandleMessage(header, decoder, body);
                    break;
            }
        }

        /// <summary>
        /// Handles the PartChanged message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="notification">The PartChanged message.</param>
        protected virtual void HandlePartChanged(IMessageHeader header, PartChanged notification)
        {
            Notify(OnPartChanged, header, notification);
        }

        /// <summary>
        /// Handles the PartDeleted message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="notification">The PartDeleted message.</param>
        protected virtual void HandlePartDeleted(IMessageHeader header, PartDeleted notification)
        {
            Notify(OnPartDeleted, header, notification);
        }

        /// <summary>
        /// Handles the PartsDeletedByRange message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="notification">The PartsDeletedByRange message.</param>
        protected virtual void HandlePartsDeletedByRange(IMessageHeader header, PartsDeletedByRange notification)
        {
            Notify(OnPartsDeletedByRange, header, notification);
        }

        /// <summary>
        /// Handles the PartsReplacedByRange message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="notification">The PartsReplacedByRange message.</param>
        protected virtual void HandlePartsReplacedByRange(IMessageHeader header, PartsReplacedByRange notification)
        {
            Notify(OnPartsReplacedByRange, header, notification);
        }
    }
}

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

namespace Energistics.Etp.v12.Protocol.StoreNotification
{
    /// <summary>
    /// Base implementation of the <see cref="IStoreNotificationStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.StoreNotification.IStoreNotificationStore" />
    public class StoreNotificationStoreHandler : Etp12ProtocolHandler, IStoreNotificationStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreNotificationStoreHandler"/> class.
        /// </summary>
        public StoreNotificationStoreHandler() : base((int)Protocols.StoreNotification, "store", "customer")
        {
        }

        /// <summary>
        /// Sends an ObjectChanged message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="change">The object change.</param>
        /// <returns>The message identifier.</returns>
        public long ObjectChanged(IMessageHeader request, ObjectChange change)
        {
            var header = CreateMessageHeader(Protocols.StoreNotification, MessageTypes.StoreNotification.ObjectChanged, request.MessageId);

            var notification = new ObjectChanged
            {
                Change = change
            };

            return Session.SendMessage(header, notification);
        }

        /// <summary>
        /// Sends a ObjectDeleted message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="changeTime">The change time.</param>
        /// <returns>The message identifier.</returns>
        public long ObjectDeleted(IMessageHeader request, string uri, long changeTime)
        {
            var header = CreateMessageHeader(Protocols.StoreNotification, MessageTypes.StoreNotification.ObjectDeleted, request.MessageId);

            var notification = new ObjectDeleted
            {
                Uri = uri,
                ChangeTime = changeTime
            };

            return Session.SendMessage(header, notification);
        }

        /// <summary>
        /// Sends a ObjectAccessRevoked message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="changeTime">The change time.</param>
        /// <returns>The message identifier.</returns>
        public long ObjectAccessRevoked(IMessageHeader request, string uri, long changeTime)
        {
            var header = CreateMessageHeader(Protocols.StoreNotification, MessageTypes.StoreNotification.ObjectAccessRevoked, request.MessageId);

            var notification = new ObjectAccessRevoked
            {
                Uri = uri,
                ChangeTime = changeTime
            };

            return Session.SendMessage(header, notification);
        }

        /// <summary>
        /// Handles the SubscribeNotification event from a customer.
        /// </summary>
        public event ProtocolEventHandler<SubscribeNotification> OnSubscribeNotification;

        /// <summary>
        /// Handles the SubscribeNotification event from a customer.
        /// </summary>
        public event ProtocolEventHandler<SubscribeNotification2> OnSubscribeNotification2;

        /// <summary>
        /// Handles the UnsubscribeNotification event from a customer.
        /// </summary>
        public event ProtocolEventHandler<UnsubscribeNotification> OnUnsubscribeNotification;

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
                case (int)MessageTypes.StoreNotification.SubscribeNotification:
                    HandleSubscribeNotification(header, decoder.Decode<SubscribeNotification>(body));
                    break;

                case (int)MessageTypes.StoreNotification.SubscribeNotification2:
                    HandleSubscribeNotification2(header, decoder.Decode<SubscribeNotification2>(body));
                    break;

                case (int)MessageTypes.StoreNotification.UnsubscribeNotification:
                    HandleUnsubscribeNotification(header, decoder.Decode<UnsubscribeNotification>(body));
                    break;

                default:
                    base.HandleMessage(header, decoder, body);
                    break;
            }
        }

        /// <summary>
        /// Handles the SubscribeNotification message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="request">The SubscribeNotification message.</param>
        protected virtual void HandleSubscribeNotification(IMessageHeader header, SubscribeNotification request)
        {
            Notify(OnSubscribeNotification, header, request);
        }

        /// <summary>
        /// Handles the SubscribeNotification message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="request">The SubscribeNotification message.</param>
        protected virtual void HandleSubscribeNotification2(IMessageHeader header, SubscribeNotification2 request)
        {
            Notify(OnSubscribeNotification2, header, request);
        }

        /// <summary>
        /// Handles the UnsubscribeNotification message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="request">The UnsubscribeNotification message.</param>
        protected virtual void HandleUnsubscribeNotification(IMessageHeader header, UnsubscribeNotification request)
        {
            Notify(OnUnsubscribeNotification, header, request);
        }
    }
}

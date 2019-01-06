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
using Energistics.Etp.v12.Datatypes;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.GrowingObjectNotification
{
    /// <summary>
    /// Base implementation of the <see cref="IGrowingObjectNotificationStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.GrowingObjectNotification.IGrowingObjectNotificationStore" />
    public class GrowingObjectNotificationStoreHandler : Etp12ProtocolHandler, IGrowingObjectNotificationStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrowingObjectNotificationStoreHandler"/> class.
        /// </summary>
        public GrowingObjectNotificationStoreHandler() : base((int)Protocols.GrowingObjectNotification, "store", "customer")
        {
            RegisterMessageHandler<SubscribePartNotification>(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.SubscribePartNotification, HandleSubscribePartNotification);
            RegisterMessageHandler<UnsubscribePartNotification>(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.UnsubscribePartNotification, HandleUnsubscribePartNotification);
        }

        /// <summary>
        /// Sends a PartChanged message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="uid">The UID.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="data">The data.</param>
        /// <param name="changeKind">The change kind.</param>
        /// <param name="changeTime">The change time.</param>
        /// <returns>The message identifier.</returns>
        public long PartChanged(IMessageHeader request, string uri, string uid, string contentType, byte[] data, ObjectChangeKind changeKind, long changeTime)
        {
            var header = CreateMessageHeader(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.PartChanged, request.MessageId);

            var message = new PartChanged
            {
                Uri = uri,
                Uid = uid,
                ContentType = contentType,
                Data = data,
                ChangeKind = changeKind,
                ChangeTime = changeTime
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Sends a PartDeleted message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="uid">The UID.</param>
        /// <param name="changeTime">The change time.</param>
        /// <returns>The message identifier.</returns>
        public long PartDeleted(IMessageHeader request, string uri, string uid, long changeTime)
        {
            var header = CreateMessageHeader(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.PartDeleted, request.MessageId);

            var message = new PartDeleted
            {
                Uri = uri,
                Uid = uid,
                ChangeTime = changeTime
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Sends a PartsDeletedByRange message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="uom">The unit of measure.</param>
        /// <param name="depthDatum">The depth datum.</param>
        /// <param name="includeOverlappingIntervals"><c>true</c> if overlapping intervals should be included; otherwise, <c>false</c>.</param>
        /// <param name="changeTime">The change time.</param>
        /// <returns>The message identifier.</returns>
        public long PartsDeletedByRange(IMessageHeader request, string uri, object startIndex, object endIndex, string uom, string depthDatum, bool includeOverlappingIntervals, long changeTime)
        {
            var header = CreateMessageHeader(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.PartsDeletedByRange, request.MessageId);

            var message = new PartsDeletedByRange
            {
                Uri = uri,
                DeletedInterval = new IndexInterval
                {
                    StartIndex = new IndexValue { Item = startIndex },
                    EndIndex = new IndexValue { Item = endIndex },
                    Uom = uom ?? string.Empty,
                    DepthDatum = depthDatum ?? string.Empty
                },
                IncludeOverlappingIntervals = includeOverlappingIntervals,
                ChangeTime = changeTime
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Sends a PartsReplacedByRange message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="uid">The uid.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="data">The data.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="uom">The unit of measure.</param>
        /// <param name="depthDatum">The depth datum.</param>
        /// <param name="includeOverlappingIntervals"><c>true</c> if overlapping intervals should be included; otherwise, <c>false</c>.</param>
        /// <param name="changeTime">The change time.</param>
        /// <returns>The message identifier.</returns>
        public long PartsReplacedByRange(IMessageHeader request, string uri, string uid, string contentType, byte[] data, object startIndex, object endIndex, string uom, string depthDatum, bool includeOverlappingIntervals, long changeTime)
        {
            var header = CreateMessageHeader(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.PartsReplacedByRange, request.MessageId);

            var message = new PartsReplacedByRange
            {
                Uri = uri,
                Uid = uid,
                ContentType = contentType,
                Data = data,
                DeletedInterval = new IndexInterval
                {
                    StartIndex = new IndexValue { Item = startIndex },
                    EndIndex = new IndexValue { Item = endIndex },
                    Uom = uom ?? string.Empty,
                    DepthDatum = depthDatum ?? string.Empty
                },
                IncludeOverlappingIntervals = includeOverlappingIntervals,
                ChangeTime = changeTime
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Handles the SubscribePartNotification event from a customer.
        /// </summary>
        public event ProtocolEventHandler<SubscribePartNotification> OnSubscribePartNotification;

        /// <summary>
        /// Handles the UnsubscribePartNotification event from a customer.
        /// </summary>
        public event ProtocolEventHandler<UnsubscribePartNotification> OnUnsubscribePartNotification;

        /// <summary>
        /// Handles the SubscribePartNotification message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="request">The SubscribePartNotification message.</param>
        protected virtual void HandleSubscribePartNotification(IMessageHeader header, SubscribePartNotification request)
        {
            Notify(OnSubscribePartNotification, header, request);
        }

        /// <summary>
        /// Handles the UnsubscribePartNotification message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="request">The UnsubscribePartNotification message.</param>
        protected virtual void HandleUnsubscribePartNotification(IMessageHeader header, UnsubscribePartNotification request)
        {
            Notify(OnUnsubscribePartNotification, header, request);
        }
    }
}

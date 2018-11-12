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
        }

        /// <summary>
        /// Sends a PartChangeNotification message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="uid">The UID.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="data">The data.</param>
        /// <param name="changeKind">The change kind.</param>
        /// <param name="changeTime">The change time.</param>
        /// <returns>The message identifier.</returns>
        public long PartChangeNotification(IMessageHeader request, string uri, string uid, string contentType, byte[] data, ObjectChangeKind changeKind, long changeTime)
        {
            var header = CreateMessageHeader(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.PartChangeNotification, request.MessageId);

            var message = new PartChangeNotification
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
        /// Sends a PartDeleteNotification message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="uid">The UID.</param>
        /// <param name="changeTime">The change time.</param>
        /// <returns>The message identifier.</returns>
        public long PartDeleteNotification(IMessageHeader request, string uri, string uid, long changeTime)
        {
            var header = CreateMessageHeader(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.PartDeleteNotification, request.MessageId);

            var message = new PartDeleteNotification
            {
                Uri = uri,
                Uid = uid,
                ChangeTime = changeTime
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Sends a DeletePartsByRangeNotification message to a customer.
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
        public long DeletePartsByRangeNotification(IMessageHeader request, string uri, object startIndex, object endIndex, string uom, string depthDatum, bool includeOverlappingIntervals, long changeTime)
        {
            var header = CreateMessageHeader(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.PartDeleteNotification, request.MessageId);

            var message = new DeletePartsByRangeNotification
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
        /// Sends a ReplacePartsByRangeNotification message to a customer.
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
        public long ReplacePartsByRangeNotification(IMessageHeader request, string uri, string uid, string contentType, byte[] data, object startIndex, object endIndex, string uom, string depthDatum, bool includeOverlappingIntervals, long changeTime)
        {
            var header = CreateMessageHeader(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.PartDeleteNotification, request.MessageId);

            var message = new ReplacePartsByRangeNotification
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
        /// Handles the RequestPartNotification event from a customer.
        /// </summary>
        public event ProtocolEventHandler<RequestPartNotification> OnRequestPartNotification;

        /// <summary>
        /// Handles the CancelPartNotification event from a customer.
        /// </summary>
        public event ProtocolEventHandler<CancelPartNotification> OnCancelPartNotification;

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
                case (int)MessageTypes.GrowingObjectNotification.RequestPartNotification:
                    HandleRequestPartNotification(header, decoder.Decode<RequestPartNotification>(body));
                    break;

                case (int)MessageTypes.GrowingObjectNotification.CancelPartNotification:
                    HandleCancelPartNotification(header, decoder.Decode<CancelPartNotification>(body));
                    break;

                default:
                    base.HandleMessage(header, decoder, body);
                    break;
            }
        }

        /// <summary>
        /// Handles the RequestPartNotification message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="request">The RequestPartNotification message.</param>
        protected virtual void HandleRequestPartNotification(IMessageHeader header, RequestPartNotification request)
        {
            Notify(OnRequestPartNotification, header, request);
        }

        /// <summary>
        /// Handles the CancelPartNotification message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="request">The CancelPartNotification message.</param>
        protected virtual void HandleCancelPartNotification(IMessageHeader header, CancelPartNotification request)
        {
            Notify(OnCancelPartNotification, header, request);
        }
    }
}

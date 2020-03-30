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
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

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
        /// Sends a PartsChanged message to a customer.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="uri">The URI of the growing object.</param>
        /// <param name="parts">The changed parts.</param>
        /// <param name="changeKind">The change kind.</param>
        /// <param name="changeTime">The change time.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <returns>The message identifier.</returns>
        public virtual long PartsChanged(Guid requestUuid, string uri, IList<ObjectPart> parts, ObjectChangeKind changeKind, long changeTime, string format = "xml")
        {
            var header = CreateMessageHeader(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.PartsChanged);
            var message = new PartsChanged
            {
                RequestUuid = requestUuid.ToUuid(),
                Uri = uri,
                ChangeKind = changeKind,
                ChangeTime = changeTime,
                Format = format ?? "xml",
            };

            return Session.Send12MultipartResponse(header, message, parts, (m, i) => m.Parts = i);
        }

        /// <summary>
        /// Sends a PartsDeleted message to a customer.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="uri">The URI of the growing object.</param>
        /// <param name="uids">The UIDs of the deleted parts.</param>
        /// <param name="changeTime">The change time.</param>
        /// <returns>The message identifier.</returns>
        public virtual long PartsDeleted(Guid requestUuid, string uri, IList<string> uids, long changeTime)
        {
            var header = CreateMessageHeader(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.PartsDeleted);
            var message = new PartsDeleted
            {
                RequestUuid = requestUuid.ToUuid(),
                Uri = uri,
                ChangeTime = changeTime
            };

            return Session.Send12MultipartResponse(header, message, uids, (m, i) => m.Uids = i);
        }

        /// <summary>
        /// Sends a PartsDeletedByRange message to a customer.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="deletedInterval">The index interval for the deleted range.</param>
        /// <param name="includeOverlappingIntervals"><c>true</c> if overlapping intervals were included; otherwise, <c>false</c>.</param>
        /// <param name="changeTime">The change time.</param>
        /// <returns>The message identifier.</returns>
        public virtual long PartsDeletedByRange(Guid requestUuid, string uri, IndexInterval deletedInterval, bool includeOverlappingIntervals, long changeTime)
        {
            var header = CreateMessageHeader(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.PartsDeletedByRange);

            var message = new PartsDeletedByRange
            {
                RequestUuid = requestUuid.ToUuid(),
                Uri = uri,
                DeletedInterval = deletedInterval,
                IncludeOverlappingIntervals = includeOverlappingIntervals,
                ChangeTime = changeTime
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Sends a PartsReplacedByRange message to a customer.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="deletedInterval">The index interval for the deleted range.</param>
        /// <param name="includeOverlappingIntervals"><c>true</c> if overlapping intervals were included; otherwise, <c>false</c>.</param>
        /// <param name="parts">The map of UIDs and data of the parts that were put.</param>
        /// <param name="changeTime">The change time.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <returns>The message identifier.</returns>
        public virtual long PartsReplacedByRange(Guid requestUuid, string uri, IndexInterval deletedInterval, bool includeOverlappingIntervals, IList<ObjectPart> parts, long changeTime, string format = "xml")
        {
            var header = CreateMessageHeader(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.PartsReplacedByRange);
            var message = new PartsReplacedByRange
            {
                RequestUuid = requestUuid.ToUuid(),
                Uri = uri,
                DeletedInterval = deletedInterval,
                IncludeOverlappingIntervals = includeOverlappingIntervals,
                ChangeTime = changeTime,
                Format = format ?? "xml",
            };

            return Session.Send12MultipartResponse(header, message, parts, (m, i) => m.Parts = i);
        }

        /// <summary>
        /// Sends a PartSubscriptionEnded message to a customer.
        /// </summary>
        /// <param name="requestUuid">The UUID of the subscription that has ended.</param>
        /// <returns>The message identifier.</returns>
        public virtual long PartSubscriptionEnded(Guid requestUuid)
        {
            var header = CreateMessageHeader(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.PartSubscriptionEnded);

            var message = new PartSubscriptionEnded
            {
                RequestUuid = requestUuid.ToUuid(),
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Sends an UnsolicitedPartNotifications message to a customer.
        /// </summary>
        /// <param name="subscriptions">The unsolicited subscriptions.</param>
        /// <returns>The message identifier.</returns>
        public virtual long UnsolicitedPartNotifications(IList<SubscriptionInfo> subscriptions)
        {
            var header = CreateMessageHeader(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.UnsolicitedPartNotifications);

            var message = new UnsolicitedPartNotifications
            {
                Subscriptions = subscriptions,
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

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
using System.Collections.Generic;

namespace Energistics.Etp.v12.Protocol.GrowingObjectNotification
{
    /// <summary>
    /// Defines the interface that must be implemented by the store role of the growing object notification protocol.
    /// </summary>
    /// <seealso cref="IProtocolHandler" />
    [ProtocolRole((int)Protocols.GrowingObjectNotification, Roles.Store, Roles.Customer)]
    public interface IGrowingObjectNotificationStore : IProtocolHandlerWithCapabilities<ICapabilitiesStore>
    {
        /// <summary>
        /// Handles the SubscribePartNotifications event from a customer.
        /// </summary>
        event EventHandler<MapRequestEventArgs<SubscribePartNotifications, string>> OnSubscribePartNotifications;

        /// <summary>
        /// Sends a SubscribePartNotificationsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<SubscribePartNotificationsResponse> SubscribePartNotificationsResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, bool isFinalPart = true, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a complete multi-part set of SubscribePartNotificationsResponse and ProtocolException messages to a customer.
        /// If there are no successes, an empty SubscribePartNotificationsRecord message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the SubscribePartNotificationsResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        EtpMessage<SubscribePartNotificationsResponse> SubscribePartNotificationsResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null);

        /// <summary>
        /// Sends an UnsolicitedPartNotifications message to a customer.
        /// </summary>
        /// <param name="subscriptions">The unsolicited subscriptions.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<UnsolicitedPartNotifications> UnsolicitedPartNotifications(IList<SubscriptionInfo> subscriptions, IMessageHeaderExtension exceptionExtension = null);

        /// <summary>
        /// Sends a PartsChanged message to a customer.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="uri">The URI of the growing object.</param>
        /// <param name="parts">The changed parts.</param>
        /// <param name="changeKind">The change kind.</param>
        /// <param name="changeTime">The change time.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<PartsChanged> PartsChanged(Guid requestUuid, string uri, IList<ObjectPart> parts, ObjectChangeKind changeKind, long changeTime, string format = Formats.Xml, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a PartsDeleted message to a customer.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="uri">The URI of the growing object.</param>
        /// <param name="uids">The UIDs of the deleted parts.</param>
        /// <param name="changeTime">The change time.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<PartsDeleted> PartsDeleted(Guid requestUuid, string uri, IList<string> uids, long changeTime, IMessageHeaderExtension extension = null);

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
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<PartsReplacedByRange> PartsReplacedByRange(Guid requestUuid, string uri, IndexInterval deletedInterval, bool includeOverlappingIntervals, IList<ObjectPart> parts, long changeTime, string format = Formats.Xml, bool isFinalPart = true, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the UnsubscribePartNotification event from a customer.
        /// </summary>
        event EventHandler<RequestEventArgs<UnsubscribePartNotification, Guid>> OnUnsubscribePartNotification;

        /// <summary>
        /// Sends a PartSubscriptionEnded message to a customer in response to a UnsubscribePartNotification message.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="requestUuid">The reqyest UUId.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<PartSubscriptionEnded> ResponsePartSubscriptionEnded(IMessageHeader correlatedHeader, Guid requestUuid, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a PartSubscriptionEnded message to a customer as a notification.
        /// </summary>
        /// <param name="requestUuid">The reqyest UUId.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<PartSubscriptionEnded> NotificationPartSubscriptionEnded(Guid requestUuid, IMessageHeaderExtension extension = null);
    }
}

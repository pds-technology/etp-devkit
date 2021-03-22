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

namespace Energistics.Etp.v12.Protocol.StoreNotification
{
    /// <summary>
    /// Defines the interface that must be implemented by the store role of the store notification protocol.
    /// </summary>
    /// <seealso cref="IProtocolHandler" />
    [ProtocolRole((int)Protocols.StoreNotification, Roles.Store, Roles.Customer)]
    public interface IStoreNotificationStore : IProtocolHandler<ICapabilitiesStore, ICapabilitiesCustomer>
    {
        /// <summary>
        /// Handles the SubscribeNotifications event from a customer.
        /// </summary>
        event EventHandler<MapRequestEventArgs<SubscribeNotifications, string>> OnSubscribeNotifications;

        /// <summary>
        /// Sends a SubscribeNotificationsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="isFinalPart">Whether or not this is the final  of a multi- message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<SubscribeNotificationsResponse> SubscribeNotificationsResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, bool isFinalPart = true, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a complete multi- set of SubscribeNotificationsResponse and ProtocolException messages to a customer.
        /// If there are no successes, an empty SubscribeNotificationsRecord message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final  flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the SubscribeNotificationsResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        EtpMessage<SubscribeNotificationsResponse> SubscribeNotificationsResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null);

        /// <summary>
        /// Sends an UnsolicitedStoreNotifications message to a customer.
        /// </summary>
        /// <param name="subscriptions">The unsolicited subscriptions.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<UnsolicitedStoreNotifications> UnsolicitedStoreNotifications(IList<SubscriptionInfo> subscriptions, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends an ObjectChanged message to a customer.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="change">The object change.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ObjectChanged> ObjectChanged(Guid requestUuid, ObjectChange change, bool isFinalPart = true, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a Chunk message to a customer as part of a multi-part ObjectChanged message.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="blobId">The blob ID.</param>
        /// <param name="data">The chunk data.</param>
        /// <param name="final">Whether or not this is the final chunk for the blob ID.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<Chunk> ObjectChangedChunk(IMessageHeader correlatedHeader, Guid blobId, byte[] data, bool final, bool isFinalPart = true, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends an ObjectActiveStatusChanged message to a customer.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="activeStatus">The active status.</param>
        /// <param name="changeTime">The change time.</param>
        /// <param name="resource">The resource on which the active status has changed.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ObjectActiveStatusChanged> ObjectActiveStatusChanged(Guid requestUuid, ActiveStatusKind activeStatus, long changeTime, Resource resource, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends an ObjectAccessRevoked message to a customer.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="changeTime">The change time.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ObjectAccessRevoked> ObjectAccessRevoked(Guid requestUuid, string uri, long changeTime, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends an ObjectDeleted message to a customer.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="changeTime">The change time.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ObjectDeleted> ObjectDeleted(Guid requestUuid, string uri, long changeTime, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the UnsubscribeNotifications event from a customer.
        /// </summary>
        event EventHandler<RequestEventArgs<UnsubscribeNotifications, Guid>> OnUnsubscribeNotifications;

        /// <summary>
        /// Sends a SubscriptionEnded message to a customer in response to a UnsubscribeNotifications message.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="requestUuid">The reqyest UUId.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<SubscriptionEnded> ResponseSubscriptionEnded(IMessageHeader correlatedHeader, Guid requestUuid, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a SubscriptionEnded message to a customer as a notification.
        /// </summary>
        /// <param name="requestUuid">The reqyest UUId.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<SubscriptionEnded> NotificationSubscriptionEnded(Guid requestUuid, IMessageHeaderExtension extension = null);
    }
}

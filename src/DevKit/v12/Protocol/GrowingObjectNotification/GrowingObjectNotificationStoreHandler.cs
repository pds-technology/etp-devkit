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
    public class GrowingObjectNotificationStoreHandler : Etp12ProtocolHandlerWithCapabilities<CapabilitiesStore, ICapabilitiesStore>, IGrowingObjectNotificationStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrowingObjectNotificationStoreHandler"/> class.
        /// </summary>
        public GrowingObjectNotificationStoreHandler() : base((int)Protocols.GrowingObjectNotification, Roles.Store, Roles.Customer)
        {
            RegisterMessageHandler<SubscribePartNotifications>(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.SubscribePartNotifications, HandleSubscribePartNotifications);
            RegisterMessageHandler<UnsubscribePartNotification>(Protocols.GrowingObjectNotification, MessageTypes.GrowingObjectNotification.UnsubscribePartNotification, HandleUnsubscribePartNotification);
        }

        /// <summary>
        /// Handles the SubscribePartNotifications event from a customer.
        /// </summary>
        public event EventHandler<MapRequestEventArgs<SubscribePartNotifications, string>> OnSubscribePartNotifications;

        /// <summary>
        /// Sends a SubscribePartNotificationsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<SubscribePartNotificationsResponse> SubscribePartNotificationsResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new SubscribePartNotificationsResponse
            {
                Success = success ?? new Dictionary<string, string>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

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
        public virtual EtpMessage<SubscribePartNotificationsResponse> SubscribePartNotificationsResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
        {
            return SendMapResponse(SubscribePartNotificationsResponse, correlatedHeader, success, errors, setFinalPart: setFinalPart, responseExtension: responseExtension, exceptionExtension: exceptionExtension);
        }

        /// <summary>
        /// Sends an UnsolicitedPartNotifications message to a customer.
        /// </summary>
        /// <param name="subscriptions">The unsolicited subscriptions.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<UnsolicitedPartNotifications> UnsolicitedPartNotifications(IList<SubscriptionInfo> subscriptions, IMessageHeaderExtension extension = null)
        {
            var body = new UnsolicitedPartNotifications
            {
                Subscriptions = subscriptions ?? new List<SubscriptionInfo>(),
            };

            var message = SendNotification(body, extension: extension, onBeforeSend: (m) => TryRegisterSubscriptions(m, subscriptions.ToMap(), nameof(SubscriptionInfo.RequestUuid)));

            if (message == null)
            {
                foreach (var subscription in body.Subscriptions)
                    TryUnregisterSubscription(subscription);
            }

            return message;
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
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PartsChanged> PartsChanged(Guid requestUuid, string uri, IList<ObjectPart> parts, ObjectChangeKind changeKind, DateTime changeTime, string format = Formats.Xml, IMessageHeaderExtension extension = null)
        {
            var body = new PartsChanged
            {
                RequestUuid = requestUuid,
                Uri = uri ?? string.Empty,
                Parts = parts ?? new List<ObjectPart>(),
                ChangeKind = changeKind,
                Format = format ?? Formats.Xml,
                ChangeTime = changeTime,
            };

            return SendNotification(body, extension: extension);
        }

        /// <summary>
        /// Sends a PartsDeleted message to a customer.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="uri">The URI of the growing object.</param>
        /// <param name="uids">The UIDs of the deleted parts.</param>
        /// <param name="changeTime">The change time.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PartsDeleted> PartsDeleted(Guid requestUuid, string uri, IList<string> uids, DateTime changeTime, IMessageHeaderExtension extension = null)
        {
            var body = new PartsDeleted
            {
                RequestUuid = requestUuid,
                Uri = uri ?? string.Empty,
                Uids = uids ?? new List<string>(),
                ChangeTime = changeTime,
            };

            return SendNotification(body, extension: extension);
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
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PartsReplacedByRange> PartsReplacedByRange(Guid requestUuid, string uri, IndexInterval deletedInterval, bool includeOverlappingIntervals, IList<ObjectPart> parts, DateTime changeTime, string format = Formats.Xml, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new PartsReplacedByRange
            {
                RequestUuid = requestUuid,
                Uri = uri ?? string.Empty,
                DeletedInterval = deletedInterval,
                IncludeOverlappingIntervals = includeOverlappingIntervals,
                Parts = parts ?? new List<ObjectPart>(),
                Format = format ?? Formats.Xml,
                ChangeTime = changeTime,
            };

            return SendNotification(body, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Handles the UnsubscribePartNotification event from a customer.
        /// </summary>
        public event EventHandler<RequestWithContextEventArgs<UnsubscribePartNotification, Guid, PartSubscriptionEndedReason>> OnUnsubscribePartNotification;

        /// <summary>
        /// Sends a PartSubscriptionEnded message to a customer in response to a UnsubscribePartNotification message.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="requestUuid">The reqyest UUId.</param>
        /// <param name="reason">The human readable reason why the part subscription ended.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PartSubscriptionEnded> ResponsePartSubscriptionEnded(IMessageHeader correlatedHeader, Guid requestUuid, string reason, IMessageHeaderExtension extension = null)
        {
            var body = new PartSubscriptionEnded
            {
                RequestUuid = requestUuid,
                Reason = reason ?? string.Empty,
            };

            return SendResponse(body, correlatedHeader, extension: extension);
        }

        /// <summary>
        /// Sends a PartSubscriptionEnded message to a customer as a notification.
        /// </summary>
        /// <param name="requestUuid">The reqyest UUId.</param>
        /// <param name="reason">The human readable reason why the part subscription ended.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PartSubscriptionEnded> NotificationPartSubscriptionEnded(Guid requestUuid, string reason, IMessageHeaderExtension extension = null)
        {
            var body = new PartSubscriptionEnded
            {
                RequestUuid = requestUuid,
                Reason = reason ?? string.Empty,
            };

            return SendNotification(body, extension: extension);
        }

        /// <summary>
        /// Handles the SubscribePartNotifications message from a customer.
        /// </summary>
        /// <param name="message">The SubscribePartNotifications message.</param>
        protected virtual void HandleSubscribePartNotifications(EtpMessage<SubscribePartNotifications> message)
        {
            var errors = TryRegisterSubscriptions(message, message.Body.Request, nameof(message.Body.Request));

            HandleRequestMessage(message, OnSubscribePartNotifications, HandleSubscribePartNotifications,
                args: new MapRequestEventArgs<SubscribePartNotifications, string>(message) { ErrorMap = errors },
                responseMethod: (args) => SubscribePartNotificationsResponse(args.Request?.Header, args.ResponseMap, isFinalPart: !args.HasErrors, extension: args.ResponseMapExtension));
        }

        /// <summary>
        /// Handles the response to an SubscribePartNotifications message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="MapRequestEventArgs{SubscribePartNotifications, string}"/> instance containing the event data.</param>
        protected virtual void HandleSubscribePartNotifications(MapRequestEventArgs<SubscribePartNotifications, string> args)
        {
        }

        /// <summary>
        /// Handles the UnsubscribePartNotification message from a customer.
        /// </summary>
        /// <param name="message">The UnsubscribePartNotification message.</param>
        protected virtual void HandleUnsubscribePartNotification(EtpMessage<UnsubscribePartNotification> message)
        {
            var error = TryUnregisterSubscription(message.Body, nameof(message.Body.RequestUuid), message);

            HandleRequestMessage(message, OnUnsubscribePartNotification, HandleUnsubscribePartNotification,
                args: new RequestWithContextEventArgs<UnsubscribePartNotification, Guid, PartSubscriptionEndedReason>(message) { FinalError = error },
                responseMethod: (args) => { if (!args.HasErrors) { ResponsePartSubscriptionEnded(args.Request?.Header, args.Response, args.Context.Reason, extension: args.ResponseExtension); } });
        }

        /// <summary>
        /// Handles the response to a UnsubscribePartNotification message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="RequestWithContextEventArgs{UnsubscribePartNotification, Guid, PartSubscriptionEndedReason}"/> instance containing the event data.</param>
        protected virtual void HandleUnsubscribePartNotification(RequestWithContextEventArgs<UnsubscribePartNotification, Guid, PartSubscriptionEndedReason> args)
        {
        }
    }
}

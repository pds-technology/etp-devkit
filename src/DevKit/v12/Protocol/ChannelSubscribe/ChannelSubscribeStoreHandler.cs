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

using System;
using System.Collections.Generic;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v12.Datatypes;
using Energistics.Etp.v12.Datatypes.ChannelData;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.ChannelSubscribe
{
    /// <summary>
    /// Base implementation of the <see cref="IChannelSubscribeStore"/> interface.
    /// </summary>
    /// <seealso cref="Energistics.Etp.v12.Protocol.Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.ChannelSubscribe.IChannelSubscribeStore" />
    public class ChannelSubscribeStoreHandler : Etp12ProtocolHandler<CapabilitiesStore, ICapabilitiesStore, CapabilitiesCustomer, ICapabilitiesCustomer>, IChannelSubscribeStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelSubscribeStoreHandler"/> class.
        /// </summary>
        public ChannelSubscribeStoreHandler() : base((int)Protocols.ChannelSubscribe, Roles.Store, Roles.Customer)
        {
            RegisterMessageHandler<GetChannelMetadata>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.GetChannelMetadata, HandleGetChannelMetadata);
            RegisterMessageHandler<GetChangeAnnotations>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.GetChangeAnnotations, HandleGetChangeAnnotations);
            RegisterMessageHandler<SubscribeChannels>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.SubscribeChannels, HandleSubscribeChannels);
            RegisterMessageHandler<UnsubscribeChannels>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.UnsubscribeChannels, HandleUnsubscribeChannels);
            RegisterMessageHandler<GetRanges>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.GetRanges, HandleGetRanges);
            RegisterMessageHandler<CancelGetRanges>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.CancelGetRanges, HandleCancelGetRanges);
        }

        /// <summary>
        /// Handles the GetChannelMetadata event from a customer.
        /// </summary>
        public event EventHandler<MapRequestEventArgs<GetChannelMetadata, ChannelMetadataRecord>> OnGetChannelMetadata;

        /// <summary>
        /// Sends a GetChannelMetadataResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="metadata">The metadata.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetChannelMetadataResponse> GetChannelMetadataResponse(IMessageHeader correlatedHeader, IDictionary<string, ChannelMetadataRecord> metadata, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new GetChannelMetadataResponse
            {
                Metadata = metadata ?? new Dictionary<string, ChannelMetadataRecord>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a complete multi-part set of GetChannelMetadataResponse and ProtocolException messages to a customer.
        /// If there are no channel metadata, an empty ChannelMetadataRecord message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="metadata">The metadata.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the GetChannelMetadataResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetChannelMetadataResponse> GetChannelMetadataResponse(IMessageHeader correlatedHeader, IDictionary<string, ChannelMetadataRecord> metadata, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
        {
            return SendMapResponse(GetChannelMetadataResponse, correlatedHeader, metadata, errors, setFinalPart: setFinalPart, responseExtension: responseExtension, exceptionExtension: exceptionExtension);
        }

        /// <summary>
        /// Handles the GetChangeAnnotations event from a customer.
        /// </summary>
        public event EventHandler<MapRequestEventArgs<GetChangeAnnotations, ChangeResponseInfo>> OnGetChangeAnnotations;

        /// <summary>
        /// Sends a GetChangeAnnotationsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="changes">The changes.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetChangeAnnotationsResponse> GetChangeAnnotationsResponse(IMessageHeader correlatedHeader, IDictionary<string, ChangeResponseInfo> changes, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new GetChangeAnnotationsResponse
            {
                Changes = changes ?? new Dictionary<string, ChangeResponseInfo>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a complete multi-part set of GetChangeAnnotationsResponse and ProtocolException messages to a customer.
        /// If there are no changes, an empty ChangeAnnotationsRecord message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="changes">The changes.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the GetChangeAnnotationsResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetChangeAnnotationsResponse> GetChangeAnnotationsResponse(IMessageHeader correlatedHeader, IDictionary<string, ChangeResponseInfo> changes, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
        {
            return SendMapResponse(GetChangeAnnotationsResponse, correlatedHeader, changes, errors, setFinalPart: setFinalPart, responseExtension: responseExtension, exceptionExtension: exceptionExtension);
        }

        /// <summary>
        /// Handles the SubscribeChannels event from a customer.
        /// </summary>
        public event EventHandler<MapRequestEventArgs<SubscribeChannels, string>> OnSubscribeChannels;

        /// <summary>
        /// Sends a SubscribeChannelsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<SubscribeChannelsResponse> SubscribeChannelsResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new SubscribeChannelsResponse
            {
                Success = success ?? new Dictionary<string, string>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a complete multi-part set of SubscribeChannelsResponse and ProtocolException messages to a customer.
        /// If there are no successes, an empty SubscribeChannelsRecord message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the SubscribeChannelsResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<SubscribeChannelsResponse> SubscribeChannelsResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
        {
            return SendMapResponse(SubscribeChannelsResponse, correlatedHeader, success, errors, setFinalPart: setFinalPart, responseExtension: responseExtension, exceptionExtension: exceptionExtension);
        }

        /// <summary>
        /// Sends a ChannelData message to a customer.
        /// </summary>
        /// <param name="data">The list of <see cref="DataItem" /> objects.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ChannelData> ChannelData(IList<DataItem> data, IMessageHeaderExtension extension = null)
        {
            var body = new ChannelData
            {
                Data = data ?? new List<DataItem>(),
            };

            return SendData(body, extension: extension);
        }

        /// <summary>
        /// Sends a ChannelsTruncated message to a customer.
        /// </summary>
        /// <param name="changeTime">The time of the change.</param>
        /// <param name="channels">The list of <see cref="TruncateInfo" /> objects.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ChannelsTruncated> ChannelsTruncated(DateTime changeTime, IList<TruncateInfo> channels, IMessageHeaderExtension extension = null)
        {
            var body = new ChannelsTruncated
            {
                ChangeTime = changeTime,
                Channels = channels ?? new List<TruncateInfo>(),
            };

            return SendNotification(body, extension: extension);
        }

        /// <summary>
        /// Sends a RangeReplaced message to a customer.
        /// </summary>
        /// <param name="changeTime">The time of the change.</param>
        /// <param name="channelIds">The IDs of the channels that are changing.</param>
        /// <param name="changedInterval">The indexes that define the interval that is changing.</param>
        /// <param name="data">The channel data of the changed interval.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<RangeReplaced> RangeReplaced(DateTime changeTime, IList<long> channelIds, IndexInterval changedInterval, IList<DataItem> data, bool isFinalPart = true, IMessageHeader correlatedHeader = null, IMessageHeaderExtension extension = null)
        {
            var body = new RangeReplaced
            {
                ChangeTime = changeTime,
                ChangedInterval = changedInterval,
                ChannelIds = channelIds ?? new List<long>(),
                Data = data ?? new List<DataItem>(),
            };

            return SendNotification(body, extension: extension, isMultiPart: true, correlatedHeader: correlatedHeader, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Handles the UnsubscribeChannels event from a customer.
        /// </summary>
        public event EventHandler<MapRequestWithContextEventArgs<UnsubscribeChannels, long, SubscriptionsStoppedReason>> OnUnsubscribeChannels;

        /// <summary>
        /// Sends a SubscriptionsStopped message to a customer in response to a UnsubscribeChannels message.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="channelIds">The channel IDs.</param>
        /// <param name="reason">The human readable reason why the subscriptions were stopped.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<SubscriptionsStopped> ResponseSubscriptionsStopped(IMessageHeader correlatedHeader, IDictionary<string, long> channelIds, string reason, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new SubscriptionsStopped
            {
                ChannelIds = channelIds ?? new Dictionary<string, long>(),
                Reason = reason ?? string.Empty,
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a complete multi-part set of SubscriptionsStopped and ProtocolException messages to a customer in response to a UnsubscribeChannels message.
        /// If there are no closed channels, an empty SubscriptionsStopped message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="channelIds">The channel IDs.</param>
        /// <param name="reason">The human readable reason why the subscriptions were stopped.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the SubscriptionsStopped message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<SubscriptionsStopped> ResponseSubscriptionsStopped(IMessageHeader correlatedHeader, IDictionary<string, long> channelIds, string reason, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
        {
            return SendMapResponse(ResponseSubscriptionsStopped, correlatedHeader, channelIds, reason, errors, setFinalPart: setFinalPart, responseExtension: responseExtension, exceptionExtension: exceptionExtension);
        }

        /// <summary>
        /// Sends a SubscriptionsStopped message to a customer as a notification.
        /// </summary>
        /// <param name="channelIds">The IDs of the channels for which subscriptions were stopped.</param>
        /// <param name="reason">The human readable reason why the subscriptions were stopped.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<SubscriptionsStopped> NotificationSubscriptionsStopped(IDictionary<string, long> channelIds, string reason, IMessageHeaderExtension extension = null)
        {
            var body = new SubscriptionsStopped
            {
                ChannelIds = channelIds ?? new Dictionary<string, long>(),
                Reason = reason ?? string.Empty,
            };

            return SendNotification(body, extension: extension, isMultiPart: true, isFinalPart: true);
        }

        /// <summary>
        /// Sends a SubscriptionsStopped message to a customer as a notification.
        /// </summary>
        /// <param name="channelIds">The IDs of the channels for which subscriptions were stopped.</param>
        /// <param name="reason">The human readable reason why the subscriptions were stopped.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<SubscriptionsStopped> NotificationSubscriptionsStopped(IList<long> channelIds, string reason, IMessageHeaderExtension extension = null) => NotificationSubscriptionsStopped(channelIds.ToMap(), reason, extension: extension);

        /// <summary>
        /// Handles the GetRanges event from a customer.
        /// </summary>
        public event EventHandler<ListRequestEventArgs<GetRanges, DataItem>> OnGetRanges;

        /// <summary>
        /// Sends a GetRangesResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="requestUuid">The request UUID associated with this response.</param>
        /// <param name="data">The data items.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="unregisterRequest">Whether or not to unregister the request when sending the message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetRangesResponse> GetRangesResponse(IMessageHeader correlatedHeader, Guid requestUuid, IList<DataItem> data, bool isFinalPart = true, bool unregisterRequest = true, IMessageHeaderExtension extension = null)
        {
            var body = new GetRangesResponse
            {
                Data = data ?? new List<DataItem>(),
            };

            var message = SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);

            if (unregisterRequest)
                TryUnregisterRequest(requestUuid);

            return message;
        }

        /// <summary>
        /// Handles the CancelGetRanges event from a customer.
        /// </summary>
        public event EventHandler<CancellationRequestEventArgs<GetRanges, CancelGetRanges>> OnCancelGetRanges;

        /// <summary>
        /// Handles the GetChannelMetadata message from a customer.
        /// </summary>
        /// <param name="message">The GetChannelMetadata message.</param>
        protected virtual void HandleGetChannelMetadata(EtpMessage<GetChannelMetadata> message)
        {
            HandleRequestMessage(message, OnGetChannelMetadata, HandleGetChannelMetadata,
                responseMethod: (args) => GetChannelMetadataResponse(args.Request?.Header, args.ResponseMap, isFinalPart: !args.HasErrors, extension: args.ResponseMapExtension));
        }

        /// <summary>
        /// Handles the response to an GetChannelMetadata message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="MapRequestEventArgs{GetChannelMetadata, ChannelMetadataRecord}"/> instance containing the event data.</param>
        protected virtual void HandleGetChannelMetadata(MapRequestEventArgs<GetChannelMetadata, ChannelMetadataRecord> args)
        {
        }

        /// <summary>
        /// Handles the GetChangeAnnotations message from a customer.
        /// </summary>
        /// <param name="message">The GetChangeAnnotations message.</param>
        protected virtual void HandleGetChangeAnnotations(EtpMessage<GetChangeAnnotations> message)
        {
            HandleRequestMessage(message, OnGetChangeAnnotations, HandleGetChangeAnnotations,
                responseMethod: (args) => GetChangeAnnotationsResponse(args.Request?.Header, args.ResponseMap, isFinalPart: !args.HasErrors, extension: args.ResponseMapExtension));
        }

        /// <summary>
        /// Handles the response to an GetChangeAnnotations message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="MapRequestEventArgs{GetChangeAnnotations, ChangeResponseInfo}"/> instance containing the event data.</param>
        protected virtual void HandleGetChangeAnnotations(MapRequestEventArgs<GetChangeAnnotations, ChangeResponseInfo> args)
        {
        }

        /// <summary>
        /// Handles the SubscribeChannels message from a customer.
        /// </summary>
        /// <param name="message">The SubscribeChannels message.</param>
        protected virtual void HandleSubscribeChannels(EtpMessage<SubscribeChannels> message)
        {
            HandleRequestMessage(message, OnSubscribeChannels, HandleSubscribeChannels,
                responseMethod: (args) => SubscribeChannelsResponse(args.Request?.Header, args.ResponseMap, isFinalPart: !args.HasErrors, extension: args.ResponseMapExtension));
        }

        /// <summary>
        /// Handles the response to an SubscribeChannels message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="MapRequestEventArgs{SubscribeChannels, string}"/> instance containing the event data.</param>
        protected virtual void HandleSubscribeChannels(MapRequestEventArgs<SubscribeChannels, string> args)
        {
        }

        /// <summary>
        /// Handles the UnsubscribeChannels message from a customer.
        /// </summary>
        /// <param name="message">The UnsubscribeChannels message.</param>
        protected virtual void HandleUnsubscribeChannels(EtpMessage<UnsubscribeChannels> message)
        {
            HandleRequestMessage(message, OnUnsubscribeChannels, HandleUnsubscribeChannels,
                responseMethod: (args) => ResponseSubscriptionsStopped(args.Request?.Header, args.ResponseMap, args.Context.Reason, isFinalPart: !args.HasErrors, extension: args.ResponseMapExtension));
        }

        /// <summary>
        /// Handles the response to a UnsubscribeChannels message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="MapRequestWithContextEventArgs{UnsubscribeChannels, long, SubscriptionsStoppedReason}"/> instance containing the event data.</param>
        protected virtual void HandleUnsubscribeChannels(MapRequestWithContextEventArgs<UnsubscribeChannels, long, SubscriptionsStoppedReason> args)
        {
        }

        /// <summary>
        /// Handles the GetRanges message from a customer.
        /// </summary>
        /// <param name="message">The GetRanges message.</param>
        protected virtual void HandleGetRanges(EtpMessage<GetRanges> message)
        {
            var error = TryRegisterRequest(message.Body, nameof(message.Body.RequestUuid), message);

            HandleRequestMessage(message, OnGetRanges, HandleGetRanges,
                args: new ListRequestEventArgs<GetRanges, DataItem>(message) { FinalError = error },
                responseMethod: (args) => GetRangesResponse(args.Request?.Header, message.Body.RequestUuid, args.Responses, isFinalPart: !args.HasErrors, unregisterRequest: true, extension: args.ResponseExtension));
        }

        /// <summary>
        /// Handles the GetRanges message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="ListRequestEventArgs{GetRanges, DataItem}"/> instance containing the event data.</param>
        protected virtual void HandleGetRanges(ListRequestEventArgs<GetRanges, DataItem> args)
        {
        }

        /// <summary>
        /// Handles the CancelGetRanges message from a customer.
        /// </summary>
        /// <param name="message">The CancelGetRanges message.</param>
        protected virtual void HandleCancelGetRanges(EtpMessage<CancelGetRanges> message)
        {
            EtpMessage<GetRanges> request;
            var error = TryGetRequest(message.Body, nameof(message.Body.RequestUuid), message, out request);

            HandleCancellationMessage(request, message, OnCancelGetRanges, HandleCancelGetRanges,
                args: new CancellationRequestEventArgs<GetRanges, CancelGetRanges>(request, message) { FinalError = error });
        }

        /// <summary>
        /// Handles the CancelGetRanges message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="CancellationRequestEventArgs{GetRanges, CancelGetRanges}"/> instance containing the event data.</param>
        protected virtual void HandleCancelGetRanges(CancellationRequestEventArgs<GetRanges, CancelGetRanges> args)
        {
        }
    }
}

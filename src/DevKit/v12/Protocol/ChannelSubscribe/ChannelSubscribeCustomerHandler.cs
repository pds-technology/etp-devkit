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
using Energistics.Etp.Common.Protocol.Core;
using Energistics.Etp.v12.Datatypes;
using Energistics.Etp.v12.Datatypes.ChannelData;

namespace Energistics.Etp.v12.Protocol.ChannelSubscribe
{
    /// <summary>
    /// Base implementation of the <see cref="IChannelSubscribeCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Energistics.Etp.v12.Protocol.Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.ChannelSubscribe.IChannelSubscribeCustomer" />
    public class ChannelSubscribeCustomerHandler : Etp12ProtocolHandler<CapabilitiesCustomer, ICapabilitiesCustomer, CapabilitiesStore, ICapabilitiesStore>, IChannelSubscribeCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelSubscribeCustomerHandler"/> class.
        /// </summary>
        public ChannelSubscribeCustomerHandler() : base((int)Protocols.ChannelSubscribe, Roles.Customer, Roles.Store)
        {
            ChannelMetadataRecords = new List<ChannelMetadataRecord>(0);

            RegisterMessageHandler<GetChannelMetadataResponse>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.GetChannelMetadataResponse, HandleGetChannelMetadataResponse);
            RegisterMessageHandler<GetChangeAnnotationsResponse>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.GetChangeAnnotationsResponse, HandleGetChangeAnnotationsResponse);
            RegisterMessageHandler<SubscribeChannelsResponse>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.SubscribeChannelsResponse, HandleSubscribeChannelsResponse);
            RegisterMessageHandler<ChannelData>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.ChannelData, HandleChannelData);
            RegisterMessageHandler<ChannelsTruncated>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.ChannelsTruncated, HandleChannelsTruncated);
            RegisterMessageHandler<RangeReplaced>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.RangeReplaced, HandleRangeReplaced);
            RegisterMessageHandler<SubscriptionsStopped>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.SubscriptionsStopped, HandleSubscriptionsStopped);
            RegisterMessageHandler<GetRangesResponse>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.GetRangesResponse, HandleGetRangesResponse);
        }

        /// <summary>
        /// Gets the list of <see cref="ChannelMetadataRecords"/> objects returned by the store.
        /// </summary>
        /// <value>The list of <see cref="ChannelMetadataRecords"/> objects.</value>
        protected IList<ChannelMetadataRecord> ChannelMetadataRecords { get; }

        /// <summary>
        /// Sends a GetChannelMetadata message to a store with the specified URIs.
        /// </summary>
        /// <param name="uris">The list of URIs.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetChannelMetadata> GetChannelMetadata(IDictionary<string, string> uris, IMessageHeaderExtension extension = null)
        {
            var body = new GetChannelMetadata
            {
                Uris = uris ?? new Dictionary<string, string>(),
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Sends a GetChannelMetadata message to a store with the specified URIs.
        /// </summary>
        /// <param name="uris">The list of URIs.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetChannelMetadata> GetChannelMetadata(IList<string> uris, IMessageHeaderExtension extension = null) => GetChannelMetadata(uris.ToMap(), extension: extension);

        /// <summary>
        /// Handles the GetChannelMetadataResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<GetChannelMetadata, GetChannelMetadataResponse>> OnGetChannelMetadataResponse;

        /// <summary>
        /// Sends a GetChangeAnnotations message to a store.
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <param name="latestOnly">Whether or not to only get the latest change annotation for each channel.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetChangeAnnotations> GetChangeAnnotations(IDictionary<string, ChannelChangeRequestInfo> channels, bool latestOnly = false, IMessageHeaderExtension extension = null)
        {
            var body = new GetChangeAnnotations
            {
                Channels = channels ?? new Dictionary<string, ChannelChangeRequestInfo>(),
                LatestOnly = latestOnly,
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Sends a GetChangeAnnotations message to a store.
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <param name="latestOnly">Whether or not to only get the latest change annotation for each channel.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetChangeAnnotations> GetChangeAnnotations(IList<ChannelChangeRequestInfo> channels, bool latestOnly = false, IMessageHeaderExtension extension = null) => GetChangeAnnotations(channels.ToMap(), latestOnly: latestOnly, extension: extension);

        /// <summary>
        /// Handles the GetChangeAnnotationsResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<GetChangeAnnotations, GetChangeAnnotationsResponse>> OnGetChangeAnnotationsResponse;

        /// <summary>
        /// Sends a SubscribeChannels message to a store.
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<SubscribeChannels> SubscribeChannels(IDictionary<string, ChannelSubscribeInfo> channels, IMessageHeaderExtension extension = null)
        {
            var body = new SubscribeChannels
            {
                Channels = channels ?? new Dictionary<string, ChannelSubscribeInfo>(),
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Sends a SubscribeChannels message to a store.
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<SubscribeChannels> SubscribeChannels(IList<ChannelSubscribeInfo> channels, IMessageHeaderExtension extension = null) => SubscribeChannels(channels.ToMap(), extension: extension);

        /// <summary>
        /// Handles the SubscribeChannelsResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<SubscribeChannels, SubscribeChannelsResponse>> OnSubscribeChannelsResponse;

        /// <summary>
        /// Handles the ChannelData event from a store when not sent in response to a request.
        /// </summary>
        public event EventHandler<FireAndForgetEventArgs<ChannelData>> OnChannelData;

        /// <summary>
        /// Handles the ChannelsTruncated event from a store when not sent in response to a request.
        /// </summary>
        public event EventHandler<FireAndForgetEventArgs<ChannelsTruncated>> OnChannelsTruncated;

        /// <summary>
        /// Handles the RangeReplaced event from a store when not sent in response to a request.
        /// </summary>
        public event EventHandler<FireAndForgetEventArgs<RangeReplaced>> OnRangeReplaced;

        /// <summary>
        /// Sends a UnsubscribeChannels message to a store.
        /// </summary>
        /// <param name="channelIds">The channel IDs.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<UnsubscribeChannels> UnsubscribeChannels(IDictionary<string, long> channelIds, IMessageHeaderExtension extension = null)
        {
            var body = new UnsubscribeChannels
            {
                ChannelIds = channelIds ?? new Dictionary<string, long>(),
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Sends a UnsubscribeChannels message to a store.
        /// </summary>
        /// <param name="channelIds">The channel IDs.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<UnsubscribeChannels> UnsubscribeChannels(IList<long> channelIds, IMessageHeaderExtension extension = null) => UnsubscribeChannels(channelIds.ToMap(), extension: extension);

        /// <summary>
        /// Handles the SubscriptionsStopped event from a store when sent in response to a UnsubscribeChannels.
        /// </summary>
        public event EventHandler<ResponseEventArgs<UnsubscribeChannels, SubscriptionsStopped>> OnResponseSubscriptionsStopped;

        /// <summary>
        /// Handles the SubscriptionsStopped event from a store when not sent in response to a request.
        /// </summary>
        public event EventHandler<FireAndForgetEventArgs<SubscriptionsStopped>> OnNotificationSubscriptionsStopped;

        /// <summary>
        /// Sends a GetRanges message to a store.
        /// </summary>
        /// <param name="channelRanges">The channel ranges.</param>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetRanges> GetRanges(IList<ChannelRangeInfo> channelRanges, Guid requestUuid, IMessageHeaderExtension extension = null)
        {
            var body = new GetRanges
            {
                ChannelRanges = channelRanges ?? new List<ChannelRangeInfo>(),
                RequestUuid = requestUuid.ToUuid<Uuid>()
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Handles the GetRangesResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<GetRanges, GetRangesResponse>> OnGetRangesResponse;

        /// <summary>
        /// Sends a CancelGetRanges message to a store.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<CancelGetRanges> CancelGetRanges(Guid requestUuid, IMessageHeaderExtension extension = null)
        {
            var body = new CancelGetRanges()
            {
                RequestUuid = requestUuid.ToUuid<Uuid>(),
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Event raised when there is an exception received in response to a CancelGetRanges message.
        /// </summary>
        public event EventHandler<VoidResponseEventArgs<CancelGetRanges>> OnCancelGetRangesException;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<GetChannelMetadata>)
                HandleResponseMessage(request as EtpMessage<GetChannelMetadata>, message, OnGetChannelMetadataResponse, HandleGetChannelMetadataResponse);
            else if (request is EtpMessage<GetChangeAnnotations>)
                HandleResponseMessage(request as EtpMessage<GetChangeAnnotations>, message, OnGetChangeAnnotationsResponse, HandleGetChangeAnnotationsResponse);
            else if (request is EtpMessage<SubscribeChannels>)
                HandleResponseMessage(request as EtpMessage<SubscribeChannels>, message, OnSubscribeChannelsResponse, HandleSubscribeChannelsResponse);
            else if (request is EtpMessage<UnsubscribeChannels>)
                HandleResponseMessage(request as EtpMessage<UnsubscribeChannels>, message, OnResponseSubscriptionsStopped, HandleResponseSubscriptionsStopped);
            else if (request is EtpMessage<GetRanges>)
                HandleResponseMessage(request as EtpMessage<GetRanges>, message, OnGetRangesResponse, HandleGetRangesResponse);
            else if (request is EtpMessage<CancelGetRanges>)
                HandleResponseMessage(request as EtpMessage<CancelGetRanges>, message, OnCancelGetRangesException, HandleCancelGetRangesException);
        }

        /// <summary>
        /// Handles the GetChannelMetadataResponse message from a store.
        /// </summary>
        /// <param name="message">The GetChannelMetadataResponse message.</param>
        protected virtual void HandleGetChannelMetadataResponse(EtpMessage<GetChannelMetadataResponse> message)
        {
            foreach (var channel in message.Body.Metadata.Values)
                ChannelMetadataRecords.Add(channel);

            var request = TryGetCorrelatedMessage<GetChannelMetadata>(message);
            HandleResponseMessage(request, message, OnGetChannelMetadataResponse, HandleGetChannelMetadataResponse);
        }

        /// <summary>
        /// Handles the GetChannelMetadataResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{GetChannelMetadata, GetChannelMetadataResponse}"/> instance containing the event data.</param>
        protected virtual void HandleGetChannelMetadataResponse(ResponseEventArgs<GetChannelMetadata, GetChannelMetadataResponse> args)
        {
        }

        /// <summary>
        /// Handles the GetChangeAnnotationsResponse message from a store.
        /// </summary>
        /// <param name="message">The GetChangeAnnotationsResponse message.</param>
        protected virtual void HandleGetChangeAnnotationsResponse(EtpMessage<GetChangeAnnotationsResponse> message)
        {
            var request = TryGetCorrelatedMessage<GetChangeAnnotations>(message);
            HandleResponseMessage(request, message, OnGetChangeAnnotationsResponse, HandleGetChangeAnnotationsResponse);
        }

        /// <summary>
        /// Handles the GetChangeAnnotationsResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{GetChangeAnnotations, GetChangeAnnotationsResponse}"/> instance containing the event data.</param>
        protected virtual void HandleGetChangeAnnotationsResponse(ResponseEventArgs<GetChangeAnnotations, GetChangeAnnotationsResponse> args)
        {
        }

        /// <summary>
        /// Handles the SubscribeChannelsResponse message from a store.
        /// </summary>
        /// <param name="message">The SubscribeChannelsResponse message.</param>
        protected virtual void HandleSubscribeChannelsResponse(EtpMessage<SubscribeChannelsResponse> message)
        {
            var request = TryGetCorrelatedMessage<SubscribeChannels>(message);
            HandleResponseMessage(request, message, OnSubscribeChannelsResponse, HandleSubscribeChannelsResponse);
        }

        /// <summary>
        /// Handles the SubscribeChannelsResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{SubscribeChannels, SubscribeChannelsResponse}"/> instance containing the event data.</param>
        protected virtual void HandleSubscribeChannelsResponse(ResponseEventArgs<SubscribeChannels, SubscribeChannelsResponse> args)
        {
        }

        /// <summary>
        /// Handles the ChannelData message from a store.
        /// </summary>
        /// <param name="message">The ChannelData message.</param>
        protected virtual void HandleChannelData(EtpMessage<ChannelData> message)
        {
            HandleFireAndForgetMessage(message, OnChannelData, HandleChannelData);
        }

        /// <summary>
        /// Handles the ChannelData message from a store.
        /// </summary>
        /// <param name="args">The <see cref="FireAndForgetEventArgs{ChannelData}"/> instance containing the event data.</param>
        protected virtual void HandleChannelData(FireAndForgetEventArgs<ChannelData> args)
        {
        }

        /// <summary>
        /// Handles the ChannelsTruncated message from a store.
        /// </summary>
        /// <param name="message">The ChannelsTruncated message.</param>
        protected virtual void HandleChannelsTruncated(EtpMessage<ChannelsTruncated> message)
        {
            HandleFireAndForgetMessage(message, OnChannelsTruncated, HandleChannelsTruncated);
        }

        /// <summary>
        /// Handles the ChannelsTruncated message from a store.
        /// </summary>
        /// <param name="args">The <see cref="FireAndForgetEventArgs{ChannelsTruncated}"/> instance containing the event data.</param>
        protected virtual void HandleChannelsTruncated(FireAndForgetEventArgs<ChannelsTruncated> args)
        {
        }

        /// <summary>
        /// Handles the RangeReplaced message from a store.
        /// </summary>
        /// <param name="message">The RangeReplaced message.</param>
        protected virtual void HandleRangeReplaced(EtpMessage<RangeReplaced> message)
        {
            HandleFireAndForgetMessage(message, OnRangeReplaced, HandleRangeReplaced);
        }

        /// <summary>
        /// Handles the RangeReplaced message from a store.
        /// </summary>
        /// <param name="args">The <see cref="FireAndForgetEventArgs{RangeReplaced}"/> instance containing the event data.</param>
        protected virtual void HandleRangeReplaced(FireAndForgetEventArgs<RangeReplaced> args)
        {
        }

        /// <summary>
        /// Handles the SubscriptionsStopped message from a store.
        /// </summary>
        /// <param name="message">The SubscriptionsStopped message.</param>
        protected virtual void HandleSubscriptionsStopped(EtpMessage<SubscriptionsStopped> message)
        {
            if (message.Header.CorrelationId == 0)
            {
                HandleFireAndForgetMessage(message, OnNotificationSubscriptionsStopped, HandleNotificationSubscriptionsStopped);
            }
            else
            {
                var request = TryGetCorrelatedMessage<UnsubscribeChannels>(message);
                HandleResponseMessage(request, message, OnResponseSubscriptionsStopped, HandleResponseSubscriptionsStopped);
            }
        }

        /// <summary>
        /// Handles the SubscriptionsStopped message from a store when sent as a notification.
        /// </summary>
        /// <param name="args">The <see cref="FireAndForgetEventArgs{SubscriptionsStopped}"/> instance containing the event data.</param>
        protected virtual void HandleNotificationSubscriptionsStopped(FireAndForgetEventArgs<SubscriptionsStopped> args)
        {
        }

        /// <summary>
        /// Handles the SubscriptionsStopped message from a store when sent in response to a UnsubscribeChannels message.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{UnsubscribeChannels, SubscriptionsStopped}"/> instance containing the event data.</param>
        protected virtual void HandleResponseSubscriptionsStopped(ResponseEventArgs<UnsubscribeChannels, SubscriptionsStopped> args)
        {
        }

        /// <summary>
        /// Handles the GetRangesResponse message from a store.
        /// </summary>
        /// <param name="message">The GetRangesResponse message.</param>
        protected virtual void HandleGetRangesResponse(EtpMessage<GetRangesResponse> message)
        {
            var request = TryGetCorrelatedMessage<GetRanges>(message);
            HandleResponseMessage(request, message, OnGetRangesResponse, HandleGetRangesResponse);
        }

        /// <summary>
        /// Handles the GetRangesResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{GetRanges, GetRangesResponse}"/> instance containing the event data.</param>
        protected virtual void HandleGetRangesResponse(ResponseEventArgs<GetRanges, GetRangesResponse> args)
        {
        }

        /// <summary>
        /// Handles exceptions to the CancelGetRanges message from a store.
        /// </summary>
        /// <param name="args">The <see cref="VoidResponseEventArgs{CancelGetRanges}"/> instance containing the event data.</param>
        protected virtual void HandleCancelGetRangesException(VoidResponseEventArgs<CancelGetRanges> args)
        {
        }
    }
}

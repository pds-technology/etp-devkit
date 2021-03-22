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
using Energistics.Etp.v12.Datatypes.ChannelData;

namespace Energistics.Etp.v12.Protocol.ChannelSubscribe
{
    /// <summary>
    /// Defines the interface that must be implemented by the customer role of the ChannelSubscribe protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.ChannelSubscribe, Roles.Customer, Roles.Store)]
    public interface IChannelSubscribeCustomer : IProtocolHandler<ICapabilitiesCustomer, ICapabilitiesStore>
    {
        /// <summary>
        /// Sends a GetChannelMetadata message to a store with the specified URIs.
        /// </summary>
        /// <param name="uris">The list of URIs.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetChannelMetadata> GetChannelMetadata(IDictionary<string, string> uris, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a GetChannelMetadata message to a store with the specified URIs.
        /// </summary>
        /// <param name="uris">The list of URIs.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetChannelMetadata> GetChannelMetadata(IList<string> uris, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the GetChannelMetadataResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<GetChannelMetadata, GetChannelMetadataResponse>> OnGetChannelMetadataResponse;

        /// <summary>
        /// Sends a GetChangeAnnotations message to a store.
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <param name="latestOnly">Whether or not to only get the latest change annotation for each channel.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetChangeAnnotations> GetChangeAnnotations(IDictionary<string, ChannelChangeRequestInfo> channels, bool latestOnly = false, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a GetChangeAnnotations message to a store.
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <param name="latestOnly">Whether or not to only get the latest change annotation for each channel.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetChangeAnnotations> GetChangeAnnotations(IList<ChannelChangeRequestInfo> channels, bool latestOnly = false, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the GetChangeAnnotationsResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<GetChangeAnnotations, GetChangeAnnotationsResponse>> OnGetChangeAnnotationsResponse;

        /// <summary>
        /// Sends a SubscribeChannels message to a store.
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<SubscribeChannels> SubscribeChannels(IDictionary<string, ChannelSubscribeInfo> channels, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a SubscribeChannels message to a store.
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<SubscribeChannels> SubscribeChannels(IList<ChannelSubscribeInfo> channels, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the SubscribeChannelsResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<SubscribeChannels, SubscribeChannelsResponse>> OnSubscribeChannelsResponse;

        /// <summary>
        /// Handles the ChannelData event from a store when not sent in response to a request.
        /// </summary>
        event EventHandler<FireAndForgetEventArgs<ChannelData>> OnChannelData;

        /// <summary>
        /// Handles the ChannelsTruncated event from a store when not sent in response to a request.
        /// </summary>
        event EventHandler<FireAndForgetEventArgs<ChannelsTruncated>> OnChannelsTruncated;

        /// <summary>
        /// Handles the RangeReplaced event from a store when not sent in response to a request.
        /// </summary>
        event EventHandler<FireAndForgetEventArgs<RangeReplaced>> OnRangeReplaced;

        /// <summary>
        /// Sends a UnsubscribeChannels message to a store.
        /// </summary>
        /// <param name="channelIds">The channel IDs.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<UnsubscribeChannels> UnsubscribeChannels(IDictionary<string, long> channelIds, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a UnsubscribeChannels message to a store.
        /// </summary>
        /// <param name="channelIds">The channel IDs.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<UnsubscribeChannels> UnsubscribeChannels(IList<long> channelIds, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the SubscriptionsStopped event from a store when sent in response to a UnsubscribeChannels.
        /// </summary>
        event EventHandler<ResponseEventArgs<UnsubscribeChannels, SubscriptionsStopped>> OnResponseSubscriptionsStopped;

        /// <summary>
        /// Handles the SubscriptionsStopped event from a store when not sent in response to a request.
        /// </summary>
        event EventHandler<FireAndForgetEventArgs<SubscriptionsStopped>> OnNotificationSubscriptionsStopped;

        /// <summary>
        /// Sends a GetRanges message to a store.
        /// </summary>
        /// <param name="channelRanges">The channel ranges.</param>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetRanges> GetRanges(IList<ChannelRangeInfo> channelRanges, Guid requestUuid, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the GetRangesResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<GetRanges, GetRangesResponse>> OnGetRangesResponse;

        /// <summary>
        /// Sends a CancelGetRanges message to a store.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<CancelGetRanges> CancelGetRanges(Guid requestUuid, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Event raised when there is an exception received in response to a CancelGetRanges message.
        /// </summary>
        event EventHandler<VoidResponseEventArgs<CancelGetRanges>> OnCancelGetRangesException;
    }
}

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
    /// Defines the interface that must be implemented by the store role of the ChannelSubscribe protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.ChannelSubscribe, Roles.Store, Roles.Customer)]
    public interface IChannelSubscribeStore : IProtocolHandler<ICapabilitiesStore, ICapabilitiesCustomer>
    {
        /// <summary>
        /// Handles the GetChannelMetadata event from a customer.
        /// </summary>
        event EventHandler<MapRequestEventArgs<GetChannelMetadata, ChannelMetadataRecord>> OnGetChannelMetadata;

        /// <summary>
        /// Sends a GetChannelMetadataResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="metadata">The metadata.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetChannelMetadataResponse> GetChannelMetadataResponse(IMessageHeader correlatedHeader, IDictionary<string, ChannelMetadataRecord> metadata, bool isFinalPart = true, IMessageHeaderExtension extension = null);

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
        EtpMessage<GetChannelMetadataResponse> GetChannelMetadataResponse(IMessageHeader correlatedHeader, IDictionary<string, ChannelMetadataRecord> metadata, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null);

        /// <summary>
        /// Handles the GetChangeAnnotations event from a customer.
        /// </summary>
        event EventHandler<MapRequestEventArgs<GetChangeAnnotations, ChannelChangeResponseInfo>> OnGetChangeAnnotations;

        /// <summary>
        /// Sends a GetChangeAnnotationsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="changes">The changes.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetChangeAnnotationsResponse> GetChangeAnnotationsResponse(IMessageHeader correlatedHeader, IDictionary<string, ChannelChangeResponseInfo> changes, bool isFinalPart = true, IMessageHeaderExtension extension = null);

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
        EtpMessage<GetChangeAnnotationsResponse> GetChangeAnnotationsResponse(IMessageHeader correlatedHeader, IDictionary<string, ChannelChangeResponseInfo> changes, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null);

        /// <summary>
        /// Handles the SubscribeChannels event from a customer.
        /// </summary>
        event EventHandler<MapRequestEventArgs<SubscribeChannels, string>> OnSubscribeChannels;

        /// <summary>
        /// Sends a SubscribeChannelsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<SubscribeChannelsResponse> SubscribeChannelsResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, bool isFinalPart = true, IMessageHeaderExtension extension = null);

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
        EtpMessage<SubscribeChannelsResponse> SubscribeChannelsResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null);

        /// <summary>
        /// Sends a ChannelData message to a customer.
        /// </summary>
        /// <param name="data">The list of <see cref="DataItem" /> objects.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ChannelData> ChannelData(IList<DataItem> data, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a ChannelsTruncated message to a customer.
        /// </summary>
        /// <param name="changeTime">The time of the change.</param>
        /// <param name="channels">The list of <see cref="TruncateInfo" /> objects.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ChannelsTruncated> ChannelsTruncated(long changeTime, IList<TruncateInfo> channels, IMessageHeaderExtension extension = null);

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
        EtpMessage<RangeReplaced> RangeReplaced(long changeTime, IList<long> channelIds, IndexInterval changedInterval, IList<DataItem> data, bool isFinalPart = true, IMessageHeader correlatedHeader = null, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the UnsubscribeChannels event from a customer.
        /// </summary>
        event EventHandler<MapRequestEventArgs<UnsubscribeChannels, long>> OnUnsubscribeChannels;

        /// <summary>
        /// Sends a SubscriptionsStopped message to a customer in response to a UnsubscribeChannels message.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="channelIds">The channel IDs.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<SubscriptionsStopped> ResponseSubscriptionsStopped(IMessageHeader correlatedHeader, IDictionary<string, long> channelIds, bool isFinalPart = true, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a complete multi-part set of SubscriptionsStopped and ProtocolException messages to a customer in response to a UnsubscribeChannels message.
        /// If there are no closed channels, an empty SubscriptionsStopped message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="channelIds">The channel IDs.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the SubscriptionsStopped message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        EtpMessage<SubscriptionsStopped> ResponseSubscriptionsStopped(IMessageHeader correlatedHeader, IDictionary<string, long> channelIds, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null);

        /// <summary>
        /// Sends a SubscriptionsStopped message to a customer as a notification.
        /// </summary>
        /// <param name="channelIds">The IDs of the closed channels.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<SubscriptionsStopped> NotificationSubscriptionsStopped(IDictionary<string, long> channelIds, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a SubscriptionsStopped message to a customer as a notification.
        /// </summary>
        /// <param name="channelIds">The IDs of the closed channels.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<SubscriptionsStopped> NotificationSubscriptionsStopped(IList<long> channelIds, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the GetRanges event from a customer.
        /// </summary>
        event EventHandler<ListRequestEventArgs<GetRanges, DataItem>> OnGetRanges;

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
        EtpMessage<GetRangesResponse> GetRangesResponse(IMessageHeader correlatedHeader, Guid requestUuid, IList<DataItem> data, bool isFinalPart = true, bool unregisterRequest = true, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the CancelGetRanges event from a customer.
        /// </summary>
        event EventHandler<CancellationRequestEventArgs<GetRanges, CancelGetRanges>> OnCancelGetRanges;
    }
}

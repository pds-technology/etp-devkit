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

namespace Energistics.Etp.v12.Protocol.ChannelDataLoad
{
    /// <summary>
    /// Describes the interface that must be implemented by the store role of the ChannelDataLoad protocol.
    /// </summary>
    /// <seealso cref="IProtocolHandler" />
    [ProtocolRole((int)Protocols.ChannelDataLoad, Roles.Store, Roles.Customer)]
    public interface IChannelDataLoadStore : IProtocolHandler<ICapabilitiesStore, ICapabilitiesCustomer>
    {
        /// <summary>
        /// Handles the OpenChannels event from a customer.
        /// </summary>
        event EventHandler<MapRequestEventArgs<OpenChannels, OpenChannelInfo>> OnOpenChannels;

        /// <summary>
        /// Sends a OpenChannelsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="channels">The channels.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<OpenChannelsResponse> OpenChannelsResponse(IMessageHeader correlatedHeader, IDictionary<string, OpenChannelInfo> channels, bool isFinalPart = true, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a complete multi-part set of OpenChannelsResponse and ProtocolException messages to a customer.
        /// If there are no opened channels, an empty OpenChannelsResponse message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="channels">The channels.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the OpenChannelsResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        EtpMessage<OpenChannelsResponse> OpenChannelsResponse(IMessageHeader correlatedHeader, IDictionary<string, OpenChannelInfo> channels, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null);

        /// <summary>
        /// Handles the ChannelData event from a customer.
        /// </summary>
        event EventHandler<FireAndForgetEventArgs<ChannelData>> OnChannelData;

        /// <summary>
        /// Handles the TruncateChannels event from a customer.
        /// </summary>
        event EventHandler<MapRequestEventArgs<TruncateChannels, DateTime>> OnTruncateChannels;

        /// <summary>
        /// Sends a TruncateChannelsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="channelsTruncatedTime">The times at which the channels were truncated.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<TruncateChannelsResponse> TruncateChannelsResponse(IMessageHeader correlatedHeader, IDictionary<string, DateTime> channelsTruncatedTime, bool isFinalPart = true, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a complete multi-part set of TruncateChannelsResponse and ProtocolException messages to a customer.
        /// If there are no truncated channels, an empty TruncateChannelsResponse message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="channelsTruncatedTime">The times at which the channels were truncated.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the OpenChannelsResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        EtpMessage<TruncateChannelsResponse> TruncateChannelsResponse(IMessageHeader correlatedHeader, IDictionary<string, DateTime> channelsTruncatedTime, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null);

        /// <summary>
        /// Handles the ReplaceRange event from a customer.
        /// </summary>
        event EventHandler<RequestEventArgs<ReplaceRange, DateTime>> OnReplaceRange;

        /// <summary>
        /// Sends a ReplaceRangeResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="channelChangeTime">The channel change time.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ReplaceRangeResponse> ReplaceRangeResponse(IMessageHeader correlatedHeader, DateTime channelChangeTime, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the CloseChannels event from a customer.
        /// </summary>
        event EventHandler<MapRequestWithContextEventArgs<CloseChannels, long, ChannelsClosedReason>> OnCloseChannels;

        /// <summary>
        /// Sends a ChannelsClosed message to a customer in response to a CloseChannels message.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="channelIds">The channel IDs.</param>
        /// <param name="reason">The human readable reason why the channels were closed.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ChannelsClosed> ResponseChannelsClosed(IMessageHeader correlatedHeader, IDictionary<string, long> channelIds, string reason, bool isFinalPart = true, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a complete multi-part set of ChannelsClosed and ProtocolException messages to a customer in response to a CloseChannels message.
        /// If there are no closed channels, an empty CloseChannels message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="channelIds">The channel IDs.</param>
        /// <param name="reason">The human readable reason why the channels were closed.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the ChannelsClosed message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        EtpMessage<ChannelsClosed> ResponseChannelsClosed(IMessageHeader correlatedHeader, IDictionary<string, long> channelIds, string reason, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null);

        /// <summary>
        /// Sends a ChannelsClosed message to a customer as a notification.
        /// </summary>
        /// <param name="channelIds">The IDs of the closed channels.</param>
        /// <param name="reason">The human readable reason why the channels were closed.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ChannelsClosed> NotificationChannelsClosed(IDictionary<string, long> channelIds, string reason, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a ChannelsClosed message to a customer as a notification.
        /// </summary>
        /// <param name="channelIds">The IDs of the closed channels.</param>
        /// <param name="reason">The human readable reason why the channels were closed.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ChannelsClosed> NotificationChannelsClosed(IList<long> channelIds, string reason, IMessageHeaderExtension extension = null);
    }
    public class ChannelsClosedReason
    {
        public string Reason { get; set; } = "Customer Request";
    }
}

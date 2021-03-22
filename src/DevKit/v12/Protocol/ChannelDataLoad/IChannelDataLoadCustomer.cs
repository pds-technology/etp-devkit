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
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.ChannelDataLoad
{
    /// <summary>
    /// Describes the interface that must be implemented by the customer role of the ChannelDataLoad protocol.
    /// </summary>
    /// <seealso cref="IProtocolHandler" />
    [ProtocolRole((int)Protocols.ChannelDataLoad, Roles.Customer, Roles.Store)]
    public interface IChannelDataLoadCustomer : IProtocolHandler<ICapabilitiesCustomer, ICapabilitiesStore>
    {
        /// <summary>
        /// Sends a OpenChannels message to a store.
        /// </summary>
        /// <param name="uris">The channel URIs.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<OpenChannels> OpenChannels(IDictionary<string, string> uris, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a OpenChannels message to a store.
        /// </summary>
        /// <param name="uris">The channel URIs.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<OpenChannels> OpenChannels(IList<string> uris, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the OpenChannelsResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<OpenChannels, OpenChannelsResponse>> OnOpenChannelsResponse;

        /// <summary>
        /// Sends a ChannelData message to a store.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ChannelData> ChannelData(IList<DataItem> data, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a TruncateChannels message to a store.
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<TruncateChannels> TruncateChannels(IDictionary<string, TruncateInfo> channels, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a TruncateChannels message to a store.
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<TruncateChannels> TruncateChannels(IList<TruncateInfo> channels, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the TruncateChannelsResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<TruncateChannels, TruncateChannelsResponse>> OnTruncateChannelsResponse;

        /// <summary>
        /// Sends a ReplaceRange message to a store.
        /// </summary>
        /// <param name="changedInterval">The changed interval.</param>
        /// <param name="channelIds">The channel IDs.</param>
        /// <param name="data">The changed data.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ReplaceRange> ReplaceRange(IndexInterval changedInterval, IList<long> channelIds, IList<DataItem> data, bool isFinalPart = true, IMessageHeader correlatedHeader = null, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the ReplaceRangeResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<ReplaceRange, ReplaceRangeResponse>> OnReplaceRangeResponse;

        /// <summary>
        /// Sends a CloseChannels message to a store.
        /// </summary>
        /// <param name="channelIds">The channel IDs.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<CloseChannels> CloseChannels(IDictionary<string, long> channelIds, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a CloseChannels message to a store.
        /// </summary>
        /// <param name="channelIds">The channel IDs.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<CloseChannels> CloseChannels(IList<long> channelIds, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the ChannelsClosed event from a store when sent in response to a CloseChannels.
        /// </summary>
        event EventHandler<ResponseEventArgs<CloseChannels, ChannelsClosed>> OnResponseChannelsClosed;

        /// <summary>
        /// Handles the ChannelsClosed event from a store when not sent in response to a request.
        /// </summary>
        event EventHandler<FireAndForgetEventArgs<ChannelsClosed>> OnNotificationChannelsClosed;
    }
}

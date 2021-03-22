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
using Energistics.Etp.v11.Datatypes.ChannelData;

namespace Energistics.Etp.v11.Protocol.ChannelStreaming
{
    /// <summary>
    /// Defines the interface that must be implemented by the producer role of the ChannelStreaming protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.ChannelStreaming, Roles.Producer, Roles.Consumer)]
    public interface IChannelStreamingProducer : IProtocolHandlerWithCapabilities<ICapabilitiesProducer>
    {
        /// <summary>
        /// Gets the maximum data items.
        /// </summary>
        /// <value>The maximum data items.</value>
        int MaxDataItems { get; }

        /// <summary>
        /// Gets the maximum message rate.
        /// </summary>
        /// <value>The maximum message rate.</value>
        int MaxMessageRate { get; }

        /// <summary>
        /// Handles the Start event from a consumer.
        /// </summary>
        event EventHandler<VoidRequestEventArgs<Start>> OnStart;

        /// <summary>
        /// Handles the ChannelDescribe event from a consumer.
        /// </summary>
        event EventHandler<ListRequestEventArgs<ChannelDescribe, ChannelMetadataRecord>> OnChannelDescribe;

        /// <summary>
        /// Sends a ChannelMetadata message to a consumer.
        /// </summary>
        /// <param name="subscriptionUuid">The subscription UUID associated with the ChannelDescribe message that the message to send is correlated with.</param>
        /// <param name="channels">The list of <see cref="ChannelMetadataRecord" /> objects.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ChannelMetadata> ChannelMetadata(Guid subscriptionUuid, IList<ChannelMetadataRecord> channels, bool isFinalPart = true);

        /// <summary>
        /// Sends a ChannelMetadata message to a consumer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="channels">The list of <see cref="ChannelMetadataRecord" /> objects.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ChannelMetadata> ChannelMetadata(IMessageHeader correlatedHeader, IList<ChannelMetadataRecord> channels, bool isFinalPart = true);

        /// <summary>
        /// Handles the ChannelStreamingStart event from a consumer.
        /// </summary>
        event EventHandler<VoidRequestEventArgs<ChannelStreamingStart>> OnChannelStreamingStart;

        /// <summary>
        /// Handles the ChannelRangeRequest event from a consumer.
        /// </summary>
        event EventHandler<ListRequestEventArgs<ChannelRangeRequest, DataItem>> OnChannelRangeRequest;

        /// <summary>
        /// Sends a streaming ChannelData message to a consumer.
        /// </summary>
        /// <param name="data">The list of <see cref="DataItem" /> objects.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ChannelData> StreamingChannelData(IList<DataItem> data);

        /// <summary>
        /// Sends a ChannelData message to a consumer in response to a range request.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="data">The list of <see cref="DataItem" /> objects.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ChannelData> ChannelRangeRequestChannelData(IMessageHeader correlatedHeader, IList<DataItem> data, bool isFinalPart = true);

        /// <summary>
        /// Sends a ChannelDataChange message to a consumer.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="dataItems">The data items.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ChannelDataChange> ChannelDataChange(long channelId, long startIndex, long endIndex, IList<DataItem> dataItems);

        /// <summary>
        /// Sends a ChannelStatusChange message to a consumer.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="status">The channel status.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ChannelStatusChange> ChannelStatusChange(long channelId, ChannelStatuses status);

        /// <summary>
        /// Sends a ChannelRemove message to a consumer.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="reason">The reason.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ChannelRemove> ChannelRemove(long channelId, string reason = null);

        /// <summary>
        /// Handles the ChannelStreamingStop event from a consumer.
        /// </summary>
        event EventHandler<VoidRequestEventArgs<ChannelStreamingStop>> OnChannelStreamingStop;
    }
}

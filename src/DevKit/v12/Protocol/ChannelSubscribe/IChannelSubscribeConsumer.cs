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
    /// Defines the interface that must be implemented by the consumer role of the ChannelSubscribe protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.ChannelSubscribe, "consumer", "producer")]
    public interface IChannelSubscribeConsumer : IProtocolHandler
    {
        /// <summary>
        /// Sends a GetChannelMetadata message to a producer with the specified URIs.
        /// </summary>
        /// <param name="uris">The list of URIs.</param>
        /// <returns>The message identifier.</returns>
        long GetChannelMetadata(IList<string> uris);

        /// <summary>
        /// Handles the GetChannelMetadataResponse event from a producer.
        /// </summary>
        event ProtocolEventHandler<GetChannelMetadataResponse> OnGetChannelMetadataResponse;

        /// <summary>
        /// Sends a SubscribeChannels message to a producer.
        /// </summary>
        /// <param name="channels">The list of channels.</param>
        /// <returns>The message identifier.</returns>
        long SubscribeChannels(IList<ChannelSubscribeInfo> channels);

        /// <summary>
        /// Handles the RealtimeData event from a producer.
        /// </summary>
        event ProtocolEventHandler<RealtimeData> OnRealtimeData;

        /// <summary>
        /// Handles the ReplaceRange event from a producer.
        /// </summary>
        event ProtocolEventHandler<ReplaceRange> OnReplaceRange;

        /// <summary>
        /// Sends a UnsubscribeChannels message to a producer.
        /// </summary>
        /// <param name="channelIds">The list of channel identifiers.</param>
        /// <returns>The message identifier.</returns>
        long UnsubscribeChannels(IList<long> channelIds);

        /// <summary>
        /// Handles the SubscriptionsStopped event from a producer.
        /// </summary>
        event ProtocolEventHandler<SubscriptionsStopped> OnSubscriptionsStopped;

        /// <summary>
        /// Sends a GetRanges message to a producer.
        /// </summary>
        /// <param name="requestUuid">The request identifier.</param>
        /// <param name="channelRanges">The list of channelRanges.</param>
        /// <returns>The message identifier.</returns>
        long GetRanges(Guid requestUuid, IList<ChannelRangeInfo> channelRanges);

        /// <summary>
        /// Handles the GetRangesResponse event from a producer.
        /// </summary>
        event ProtocolEventHandler<GetRangesResponse> OnGetRangesResponse;

        /// <summary>
        /// Sends a CancelGetRanges message to a producer.
        /// </summary>
        /// <param name="requestUuid">The request identifier.</param>
        /// <returns>The message identifier.</returns>
        long CancelGetRanges(Guid requestUuid);
    }
}

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
        /// Sends a SubscribeChannels message to a producer.
        /// </summary>
        /// <param name="channelSubscribeInfos">The list of <see cref="ChannelSubscribeInfo"/> objects.</param>
        /// <returns>The message identifier.</returns>
        long SubscribeChannels(IList<ChannelSubscribeInfo> channelSubscribeInfos);

        /// <summary>
        /// Sends a UnsubscribeChannels message to a producer.
        /// </summary>
        /// <param name="channelIds">The list of channel identifiers.</param>
        /// <returns>The message identifier.</returns>
        long UnsubscribeChannels(IList<long> channelIds);

        /// <summary>
        /// Sends a GetRange message to a producer.
        /// </summary>
        /// <param name="requestUuid">The request identifier.</param>
        /// <param name="channelRangeInfos">The list of <see cref="ChannelRangeInfo" /> objects.</param>
        /// <returns>The message identifier.</returns>
        long GetRange(Guid requestUuid, IList<ChannelRangeInfo> channelRangeInfos);

        /// <summary>
        /// Sends a CancelGetRange message to a producer.
        /// </summary>
        /// <param name="requestUuid">The request identifier.</param>
        /// <returns>The message identifier.</returns>
        long CancelGetRange(Guid requestUuid);

        /// <summary>
        /// Handles the GetChannelMetadataResponse event from a producer.
        /// </summary>
        event ProtocolEventHandler<GetChannelMetadataResponse> OnGetChannelMetadataResponse;

        /// <summary>
        /// Handles the RealtimeData event from a producer.
        /// </summary>
        event ProtocolEventHandler<RealtimeData> OnRealtimeData;

        /// <summary>
        /// Handles the InfillData event from a producer.
        /// </summary>
        event ProtocolEventHandler<InfillData> OnInfillData;

        /// <summary>
        /// Handles the ChangedData event from a producer.
        /// </summary>
        event ProtocolEventHandler<ChangedData> OnChangedData;

        /// <summary>
        /// Handles the SubscriptionStopped event from a producer.
        /// </summary>
        event ProtocolEventHandler<SubscriptionStopped> OnSubscriptionStopped;

        /// <summary>
        /// Handles the GetRangeResponse event from a producer.
        /// </summary>
        event ProtocolEventHandler<GetRangeResponse> OnGetRangeResponse;
    }
}

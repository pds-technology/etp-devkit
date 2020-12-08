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
    [ProtocolRole((int)Protocols.ChannelSubscribe, "customer", "store")]
    public interface IChannelSubscribeCustomer : IProtocolHandler
    {
        /// <summary>
        /// Sets limits on maximum indexCount (number of indexes "back" from the current index that a store will provide) for StreamingStartIndex.
        /// </summary>
        long StoreMaxIndexCount { get; }

        /// <summary>
        /// Indicates the maximum time in integer number of seconds a store allows no streaming data to occur before setting the channelStatus to 'inactive'.
        /// </summary>
        long StoreStreamingTimeoutPeriod { get; }

        /// <summary>
        /// Maximum number of data points to return in each message.
        /// </summary>
        long MaxDataItemCount { get; set; }

        /// <summary>
        /// Sends a GetChannelMetadata message to a store with the specified URIs.
        /// </summary>
        /// <param name="uris">The list of URIs.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long GetChannelMetadata(IList<string> uris);

        /// <summary>
        /// Handles the GetChannelMetadataResponse event from a store.
        /// </summary>
        event ProtocolEventHandler<GetChannelMetadataResponse> OnGetChannelMetadataResponse;

        /// <summary>
        /// Sends a SubscribeChannels message to a store.
        /// </summary>
        /// <param name="channels">The list of channels.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long SubscribeChannels(IList<ChannelSubscribeInfo> channels);

        /// <summary>
        /// Handles the ChannelData event from a store.
        /// </summary>
        event ProtocolEventHandler<ChannelData> OnChannelData;

        /// <summary>
        /// Handles the RangeReplaced event from a store.
        /// </summary>
        event ProtocolEventHandler<RangeReplaced> OnRangeReplaced;

        /// <summary>
        /// Sends a UnsubscribeChannels message to a store.
        /// </summary>
        /// <param name="channelIds">The list of channel identifiers.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long UnsubscribeChannels(IList<long> channelIds);

        /// <summary>
        /// Handles the SubscriptionsStopped event from a store.
        /// </summary>
        event ProtocolEventHandler<SubscriptionsStopped> OnSubscriptionsStopped;

        /// <summary>
        /// Sends a GetRanges message to a store.
        /// </summary>
        /// <param name="requestUuid">The request identifier.</param>
        /// <param name="channelRanges">The list of channelRanges.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long GetRanges(Guid requestUuid, IList<ChannelRangeInfo> channelRanges);

        /// <summary>
        /// Handles the GetRangesResponse event from a store.
        /// </summary>
        event ProtocolEventHandler<GetRangesResponse> OnGetRangesResponse;

        /// <summary>
        /// Sends a CancelGetRanges message to a store.
        /// </summary>
        /// <param name="requestUuid">The request identifier.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long CancelGetRanges(Guid requestUuid);
    }
}

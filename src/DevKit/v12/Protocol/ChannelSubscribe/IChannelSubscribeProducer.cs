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

using System.Collections.Generic;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v12.Datatypes;
using Energistics.Etp.v12.Datatypes.ChannelData;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.ChannelSubscribe
{
    /// <summary>
    /// Defines the interface that must be implemented by the producer role of the ChannelSubscribe protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.ChannelSubscribe, "producer", "consumer")]
    public interface IChannelSubscribeProducer : IProtocolHandler
    {
        /// <summary>
        /// Sets limits on maximum indexCount (number of indexes "back" from the current index that a producer will provide) for StreamingStartIndex.
        /// </summary>
        long MaxIndexCount { get; set; }

        /// <summary>
        /// Indicates the maximum time in integer number of seconds a store allows no streaming data to occur before setting the channelStatus to 'inactive'.
        /// </summary>
        long StreamingTimeoutPeriod { get; set; }

        /// <summary>
        /// Maximum number of data points to return in each message.
        /// </summary>
        long CustomerMaxDataItemCount { get; }

        /// <summary>
        /// Handles the GetChannelMetadata event from a consumer.
        /// </summary>
        event ProtocolEventWithErrorsHandler<GetChannelMetadata, ChannelMetadataRecord, ErrorInfo> OnGetChannelMetadata;

        /// <summary>
        /// Sends a GetChannelMetadataResponse message to a consumer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="metadata">The channel metadata records.</param>
        /// <param name="errors">The errors.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long GetChannelMetadataResponse(IMessageHeader request, IDictionary<string, ChannelMetadataRecord> metadata, IDictionary<string, ErrorInfo> errors);

        /// <summary>
        /// Handles the SubscribeChannels event from a consumer.
        /// </summary>
        event ProtocolEventWithErrorsHandler<SubscribeChannels, ErrorInfo> OnSubscribeChannels;

        /// <summary>
        /// Sends a ChannelData message to a consumer.
        /// </summary>
        /// <param name="dataItems">The list of <see cref="DataItem" /> objects.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long ChannelData(IList<DataItem> dataItems);

        /// <summary>
        /// Sends a RangeReplaced message to a consumer.
        /// </summary>
        /// <param name="channelIds">The IDs of the channels that are changing.</param>
        /// <param name="changedInterval">The indexes that define the interval that is changing.</param>
        /// <param name="dataItems">The channel data of the changed interval.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long RangeReplaced(IList<long> channelIds, IndexInterval changedInterval, IList<DataItem> dataItems);

        /// <summary>
        /// Handles the UnsubscribeChannels event from a consumer.
        /// </summary>
        event ProtocolEventWithErrorsHandler<UnsubscribeChannels, long, ErrorInfo> OnUnsubscribeChannels;

        /// <summary>
        /// Sends a SubscriptionsStopped message to a consumer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="channelIds">The channel identifiers.</param>
        /// <param name="errors">The errors if any.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long SubscriptionsStopped(IMessageHeader request, IDictionary<string, long> channelIds, IDictionary<string, ErrorInfo> errors);

        /// <summary>
        /// Handles the GetRanges event from a consumer.
        /// </summary>
        event ProtocolEventHandler<GetRanges> OnGetRanges;

        /// <summary>
        /// Sends a GetRangesResponse message to a consumer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="dataItems">The data items.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long GetRangesResponse(IMessageHeader request, IList<DataItem> dataItems);

        /// <summary>
        /// Handles the CancelGetRanges event from a consumer.
        /// </summary>
        event ProtocolEventHandler<CancelGetRanges> OnCancelGetRanges;
    }
}

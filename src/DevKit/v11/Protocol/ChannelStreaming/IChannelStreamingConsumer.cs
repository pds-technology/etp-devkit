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
using Energistics.Etp.v11.Datatypes.ChannelData;

namespace Energistics.Etp.v11.Protocol.ChannelStreaming
{
    /// <summary>
    /// Defines the interface that must be implemented by the consumer role of the ChannelStreaming protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.ChannelStreaming, "consumer", "producer")]
    public interface IChannelStreamingConsumer : IProtocolHandler
    {
        /// <summary>
        /// Sends a Start message to a producer with the specified throttling parameters.
        /// </summary>
        /// <param name="maxDataItems">The maximum data items.</param>
        /// <param name="maxMessageRate">The maximum message rate.</param>
        /// <returns>The message identifier.</returns>
        long Start(int maxDataItems = 10000, int maxMessageRate = 1000);

        /// <summary>
        /// Sends a ChannelDescribe message to a producer with the specified URIs.
        /// </summary>
        /// <param name="uris">The list of URIs.</param>
        /// <returns>The message identifier.</returns>
        long ChannelDescribe(IList<string> uris);

        /// <summary>
        /// Sends a ChannelStreamingStart message to a producer.
        /// </summary>
        /// <param name="channelStreamingInfos">The list of <see cref="ChannelStreamingInfo"/> objects.</param>
        /// <returns>The message identifier.</returns>
        long ChannelStreamingStart(IList<ChannelStreamingInfo> channelStreamingInfos);

        /// <summary>
        /// Sends a ChannelStreamingStop message to a producer.
        /// </summary>
        /// <param name="channelIds">The list of channel identifiers.</param>
        /// <returns>The message identifier.</returns>
        long ChannelStreamingStop(IList<long> channelIds);

        /// <summary>
        /// Sends a ChannelRangeRequest message to a producer.
        /// </summary>
        /// <param name="channelRangeInfos">The list of <see cref="ChannelRangeInfo"/> objects.</param>
        /// <returns>The message identifier.</returns>
        long ChannelRangeRequest(IList<ChannelRangeInfo> channelRangeInfos);

        /// <summary>
        /// Handles the ChannelMetadata event from a producer.
        /// </summary>
        event ProtocolEventHandler<ChannelMetadata> OnChannelMetadata;

        /// <summary>
        /// Handles the ChannelData event from a producer.
        /// </summary>
        event ProtocolEventHandler<ChannelData> OnChannelData;

        /// <summary>
        /// Handles the ChannelDataChange event from a producer.
        /// </summary>
        event ProtocolEventHandler<ChannelDataChange> OnChannelDataChange;

        /// <summary>
        /// Handles the ChannelStatusChange event from a producer.
        /// </summary>
        event ProtocolEventHandler<ChannelStatusChange> OnChannelStatusChange;

        /// <summary>
        /// Handles the ChannelRemove event from a producer.
        /// </summary>
        event ProtocolEventHandler<ChannelRemove> OnChannelRemove;
    }
}

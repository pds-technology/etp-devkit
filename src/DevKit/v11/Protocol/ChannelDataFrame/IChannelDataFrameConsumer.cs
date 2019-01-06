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

using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;

namespace Energistics.Etp.v11.Protocol.ChannelDataFrame
{
    /// <summary>
    /// Defines the interface that must be implemented by the consumer role of the ChannelDataFrame protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.ChannelDataFrame, "consumer", "producer")]
    public interface IChannelDataFrameConsumer : IProtocolHandler
    {
        /// <summary>
        /// Sends a RequestChannelData message to a producer.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="fromIndex">From index.</param>
        /// <param name="toIndex">To index.</param>
        /// <returns>The message identifier.</returns>
        long RequestChannelData(string uri, long? fromIndex = null, long? toIndex = null);

        /// <summary>
        /// Handles the ChannelMetadata event from a producer.
        /// </summary>
        event ProtocolEventHandler<ChannelMetadata> OnChannelMetadata;

        /// <summary>
        /// Handles the ChannelDataFrameSet event from a producer.
        /// </summary>
        event ProtocolEventHandler<ChannelDataFrameSet> OnChannelDataFrameSet;
    }
}

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
using Energistics.Etp.v12.Datatypes.ChannelData;

namespace Energistics.Etp.v12.Protocol.ChannelStreaming
{
    /// <summary>
    /// Defines the interface that must be implemented by the producer role of the ChannelStreaming protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.ChannelStreaming, "producer", "consumer")]
    public interface IChannelStreamingProducer : IProtocolHandler
    {
        /// <summary>
        /// Sends a ChannelMetadata message to a consumer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="channelMetadataRecords">The list of <see cref="ChannelMetadataRecord" /> objects.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        long ChannelMetadata(IMessageHeader request, IList<ChannelMetadataRecord> channelMetadataRecords, MessageFlags messageFlag = MessageFlags.MultiPartAndFinalPart);

        /// <summary>
        /// Sends a ChannelData message to a consumer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="dataItems">The list of <see cref="DataItem" /> objects.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        long ChannelData(IMessageHeader request, IList<DataItem> dataItems, MessageFlags messageFlag = MessageFlags.MultiPart);

        /// <summary>
        /// Handles the StartStreaming event from a consumer.
        /// </summary>
        event ProtocolEventHandler<StartStreaming> OnStartStreaming;

        /// <summary>
        /// Handles the StopStreaming event from a consumer.
        /// </summary>
        event ProtocolEventHandler<StopStreaming> OnStopStreaming;
    }
}

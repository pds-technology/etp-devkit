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
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.ChannelDataLoad
{
    /// <summary>
    /// Describes the interface that must be implemented by the producer role of the ChannelDataLoad protocol.
    /// </summary>
    /// <seealso cref="IProtocolHandler" />
    [ProtocolRole((int)Protocols.ChannelDataLoad, "producer", "consumer")]
    public interface IChannelDataLoadProducer : IProtocolHandler
    {
        /// <summary>
        /// Sends a OpenChannels message to a consumer.
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long OpenChannels(IList<ChannelMetadataRecord> channels);

        /// <summary>
        /// Handles the OpenChannelsResponse event from a consumer.
        /// </summary>
        event ProtocolEventHandler<OpenChannelsResponse> OnOpenChannelsResponse;

        /// <summary>
        /// Sends a CloseChannel message to a consumer.
        /// </summary>
        /// <param name="channelIds">The channel IDs.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long CloseChannel(IList<long> channelIds);

        /// <summary>
        /// Sends a RealtimeData message to a consumer.
        /// </summary>
        /// <param name="dataItems">The data items.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long RealtimeData(IList<DataItem> dataItems);

        /// <summary>
        /// Sends a ReplaceRange message to a consumer.
        /// </summary>
        /// <param name="channelIds">The IDs of the channels that are changing.</param>
        /// <param name="changedInterval">The indexes that define the interval that is changing.</param>
        /// <param name="dataItems">The channel data of the changed interval.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long ReplaceRange(IList<long> channelIds, IndexInterval changedInterval, IList<DataItem> dataItems);

    }
}

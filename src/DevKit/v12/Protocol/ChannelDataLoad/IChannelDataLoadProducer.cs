//----------------------------------------------------------------------- 
// ETP DevKit, 1.2
//
// Copyright 2018 Energistics
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
        /// Sends a OpenChannel message to a store.
        /// </summary>
        /// <param name="uri">The channel URI.</param>
        /// <param name="id">The channel identifier.</param>
        /// <returns>The message identifier.</returns>
        long OpenChannel(string uri, long id);

        /// <summary>
        /// Sends a CloseChannel message to a store.
        /// </summary>
        /// <param name="id">The channel identifier.</param>
        /// <param name="reason">The close reason.</param>
        /// <returns>The message identifier.</returns>
        long CloseChannel(long id, string reason);

        /// <summary>
        /// Sends a RealtimeData message to a store.
        /// </summary>
        /// <param name="dataPoints">The data points.</param>
        /// <returns>The message identifier.</returns>
        long RealtimeData(IList<DataPoint> dataPoints);

        /// <summary>
        /// Sends a InfillRealtimeData message to a store.
        /// </summary>
        /// <param name="dataPoints">The data points.</param>
        /// <returns>The message identifier.</returns>
        long InfillRealtimeData(IList<DataPoint> dataPoints);

        /// <summary>
        /// Sends a ChannelDataChange message to a store.
        /// </summary>
        /// <param name="id">The channel identifier.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="dataPoints">The data points.</param>
        /// <returns>The message identifier.</returns>
        long ChannelDataChange(long id, object startIndex, object endIndex, IList<DataPoint> dataPoints);

        /// <summary>
        /// Handles the OpenChannelResponse event from a store.
        /// </summary>
        event ProtocolEventHandler<OpenChannelResponse> OnOpenChannelResponse;
    }
}

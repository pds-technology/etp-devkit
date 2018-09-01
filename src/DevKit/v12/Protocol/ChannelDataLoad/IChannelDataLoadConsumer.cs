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

using System;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;

namespace Energistics.Etp.v12.Protocol.ChannelDataLoad
{
    /// <summary>
    /// Describes the interface that must be implemented by the consumer role of the ChannelDataLoad protocol.
    /// </summary>
    /// <seealso cref="IProtocolHandler" />
    [ProtocolRole((int)Protocols.ChannelDataLoad, "consumer", "producer")]
    public interface IChannelDataLoadConsumer : IProtocolHandler
    {
        /// <summary>
        /// Sends a OpenChannelResponse message to a store.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="uri">The channel URI.</param>
        /// <param name="id">The channel identifier.</param>
        /// <param name="uuid">The channel UUID.</param>
        /// <param name="lastIndex">The last index.</param>
        /// <param name="infill">if set to <c>true</c> provide infill data.</param>
        /// <param name="dataChanges">if set to <c>true</c> provide channel data changes.</param>
        /// <returns>The message identifier.</returns>
        long OpenChannelResponse(IMessageHeader request, string uri, long id, Guid uuid, object lastIndex = null, bool infill = true, bool dataChanges = true);

        /// <summary>
        /// Handles the OpenChannel event from a store.
        /// </summary>
        event ProtocolEventHandler<OpenChannel> OnOpenChannel;

        /// <summary>
        /// Handles the CloseChannel event from a store.
        /// </summary>
        event ProtocolEventHandler<CloseChannel> OnCloseChannel;

        /// <summary>
        /// Handles the RealtimeData event from a store.
        /// </summary>
        event ProtocolEventHandler<RealtimeData> OnRealtimeData;

        /// <summary>
        /// Handles the InfillRealtimeData event from a store.
        /// </summary>
        event ProtocolEventHandler<InfillRealtimeData> OnInfillRealtimeData;

        /// <summary>
        /// Handles the ChannelDataChange event from a store.
        /// </summary>
        event ProtocolEventHandler<ChannelDataChange> OnChannelDataChange;
    }
}

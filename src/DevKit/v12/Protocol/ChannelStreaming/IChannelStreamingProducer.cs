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

namespace Energistics.Etp.v12.Protocol.ChannelStreaming
{
    /// <summary>
    /// Defines the interface that must be implemented by the producer role of the ChannelStreaming protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.ChannelStreaming, Roles.Producer, Roles.Consumer)]
    public interface IChannelStreamingProducer : IProtocolHandler
    {
        /// <summary>
        /// Handles the StartStreaming event from a consumer.
        /// </summary>
        event EventHandler<VoidRequestEventArgs<StartStreaming>> OnStartStreaming;

        /// <summary>
        /// Sends a ChannelMetadata message to a consumer.
        /// </summary>
        /// <param name="channels">The list of <see cref="ChannelMetadataRecord" /> objects.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ChannelMetadata> ChannelMetadata(IList<ChannelMetadataRecord> channels, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a ChannelData message to a consumer.
        /// </summary>
        /// <param name="data">The list of <see cref="DataItem" /> objects.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ChannelData> ChannelData(IList<DataItem> data, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a TruncateChannels message to a consumer.
        /// </summary>
        /// <param name="channels">The list of <see cref="TruncateInfo" /> objects.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<TruncateChannels> TruncateChannels(IList<TruncateInfo> channels, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the StopStreaming event from a consumer.
        /// </summary>
        event EventHandler<VoidRequestEventArgs<StopStreaming>> OnStopStreaming;
    }
}

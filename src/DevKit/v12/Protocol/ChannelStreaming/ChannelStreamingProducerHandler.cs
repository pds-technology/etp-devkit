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

namespace Energistics.Etp.v12.Protocol.ChannelStreaming
{
    /// <summary>
    /// Base implementation of the <see cref="IChannelStreamingProducer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.ChannelStreaming.IChannelStreamingProducer" />
    public class ChannelStreamingProducerHandler : Etp12ProtocolHandler, IChannelStreamingProducer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelStreamingProducerHandler"/> class.
        /// </summary>
        public ChannelStreamingProducerHandler() : base((int)Protocols.ChannelStreaming, "producer", "consumer")
        {
            MaxDataItemCount = EtpSettings.DefaultMaxDataItemCount;

            RegisterMessageHandler<StartStreaming>(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.StartStreaming, HandleStartStreaming);
            RegisterMessageHandler<StopStreaming>(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.StopStreaming, HandleStopStreaming);
        }

        /// <summary>
        /// Gets the maximum data items.
        /// </summary>
        public long MaxDataItemCount { get; set; }

        /// <summary>
        /// Gets the capabilities supported by the protocol handler.
        /// </summary>
        /// <param name="capabilities">The protocol's capabilities.</param>
        public override void GetCapabilities(EtpProtocolCapabilities capabilities)
        {
            base.GetCapabilities(capabilities);

            capabilities.MaxDataItemCount = MaxDataItemCount;
        }

        /// <summary>
        /// Handles the StartStreaming event from a consumer.
        /// </summary>
        public event ProtocolEventHandler<StartStreaming> OnStartStreaming;

        /// <summary>
        /// Handles the StopStreaming event from a consumer.
        /// </summary>
        public event ProtocolEventHandler<StopStreaming> OnStopStreaming;

        /// <summary>
        /// Sends a ChannelMetadata message to a consumer.
        /// </summary>
        /// <param name="channelMetadataRecords">The list of <see cref="ChannelMetadataRecord" /> objects.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long ChannelMetadata(IList<ChannelMetadataRecord> channelMetadataRecords)
        {
            var header = CreateMessageHeader(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelMetadata);

            var channelMetadata = new ChannelMetadata
            {
                Channels = channelMetadataRecords
            };

            return Session.SendMessage(header, channelMetadata);
        }

        /// <summary>
        /// Sends a ChannelData message to a consumer.
        /// </summary>
        /// <param name="dataItems">The list of <see cref="DataItem" /> objects.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long ChannelData(IList<DataItem> dataItems)
        {
            var header = CreateMessageHeader(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelData);

            var channelData = new ChannelData
            {
                Data = dataItems
            };

            return Session.SendMessage(header, channelData);
        }

        /// <summary>
        /// Handles the StartStreaming message from a consumer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="startStreaming">The StartStreaming message.</param>
        protected virtual void HandleStartStreaming(IMessageHeader header, StartStreaming startStreaming)
        {
            Notify(OnStartStreaming, header, startStreaming);
        }

        /// <summary>
        /// Handles the StopStreaming message from a consumer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="stopStreaming">The StopStreaming message.</param>
        protected virtual void HandleStopStreaming(IMessageHeader header, StopStreaming stopStreaming)
        {
            Notify(OnStopStreaming, header, stopStreaming);
        }
    }
}

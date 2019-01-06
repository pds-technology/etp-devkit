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
    /// Base implementation of the <see cref="IChannelStreamingConsumer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.ChannelStreaming.IChannelStreamingConsumer" />
    public class ChannelStreamingConsumerHandler : Etp12ProtocolHandler, IChannelStreamingConsumer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelStreamingConsumerHandler"/> class.
        /// </summary>
        public ChannelStreamingConsumerHandler() : base((int)Protocols.ChannelStreaming, "consumer", "producer")
        {
            ChannelMetadataRecords = new List<ChannelMetadataRecord>();

            RegisterMessageHandler<ChannelMetadata>(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelMetadata, HandleChannelMetadata);
            RegisterMessageHandler<ChannelData>(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelData, HandleChannelData);
        }

        /// <summary>
        /// Gets the list of <see cref="ChannelMetadataRecords"/> objects returned by the producer.
        /// </summary>
        /// <value>The list of <see cref="ChannelMetadataRecords"/> objects.</value>
        protected IList<ChannelMetadataRecord> ChannelMetadataRecords { get; }

        /// <summary>
        /// Sends a StartStreaming message to a producer.
        /// </summary>
        /// <returns>The message identifier.</returns>
        public virtual long StartStreaming()
        {
            var header = CreateMessageHeader(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.StartStreaming);

            var start = new StartStreaming();
            ChannelMetadataRecords.Clear();

            return Session.SendMessage(header, start);
        }

        /// <summary>
        /// Sends a StopStreaming message to a producer.
        /// </summary>
        /// <returns>The message identifier.</returns>
        public virtual long StopStreaming()
        {
            var header = CreateMessageHeader(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.StopStreaming);

            var channelStreamingStop = new StopStreaming();

            return Session.SendMessage(header, channelStreamingStop);
        }

        /// <summary>
        /// Handles the ChannelMetadata event from a producer.
        /// </summary>
        public event ProtocolEventHandler<ChannelMetadata> OnChannelMetadata;

        /// <summary>
        /// Handles the ChannelData event from a producer.
        /// </summary>
        public event ProtocolEventHandler<ChannelData> OnChannelData;

        /// <summary>
        /// Handles the ChannelMetadata message from a producer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="channelMetadata">The ChannelMetadata message.</param>
        protected virtual void HandleChannelMetadata(IMessageHeader header, ChannelMetadata channelMetadata)
        {
            foreach (var channel in channelMetadata.Channels)
                ChannelMetadataRecords.Add(channel);

            Notify(OnChannelMetadata, header, channelMetadata);
        }

        /// <summary>
        /// Handles the ChannelData message from a producer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="channelData">The ChannelData message.</param>
        protected virtual void HandleChannelData(IMessageHeader header, ChannelData channelData)
        {
            Notify(OnChannelData, header, channelData);
        }
    }
}

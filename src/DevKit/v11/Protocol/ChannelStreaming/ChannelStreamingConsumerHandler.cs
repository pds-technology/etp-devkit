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
    /// Base implementation of the <see cref="IChannelStreamingConsumer"/> interface.
    /// </summary>
    /// <seealso cref="Etp11ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v11.Protocol.ChannelStreaming.IChannelStreamingConsumer" />
    public class ChannelStreamingConsumerHandler : Etp11ProtocolHandler, IChannelStreamingConsumer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelStreamingConsumerHandler"/> class.
        /// </summary>
        public ChannelStreamingConsumerHandler() : base((int)Protocols.ChannelStreaming, "consumer", "producer")
        {
            ChannelMetadataRecords = new List<ChannelMetadataRecord>(0);

            RegisterMessageHandler<ChannelMetadata>(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelMetadata, HandleChannelMetadata);
            RegisterMessageHandler<ChannelData>(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelData, HandleChannelData);
            RegisterMessageHandler<ChannelDataChange>(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelDataChange, HandleChannelDataChange);
            RegisterMessageHandler<ChannelStatusChange>(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelStatusChange, HandleChannelStatusChange);
            RegisterMessageHandler<ChannelRemove>(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelRemove, HandleChannelRemove);
        }

        /// <summary>
        /// Gets the list of <see cref="ChannelMetadataRecords"/> objects returned by the producer.
        /// </summary>
        /// <value>The list of <see cref="ChannelMetadataRecords"/> objects.</value>
        protected IList<ChannelMetadataRecord> ChannelMetadataRecords { get; }

        /// <summary>
        /// Sends a Start message to a producer with the specified throttling parameters.
        /// </summary>
        /// <param name="maxDataItems">The maximum data items.</param>
        /// <param name="maxMessageRate">The maximum message rate.</param>
        /// <returns>The message identifier.</returns>
        public virtual long Start(int maxDataItems = 10000, int maxMessageRate = 1000)
        {
            var header = CreateMessageHeader(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.Start);

            var start = new Start()
            {
                MaxDataItems = maxDataItems,
                MaxMessageRate = maxMessageRate
            };

            ChannelMetadataRecords.Clear();
            return Session.SendMessage(header, start);
        }

        /// <summary>
        /// Sends a ChannelDescribe message to a producer with the specified URIs.
        /// </summary>
        /// <param name="uris">The list of URIs.</param>
        /// <returns>The message identifier.</returns>
        public virtual long ChannelDescribe(IList<string> uris)
        {
            var header = CreateMessageHeader(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelDescribe);

            var channelDescribe = new ChannelDescribe()
            {
                Uris = uris
            };

            return Session.SendMessage(header, channelDescribe);
        }

        /// <summary>
        /// Sends a ChannelStreamingStart message to a producer.
        /// </summary>
        /// <param name="channelStreamingInfos">The list of <see cref="ChannelStreamingInfo" /> objects.</param>
        /// <returns>The message identifier.</returns>
        public virtual long ChannelStreamingStart(IList<ChannelStreamingInfo> channelStreamingInfos)
        {
            var header = CreateMessageHeader(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelStreamingStart);

            var channelStreamingStart = new ChannelStreamingStart()
            {
                Channels = channelStreamingInfos
            };

            return Session.SendMessage(header, channelStreamingStart);
        }

        /// <summary>
        /// Sends a ChannelStreamingStop message to a producer.
        /// </summary>
        /// <param name="channelIds">The list of channel identifiers.</param>
        /// <returns>The message identifier.</returns>
        public virtual long ChannelStreamingStop(IList<long> channelIds)
        {
            var header = CreateMessageHeader(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelStreamingStop);

            var channelStreamingStop = new ChannelStreamingStop()
            {
                Channels = channelIds
            };

            return Session.SendMessage(header, channelStreamingStop);
        }

        /// <summary>
        /// Sends a ChannelRangeRequest message to a producer.
        /// </summary>
        /// <param name="channelRangeInfos">The list of <see cref="ChannelRangeInfo" /> objects.</param>
        /// <returns>The message identifier.</returns>
        public virtual long ChannelRangeRequest(IList<ChannelRangeInfo> channelRangeInfos)
        {
            var header = CreateMessageHeader(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelRangeRequest);

            var channelRangeRequest = new ChannelRangeRequest()
            {
                ChannelRanges = channelRangeInfos
            };

            return Session.SendMessage(header, channelRangeRequest);
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
        /// Handles the ChannelDataChange event from a producer.
        /// </summary>
        public event ProtocolEventHandler<ChannelDataChange> OnChannelDataChange;

        /// <summary>
        /// Handles the ChannelStatusChange event from a producer.
        /// </summary>
        public event ProtocolEventHandler<ChannelStatusChange> OnChannelStatusChange;

        /// <summary>
        /// Handles the ChannelDelete event from a producer.
        /// </summary>
        public event ProtocolEventHandler<ChannelRemove> OnChannelRemove;

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

        /// <summary>
        /// Handles the ChannelDataChange message from a producer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="channelDataChange">The ChannelDataChange message.</param>
        protected virtual void HandleChannelDataChange(IMessageHeader header, ChannelDataChange channelDataChange)
        {
            Notify(OnChannelDataChange, header, channelDataChange);
        }

        /// <summary>
        /// Handles the ChannelStatusChange message from a producer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="channelStatusChange">The ChannelStatusChange message.</param>
        protected virtual void HandleChannelStatusChange(IMessageHeader header, ChannelStatusChange channelStatusChange)
        {
            Notify(OnChannelStatusChange, header, channelStatusChange);
        }

        /// <summary>
        /// Handles the ChannelRemove message from a producer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="channelRemove">The ChannelRemove message.</param>
        protected virtual void HandleChannelRemove(IMessageHeader header, ChannelRemove channelRemove)
        {
            Notify(OnChannelRemove, header, channelRemove);
        }
    }
}

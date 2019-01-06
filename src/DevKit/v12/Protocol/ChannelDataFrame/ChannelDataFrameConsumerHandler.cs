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
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.ChannelDataFrame
{
    /// <summary>
    /// Base implementation of the <see cref="IChannelDataFrameConsumer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.ChannelDataFrame.IChannelDataFrameConsumer" />
    public class ChannelDataFrameConsumerHandler : Etp12ProtocolHandler, IChannelDataFrameConsumer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelDataFrameConsumerHandler"/> class.
        /// </summary>
        public ChannelDataFrameConsumerHandler() : base((int)Protocols.ChannelDataFrame, "consumer", "producer")
        {
            RegisterMessageHandler<ChannelMetadata>(Protocols.ChannelDataFrame, MessageTypes.ChannelDataFrame.ChannelMetadata, HandleChannelMetadata);
            RegisterMessageHandler<ChannelDataFrameSet>(Protocols.ChannelDataFrame, MessageTypes.ChannelDataFrame.ChannelDataFrameSet, HandleChannelDataFrameSet);
        }

        /// <summary>
        /// Sends a RequestChannelData message to a producer.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="requestedInterval">The requested interval.</param>
        /// <returns>The message identifier.</returns>
        public virtual long RequestChannelData(string uri, IndexInterval requestedInterval)
        {
            var header = CreateMessageHeader(Protocols.ChannelDataFrame, MessageTypes.ChannelDataFrame.RequestChannelData);

            var requestChannelData = new RequestChannelData
            {
                Uri = uri,
                RequestedInterval = requestedInterval
            };

            return Session.SendMessage(header, requestChannelData);
        }

        /// <summary>
        /// Handles the ChannelMetadata event from a producer.
        /// </summary>
        public event ProtocolEventHandler<ChannelMetadata> OnChannelMetadata;

        /// <summary>
        /// Handles the ChannelDataFrameSet event from a producer.
        /// </summary>
        public event ProtocolEventHandler<ChannelDataFrameSet> OnChannelDataFrameSet;

        /// <summary>
        /// Handles the ChannelMetadata message from a producer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="channelMetadata">The ChannelMetadata message.</param>
        protected virtual void HandleChannelMetadata(IMessageHeader header, ChannelMetadata channelMetadata)
        {
            Notify(OnChannelMetadata, header, channelMetadata);
        }

        /// <summary>
        /// Handles the ChannelDataFrameSet message from a producer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="channelDataFrameSet">The ChannelDataFrameSet message.</param>
        protected virtual void HandleChannelDataFrameSet(IMessageHeader header, ChannelDataFrameSet channelDataFrameSet)
        {
            Notify(OnChannelDataFrameSet, header, channelDataFrameSet);
        }
    }
}

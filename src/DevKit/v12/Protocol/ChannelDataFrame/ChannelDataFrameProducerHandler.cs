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

namespace Energistics.Etp.v12.Protocol.ChannelDataFrame
{
    /// <summary>
    /// Base implementation of the <see cref="IChannelDataFrameProducer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.ChannelDataFrame.IChannelDataFrameProducer" />
    public class ChannelDataFrameProducerHandler : Etp12ProtocolHandler, IChannelDataFrameProducer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelDataFrameProducerHandler"/> class.
        /// </summary>
        public ChannelDataFrameProducerHandler() : base((int)Protocols.ChannelDataFrame, "producer", "consumer")
        {
            RegisterMessageHandler<RequestChannelData>(Protocols.ChannelDataFrame, MessageTypes.ChannelDataFrame.RequestChannelData, HandleRequestChannelData);
        }

        /// <summary>
        /// Sends a ChannelMetadata message to a consumer.
        /// </summary>
        /// <param name="channelMetadata">The channel metadata.</param>
        /// <returns>The message identifier.</returns>
        public virtual long ChannelMetadata(ChannelMetadata channelMetadata)
        {
            var header = CreateMessageHeader(Protocols.ChannelDataFrame, MessageTypes.ChannelDataFrame.ChannelMetadata);

            return Session.SendMessage(header, channelMetadata);
        }

        /// <summary>
        /// Sends a ChannelDataFrameSet message to a customer.
        /// </summary>
        /// <param name="channelIds">The channel ids.</param>
        /// <param name="dataFrames">The data frames.</param>
        /// <returns>The message identifier.</returns>
        public virtual long ChannelDataFrameSet(IList<long> channelIds, IList<DataFrame> dataFrames)
        {
            var header = CreateMessageHeader(Protocols.ChannelDataFrame, MessageTypes.ChannelDataFrame.ChannelDataFrameSet);

            var channelDataFrameSet = new ChannelDataFrameSet()
            {
                Channels = channelIds,
                Data = dataFrames
            };

            return Session.SendMessage(header, channelDataFrameSet);
        }

        /// <summary>
        /// Handles the RequestChannelData event from a customer.
        /// </summary>
        public event ProtocolEventHandler<RequestChannelData, ChannelMetadata> OnRequestChannelData;

        /// <summary>
        /// Handles the RequestChannelData message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="requestChannelData">The RequestChannelData message.</param>
        protected virtual void HandleRequestChannelData(IMessageHeader header, RequestChannelData requestChannelData)
        {
            var args = Notify(OnRequestChannelData, header, requestChannelData, new ChannelMetadata());
            HandleRequestChannelData(args);

            if (!args.Cancel)
            {
                ChannelMetadata(args.Context);
            }
        }

        /// <summary>
        /// Handles the RequestChannelData message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="ProtocolEventArgs{RequestChannelData, ChannelMetadata}"/> instance containing the event data.</param>
        protected virtual void HandleRequestChannelData(ProtocolEventArgs<RequestChannelData, ChannelMetadata> args)
        {
        }
    }
}

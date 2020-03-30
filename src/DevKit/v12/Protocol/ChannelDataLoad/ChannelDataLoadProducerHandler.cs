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
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.ChannelDataLoad
{
    /// <summary>
    /// Base implementation of the <see cref="IChannelDataLoadProducer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.ChannelDataLoad.IChannelDataLoadProducer" />
    public class ChannelDataLoadProducerHandler : Etp12ProtocolHandler, IChannelDataLoadProducer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelDataLoadProducerHandler"/> class.
        /// </summary>
        public ChannelDataLoadProducerHandler() : base((int)Protocols.ChannelDataLoad, "producer", "consumer")
        {
            RegisterMessageHandler<OpenChannelsResponse>(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.OpenChannelsResponse, HandleOpenChannelsResponse);
        }

        /// <summary>
        /// Sends a OpenChannels message to a store.
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <returns>The message identifier.</returns>
        public virtual long OpenChannels(IList<ChannelMetadataRecord> channels)
        {
            var header = CreateMessageHeader(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.OpenChannels);

            var message = new OpenChannels
            {
                Channels = channels.ToMap(),
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Handles the OpenChannelsResponse event from a store.
        /// </summary>
        public event ProtocolEventHandler<OpenChannelsResponse> OnOpenChannelsResponse;

        /// <summary>
        /// Sends a CloseChannel message to a consumer.
        /// </summary>
        /// <param name="channelIds">The channel IDs.</param>
        /// <returns>The message identifier.</returns>
        public virtual long CloseChannel(IList<long> channelIds)
        {
            var header = CreateMessageHeader(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.CloseChannel);

            var message = new CloseChannel
            {
                Id = channelIds.ToMap(),
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Sends a RealtimeData message to a store.
        /// </summary>
        /// <param name="dataItems">The data items.</param>
        /// <returns>The message identifier.</returns>
        public virtual long RealtimeData(IList<DataItem> dataItems)
        {
            var header = CreateMessageHeader(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.RealtimeData);

            var message = new RealtimeData
            {
                Data = dataItems
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Sends a ReplaceRange message to a consumer.
        /// </summary>
        /// <param name="channelIds">The IDs of the channels that are changing.</param>
        /// <param name="changedInterval">The indexes that define the interval that is changing.</param>
        /// <param name="dataItems">The channel data of the changed interval.</param>
        /// <returns>The message identifier.</returns>
        public virtual long ReplaceRange(IList<long> channelIds, IndexInterval changedInterval, IList<DataItem> dataItems)
        {
            var header = CreateMessageHeader(Protocols.ChannelSubscribe, MessageTypes.ChannelDataLoad.ReplaceRange);

            var message = new ReplaceRange
            {
                ChannelIds = channelIds,
                ChangedInterval = changedInterval,
                Data = dataItems,
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Handles the OpenChannelsResponse message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The OpenChannelsResponse message.</param>
        protected virtual void HandleOpenChannelsResponse(IMessageHeader header, OpenChannelsResponse message)
        {
            var args = Notify(OnOpenChannelsResponse, header, message);
            HandleOpenChannelsResponse(args);
        }

        /// <summary>
        /// Handles the OpenChannelsResponse message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="ProtocolEventArgs{OpenChannelResponse}"/> instance containing the event data.</param>
        protected virtual void HandleOpenChannelsResponse(ProtocolEventArgs<OpenChannelsResponse> args)
        {
        }
    }
}

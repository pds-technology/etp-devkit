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
        public ChannelStreamingProducerHandler() : base((int)Protocols.ChannelStreaming, Roles.Producer, Roles.Consumer)
        {
            RegisterMessageHandler<StartStreaming>(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.StartStreaming, HandleStartStreaming);
            RegisterMessageHandler<StopStreaming>(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.StopStreaming, HandleStopStreaming);
        }

        /// <summary>
        /// Handles the StartStreaming event from a consumer.
        /// </summary>
        public event EventHandler<VoidRequestEventArgs<StartStreaming>> OnStartStreaming;

        /// <summary>
        /// Sends a ChannelMetadata message to a consumer.
        /// </summary>
        /// <param name="channels">The list of <see cref="ChannelMetadataRecord" /> objects.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ChannelMetadata> ChannelMetadata(IList<ChannelMetadataRecord> channels, IMessageHeaderExtension extension = null)
        {
            var body = new ChannelMetadata()
            {
                Channels = channels ?? new List<ChannelMetadataRecord>(),
            };

            return SendNotification(body, extension: extension);
        }

        /// <summary>
        /// Sends a ChannelData message to a consumer.
        /// </summary>
        /// <param name="data">The list of <see cref="DataItem" /> objects.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ChannelData> ChannelData(IList<DataItem> data, IMessageHeaderExtension extension = null)
        {
            var body = new ChannelData()
            {
                Data = data ?? new List<DataItem>(),
            };

            return SendData(body, extension: extension);
        }

        /// <summary>
        /// Sends a TruncateChannels message to a consumer.
        /// </summary>
        /// <param name="channels">The list of <see cref="TruncateInfo" /> objects.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<TruncateChannels> TruncateChannels(IList<TruncateInfo> channels, IMessageHeaderExtension extension = null)
        {
            var body = new TruncateChannels()
            {
                Channels = channels ?? new List<TruncateInfo>(),
            };

            return SendNotification(body, extension: extension);
        }

        /// <summary>
        /// Handles the StopStreaming event from a consumer.
        /// </summary>
        public event EventHandler<VoidRequestEventArgs<StopStreaming>> OnStopStreaming;

        /// <summary>
        /// Handles the StartStreaming message from a consumer.
        /// </summary>
        /// <param name="message">The StartStreaming message.</param>
        protected virtual void HandleStartStreaming(EtpMessage<StartStreaming> message)
        {
            HandleRequestMessage(message, OnStartStreaming, HandleStartStreaming);
        }

        /// <summary>
        /// Handles the StartStreaming message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="VoidRequestEventArgs{StartStreaming}"/> instance containing the event data.</param>
        protected virtual void HandleStartStreaming(VoidRequestEventArgs<StartStreaming> args)
        {
        }

        /// <summary>
        /// Handles the StopStreaming message from a consumer.
        /// </summary>
        /// <param name="message">The StopStreaming message.</param>
        protected virtual void HandleStopStreaming(EtpMessage<StopStreaming> message)
        {
            HandleRequestMessage(message, OnStopStreaming, HandleStopStreaming);
        }

        /// <summary>
        /// Handles the StopStreaming message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="VoidRequestEventArgs{StopStreaming}"/> instance containing the event data.</param>
        protected virtual void HandleStopStreaming(VoidRequestEventArgs<StopStreaming> args)
        {
        }
    }
}

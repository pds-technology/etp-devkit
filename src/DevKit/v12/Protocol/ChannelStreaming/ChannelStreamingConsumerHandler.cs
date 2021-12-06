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
using Energistics.Etp.Common.Protocol.Core;
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
        public ChannelStreamingConsumerHandler() : base((int)Protocols.ChannelStreaming, Roles.Consumer, Roles.Producer)
        {
            ChannelMetadataRecords = new List<ChannelMetadataRecord>();

            RegisterMessageHandler<ChannelMetadata>(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelMetadata, HandleChannelMetadata);
            RegisterMessageHandler<ChannelData>(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelData, HandleChannelData);
            RegisterMessageHandler<TruncateChannels>(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.TruncateChannels, HandleTruncateChannels);
        }

        /// <summary>
        /// Gets the list of <see cref="ChannelMetadataRecords"/> objects returned by the producer.
        /// </summary>
        /// <value>The list of <see cref="ChannelMetadataRecords"/> objects.</value>
        protected IList<ChannelMetadataRecord> ChannelMetadataRecords { get; }

        /// <summary>
        /// Sends a StartStreaming message to a producer.
        /// </summary>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<StartStreaming> StartStreaming(IMessageHeaderExtension extension = null)
        {
            var body = new StartStreaming
            {
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Event raised when there is an exception received in response to a StartStreaming message.
        /// </summary>
        public event EventHandler<VoidResponseEventArgs<StartStreaming>> OnStartStreamingException;

        /// <summary>
        /// Handles the ChannelMetadata event from a producer.
        /// </summary>
        public event EventHandler<FireAndForgetEventArgs<ChannelMetadata>> OnChannelMetadata;

        /// <summary>
        /// Handles the ChannelData event from a producer.
        /// </summary>
        public event EventHandler<FireAndForgetEventArgs<ChannelData>> OnChannelData;

        /// <summary>
        /// Handles the TruncateChannels event from a producer.
        /// </summary>
        public event EventHandler<FireAndForgetEventArgs<TruncateChannels>> OnTruncateChannels;

        /// <summary>
        /// Sends a StopStreaming message to a producer.
        /// </summary>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<StopStreaming> StopStreaming(IMessageHeaderExtension extension = null)
        {
            var body = new StopStreaming
            {
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Event raised when there is an exception received in response to a StopStreaming message.
        /// </summary>
        public event EventHandler<VoidResponseEventArgs<StopStreaming>> OnStopStreamingException;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<StartStreaming>)
                HandleResponseMessage(request as EtpMessage<StartStreaming>, message, OnStartStreamingException, HandleStartStreamingException);
            else if (request is EtpMessage<StopStreaming>)
                HandleResponseMessage(request as EtpMessage<StopStreaming>, message, OnStopStreamingException, HandleStopStreamingException);
        }

        /// <summary>
        /// Handles exceptions to the StartStreaming message from a producer.
        /// </summary>
        /// <param name="args">The <see cref="VoidResponseEventArgs{StartStreaming}"/> instance containing the event data.</param>
        protected virtual void HandleStartStreamingException(VoidResponseEventArgs<StartStreaming> args)
        {
        }

        /// <summary>
        /// Handles the ChannelMetadata message from a producer.
        /// </summary>
        /// <param name="message">The ChannelMetadata message.</param>
        protected virtual void HandleChannelMetadata(EtpMessage<ChannelMetadata> message)
        {
            foreach (var channel in message.Body.Channels)
                ChannelMetadataRecords.Add(channel);

            HandleFireAndForgetMessage(message, OnChannelMetadata, HandleChannelMetadata);
        }

        /// <summary>
        /// Handles the ChannelMetadata message from a producer.
        /// </summary>
        /// <param name="args">The <see cref="FireAndForgetEventArgs{ChannelMetadata}"/> instance containing the event data.</param>
        protected virtual void HandleChannelMetadata(FireAndForgetEventArgs<ChannelMetadata> args)
        {
        }

        /// <summary>
        /// Handles the ChannelData message from a producer.
        /// </summary>
        /// <param name="message">The ChannelData message.</param>
        protected virtual void HandleChannelData(EtpMessage<ChannelData> message)
        {
            HandleFireAndForgetMessage(message, OnChannelData, HandleChannelData);
        }

        /// <summary>
        /// Handles the ChannelData message from a producer.
        /// </summary>
        /// <param name="args">The <see cref="FireAndForgetEventArgs{ChannelData}"/> instance containing the event data.</param>
        protected virtual void HandleChannelData(FireAndForgetEventArgs<ChannelData> args)
        {
        }

        /// <summary>
        /// Handles the TruncateChannels message from a producer.
        /// </summary>
        /// <param name="message">The TruncateChannels message.</param>
        protected virtual void HandleTruncateChannels(EtpMessage<TruncateChannels> message)
        {
            HandleFireAndForgetMessage(message, OnTruncateChannels, HandleTruncateChannels);
        }

        /// <summary>
        /// Handles the TruncateChannels message from a producer.
        /// </summary>
        /// <param name="args">The <see cref="FireAndForgetEventArgs{TruncateChannels}"/> instance containing the event data.</param>
        protected virtual void HandleTruncateChannels(FireAndForgetEventArgs<TruncateChannels> args)
        {
        }

        /// <summary>
        /// Handles exceptions to the StopStreaming message from a producer.
        /// </summary>
        /// <param name="args">The <see cref="VoidResponseEventArgs{StopStreaming}"/> instance containing the event data.</param>
        protected virtual void HandleStopStreamingException(VoidResponseEventArgs<StopStreaming> args)
        {
        }
    }
}

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
using Energistics.Etp.v11.Datatypes.ChannelData;

namespace Energistics.Etp.v11.Protocol.ChannelStreaming
{
    /// <summary>
    /// Base implementation of the <see cref="IChannelStreamingConsumer"/> interface.
    /// </summary>
    /// <seealso cref="Etp11ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v11.Protocol.ChannelStreaming.IChannelStreamingConsumer" />
    public class ChannelStreamingConsumerHandler : Etp11ProtocolHandlerWithCounterpartCapabilities<CapabilitiesProducer, ICapabilitiesProducer>, IChannelStreamingConsumer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelStreamingConsumerHandler"/> class.
        /// </summary>
        public ChannelStreamingConsumerHandler() : base((int)Protocols.ChannelStreaming, Common.Roles.Consumer, Common.Roles.Producer)
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
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<Start> Start(int maxDataItems = 10000, int maxMessageRate = 1000)
        {
            var body = new Start()
            {
                MaxDataItems = maxDataItems,
                MaxMessageRate = maxMessageRate
            };

            ChannelMetadataRecords.Clear();
            return SendRequest(body);
        }

        /// <summary>
        /// Event raised when there is an exception received in response to a Start message.
        /// </summary>
        public event EventHandler<VoidResponseEventArgs<Start>> OnStartException;

        /// <summary>
        /// Sends a ChannelDescribe message to a producer with the specified URIs.
        /// </summary>
        /// <param name="uris">The list of URIs.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ChannelDescribe> ChannelDescribe(IList<string> uris)
        {
            var body = new ChannelDescribe()
            {
                Uris = uris ?? new List<string>()
            };

            return SendRequest(body, isLongLived: true);
        }

        /// <summary>
        /// Sends a ChannelStreamingStart message to a producer.
        /// </summary>
        /// <param name="channelStreamingInfos">The list of <see cref="ChannelStreamingInfo" /> objects.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ChannelStreamingStart> ChannelStreamingStart(IList<ChannelStreamingInfo> channelStreamingInfos)
        {
            var body = new ChannelStreamingStart()
            {
                Channels = channelStreamingInfos ?? new List<ChannelStreamingInfo>()
            };

            return SendRequest(body);
        }

        /// <summary>
        /// Event raised when there is an exception received in response to a ChannelStreamingStart message.
        /// </summary>
        public event EventHandler<VoidResponseEventArgs<ChannelStreamingStart>> OnChannelStreamingStartException;

        /// <summary>
        /// Sends a ChannelStreamingStop message to a producer.
        /// </summary>
        /// <param name="channelIds">The list of channel identifiers.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ChannelStreamingStop> ChannelStreamingStop(IList<long> channelIds)
        {
            var body = new ChannelStreamingStop()
            {
                Channels = channelIds ?? new List<long>()
            };

            return SendRequest(body);
        }

        /// <summary>
        /// Event raised when there is an exception received in response to a ChannelStreamingStop message.
        /// </summary>
        public event EventHandler<VoidResponseEventArgs<ChannelStreamingStop>> OnChannelStreamingStopException;

        /// <summary>
        /// Sends a ChannelRangeRequest message to a producer.
        /// </summary>
        /// <param name="channelRangeInfos">The list of <see cref="ChannelRangeInfo" /> objects.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ChannelRangeRequest> ChannelRangeRequest(IList<ChannelRangeInfo> channelRangeInfos)
        {
            var body = new ChannelRangeRequest()
            {
                ChannelRanges = channelRangeInfos ?? new List<ChannelRangeInfo>()
            };

            return SendRequest(body);
        }

        /// <summary>
        /// Handles the ChannelMetadata event from a simple streamer producer.
        /// </summary>
        public event EventHandler<FireAndForgetEventArgs<ChannelMetadata>> OnSimpleStreamerChannelMetadata;

        /// <summary>
        /// Handles the ChannelMetadata event from a producer.
        /// </summary>
        public event EventHandler<ResponseEventArgs<ChannelDescribe, ChannelMetadata>> OnBasicStreamerChannelMetadata;

        /// <summary>
        /// Handles the ChannelData event from a producer when sent as streaming data.
        /// </summary>
        public event EventHandler<FireAndForgetEventArgs<ChannelData>> OnStreamingChannelData;

        /// <summary>
        /// Handles the ChannelData event from a producer when sent in response to a ChannelRangeRequest.
        /// </summary>
        public event EventHandler<ResponseEventArgs<ChannelRangeRequest, ChannelData>> OnChannelRangeRequestChannelData;

        /// <summary>
        /// Handles the ChannelDataChange event from a producer.
        /// </summary>
        public event EventHandler<FireAndForgetEventArgs<ChannelDataChange>> OnChannelDataChange;

        /// <summary>
        /// Handles the ChannelStatusChange event from a producer.
        /// </summary>
        public event EventHandler<FireAndForgetEventArgs<ChannelStatusChange>> OnChannelStatusChange;

        /// <summary>
        /// Handles the ChannelDelete event from a producer.
        /// </summary>
        public event EventHandler<FireAndForgetEventArgs<ChannelRemove>> OnChannelRemove;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<Start>)
                HandleResponseMessage(request as EtpMessage<Start>, message, OnStartException, HandleStartException);
            else if (request is EtpMessage<ChannelStreamingStart>)
                HandleResponseMessage(request as EtpMessage<ChannelStreamingStart>, message, OnChannelStreamingStartException, HandleChannelStreamingStartException);
            else if (request is EtpMessage<ChannelStreamingStop>)
                HandleResponseMessage(request as EtpMessage<ChannelStreamingStop>, message, OnChannelStreamingStopException, HandleChannelStreamingStopException);
            else if (request is EtpMessage<ChannelDescribe>)
                HandleResponseMessage(request as EtpMessage<ChannelDescribe>, message, OnBasicStreamerChannelMetadata, HandleBasicStreamerChannelMetadata);
            else if (request is EtpMessage<ChannelRangeRequest>)
                HandleResponseMessage(request as EtpMessage<ChannelRangeRequest>, message, OnChannelRangeRequestChannelData, HandleChannelRangeRequestChannelData);
        }

        /// <summary>
        /// Handles exceptions to the Start message from a producer.
        /// </summary>
        /// <param name="args">The <see cref="VoidResponseEventArgs{Start}"/> instance containing the event data.</param>
        protected virtual void HandleStartException(VoidResponseEventArgs<Start> args)
        {
        }

        /// <summary>
        /// Handles exceptions to the ChannelStreamingStart message from a producer.
        /// </summary>
        /// <param name="args">The <see cref="VoidResponseEventArgs{ChannelStreamingStart}"/> instance containing the event data.</param>
        protected virtual void HandleChannelStreamingStartException(VoidResponseEventArgs<ChannelStreamingStart> args)
        {
        }

        /// <summary>
        /// Handles exceptions to the ChannelStreamingStop message from a producer.
        /// </summary>
        /// <param name="args">The <see cref="VoidResponseEventArgs{ChannelStreamingStop}"/> instance containing the event data.</param>
        protected virtual void HandleChannelStreamingStopException(VoidResponseEventArgs<ChannelStreamingStop> args)
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

            if (CounterpartCapabilities.SimpleStreamer ?? false)
            {
                HandleFireAndForgetMessage(message, OnSimpleStreamerChannelMetadata, HandleSimpleStreamerChannelMetadata);
            }
            else
            {
                var request = TryGetCorrelatedMessage<ChannelDescribe>(message);
                HandleResponseMessage(request, message, OnBasicStreamerChannelMetadata, HandleBasicStreamerChannelMetadata);
            }
        }

        /// <summary>
        /// Handles the ChannelMetadata message from producer that is a simple streamer.
        /// </summary>
        /// <param name="args">The <see cref="FireAndForgetEventArgs{ChannelMetadata}"/> instance containing the event data.</param>
        protected virtual void HandleSimpleStreamerChannelMetadata(FireAndForgetEventArgs<ChannelMetadata> args)
        {
        }

        /// <summary>
        /// Handles the ChannelMetadata message from producer that is a basic streamer.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{ChannelDescribe, ChannelMetadata}"/> instance containing the event data.</param>
        protected virtual void HandleBasicStreamerChannelMetadata(ResponseEventArgs<ChannelDescribe, ChannelMetadata> args)
        {
        }

        /// <summary>
        /// Handles the ChannelData message from a producer.
        /// </summary>
        /// <param name="message">The ChannelData message.</param>
        protected virtual void HandleChannelData(EtpMessage<ChannelData> message)
        {
            if (message.Header.CorrelationId == 0)
            {
                HandleFireAndForgetMessage(message, OnStreamingChannelData, HandleStreamingChannelData);
            }
            else
            {
                var request = TryGetCorrelatedMessage<ChannelRangeRequest>(message);
                HandleResponseMessage(request, message, OnChannelRangeRequestChannelData, HandleChannelRangeRequestChannelData);
            }
        }

        /// <summary>
        /// Handles the ChannelData message when sent as streaming data.
        /// </summary>
        /// <param name="args">The <see cref="FireAndForgetEventArgs{ChannelData}"/> instance containing the event data.</param>
        protected virtual void HandleStreamingChannelData(FireAndForgetEventArgs<ChannelData> args)
        {
        }

        /// <summary>
        /// Handles the ChannelData message when sent in response to a ChannelRangeRequest message.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{ChannelRangeRequest, ChannelData}"/> instance containing the event data.</param>
        protected virtual void HandleChannelRangeRequestChannelData(ResponseEventArgs<ChannelRangeRequest, ChannelData> args)
        {
        }

        /// <summary>
        /// Handles the ChannelDataChange message from a producer.
        /// </summary>
        /// <param name="message">The ChannelDataChange message.</param>
        protected virtual void HandleChannelDataChange(EtpMessage<ChannelDataChange> message)
        {
            HandleFireAndForgetMessage(message, OnChannelDataChange, HandleChannelDataChange);
        }

        /// <summary>
        /// Handles the ChannelDataChange message from a producer.
        /// </summary>
        /// <param name="args">The <see cref="FireAndForgetEventArgs{ChannelDataChange}"/> instance containing the event data.</param>
        protected virtual void HandleChannelDataChange(FireAndForgetEventArgs<ChannelDataChange> args)
        {
        }

        /// <summary>
        /// Handles the ChannelStatusChange message from a producer.
        /// </summary>
        /// <param name="message">The ChannelStatusChange message.</param>
        protected virtual void HandleChannelStatusChange(EtpMessage<ChannelStatusChange> message)
        {
            HandleFireAndForgetMessage(message, OnChannelStatusChange, HandleChannelStatusChange);
        }

        /// <summary>
        /// Handles the ChannelStatusChange message from a producer.
        /// </summary>
        /// <param name="args">The <see cref="FireAndForgetEventArgs{ChannelStatusChange}"/> instance containing the event data.</param>
        protected virtual void HandleChannelStatusChange(FireAndForgetEventArgs<ChannelStatusChange> args)
        {
        }

        /// <summary>
        /// Handles the ChannelRemove message from a producer.
        /// </summary>
        /// <param name="message">The ChannelRemove message.</param>
        protected virtual void HandleChannelRemove(EtpMessage<ChannelRemove> message)
        {
            HandleFireAndForgetMessage(message, OnChannelRemove, HandleChannelRemove);
        }

        /// <summary>
        /// Handles the ChannelRemove message from a producer.
        /// </summary>
        /// <param name="args">The <see cref="FireAndForgetEventArgs{ChannelRemove}"/> instance containing the event data.</param>
        protected virtual void HandleChannelRemove(FireAndForgetEventArgs<ChannelRemove> args)
        {
        }
    }
}

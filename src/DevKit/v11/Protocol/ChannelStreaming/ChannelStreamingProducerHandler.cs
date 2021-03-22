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
using Energistics.Etp.Common.Datatypes.ChannelData;
using Energistics.Etp.v11.Datatypes.ChannelData;

namespace Energistics.Etp.v11.Protocol.ChannelStreaming
{
    /// <summary>
    /// Base implementation of the <see cref="IChannelStreamingProducer"/> interface.
    /// </summary>
    /// <seealso cref="Etp11ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v11.Protocol.ChannelStreaming.IChannelStreamingProducer" />
    public class ChannelStreamingProducerHandler : Etp11ProtocolHandlerWithCapabilities<CapabilitiesProducer, ICapabilitiesProducer>, IChannelStreamingProducer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelStreamingProducerHandler"/> class.
        /// </summary>
        public ChannelStreamingProducerHandler() : base((int)Protocols.ChannelStreaming, Roles.Producer, Roles.Consumer)
        {
            RegisterMessageHandler<Start>(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.Start, HandleStart);
            RegisterMessageHandler<ChannelDescribe>(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelDescribe, HandleChannelDescribe);
            RegisterMessageHandler<ChannelStreamingStart>(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelStreamingStart, HandleChannelStreamingStart);
            RegisterMessageHandler<ChannelStreamingStop>(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelStreamingStop, HandleChannelStreamingStop);
            RegisterMessageHandler<ChannelRangeRequest>(Protocols.ChannelStreaming, MessageTypes.ChannelStreaming.ChannelRangeRequest, HandleChannelRangeRequest);
        }

        /// <summary>
        /// Gets the maximum data items.
        /// </summary>
        /// <value>The maximum data items.</value>
        public int MaxDataItems { get; private set; }

        /// <summary>
        /// Gets the maximum message rate.
        /// </summary>
        /// <value>The maximum message rate.</value>
        public int MaxMessageRate { get; private set; }

        /// <summary>
        /// Handles the Start event from a consumer.
        /// </summary>
        public event EventHandler<VoidRequestEventArgs<Start>> OnStart;

        /// <summary>
        /// Handles the ChannelDescribe event from a consumer.
        /// </summary>
        public event EventHandler<ListRequestEventArgs<ChannelDescribe, ChannelMetadataRecord>> OnChannelDescribe;

        /// <summary>
        /// Sends a ChannelMetadata message to a consumer.
        /// </summary>
        /// <param name="subscriptionUuid">The subscription UUID associated with the ChannelDescribe message that the message to send is correlated with.</param>
        /// <param name="channels">The list of <see cref="ChannelMetadataRecord" /> objects.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ChannelMetadata> ChannelMetadata(Guid subscriptionUuid, IList<ChannelMetadataRecord> channels, bool isFinalPart = true)
        {
            var subscription = TryGetSubscription<IChannelDescribeSubscription>(subscriptionUuid);
            if (subscription == null)
                return null;

            return ChannelMetadata(subscription?.Header, channels, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a ChannelMetadata message to a consumer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="channels">The list of <see cref="ChannelMetadataRecord" /> objects.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ChannelMetadata> ChannelMetadata(IMessageHeader correlatedHeader, IList<ChannelMetadataRecord> channels, bool isFinalPart = true)
        {
            var body = new ChannelMetadata()
            {
                Channels = channels ?? new List<ChannelMetadataRecord>(),
            };

            return SendResponse(body, correlatedHeader, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Handles the ChannelStreamingStart event from a consumer.
        /// </summary>
        public event EventHandler<VoidRequestEventArgs<ChannelStreamingStart>> OnChannelStreamingStart;

        /// <summary>
        /// Handles the ChannelRangeRequest event from a consumer.
        /// </summary>
        public event EventHandler<ListRequestEventArgs<ChannelRangeRequest, DataItem>> OnChannelRangeRequest;

        /// <summary>
        /// Sends a streaming ChannelData message to a consumer.
        /// </summary>
        /// <param name="data">The list of <see cref="DataItem" /> objects.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ChannelData> StreamingChannelData(IList<DataItem> data)
        {
            var body = new ChannelData()
            {
                Data = data ?? new List<DataItem>(),
            };

            return SendData(body, isMultiPart: true);
        }

        /// <summary>
        /// Sends a ChannelData message to a consumer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="dataItems">The list of <see cref="DataItem" /> objects.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ChannelData> ChannelRangeRequestChannelData(IMessageHeader correlatedHeader, IList<DataItem> data, bool isFinalPart = true)
        {
            var body = new ChannelData()
            {
                Data = data ?? new List<DataItem>(),
            };

            return SendResponse(body, correlatedHeader, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a ChannelDataChange message to a consumer.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="dataItems">The data items.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ChannelDataChange> ChannelDataChange(long channelId, long startIndex, long endIndex, IList<DataItem> dataItems)
        {
            var body = new ChannelDataChange()
            {
                ChannelId = channelId,
                StartIndex = startIndex,
                EndIndex = endIndex,
                Data = dataItems ?? new List<DataItem>(),
            };

            return SendNotification(body);
        }

        /// <summary>
        /// Sends a ChannelStatusChange message to a consumer.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="status">The channel status.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ChannelStatusChange> ChannelStatusChange(long channelId, ChannelStatuses status)
        {
            var body = new ChannelStatusChange()
            {
                ChannelId = channelId,
                Status = status
            };

            return SendNotification(body);
        }

        /// <summary>
        /// Sends a ChannelRemove message to a consumer.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="reason">The reason.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ChannelRemove> ChannelRemove(long channelId, string reason = null)
        {
            var body = new ChannelRemove()
            {
                ChannelId = channelId,
                RemoveReason = reason
            };

            return SendNotification(body);
        }

        /// <summary>
        /// Handles the ChannelStreamingStop event from a consumer.
        /// </summary>
        public event EventHandler<VoidRequestEventArgs<ChannelStreamingStop>> OnChannelStreamingStop;

        /// <summary>
        /// Handles the Start message from a consumer.
        /// </summary>
        /// <param name="message">The Start message.</param>
        protected virtual void HandleStart(EtpMessage<Start> message)
        {
            MaxDataItems = message.Body.MaxDataItems;
            MaxMessageRate = message.Body.MaxMessageRate;
            if (Capabilities.SimpleStreamer ?? false) // Treat the start message as the subscription for simple streamers
            {
                var describe = new ChannelDescribe { Uris = new List<string> { EtpUri.RootUri11 } };
                TryRegisterSubscriptions(message, describe.GetSubscriptions(Session.SessionId).ToMap(), nameof(describe.Uris));
            }

            HandleRequestMessage(message, OnStart, HandleStart);
        }

        /// <summary>
        /// Handles the Start message from a consumer.
        /// </summary>
        /// <param name="args">The <see cref="VoidRequestEventArgs{Start}"/> instance containing the event data.</param>
        protected virtual void HandleStart(VoidRequestEventArgs<Start> args)
        {
        }

        /// <summary>
        /// Handles the ChannelDescribe message from a consumer.
        /// </summary>
        /// <param name="message">The ChannelDescribe message.</param>
        protected virtual void HandleChannelDescribe(EtpMessage<ChannelDescribe> message)
        {
            TryRegisterSubscriptions(message, message.Body.GetSubscriptions(Session.SessionId).ToMap(), nameof(message.Body.Uris));

            HandleRequestMessage(message, OnChannelDescribe, HandleChannelDescribe,
                responseMethod: (args) => ChannelMetadata(args.Request?.Header, args.Responses));
        }

        /// <summary>
        /// Handles the ChannelDescribe message from a consumer.
        /// </summary>
        /// <param name="args">The <see cref="ListRequestEventArgs{ChannelDescribe, ChannelMetadataRecord}"/> instance containing the event data.</param>
        protected virtual void HandleChannelDescribe(ListRequestEventArgs<ChannelDescribe, ChannelMetadataRecord> args)
        {
        }

        /// <summary>
        /// Handles the ChannelStreamingStart message from a consumer.
        /// </summary>
        /// <param name="message">The ChannelStreamingStart message.</param>
        protected virtual void HandleChannelStreamingStart(EtpMessage<ChannelStreamingStart> message)
        {
            HandleRequestMessage(message, OnChannelStreamingStart, HandleChannelStreamingStart);
        }

        /// <summary>
        /// Handles the ChannelStreamingStart message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="VoidRequestEventArgs{ChannelStreamingStart}"/> instance containing the event data.</param>
        protected virtual void HandleChannelStreamingStart(VoidRequestEventArgs<ChannelStreamingStart> args)
        {
        }

        /// <summary>
        /// Handles the ChannelRangeRequest message from a consumer.
        /// </summary>
        /// <param name="message">The ChannelRangeRequest message.</param>
        protected virtual void HandleChannelRangeRequest(EtpMessage<ChannelRangeRequest> message)
        {
            HandleRequestMessage(message, OnChannelRangeRequest, HandleChannelRangeRequest,
                responseMethod: (args) => ChannelRangeRequestChannelData(args.Request?.Header, args.Responses));
        }

        /// <summary>
        /// Handles the ChannelRangeRequest message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="ListRequestEventArgs{ChannelRangeRequest, DataItem}"/> instance containing the event data.</param>
        protected virtual void HandleChannelRangeRequest(ListRequestEventArgs<ChannelRangeRequest, DataItem> args)
        {
        }

        /// <summary>
        /// Handles the ChannelStreamingStop message from a consumer.
        /// </summary>
        /// <param name="message">The ChannelStreamingStop message.</param>
        protected virtual void HandleChannelStreamingStop(EtpMessage<ChannelStreamingStop> message)
        {
            HandleRequestMessage(message, OnChannelStreamingStop, HandleChannelStreamingStop);
        }

        /// <summary>
        /// Handles the ChannelStreamingStop message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="VoidRequestEventArgs{ChannelStreamingStop}"/> instance containing the event data.</param>
        protected virtual void HandleChannelStreamingStop(VoidRequestEventArgs<ChannelStreamingStop> args)
        {
        }
    }
}

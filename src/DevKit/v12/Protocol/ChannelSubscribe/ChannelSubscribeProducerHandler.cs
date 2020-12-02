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

namespace Energistics.Etp.v12.Protocol.ChannelSubscribe
{
    /// <summary>
    /// Base implementation of the <see cref="IChannelSubscribeProducer"/> interface.
    /// </summary>
    /// <seealso cref="Energistics.Etp.v12.Protocol.Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.ChannelSubscribe.IChannelSubscribeProducer" />
    public class ChannelSubscribeProducerHandler : Etp12ProtocolHandler, IChannelSubscribeProducer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelSubscribeProducerHandler"/> class.
        /// </summary>
        public ChannelSubscribeProducerHandler() : base((int)Protocols.ChannelSubscribe, "producer", "consumer")
        {
            RegisterMessageHandler<GetChannelMetadata>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.GetChannelMetadata, HandleGetChannelMetadata);
            RegisterMessageHandler<SubscribeChannels>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.SubscribeChannels, HandleSubscribeChannels);
            RegisterMessageHandler<UnsubscribeChannels>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.UnsubscribeChannels, HandleUnsubscribeChannels);
            RegisterMessageHandler<GetRanges>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.GetRanges, HandleGetRanges);
            RegisterMessageHandler<CancelGetRanges>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.CancelGetRanges, HandleCancelGetRanges);
        }

        /// <summary>
        /// Sets limits on maximum indexCount (number of indexes "back" from the current index that a producer will provide) for StreamingStartIndex.
        /// </summary>
        public long MaxIndexCount { get; set; }

        /// <summary>
        /// Indicates the maximum time in integer number of seconds a store allows no streaming data to occur before setting the channelStatus to 'inactive'.
        /// </summary>
        public long StreamingTimeoutPeriod { get; set; }

        /// <summary>
        /// Maximum number of data points to return in each message.
        /// </summary>
        public long CustomerMaxDataItemCount { get; private set; }

        /// <summary>
        /// Gets the capabilities supported by the protocol handler.
        /// </summary>
        /// <param name="capabilities">The protocol's capabilities.</param>
        public override void GetCapabilities(EtpProtocolCapabilities capabilities)
        {
            base.GetCapabilities(capabilities);

            capabilities.MaxIndexCount = MaxIndexCount;
            capabilities.StreamingTimeoutPeriod = StreamingTimeoutPeriod;
        }

        /// <summary>
        /// Sets properties based on counterpart capabilities.
        /// </summary>
        /// <param name="counterpartCapabilities">The counterpart's protocol capabilities.</param>
        public override void OnSessionOpened(EtpProtocolCapabilities counterpartCapabilities)
        {
            base.OnSessionOpened(counterpartCapabilities);

            CustomerMaxDataItemCount = counterpartCapabilities.MaxDataItemCount ?? long.MaxValue;
        }

        /// <summary>
        /// Handles the GetChannelMetadata event from a consumer.
        /// </summary>
        public event ProtocolEventWithErrorsHandler<GetChannelMetadata, ChannelMetadataRecord, ErrorInfo> OnGetChannelMetadata;

        /// <summary>
        /// Sends a GetChannelMetadataResponse message to a consumer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="metadata">The channel metadata records.</param>
        /// <param name="errors">The errors.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long GetChannelMetadataResponse(IMessageHeader request, IDictionary<string, ChannelMetadataRecord> metadata, IDictionary<string, ErrorInfo> errors)
        {
            var header = CreateMessageHeader(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.GetChannelMetadataResponse, request.MessageId);

            var message = new GetChannelMetadataResponse
            {
            };

            return SendMultipartResponse(header, message, metadata, errors, (m, i) => m.Metadata = i);
        }

        /// <summary>
        /// Handles the SubscribeChannels event from a consumer.
        /// </summary>
        public event ProtocolEventWithErrorsHandler<SubscribeChannels, ErrorInfo> OnSubscribeChannels;

        /// <summary>
        /// Sends a ChannelData message to a consumer.
        /// </summary>
        /// <param name="dataItems">The list of <see cref="DataItem" /> objects.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long ChannelData(IList<DataItem> dataItems)
        {
            var header = CreateMessageHeader(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.ChannelData);

            var message = new ChannelData
            {
                Data = dataItems
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Sends a RangeReplaced message to a consumer.
        /// </summary>
        /// <param name="channelIds">The IDs of the channels that are changing.</param>
        /// <param name="changedInterval">The indexes that define the interval that is changing.</param>
        /// <param name="dataItems">The channel data of the changed interval.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long RangeReplaced(IList<long> channelIds, IndexInterval changedInterval, IList<DataItem> dataItems)
        {
            var header = CreateMessageHeader(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.RangeReplaced);

            var message = new RangeReplaced
            {
                ChannelIds = channelIds,
                ChangedInterval = changedInterval,
                Data = dataItems,
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Handles the UnsubscribeChannels event from a consumer.
        /// </summary>
        public event ProtocolEventWithErrorsHandler<UnsubscribeChannels, long, ErrorInfo> OnUnsubscribeChannels;

        /// <summary>
        /// Sends a SubscriptionsStopped message to a consumer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="channelIds">The channel identifiers.</param>
        /// <param name="errors">The errors if any.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long SubscriptionsStopped(IMessageHeader request, IDictionary<string, long> channelIds, IDictionary<string, ErrorInfo> errors)
        {
            var header = CreateMessageHeader(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.SubscriptionsStopped, request?.MessageId ?? 0);
            var message = new SubscriptionsStopped
            {
            };

            return SendMultipartResponse(header, message, channelIds, errors, (m, i) => m.ChannelIds = i);
        }

        /// <summary>
        /// Handles the GetRange event from a consumer.
        /// </summary>
        public event ProtocolEventHandler<GetRanges> OnGetRanges;

        /// <summary>
        /// Sends a GetRangesResponse message to a consumer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="dataItems">The list of <see cref="DataItem" /> objects.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long GetRangesResponse(IMessageHeader request, IList<DataItem> dataItems)
        {
            var header = CreateMessageHeader(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.GetRangesResponse);

            var message = new GetRangesResponse
            {
                Data = dataItems
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Handles the CancelGetRange event from a consumer.
        /// </summary>
        public event ProtocolEventHandler<CancelGetRanges> OnCancelGetRanges;

        /// <summary>
        /// Handles the GetChannelMetadata message from a consumer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GetChannelMetadata message.</param>
        protected virtual void HandleGetChannelMetadata(IMessageHeader header, GetChannelMetadata message)
        {
            var args = Notify(OnGetChannelMetadata, header, message, new Dictionary<string, ChannelMetadataRecord>(), new Dictionary<string, ErrorInfo>());
            if (args.Cancel)
                return;

            if (!HandleGetChannelMetadata(header, message, args.Context, args.Errors))
                return;

            GetChannelMetadataResponse(header, args.Context, args.Errors);
        }

        /// <summary>
        /// Handles the GetChannelMetadata message from a consumer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="response">The response.</param>
        /// <param name="errors">The errors.</param>
        protected virtual bool HandleGetChannelMetadata(IMessageHeader header, GetChannelMetadata message, IDictionary<string, ChannelMetadataRecord> response, IDictionary<string, ErrorInfo> errors)
        {
            return true;
        }

        /// <summary>
        /// Handles the SubscribeChannels message from a consumer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The SubscribeChannels message.</param>
        protected virtual void HandleSubscribeChannels(IMessageHeader header, SubscribeChannels message)
        {
            var args = Notify(OnSubscribeChannels, header, message, new Dictionary<string, ErrorInfo>());
            if (args.Cancel)
                return;

            if (!HandleSubscribeChannels(header, message, args.Errors))
                return;

            SendMultipartResponse(header, message, args.Errors);
        }

        /// <summary>
        /// Handles the SubscribeChannels message from a consumer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="errors">The errors.</param>
        protected virtual bool HandleSubscribeChannels(IMessageHeader header, SubscribeChannels message, IDictionary<string, ErrorInfo> errors)
        {
            return true;
        }

        /// <summary>
        /// Handles the UnsubscribeChannels message from a consumer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The UnsubscribeChannels message.</param>
        protected virtual void HandleUnsubscribeChannels(IMessageHeader header, UnsubscribeChannels message)
        {
            var args = Notify(OnUnsubscribeChannels, header, message, new Dictionary<string, long>(), new Dictionary<string, ErrorInfo>());
            if (args.Cancel)
                return;

            if (!HandleUnsubscribeChannels(header, message, args.Context, args.Errors))
                return;

            SubscriptionsStopped(header, args.Context, args.Errors);
        }

        /// <summary>
        /// Handles the UnsubscribeChannels message from a consumer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="response">The response.</param>
        /// <param name="errors">The errors.</param>
        protected virtual bool HandleUnsubscribeChannels(IMessageHeader header, UnsubscribeChannels message, IDictionary<string, long> response, IDictionary<string, ErrorInfo> errors)
        {
            return true;
        }


        /// <summary>
        /// Handles the GetRanges message from a consumer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GetRanges message.</param>
        protected virtual void HandleGetRanges(IMessageHeader header, GetRanges message)
        {
            Notify(OnGetRanges, header, message);
        }

        /// <summary>
        /// Handles the CancelGetRange message from a consumer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The CancelGetRange message.</param>
        protected virtual void HandleCancelGetRanges(IMessageHeader header, CancelGetRanges message)
        {
            Notify(OnCancelGetRanges, header, message);
        }
    }
}

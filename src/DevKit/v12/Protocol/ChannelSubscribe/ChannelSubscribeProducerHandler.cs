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
            RegisterMessageHandler<GetRange>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.GetRange, HandleGetRange);
            RegisterMessageHandler<CancelGetRange>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.CancelGetRange, HandleCancelGetRange);
        }

        /// <summary>
        /// Sends a GetChannelMetadataResponse message to a consumer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="channelMetadataRecords">The list of <see cref="ChannelMetadataRecord" /> objects.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        public virtual long GetChannelMetadataResponse(IMessageHeader request, IList<ChannelMetadataRecord> channelMetadataRecords, IList<ErrorInfo> errors, MessageFlags messageFlag = MessageFlags.MultiPartAndFinalPart)
        {
            var header = CreateMessageHeader(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.GetChannelMetadataResponse, request.MessageId, messageFlag);

            var channelMetadata = new GetChannelMetadataResponse
            {
                Metadata = channelMetadataRecords ?? new List<ChannelMetadataRecord>(),
                Errors = errors ?? new List<ErrorInfo>()
            };

            return Session.SendMessage(header, channelMetadata);
        }

        /// <summary>
        /// Sends a RealtimeData message to a consumer.
        /// </summary>
        /// <param name="dataItems">The list of <see cref="DataItem" /> objects.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        public virtual long RealtimeData(IList<DataItem> dataItems, MessageFlags messageFlag = MessageFlags.MultiPart)
        {
            var header = CreateMessageHeader(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.RealtimeData, messageFlags: messageFlag);

            var channelData = new RealtimeData
            {
                Data = dataItems
            };

            return Session.SendMessage(header, channelData);
        }

        /// <summary>
        /// Sends a InfillData message to a consumer.
        /// </summary>
        /// <param name="dataItems">The list of <see cref="DataItem" /> objects.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        public virtual long InfillData(IList<DataItem> dataItems, MessageFlags messageFlag = MessageFlags.MultiPart)
        {
            var header = CreateMessageHeader(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.InfillData, messageFlags: messageFlag);

            var channelData = new InfillData
            {
                Data = dataItems
            };

            return Session.SendMessage(header, channelData);
        }

        /// <summary>
        /// Sends a ChangedData message to a consumer.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="uom">The unit of measure.</param>
        /// <param name="depthDatum">The depth datum.</param>
        /// <param name="dataItems">The data items.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        public virtual long ChangedData(object startIndex, object endIndex, string uom, string depthDatum, IList<DataItem> dataItems, MessageFlags messageFlag = MessageFlags.MultiPart)
        {
            var header = CreateMessageHeader(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.ChangedData, messageFlags: messageFlag);

            var channelData = new ChangedData
            {
                ChangedInterval = new IndexInterval
                {
                    StartIndex = new IndexValue { Item = startIndex },
                    EndIndex = new IndexValue { Item = endIndex },
                    Uom = uom,
                    DepthDatum = depthDatum
                },
                Data = dataItems
            };

            return Session.SendMessage(header, channelData);
        }

        /// <summary>
        /// Sends a SubscriptionStopped message to a consumer.
        /// </summary>
        /// <param name="channelIds">The channel identifiers.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        public virtual long SubscriptionStopped(IList<long> channelIds, MessageFlags messageFlag = MessageFlags.MultiPartAndFinalPart)
        {
            var header = CreateMessageHeader(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.SubscriptionStopped, messageFlags: messageFlag);

            var channelData = new SubscriptionStopped
            {
                ChannelIds = channelIds
            };

            return Session.SendMessage(header, channelData);
        }

        /// <summary>
        /// Sends a GetRangeResponse message to a consumer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="dataItems">The list of <see cref="DataItem" /> objects.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        public virtual long GetRangeResponse(IMessageHeader request, IList<DataItem> dataItems, MessageFlags messageFlag = MessageFlags.MultiPart)
        {
            var header = CreateMessageHeader(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.GetRangeResponse, request.MessageId, messageFlag);

            var channelData = new GetRangeResponse
            {
                Data = dataItems
            };

            return Session.SendMessage(header, channelData);
        }

        /// <summary>
        /// Handles the GetChannelMetadata event from a consumer.
        /// </summary>
        public event ProtocolEventHandler<GetChannelMetadata> OnGetChannelMetadata;

        /// <summary>
        /// Handles the SubscribeChannels event from a consumer.
        /// </summary>
        public event ProtocolEventHandler<SubscribeChannels> OnSubscribeChannels;

        /// <summary>
        /// Handles the UnsubscribeChannels event from a consumer.
        /// </summary>
        public event ProtocolEventHandler<UnsubscribeChannels> OnUnsubscribeChannels;

        /// <summary>
        /// Handles the GetRange event from a consumer.
        /// </summary>
        public event ProtocolEventHandler<GetRange> OnGetRange;

        /// <summary>
        /// Handles the CancelGetRange event from a consumer.
        /// </summary>
        public event ProtocolEventHandler<CancelGetRange> OnCancelGetRange;

        /// <summary>
        /// Handles the GetChannelMetadata message from a consumer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="getChannelMetadata">The GetChannelMetadata message.</param>
        protected virtual void HandleGetChannelMetadata(IMessageHeader header, GetChannelMetadata getChannelMetadata)
        {
            var args = Notify(OnGetChannelMetadata, header, getChannelMetadata);
            var metadata = new List<ChannelMetadataRecord>();
            var errors = new List<ErrorInfo>();

            HandleGetChannelMetadata(args, metadata, errors);

            if (!args.Cancel)
            {
                GetChannelMetadataResponse(header, metadata, errors);
            }
        }

        /// <summary>
        /// Handles the GetChannelMetadata message from a consumer.
        /// </summary>
        /// <param name="args">The <see cref="ProtocolEventArgs{GetChannelMetadata}" /> instance containing the event data.</param>
        /// <param name="metadata">The metadata.</param>
        /// <param name="errors">The errors.</param>
        protected virtual void HandleGetChannelMetadata(ProtocolEventArgs<GetChannelMetadata> args, IList<ChannelMetadataRecord> metadata, IList<ErrorInfo> errors)
        {
        }

        /// <summary>
        /// Handles the SubscribeChannels message from a consumer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="subscribeChannels">The SubscribeChannels message.</param>
        protected virtual void HandleSubscribeChannels(IMessageHeader header, SubscribeChannels subscribeChannels)
        {
            Notify(OnSubscribeChannels, header, subscribeChannels);
        }

        /// <summary>
        /// Handles the UnsubscribeChannels message from a consumer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="unsubscribeChannels">The UnsubscribeChannels message.</param>
        protected virtual void HandleUnsubscribeChannels(IMessageHeader header, UnsubscribeChannels unsubscribeChannels)
        {
            Notify(OnUnsubscribeChannels, header, unsubscribeChannels);
        }

        /// <summary>
        /// Handles the GetRange message from a consumer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="getRange">The GetRange message.</param>
        protected virtual void HandleGetRange(IMessageHeader header, GetRange getRange)
        {
            Notify(OnGetRange, header, getRange);
        }

        /// <summary>
        /// Handles the CancelGetRange message from a consumer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="cancelGetRange">The CancelGetRange message.</param>
        protected virtual void HandleCancelGetRange(IMessageHeader header, CancelGetRange cancelGetRange)
        {
            Notify(OnCancelGetRange, header, cancelGetRange);
        }
    }
}

//----------------------------------------------------------------------- 
// ETP DevKit, 1.2
//
// Copyright 2018 Energistics
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
using Avro.IO;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v12.Datatypes.ChannelData;

namespace Energistics.Etp.v12.Protocol.ChannelSubscribe
{
    /// <summary>
    /// Base implementation of the <see cref="IChannelSubscribeConsumer"/> interface.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.ChannelSubscribe.IChannelSubscribeConsumer" />
    public class ChannelSubscribeConsumerHandler : Etp12ProtocolHandler, IChannelSubscribeConsumer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelSubscribeConsumerHandler"/> class.
        /// </summary>
        public ChannelSubscribeConsumerHandler() : base((int)Protocols.ChannelSubscribe, "consumer", "producer")
        {
            ChannelMetadataRecords = new List<ChannelMetadataRecord>(0);
        }

        /// <summary>
        /// Gets the list of <see cref="ChannelMetadataRecords"/> objects returned by the producer.
        /// </summary>
        /// <value>The list of <see cref="ChannelMetadataRecords"/> objects.</value>
        protected IList<ChannelMetadataRecord> ChannelMetadataRecords { get; }

        /// <summary>
        /// Sends a GetChannelMetadata message to a producer with the specified URIs.
        /// </summary>
        /// <param name="uris">The list of URIs.</param>
        /// <returns>The message identifier.</returns>
        public virtual long GetChannelMetadata(IList<string> uris)
        {
            var header = CreateMessageHeader(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.GetChannelMetadata);

            var channelDescribe = new GetChannelMetadata
            {
                Uris = uris
            };

            return Session.SendMessage(header, channelDescribe);
        }

        /// <summary>
        /// Sends a SubscribeChannels message to a producer.
        /// </summary>
        /// <param name="channelSubscribeInfos">The list of <see cref="ChannelSubscribeInfo" /> objects.</param>
        /// <returns>The message identifier.</returns>
        public virtual long SubscribeChannels(IList<ChannelSubscribeInfo> channelSubscribeInfos)
        {
            var header = CreateMessageHeader(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.SubscribeChannels);

            var channelSubscribeStart = new SubscribeChannels
            {
                Channels = channelSubscribeInfos
            };

            return Session.SendMessage(header, channelSubscribeStart);
        }

        /// <summary>
        /// Sends a UnsubscribeChannels message to a producer.
        /// </summary>
        /// <param name="channelIds">The list of channel identifiers.</param>
        /// <returns>The message identifier.</returns>
        public virtual long UnsubscribeChannels(IList<long> channelIds)
        {
            var header = CreateMessageHeader(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.UnsubscribeChannels);

            var channelSubscribeStop = new UnsubscribeChannels
            {
                ChannelIds = channelIds
            };

            return Session.SendMessage(header, channelSubscribeStop);
        }

        /// <summary>
        /// Sends a GetRange message to a producer.
        /// </summary>
        /// <param name="channelRangeInfos">The list of <see cref="ChannelRangeInfo" /> objects.</param>
        /// <returns>The message identifier.</returns>
        public virtual long GetRange(IList<ChannelRangeInfo> channelRangeInfos)
        {
            var header = CreateMessageHeader(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.GetRange);

            var channelRangeRequest = new GetRange
            {
                ChannelRanges = channelRangeInfos
            };

            return Session.SendMessage(header, channelRangeRequest);
        }

        /// <summary>
        /// Handles the GetChannelMetadataResponse event from a producer.
        /// </summary>
        public event ProtocolEventHandler<GetChannelMetadataResponse> OnGetChannelMetadataResponse;

        /// <summary>
        /// Handles the RealtimeData event from a producer.
        /// </summary>
        public event ProtocolEventHandler<RealtimeData> OnRealtimeData;

        /// <summary>
        /// Handles the InfillData event from a producer.
        /// </summary>
        public event ProtocolEventHandler<InfillData> OnInfillData;

        /// <summary>
        /// Handles the ChangedData event from a producer.
        /// </summary>
        public event ProtocolEventHandler<ChangedData> OnChangedData;

        /// <summary>
        /// Handles the SubscriptionStopped event from a producer.
        /// </summary>
        public event ProtocolEventHandler<SubscriptionStopped> OnSubscriptionStopped;

        /// <summary>
        /// Handles the GetRangeResponse event from a producer.
        /// </summary>
        public event ProtocolEventHandler<GetRangeResponse> OnGetRangeResponse;

        /// <summary>
        /// Decodes the message based on the message type contained in the specified <see cref="IMessageHeader" />.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="decoder">The message decoder.</param>
        /// <param name="body">The message body.</param>
        protected override void HandleMessage(IMessageHeader header, Decoder decoder, string body)
        {
            switch (header.MessageType)
            {
                case (int)MessageTypes.ChannelSubscribe.GetChannelMetadataResponse:
                    HandleGetChannelMetadataResponse(header, decoder.Decode<GetChannelMetadataResponse>(body));
                    break;

                case (int)MessageTypes.ChannelSubscribe.RealtimeData:
                    HandleRealtimeData(header, decoder.Decode<RealtimeData>(body));
                    break;

                case (int)MessageTypes.ChannelSubscribe.InfillData:
                    HandleInfillData(header, decoder.Decode<InfillData>(body));
                    break;

                case (int)MessageTypes.ChannelSubscribe.ChangedData:
                    HandleChangedData(header, decoder.Decode<ChangedData>(body));
                    break;

                case (int)MessageTypes.ChannelSubscribe.SubscriptionStopped:
                    HandleSubscriptionStopped(header, decoder.Decode<SubscriptionStopped>(body));
                    break;

                case (int)MessageTypes.ChannelSubscribe.GetRangeResponse:
                    HandleGetRangeResponse(header, decoder.Decode<GetRangeResponse>(body));
                    break;

                default:
                    base.HandleMessage(header, decoder, body);
                    break;
            }
        }

        /// <summary>
        /// Handles the GetChannelMetadataResponse message from a producer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="getChannelMetadataResponse">The GetChannelMetadataResponse message.</param>
        protected virtual void HandleGetChannelMetadataResponse(IMessageHeader header, GetChannelMetadataResponse getChannelMetadataResponse)
        {
            foreach (var channel in getChannelMetadataResponse.Metadata)
                ChannelMetadataRecords.Add(channel);

            Notify(OnGetChannelMetadataResponse, header, getChannelMetadataResponse);
        }

        /// <summary>
        /// Handles the RealtimeData message from a producer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="realtimeData">The RealtimeData message.</param>
        protected virtual void HandleRealtimeData(IMessageHeader header, RealtimeData realtimeData)
        {
            Notify(OnRealtimeData, header, realtimeData);
        }

        /// <summary>
        /// Handles the InfillData message from a producer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="infillData">The InfillData message.</param>
        protected virtual void HandleInfillData(IMessageHeader header, InfillData infillData)
        {
            Notify(OnInfillData, header, infillData);
        }

        /// <summary>
        /// Handles the ChangedData message from a producer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="changedData">The ChangedData message.</param>
        protected virtual void HandleChangedData(IMessageHeader header, ChangedData changedData)
        {
            Notify(OnChangedData, header, changedData);
        }

        /// <summary>
        /// Handles the SubscriptionStopped message from a producer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="subscriptionStopped">The SubscriptionStopped message.</param>
        protected virtual void HandleSubscriptionStopped(IMessageHeader header, SubscriptionStopped subscriptionStopped)
        {
            Notify(OnSubscriptionStopped, header, subscriptionStopped);
        }

        /// <summary>
        /// Handles the GetRangeResponse message from a producer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="getRangeResponse">The GetRangeResponse message.</param>
        protected virtual void HandleGetRangeResponse(IMessageHeader header, GetRangeResponse getRangeResponse)
        {
            Notify(OnGetRangeResponse, header, getRangeResponse);
        }
    }
}

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
using Energistics.Etp.v12.Datatypes.ChannelData;

namespace Energistics.Etp.v12.Protocol.ChannelSubscribe
{
    /// <summary>
    /// Base implementation of the <see cref="IChannelSubscribeConsumer"/> interface.
    /// </summary>
    /// <seealso cref="Energistics.Etp.v12.Protocol.Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.ChannelSubscribe.IChannelSubscribeConsumer" />
    public class ChannelSubscribeConsumerHandler : Etp12ProtocolHandler, IChannelSubscribeConsumer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelSubscribeConsumerHandler"/> class.
        /// </summary>
        public ChannelSubscribeConsumerHandler() : base((int)Protocols.ChannelSubscribe, "consumer", "producer")
        {
            ChannelMetadataRecords = new List<ChannelMetadataRecord>(0);

            RegisterMessageHandler<GetChannelMetadataResponse>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.GetChannelMetadataResponse, HandleGetChannelMetadataResponse);
            RegisterMessageHandler<RealtimeData>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.RealtimeData, HandleRealtimeData);
            RegisterMessageHandler<InfillData>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.InfillData, HandleInfillData);
            RegisterMessageHandler<ChangedData>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.ChangedData, HandleChangedData);
            RegisterMessageHandler<SubscriptionStopped>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.SubscriptionStopped, HandleSubscriptionStopped);
            RegisterMessageHandler<GetRangeResponse>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.GetRangeResponse, HandleGetRangeResponse);
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
        /// <param name="requestUuid">The request identifier.</param>
        /// <param name="channelRangeInfos">The list of <see cref="ChannelRangeInfo" /> objects.</param>
        /// <returns>The message identifier.</returns>
        public virtual long GetRange(Guid requestUuid, IList<ChannelRangeInfo> channelRangeInfos)
        {
            var header = CreateMessageHeader(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.GetRange);

            var channelRangeRequest = new GetRange
            {
                RequestUuid = requestUuid.ToUuid(),
                ChannelRanges = channelRangeInfos
            };

            return Session.SendMessage(header, channelRangeRequest);
        }

        /// <summary>
        /// Sends a CancelGetRange message to a producer.
        /// </summary>
        /// <param name="requestUuid">The request identifier.</param>
        /// <returns>The message identifier.</returns>
        public virtual long CancelGetRange(Guid requestUuid)
        {
            var header = CreateMessageHeader(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.CancelGetRange);

            var cancelGetRange = new CancelGetRange
            {
                RequestUuid = requestUuid.ToUuid()
            };

            return Session.SendMessage(header, cancelGetRange);
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

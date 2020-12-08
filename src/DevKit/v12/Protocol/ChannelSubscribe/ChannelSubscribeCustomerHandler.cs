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
    /// Base implementation of the <see cref="IChannelSubscribeCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Energistics.Etp.v12.Protocol.Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.ChannelSubscribe.IChannelSubscribeCustomer" />
    public class ChannelSubscribeCustomerHandler : Etp12ProtocolHandler, IChannelSubscribeCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelSubscribeCustomerHandler"/> class.
        /// </summary>
        public ChannelSubscribeCustomerHandler() : base((int)Protocols.ChannelSubscribe, "customer", "store")
        {
            MaxDataItemCount = EtpSettings.DefaultMaxDataItemCount;

            ChannelMetadataRecords = new List<ChannelMetadataRecord>(0);

            RegisterMessageHandler<GetChannelMetadataResponse>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.GetChannelMetadataResponse, HandleGetChannelMetadataResponse);
            RegisterMessageHandler<ChannelData>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.ChannelData, HandleChannelData);
            RegisterMessageHandler<RangeReplaced>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.RangeReplaced, HandleRangeReplaced);
            RegisterMessageHandler<SubscriptionsStopped>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.SubscriptionsStopped, HandleSubscriptionsStopped);
            RegisterMessageHandler<GetRangesResponse>(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.GetRangesResponse, HandleGetRangesResponse);
        }

        /// <summary>
        /// Sets limits on maximum indexCount (number of indexes "back" from the current index that a store will provide) for StreamingStartIndex.
        /// </summary>
        public long StoreMaxIndexCount { get; private set; }

        /// <summary>
        /// Indicates the maximum time in integer number of seconds a store allows no streaming data to occur before setting the channelStatus to 'inactive'.
        /// </summary>
        public long StoreStreamingTimeoutPeriod { get; private set; }

        /// <summary>
        /// Maximum number of data points to return in each message.
        /// </summary>
        public long MaxDataItemCount { get; set; }

        /// <summary>
        /// Gets the capabilities supported by the protocol handler.
        /// </summary>
        /// <param name="capabilities">The protocol's capabilities.</param>
        public override void GetCapabilities(EtpProtocolCapabilities capabilities)
        {
            base.GetCapabilities(capabilities);

            capabilities.MaxDataItemCount = MaxDataItemCount;
        }

        /// <summary>
        /// Sets properties based on counterpart capabilities.
        /// </summary>
        /// <param name="counterpartCapabilities">The counterpart's protocol capabilities.</param>
        public override void OnSessionOpened(EtpProtocolCapabilities counterpartCapabilities)
        {
            base.OnSessionOpened(counterpartCapabilities);

            StoreMaxIndexCount = counterpartCapabilities.MaxIndexCount ?? long.MaxValue;
            StoreStreamingTimeoutPeriod = counterpartCapabilities.StreamingTimeoutPeriod ?? long.MaxValue;
        }

        /// <summary>
        /// Gets the list of <see cref="ChannelMetadataRecords"/> objects returned by the store.
        /// </summary>
        /// <value>The list of <see cref="ChannelMetadataRecords"/> objects.</value>
        protected IList<ChannelMetadataRecord> ChannelMetadataRecords { get; }

        /// <summary>
        /// Sends a GetChannelMetadata message to a store with the specified URIs.
        /// </summary>
        /// <param name="uris">The list of URIs.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long GetChannelMetadata(IList<string> uris)
        {
            var header = CreateMessageHeader(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.GetChannelMetadata);

            var channelDescribe = new GetChannelMetadata
            {
                Uris = uris.ToMap(),
            };

            return Session.SendMessage(header, channelDescribe);
        }

        /// <summary>
        /// Handles the GetChannelMetadataResponse event from a store.
        /// </summary>
        public event ProtocolEventHandler<GetChannelMetadataResponse> OnGetChannelMetadataResponse;

        /// <summary>
        /// Sends a SubscribeChannels message to a store.
        /// </summary>
        /// <param name="channels">The list of channels.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long SubscribeChannels(IList<ChannelSubscribeInfo> channels)
        {
            var header = CreateMessageHeader(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.SubscribeChannels);

            var channelSubscribeStart = new SubscribeChannels
            {
                Channels = channels.ToMap(),
            };

            return Session.SendMessage(header, channelSubscribeStart);
        }

        /// <summary>
        /// Handles the ChannelData event from a store.
        /// </summary>
        public event ProtocolEventHandler<ChannelData> OnChannelData;

        /// <summary>
        /// Handles the RangeReplaced event from a store.
        /// </summary>
        public event ProtocolEventHandler<RangeReplaced> OnRangeReplaced;

        /// <summary>
        /// Sends a UnsubscribeChannels message to a store.
        /// </summary>
        /// <param name="channelIds">The list of channel identifiers.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long UnsubscribeChannels(IList<long> channelIds)
        {
            var header = CreateMessageHeader(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.UnsubscribeChannels);

            var channelSubscribeStop = new UnsubscribeChannels
            {
                ChannelIds = channelIds.ToMap(),
            };

            return Session.SendMessage(header, channelSubscribeStop);
        }

        /// <summary>
        /// Handles the SubscriptionsStopped event from a store.
        /// </summary>
        public event ProtocolEventHandler<SubscriptionsStopped> OnSubscriptionsStopped;

        /// <summary>
        /// Sends a GetRanges message to a store.
        /// </summary>
        /// <param name="requestUuid">The request identifier.</param>
        /// <param name="channelRanges">The list of channelRanges.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long GetRanges(Guid requestUuid, IList<ChannelRangeInfo> channelRanges)
        {
            var header = CreateMessageHeader(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.GetRanges);

            var channelRangeRequest = new GetRanges
            {
                RequestUuid = requestUuid.ToUuid(),
                ChannelRanges = channelRanges,
            };

            return Session.SendMessage(header, channelRangeRequest);
        }

        /// <summary>
        /// Handles the GetRangesResponse event from a store.
        /// </summary>
        public event ProtocolEventHandler<GetRangesResponse> OnGetRangesResponse;

        /// <summary>
        /// Sends a CancelGetRanges message to a store.
        /// </summary>
        /// <param name="requestUuid">The request identifier.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long CancelGetRanges(Guid requestUuid)
        {
            var header = CreateMessageHeader(Protocols.ChannelSubscribe, MessageTypes.ChannelSubscribe.CancelGetRanges);

            var cancelGetRange = new CancelGetRanges
            {
                RequestUuid = requestUuid.ToUuid()
            };

            return Session.SendMessage(header, cancelGetRange);
        }

        /// <summary>
        /// Handles the GetChannelMetadataResponse message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GetChannelMetadataResponse message.</param>
        protected virtual void HandleGetChannelMetadataResponse(IMessageHeader header, GetChannelMetadataResponse message)
        {
            foreach (var channel in message.Metadata)
                ChannelMetadataRecords.Add(channel.Value);

            Notify(OnGetChannelMetadataResponse, header, message);
        }

        /// <summary>
        /// Handles the ChannelData message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The ChannelData message.</param>
        protected virtual void HandleChannelData(IMessageHeader header, ChannelData message)
        {
            Notify(OnChannelData, header, message);
        }

        /// <summary>
        /// Handles the RangeReplaced message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The RangeReplaced message.</param>
        protected virtual void HandleRangeReplaced(IMessageHeader header, RangeReplaced message)
        {
            Notify(OnRangeReplaced, header, message);
        }

        /// <summary>
        /// Handles the SubscriptionsStopped message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The SubscriptionStopped message.</param>
        protected virtual void HandleSubscriptionsStopped(IMessageHeader header, SubscriptionsStopped message)
        {
            Notify(OnSubscriptionsStopped, header, message);
        }

        /// <summary>
        /// Handles the GetRangesResponse message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GetRangesResponse message.</param>
        protected virtual void HandleGetRangesResponse(IMessageHeader header, GetRangesResponse message)
        {
            Notify(OnGetRangesResponse, header, message);
        }
    }
}

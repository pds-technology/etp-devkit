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

namespace Energistics.Etp.v12.Protocol.ChannelDataLoad
{
    /// <summary>
    /// Base implementation of the <see cref="IChannelDataLoadConsumer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.ChannelDataLoad.IChannelDataLoadConsumer" />
    public class ChannelDataLoadConsumerHandler : Etp12ProtocolHandler, IChannelDataLoadConsumer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelDataLoadConsumerHandler"/> class.
        /// </summary>
        public ChannelDataLoadConsumerHandler() : base((int)Protocols.ChannelDataLoad, "consumer", "producer")
        {
            RegisterMessageHandler<OpenChannels>(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.OpenChannels, HandleOpenChannels);
            RegisterMessageHandler<CloseChannel>(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.CloseChannel, HandleCloseChannel);
            RegisterMessageHandler<RealtimeData>(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.RealtimeData, HandleRealtimeData);
            RegisterMessageHandler<ReplaceRange>(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.ReplaceRange, HandleReplaceRange);
        }

        /// <summary>
        /// Handles the OpenChannels event from a store.
        /// </summary>
        public event ProtocolEventWithErrorsHandler<OpenChannels, OpenChannelInfo, ErrorInfo> OnOpenChannels;

        /// <summary>
        /// Sends a OpenChannelsResponse message to a store.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="channels">The channels.</param>
        /// <param name="errors">The errors.</param>
        /// <returns>The message identifier.</returns>
        public virtual long OpenChannelsResponse(IMessageHeader request, IDictionary<string, OpenChannelInfo> channels, IDictionary<string, ErrorInfo> errors)
        {
            var header = CreateMessageHeader(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.OpenChannelsResponse, request.MessageId);

            var message = new OpenChannelsResponse
            {
            };

            return SendMultipartResponse(header, message, channels, errors, (m, i) => m.Channels = i);
        }

        /// <summary>
        /// Handles the CloseChannel event from a store.
        /// </summary>
        public event ProtocolEventHandler<CloseChannel> OnCloseChannel;

        /// <summary>
        /// Handles the RealtimeData event from a store.
        /// </summary>
        public event ProtocolEventHandler<RealtimeData> OnRealtimeData;

        /// <summary>
        /// Handles the ReplaceRange event from a store.
        /// </summary>
        public event ProtocolEventHandler<ReplaceRange> OnReplaceRange;

        /// <summary>
        /// Sends a ChannelClosed message to a producer.
        /// </summary>
        /// <param name="channelIds">The IDs of the closed channels.</param>
        /// <returns></returns>
        public virtual long ChannelClosed(IList<long> channelIds)
        {
            var header = CreateMessageHeader(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.ChannelClosed);

            var message = new ChannelClosed
            {
                Id = channelIds.ToMap(),
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Handles the OpenChannels message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The OpenChannels message.</param>
        protected virtual void HandleOpenChannels(IMessageHeader header, OpenChannels message)
        {
            var args = Notify(OnOpenChannels, header, message, new Dictionary<string, OpenChannelInfo>(), new Dictionary<string, ErrorInfo>());
            if (args.Cancel)
                return;

            if (!HandleOpenChannels(header, message, args.Context, args.Errors))
                return;

            OpenChannelsResponse(header, args.Context, args.Errors);
        }

        /// <summary>
        /// Handles the OpenChannel message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="response">The response.</param>
        /// <param name="errors">The errors.</param>
        protected virtual bool HandleOpenChannels(IMessageHeader header, OpenChannels message, IDictionary<string, OpenChannelInfo> response, IDictionary<string, ErrorInfo> errors)
        {
            return true;
        }

        /// <summary>
        /// Handles the CloseChannel message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The CloseChannel message.</param>
        protected virtual void HandleCloseChannel(IMessageHeader header, CloseChannel message)
        {
            Notify(OnCloseChannel, header, message);
        }

        /// <summary>
        /// Handles the RealtimeData message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The RealtimeData message.</param>
        protected virtual void HandleRealtimeData(IMessageHeader header, RealtimeData message)
        {
            Notify(OnRealtimeData, header, message);
        }

        /// <summary>
        /// Handles the ChangedData message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The ChangedData message.</param>
        protected virtual void HandleReplaceRange(IMessageHeader header, ReplaceRange message)
        {
            Notify(OnReplaceRange, header, message);
        }
    }
}

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
using Energistics.Etp.v12.Datatypes;
using Energistics.Etp.v12.Datatypes.ChannelData;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.ChannelDataLoad
{
    /// <summary>
    /// Base implementation of the <see cref="IChannelDataLoadCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.ChannelDataLoad.IChannelDataLoadCustomer" />
    public class ChannelDataLoadCustomerHandler : Etp12ProtocolHandler<CapabilitiesCustomer, ICapabilitiesCustomer, CapabilitiesStore, ICapabilitiesStore>, IChannelDataLoadCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelDataLoadCustomerHandler"/> class.
        /// </summary>
        public ChannelDataLoadCustomerHandler() : base((int)Protocols.ChannelDataLoad, Roles.Customer, Roles.Store)
        {
            RegisterMessageHandler<OpenChannelsResponse>(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.OpenChannelsResponse, HandleOpenChannelsResponse);
            RegisterMessageHandler<ReplaceRangeResponse>(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.ReplaceRangeResponse, HandleReplaceRangeResponse);
            RegisterMessageHandler<TruncateChannelsResponse>(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.TruncateChannelsResponse, HandleTruncateChannelsResponse);
        }

        /// <summary>
        /// Sends a OpenChannels message to a customer.
        /// </summary>
        /// <param name="uris">The channel URIs.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<OpenChannels> OpenChannels(IDictionary<string, string> uris, IMessageHeaderExtension extension = null)
        {
            var body = new OpenChannels
            {
                Uris = uris ?? new Dictionary<string, string>(),
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Sends a OpenChannels message to a customer.
        /// </summary>
        /// <param name="uris">The channel URIs.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<OpenChannels> OpenChannels(IList<string> uris, IMessageHeaderExtension extension = null) => OpenChannels(uris.ToMap(), extension);

        /// <summary>
        /// Handles the OpenChannelsResponse event from a customer.
        /// </summary>
        public event EventHandler<ResponseEventArgs<OpenChannels, OpenChannelsResponse>> OnOpenChannelsResponse;

        /// <summary>
        /// Sends a ChannelData message to a customer.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ChannelData> ChannelData(IList<DataItem> data, IMessageHeaderExtension extension = null)
        {
            var body = new ChannelData
            {
                Data = data ?? new List<DataItem>(),
            };

            return SendData(body, extension: extension);
        }

        /// <summary>
        /// Sends a TruncateChannels message to a store.
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<TruncateChannels> TruncateChannels(IDictionary<string, TruncateInfo> channels, IMessageHeaderExtension extension = null)
        {
            var body = new TruncateChannels
            {
                Channels = channels ?? new Dictionary<string, TruncateInfo>(),
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Sends a TruncateChannels message to a store.
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<TruncateChannels> TruncateChannels(IList<TruncateInfo> channels, IMessageHeaderExtension extension = null) => TruncateChannels(channels.ToMap(), extension);

        /// <summary>
        /// Handles the TruncateChannelsResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<TruncateChannels, TruncateChannelsResponse>> OnTruncateChannelsResponse;

        /// <summary>
        /// Sends a ReplaceRange message to a store.
        /// </summary>
        /// <param name="changedInterval">The changed interval.</param>
        /// <param name="channelIds">The channel IDs.</param>
        /// <param name="data">The changed data.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ReplaceRange> ReplaceRange(IndexInterval changedInterval, IList<long> channelIds, IList<DataItem> data, bool isFinalPart = true, IMessageHeader correlatedHeader = null, IMessageHeaderExtension extension = null)
        {
            var body = new ReplaceRange
            {
                ChangedInterval = changedInterval,
                ChannelIds = channelIds ?? new List<long>(),
                Data = data ?? new List<DataItem>(),
            };

            return SendRequest(body, extension: extension, isMultiPart: true, correlatedHeader: correlatedHeader, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Handles the ReplaceRangeResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<ReplaceRange, ReplaceRangeResponse>> OnReplaceRangeResponse;

        /// <summary>
        /// Sends a CloseChannels message to a store.
        /// </summary>
        /// <param name="channelIds">The channel IDs.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<CloseChannels> CloseChannels(IDictionary<string, long> channelIds, IMessageHeaderExtension extension = null)
        {
            var body = new CloseChannels
            {
                Id = channelIds ?? new Dictionary<string, long>(),
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Sends a CloseChannels message to a store.
        /// </summary>
        /// <param name="channelIds">The channel IDs.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<CloseChannels> CloseChannels(IList<long> channelIds, IMessageHeaderExtension extension = null) => CloseChannels(channelIds.ToMap(), extension);

        /// <summary>
        /// Handles the ChannelsClosed event from a store when sent in response to a CloseChannels.
        /// </summary>
        public event EventHandler<ResponseEventArgs<CloseChannels, ChannelsClosed>> OnResponseChannelsClosed;

        /// <summary>
        /// Handles the ChannelsClosed event from a store when not sent in response to a request.
        /// </summary>
        public event EventHandler<FireAndForgetEventArgs<ChannelsClosed>> OnNotificationChannelsClosed;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<OpenChannels>)
                HandleResponseMessage(request as EtpMessage<OpenChannels>, message, OnOpenChannelsResponse, HandleOpenChannelsResponse);
            else if (request is EtpMessage<TruncateChannels>)
                HandleResponseMessage(request as EtpMessage<TruncateChannels>, message, OnTruncateChannelsResponse, HandleTruncateChannelsResponse);
            else if (request is EtpMessage<ReplaceRange>)
                HandleResponseMessage(request as EtpMessage<ReplaceRange>, message, OnReplaceRangeResponse, HandleReplaceRangeResponse);
            else if (request is EtpMessage<CloseChannels>)
                HandleResponseMessage(request as EtpMessage<CloseChannels>, message, OnResponseChannelsClosed, HandleResponseChannelsClosed);
        }

        /// <summary>
        /// Handles the OpenChannelsResponse message from a store.
        /// </summary>
        /// <param name="message">The OpenChannelsResponse message.</param>
        protected virtual void HandleOpenChannelsResponse(EtpMessage<OpenChannelsResponse> message)
        {
            var request = TryGetCorrelatedMessage<OpenChannels>(message);
            HandleResponseMessage(request, message, OnOpenChannelsResponse, HandleOpenChannelsResponse);
        }

        /// <summary>
        /// Handles the response to an OpenChannelsResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{OpenChannels, OpenChannelsResponse}"/> instance containing the event data.</param>
        protected virtual void HandleOpenChannelsResponse(ResponseEventArgs<OpenChannels, OpenChannelsResponse> args)
        {
        }

        /// <summary>
        /// Handles the TruncateChannelsResponse message from a store.
        /// </summary>
        /// <param name="message">The TruncateChannelsResponse message.</param>
        protected virtual void HandleTruncateChannelsResponse(EtpMessage<TruncateChannelsResponse> message)
        {
            var request = TryGetCorrelatedMessage<TruncateChannels>(message);
            HandleResponseMessage(request, message, OnTruncateChannelsResponse, HandleTruncateChannelsResponse);
        }

        /// <summary>
        /// Handles the response to a TruncateChannels message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{TruncateChannels, TruncateChannelsResponse}"/> instance containing the event data.</param>
        protected virtual void HandleTruncateChannelsResponse(ResponseEventArgs<TruncateChannels, TruncateChannelsResponse> args)
        {
        }

        /// <summary>
        /// Handles the ReplaceRangeResponse message from a store.
        /// </summary>
        /// <param name="message">The ReplaceRangeResponse message.</param>
        protected virtual void HandleReplaceRangeResponse(EtpMessage<ReplaceRangeResponse> message)
        {
            var request = TryGetCorrelatedMessage<ReplaceRange>(message);
            HandleResponseMessage(request, message, OnReplaceRangeResponse, HandleReplaceRangeResponse);
        }

        /// <summary>
        /// Handles the response to a ReplaceRange message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{ReplaceRange, ReplaceRangeResponse}"/> instance containing the event data.</param>
        protected virtual void HandleReplaceRangeResponse(ResponseEventArgs<ReplaceRange, ReplaceRangeResponse> args)
        {
        }

        /// <summary>
        /// Handles the ChannelsClosed message from a stor.
        /// </summary>
        /// <param name="message">The ChannelsClosed message.</param>
        protected virtual void HandleChannelsClosed(EtpMessage<ChannelsClosed> message)
        {
            if (message.Header.CorrelationId == 0)
            {
                HandleFireAndForgetMessage(message, OnNotificationChannelsClosed, HandleNotificationChannelsClosed);
            }
            else
            {
                var request = TryGetCorrelatedMessage<CloseChannels>(message);
                HandleResponseMessage(request, message, OnResponseChannelsClosed, HandleResponseChannelsClosed);
            }
        }

        /// <summary>
        /// Handles the ChannelsClosed message from a store when not sent in response to a request.
        /// </summary>
        /// <param name="args">The <see cref="FireAndForgetEventArgs{ChannelsClosed}"/> instance containing the event data.</param>
        protected virtual void HandleNotificationChannelsClosed(FireAndForgetEventArgs<ChannelsClosed> args)
        {
        }

        /// <summary>
        /// Handles the ChannelsClosed message from a store when sent in response to a CloseChannels.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{CloseChannels, ChannelsClosed}"/> instance containing the event data.</param>
        protected virtual void HandleResponseChannelsClosed(ResponseEventArgs<CloseChannels, ChannelsClosed> args)
        {
        }
    }
}

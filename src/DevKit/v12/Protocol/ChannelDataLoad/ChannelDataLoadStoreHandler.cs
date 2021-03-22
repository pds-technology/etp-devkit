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
using Energistics.Etp.v12.Datatypes;
using Energistics.Etp.v12.Datatypes.ChannelData;

namespace Energistics.Etp.v12.Protocol.ChannelDataLoad
{
    /// <summary>
    /// Base implementation of the <see cref="IChannelDataLoadStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.ChannelDataLoad.IChannelDataLoadStore" />
    public class ChannelDataLoadStoreHandler : Etp12ProtocolHandler<CapabilitiesStore, ICapabilitiesStore, CapabilitiesCustomer, ICapabilitiesCustomer>, IChannelDataLoadStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelDataLoadStoreHandler"/> class.
        /// </summary>
        public ChannelDataLoadStoreHandler() : base((int)Protocols.ChannelDataLoad, Roles.Store, Roles.Customer)
        {
            RegisterMessageHandler<OpenChannels>(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.OpenChannels, HandleOpenChannels);
            RegisterMessageHandler<ChannelData>(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.ChannelData, HandleChannelData);
            RegisterMessageHandler<TruncateChannels>(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.TruncateChannels, HandleTruncateChannels);
            RegisterMessageHandler<ReplaceRange>(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.ReplaceRange, HandleReplaceRange);
            RegisterMessageHandler<CloseChannels>(Protocols.ChannelDataLoad, MessageTypes.ChannelDataLoad.CloseChannels, HandleCloseChannels);
        }

        /// <summary>
        /// Handles the OpenChannels event from a customer.
        /// </summary>
        public event EventHandler<MapRequestEventArgs<OpenChannels, OpenChannelInfo>> OnOpenChannels;

        /// <summary>
        /// Sends a OpenChannelsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="channels">The channels.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<OpenChannelsResponse> OpenChannelsResponse(IMessageHeader correlatedHeader, IDictionary<string, OpenChannelInfo> channels, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new OpenChannelsResponse
            {
                Channels = channels ?? new Dictionary<string, OpenChannelInfo>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a complete multi-part set of OpenChannelsResponse and ProtocolException messages to a customer.
        /// If there are no opened channels, an empty OpenChannelsResponse message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="channels">The channels.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the OpenChannelsResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<OpenChannelsResponse> OpenChannelsResponse(IMessageHeader correlatedHeader, IDictionary<string, OpenChannelInfo> channels, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
        {
            return SendMapResponse(OpenChannelsResponse, correlatedHeader, channels, errors, setFinalPart: setFinalPart, responseExtension: responseExtension, exceptionExtension: exceptionExtension);
        }

        /// <summary>
        /// Handles the ChannelData event from a customer.
        /// </summary>
        public event EventHandler<FireAndForgetEventArgs<ChannelData>> OnChannelData;

        /// <summary>
        /// Handles the TruncateChannels event from a customer.
        /// </summary>
        public event EventHandler<MapRequestEventArgs<TruncateChannels, long>> OnTruncateChannels;

        /// <summary>
        /// Sends a TruncateChannelsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="channelsTruncatedTime">The times at which the channels were truncated.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<TruncateChannelsResponse> TruncateChannelsResponse(IMessageHeader correlatedHeader, IDictionary<string, long> channelsTruncatedTime, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new TruncateChannelsResponse
            {
                ChannelsTruncatedTime = channelsTruncatedTime ?? new Dictionary<string, long>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a complete multi-part set of TruncateChannelsResponse and ProtocolException messages to a customer.
        /// If there are no truncated channels, an empty TruncateChannelsResponse message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="channelsTruncatedTime">The times at which the channels were truncated.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the OpenChannelsResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<TruncateChannelsResponse> TruncateChannelsResponse(IMessageHeader correlatedHeader, IDictionary<string, long> channelsTruncatedTime, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
        {
            return SendMapResponse(TruncateChannelsResponse, correlatedHeader, channelsTruncatedTime, errors, setFinalPart: setFinalPart, responseExtension: responseExtension, exceptionExtension: exceptionExtension);
        }

        /// <summary>
        /// Handles the ReplaceRange event from a customer.
        /// </summary>
        public event EventHandler<RequestEventArgs<ReplaceRange, IDictionary<string, long>>> OnReplaceRange;

        /// <summary>
        /// Sends a ReplaceRangeResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="channelChangeTime">The channel change times.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ReplaceRangeResponse> ReplaceRangeResponse(IMessageHeader correlatedHeader, IDictionary<string, long> channelChangeTime, IMessageHeaderExtension extension = null)
        {
            var body = new ReplaceRangeResponse
            {
                ChannelChangeTime = channelChangeTime ?? new Dictionary<string, long>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension);
        }

        /// <summary>
        /// Handles the CloseChannels event from a customer.
        /// </summary>
        public event EventHandler<MapRequestEventArgs<CloseChannels, long>> OnCloseChannels;

        /// <summary>
        /// Sends a ChannelsClosed message to a customer in response to a CloseChannels message.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="channelIds">The channel IDs.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ChannelsClosed> ResponseChannelsClosed(IMessageHeader correlatedHeader, IDictionary<string, long> channelIds, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new ChannelsClosed
            {
                Id = channelIds ?? new Dictionary<string, long>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a complete multi-part set of ChannelsClosed and ProtocolException messages to a customer in response to a CloseChannels message.
        /// If there are no closed channels, an empty CloseChannels message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="channelIds">The channel IDs.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the ChannelsClosed message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ChannelsClosed> ResponseChannelsClosed(IMessageHeader correlatedHeader, IDictionary<string, long> channelIds, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
        {
            return SendMapResponse(ResponseChannelsClosed, correlatedHeader, channelIds, errors, setFinalPart: setFinalPart, responseExtension: responseExtension, exceptionExtension: exceptionExtension);
        }

        /// <summary>
        /// Sends a ChannelsClosed message to a customer as a notification.
        /// </summary>
        /// <param name="channelIds">The IDs of the closed channels.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ChannelsClosed> NotificationChannelsClosed(IDictionary<string, long> channelIds, IMessageHeaderExtension extension = null)
        {
            var body = new ChannelsClosed
            {
                Id = channelIds ?? new Dictionary<string, long>(),
            };

            return SendNotification(body, extension: extension, isMultiPart: true, isFinalPart: true);
        }

        /// <summary>
        /// Sends a ChannelsClosed message to a customer as a notification.
        /// </summary>
        /// <param name="channelIds">The IDs of the closed channels.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ChannelsClosed> NotificationChannelsClosed(IList<long> channelIds, IMessageHeaderExtension extension = null) => NotificationChannelsClosed(channelIds.ToMap(), extension: extension);

        /// <summary>
        /// Handles the OpenChannels message from a customer.
        /// </summary>
        /// <param name="message">The OpenChannels message.</param>
        protected virtual void HandleOpenChannels(EtpMessage<OpenChannels> message)
        {
            HandleRequestMessage(message, OnOpenChannels, HandleOpenChannels,
                responseMethod: (args) => OpenChannelsResponse(args.Request?.Header, args.ResponseMap, isFinalPart: !args.HasErrors, extension: args.ResponseMapExtension));
        }

        /// <summary>
        /// Handles the response to an OpenChannels message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="MapRequestEventArgs{OpenChannels, OpenChannelInfo, ErrorInfo}"/> instance containing the event data.</param>
        protected virtual void HandleOpenChannels(MapRequestEventArgs<OpenChannels, OpenChannelInfo> args)
        {
        }

        /// <summary>
        /// Handles the ChannelData message from a customer.
        /// </summary>
        /// <param name="message">The ChannelData message.</param>
        protected virtual void HandleChannelData(EtpMessage<ChannelData> message)
        {
            HandleFireAndForgetMessage(message, OnChannelData, HandleChannelData);
        }

        /// <summary>
        /// Handles the ChannelData message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="FireAndForgetEventArgs{ChannelData}"/> instance containing the event data.</param>
        protected virtual void HandleChannelData(FireAndForgetEventArgs<ChannelData> args)
        {
        }

        /// <summary>
        /// Handles the TruncateChannels message from a customer.
        /// </summary>
        /// <param name="message">The TruncateChannels message.</param>
        protected virtual void HandleTruncateChannels(EtpMessage<TruncateChannels> message)
        {
            HandleRequestMessage(message, OnTruncateChannels, HandleTruncateChannels,
                responseMethod: (args) => TruncateChannelsResponse(args.Request?.Header, args.ResponseMap, isFinalPart: !args.HasErrors, extension: args.ResponseMapExtension));
        }

        /// <summary>
        /// Handles the response to an TruncateChannels message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="MapRequestEventArgs{TruncateChannels, long, ErrorInfo}"/> instance containing the event data.</param>
        protected virtual void HandleTruncateChannels(MapRequestEventArgs<TruncateChannels, long> args)
        {
        }

        /// <summary>
        /// Handles the ReplaceRange message from a customer.
        /// </summary>
        /// <param name="message">The ReplaceRange message.</param>
        protected virtual void HandleReplaceRange(EtpMessage<ReplaceRange> message)
        {
            HandleRequestMessage(message, OnReplaceRange, HandleReplaceRange,
                responseMethod: (args) => { if (!args.HasErrors) { ReplaceRangeResponse(args.Request?.Header, args.Response, extension: args.ResponseExtension); } });
        }

        /// <summary>
        /// Handles the response to an ReplaceRange message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="RequestEventArgs{ReplaceRange, IDictionary{string, long}}"/> instance containing the event data.</param>
        protected virtual void HandleReplaceRange(RequestEventArgs<ReplaceRange, IDictionary<string, long>> args)
        {
        }

        /// <summary>
        /// Handles the CloseChannels message from a customer.
        /// </summary>
        /// <param name="message">The CloseChannels message.</param>
        protected virtual void HandleCloseChannels(EtpMessage<CloseChannels> message)
        {
            HandleRequestMessage(message, OnCloseChannels, HandleCloseChannels,
                responseMethod: (args) => ResponseChannelsClosed(args.Request?.Header, args.ResponseMap, isFinalPart: !args.HasErrors, extension: args.ResponseMapExtension));
        }

        /// <summary>
        /// Handles the response to a CloseChannels message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="MapRequestEventArgs{CloseChannels, long, ErrorInfo}"/> instance containing the event data.</param>
        protected virtual void HandleCloseChannels(MapRequestEventArgs<CloseChannels, long> args)
        {
        }
    }
}

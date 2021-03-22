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

namespace Energistics.Etp.v12.Protocol.ChannelDataFrame
{
    /// <summary>
    /// Base implementation of the <see cref="IChannelDataFrameStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.ChannelDataFrame.IChannelDataFrameStore" />
    public class ChannelDataFrameStoreHandler : Etp12ProtocolHandler<CapabilitiesStore, ICapabilitiesStore, CapabilitiesCustomer, ICapabilitiesCustomer>, IChannelDataFrameStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelDataFrameStoreHandler"/> class.
        /// </summary>
        public ChannelDataFrameStoreHandler() : base((int)Protocols.ChannelDataFrame, Roles.Store, Roles.Customer)
        {
            RegisterMessageHandler<GetFrameMetadata>(Protocols.ChannelDataFrame, MessageTypes.ChannelDataFrame.GetFrameMetadata, HandleGetFrameMetadata);
            RegisterMessageHandler<GetFrame>(Protocols.ChannelDataFrame, MessageTypes.ChannelDataFrame.GetFrame, HandleGetFrame);
            RegisterMessageHandler<CancelGetFrame>(Protocols.ChannelDataFrame, MessageTypes.ChannelDataFrame.CancelGetFrame, HandleCancelGetFrame);
        }

        /// <summary>
        /// Handles the GetFrameMetadata event from a customer.
        /// </summary>
        public event EventHandler<RequestEventArgs<GetFrameMetadata, FrameMetadata>> OnGetFrameMetadata;

        /// <summary>
        /// Sends a GetFrameMetadataResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="uri">The frame URI.</param>
        /// <param name="indexes">The index metadata.</param>
        /// <param name="channels">The channel metadata.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetFrameMetadataResponse> GetFrameMetadataResponse(IMessageHeader correlatedHeader, string uri, IList<IndexMetadataRecord> indexes, IList<FrameChannelMetadataRecord> channels, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new GetFrameMetadataResponse()
            {
                Uri = uri,
                Indexes = indexes ?? new List<IndexMetadataRecord>(),
                Channels = channels ?? new List<FrameChannelMetadataRecord>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Handles the GetFrame event from a customer.
        /// </summary>
        public event EventHandler<DualListRequestEventArgs<GetFrame, string, FrameRow>> OnGetFrame;

        /// <summary>
        /// Sends a GetFrameResponseHeader message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="requestUuid">The request UUID associated with this response.</param>
        /// <param name="channelUris">The channel URIs.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="unregisterRequest">Whether or not to unregister the request when sending the message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetFrameResponseHeader> GetFrameResponseHeader(IMessageHeader correlatedHeader, Guid requestUuid, IList<string> channelUris, bool isFinalPart = false, bool unregisterRequest = true, IMessageHeaderExtension extension = null)
        {
            var body = new GetFrameResponseHeader()
            {
                ChannelUris = channelUris ?? new List<string>(),
            };

            var message = SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);

            if (unregisterRequest)
                TryUnregisterRequest(requestUuid);

            return message;
        }

        /// <summary>
        /// Sends a GetFrameResponseRows message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="requestUuid">The request UUID associated with this response.</param>
        /// <param name="frame">The frame.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="unregisterRequest">Whether or not to unregister the request when sending the message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetFrameResponseRows> GetFrameResponseRows(IMessageHeader correlatedHeader, Guid requestUuid, IList<FrameRow> frame, bool isFinalPart = true, bool unregisterRequest = true, IMessageHeaderExtension extension = null)
        {
            var body = new GetFrameResponseRows()
            {
                Frame = frame ?? new List<FrameRow>(),
            };

            var message = SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);

            if (unregisterRequest)
                TryUnregisterRequest(requestUuid);

            return message;
        }

        /// <summary>
        /// Sends a complete multi-part set of GetFrameResponseHeader and GetFrameResponseRows messages to a customer.
        /// If there are no frames, an empty GetFrameResponseHeader message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="requestUuid">The request UUID associated with this response.</param>
        /// <param name="channelUris">The channel URIs.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="unregisterRequest">Whether or not to unregister the request when sending the message.</param>
        /// <param name="headerExtension">The message header extension for the GetFrameResponseHeader message.</param>
        /// <param name="rowsExtension">The message header extension for the GetFrameResponseRows message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetFrameResponseHeader> GetFrameResponse(IMessageHeader correlatedHeader, Guid requestUuid, IList<string> channelUris, IList<FrameRow> frame, bool setFinalPart = true, bool unregisterRequest = true, IMessageHeaderExtension headerExtension = null, IMessageHeaderExtension rowsExtension = null)
        {
            if (frame == null || frame.Count == 0)
                return GetFrameResponseHeader(correlatedHeader, requestUuid, null, isFinalPart: setFinalPart, unregisterRequest: unregisterRequest, extension: headerExtension);

            var message = GetFrameResponseHeader(correlatedHeader, requestUuid, channelUris, isFinalPart: false, unregisterRequest: false, extension: headerExtension);
            if (message == null)
            {
                if (unregisterRequest)
                    TryUnregisterRequest(requestUuid);

                return null;
            }

            var ret = GetFrameResponseRows(correlatedHeader, requestUuid, frame, isFinalPart: setFinalPart, unregisterRequest: unregisterRequest, extension: rowsExtension);
            if (ret == null)
                return null;

            return message;
        }

        /// <summary>
        /// Handles the CancelGetFrame event from a customer.
        /// </summary>
        public event EventHandler<CancellationRequestEventArgs<GetFrame, CancelGetFrame>> OnCancelGetFrame;

        /// <summary>
        /// Handles the GetFrameMetadata message from a customer.
        /// </summary>
        /// <param name="message">The GetFrameMetadata message.</param>
        protected virtual void HandleGetFrameMetadata(EtpMessage<GetFrameMetadata> message)
        {
            HandleRequestMessage(message, OnGetFrameMetadata, HandleGetFrameMetadata,
                responseMethod: (args) => GetFrameMetadataResponse(args.Request?.Header, args.Response?.Uri, args.Response?.Indexes, args.Response?.Channels, isFinalPart: !args.HasErrors, extension: args.ResponseExtension));
        }

        /// <summary>
        /// Handles the GetFrameMetadata message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="RequestEventArgs{GetFrameMetadata, FrameMetadata}"/> instance containing the event data.</param>
        protected virtual void HandleGetFrameMetadata(RequestEventArgs<GetFrameMetadata, FrameMetadata> args)
        {
        }

        /// <summary>
        /// Handles the GetFrame message from a customer.
        /// </summary>
        /// <param name="message">The GetFrame message.</param>
        protected virtual void HandleGetFrame(EtpMessage<GetFrame> message)
        {
            var error = TryRegisterRequest(message.Body, nameof(message.Body.RequestUuid), message);

            HandleRequestMessage(message, OnGetFrame, HandleGetFrame,
                args: new DualListRequestEventArgs<GetFrame, string, FrameRow>(message) { FinalError = error },
                responseMethod: (args) => GetFrameResponse(args.Request?.Header, message.Body.RequestUuid.ToGuid(), args.Responses1, args.Responses2, setFinalPart: !args.HasErrors, unregisterRequest: true, headerExtension: args.Response1Extension, rowsExtension: args.Response2Extension));
        }

        /// <summary>
        /// Handles the GetFrame message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="DualListRequestEventArgs{GetFrame, string, FrameRow}"/> instance containing the event data.</param>
        protected virtual void HandleGetFrame(DualListRequestEventArgs<GetFrame, string, FrameRow> args)
        {
        }

        /// <summary>
        /// Handles the CancelGetFrame message from a customer.
        /// </summary>
        /// <param name="message">The CancelGetFrame message.</param>
        protected virtual void HandleCancelGetFrame(EtpMessage<CancelGetFrame> message)
        {
            EtpMessage<GetFrame> request;
            var error = TryGetRequest(message.Body, nameof(message.Body.RequestUuid), message, out request);

            HandleCancellationMessage(request, message, OnCancelGetFrame, HandleCancelGetFrame,
                args: new CancellationRequestEventArgs<GetFrame, CancelGetFrame>(request, message) { FinalError = error });
        }

        /// <summary>
        /// Handles the CancelGetFrame message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="CancellationRequestEventArgs{GetFrame, CancelGetFrame}"/> instance containing the event data.</param>
        protected virtual void HandleCancelGetFrame(CancellationRequestEventArgs<GetFrame, CancelGetFrame> args)
        {
        }
    }
}

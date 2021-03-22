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

using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Protocol.Core;
using Energistics.Etp.v12.Datatypes;
using Energistics.Etp.v12.Datatypes.Object;
using System;

namespace Energistics.Etp.v12.Protocol.ChannelDataFrame
{
    /// <summary>
    /// Base implementation of the <see cref="IChannelDataFrameCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.ChannelDataFrame.IChannelDataFrameCustomer" />
    public class ChannelDataFrameCustomerHandler : Etp12ProtocolHandler<CapabilitiesCustomer, ICapabilitiesCustomer, CapabilitiesStore, ICapabilitiesStore>, IChannelDataFrameCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelDataFrameCustomerHandler"/> class.
        /// </summary>
        public ChannelDataFrameCustomerHandler() : base((int)Protocols.ChannelDataFrame, Roles.Customer, Roles.Store)
        {
            RegisterMessageHandler<GetFrameMetadataResponse>(Protocols.ChannelDataFrame, MessageTypes.ChannelDataFrame.GetFrameMetadataResponse, HandleGetFrameMetadataResponse);
            RegisterMessageHandler<GetFrameResponseHeader>(Protocols.ChannelDataFrame, MessageTypes.ChannelDataFrame.GetFrameResponseHeader, HandleGetFrameResponseHeader);
            RegisterMessageHandler<GetFrameResponseRows>(Protocols.ChannelDataFrame, MessageTypes.ChannelDataFrame.GetFrameResponseRows, HandleGetFrameResponseRows);
        }

        /// <summary>
        /// Sends a GetFrameMetadata message to a store.
        /// </summary>
        /// <param name="uri">The frame URI.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetFrameMetadata> GetFrameMetadata(string uri, IMessageHeaderExtension extension = null)
        {
            var body = new GetFrameMetadata()
            {
                Uri = uri,
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Handles the GetFrameResponseHeader or GetFrameResponseRows events from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<GetFrameMetadata, GetFrameMetadataResponse>> OnGetFrameMetadataResponse;

        /// <summary>
        /// Sends a GetFrame message to a store.
        /// </summary>
        /// <param name="uri">The frame URI.</param>
        /// <param name="requestedInterval">The requested interval.</param>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetFrame> GetFrame(string uri, IndexInterval requestedInterval, Guid requestUuid, IMessageHeaderExtension extension = null)
        {
            var body = new GetFrame()
            {
                Uri = uri,
                RequestedInterval = requestedInterval,
                RequestUuid = requestUuid.ToUuid<Uuid>(),
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Handles the GetFrameResponseHeader or GetFrameResponseRows events from a store.
        /// </summary>
        public event EventHandler<DualResponseEventArgs<GetFrame, GetFrameResponseHeader, GetFrameResponseRows>> OnGetFrameResponse;

        /// <summary>
        /// Sends a CancelGetFrame message to a store.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<CancelGetFrame> CancelGetFrame(Guid requestUuid, IMessageHeaderExtension extension = null)
        {
            var body = new CancelGetFrame()
            {
                RequestUuid = requestUuid.ToUuid<Uuid>(),
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Event raised when there is an exception received in response to a CancelGetFrame message.
        /// </summary>
        public event EventHandler<VoidResponseEventArgs<CancelGetFrame>> OnCancelGetFrameException;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<GetFrameMetadata>)
                HandleResponseMessage(request as EtpMessage<GetFrameMetadata>, message, OnGetFrameMetadataResponse, HandleGetFrameMetadataResponse);
            else if (request is EtpMessage<GetFrame>)
                HandleResponseMessage(request as EtpMessage<GetFrame>, message, OnGetFrameResponse, HandleGetFrameResponse);
            else if (request is EtpMessage<CancelGetFrame>)
                HandleResponseMessage(request as EtpMessage<CancelGetFrame>, message, OnCancelGetFrameException, HandleCancelGetFrameException);
        }

        /// <summary>
        /// Handles the GetFrameMetadataResponse message from a store.
        /// </summary>
        /// <param name="message">The GetFrameMetadataResponse message.</param>
        protected virtual void HandleGetFrameMetadataResponse(EtpMessage<GetFrameMetadataResponse> message)
        {
            var request = TryGetCorrelatedMessage<GetFrameMetadata>(message);
            HandleResponseMessage(request, message, OnGetFrameMetadataResponse, HandleGetFrameMetadataResponse);
        }

        /// <summary>
        /// Handles the response to a GetFrame message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{GetFrameMetadata, GetFrameMetadataResponse}"/> instance containing the event data.</param>
        protected virtual void HandleGetFrameMetadataResponse(ResponseEventArgs<GetFrameMetadata, GetFrameMetadataResponse> args)
        {
        }

        /// <summary>
        /// Handles the GetFrameResponseHeader message from a store.
        /// </summary>
        /// <param name="message">The GetFrameResponseHeader message.</param>
        protected virtual void HandleGetFrameResponseHeader(EtpMessage<GetFrameResponseHeader> message)
        {
            var request = TryGetCorrelatedMessage<GetFrame>(message);
            HandleResponseMessage(request, message, OnGetFrameResponse, HandleGetFrameResponse);
        }

        /// <summary>
        /// Handles the GetFrameResponseRows message from a store.
        /// </summary>
        /// <param name="message">The GetFrameResponseRows message.</param>
        protected virtual void HandleGetFrameResponseRows(EtpMessage<GetFrameResponseRows> message)
        {
            var request = TryGetCorrelatedMessage<GetFrame>(message);
            HandleResponseMessage(request, message, OnGetFrameResponse, HandleGetFrameResponse);
        }

        /// <summary>
        /// Handles the response to a GetFrame message from a store.
        /// </summary>
        /// <param name="args">The <see cref="DualResponseEventArgs{GetFrame, GetFrameResponseHeader, GetFrameResponseRows}"/> instance containing the event data.</param>
        protected virtual void HandleGetFrameResponse(DualResponseEventArgs<GetFrame, GetFrameResponseHeader, GetFrameResponseRows> args)
        {
        }

        /// <summary>
        /// Handles exceptions to the CancelGetFrame message from a store.
        /// </summary>
        /// <param name="args">The <see cref="VoidResponseEventArgs{CancelGetFrame}"/> instance containing the event data.</param>
        protected virtual void HandleCancelGetFrameException(VoidResponseEventArgs<CancelGetFrame> args)
        {
        }
    }
}

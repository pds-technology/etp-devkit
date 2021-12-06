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
    /// Defines the interface that must be implemented by the store role of the ChannelDataFrame protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.ChannelDataFrame, Roles.Store, Roles.Customer)]
    public interface IChannelDataFrameStore : IProtocolHandler<ICapabilitiesStore, ICapabilitiesCustomer>
    {
        /// <summary>
        /// Handles the GetFrameMetadata event from a customer.
        /// </summary>
        event EventHandler<RequestEventArgs<GetFrameMetadata, FrameMetadata>> OnGetFrameMetadata;

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
        EtpMessage<GetFrameMetadataResponse> GetFrameMetadataResponse(IMessageHeader correlatedHeader, string uri, IList<IndexMetadataRecord> indexes, IList<FrameChannelMetadataRecord> channels, bool isFinalPart = true, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the GetFrame event from a customer.
        /// </summary>
        event EventHandler<SingleAndListRequestEventArgs<GetFrame, FrameHeader, FrameRow>> OnGetFrame;

        /// <summary>
        /// Sends a GetFrameResponseHeader message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="requestUuid">The request UUID associated with this response.</param>
        /// <param name="indexes">The index metadata.</param>
        /// <param name="channelUris">The channel URIs.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="unregisterRequest">Whether or not to unregister the request when sending the message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetFrameResponseHeader> GetFrameResponseHeader(IMessageHeader correlatedHeader, Guid requestUuid, IList<IndexMetadataRecord> indexes, IList<string> channelUris, bool isFinalPart = false, bool unregisterRequest = false, IMessageHeaderExtension extension = null);

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
        EtpMessage<GetFrameResponseRows> GetFrameResponseRows(IMessageHeader correlatedHeader, Guid requestUuid, IList<FrameRow> frame, bool isFinalPart = true, bool unregisterRequest = true, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a complete multi-part set of GetFrameResponseHeader and GetFrameResponseRows messages to a customer.
        /// If there are no frames, an empty GetFrameResponseHeader message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="requestUuid">The request UUID associated with this response.</param>
        /// <param name="indexes">The index metadata.</param>
        /// <param name="channelUris">The channel URIs.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="unregisterRequest">Whether or not to unregister the request when sending the message.</param>
        /// <param name="headerExtension">The message header extension for the GetFrameResponseHeader message.</param>
        /// <param name="rowsExtension">The message header extension for the GetFrameResponseRows message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetFrameResponseHeader> GetFrameResponse(IMessageHeader correlatedHeader, Guid requestUuid, IList<IndexMetadataRecord> indexes, IList<string> channelUris, IList<FrameRow> frame, bool setFinalPart = true, bool unregisterRequest = true, IMessageHeaderExtension headerExtension = null, IMessageHeaderExtension rowsExtension = null);

        /// <summary>
        /// Handles the CancelGetFrame event from a customer.
        /// </summary>
        event EventHandler<CancellationRequestEventArgs<GetFrame, CancelGetFrame>> OnCancelGetFrame;
    }

    public class FrameHeader
    {
        public IList<IndexMetadataRecord> Indexes { get; set; }

        public IList<string> ChannelUris { get; set; }
    }

    public class FrameMetadata
    {
        public string Uri { get; set; }

        public IList<IndexMetadataRecord> Indexes { get; set; }

        public IList<FrameChannelMetadataRecord> Channels { get; set; }
    }
}

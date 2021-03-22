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
using Energistics.Etp.v12.Datatypes;
using Energistics.Etp.v12.Datatypes.Object;
using System;

namespace Energistics.Etp.v12.Protocol.ChannelDataFrame
{
    /// <summary>
    /// Defines the interface that must be implemented by the customer role of the ChannelDataFrame protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.ChannelDataFrame, Roles.Customer, Roles.Store)]
    public interface IChannelDataFrameCustomer : IProtocolHandler<ICapabilitiesCustomer, ICapabilitiesStore>
    {
        /// <summary>
        /// Sends a GetFrameMetadata message to a store.
        /// </summary>
        /// <param name="uri">The frame URI.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetFrameMetadata> GetFrameMetadata(string uri, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the GetFrameResponseHeader or GetFrameResponseRows events from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<GetFrameMetadata, GetFrameMetadataResponse>> OnGetFrameMetadataResponse;

        /// <summary>
        /// Sends a GetFrame message to a store.
        /// </summary>
        /// <param name="uri">The frame URI.</param>
        /// <param name="requestedInterval">The requested interval.</param>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetFrame> GetFrame(string uri, IndexInterval requestedInterval, Guid requestUuid, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the GetFrameResponseHeader or GetFrameResponseRows events from a store.
        /// </summary>
        event EventHandler<DualResponseEventArgs<GetFrame, GetFrameResponseHeader, GetFrameResponseRows>> OnGetFrameResponse;

        /// <summary>
        /// Sends a CancelGetFrame message to a store.
        /// </summary>
        /// <param name="requestUuid">The request UUID.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<CancelGetFrame> CancelGetFrame(Guid requestUuid, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Event raised when there is an exception received in response to a CancelGetFrame message.
        /// </summary>
        event EventHandler<VoidResponseEventArgs<CancelGetFrame>> OnCancelGetFrameException;
    }
}

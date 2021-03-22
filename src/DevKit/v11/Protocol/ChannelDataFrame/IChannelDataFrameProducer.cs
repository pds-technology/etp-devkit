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
using Energistics.Etp.v11.Datatypes.ChannelData;

namespace Energistics.Etp.v11.Protocol.ChannelDataFrame
{
    /// <summary>
    /// Defines the interface that must be implemented by the producer role of the ChannelDataFrame protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.ChannelDataFrame, Roles.Producer, Roles.Consumer)]
    public interface IChannelDataFrameProducer : IProtocolHandler
    {
        /// <summary>
        /// Sends a ChannelMetadata message to a consumer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="channelMetadata">The channel metadata.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ChannelMetadata> ChannelMetadata(IMessageHeader correlatedHeader, IList<ChannelMetadataRecord> channelMetadata);

        /// <summary>
        /// Sends a ChannelDataFrameSet message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="frameSet">The channel data frame set.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ChannelDataFrameSet> ChannelDataFrameSet(IMessageHeader correlatedHeader, ChannelDataFrameSet frameSet, bool isFinalPart = true);

        /// <summary>
        /// Sends a complete multi-part set of ChannelMetadata and ChannelDataFrameSet messages to a customer.
        /// If there are no frame sets in the list, an empty ChannelDataFrameSet message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="channelMetadata">The list of channel metadata.</param>
        /// <param name="frameSet">The frame set.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last ChannelDataFrameSet message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        EtpMessage<ChannelMetadata> RequestChannelDataResponse(IMessageHeader correlatedHeader, IList<ChannelMetadataRecord> channelMetadata, ChannelDataFrameSet frameSet, bool setFinalPart = true);

        /// <summary>
        /// Handles the RequestChannelData event from a customer.
        /// </summary>
        event EventHandler<ListAndSingleRequestEventArgs<RequestChannelData, ChannelMetadataRecord, ChannelDataFrameSet>> OnRequestChannelData;
    }
}

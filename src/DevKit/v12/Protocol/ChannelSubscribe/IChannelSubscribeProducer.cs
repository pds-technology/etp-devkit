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

namespace Energistics.Etp.v12.Protocol.ChannelSubscribe
{
    /// <summary>
    /// Defines the interface that must be implemented by the producer role of the ChannelSubscribe protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.ChannelSubscribe, "producer", "consumer")]
    public interface IChannelSubscribeProducer : IProtocolHandler
    {
        /// <summary>
        /// Sends a GetChannelMetadataResponse message to a consumer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="channelMetadataRecords">The list of <see cref="ChannelMetadataRecord" /> objects.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        long GetChannelMetadataResponse(IMessageHeader request, IList<ChannelMetadataRecord> channelMetadataRecords, IList<ErrorInfo> errors, MessageFlags messageFlag = MessageFlags.MultiPartAndFinalPart);

        /// <summary>
        /// Sends a RealtimeData message to a consumer.
        /// </summary>
        /// <param name="dataItems">The list of <see cref="DataItem" /> objects.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        long RealtimeData(IList<DataItem> dataItems, MessageFlags messageFlag = MessageFlags.MultiPart);

        /// <summary>
        /// Sends a InfillData message to a consumer.
        /// </summary>
        /// <param name="dataItems">The list of <see cref="DataItem" /> objects.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        long InfillData(IList<DataItem> dataItems, MessageFlags messageFlag = MessageFlags.MultiPart);

        /// <summary>
        /// Sends a ChangedData message to a consumer.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="uom">The unit of measure.</param>
        /// <param name="depthDatum">The depth datum.</param>
        /// <param name="dataItems">The data items.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        long ChangedData(object startIndex, object endIndex, string uom, string depthDatum, IList<DataItem> dataItems, MessageFlags messageFlag = MessageFlags.MultiPart);

        /// <summary>
        /// Sends a SubscriptionStopped message to a consumer.
        /// </summary>
        /// <param name="channelIds">The channel identifiers.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        long SubscriptionStopped(IList<long> channelIds, MessageFlags messageFlag = MessageFlags.MultiPartAndFinalPart);

        /// <summary>
        /// Sends a GetRangeResponse message to a consumer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="dataItems">The data items.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        long GetRangeResponse(IMessageHeader request, IList<DataItem> dataItems, MessageFlags messageFlag = MessageFlags.MultiPart);

        /// <summary>
        /// Handles the GetChannelMetadata event from a consumer.
        /// </summary>
        event ProtocolEventHandler<GetChannelMetadata> OnGetChannelMetadata;

        /// <summary>
        /// Handles the SubscribeChannels event from a consumer.
        /// </summary>
        event ProtocolEventHandler<SubscribeChannels> OnSubscribeChannels;

        /// <summary>
        /// Handles the UnsubscribeChannels event from a consumer.
        /// </summary>
        event ProtocolEventHandler<UnsubscribeChannels> OnUnsubscribeChannels;

        /// <summary>
        /// Handles the GetRange event from a consumer.
        /// </summary>
        event ProtocolEventHandler<GetRange> OnGetRange;

        /// <summary>
        /// Handles the CancelGetRange event from a consumer.
        /// </summary>
        event ProtocolEventHandler<CancelGetRange> OnCancelGetRange;
    }
}

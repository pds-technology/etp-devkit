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
    /// Base implementation of the <see cref="IChannelDataFrameProducer"/> interface.
    /// </summary>
    /// <seealso cref="Etp11ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v11.Protocol.ChannelDataFrame.IChannelDataFrameProducer" />
    public class ChannelDataFrameProducerHandler : Etp11ProtocolHandler, IChannelDataFrameProducer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelDataFrameProducerHandler"/> class.
        /// </summary>
        public ChannelDataFrameProducerHandler() : base((int)Protocols.ChannelDataFrame, Common.Roles.Producer, Common.Roles.Consumer)
        {
            RegisterMessageHandler<RequestChannelData>(Protocols.ChannelDataFrame, MessageTypes.ChannelDataFrame.RequestChannelData, HandleRequestChannelData);
        }

        /// <summary>
        /// Sends a ChannelMetadata message to a consumer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="channelMetadata">The channel metadata.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ChannelMetadata> ChannelMetadata(IMessageHeader correlatedHeader, IList<ChannelMetadataRecord> channelMetadata)
        {
            var body = new ChannelMetadata
            {
                Channels = channelMetadata ?? new List<ChannelMetadataRecord>(),
            };

            return SendResponse(body, correlatedHeader, isMultiPart: true, isNoData: channelMetadata?.Count == 0);
        }

        /// <summary>
        /// Sends a ChannelDataFrameSet message to a consumer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="frameSet">The channel data frame set.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ChannelDataFrameSet> ChannelDataFrameSet(IMessageHeader correlatedHeader, ChannelDataFrameSet frameSet, bool isFinalPart = true)
        {
            var body = frameSet;

            return SendResponse(body, correlatedHeader, isMultiPart: true, isFinalPart: isFinalPart, isNoData: frameSet?.Data?.Count == 0);
        }

        /// <summary>
        /// Sends a complete multi-part set of ChannelMetadata and ChannelDataFrameSet messages to a consumer.
        /// If there are no frame sets in the list, an empty ChannelDataFrameSet message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="channelMetadata">The list of channel metadata.</param>
        /// <param name="frameSet">The frame set.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last ChannelDataFrameSet message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ChannelMetadata> RequestChannelDataResponse(IMessageHeader correlatedHeader, IList<ChannelMetadataRecord> channelMetadata, ChannelDataFrameSet frameSet, bool setFinalPart = true)
        {
            var message = ChannelMetadata(correlatedHeader, channelMetadata ?? new List<ChannelMetadataRecord>());

            if (frameSet != null)
            {
                var ret = ChannelDataFrameSet(correlatedHeader, frameSet, isFinalPart: setFinalPart);
                if (ret == null)
                    return null;
            }

            return message;
        }

        /// <summary>
        /// Handles the RequestChannelData event from a consumer.
        /// </summary>
        public event EventHandler<ListAndSingleRequestEventArgs<RequestChannelData, ChannelMetadataRecord, ChannelDataFrameSet>> OnRequestChannelData;

        /// <summary>
        /// Handles the RequestChannelData message from a consumer.
        /// </summary>
        /// <param name="message">The RequestChannelData message.</param>
        protected virtual void HandleRequestChannelData(EtpMessage<RequestChannelData> message)
        {
            HandleRequestMessage(message, OnRequestChannelData, HandleRequestChannelData,
                responseMethod: (args) => RequestChannelDataResponse(args.Request?.Header, args.Responses1, args.Response2, setFinalPart: !args.HasErrors));
        }

        /// <summary>
        /// Handles the RequestChannelData message from a consumer.
        /// </summary>
        /// <param name="args">The <see cref="ListAndSingleRequestEventArgs{RequestChannelData, ChannelMetadataRecord, ChannelDataFrameSet}"/> instance containing the event data.</param>
        protected virtual void HandleRequestChannelData(ListAndSingleRequestEventArgs<RequestChannelData, ChannelMetadataRecord, ChannelDataFrameSet> args)
        {
        }
    }
}

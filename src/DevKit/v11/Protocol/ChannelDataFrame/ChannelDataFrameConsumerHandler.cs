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
using Energistics.Etp.Common.Protocol.Core;
using System;

namespace Energistics.Etp.v11.Protocol.ChannelDataFrame
{
    /// <summary>
    /// Base implementation of the <see cref="IChannelDataFrameConsumer"/> interface.
    /// </summary>
    /// <seealso cref="Etp11ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v11.Protocol.ChannelDataFrame.IChannelDataFrameConsumer" />
    public class ChannelDataFrameConsumerHandler : Etp11ProtocolHandler, IChannelDataFrameConsumer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelDataFrameConsumerHandler"/> class.
        /// </summary>
        public ChannelDataFrameConsumerHandler() : base((int)Protocols.ChannelDataFrame, Roles.Consumer, Roles.Producer)
        {
            RegisterMessageHandler<ChannelMetadata>(Protocols.ChannelDataFrame, MessageTypes.ChannelDataFrame.ChannelMetadata, HandleChannelMetadata);
            RegisterMessageHandler<ChannelDataFrameSet>(Protocols.ChannelDataFrame, MessageTypes.ChannelDataFrame.ChannelDataFrameSet, HandleChannelDataFrameSet);
        }

        /// <summary>
        /// Sends a RequestChannelData message to a producer.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="fromIndex">From index.</param>
        /// <param name="toIndex">To index.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<RequestChannelData> RequestChannelData(string uri, long? fromIndex = null, long? toIndex = null)
        {
            var body = new RequestChannelData
            {
                Uri = uri,
                FromIndex = fromIndex,
                ToIndex = toIndex
            };

            return SendRequest(body);
        }

        /// <summary>
        /// Handles the ChannelMetadata or ChannelDataFrameSet event from a producer.
        /// </summary>
        public event EventHandler<DualResponseEventArgs<RequestChannelData, ChannelMetadata, ChannelDataFrameSet>> OnRequestChannelDataResponse;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<RequestChannelData>)
                HandleResponseMessage(request as EtpMessage<RequestChannelData>, message, OnRequestChannelDataResponse, HandleRequestChannelDataResponse);
        }

        /// <summary>
        /// Handles the ChannelMetadata message from a producer.
        /// </summary>
        /// <param name="message">The ChannelMetadata message.</param>
        protected virtual void HandleChannelMetadata(EtpMessage<ChannelMetadata> message)
        {
            var request = TryGetCorrelatedMessage<RequestChannelData>(message);
            HandleResponseMessage(request, message, OnRequestChannelDataResponse, HandleRequestChannelDataResponse);
        }

        /// <summary>
        /// Handles the ChannelDataFrameSet message from a producer.
        /// </summary>
        /// <param name="message">The ChannelDataFrameSet message.</param>
        protected virtual void HandleChannelDataFrameSet(EtpMessage<ChannelDataFrameSet> message)
        {
            var request = TryGetCorrelatedMessage<RequestChannelData>(message);
            HandleResponseMessage(request, message, OnRequestChannelDataResponse, HandleRequestChannelDataResponse);
        }

        /// <summary>
        /// Handles the response to a RequestChannelData message from a producer.
        /// </summary>
        /// <param name="args">The <see cref="DualResponseEventArgs{RequestChannelData, ChannelMetadata, ChannelDataFrameSet}"/> instance containing the event data.</param>
        protected virtual void HandleRequestChannelDataResponse(DualResponseEventArgs<RequestChannelData, ChannelMetadata, ChannelDataFrameSet> args)
        {
        }
    }
}

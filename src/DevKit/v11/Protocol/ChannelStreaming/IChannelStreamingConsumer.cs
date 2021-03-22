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

namespace Energistics.Etp.v11.Protocol.ChannelStreaming
{
    /// <summary>
    /// Defines the interface that must be implemented by the consumer role of the ChannelStreaming protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.ChannelStreaming, Roles.Consumer, Roles.Producer)]
    public interface IChannelStreamingConsumer : IProtocolHandlerWithCounterpartCapabilities<ICapabilitiesProducer>
    {
        /// <summary>
        /// Sends a Start message to a producer with the specified throttling parameters.
        /// </summary>
        /// <param name="maxDataItems">The maximum data items.</param>
        /// <param name="maxMessageRate">The maximum message rate.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<Start> Start(int maxDataItems = 10000, int maxMessageRate = 1000);

        /// <summary>
        /// Event raised when there is an exception received in response to a Start message.
        /// </summary>
        event EventHandler<VoidResponseEventArgs<Start>> OnStartException;

        /// <summary>
        /// Sends a ChannelDescribe message to a producer with the specified URIs.
        /// </summary>
        /// <param name="uris">The list of URIs.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ChannelDescribe> ChannelDescribe(IList<string> uris);

        /// <summary>
        /// Sends a ChannelStreamingStart message to a producer.
        /// </summary>
        /// <param name="channelStreamingInfos">The list of <see cref="ChannelStreamingInfo"/> objects.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ChannelStreamingStart> ChannelStreamingStart(IList<ChannelStreamingInfo> channelStreamingInfos);

        /// <summary>
        /// Event raised when there is an exception received in response to a ChannelStreamingStart message.
        /// </summary>
        event EventHandler<VoidResponseEventArgs<ChannelStreamingStart>> OnChannelStreamingStartException;

        /// <summary>
        /// Sends a ChannelStreamingStop message to a producer.
        /// </summary>
        /// <param name="channelIds">The list of channel identifiers.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ChannelStreamingStop> ChannelStreamingStop(IList<long> channelIds);

        /// <summary>
        /// Event raised when there is an exception received in response to a ChannelStreamingStop message.
        /// </summary>
        event EventHandler<VoidResponseEventArgs<ChannelStreamingStop>> OnChannelStreamingStopException;

        /// <summary>
        /// Sends a ChannelRangeRequest message to a producer.
        /// </summary>
        /// <param name="channelRangeInfos">The list of <see cref="ChannelRangeInfo"/> objects.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ChannelRangeRequest> ChannelRangeRequest(IList<ChannelRangeInfo> channelRangeInfos);

        /// <summary>
        /// Handles the ChannelMetadata event from a simple streamer producer.
        /// </summary>
        event EventHandler<FireAndForgetEventArgs<ChannelMetadata>> OnSimpleStreamerChannelMetadata;

        /// <summary>
        /// Handles the ChannelMetadata event from a producer.
        /// </summary>
        event EventHandler<ResponseEventArgs<ChannelDescribe, ChannelMetadata>> OnBasicStreamerChannelMetadata;

        /// <summary>
        /// Handles the ChannelData event from a producer when sent as streaming data.
        /// </summary>
        event EventHandler<FireAndForgetEventArgs<ChannelData>> OnStreamingChannelData;

        /// <summary>
        /// Handles the ChannelData event from a producer when sent in response to a ChannelRangeRequest.
        /// </summary>
        event EventHandler<ResponseEventArgs<ChannelRangeRequest, ChannelData>> OnChannelRangeRequestChannelData;

        /// <summary>
        /// Handles the ChannelDataChange event from a producer.
        /// </summary>
        event EventHandler<FireAndForgetEventArgs<ChannelDataChange>> OnChannelDataChange;

        /// <summary>
        /// Handles the ChannelStatusChange event from a producer.
        /// </summary>
        event EventHandler<FireAndForgetEventArgs<ChannelStatusChange>> OnChannelStatusChange;

        /// <summary>
        /// Handles the ChannelRemove event from a producer.
        /// </summary>
        event EventHandler<FireAndForgetEventArgs<ChannelRemove>> OnChannelRemove;
    }
}

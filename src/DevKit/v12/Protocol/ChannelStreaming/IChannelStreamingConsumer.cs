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
using System;

namespace Energistics.Etp.v12.Protocol.ChannelStreaming
{
    /// <summary>
    /// Defines the interface that must be implemented by the consumer role of the ChannelStreaming protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.ChannelStreaming, Roles.Consumer, Roles.Producer)]
    public interface IChannelStreamingConsumer : IProtocolHandler
    {
        /// <summary>
        /// Sends a StartStreaming message to a producer.
        /// </summary>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<StartStreaming> StartStreaming(IMessageHeaderExtension extension = null);

        /// <summary>
        /// Event raised when there is an exception received in response to a StartStreaming message.
        /// </summary>
        event EventHandler<VoidResponseEventArgs<StartStreaming>> OnStartStreamingException;

        /// <summary>
        /// Handles the ChannelMetadata event from a producer.
        /// </summary>
        event EventHandler<FireAndForgetEventArgs<ChannelMetadata>> OnChannelMetadata;

        /// <summary>
        /// Handles the ChannelData event from a producer.
        /// </summary>
        event EventHandler<FireAndForgetEventArgs<ChannelData>> OnChannelData;

        /// <summary>
        /// Handles the TruncateChannels event from a producer.
        /// </summary>
        event EventHandler<FireAndForgetEventArgs<TruncateChannels>> OnTruncateChannels;

        /// <summary>
        /// Sends a StopStreaming message to a producer.
        /// </summary>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<StopStreaming> StopStreaming(IMessageHeaderExtension extension = null);

        /// <summary>
        /// Event raised when there is an exception received in response to a StopStreaming message.
        /// </summary>
        event EventHandler<VoidResponseEventArgs<StopStreaming>> OnStopStreamingException;
    }
}

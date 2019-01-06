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
using System.Linq;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v11.Protocol.ChannelStreaming;

namespace Energistics.Etp.Providers
{
    /// <summary>
    /// Custom implementation of <see cref="IChannelStreamingConsumer"/> for connecting to a Simple Producer
    /// </summary>
    /// <seealso cref="Energistics.Etp.v11.Protocol.ChannelStreaming.ChannelStreamingConsumerHandler" />
    public class MockChannelStreamingConsumer : ChannelStreamingConsumerHandler
    {
        /// <summary>
        /// Handles the ChannelMetadata message from a producer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="channelMetadata">The ChannelMetadata message.</param>
        protected override void HandleChannelMetadata(IMessageHeader header, ChannelMetadata channelMetadata)
        {
            Console.WriteLine(string.Join(Environment.NewLine, channelMetadata.Channels.Select(d => EtpExtensions.Serialize(d))));
        }

        /// <summary>
        /// Handles the ChannelData message from a producer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="channelData">The ChannelData message.</param>
        protected override void HandleChannelData(IMessageHeader header, ChannelData channelData)
        {
            Console.WriteLine(string.Join(Environment.NewLine, channelData.Data.Select(d => EtpExtensions.Serialize(d))));
        }
    }
}

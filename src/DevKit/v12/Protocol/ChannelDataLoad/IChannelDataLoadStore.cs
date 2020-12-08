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

namespace Energistics.Etp.v12.Protocol.ChannelDataLoad
{
    /// <summary>
    /// Describes the interface that must be implemented by the store role of the ChannelDataLoad protocol.
    /// </summary>
    /// <seealso cref="IProtocolHandler" />
    [ProtocolRole((int)Protocols.ChannelDataLoad, "store", "customer")]
    public interface IChannelDataLoadStore : IProtocolHandler
    {
        /// <summary>
        /// Handles the OpenChannels event from a customer.
        /// </summary>
        event ProtocolEventWithErrorsHandler<OpenChannels, OpenChannelInfo, ErrorInfo> OnOpenChannels;

        /// <summary>
        /// Sends a OpenChannelsResponse message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="channels">The channels.</param>
        /// <param name="errors">The errors.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long OpenChannelsResponse(IMessageHeader request, IDictionary<string, OpenChannelInfo> channels, IDictionary<string, ErrorInfo> errors);

        /// <summary>
        /// Handles the CloseChannels event from a customer.
        /// </summary>
        event ProtocolEventHandler<CloseChannels> OnCloseChannels;

        /// <summary>
        /// Handles the ChannelData event from a customer.
        /// </summary>
        event ProtocolEventHandler<ChannelData> OnChannelData;

        /// <summary>
        /// Handles the ReplaceRange event from a customer.
        /// </summary>
        event ProtocolEventHandler<ReplaceRange> OnReplaceRange;

        /// <summary>
        /// Sends a ChannelsClosed message to a customer.
        /// </summary>
        /// <param name="channelIds">The IDs of the closed channels.</param>
        /// <returns></returns>
        long ChannelsClosed(IList<long> channelIds);
    }
}

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

namespace Energistics.Etp.v12.Protocol.ChannelDataFrame
{
    /// <summary>
    /// Read-only interface for ChannelDataFrame store capabilities.
    /// </summary>
    public interface ICapabilitiesStore : IProtocolCapabilities
    {
        /// <summary>
        /// The maximum total count of rows allowed in a complete multipart message response to a single request.
        /// </summary>
        long? MaxFrameResponseRowCount { get; }

        /// <summary>
        /// The maximum time period in seconds for updates to a channel to be visible.
        /// </summary>
        long? FrameChangeDetectionPeriod { get; }
    }
}

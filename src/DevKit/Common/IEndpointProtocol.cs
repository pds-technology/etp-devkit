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

using Energistics.Etp.Common.Datatypes;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Defines common properties for a protocol supported by an endpoint.
    /// </summary>
    public interface IEndpointProtocol
    {
        /// <summary>
        /// The ETP version of the protocol.
        /// </summary>
        EtpVersion EtpVersion { get; }

        /// <summary>
        /// The protocol number.
        /// </summary>
        int Protocol { get; }

        /// <summary>
        /// The endpoint's role in the protocol.
        /// </summary>
        string Role { get; }

        /// <summary>
        /// The counterpart's role in the protocol.
        /// </summary>
        string CounterpartRole { get; }

        /// <summary>
        /// The endpoint's protocol capabilities.
        /// </summary>
        IProtocolCapabilities Capabilities { get; }
    }
}

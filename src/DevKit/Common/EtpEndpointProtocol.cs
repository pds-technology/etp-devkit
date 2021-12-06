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
    public class EtpEndpointProtocol : IEndpointProtocol
    {
        /// <summary>
        /// Initializes a new <see cref="EtpEndpointProtocol"/> instance.
        /// </summary>
        /// <param name="supportedProtocol">The supported protocol to initialize from.</param>
        /// <param name="useRole">If <c>true</c>, use the supported protocol's role.  Otherwise, use the supported protocol's counterpart role.</param>
        public EtpEndpointProtocol(ISupportedProtocol supportedProtocol, bool useRole)
        {
            EtpVersion = supportedProtocol.EtpVersion;
            Protocol = supportedProtocol.Protocol;
            Role = useRole ? supportedProtocol.Role : Roles.GetCounterpartRole(supportedProtocol.Role);
            CounterpartRole = useRole ? Roles.GetCounterpartRole(supportedProtocol.Role) : supportedProtocol.Role;

            Capabilities = new EtpProtocolCapabilities(supportedProtocol.EtpVersion, supportedProtocol.ProtocolCapabilities);
        }

        /// <summary>
        /// The ETP version of the protocol.
        /// </summary>
        public EtpVersion EtpVersion { get; }

        /// <summary>
        /// The protocol number.
        /// </summary>
        public int Protocol { get; }

        /// <summary>
        /// The endpoint's role in the protocol.
        /// </summary>
        public string Role { get; }

        /// <summary>
        /// The counterpart's role in the protocol.
        /// </summary>
        public string CounterpartRole { get; }

        /// <summary>
        /// The endpoint's protocol capabilities.
        /// </summary>
        public IProtocolCapabilities Capabilities { get; }
    }
}

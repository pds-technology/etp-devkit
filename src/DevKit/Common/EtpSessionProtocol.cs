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
using System.Collections;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Encapsulates information about protocols in use during a session.
    /// </summary>
    public class EtpSessionProtocol : ISupportedProtocol
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EtpSessionProtocol"/> class.
        /// </summary>
        /// <param name="protocol">The protocol number.</param>
        /// <param name="role">The role in the protocol.</param>
        /// <param name="protocolVersion">The protocol version.</param>
        /// <param name="counterpartRole">The counterpart's role in the protocol.</param>
        /// <param name="capabilities">The capabilities for the protocol.</param>
        /// <param name="counterpartCapabilities">The counterpart's capabilities in the protocol.</param>
        public EtpSessionProtocol(int protocol, IVersion protocolVersion, string role, string counterpartRole, EtpProtocolCapabilities capabilities, EtpProtocolCapabilities counterpartCapabilities)
        {
            Protocol = protocol;
            ProtocolVersion = protocolVersion;
            Role = role;
            CounterpartRole = counterpartRole;
            Capabilities = capabilities;
            CounterpartCapabilities = counterpartCapabilities;
        }

        /// <summary>
        /// Creates a new <see cref="ISupportedProtocol"/> instance from this instance.
        /// </summary>
        /// <typeparam name="TSupportedProtocol">The concrete type of the <see cref="ISupportedProtocol"/> instance to create.</typeparam>
        /// <typeparam name="TDataValue">The concrete <see cref="IDataValue"/> instance to create.</typeparam>
        /// <returns></returns>
        public TSupportedProtocol AsSupportedProtocol<TSupportedProtocol, TVersion, TDataValue>()
            where TSupportedProtocol : ISupportedProtocol, new()
            where TVersion : IVersion, new()
            where TDataValue : IDataValue, new()
        {
            return new TSupportedProtocol
            {
                Protocol = Protocol,
                ProtocolVersion = new TVersion
                {
                    Major = ProtocolVersion.Major,
                    Minor = ProtocolVersion.Minor,
                    Patch = ProtocolVersion.Patch,
                    Revision = ProtocolVersion.Revision,
                },
                ProtocolCapabilities = Capabilities.AsDataValueDictionary<TDataValue>(),
                Role = Role,
            };
        }

        /// <summary>
        /// The protocol number.
        /// </summary>
        public int Protocol { get; set; }

        /// <summary>
        /// The protocol version.
        /// </summary>
        public IVersion ProtocolVersion { get; set; }

        /// <summary>
        /// <see cref="ISupportedProtocol"/> support.
        /// </summary>
        public string VersionString => $"{ProtocolVersion.Major}.{ProtocolVersion.Minor}.{ProtocolVersion.Revision}.{ProtocolVersion.Patch}";

        /// <summary>
        /// The role in the protocol.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// The counterpart's role in the protocol.
        /// </summary>
        public string CounterpartRole { get; set; }

        /// <summary>
        /// <see cref="ISupportedProtocol"/> support.
        /// </summary>
        IDictionary ISupportedProtocol.ProtocolCapabilities
        {
            get { return Capabilities.Capabilities; }
            set { Capabilities.Capabilities = value; }
        }

        /// <summary>
        /// The capabilities for the protocol.
        /// </summary>
        public EtpProtocolCapabilities Capabilities { get; set; }

        /// <summary>
        /// The counterpart's capabilities in the protocol.
        /// </summary>
        public EtpProtocolCapabilities CounterpartCapabilities { get; set; }
    }
}

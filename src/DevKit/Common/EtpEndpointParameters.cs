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
using System.Linq;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Provides common input parameters for an ETP Endpoint.
    /// </summary>
    public class EtpEndpointParameters : IEndpointDetails
    {
        /// <summary>
        /// Initializes a new <see cref="EtpEndpointParameters"/> instance.
        /// </summary>
        public EtpEndpointParameters()
            : this(EtpVersion.Unknown)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="EtpEndpointParameters"/> instance.
        /// </summary>
        /// <param name="version">The supported ETP version.</param>
        public EtpEndpointParameters(EtpVersion version)
        {
            Capabilities = new EtpEndpointCapabilities(version);
        }

        /// <summary>
        /// Initializes a new <see cref="EtpEndpointParameters"/> instance.
        /// </summary>
        /// <param name="capabilities">The endpoint's capabilities</param>
        /// <param name="supportedProtocols">The protocols supported by this endpoint</param>
        /// <param name="supportedDataObjects">The types of data objects supported by this endpoint</param>
        /// <param name="supportedCompression">The types of compression supported by this endpoint</param>
        /// <param name="supportedFormats">The formats supported by this endpoint</param>
        public EtpEndpointParameters(EtpEndpointCapabilities capabilities, List<IProtocolHandler> supportedProtocols, List<EtpSupportedDataObject> supportedDataObjects, List<string> supportedCompression, List<string> supportedFormats)
        {
            Capabilities = capabilities;
            SupportedProtocols = supportedProtocols;
            SupportedDataObjects = supportedDataObjects;
            SupportedCompression = supportedCompression;
            SupportedFormats = supportedFormats;
        }

        /// <summary>
        /// Gets the endpoint's capabilities.
        /// </summary>
        public EtpEndpointCapabilities Capabilities { get; }

        /// <summary>
        /// Gets the protocols supported by this endpoint.
        /// </summary>
        public List<IProtocolHandler> SupportedProtocols { get; } = new List<IProtocolHandler>();

        /// <summary>
        /// Gets the types of data objects supported by this endpoint.
        /// </summary>
        public List<EtpSupportedDataObject> SupportedDataObjects { get; } = new List<EtpSupportedDataObject>();

        /// <summary>
        /// Gets the types of compression supported by this endpoint.
        /// </summary>
        public List<string> SupportedCompression { get; } = new List<string> { EtpCompression.Gzip };

        /// <summary>
        /// Gets the formats supported by this endpoint.
        /// </summary>
        /// <returns>A list of formats supported by this endpoint.</returns>
        public List<string> SupportedFormats { get; } = new List<string> { Formats.Xml };

        /// <summary>
        /// Gets the endpoint's capabilities.
        /// </summary>
        IEndpointCapabilities IEndpointDetails.Capabilities => Capabilities;

        /// <summary>
        /// Gets the protocols supported by this endpoint.
        /// </summary>
        /// <returns>A list of protocols supported by this endpoint.</returns>
        IReadOnlyList<IEndpointProtocol> IEndpointDetails.SupportedProtocols => SupportedProtocols;

        /// <summary>
        /// Gets the types of data objects supported by this endpoint and their capabilities.
        /// </summary>
        IReadOnlyList<IEndpointSupportedDataObject> IEndpointDetails.SupportedDataObjects => SupportedDataObjects;

        /// <summary>
        /// Gets the types of compression supported by this endpoint.
        /// </summary>
        IReadOnlyList<string> IEndpointDetails.SupportedCompression => SupportedCompression;

        /// <summary>
        /// Gets the formats supported by this endpoint.
        /// </summary>
        IReadOnlyList<string> IEndpointDetails.SupportedFormats => SupportedFormats;

        /// <summary>
        /// Creates a deep copy of this instance for the specified ETP version.
        /// Any protocol handlers not supported by the specified version will be removed from the clone.
        /// </summary>
        /// <param name="version">The ETP version for the clone.</param>
        /// <returns>A deep copy of this instance.</returns>
        public EtpEndpointParameters CloneForVersion(EtpVersion version)
        {
            return new EtpEndpointParameters(
                new EtpEndpointCapabilities(Capabilities),
                SupportedProtocols.Where(p => version == EtpVersion.Unknown || p.EtpVersion == EtpVersion.Unknown || p.EtpVersion == version).Select(p => p.Clone()).ToList(),
                new List<EtpSupportedDataObject>(SupportedDataObjects),
                new List<string>(SupportedCompression),
                new List<string>(SupportedFormats)
            );
        }
    }
}

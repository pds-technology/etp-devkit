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

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Provides common information about an ETP Endpoint.
    /// </summary>
    public interface IEndpointDetails
    {
        /// <summary>
        /// Gets the endpoint's capabilities.
        /// </summary>
        IEndpointCapabilities Capabilities { get; }

        /// <summary>
        /// Gets the protocols supported by this endpoint.
        /// </summary>
        /// <returns>A list of protocols supported by this endpoint.</returns>
        IReadOnlyList<IEndpointProtocol> SupportedProtocols { get; }

        /// <summary>
        /// Gets the types of data objects supported by this endpoint and their capabilities.
        /// </summary>
        IReadOnlyList<IEndpointSupportedDataObject> SupportedDataObjects { get; }

        /// <summary>
        /// Gets the types of compression supported by this endpoint.
        /// </summary>
        IReadOnlyList<string> SupportedCompression { get; }

        /// <summary>
        /// Gets the formats supported by this endpoint.
        /// </summary>
        IReadOnlyList<string> SupportedFormats { get; }
    }
}
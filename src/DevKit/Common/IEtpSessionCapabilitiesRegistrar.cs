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
    /// Common interface for entities that can register capabilities supported in an ETP session.
    /// </summary>
    public interface IEtpSessionCapabilitiesRegistrar
    {
        /// <summary>
        /// Returns whether the specified ETP version is supported.
        /// </summary>
        /// <param name="version">The specified ETP version.</param>
        /// <returns><c>true</c> if the version is supported; <c>false</c> otherwise.</returns>
        bool IsEtpVersionSupported(EtpVersion version);

        /// <summary>
        /// Registers a protocol handler.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        void Register(IProtocolHandler handler);

        /// <summary>
        /// Registers protocol handlers.
        /// </summary>
        /// <param name="handlers">The protocol handlers.</param>
        void Register(IEnumerable<IProtocolHandler> handlers);

        /// <summary>
        /// Registers a supported data object.
        /// </summary>
        /// <param name="supportedDataObject">The supported data object.</param>
        void Register(IEndpointSupportedDataObject supportedDataObject);

        /// <summary>
        /// Registers supported data objects.
        /// </summary>
        /// <param name="supportedDataObjects">The supported data objects.</param>
        void Register(IEnumerable<IEndpointSupportedDataObject> supportedDataObjects);
    }
}

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
    /// Provides common functionality for ETP protocol capabilities.
    /// </summary>
    public class EtpProtocolCapabilities : EtpCapabilities, IProtocolCapabilities
    {
        /// <summary>
        /// Initializes a new <see cref="EtpProtocolCapabilities"/> instance.
        /// </summary>
        public EtpProtocolCapabilities()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="EtpProtocolCapabilities"/> instance.
        /// </summary>
        /// <param name="version">The ETP version the capabilities are for.</param>
        public EtpProtocolCapabilities(EtpVersion version)
            : base(version)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="EtpProtocolCapabilities"/> instance.
        /// </summary>
        /// <param name="capabilities">The capabilities to initialize this from.</param>
        public EtpProtocolCapabilities(IReadOnlyCapabilities capabilities)
            : base(capabilities)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="EtpProtocolCapabilities"/> instance.
        /// </summary>
        /// <param name="version">The ETP version the capabilities are for.</param>
        /// <param name="capabilities">The capabilities to initialize this from.</param>
        public EtpProtocolCapabilities(EtpVersion version, IReadOnlyDataValueDictionary capabilities)
            : base(version, capabilities)
        {
        }
    }
}

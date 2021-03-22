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
    /// Provides common input parameters for an ETP web server or server manager.
    /// </summary>
    public class EtpWebServerDetails
    {
        /// <summary>
        /// Initializes a new <see cref="EtpWebServerDetails"/> instance.
        /// </summary>
        public EtpWebServerDetails()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="EtpWebServerDetails"/> instance.
        /// </summary>
        /// <param name="supportedEtpVersions">The ETP versions supported.</param>
        /// <param name="supportedEncodings">The ETP encodings supported.</param>
        public EtpWebServerDetails(IEnumerable<EtpVersion> supportedEtpVersions, IEnumerable<EtpEncoding> supportedEncodings)
        {
            SupportedVersions = new List<EtpVersion>(supportedEtpVersions);
            SupportedEncodings = new List<EtpEncoding>(supportedEncodings);
        }

        /// <summary>
        /// The organization hosting the web server.
        /// </summary>
        public string OrganizationName { get; set; } = "Energistics";

        /// <summary>
        /// The contact person or group in the organization hosting the web server.
        /// </summary>
        public string ContactName { get; set; } = "Energistics";

        /// <summary>
        /// The phone number for the contact person or group in the organization hosting the web server.
        /// </summary>
        public string ContactPhone { get; set; }

        /// <summary>
        /// The e-mail address for the contact person or group in the organization hosting the web server.
        /// </summary>
        public string ContactEmail { get; set; } = "info@energistics.org";

        /// <summary>
        /// Gets or sets the list of supported ETP versions.
        /// </summary>
        /// <value>The ETP versions supported by this server manager.</value>
        public IReadOnlyList<EtpVersion> SupportedVersions { get; } = new List<EtpVersion> { EtpVersion.v11, EtpVersion.v12 };

        /// <summary>
        /// Gets or sets the list of supported Avro message encodings.
        /// </summary>
        /// <value>The avro encodings supported by this server manager.</value>
        public IReadOnlyList<EtpEncoding> SupportedEncodings { get; } = new List<EtpEncoding> { EtpEncoding.Binary, EtpEncoding.Json };

        /// <summary>
        /// Checks if the specified encoding is supported.
        /// </summary>
        /// <param name="encoding">The encoding to check.</param>
        /// <returns><c>true</c> if the encoding is supported; <c>false</c> otherwise.</returns>
        public bool IsEncodingSupported(EtpEncoding encoding)
        {
            return SupportedEncodings.Any(e => e == encoding);
        }

        /// <summary>
        /// Checks if the specified ETP version is supported.
        /// </summary>
        /// <param name="version">The ETP version to check.</param>
        /// <returns><c>true</c> if the ETP version is supported; <c>false</c> otherwise.</returns>
        public bool IsVersionSupported(EtpVersion version)
        {
            return SupportedVersions.Any(v => v == version);
        }
    }
}

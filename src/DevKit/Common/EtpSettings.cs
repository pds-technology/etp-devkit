//----------------------------------------------------------------------- 
// ETP DevKit, 1.2
//
// Copyright 2018 Energistics
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
using Energistics.Etp.Properties;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Defines static fields for the ETP settings.
    /// </summary>
    public static class EtpSettings
    {
        /// <summary>
        /// The legacy ETP sub protocol name
        /// </summary>
        public const string LegacySubProtocol = "energistics-tp";

        /// <summary>
        /// The ETP v1.1 sub protocol name
        /// </summary>
        public const string Etp11SubProtocol = "etp11.energistics.org";

        /// <summary>
        /// The ETP v1.2 sub protocol name
        /// </summary>
        public const string Etp12SubProtocol = "etp12.energistics.org";

        /// <summary>
        /// A list of supported ETP sub protocols
        /// </summary>
        public static readonly List<string> EtpSubProtocols = new List<string>
        {
            Etp12SubProtocol, Etp11SubProtocol, LegacySubProtocol
        };

        /// <summary>
        /// The default ETP sub protocol name
        /// </summary>
        public static string EtpSubProtocolName = Settings.Default.EtpSubProtocolName;

        /// <summary>
        /// The default ETP encoding header
        /// </summary>
        public static string EtpEncodingHeader = Settings.Default.EtpEncodingHeader;

        /// <summary>
        /// The default ETP version header
        /// </summary>
        public static string EtpVersionHeader = Settings.Default.EtpVersionHeader;

        /// <summary>
        /// The binary ETP encoding.
        /// </summary>
        public static string EtpEncodingBinary = Settings.Default.EtpEncodingBinary;

        /// <summary>
        /// The JSON ETP encoding.
        /// </summary>
        public static string EtpEncodingJson = Settings.Default.EtpEncodingJson;
    }
}

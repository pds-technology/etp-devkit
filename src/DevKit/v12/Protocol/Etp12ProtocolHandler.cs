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
using Energistics.Etp.Common.Datatypes;

namespace Energistics.Etp.v12.Protocol
{
    /// <summary>
    /// Provides common functionality for ETP 1.2 protocol handlers.
    /// </summary>
    /// <seealso cref="EtpProtocolHandler" />
    public abstract class Etp12ProtocolHandler : EtpProtocolHandler, IEtp12ProtocolHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Etp12ProtocolHandler"/> class.
        /// </summary>
        /// <param name="protocol">The protocol.</param>
        /// <param name="role">The role.</param>
        /// <param name="requestedRole">The requested role.</param>
        protected Etp12ProtocolHandler(int protocol, string role, string requestedRole)
            : base(EtpVersion.v12, protocol, role, requestedRole)
        {
        }
    }
}

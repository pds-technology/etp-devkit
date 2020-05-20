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

using System;

namespace Energistics.Etp.Common.Datatypes
{
    /// <summary>
    /// Specifies protocol and role requirements for a protocol handler.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true, AllowMultiple = false)]
    public class ProtocolRoleAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolRoleAttribute"/> class.
        /// </summary>
        /// <param name="protocol">The protocol.</param>
        /// <param name="role">The role in the protocol.</param>
        /// <param name="counterpartRole">The counterpart's role in the protocol.</param>
        public ProtocolRoleAttribute(int protocol, string role, string counterpartRole)
        {
            Protocol = protocol;
            Role = role;
            CounterpartRole = counterpartRole;
        }

        /// <summary>
        /// Gets the protocol.
        /// </summary>
        /// <value>The protocol.</value>
        public int Protocol { get; }

        /// <summary>
        /// Gets the role in the protocol.
        /// </summary>
        /// <value>The role in the protocol.</value>
        public string Role { get; }

        /// <summary>
        /// Gets the counterpart's role in the protocol.
        /// </summary>
        /// <value>The counterpart's role in the protocol.</value>
        public string CounterpartRole { get; }
    }
}

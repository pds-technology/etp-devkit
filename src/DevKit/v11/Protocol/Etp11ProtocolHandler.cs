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

namespace Energistics.Etp.v11.Protocol
{
    /// <summary>
    /// Provides common functionality for ETP 1.1 protocol handlers.
    /// </summary>
    /// <seealso cref="EtpProtocolHandler" />
    public abstract class Etp11ProtocolHandler<TCapabilities, TCapabilitiesInterface, TCounterpartCapabilities, TCounterpartCapabilitiesInterface> : EtpProtocolHandler<TCapabilities, TCapabilitiesInterface, TCounterpartCapabilities, TCounterpartCapabilitiesInterface>, IEtp11ProtocolHandler
        where TCapabilities : Etp11ProtocolCapabilities, TCapabilitiesInterface, new()
        where TCapabilitiesInterface : class, IProtocolCapabilities
        where TCounterpartCapabilities : Etp11ProtocolCapabilities, TCounterpartCapabilitiesInterface, new()
        where TCounterpartCapabilitiesInterface : class, IProtocolCapabilities
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Etp11ProtocolHandler{TCapabilities, TCapabilitiesInterface, TCounterpartCapabilities, TCounterpartCapabilitiesInterface}"/> class.
        /// </summary>
        /// <param name="protocol">The protocol.</param>
        /// <param name="role">This handler's role in the protocol.</param>
        /// <param name="counterpartRole">The role for this handler's counterpart in the protocol.</param>
        protected Etp11ProtocolHandler(int protocol, string role, string counterpartRole)
            : base(EtpVersion.v11, protocol, role, counterpartRole)
        {
        }
    }

    /// <summary>
    /// Provides default common functionality for ETP 1.1 protocol handlers.
    /// </summary>
    /// <seealso cref="EtpProtocolHandler" />
    public abstract class Etp11ProtocolHandler : Etp11ProtocolHandler<Etp11ProtocolCapabilities, IProtocolCapabilities, Etp11ProtocolCapabilities, IProtocolCapabilities>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Etp11ProtocolHandler{TCapabilities, TCapabilitiesInterface, TCounterpartCapabilities, TCounterpartCapabilitiesInterface}"/> class.
        /// </summary>
        /// <param name="protocol">The protocol.</param>
        /// <param name="role">This handler's role in the protocol.</param>
        /// <param name="counterpartRole">The role for this handler's counterpart in the protocol.</param>
        protected Etp11ProtocolHandler(int protocol, string role, string counterpartRole)
            : base(protocol, role, counterpartRole)
        {
        }
    }

    /// <summary>
    /// Provides default common functionality for ETP 1.1 protocol handlers.
    /// </summary>
    /// <seealso cref="EtpProtocolHandler" />
    public abstract class Etp11ProtocolHandlerWithCounterpartCapabilities<TCounterpartCapabilities, TCounterpartCapabilitiesInterface> : Etp11ProtocolHandler<Etp11ProtocolCapabilities, IProtocolCapabilities, TCounterpartCapabilities, TCounterpartCapabilitiesInterface>
        where TCounterpartCapabilities : Etp11ProtocolCapabilities, TCounterpartCapabilitiesInterface, new()
        where TCounterpartCapabilitiesInterface : class, IProtocolCapabilities
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Etp11ProtocolHandlerWithCounterpartCapabilities{TCounterpartCapabilities, TCounterpartCapabilitiesInterface}"/> class.
        /// </summary>
        /// <param name="protocol">The protocol.</param>
        /// <param name="role">This handler's role in the protocol.</param>
        /// <param name="counterpartRole">The role for this handler's counterpart in the protocol.</param>
        protected Etp11ProtocolHandlerWithCounterpartCapabilities(int protocol, string role, string counterpartRole)
            : base(protocol, role, counterpartRole)
        {
        }
    }

    /// <summary>
    /// Provides default common functionality for ETP 1.1 protocol handlers.
    /// </summary>
    /// <seealso cref="EtpProtocolHandler" />
    public abstract class Etp11ProtocolHandlerWithCapabilities<TCapabilities, TCapabilitiesInterface> : Etp11ProtocolHandler<TCapabilities, TCapabilitiesInterface, Etp11ProtocolCapabilities, IProtocolCapabilities>
        where TCapabilities : Etp11ProtocolCapabilities, TCapabilitiesInterface, new()
        where TCapabilitiesInterface : class, IProtocolCapabilities
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Etp11ProtocolHandlerWithCapabilities{TCapabilities, TCapabilitiesInterface}"/> class.
        /// </summary>
        /// <param name="protocol">The protocol.</param>
        /// <param name="role">This handler's role in the protocol.</param>
        /// <param name="Role">The role for this handler's  in the protocol.</param>
        protected Etp11ProtocolHandlerWithCapabilities(int protocol, string role, string Role)
            : base(protocol, role, Role)
        {
        }
    }
}

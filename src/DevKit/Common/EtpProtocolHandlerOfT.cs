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

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Provides common functionality for ETP protocol handlers with specific capability types.
    /// </summary>
    /// <seealso cref="EtpProtocolHandler" />
    public abstract class EtpProtocolHandler<TCapabilities, TCapabilitiesInterface, TCounterpartCapabilities, TCounterpartCapabilitiesInterface> : EtpProtocolHandler, IProtocolHandler<TCapabilitiesInterface, TCounterpartCapabilitiesInterface>
        where TCapabilities : EtpProtocolCapabilities, TCapabilitiesInterface, new()
        where TCapabilitiesInterface : class, IProtocolCapabilities
        where TCounterpartCapabilities : EtpProtocolCapabilities, TCounterpartCapabilitiesInterface, new()
        where TCounterpartCapabilitiesInterface : class, IProtocolCapabilities
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EtpProtocolHandler{TCapabilities, TCapabilitiesInterface, TCounterpartCapabilities, TCounterpartCapabilitiesInterface}"/> class.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <param name="protocol">The protocol.</param>
        /// <param name="role">This handler's role in the protocol.</param>
        /// <param name="counterpartRole">The role for this handler's counterpart in the protocol.</param>
        protected EtpProtocolHandler(EtpVersion version, int protocol, string role, string counterpartRole)
            : base(version, protocol, role, counterpartRole, capabilities: new TCapabilities())
        {
        }

        /// <summary>
        /// Creates the actual counterpart capabilities to use.
        /// </summary>
        /// <param name="counterpartCapabilities">The capabilities received from the counterpart.</param>
        /// <returns>The coutnerpart capabilities to use.</returns>
        protected override IProtocolCapabilities CreateCounterpartCapabilities(IProtocolCapabilities counterpartCapabilities)
        {
            var capabilities = new TCounterpartCapabilities();
            capabilities.LoadFrom(counterpartCapabilities);

            return capabilities;
        }

        /// <summary>
        /// The endpoint's protocol capabilities.
        /// </summary>
        public new TCapabilities Capabilities => base.Capabilities as TCapabilities;

        /// <summary>
        /// The endpoint's protocol capabilities.
        /// </summary>
        TCapabilitiesInterface IProtocolHandler<TCapabilitiesInterface, TCounterpartCapabilitiesInterface>.Capabilities => base.Capabilities as TCapabilitiesInterface;

        /// <summary>
        /// The endpoint counterpart's protocol capabilities.
        /// </summary>
        public new TCounterpartCapabilitiesInterface CounterpartCapabilities => base.CounterpartCapabilities as TCounterpartCapabilitiesInterface;
    }
}

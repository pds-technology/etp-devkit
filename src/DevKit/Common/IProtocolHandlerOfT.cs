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
    /// Defines properties and methods that can be used to handle ETP messages.
    /// </summary>
    public interface IProtocolHandler<TCapabilitiesInterface, TCounterpartCapabilitiesInterface> : IProtocolHandler
        where TCapabilitiesInterface : IProtocolCapabilities
        where TCounterpartCapabilitiesInterface : IProtocolCapabilities
    {
        /// <summary>
        /// The endpoint's protocol capabilities.
        /// </summary>
        new TCapabilitiesInterface Capabilities { get; }

        /// <summary>
        /// The endpoint counterpart's protocol capabilities.
        /// </summary>
        new TCounterpartCapabilitiesInterface CounterpartCapabilities { get; }
    }

    /// <summary>
    /// Defines properties and methods that can be used to handle ETP messages.
    /// </summary>
    public interface IProtocolHandlerWithCapabilities<TCapabilitiesInterface> : IProtocolHandler<TCapabilitiesInterface, IProtocolCapabilities>
        where TCapabilitiesInterface : IProtocolCapabilities
    {
    }

    /// <summary>
    /// Defines properties and methods that can be used to handle ETP messages.
    /// </summary>
    public interface IProtocolHandlerWithCounterpartCapabilities<TCounterpartCapabilitiesInterface> : IProtocolHandler<IProtocolCapabilities, TCounterpartCapabilitiesInterface>
        where TCounterpartCapabilitiesInterface : IProtocolCapabilities
    {
    }
}

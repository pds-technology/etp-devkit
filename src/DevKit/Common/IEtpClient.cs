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

using System.Threading.Tasks;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Defines the properties and methods needed to manage an ETP client.
    /// </summary>
    /// <seealso cref="System.IEtpSession" />
    public interface IEtpClient : IEtpSession
    {
        /// <summary>
        /// Registers a protocol handler for the specified contract type.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <typeparam name="THandler">The type of the handler.</typeparam>
        void Register<TContract, THandler>() where TContract : IProtocolHandler where THandler : TContract;

        /// <summary>
        /// Gets a value indicating whether the connection is open.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the connection is open; otherwise, <c>false</c>.
        /// </value>
        bool IsOpen { get; }

        /// <summary>
        /// Opens the WebSocket connection.
        /// </summary>
        void Open();

        /// <summary>
        /// Asynchronously pens the WebSocket connection.
        /// </summary>
        Task<bool> OpenAsync();
    }
}

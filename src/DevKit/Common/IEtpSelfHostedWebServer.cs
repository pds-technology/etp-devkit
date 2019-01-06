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
using System.Threading.Tasks;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Defines the properties and methods needed to manage a self-hosted ETP web server.
    /// </summary>
    /// <seealso cref="IEtpWebServer" />
    public interface IEtpSelfHostedWebServer : IEtpWebServer
    {
        /// <summary>
        /// The root URI for the server.
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// Starts the web server.
        /// </summary>
        void Start();

        /// <summary>
        /// Asynchronously starts the web server.
        /// </summary>
        Task StartAsync();

        /// <summary>
        /// Stops the web server.
        /// </summary>
        void Stop();

        /// <summary>
        /// Asynchronously stops the web server.
        /// </summary>
        Task StopAsync();

        /// <summary>
        /// Gets a value indicating whether the web server is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if the web server is running; otherwise, <c>false</c>.
        /// </value>
        bool IsRunning { get; }
    }
}

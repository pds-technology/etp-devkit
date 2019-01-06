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
using log4net;
using System;
using System.Collections.Generic;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Defines the properties and methods needed to manage an ETP web server.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IEtpWebServer : IDisposable
    {
        /// <summary>
        /// Gets the logger for the web server.
        /// </summary>
        ILog Logger { get; }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        /// <value>The name of the application.</value>
        string ApplicationName { get; }

        /// <summary>
        /// Gets the application version.
        /// </summary>
        /// <value>The application version.</value>
        string ApplicationVersion { get; }

        /// <summary>
        /// Gets or sets the supported compression type.
        /// </summary>
        string SupportedCompression { get; set; }

        /// <summary>
        /// Gets or sets the list of supported objects.
        /// </summary>
        /// <value>The supported objects.</value>
        IList<string> SupportedObjects { get; set; }

        /// <summary>
        /// Gets or sets the list of supported encodings.
        /// </summary>
        /// <value>The supported encodings.</value>
        IList<string> SupportedEncodings { get; set; }

        /// <summary>
        /// Gets or sets a delegate to process logging messages.
        /// </summary>
        /// <value>The output delegate.</value>
        Action<string> Output { get; set; }

        /// <summary>
        /// Logs the specified message using the Output delegate, if available.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The message.</returns>
        string Log(string message);

        /// <summary>
        /// Logs the specified message using the Output delegate, if available.
        /// </summary>
        /// <param name="message">The message format string.</param>
        /// <param name="args">The format parameter values.</param>
        /// <returns>The formatted message.</returns>
        string Log(string message, params object[] args);

        /// <summary>
        /// Occurs when an ETP session is connected.
        /// </summary>
        event EventHandler<IEtpSession> SessionConnected;

        /// <summary>
        /// Occurs when an ETP session is closed.
        /// </summary>
        event EventHandler<IEtpSession> SessionClosed;

        /// <summary>
        /// Resolves the ETP adapter for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <returns>The adapter for the version.</returns>
        IEtpAdapter ResolveEtpAdapter(EtpVersion version);

        /// <summary>
        /// Registers the ETP version-specific protocol handler for the specified contract type.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <typeparam name="THandler">The type of the handler.</typeparam>
        void Register<TContract, THandler>() where TContract : IProtocolHandler where THandler : TContract;

        /// <summary>
        /// Registers an ETP version-specific protocol handler factory for the specified contract type.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="factory">The factory.</param>
        void Register<TContract>(Func<TContract> factory) where TContract : IProtocolHandler;

        /// <summary>
        /// Gets the supported protocols for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <returns>A list of supported protocols.</returns>
        IList<ISupportedProtocol> GetSupportedProtocols(EtpVersion version);
    }
}

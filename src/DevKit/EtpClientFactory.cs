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


using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Properties;
using System;
using System.Collections.Generic;

namespace Energistics.Etp
{
    /// <summary>
    /// Provides a factory to create ETP Clients.
    /// </summary>
    public static class EtpClientFactory
    {
        private static readonly IDictionary<string, string> EmptyHeaders = new Dictionary<string, string>();

        /// <summary>
        /// Creates an <see cref="IEtpClient"/> using the default WebSocket type.
        /// </summary>
        /// <param name="uri">The ETP server URI.</param>
        /// <param name="application">The client application name.</param>
        /// <param name="version">The client application version.</param>
        /// <param name="etpSubProtocol">The ETP sub protocol.</param>
        /// <returns>The <see cref="IEtpClient"/></returns>
        public static IEtpClient CreateClient(string uri, string application, string version, string etpSubProtocol)
        {
            return CreateClient(Settings.Default.DefaultWebSocketType, uri, application, version, etpSubProtocol, EmptyHeaders);
        }

        /// <summary>
        /// Creates an <see cref="IEtpClient"/> using the specified WebSocket type.
        /// </summary>
        /// <param name="uri">The ETP server URI.</param>
        /// <param name="application">The client application name.</param>
        /// <param name="version">The client application version.</param>
        /// <param name="etpSubProtocol">The ETP sub protocol.</param>
        /// <returns>The <see cref="IEtpClient"/></returns>
        public static IEtpClient CreateClient(WebSocketType webSocketType, string uri, string application, string version, string etpSubProtocol)
        {
            return CreateClient(webSocketType, uri, application, version, etpSubProtocol, EmptyHeaders);
        }

        /// <summary>
        /// Creates an <see cref="IEtpClient"/> using the default WebSocket type.
        /// </summary>
        /// <param name="uri">The ETP server URI.</param>
        /// <param name="application">The client application name.</param>
        /// <param name="version">The client application version.</param>
        /// <param name="etpSubProtocol">The ETP sub protocol.</param>
        /// <param name="headers">The WebSocket headers.</param>
        /// <returns>The <see cref="IEtpClient"/></returns>
        public static IEtpClient CreateClient(string uri, string application, string version, string etpSubProtocol, IDictionary<string, string> headers)
        {
            return CreateClient(Settings.Default.DefaultWebSocketType, uri, application, version, etpSubProtocol, headers);
        }

        /// <summary>
        /// Creates an <see cref="IEtpClient"/> using the specified WebSocket type.
        /// </summary>
        /// <param name="uri">The ETP server URI.</param>
        /// <param name="application">The client application name.</param>
        /// <param name="version">The client application version.</param>
        /// <param name="etpSubProtocol">The ETP sub protocol.</param>
        /// <param name="headers">The WebSocket headers.</param>
        /// <returns>The <see cref="IEtpClient"/></returns>
        public static IEtpClient CreateClient(WebSocketType webSocketType, string uri, string application, string version, string etpSubProtocol, IDictionary<string, string> headers)
        {
            switch (webSocketType)
            {
                case WebSocketType.Native:
                    return new Native.EtpClient(uri, application, version, etpSubProtocol, headers);
                case WebSocketType.WebSocket4Net:
                    return new WebSocket4Net.EtpClient(uri, application, version, etpSubProtocol, headers);                    

                default:
                    throw new ArgumentException($"Unrecognized WebSocket type: {webSocketType}", "webSocketType");
            }
        }
    }
}

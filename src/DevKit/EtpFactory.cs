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
using Energistics.Etp.Properties;
using System;
using System.Collections.Generic;

namespace Energistics.Etp
{
    /// <summary>
    /// Provides a factory to create ETP Clients and WebServers.
    /// </summary>
    public static class EtpFactory
    {
        private static log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(EtpFactory));

        private static bool? _isNativeSupported;

        /// <summary>
        /// Checks if Native .NET WebSockets are supported on this platform.
        /// </summary>
        public static bool IsNativeSupported
        {
            get
            {
                if (_isNativeSupported.HasValue) return _isNativeSupported.Value;

                try
                {
                    var webSocket = new System.Net.WebSockets.ClientWebSocket();
                    webSocket.Dispose();
                    _isNativeSupported = true;
                }
                catch (PlatformNotSupportedException)
                {
                    Logger.Debug($"Native .NET WebSockets not supported on this platform.  Falling back to {FallbackWebSocketType} WebSockets.");
                    _isNativeSupported = false;
                }

                return _isNativeSupported.Value;
            }
        }

        /// <summary>
        /// The fallback WebSocketType if Native WebSockets are not supported.
        /// </summary>
        public static WebSocketType FallbackWebSocketType {  get { return WebSocketType.WebSocket4Net; } }

        #region IEtpClient
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
        /// <param name="webSocketType">The specified WebSocket type.</param>
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
        /// <param name="webSocketType">The specified WebSocket type.</param>
        /// <param name="uri">The ETP server URI.</param>
        /// <param name="application">The client application name.</param>
        /// <param name="version">The client application version.</param>
        /// <param name="etpSubProtocol">The ETP sub protocol.</param>
        /// <param name="headers">The WebSocket headers.</param>
        /// <returns>The <see cref="IEtpClient"/></returns>
        public static IEtpClient CreateClient(WebSocketType webSocketType, string uri, string application, string version, string etpSubProtocol, IDictionary<string, string> headers)
        {
            if (webSocketType == WebSocketType.Native && !IsNativeSupported)
                webSocketType = FallbackWebSocketType;

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
        #endregion

        #region IEtpSelfHostedWebServer
        /// <summary>
        /// Creates an <see cref="IEtpSelfHostedWebServer"/> using the default WebSocket type.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="application">The server application name.</param>
        /// <param name="version">The server application version.</param>
        /// <returns>The <see cref="IEtpSelfHostedWebServer"/></returns>
        public static IEtpSelfHostedWebServer CreateSelfHostedWebServer(int port, string application, string version)
        {
            return CreateSelfHostedWebServer(Settings.Default.DefaultWebSocketType, port, application, version);
        }

        /// <summary>
        /// Creates an <see cref="IEtpSelfHostedWebServer"/> using the specified WebSocket type.
        /// </summary>
        /// <param name="webSocketType">The specified WebSocket type.</param>
        /// <param name="port">The port.</param>
        /// <param name="application">The server application name.</param>
        /// <param name="version">The server application version.</param>
        /// <returns>The <see cref="IEtpSelfHostedWebServer"/></returns>
        public static IEtpSelfHostedWebServer CreateSelfHostedWebServer(WebSocketType webSocketType, int port, string application, string version)
        {
            if (webSocketType == WebSocketType.Native && !IsNativeSupported)
                webSocketType = FallbackWebSocketType;

            switch (webSocketType)
            {
                case WebSocketType.Native:
                    return new Native.EtpSelfHostedWebServer(port, application, version);
                case WebSocketType.WebSocket4Net:
                    return new WebSocket4Net.EtpSelfHostedWebServer(port, application, version);

                default:
                    throw new ArgumentException($"Unrecognized WebSocket type: {webSocketType}", "webSocketType");
            }
        }
        #endregion
    }
}

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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using Energistics.Etp.Common.Datatypes;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Defines static helper methods that can be used to validate ETP websocket sessions.
    /// </summary>
    public static class EtpWebSocketValidation
    {
        /// <summary>
        /// Checks whether the websocket session is requesting an upgrade.
        /// </summary>
        /// <param name="requestHeaders">The request headers.</param>
        /// <returns><c>true</c> if the websocket session is requesting an upgrade; <c>false</c> otherwise.</returns>
        public static bool IsWebSocketRequestUpgrading(IDictionary<string, string> requestHeaders)
        {
            return requestHeaders != null && requestHeaders.ContainsKey("Upgrade");
        }

        /// <summary>
        /// Gets the preferred ETP websocket subprotocol for the session.
        /// </summary>
        /// <param name="requestHeaders">The request headers.</param>
        /// <returns>The preferred ETP websocket subprotocol if a recognized subprotocol was requested; <c>null</c> otherwise.</returns>
        public static string GetPreferredSubProtocol(IDictionary<string, string> requestHeaders)
        {
            if (requestHeaders == null) return null;

            var value = requestHeaders["Sec-WebSocket-Protocol"];
            if (string.IsNullOrEmpty(value)) return null;

            var requestedProtocols = value.Split(',');
            foreach (var requestedProtocol in requestedProtocols)
            {
                var version = GetEtpVersion(requestedProtocol);
                if (version == EtpVersion.v11)
                    return requestedProtocol;
                if (version == EtpVersion.v12)
                    return requestedProtocol;
            }

            return null;
        }

        /// <summary>
        /// Gets the supported websocket subprotocols based on the supported ETP versions.
        /// </summary>
        /// <param name="supportedVersions">The supported ETP versions.</param>
        /// <returns>The supported websocket subprotocols.</returns>
        public static List<string> GetSupportedSubProtocols(IEnumerable<EtpVersion> supportedVersions)
        {
            var supportedSubProtocols = new List<string>();
            foreach (var version in supportedVersions)
            {
                if (version == EtpVersion.v11)
                    supportedSubProtocols.Add(EtpSubProtocols.v11);
                else if (version == EtpVersion.v12)
                    supportedSubProtocols.Add(EtpSubProtocols.v12);
            }

            return supportedSubProtocols;
        }
        /// <summary>
        /// Gets the ETP version for the session.
        /// </summary>
        /// <param name="requestHeaders">The request headers.</param>
        /// <returns>The ETP version if a recognized version was requested; <see cref="EtpVersion.Unknown"/> otherwise.</returns>
        public static EtpVersion GetEtpVersion(IDictionary<string, string> requestHeaders)
        {
            return GetEtpVersion(GetPreferredSubProtocol(requestHeaders));
        }

        /// <summary>
        /// Gets the ETP encoding for the session.
        /// </summary>
        /// <param name="requestHeaders">The request headers.</param>
        /// <returns>The ETP encoding if a recognized; <c>null</c> otherweise.</returns>
        public static EtpEncoding? GetEtpEncoding(IDictionary<string, string> requestHeaders)
        {
            if (requestHeaders == null) return null;

            string encoding;

            // Validate etp-encoding header is either binary, json or not specified
            if (!requestHeaders.TryGetValue(EtpHeaders.Encoding, out encoding))
                return EtpEncoding.Binary;

            return GetEtpEncoding(encoding);
        }

        /// <summary>
        /// Gets the <see cref="EtpVersion"/> for the specified sub protocol.
        /// </summary>
        /// <param name="subProtocol">The ETP websocket subprotocol.</param>
        /// <returns><see cref="EtpVersion.v12"/> if the subprotocol is etp12.energistics.org; <see cref="EtpVersion.v11"/> if the subprotocol is energitics-tp; <see cref="EtpVersion.Unknown"/> otherwise.</returns>
        public static EtpVersion GetEtpVersion(string subProtocol)
        {
            if (EtpSubProtocols.v11.Equals(subProtocol, StringComparison.OrdinalIgnoreCase))
                return EtpVersion.v11;
            if (EtpSubProtocols.v12.Equals(subProtocol, StringComparison.OrdinalIgnoreCase))
                return EtpVersion.v12;

            return EtpVersion.Unknown;
        }

        /// <summary>
        /// Gets the <see cref="EtpEncoding"/> for the specified header value.
        /// </summary>
        /// <param name="encoding">The ETP encoding header value.</param>
        /// <returns><see cref="EtpEncoding.Binary"/> if the subprotocol is binary; <see cref="EtpEncoding.Json"/> if the subprotocol is JSON; <c>null</c> otherwise.</returns>
        public static EtpEncoding? GetEtpEncoding(string encoding)
        {
            if (string.Equals(encoding, EtpEncoding.Binary.ToHeaderValue(), StringComparison.OrdinalIgnoreCase))
                return EtpEncoding.Binary;
            if (string.Equals(encoding, EtpEncoding.Json.ToHeaderValue(), StringComparison.OrdinalIgnoreCase))
                return EtpEncoding.Json;

            return null;
        }

        /// <summary>
        /// Checks if a <see cref="Exception"/> is due to various low-level errors indicating the connection has been terminated.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <returns><c>true</c> if the exception indicates the closed has been terminated; <c>false</c> otherwise.</returns>
        public static bool ExceptionMeansConnectionTerminated(this Exception ex)
        {
            var terminated = false;
            
            // Errors indicating problems with connections
            terminated = terminated || ex.WebSocketExceptionMeansConnectionTerminated();
            terminated = terminated || ex.SocketExceptionMeansConnectionTerminated();

            // Errors indicating the connection has already been closed when attempting to do something
            terminated = terminated || ex.HttpListenerExceptionMeansConnectionTerminated();
            terminated = terminated || ex.ApplicationExceptionMeansConnectionTerminated();
            terminated = terminated || ex.InvalidOperationExceptionMeansConnectionTerminated();

            return terminated;
        }

        /// <summary>
        /// Checks if a <see cref="ApplicationException"/> is due to various low-level errors indicating the connection has been terminated.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns><c>true</c> if the exception indicates the closed has been terminated; <c>false</c> otherwise.</returns>
        private static bool ApplicationExceptionMeansConnectionTerminated(this Exception exception)
        {
            var ex = exception as ApplicationException ?? exception.InnerException as ApplicationException;
            if (ex == null) return false;

            int windowsErrorCode = (int)((uint)ex.HResult & 0x0000FFFF);
            return WindowsErrorCodeMeansConnectionTerminated(windowsErrorCode);
        }

        /// <summary>
        /// Checks if a <see cref="InvalidOperationException"/> is due to various low-level errors indicating the connection has been terminated.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns><c>true</c> if the exception indicates the closed has been terminated; <c>false</c> otherwise.</returns>
        private static bool InvalidOperationExceptionMeansConnectionTerminated(this Exception exception)
        {
            var ex = exception as InvalidOperationException ?? exception.InnerException as InvalidOperationException;
            if (ex == null) return false;

            return true;
        }

        /// <summary>
        /// Checks if a <see cref="WebSocketException"/> is due to various low-level errors indicating the connection has been terminated.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns><c>true</c> if the exception indicates the closed has been terminated; <c>false</c> otherwise.</returns>
        private static bool WebSocketExceptionMeansConnectionTerminated(this Exception exception)
        {
            var ex = exception as WebSocketException ?? exception.InnerException as WebSocketException;
            if (ex == null) return false;

            int windowsErrorCode = (int)((uint)ex.ErrorCode & 0x0000FFFF);
            if (WindowsErrorCodeMeansConnectionTerminated(windowsErrorCode))
                return true;

            if (ex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely ||
                ex.WebSocketErrorCode == WebSocketError.InvalidState)
            {
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Checks if a <see cref="SocketException"/> is due to various low-level errors indicating the connection has been terminated.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns><c>true</c> if the exception indicates the closed has been terminated; <c>false</c> otherwise.</returns>
        private static bool SocketExceptionMeansConnectionTerminated(this Exception exception)
        {
            var ex = exception as SocketException ?? exception.InnerException as SocketException;
            if (ex == null) return false;

            return SocketErrorCodeMeansConnectionTerminated((int)ex.SocketErrorCode);
        }

        /// <summary>
        /// Checks if a <see cref="HttpListenerException"/> is due to various low-level errors indicating the connection has been terminated.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns><c>true</c> if the exception indicates the closed has been terminated; <c>false</c> otherwise.</returns>
        private static bool HttpListenerExceptionMeansConnectionTerminated(this Exception exception)
        {
            var ex = exception as HttpListenerException ?? exception.InnerException as HttpListenerException;
            if (ex == null) return false;

            return WindowsErrorCodeMeansConnectionTerminated(ex.ErrorCode);
        }

        /// <summary>
        /// Checks if the windows error code means that the connection has been terminated.
        /// </summary>
        /// <param name="windowsErrorCode">The windows error code</param>
        /// <returns><c>true</c> if the windows error code indicates the closed has been terminated; <c>false</c> otherwise.</returns>
        private static bool WindowsErrorCodeMeansConnectionTerminated(int windowsErrorCode)
        {
            switch (windowsErrorCode)
            {
                case 0x0001: // ERROR_INVALID_FUNCTION
                case 0x0006: // ERROR_INVALID_HANDLE
                case 0x0026: // ERROR_HANDLE_EOF
                case 0x03e3: // ERROR_INVALID_HANDLE
                case 0x04cd: // ERROR_CONNECTION_INVALID
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks if the socket error code means that the connection has been terminated.
        /// </summary>
        /// <param name="socketErrorCode">The socket error code</param>
        /// <returns><c>true</c> if the socket error code indicates the closed has been terminated; <c>false</c> otherwise.</returns>
        private static bool SocketErrorCodeMeansConnectionTerminated(int socketErrorCode)
        {
            switch (socketErrorCode)
            {
                case (int)SocketError.ConnectionAborted:
                case (int)SocketError.ConnectionReset:
                case (int)SocketError.NotConnected:
                case (int)SocketError.OperationAborted:
                    return true;

                default:
                    return false;
            }
        }
    }
}

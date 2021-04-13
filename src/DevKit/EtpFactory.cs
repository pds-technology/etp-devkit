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
using Energistics.Etp.Common.Datatypes.DataArrayTypes;
using Energistics.Etp.Common.Datatypes.Object;
using Energistics.Etp.Common.Protocol.Core;
using Energistics.Etp.Properties;
using Energistics.Etp.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace Energistics.Etp
{
    /// <summary>
    /// Provides a factory to create various ETP interface instances.
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
        /// <summary>
        /// Creates an <see cref="IEtpClient"/> using the default WebSocket type.
        /// </summary>
        /// <param name="uri">The ETP server URI.</param>
        /// <param name="etpVersion">The ETP version for the session.</param>
        /// <param name="info">The client's information.</param>
        /// <param name="parameters">The client's parameters.</param>
        /// <param name="authorization">The client's authorization details.</param>
        /// <returns>The <see cref="IEtpClient"/></returns>
        public static IEtpClient CreateClient(string uri, EtpVersion etpVersion, EtpEndpointInfo info, EtpEndpointParameters parameters = null, Authorization authorization = null)
        {
            return CreateClient(EtpSettings.DefaultWebSocketType, uri, etpVersion, EtpSettings.DefaultEncoding, info, parameters: parameters, authorization: authorization);
        }

        /// <summary>
        /// Creates an <see cref="IEtpClient"/> using the default WebSocket type.
        /// </summary>
        /// <param name="uri">The ETP server URI.</param>
        /// <param name="etpVersion">The ETP version for the session.</param>
        /// <param name="encoding">The ETP encoding for the session.</param>
        /// <param name="info">The client's information.</param>
        /// <param name="parameters">The client's parameters.</param>
        /// <param name="authorization">The client's authorization details.</param>
        /// <returns>The <see cref="IEtpClient"/></returns>
        public static IEtpClient CreateClient(string uri, EtpVersion etpVersion, EtpEncoding encoding, EtpEndpointInfo info, EtpEndpointParameters parameters = null, Authorization authorization = null)
        {
            return CreateClient(EtpSettings.DefaultWebSocketType, uri, etpVersion, encoding, info, parameters: parameters, authorization: authorization);
        }

        /// <summary>
        /// Creates an <see cref="IEtpClient"/> using the specified WebSocket type.
        /// </summary>
        /// <param name="webSocketType">The specified WebSocket type.</param>
        /// <param name="uri">The ETP server URI.</param>
        /// <param name="etpVersion">The ETP version for the session.</param>
        /// <param name="info">The client's information.</param>
        /// <param name="parameters">The client's parameters.</param>
        /// <param name="authorization">The client's authorization details.</param>
        /// <returns>The <see cref="IEtpClient"/></returns>
        public static IEtpClient CreateClient(WebSocketType webSocketType, string uri, EtpVersion etpVersion, EtpEndpointInfo info, EtpEndpointParameters parameters = null, Authorization authorization = null)
        {
            return CreateClient(webSocketType, uri, etpVersion, EtpSettings.DefaultEncoding, info, parameters: parameters, authorization: authorization);
        }

        /// <summary>
        /// Creates an <see cref="IEtpClient"/> using the specified WebSocket type.
        /// </summary>
        /// <param name="webSocketType">The specified WebSocket type.</param>
        /// <param name="uri">The ETP server URI.</param>
        /// <param name="etpVersion">The ETP version for the session.</param>
        /// <param name="encoding">The ETP encoding for the session.</param>
        /// <param name="info">The client's information.</param>
        /// <param name="parameters">The client's parameters.</param>
        /// <param name="authorization">The client's authorization details.</param>
        /// <param name="headers">The client's additional HTTP headers.</param>
        /// <returns>The <see cref="IEtpClient"/></returns>
        public static IEtpClient CreateClient(WebSocketType webSocketType, string uri, EtpVersion etpVersion, EtpEncoding encoding, EtpEndpointInfo info, EtpEndpointParameters parameters = null, Authorization authorization = null, IDictionary<string, string> headers = null)
        {
            if (webSocketType == WebSocketType.Native && !IsNativeSupported)
                webSocketType = FallbackWebSocketType;

            switch (webSocketType)
            {
                case WebSocketType.Native:
                    return new Native.EtpClient(uri, etpVersion, encoding, info, parameters: parameters, authorization: authorization, headers: headers);
                case WebSocketType.WebSocket4Net:
                    return new WebSocket4Net.EtpClient(uri, etpVersion, encoding, info, parameters: parameters, authorization: authorization, headers: headers);

                default:
                    throw new ArgumentException($"Unrecognized WebSocket type: {webSocketType}", "webSocketType");
            }
        }
        #endregion

        #region IEtpServerManager
        /// <summary>
        /// Creates an <see cref="IEtpServerManager"/> using the default WebSocket type.
        /// </summary>
        /// <param name="webServerDetails">The web server details.</param>
        /// <param name="endpointInfo">The server manager's endpoint information.</param>
        /// <param name="endpointParameters">The server manager's endpoint parameters.</param>
        /// <returns>The <see cref="IEtpServerManager"/></returns>
        public static IEtpServerManager CreateServerManager(EtpWebServerDetails webServerDetails, EtpEndpointInfo endpointInfo, EtpEndpointParameters endpointParameters = null)
        {
            return CreateServerManager(EtpSettings.DefaultWebSocketType, webServerDetails, endpointInfo, endpointParameters: endpointParameters);
        }

        /// <summary>
        /// Creates an <see cref="IEtpServerManager"/> using the specified WebSocket type.
        /// </summary>
        /// <param name="webSocketType">The specified WebSocket type.</param>
        /// <param name="webServerDetails">The web server details.</param>
        /// <param name="endpointInfo">The server manager's endpoint information.</param>
        /// <param name="endpointParameters">The server manager's endpoint parameters.</param>
        /// <returns>The <see cref="IEtpServerManager"/></returns>
        public static IEtpServerManager CreateServerManager(WebSocketType webSocketType, EtpWebServerDetails webServerDetails, EtpEndpointInfo endpointInfo, EtpEndpointParameters endpointParameters = null)
        {
            if (webSocketType == WebSocketType.Native && !IsNativeSupported)
                webSocketType = FallbackWebSocketType;

            switch (webSocketType)
            {
                case WebSocketType.Native:
                    return new Native.EtpServerManager(webServerDetails, endpointInfo, endpointParameters: endpointParameters);
#if NETFRAMEWORK
                case WebSocketType.WebSocket4Net:
                    return new WebSocket4Net.EtpServerManager(webServerDetails, endpointInfo, endpointParameters: endpointParameters);
#endif
                default:
                    throw new ArgumentException($"Unsupported WebSocket type: {webSocketType}", "webSocketType");
            }
        }
#endregion

        #region IEtpSelfHostedWebServer

        /// <summary>
        /// Creates an <see cref="IEtpSelfHostedWebServer"/> using the default WebSocket type.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="endpointInfo">The web server's endpoint information.</param>
        /// <param name="endpointParameters">The web server's endpoint parameters.</param>
        /// <param name="details">The web server's details.</param>
        /// <returns>The <see cref="IEtpSelfHostedWebServer"/></returns>
        public static IEtpSelfHostedWebServer CreateSelfHostedWebServer(int port, EtpEndpointInfo endpointInfo, EtpEndpointParameters endpointParameters = null, EtpWebServerDetails details = null)
        {
            return CreateSelfHostedWebServer(EtpSettings.DefaultWebSocketType, port, endpointInfo, endpointParameters, details: details);
        }

        /// <summary>
        /// Creates an <see cref="IEtpSelfHostedWebServer"/> using the specified WebSocket type.
        /// </summary>
        /// <param name="webSocketType">The specified WebSocket type.</param>
        /// <param name="port">The port.</param>
        /// <param name="endpointInfo">The web server's endpoint information.</param>
        /// <param name="endpointParameters">The web server's endpoint parameters.</param>
        /// <param name="details">The web server's details.</param>
        /// <returns>The <see cref="IEtpSelfHostedWebServer"/></returns>
        public static IEtpSelfHostedWebServer CreateSelfHostedWebServer(WebSocketType webSocketType, int port, EtpEndpointInfo endpointInfo, EtpEndpointParameters endpointParameters = null, EtpWebServerDetails details = null)
        {
            if (webSocketType == WebSocketType.Native && !IsNativeSupported)
                webSocketType = FallbackWebSocketType;

            switch (webSocketType)
            {
                case WebSocketType.Native:
                    return new Native.EtpSelfHostedWebServer(port, endpointInfo, endpointParameters: endpointParameters, details: details);
#if NETFRAMEWORK
                case WebSocketType.WebSocket4Net:
                    return new WebSocket4Net.EtpSelfHostedWebServer(port, endpointInfo, endpointParameters: endpointParameters, details: details);
#endif
                default:
                    throw new ArgumentException($"Unsupported WebSocket type: {webSocketType}", "webSocketType");
            }
        }
#endregion
        
        #region Miscellaneous

        /// <summary>
        /// Creates a client endpoint info
        /// </summary>
        /// <param name="applicationName">The application name</param>
        /// <param name="applicationVersion">The application version</param>
        /// <param name="clientId">The client ID, which is generally the username but may be some other identifying information.</param>
        /// <returns>The client endpoint info</returns>
        public static EtpEndpointInfo CreateClientEndpointInfo(string applicationName, string applicationVersion, string clientId)
        {
            var key = $"Application: {applicationName}; Version: {applicationVersion}; ID: {clientId}";

            return EtpEndpointInfo.FromKey(applicationName, applicationVersion, key);
        }

        /// <summary>
        /// Creates a server endpoint info
        /// </summary>
        /// <param name="applicationName">The application name</param>
        /// <param name="applicationVersion">The application version</param>
        /// <param name="useMacAddress">Whether or not to use the server MAC address in the key.</param>
        /// <returns>The server endpoint info</returns>
        public static EtpEndpointInfo CreateServerEndpointInfo(string applicationName, string applicationVersion, bool useMacAddress = true)
        {
            string key;
            if (useMacAddress)
            {
                var macAddress = NetworkInterface.GetAllNetworkInterfaces()
                                .Where(nic => nic.OperationalStatus == OperationalStatus.Up)
                                .OrderByDescending(nic => nic.GetIPStatistics().BytesSent + nic.GetIPStatistics().BytesReceived)
                                .FirstOrDefault()?.GetPhysicalAddress().ToString();

                key = $"Application: {applicationName}; Version: {applicationVersion}; MAC Address: {macAddress}";
            }
            else
            {
                key = $"Application: {applicationName}; Version: {applicationVersion}";
            }

            return EtpEndpointInfo.FromKey(applicationName, applicationVersion, key);
        }

        /// <summary>
        /// Gets the WebSocket sub protocol for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <returns>The WebSocket sub protocol</returns>
        public static string GetSubProtocol(EtpVersion version)
        {
            switch (version)
            {
                case EtpVersion.v11: return EtpSubProtocols.v11;
                case EtpVersion.v12: return EtpSubProtocols.v12;
                default:
                    {
                        var message = $"Unsupported ETP version: {version}.";
                        Logger.Debug(message);
                        throw new InvalidOperationException(message);
                    }
            }
        }

        /// <summary>
        /// Creates an <see cref="IEtpAdapter"/> instance for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <returns>A new <see cref="IEtpAdapter"/> instance.</returns>
        public static IEtpAdapter CreateEtpAdapter(EtpVersion version)
        {
            switch (version)
            {
                case EtpVersion.v11: return new v11.Etp11Adapter();
                case EtpVersion.v12: return new v12.Etp12Adapter();
                default:
                    {
                        var message = $"Unsupported ETP version: {version}.";
                        Logger.Debug(message);
                        throw new InvalidOperationException(message);
                    }
            }
        }

        /// <summary>
        /// Creates a message header for the specified protocol and message type.
        /// </summary>
        /// <typeparam name="TProtocol">The protocol enum.</typeparam>
        /// <typeparam name="TMessageType">The message type enum.</typeparam>
        /// <param name="protocol">The protocol.</param>
        /// <param name="messageType">The message type.</param>
        /// <returns>A new message header instance.</returns>
        public static IMessageHeader CreateMessageHeader<TProtocol, TMessageType>(EtpVersion version, TProtocol protocol, TMessageType messageType) where TProtocol : struct, IConvertible where TMessageType : struct, IConvertible
        {
            return CreateMessageHeader(version, Convert.ToInt32(protocol), Convert.ToInt32(messageType));
        }

        /// <summary>
        /// Creates a message header for the specified protocol and message type.
        /// </summary>
        /// <typeparam name="TMessageType">The message type enum.</typeparam>
        /// <param name="protocol">The protocol.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <returns>A new message header instance.</returns>
        public static IMessageHeader CreateMessageHeader<TMessageType>(EtpVersion version, int protocol, TMessageType messageType) where TMessageType : struct, IConvertible
        {
            return CreateMessageHeader(version, protocol, Convert.ToInt32(messageType));
        }

        /// <summary>
        /// Creates a message header for the specified protocol and message type.
        /// </summary>
        /// <param name="protocol">The protocol.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <returns>A new message header instance.</returns>
        public static IMessageHeader CreateMessageHeader(EtpVersion version, int protocol, int messageType)
        {
            var header = CreateMessageHeader(version);

            header.Protocol = protocol;
            header.MessageType = messageType;
            header.MessageId = 0; // MessageId needs to be set just before sending to ensure proper sequencing
            header.MessageFlags = 0;
            header.CorrelationId = 0;

            return header;
        }

        /// <summary>
        /// Creates an <see cref="IMessageHeader"/> instance for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <returns>A new <see cref="IMessageHeader"/> instance.</returns>
        public static IMessageHeader CreateMessageHeader(EtpVersion version)
        {
            switch (version)
            {
                case EtpVersion.v11: return new v11.Datatypes.MessageHeader();
                case EtpVersion.v12: return new v12.Datatypes.MessageHeader();
                default:
                    {
                        var message = $"Unsupported ETP version: {version}.";
                        Logger.Debug(message);
                        throw new InvalidOperationException(message);
                    }
            }
        }

        /// <summary>
        /// Creates an <see cref="IMessageHeaderExtension"/> instance for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <returns>A new <see cref="IMessageHeaderExtension"/> instance.</returns>
        public static IMessageHeaderExtension CreateMessageHeaderExtension(EtpVersion version)
        {
            switch (version)
            {
                case EtpVersion.v11: return null;
                case EtpVersion.v12: return new v12.Datatypes.MessageHeaderExtension();
                default:
                    {
                        var message = $"Unsupported ETP version: {version}.";
                        Logger.Debug(message);
                        throw new InvalidOperationException(message);
                    }
            }
        }

        /// <summary>
        /// Creates an <see cref="IAcknowledge"/> instance for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <returns>A new <see cref="IAcknowledge"/> instance.</returns>
        public static IAcknowledge CreateAcknowledge(EtpVersion version)
        {
            switch (version)
            {
                case EtpVersion.v11: return new v11.Protocol.Core.Acknowledge();
                case EtpVersion.v12: return new v12.Protocol.Core.Acknowledge();
                default:
                    {
                        var message = $"Unsupported ETP version: {version}.";
                        Logger.Debug(message);
                        throw new InvalidOperationException(message);
                    }
            }
        }

        /// <summary>
        /// Creates an <see cref="IErrorInfo"/> instance for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <returns>A new <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo CreateErrorInfo(EtpVersion version)
        {
            switch (version)
            {
                case EtpVersion.v11: return new v11.Protocol.Core.ProtocolException();
                case EtpVersion.v12: return new v12.Datatypes.ErrorInfo();
                default:
                    {
                        var message = $"Unsupported ETP version: {version}.";
                        Logger.Debug(message);
                        throw new InvalidOperationException(message);
                    }
            }
        }

        /// <summary>
        /// Creates an <see cref="IProtocolException"/> instance for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <param name="error">The error info to base the protocol exception on.</param>
        /// <returns>A new <see cref="IProtocolException"/> instance.</returns>
        public static IProtocolException CreateProtocolException(EtpVersion version, IErrorInfo error)
        {
            switch (version)
            {
                case EtpVersion.v11: return new v11.Protocol.Core.ProtocolException { ErrorCode = error?.Code ?? 0, ErrorMessage = error?.Message ?? string.Empty };
                case EtpVersion.v12: return new v12.Protocol.Core.ProtocolException { Error = error == null ? null : new v12.Datatypes.ErrorInfo { Code = error.Code, Message = error.Message ?? string.Empty }, Errors = new Dictionary<string, v12.Datatypes.ErrorInfo>() };
                default:
                    {
                        var message = $"Unsupported ETP version: {version}.";
                        Logger.Debug(message);
                        throw new InvalidOperationException(message);
                    }
            }
        }

        /// <summary>
        /// Creates an <see cref="IProtocolException"/> instance for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <param name="errors">The error infos to base the protocol exception on.</param>
        /// <returns>A new <see cref="IProtocolException"/> instance.</returns>
        public static IList<IProtocolException> CreateProtocolExceptions(EtpVersion version, IDictionary<string, IErrorInfo> errors)
        {
            switch (version)
            {
                case EtpVersion.v11: return (errors == null || errors.Count == 0)
                        ? new List<IProtocolException>()
                        : errors.Values.Select(v => CreateProtocolException(version, v)).ToList();
                case EtpVersion.v12: return new List<IProtocolException> { new v12.Protocol.Core.ProtocolException { Errors = errors.ToErrorDictionary<v12.Datatypes.ErrorInfo>() } };
                default:
                    {
                        var message = $"Unsupported ETP version: {version}.";
                        Logger.Debug(message);
                        throw new InvalidOperationException(message);
                    }
            }
        }

        /// <summary>
        /// Creates an <see cref="IRequestSession"/> instance for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <returns>A new <see cref="IRequestSession"/> instance.</returns>
        public static IRequestSession CreateRequestSession(EtpVersion version)
        {
            switch (version)
            {
                case EtpVersion.v11: return new v11.Protocol.Core.RequestSession();
                case EtpVersion.v12: return new v12.Protocol.Core.RequestSession();
                default:
                    {
                        var message = $"Unsupported ETP version: {version}.";
                        Logger.Debug(message);
                        throw new InvalidOperationException(message);
                    }
            }
        }

        /// <summary>
        /// Creates an <see cref="IRequestSession"/> instance for the specified ETP version from the specified info and details.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <param name="clientInfo">The client info.</param>
        /// <param name="clientDetails">The client details.</param>
        /// <returns>A new <see cref="IRequestSession"/> instance.</returns>
        public static IRequestSession CreateRequestSession(EtpVersion version, EtpEndpointInfo clientInfo, IEndpointDetails clientDetails)
        {
            var requestSession = CreateRequestSession(version);

            requestSession.ApplicationName = clientInfo.ApplicationName ?? string.Empty;
            requestSession.ApplicationVersion = clientInfo.ApplicationVersion ?? string.Empty;
            requestSession.ClientInstanceId = clientInfo.InstanceId;

            requestSession.SetRequestedProtocolsFrom(clientDetails.SupportedProtocols.Where(s => s.EtpVersion == version && s.Protocol != (int)Protocols.Core).Select(s => CreateSupportedProtocol(version, s, false)));
            if (version == EtpVersion.v12 || clientDetails.SupportedProtocols.Any(p => string.Equals(p.Role, Roles.Store, StringComparison.OrdinalIgnoreCase)))
                requestSession.SetSupportedDataObjectsFrom(clientDetails.SupportedDataObjects.Select(d => CreateSupportedDataObject(version, d)));
            else
                requestSession.SetSupportedDataObjectsFrom(new List<ISupportedDataObject>());
            requestSession.SupportedCompression = clientDetails.SupportedCompression.ToList();
            requestSession.SupportedFormats = clientDetails.SupportedFormats.ToList();
            requestSession.SetEndpointCapabilitiesFrom(clientDetails.Capabilities);

            return requestSession;
        }

        /// <summary>
        /// Creates an <see cref="IOpenSession"/> instance for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <returns>A new <see cref="IOpenSession"/> instance.</returns>
        public static IOpenSession CreateOpenSession(EtpVersion version)
        {
            switch (version)
            {
                case EtpVersion.v11: return new v11.Protocol.Core.OpenSession();
                case EtpVersion.v12: return new v12.Protocol.Core.OpenSession();
                default:
                    {
                        var message = $"Unsupported ETP version: {version}.";
                        Logger.Debug(message);
                        throw new InvalidOperationException(message);
                    }
            }
        }

        /// <summary>
        /// Creates an <see cref="IOpenSession"/> instance for the specified ETP version from the specified info and details.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <param name="serverInfo">The server info.</param>
        /// <param name="sessionDetails">The session details.</param>
        /// <param name="sessionId">The session ID.</param>
        /// <returns>A new <see cref="IOpenSession"/> instance.</returns>
        public static IOpenSession CreateOpenSession(EtpVersion version, EtpEndpointInfo serverInfo, IEndpointDetails sessionDetails, Guid sessionId)
        {
            var openSession = CreateOpenSession(version);

            openSession.ApplicationName = serverInfo.ApplicationName ?? string.Empty;
            openSession.ApplicationVersion = serverInfo.ApplicationVersion ?? string.Empty;
            openSession.SessionId = sessionId;
            openSession.ServerInstanceId = serverInfo.InstanceId;

            openSession.SetSupportedProtocolsFrom(sessionDetails.SupportedProtocols.Where(s => s.EtpVersion == version && s.Protocol != (int)Protocols.Core).Select(s => CreateSupportedProtocol(version, s, true)));
            if (version == EtpVersion.v12 || sessionDetails.SupportedProtocols.Any(p => string.Equals(p.Role, Roles.Store, StringComparison.OrdinalIgnoreCase)))
                openSession.SetSupportedDataObjectsFrom(sessionDetails.SupportedDataObjects.Select(d => CreateSupportedDataObject(version, d)));
            else
                openSession.SetSupportedDataObjectsFrom(new List<ISupportedDataObject>());
            openSession.SupportedCompression = sessionDetails.SupportedCompression.FirstOrDefault();
            openSession.SupportedFormats = sessionDetails.SupportedFormats.ToList();
            openSession.SetEndpointCapabilitiesFrom(sessionDetails.Capabilities);

            return openSession;
        }

        /// <summary>
        /// Creates an <see cref="ICloseSession"/> instance for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <param name="reason">The reason for closing the session.</param>
        /// <returns>A new <see cref="ICloseSession"/> instance.</returns>
        public static ICloseSession CreateCloseSession(EtpVersion version, string reason)
        {
            switch (version)
            {
                case EtpVersion.v11: return new v11.Protocol.Core.CloseSession { Reason = reason ?? string.Empty };
                case EtpVersion.v12: return new v12.Protocol.Core.CloseSession { Reason = reason ?? string.Empty };
                default:
                    {
                        var message = $"Unsupported ETP version: {version}.";
                        Logger.Debug(message);
                        throw new InvalidOperationException(message);
                    }
            }
        }

        /// <summary>
        /// Creates an <see cref="IServerCapabilities"/> instance for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <returns>A new <see cref="IServerCapabilities"/> instance.</returns>
        public static IServerCapabilities CreateServerCapabilities(EtpVersion version)
        {
            switch (version)
            {
                case EtpVersion.v11: return new v11.Datatypes.ServerCapabilities();
                case EtpVersion.v12: return new v12.Datatypes.ServerCapabilities();
                default:
                    {
                        var message = $"Unsupported ETP version: {version}.";
                        Logger.Debug(message);
                        throw new InvalidOperationException(message);
                    }
            }
        }

        /// <summary>
        /// Creates an <see cref="IServerCapabilities"/> instance for the specified ETP version from the specified info and details.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <param name="webServerDetails">The web server details.</param>
        /// <param name="info">The endpoint info.</param>
        /// <param name="details">The endpoint details.</param>
        /// <returns>A new <see cref="IServerCapabilities"/> instance.</returns>
        public static IServerCapabilities CreateServerCapabilities(EtpVersion version, EtpWebServerDetails webServerDetails, EtpEndpointInfo info, IEndpointDetails details)
        {
            var serverCapabilities = CreateServerCapabilities(version);

            serverCapabilities.ApplicationName = info.ApplicationName ?? string.Empty;
            serverCapabilities.ApplicationVersion = info.ApplicationVersion ?? string.Empty;
            serverCapabilities.ContactInformation = CreateContact(version, webServerDetails.OrganizationName, webServerDetails.ContactName, webServerDetails.ContactPhone, webServerDetails.ContactEmail);

            serverCapabilities.SupportedCompression = details.SupportedCompression.ToList();
            serverCapabilities.SupportedEncodings = webServerDetails.SupportedEncodings.Select(e => e.ToHeaderValue()).ToList();
            serverCapabilities.SupportedFormats = details.SupportedFormats.ToList();

            serverCapabilities.SetSupportedProtocolsFrom(details.SupportedProtocols.Where(s => s.EtpVersion == version && s.Protocol != (int)Protocols.Core).Select(s => CreateSupportedProtocol(version, s, true)));
            serverCapabilities.SetSupportedDataObjectsFrom(details.SupportedDataObjects.Select(d => CreateSupportedDataObject(version, d)));
            serverCapabilities.SetEndpointCapabilitiesFrom(details.Capabilities);

            return serverCapabilities;
        }


        /// <summary>
        /// Creates an <see cref="IContact"/> instance for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <param name="organizationName">The name of the organization.</param>
        /// <param name="contactName">The contact name.</param>
        /// <param name="contactPhone">The contact phone.</param>
        /// <param name="contactEmail">The contact e-mail.</param>
        /// <returns>A new <see cref="IContact"/> instance.</returns>
        public static IContact CreateContact(EtpVersion version, string organizationName, string contactName, string contactPhone, string contactEmail)
        {
            switch (version)
            {
                case EtpVersion.v11: return new v11.Datatypes.Contact { OrganizationName = organizationName ?? string.Empty, ContactName = contactName ?? string.Empty, ContactPhone = contactPhone ?? string.Empty, ContactEmail = contactEmail ?? string.Empty };
                case EtpVersion.v12: return new v12.Datatypes.Contact { OrganizationName = organizationName ?? string.Empty, ContactName = contactName ?? string.Empty, ContactPhone = contactPhone ?? string.Empty, ContactEmail = contactEmail ?? string.Empty };
                default:
                    {
                        var message = $"Unsupported ETP version: {version}.";
                        Logger.Debug(message);
                        throw new InvalidOperationException(message);
                    }
            }
        }

        /// <summary>
        /// Creates an <see cref="IDataValue"/> instance for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <param name="item">The item for the data value.</param>
        /// <returns>A new <see cref="IDataValue"/> instance.</returns>
        public static IDataValue CreateDataValue(EtpVersion version, object item = null)
        {
            switch (version)
            {
                case EtpVersion.v11: return new v11.Datatypes.DataValue { Item = item };
                case EtpVersion.v12: return new v12.Datatypes.DataValue { Item = item };
                default:
                    {
                        var message = $"Unsupported ETP version: {version}.";
                        Logger.Debug(message);
                        throw new InvalidOperationException(message);
                    }
            }
        }

        /// <summary>
        /// Creates an <see cref="IDataValue"/> instance for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <returns>A new <see cref="IDataValue"/> instance.</returns>
        public static IDataValueDictionary CreateDataValueDictionary(EtpVersion version)
        {
            switch (version)
            {
                case EtpVersion.v11: return new EtpDataValueDictionaryWrapper<v11.Datatypes.DataValue>();
                case EtpVersion.v12: return new EtpDataValueDictionaryWrapper<v12.Datatypes.DataValue>();
                case EtpVersion.Unknown: return new EtpDataValueDictionaryWrapper<CommonDataValue>();
                default:
                    {
                        var message = $"Unsupported ETP version: {version}.";
                        Logger.Debug(message);
                        throw new InvalidOperationException(message);
                    }
            }
        }

        /// <summary>
        /// Creates an <see cref="IAnyArray"/> instance for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <param name="data">The array data.</param>
        /// <returns>A new <see cref="IAnyArray"/> instance.</returns>
        public static IAnyArray CreateAnyArray(EtpVersion version, object data = null)
        {
            switch (version)
            {
                case EtpVersion.v11: return new v11.Datatypes.AnyArray { Item = data };
                case EtpVersion.v12: return new v12.Datatypes.AnyArray { Item = data };
                default:
                    {
                        var message = $"Unsupported ETP version: {version}.";
                        Logger.Debug(message);
                        throw new InvalidOperationException(message);
                    }
            }
        }

        /// <summary>
        /// Creates an <see cref="IDataArray"/> instance for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <param name="dimensions">The array dimensions.</param>
        /// <param name="data">The array data.</param>
        /// <returns>A new <see cref="IDataArray"/> instance.</returns>
        public static IDataArray CreateDataArray(EtpVersion version, IList<long> dimensions = null, object data = null)
        {
            switch (version)
            {
                case EtpVersion.v11: return new v11.Protocol.DataArray.DataArray { Dimensions = dimensions, Data = data == null ? null : new v11.Datatypes.AnyArray { Item = data } };
                case EtpVersion.v12: return new v12.Datatypes.DataArrayTypes.DataArray { Dimensions = dimensions, Data = data == null ? null : new v12.Datatypes.AnyArray { Item = data } };
                default:
                    {
                        var message = $"Unsupported ETP version: {version}.";
                        Logger.Debug(message);
                        throw new InvalidOperationException(message);
                    }
            }
        }

        /// <summary>
        /// Creates an <see cref="IUuid"/> instance for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <param name="guid">The Guid to create the UUID from.</param>
        /// <returns>A new <see cref="IDataValue"/> instance.</returns>
        public static IUuid CreateUuid(EtpVersion version, Guid guid)
        {
            switch (version)
            {
                case EtpVersion.v11: return guid.ToUuid<CommonUuid>();
                case EtpVersion.v12: return guid.ToUuid<v12.Datatypes.Uuid>();
                case EtpVersion.Unknown: return guid.ToUuid<CommonUuid>();
                default:
                    {
                        var message = $"Unsupported ETP version: {version}.";
                        Logger.Debug(message);
                        throw new InvalidOperationException(message);
                    }
            }
        }

        /// <summary>
        /// Creates an <see cref="IUuid"/> instance for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <param name="guid">The Guid to create the UUID from.</param>
        /// <returns>A new <see cref="IDataValue"/> instance.</returns>
        public static IUuid CreateUuid(EtpVersion version, string guid)
        {
            return CreateUuid(version, Guid.Parse(guid));
        }

        /// <summary>
        /// Creates an <see cref="ISupportedProtocol"/> instance for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <returns>A new <see cref="ISupportedProtocol"/> instance.</returns>
        public static ISupportedProtocol CreateSupportedProtocol(EtpVersion version)
        {
            switch (version)
            {
                case EtpVersion.v11: return new v11.Datatypes.SupportedProtocol();
                case EtpVersion.v12: return new v12.Datatypes.SupportedProtocol();
                default:
                    {
                        var message = $"Unsupported ETP version: {version}.";
                        Logger.Debug(message);
                        throw new InvalidOperationException(message);
                    }
            }
        }

        /// <summary>
        /// Creates an <see cref="ISupportedProtocol"/> instance for the specified ETP version from the specified endpoint protocol.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <param name="endpointProtocol">The endpoint protocol.</param>
        /// <param name="useRole">If <c>true</c>, the endpoint's role is used; otherwise the counterpart's role is used.</param>
        /// <returns>A new <see cref="IOpenSession"/> instance.</returns>
        public static ISupportedProtocol CreateSupportedProtocol(EtpVersion version, IEndpointProtocol endpointProtocol, bool useRole)
        {
            var supportedProtocol = CreateSupportedProtocol(version);

            supportedProtocol.EtpVersion = version;
            supportedProtocol.Protocol = endpointProtocol.Protocol;
            supportedProtocol.Role = useRole ? endpointProtocol.Role : endpointProtocol.CounterpartRole;
            supportedProtocol.SetProtocolCapabilitiesFrom(endpointProtocol.Capabilities);

            return supportedProtocol;
        }

        /// <summary>
        /// Creates an <see cref="ISupportedDataObject"/> instance for the specified ETP version.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <returns>A new <see cref="ISupportedDataObject"/> instance.</returns>
        public static ISupportedDataObject CreateSupportedDataObject(EtpVersion version)
        {
            switch (version)
            {
                case EtpVersion.v11: return new v11.Datatypes.SupportedDataObject();
                case EtpVersion.v12: return new v12.Datatypes.SupportedDataObject();
                default:
                    {
                        var message = $"Unsupported ETP version: {version}.";
                        Logger.Debug(message);
                        throw new InvalidOperationException(message);
                    }
            }
        }

        /// <summary>
        /// Creates an <see cref="ISupportedDataObject"/> instance for the specified ETP version from the specified endpoint protocol.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <param name="endpointDataObject">The endpoint data object.</param>
        /// <returns>A new <see cref="IOpenSession"/> instance.</returns>
        public static ISupportedDataObject CreateSupportedDataObject(EtpVersion version, IEndpointSupportedDataObject endpointDataObject)
        {
            var supportedDataObject = CreateSupportedDataObject(version);

            supportedDataObject.QualifiedType = endpointDataObject.QualifiedType;
            supportedDataObject.DataObjectCapabilities = endpointDataObject.Capabilities.ToList();

            return supportedDataObject;
        }

        /// <summary>
        /// Creates a session key for logging purposes.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <param name="sessionId">The session ID.</param>
        /// <param name="clientInstanceId">The client instance ID.</param>
        /// <param name="serverInstanceId">The server instance ID.</param>
        /// <returns>The session key.</returns>
        public static string CreateSessionKey(EtpVersion version, Guid sessionId, Guid clientInstanceId, Guid serverInstanceId)
        {
            switch (version)
            {
                case EtpVersion.v11: return sessionId.ToString();
                case EtpVersion.v12: return $"Session: {sessionId}; Client: {clientInstanceId}; Server: {serverInstanceId}";
                default:
                    {
                        var message = $"Unsupported ETP version: {version}.";
                        Logger.Debug(message);
                        throw new InvalidOperationException(message);
                    }
            }
        }

        /// <summary>
        /// Gets the name of a protocol message.
        /// </summary>
        /// <typeparam name="TProtocol">The protocol enum.</typeparam>
        /// <typeparam name="TMessageType">The message type enum.</typeparam>
        /// <param name="version">The ETP version.</param>
        /// <param name="protocol">The protocol.</param>
        /// <param name="messageType">The message type.</param>
        /// <returns>The message name.</returns>
        public static string GetMessageName<TProtocol, TMessageType>(EtpVersion version, TProtocol protocol, TMessageType messageType) where TProtocol : struct where TMessageType : struct, IConvertible
        {
            return GetMessageName(version, Convert.ToInt32(protocol), Convert.ToInt32(messageType));
        }

        /// <summary>
        /// Gets the name of a protocol message.
        /// </summary>
        /// <typeparam name="TMessageType">The message type enum.</typeparam>
        /// <param name="version">The ETP version.</param>
        /// <param name="protocol">The protocol.</param>
        /// <param name="messageType">The message type.</param>
        /// <returns>The message name.</returns>
        public static string GetMessageName<TMessageType>(EtpVersion version, int protocol, TMessageType messageType) where TMessageType : struct, IConvertible
        {
            return GetMessageName(version, protocol, Convert.ToInt32(messageType));
        }

        /// <summary>
        /// Gets the name of a protocol message.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <param name="protocol">The protocol.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <returns>The message name.</returns>
        public static string GetMessageName(EtpVersion version, int protocol, int messageType)
        {
            switch (version)
            {
                case EtpVersion.v11: return v11.MessageNames.GetMessageName(protocol, messageType);
                case EtpVersion.v12: return v12.MessageNames.GetMessageName(protocol, messageType);
                default:
                    {
                        var message = $"Unsupported ETP version: {version}.";
                        Logger.Debug(message);
                        throw new InvalidOperationException(message);
                    }
            }
        }

        /// <summary>
        /// Tries to get the protocol number for an ETP message.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <param name="messageBodyType">The message body type to get the protocol for.</param>
        /// <returns>The protocol number on success; -1 otherwise.</returns>
        public static int TryGetProtocolNumber(EtpVersion version, Type messageBodyType)
        {
            switch (version)
            {
                case EtpVersion.v11: return v11.MessageReflection.TryGetProtocolNumber(messageBodyType);
                case EtpVersion.v12: return v12.MessageReflection.TryGetProtocolNumber(messageBodyType);
                default:
                    {
                        var message = $"Unsupported ETP version: {version}.";
                        Logger.Debug(message);
                        throw new InvalidOperationException(message);
                    }
            }
        }

        /// <summary>
        /// Tries to get the message type number for an ETP message.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <param name="messageBodyType">The message body type to get the protocol for.</param>
        /// <returns>The message type number on success; -1 otherwise.</returns>
        public static int TryGetMessageTypeNumber(EtpVersion version, Type messageBodyType)
        {
            switch (version)
            {
                case EtpVersion.v11: return v11.MessageReflection.TryGetMessageTypeNumber(messageBodyType);
                case EtpVersion.v12: return v12.MessageReflection.TryGetMessageTypeNumber(messageBodyType);
                default:
                    {
                        var message = $"Unsupported ETP version: {version}.";
                        Logger.Debug(message);
                        throw new InvalidOperationException(message);
                    }
            }
        }

        /// <summary>
        /// Gets the name of a protocol.
        /// </summary>
        /// <typeparam name="TProtocol">The protocol enum.</typeparam>
        /// <param name="version">The ETP version.</param>
        /// <param name="protocol">The protocol.</param>
        /// <returns>The protocol name.</returns>
        public static string GetProtocolName<TProtocol>(EtpVersion version, TProtocol protocol) where TProtocol : struct
        {
            return GetProtocolName(version, Convert.ToInt32(protocol));
        }

        /// <summary>
        /// Gets the name of a protocol.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <param name="protocol">The protocol.</param>
        /// <returns>The protocol name.</returns>
        public static string GetProtocolName(EtpVersion version, int protocol)
        {
            switch (version)
            {
                case EtpVersion.v11: return v11.ProtocolNames.GetProtocolName(protocol);
                case EtpVersion.v12: return v12.ProtocolNames.GetProtocolName(protocol);
                default:
                    {
                        var message = $"Unsupported ETP version: {version}.";
                        Logger.Debug(message);
                        throw new InvalidOperationException(message);
                    }
            }
        }

#endregion
    }
}

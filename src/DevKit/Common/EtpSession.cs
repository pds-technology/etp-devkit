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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avro.IO;
using Avro.Specific;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Protocol.Core;
using Newtonsoft.Json.Linq;
using Nito.AsyncEx;


namespace Energistics.Etp.Common
{
    /// <summary>
    /// Provides common functionality for all ETP sessions.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.EtpBase" />
    /// <seealso cref="Energistics.Etp.Common.IEtpSession" />
    public abstract class EtpSession : EtpBase, IEtpSession
    {
        private int _socketOpenedEventCount = 0;
        private long _messageId;
        private readonly long _messageIdOffset;
        private readonly List<object> _contextObjects = new List<object>();
        protected const string TimestampFormat = "yyyy-MM-dd HH:mm:ss.ffff";

        // Used to ensure only one thread at a time sends data over a websocket.
        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);
        private readonly CancellationTokenSource _sendTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpSession"/> class.
        /// </summary>
        /// <param name="etpVersion">The ETP version for the session.</param>
        /// <param name="encoding">The ETP encoding for the session.</param>
        /// <param name="info">The endpoint's information.</param>
        /// <param name="parameters">The endpoint's parameters.</param>
        /// <param name="headers">The WebSocket or HTTP headers.</param>
        /// <param name="isClient">Whether or not this is the client-side of the session.</param>
        /// <param name="sessionId">The session ID if this is a server.</param>
        /// <param name="captureAsyncContext">Where or not the synchronization context should be captured for async tasks.</param>
        protected EtpSession(EtpVersion etpVersion, EtpEncoding encoding, EtpEndpointInfo info, EtpEndpointParameters parameters, IDictionary<string, string> headers, bool isClient, string sessionId, bool captureAsyncContext)
            : base(captureAsyncContext)
        {
            IsClient = isClient;
            Encoding = encoding;

            Headers = headers ?? new Dictionary<string, string>();
            HandlersByType = new ConcurrentDictionary<Type, IProtocolHandler>();
            HandlersByProtocol = new ConcurrentDictionary<int, IProtocolHandler>();

            Headers.SetEncoding(encoding);

            ValidateHeaders();

            Adapter = EtpFactory.CreateEtpAdapter(etpVersion);

            InstanceInfo = info;
            InstanceParameters = parameters?.CloneForVersion(EtpVersion) ?? new EtpEndpointParameters(EtpVersion);
            RegisterHandlerCore(Adapter.CreateDefaultCoreHandler(IsClient));
            foreach (var handler in InstanceParameters.SupportedProtocols)
                RegisterHandlerCore(handler);

            if (!IsClient)
            {
                _messageIdOffset = -1;
                Guid guid;
                if (!Guid.TryParse(sessionId, out guid))
                    throw new ArgumentException("Not a valid GUID.", nameof(sessionId));

                SessionId = guid;
            }

            UpdateSessionKey();
        }


        /// <summary>
        /// The cancellation token for sending on the websocket.
        /// </summary>
        private CancellationToken SendToken => _sendTokenSource.Token;

        /// <summary>
        /// Whether or not sending is enabled on the websocket.
        /// </summary>
        private bool IsSendingEnabled { get; set; } = true;

        /// <summary>
        /// Gets the ETP version supported by this session.
        /// </summary>
        public EtpVersion EtpVersion => Adapter.EtpVersion;

        /// <summary>
        /// Gets the encoding used by this session (binary or json).
        /// </summary>
        public EtpEncoding Encoding { get; }

        /// <summary>
        /// Gets the version specific ETP adapter.
        /// </summary>
        public IEtpAdapter Adapter { get; private set; }

        /// <summary>
        /// Gets whether or not this is the client-side of the session.
        /// </summary>
        public bool IsClient { get; private set; }

        /// <summary>
        /// Gets this instance's info.
        /// </summary>
        public EtpEndpointInfo InstanceInfo { get; }

        /// <summary>
        /// Gets the counterpart's info.
        /// </summary>
        public EtpEndpointInfo CounterpartInfo { get; private set; }

        /// <summary>
        /// Gets the client info.
        /// </summary>
        public EtpEndpointInfo ClientInfo => IsClient ? InstanceInfo : CounterpartInfo;

        /// <summary>
        /// Gets the server info.
        /// </summary>
        public EtpEndpointInfo ServerInfo => IsClient ? CounterpartInfo : InstanceInfo;

        /// <summary>
        /// Gets this instances parameters.
        /// </summary>
        private EtpEndpointParameters InstanceParameters { get; }

        /// <summary>
        /// Gets this instance's details.
        /// </summary>
        public IEndpointDetails InstanceDetails => InstanceParameters;

        /// <summary>
        /// Gets the counterpart's details.
        /// </summary>
        public IEndpointDetails CounterpartDetails { get; private set; }

        /// <summary>
        /// Gets the client details.
        /// </summary>
        public IEndpointDetails ClientDetails => IsClient ? InstanceDetails : CounterpartDetails;

        /// <summary>
        /// Gets the server details.
        /// </summary>
        public IEndpointDetails ServerDetails => IsClient ? CounterpartDetails : InstanceDetails;

        /// <summary>
        /// Gets or sets the session key.
        /// </summary>
        /// <value>The session key.</value>
        public string SessionKey { get; private set; }

        /// <summary>
        /// Gets the session identifier.
        /// </summary>
        /// <value>The session identifier.</value>
        public Guid SessionId { get; private set; }

        /// <summary>
        /// Whether or not the session should automatically acknowledge messages that can be immediately acknowledged and request acknowledgement.
        /// </summary>
        public bool AutoAcknowledgeMessages { get; set; } = true;

        /// <summary>
        /// Gets a value indicating whether the underlying websocket connection is open.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the underlying websocket is open; otherwise, <c>false</c>.
        /// </value>
        public abstract bool IsWebSocketOpen { get; }

        /// <summary>
        /// Whether or not the websocket has successfully been closed.
        /// </summary>
        private bool IsWebSocketClosed { get; set; } = false;

        /// <summary>
        /// Gets a value indicating whether the session is open.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the session is open; otherwise, <c>false</c>.
        /// </value>
        public bool IsSessionOpen { get; private set; }

        /// <summary>
        /// Gets the collection of WebSocket or HTTP headers.
        /// </summary>
        /// <value>The headers.</value>
        public IDictionary<string, string> Headers { get; }

        /// <summary>
        /// Maximum size allowed for a WebSocket frame (which is determined by the library you use to implement WebSocket). WebSocket is the transport protocol used by ETP.
        /// </summary>
        public long SessionMaxWebSocketFramePayloadSize { get; private set; }

        /// <summary>
        /// Maximum size allowed for a WebSocket message (which is composed of multiple WebSocket frames, which is determined by the library you use to implement WebSocket). WebSocket is the transport protocol used by ETP.
        /// </summary>
        public long SessionMaxWebSocketMessagePayloadSize { get; private set; }

        /// <summary>
        /// Gets or sets the negotiated list of supported protocols for this session.
        /// </summary>
        /// <value>The negotiated list of supported protocols for this session.</value>
        public IReadOnlyDictionary<int, ISessionProtocol> SessionSupportedProtocols { get; private set; }

        /// <summary>
        /// Gets or sets the negotiated compression type for this session.
        /// </summary>
        public string SessionSupportedCompression { get; private set; }

        /// <summary>
        /// Gets or sets the negotiated list of supported objects for this session.
        /// </summary>
        /// <value>The negotiated list of supported objects for this session.</value>
        public EtpSupportedDataObjectCollection SessionSupportedDataObjects { get; private set; }

        /// <summary>
        /// Gets or sets the negotiated list of supported objects for this session.
        /// </summary>
        /// <value>The negotiated list of supported objects for this session.</value>
        ISessionSupportedDataObjectCollection IEtpSession.SessionSupportedDataObjects => SessionSupportedDataObjects;

        /// <summary>
        /// Gets or sets the negotiated list of formats supported for this session.
        /// </summary>
        public IReadOnlyList<string> SessionSupportedFormats { get; private set; }

        /// <summary>
        /// Gets the collection of registered protocol handlers by Type.
        /// </summary>
        /// <value>The handlers.</value>
        private ConcurrentDictionary<Type, IProtocolHandler> HandlersByType { get; }

        /// <summary>
        /// Gets the collection of registered protocol handlers by protocol.
        /// </summary>
        /// <value>The handlers.</value>
        private ConcurrentDictionary<int, IProtocolHandler> HandlersByProtocol { get; }

        /// <summary>
        /// The request session message for the session.
        /// </summary>
        public EtpMessage<IRequestSession> RequestSessionMessage { get; private set; }

        /// <summary>
        /// The open session message for the session.
        /// </summary>
        public EtpMessage<IOpenSession> OpenSessionMessage { get; private set; }

        /// <summary>
        /// Returns whether the specified ETP version is supported.
        /// </summary>
        /// <param name="version">The specified ETP version.</param>
        /// <returns><c>true</c> if the version is supported; <c>false</c> otherwise.</returns>
        public bool IsEtpVersionSupported(EtpVersion version) => version == EtpVersion;

        /// <summary>
        /// Gets the registered handler for the specified protocol.
        /// </summary>
        /// <param name="protocol">The protocol.</param>
        /// <returns>The registered protocol handler instance.</returns>
        public IProtocolHandler Handler(int protocol)
        {
            return GetHandler(protocol);
        }

        /// <summary>
        /// Gets the registered protocol handler for the specified ETP interface.
        /// </summary>
        /// <typeparam name="T">The protocol handler interface.</typeparam>
        /// <returns>The registered protocol handler instance.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public T Handler<T>() where T : IProtocolHandler
        {
            IProtocolHandler handler;
            if (HandlersByType.TryGetValue(typeof(T), out handler) && handler is T)
                return (T)handler;

            Logger.Debug(Log($"[{SessionKey}] Protocol handler not registered for {typeof(T).FullName}."));
            throw new NotSupportedException($"Protocol handler not registered for {typeof(T).FullName}.");
        }

        /// <summary>
        /// Determines whether this instance can handle the specified protocol.
        /// </summary>
        /// <typeparam name="T">The protocol handler interface.</typeparam>
        /// <returns>
        ///   <c>true</c> if the specified protocol handler has been registered; otherwise, <c>false</c>.
        /// </returns>
        public bool CanHandle<T>() where T : IProtocolHandler
        {
            return HandlersByType.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Occurs when the WebSocket is opened.
        /// </summary>
        public event EventHandler SocketOpened;
        
        /// <summary>
        /// Raises the SocketOpened event.
        /// </summary>
        protected void RaiseSocketOpened()
        {
            var prevSocketOpenedEventCount = Interlocked.CompareExchange(ref _socketOpenedEventCount, 1, 0);

            if (prevSocketOpenedEventCount == 0)
            {
                Logger.Debug(Log($"[{SessionKey}] Socket opened."));

                SocketOpened?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Occurs when the WebSocket is closed.
        /// </summary>
        public event EventHandler SocketClosed;

        /// <summary>
        /// Raises the SocketClosed event.
        /// </summary>
        protected void RaiseSocketClosed()
        {
            var prevSocketOpenedEventCount = Interlocked.CompareExchange(ref _socketOpenedEventCount, 0, 1);

            if (prevSocketOpenedEventCount == 1)
            {
                if (IsSessionOpen)
                    RaiseSessionClosed(false, "Socket closed.");

                Logger.Debug(Log($"[{SessionKey}] Socket closed."));

                SocketClosed?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Occurs when the WebSocket has an error.
        /// </summary>
        public event EventHandler<ErrorEventArgs> SocketError;

        /// <summary>
        /// Raises the SocketError event.
        /// </summary>
        /// <param name="ex">The socket exception.</param>
        protected void RaiseSocketError(Exception ex)
        {
            var prevSocketOpenedEventCount = Interlocked.CompareExchange(ref _socketOpenedEventCount, 0, 1);

            if (prevSocketOpenedEventCount == 1)
                SocketError?.Invoke(this, new ErrorEventArgs(ex));
        }

        /// <summary>
        /// Sends the RequestSession message to establish a session.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<EtpMessage<IRequestSession>> RequestSessionAsync()
        {
            if (!IsClient)
                throw new InvalidOperationException("RequestSession can only be called from a client.");
            if (IsSessionOpen)
                throw new InvalidOperationException("Session is already open.");

            Logger.Trace($"[{SessionKey}] Sending Request Session.");

            var header = EtpFactory.CreateMessageHeader(EtpVersion, Protocols.Core, MessageTypes.Core.RequestSession);

            var body = EtpFactory.CreateRequestSession(EtpVersion, InstanceInfo, InstanceDetails);

            var message = new EtpMessage<IRequestSession>(header, body);

            RequestSessionMessage = message;

            return await SendMessageAsync(message, onBeforeSend: (m) => m.Body.CurrentDateTime = DateTime.UtcNow.ToEtpTimestamp()).ConfigureAwait(CaptureAsyncContext);
        }

        protected virtual void HandleRequestSession(EtpMessage<IRequestSession> message)
        {
            Logger.Verbose($"[{SessionKey}] Handling request session.");

            RequestSessionMessage = message;
            if (!message.Body.IsClientInstanceIdValid)
            {
                Logger.Debug($"[{SessionKey}] Invalid client instance ID: {message.Body.RawClientInstanceId}.  Closing session.");

                ProtocolException((int)Protocols.Core, ErrorInfo().InvalidArgument(nameof(message.Body.ClientInstanceId), message.Body.RawClientInstanceId), correlatedHeader: message.Header);
                RaiseSessionOpened(false);
                CloseSession();
                return;
            }

            var success = InitializeSession(message.ToEndpointInfo(), message.ToEndpointDetails());

            if (success && SessionSupportedProtocols.Count > 0)
            {
                OpenSession(message.Header);
            }
            else
            {
                if (SessionSupportedProtocols.Count == 0)
                {
                    Logger.Debug($"[{SessionKey}] No supported protocols.  Closing session.");
                    ProtocolException((int)Protocols.Core, ErrorInfo().NoSupportedProtocols(), message.Header);
                }
                else
                {
                    Logger.Debug($"[{SessionKey}] Session initialization failed.  Closing session.");
                    ProtocolException((int)Protocols.Core, ErrorInfo().InvalidState(), message.Header);
                }

                RaiseSessionOpened(false);
                CloseSession();
            }
        }

        protected virtual EtpMessage<IOpenSession> OpenSession(IMessageHeader correlatedHeader)
        {
            if (IsClient)
                throw new InvalidOperationException("RequestSession can only be called from a server.");
            if (IsSessionOpen)
                throw new InvalidOperationException("Session is already open.");

            Logger.Trace($"[{SessionKey}] Sending Open Session.");

            var header = EtpFactory.CreateMessageHeader(EtpVersion, Protocols.Core, MessageTypes.Core.OpenSession);

            var sessionDetails = new EtpEndpointDetails(
                InstanceDetails.Capabilities,
                SessionSupportedProtocols.Values.ToList(),
                InstanceDetails.SupportedDataObjects,
                new List<string> { SessionSupportedCompression },
                SessionSupportedFormats
            );

            var body = EtpFactory.CreateOpenSession(EtpVersion, InstanceInfo, sessionDetails, SessionId);

            var message = new EtpMessage<IOpenSession>(header, body);

            OpenSessionMessage = message;

            message = SendMessage(message, onBeforeSend: (m) => m.Body.CurrentDateTime = DateTime.UtcNow.ToEtpTimestamp());

            IsSessionOpen = message != null;

            if (IsSessionOpen)
                StartHandlers();

            RaiseSessionOpened(IsSessionOpen);

            return message;
        }

        protected virtual void HandleOpenSession(EtpMessage<IOpenSession> message)
        {
            Logger.Verbose($"[{SessionKey}] Handling open session.");

            if (!message.Body.IsSessionIdValid || !message.Body.IsServerInstanceIdValid)
            {
                if (!message.Body.IsSessionIdValid)
                {
                    Logger.Debug($"[{SessionKey}] Invalid session ID: {message.Body.RawSessionId}.  Closing session.");
                    ProtocolException((int)Protocols.Core, ErrorInfo().InvalidArgument(nameof(message.Body.SessionId), message.Body.RawSessionId), correlatedHeader: message.Header);
                }
                else
                {
                    Logger.Debug($"[{SessionKey}] Invalid server instance ID: {message.Body.RawServerInstanceId}.  Closing session.");
                    ProtocolException((int)Protocols.Core, ErrorInfo().InvalidArgument(nameof(message.Body.SessionId), message.Body.RawServerInstanceId), correlatedHeader: message.Header);
                }

                RaiseSessionOpened(false);
                CloseSession();
                return;
            }

            SessionId = message.Body.SessionId;
            var success = InitializeSession(message.ToEndpointInfo(), message.ToEndpointDetails());

            if (success)
            {
                IsSessionOpen = true;
                StartHandlers();
                RaiseSessionOpened(true);
            }
            else
            {
                Logger.Debug($"[{SessionKey}] Session initialization failed.  Closing session.");

                ProtocolException((int)Protocols.Core, ErrorInfo().InvalidState(), message.Header);

                RaiseSessionOpened(false);
                CloseSession();
            }
        }

        /// <summary>
        /// Starts the protocol handlers.
        /// </summary>
        private void StartHandlers()
        {
            foreach (var protocol in SessionSupportedProtocols.Keys)
            {
                HandlersByProtocol[protocol].StartHandling();
            }
        }

        /// <summary>
        /// Called when the ETP session is opened.
        /// </summary>
        /// <param name="openedSuccessfully"><c>true</c> if the session opened without errors; <c>false</c> if there were errors when opening the session.</param>
        protected void RaiseSessionOpened(bool openedSuccessfully)
        {
            Logger.Trace($"[{SessionKey}] Session opened; Success: {openedSuccessfully}");

            SessionOpened?.Invoke(this, new SessionOpenedEventArgs(openedSuccessfully));
        }

        /// <summary>
        /// Event raised when the session is opened.
        /// </summary>
        public event EventHandler<SessionOpenedEventArgs> SessionOpened;

        /// <summary>
        /// Initialize the session based on details from the counterpart.
        /// After this, the protocols, objects, compression, formats and capabilities that will be used in the session will be initialized.
        /// </summary>
        /// <param name="counterpartInfo">The counterpart's info.</param>
        /// <param name="counterpartDetails">The counterpart's details.</param>
        /// <returns><c>true</c> if the session was successfully initialized; <c>false</c> otherwise.</returns>
        protected virtual bool InitializeSession(EtpEndpointInfo counterpartInfo, EtpEndpointDetails counterpartDetails)
        {
            var success = true;

            CounterpartInfo = counterpartInfo;
            CounterpartDetails = counterpartDetails;

            UpdateSessionKey();

            if (!InitializeSessionSupportedProtocols())
                success = false;

            if (!InitializeSessionSupportedDataObjects())
                success = false;

            if (!InitializeSessionSupportedCompression())
                success = false;

            if (!InitializeSessionSupportedFormats())
                success = false;

            if (!InitializeSessionCapabilities())
                success = false;

            return success;
        }

        /// <summary>
        /// Initializes the protocols for use in this session.
        /// </summary>
        /// <returns><c>true</c> if initialization was successful; <c>false</c> otherwise.</returns>
        protected virtual bool InitializeSessionSupportedProtocols()
        {
            var success = true;

            var handlers = new List<IProtocolHandler>();
            var sessionSupportedProtocols = new Dictionary<int, ISessionProtocol>();

            foreach (var protocol in InstanceParameters.SupportedProtocols.OrderBy(p => p.Protocol))
            {
                var counterpartProtocol = CounterpartDetails.SupportedProtocols.FirstOrDefault(p => p.Protocol == protocol.Protocol && p.EtpVersion == protocol.EtpVersion && p.Role.Equals(protocol.CounterpartRole, StringComparison.OrdinalIgnoreCase) && p.CounterpartRole.Equals(protocol.Role, StringComparison.OrdinalIgnoreCase));
                if (counterpartProtocol == null)
                {
                    if (IsClient)
                    {
                        Logger.Debug($"[{SessionKey}] Requested protocol not supported by server. Protocol: {protocol.Protocol}; Role: {protocol.Role}; Version: {protocol.EtpVersion.ToVersionString()}");
                    }
                    else
                    {
                        Logger.Verbose($"[{SessionKey}] Supported protocol was not requested by client. Protocol: {protocol.Protocol}; Role: {protocol.Role}; Version: {protocol.EtpVersion.ToVersionString()}");
                    }

                    continue;
                }

                handlers.Add(protocol);
                protocol.SetCounterpartCapabilities(counterpartProtocol.Capabilities);
                sessionSupportedProtocols[protocol.Protocol] = protocol;
            }

            foreach (var counterpartProtocol in CounterpartDetails.SupportedProtocols.OrderBy(p => p.Protocol))
            {
                var supportedProtocol = handlers.FirstOrDefault(p => p.Protocol == counterpartProtocol.Protocol && p.EtpVersion == counterpartProtocol.EtpVersion && p.Role.Equals(counterpartProtocol.CounterpartRole, StringComparison.OrdinalIgnoreCase) && p.CounterpartRole.Equals(counterpartProtocol.Role, StringComparison.OrdinalIgnoreCase));
                if (supportedProtocol == null)
                {
                    if (IsClient)
                        Logger.Debug($"[{SessionKey}] Ignoring unrequested protocol. Protocol: {counterpartProtocol.Protocol}; Role: {counterpartProtocol.Role}");
                    else
                        Logger.Debug($"[{SessionKey}] Requested protocol not supported. Protocol: {counterpartProtocol.Protocol}; Role: {counterpartProtocol.Role}");
                }
            }

            if (HandlersByProtocol.Count == 0)
            {
                Logger.Debug(Log($"[{SessionKey}] No handlers registered were registered for any protocol."));
            }
            else if (!HandlersByProtocol.ContainsKey((int)Protocols.Core))
            {
                Logger.Debug(Log($"[{SessionKey}] No handler registered for the Core protocol."));
            }
            else if (success)
            {
                sessionSupportedProtocols[(int)Protocols.Core] = HandlersByProtocol[(int)Protocols.Core];
            }
            else
            {
                sessionSupportedProtocols.Clear();
            }

            SessionSupportedProtocols = new ReadOnlyDictionary<int, ISessionProtocol>(sessionSupportedProtocols);

            return success;
        }

        /// <summary>
        /// Initializes the supported objects for this session.
        /// </summary>
        /// <returns><c>true</c> if initialization was successful; <c>false</c> otherwise.</returns>
        protected virtual bool InitializeSessionSupportedDataObjects()
        {
            var success = true;

            var supportedDataObjectList = new List<EtpSupportedDataObject>();
            foreach (var supportedDataObject in InstanceDetails.SupportedDataObjects)
            {
                if (!supportedDataObject.QualifiedType.IsValid || supportedDataObject.QualifiedType.IsBaseType)
                {
                    Logger.Debug($"[{SessionKey}] Invalid object type supported by this instance. Object Type: {supportedDataObject.QualifiedType.ToVersionKey(EtpVersion)}");

                    continue;
                }

                supportedDataObjectList.Add(new EtpSupportedDataObject(supportedDataObject.QualifiedType, new List<string>(supportedDataObject.Capabilities)));
            }

            var counterpartSupportedDataObjectList = new List<EtpSupportedDataObject>();
            foreach (var counterpartSupportedDataObject in CounterpartDetails.SupportedDataObjects)
            {
                if (!counterpartSupportedDataObject.QualifiedType.IsValid || counterpartSupportedDataObject.QualifiedType.IsBaseType)
                {
                    Logger.Debug($"[{SessionKey}] Invalid object type supported by counterpart. Object Type: {counterpartSupportedDataObject.QualifiedType.ToVersionKey(EtpVersion)}");

                    continue;
                }

                counterpartSupportedDataObjectList.Add(new EtpSupportedDataObject(counterpartSupportedDataObject.QualifiedType, new List<string>(counterpartSupportedDataObject.Capabilities)));
            }

            SessionSupportedDataObjects = EtpSupportedDataObjectCollection.GetSupportedDataObjectCollection(supportedDataObjectList, counterpartSupportedDataObjectList, Adapter.AreSupportedDataObjectsNegotiated);
            if (SessionSupportedDataObjects.Count == 0)
            {
                Logger.Debug($"[{SessionKey}] No mutually supported data objects.");
            }

            return success;
        }

        /// <summary>
        /// Initializes the compression for use in this session.
        /// </summary>
        /// <returns><c>true</c> if initialization was successful; <c>false</c> otherwise.</returns>
        protected virtual bool InitializeSessionSupportedCompression()
        {
            string supportedCompression = string.Empty;
            foreach (var compression in InstanceDetails.SupportedCompression)
            {
                if (!EtpCompression.IsSupportedCompressionMethod(compression))
                {
                    Logger.Debug($"[{SessionKey}] Unexpected compression supported by this instance: {compression}");
                }

                var counterpartCompression = CounterpartDetails.SupportedCompression.FirstOrDefault(c => (string.IsNullOrEmpty(c) && string.IsNullOrEmpty(compression)) || compression.Equals(c, StringComparison.OrdinalIgnoreCase));
                if (counterpartCompression != null)
                {
                    supportedCompression = counterpartCompression;
                    break;
                }
                else
                {
                    Logger.Trace($"[{SessionKey}] Ignoring compression not supported by counterpart: {compression}");
                }
            }

            // TODO: Log unsupported compression received from counterpart.

            SessionSupportedCompression = supportedCompression;
            return true;
        }

        /// <summary>
        /// Initializes the formats for use in this session.
        /// </summary>
        /// <returns><c>true</c> if initialization was successful; <c>false</c> otherwise.</returns>
        protected virtual bool InitializeSessionSupportedFormats()
        {
            var supportedFormats = new List<string>();
            foreach (var format in InstanceDetails.SupportedFormats)
            {
                if (!format.Equals(Formats.Xml, StringComparison.OrdinalIgnoreCase))
                {
                    Logger.Debug($"[{SessionKey}] Unexpected format supported by this instance: {format}");
                }

                var counterpartFormat = CounterpartDetails.SupportedFormats.FirstOrDefault(f => format.Equals(f, StringComparison.OrdinalIgnoreCase));
                if (counterpartFormat != null)
                    supportedFormats.Add(counterpartFormat);
                else
                    Logger.Trace($"[{SessionKey}] Ignoring format not supported by counterpart: {format}");
            }

            // TODO: Log unsupported formats received from counterpart.

            SessionSupportedFormats = supportedFormats;
            return true;
        }


        /// <summary>
        /// Initializes the capabilities for use in this session where special handling is needed to account for both the instance and the counterpart's capability values..
        /// </summary>
        /// <returns><c>true</c> if initialization was successful; <c>false</c> otherwise.</returns>
        protected virtual bool InitializeSessionCapabilities()
        {
            SessionMaxWebSocketFramePayloadSize = Math.Min(CounterpartDetails.Capabilities.MaxWebSocketFramePayloadSize ?? long.MaxValue, InstanceDetails.Capabilities.MaxWebSocketFramePayloadSize ?? long.MaxValue);
            SessionMaxWebSocketMessagePayloadSize = Math.Min(CounterpartDetails.Capabilities.MaxWebSocketMessagePayloadSize ?? long.MaxValue, InstanceDetails.Capabilities.MaxWebSocketMessagePayloadSize ?? long.MaxValue);

            return true;
        }

        /// <summary>
        /// Sends an Acknowledge message in response to the message associated with the correlation header.
        /// </summary>
        /// <param name="protocol">The protocol to send the acknowledge message on.</param>
        /// <param name="correlatedHeader">The message header the acknowledge message is correlated with.</param>
        /// <param name="isNoData">Whether or not the acknowledge message should have the NoData flag set.</param>
        /// <param name="extension">The message header extension to send with the message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<IAcknowledge> Acknowledge(int protocol, IMessageHeader correlatedHeader, bool isNoData = false, IMessageHeaderExtension extension = null)
        {
            var header = EtpFactory.CreateMessageHeader(EtpVersion, protocol, MessageTypes.Core.Acknowledge);
            if (isNoData)
                header.SetNoData();

            var body = EtpFactory.CreateAcknowledge(EtpVersion);

            header.PrepareHeader(correlatedHeader, false, false);
            var message = new EtpMessage<IAcknowledge>(header, body, extension: extension);

            return SendMessage(message);
        }

        /// <summary>
        /// Constructs a new <see cref="IErrorInfo"/> instance compatible with the session.
        /// </summary>
        /// <returns>The constructed error info.</returns>
        public IErrorInfo ErrorInfo() => EtpFactory.CreateErrorInfo(EtpVersion);

        /// <summary>
        /// Sends a ProtocolException message.
        /// </summary>
        /// <param name="protocol">The protocol to send the acknowledge message on.</param>
        /// <param name="error">The error in the protocol exception.</param>
        /// <param name="correlatedHeader">The message header the protocol exception is correlated with, if any.</param>
        /// <param name="isFinalPart">Whether or not the protocol exception is the final part in a multi-part message.</param>
        /// <param name="extension">The message header extension to send with the message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<IProtocolException> ProtocolException(int protocol, IErrorInfo error, IMessageHeader correlatedHeader = null, bool isFinalPart = false, IMessageHeaderExtension extension = null)
        {
            var exception = EtpFactory.CreateProtocolException(EtpVersion, error);
            return ProtocolException(protocol, exception, correlatedHeader: correlatedHeader, isFinalPart: isFinalPart, extension: extension);
        }

        /// <summary>
        /// Sends a ProtocolException message with the specified exception details.
        /// </summary>
        /// <param name="exception">The ETP exception.</param>
        /// <param name="isFinalPart">Whether or not the protocol exception is the final part in a multi-part message.</param>
        /// <param name="extension">The message header extension to send with the message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<IProtocolException> ProtocolException(EtpException exception, bool isFinalPart = false, IMessageHeaderExtension extension = null)
        {
            if (exception.InnerException != null)
                Logger.LogErrorInfo(exception.ErrorInfo);

            return ProtocolException(exception.Protocol, exception.ErrorInfo, correlatedHeader: exception.CorrelatedHeader, isFinalPart: isFinalPart, extension: extension);
        }

        /// <summary>
        /// Sends a ProtocolException message(s) with the specified exception details.
        /// </summary>
        /// <param name="protocol">The protocol to send the protocol exception on.</param>
        /// <param name="errors">The errors in the protocol exception.</param>
        /// <param name="correlatedHeader">The message header the protocol exception is correlated with, if any.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="extension">The message header extension to send with the message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<IProtocolException> ProtocolException(int protocol, IDictionary<string, IErrorInfo> errors, IMessageHeader correlatedHeader = null, bool setFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var exceptions = EtpFactory.CreateProtocolExceptions(EtpVersion, errors);

            EtpMessage<IProtocolException> message = null;

            for (int i = 0; i < exceptions.Count; i++)
            {
                var ret = ProtocolException(protocol, exceptions[i], correlatedHeader: correlatedHeader, isFinalPart: (i == exceptions.Count - 1 && setFinalPart), extension: extension);
                if (ret == null)
                    return null;
                message = message ?? ret;
            }

            return message;
        }

        /// <summary>
        /// Sends a ProtocolException message with the specified exception details.
        /// </summary>
        /// <param name="protocol">The protocol to send the protocol exception on.</param>
        /// <param name="exception">The protocol exception body to send.</param>
        /// <param name="correlatedHeader">The message header the protocol exception is correlated with, if any.</param>
        /// <param name="isFinalPart">Whether or not the protocol exception is the final part in a multi-part message.</param>
        /// <param name="extension">The message header extension to send with the message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public EtpMessage<IProtocolException> ProtocolException(int protocol, IProtocolException exception, IMessageHeader correlatedHeader = null, bool isFinalPart = false, IMessageHeaderExtension extension = null)
        {
            var header = EtpFactory.CreateMessageHeader(EtpVersion, protocol, MessageTypes.Core.ProtocolException);

            var body = exception;

            header.PrepareHeader(correlatedHeader, Adapter.IsProtocolExceptionMultiPart, isFinalPart);
            var message = new EtpMessage<IProtocolException>(header, body, extension: extension);

            return SendMessage(message);
        }


        /// <summary>
        /// Occurs when a ProtocolException message is received for any protocol.
        /// </summary>
        public event EventHandler<MessageEventArgs<IProtocolException>> OnProtocolException;

        /// <summary>
        /// Sends a CloseSession message to the session's counterpart.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ICloseSession> CloseSession(string reason = null, IMessageHeaderExtension extension = null)
        {
            Logger.Trace($"[{SessionKey}] Closing Session: {reason}");

            var header = EtpFactory.CreateMessageHeader(EtpVersion, Protocols.Core, MessageTypes.Core.CloseSession);

            var body = EtpFactory.CreateCloseSession(EtpVersion, reason);

            header.PrepareHeader(null, false, false);
            var message = new EtpMessage<ICloseSession>(header, body, extension: extension);

            message = SendMessage(message);

            RaiseSessionClosed(message != null, reason);

            CloseWebSocket(reason);

            return message;
        }

        /// <summary>
        /// Handles the CloseSession message.
        /// </summary>
        /// <param name="message">The close session message.</param>
        protected virtual void HandleCloseSession(EtpMessage<ICloseSession> message)
        {
            RaiseSessionClosed(true, message.Body.Reason);

            CloseWebSocket(message.Body.Reason);
        }

        /// <summary>
        /// Called when the ETP session is closed.
        /// </summary>
        /// <param name="closedSuccessfully"><c>true</c> if the session closed without errors; <c>false</c> if there were errors when closing the session.</param>
        /// <param name="reason">The reason provided when the session closed.</param>
        protected void RaiseSessionClosed(bool closedSuccessfully, string reason)
        {
            Logger.Trace($"[{SessionKey}] Session closed; Success: {closedSuccessfully}; Reason: {reason}");

            IsSessionOpen = false;

            var handlers = HandlersByProtocol.Values.ToList();

            // notify protocol handlers about new session
            foreach (var handler in handlers)
            {
                handler.StopHandling();
            }

            SessionClosed?.Invoke(this, new SessionClosedEventArgs(closedSuccessfully, reason));
        }

        /// <summary>
        /// Event raised when the session is closed.
        /// </summary>
        public event EventHandler<SessionClosedEventArgs> SessionClosed;

        /// <summary>
        /// Event raised when binary WebSocket data is received.
        /// </summary>
        public event EventHandler<DataReceivedEventArgs> DataReceived;

        /// <summary>
        /// Event raised when text WebSocket data is received.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// Synchronously sends the message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The message.</param>
        /// <param name="onBeforeSend">Action called just before sending the message with the actual header having the definitive message ID.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public EtpMessage<T> SendMessage<T>(EtpMessage<T> message, Action<EtpMessage<T>> onBeforeSend = null)
            where T : ISpecificRecord
        {
            return AsyncContext.Run(() => SendMessageAsync(message, onBeforeSend));
        }

        /// <summary>
        /// Asynchronously sends the message.
        /// </summary>
        /// <typeparam name="T">The type of the message body.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="onBeforeSend">Action called just before sending the message with the actual header having the definitive message ID.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public async Task<EtpMessage<T>> SendMessageAsync<T>(EtpMessage<T> message, Action<EtpMessage<T>> onBeforeSend = null) where T : ISpecificRecord
        {
            if (!IsSendingEnabled)
            {
                Logger.Trace($"[{SessionKey}] Sending disabled.");
                return null;
            }

            // Lock to ensure only one thread at a time attempts to send data and to ensure that messages are sent with sequential IDs
            try
            {
                var acquired = false;
                try
                {
                    try
                    {
                        Logger.Verbose($"[{SessionKey}] Acquiring send lock in SendMessageAsync");
                        await _sendLock.WaitAsync(SendToken).ConfigureAwait(CaptureAsyncContext);
                        acquired = true;
                    }
                    catch (OperationCanceledException)
                    {
                        Logger.Trace($"[{SessionKey}] Sending canceled.");
                        return null;
                    }
                    if (SendToken.IsCancellationRequested)
                    {
                        Logger.Trace($"[{SessionKey}] Sending canceled.");
                        return null;
                    }

                    if (!IsWebSocketOpen)
                    {
                        Log($"[{SessionKey}] Warning: Sending on a closed websocket.");
                        Logger.Debug($"[{SessionKey}] Sending on a closed websocket.");
                        return null;
                    }
                    if (!IsSessionOpen && !message.Header.IsAllowedBeforeSessionIsOpen())
                    {
                        Log($"[{SessionKey}] Sending an unsupported message before the session is open: {message.MessageName}.");
                        Logger.Debug($"[{SessionKey}] Sending an unsupported message before the session is open: {message.MessageName}.");
                    }

                    message.Header.MessageId = NewMessageId();
                    message.Header.Timestamp = DateTime.UtcNow;
                    if (message.Extension != null)
                        message.Header.SetHasHeaderExtension();

                    // Call the pre-send action in case any deterministic handling is needed with the actual message ID.
                    // Must be invoked before sending to ensure the response is not asynchronously processed before this method returns.
                    onBeforeSend?.Invoke(message);

                    // Log message just before it gets sent if needed.
                    LogMessage(message, true);

                    var includeExtension = CounterpartDetails?.Capabilities?.SupportsMessageHeaderExtension ?? false;
                    var bytes = Encoding == EtpEncoding.Json
                        ? message.Serialize(includeExtension: includeExtension)
                        : message.Encode(includeExtension: includeExtension, compression: SessionSupportedCompression);

                    var data = new ArraySegment<byte>(bytes);
                    if (!await SendAsync(data).ConfigureAwait(CaptureAsyncContext))
                        return null;
                }
                finally
                {
                    if (acquired)
                    {
                        Logger.Verbose($"[{SessionKey}] Releasing send lock in SendMessageAsync");
                        _sendLock.Release();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex);

                // Send protocol exception unless the message being sent was already a protocol exception.
                if (message.Header.MessageType != (int)MessageTypes.Core.ProtocolException)
                    ProtocolException(message.Header.Protocol, ErrorInfo().InvalidState(), message.Header);

                return null;
            }

            return message;
        }

        /// <summary>
        /// Generates a new unique message identifier for the current session.
        /// </summary>
        /// <returns>The new message identifier.  For servers, the message identifiers
        /// are non-zero odd numbers.  For clients, the message identifiers are non-zero
        /// even numbers.</returns>
        public long NewMessageId()
        {
            return Interlocked.Increment(ref _messageId) * 2 + _messageIdOffset;
        }

        /// <summary>
        /// Closes the WebSocket connection for the specified reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        public void CloseWebSocket(string reason)
        {
            AsyncContext.Run(() => CloseWebSocketAsync(reason));
        }

        /// <summary>
        /// Asynchronously closes the WebSocket connection for the specified reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        public async Task CloseWebSocketAsync(string reason)
        {
            var acquired = false;

            // Closing sends messages over the websocket so need to ensure no other messages are being sent when closing
            try
            {
                if (IsWebSocketClosed)
                {
                    Logger.Verbose($"[{SessionKey}] Websocket already closed.");
                    return;
                }

                Logger.Verbose($"[{SessionKey}] Acquiring send lock in CloseWebSocketAsync");

                try
                {
                    await _sendLock.WaitAsync(SendToken).ConfigureAwait(CaptureAsyncContext);
                    acquired = true;
                }
                catch (OperationCanceledException)
                {
                    Logger.Verbose($"[{SessionKey}] Acquiring send lock was canceled in CloseWebSocketAsync");
                    return;
                }

                Logger.Trace($"[{SessionKey}] Closing WebSocket: {reason}");

                _sendTokenSource.Cancel();
                IsSendingEnabled = false;
                IsWebSocketClosed = true;

                await CloseWebSocketAsyncCore(reason).ConfigureAwait(CaptureAsyncContext);
            }
            finally
            {
                if (acquired)
                {
                    Logger.Verbose($"[{SessionKey}] Releasing send lock in CloseWebSocketAsync");
                    _sendLock.Release();
                }
            }
        }

        /// <summary>
        /// Sets the context object of type <typeparamref name="T"/> for this session.
        /// </summary>
        /// <typeparam name="T">The type of the context object.</typeparam>
        /// <param name="context">The context object.</param>
        public void SetContext<T>(T context)
        {
            for (int i = 0; i < _contextObjects.Count; i++)
            {
                if (_contextObjects[i] is T)
                {
                    _contextObjects[i] = context;
                    return;
                }
            }

            _contextObjects.Add(context);
        }

        /// <summary>
        /// Getsets the context object of type <typeparamref name="T"/> for this session.
        /// </summary>
        /// <typeparam name="T">The type of the context object.</typeparam>
        /// <returns>The context object.</returns>
        public T GetContext<T>()
        {
            return (T)_contextObjects.FirstOrDefault(o => o is T);
        }

        /// <summary>
        /// Asynchronously closes the WebSocket connection for the specified reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        protected abstract Task CloseWebSocketAsyncCore(string reason);

        /// <summary>
        /// Sends the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        protected abstract Task<bool> SendAsync(ArraySegment<byte> data);

        /// <summary>
        /// Decodes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        protected void Decode(ArraySegment<byte> data)
        {
            if (Encoding == EtpEncoding.Binary)
            {
                DecodeBinary(data);
            }
            else
            {
                var message = System.Text.Encoding.UTF8.GetString(data.Array, data.Offset, data.Count);
                DecodeJson(message);
            }
        }

        /// <summary>
        /// Decodes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        protected void DecodeBinary(ArraySegment<byte> data)
        {
            DataReceived?.Invoke(this, new DataReceivedEventArgs(data));

            using (var inputStream = new MemoryStream(data.Array, data.Offset, data.Count, false))
            {
                // create avro binary decoder to read from memory stream
                var decoder = new BinaryDecoder(inputStream);
                // deserialize the header
                var header = Adapter.DecodeMessageHeader(decoder);
                header.Timestamp = DateTime.UtcNow;

                // log message metadata
                if (Logger.IsVerboseEnabled())
                {
                    Logger.Verbose($"[{SessionKey}] Binary message received: Name: {header.ToMessageName()}; Header: {EtpExtensions.Serialize(header)}");
                }

                using (var decompressionStream = header.IsCompressed() ? EtpCompression.TryGetDecompresionStream(SessionSupportedCompression, inputStream) : null)
                {
                    // decompress message body if compression has been negotiated
                    if (header.IsCompressed())
                    {
                        if (header.CanBeCompressed())
                        {
                            decoder = new BinaryDecoder(decompressionStream);
                        }
                        else
                        {
                            ProtocolException(header.Protocol, ErrorInfo().CompressionNotSupported(), header);
                            return;
                        }
                    }

                    // Header Extension and Body are compressed
                    var extension = header.CanHaveHeaderExtension() && header.HasHeaderExtension()
                        ? Adapter.DecodeMessageHeaderExtension(decoder)
                        : null;

                    // call processing action
                    HandleMessage(header, extension, decoder, null);
                }
            }
        }

        /// <summary>
        /// Decodes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected void DecodeJson(string message)
        {
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));

            // split message header and body
            var array = JArray.Parse(message);
            var headerJson = array[0].ToString();
            var header = Adapter.DeserializeMessageHeader(headerJson);
            header.Timestamp = DateTime.UtcNow;

            // log message metadata
            if (Logger.IsVerboseEnabled())
            {
                Logger.Verbose($"[{SessionKey}] JSON message received: Name: {header.ToMessageName()}; Header: {headerJson}");
            }

            var headerExtensionJson = header.HasHeaderExtension()
                ? array[1].ToString()
                : null;
            var body = headerExtensionJson == null
                ? array[1].ToString()
                : array[2].ToString();

            var extension = headerExtensionJson == null
                ? null
                : Adapter.DeserializeMessageHeaderExtension(headerExtensionJson);

            // call processing action
            HandleMessage(header, extension, null, body);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="extension">The header extension.</param>
        /// <param name="decoder">The decoder.</param>
        /// <param name="content">The body.</param>
        protected void HandleMessage(IMessageHeader header, IMessageHeaderExtension extension, Decoder decoder, string content)
        {
            if (Logger.IsVerboseEnabled())
                Logger.Verbose($"[{SessionKey}] Handling message: Name: {header.ToMessageName()}; Header: {EtpExtensions.Serialize(header)}");

            IProtocolHandler handler;
            var isSessionManagementMessage = header.IsRequestSession() || header.IsOpenSession() || header.IsCloseSession();

            if (!HandlersByProtocol.TryGetValue(header.Protocol, out handler) && !isSessionManagementMessage)
            {
                // Protocol handlers are cleared when the session is disposed or the socket is closed, but this method can still be called during or after that.
                if (HandlersByProtocol.Count == 0)
                    Logger.Trace($"[{SessionKey}] Ignoring message on closed session: Name: {header.ToMessageName()}; Header: {EtpExtensions.Serialize(header)}");
                else
                {
                    Logger.Debug($"[{SessionKey}] Unsupported protocol: Name: {header.ToMessageName()}; Header: {EtpExtensions.Serialize(header)}");
                    ProtocolException(header.Protocol, ErrorInfo().UnsupportedProtocol(header.Protocol), header);
                }

                return;
            }

            if (!Adapter.IsValidMessageType(header.Protocol, header.MessageType))
            {
                Logger.Debug($"[{SessionKey}] Invalid message type: Name: {header.ToMessageName()}; Header: {EtpExtensions.Serialize(header)}");
                ProtocolException(header.Protocol, ErrorInfo().InvalidMessageType(header.Protocol, header.MessageType), header);
                return;
            }

            var message = decoder != null
                ? Adapter.DecodeMessage(header, extension, decoder)
                : Adapter.DeserializeMessage(header, extension, content);

            if (message == null)
            {
                Logger.Debug($"[{SessionKey}] Invalid message: Name: {header.ToMessageName()}; Header: {EtpExtensions.Serialize(header)}");
                ProtocolException(header.Protocol, ErrorInfo().InvalidMessage(header.Protocol, header.MessageType), header);
                return;
            }

            LogMessage(message, false);

            try
            {
                // Handle global Acknowledge request
                if (AutoAcknowledgeMessages && header.IsAcknowledgeRequested() && header.CanAutoAcknowledge(EtpVersion))
                {
                    if (isSessionManagementMessage)
                        Acknowledge((int)Protocols.Core, header);
                    else
                        handler.Acknowledge(header);
                }

                if (isSessionManagementMessage)
                {
                    if (header.IsRequestSession())
                        HandleRequestSession(message as EtpMessage<IRequestSession>);
                    else if (header.IsOpenSession())
                        HandleOpenSession(message as EtpMessage<IOpenSession>);
                    else if (header.IsCloseSession())
                        HandleCloseSession(message as EtpMessage<ICloseSession>);
                }
                else if (header.IsProtocolException())
                {
                    OnProtocolException?.Invoke(this, new MessageEventArgs<IProtocolException>(message as EtpMessage<IProtocolException>));
                    handler.HandleMessage(message);
                }
                else if (IsSessionOpen || header.IsAllowedBeforeSessionIsOpen())
                    handler.HandleMessage(message);
                else
                {
                    Logger.Debug($"[{SessionKey}] Unexpected message before session is open: Name: {header.ToMessageName()}; Header: {EtpExtensions.Serialize(header)}");
                    ProtocolException(header.Protocol, ErrorInfo().InvalidState(), header);
                }
            }
            catch (Exception ex)
            {
                Logger.Debug($"[{SessionKey}] Exception while handling message: {ex}");
                ProtocolException(header.Protocol, ErrorInfo().InvalidState(), header);
            }
        }

        /// <summary>
        /// Registers a protocol handler.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        public void Register(IProtocolHandler handler)
        {
            Register(new List<IProtocolHandler> { handler });
        }

        /// <summary>
        /// Registers protocol handlers.
        /// </summary>
        /// <param name="handlers">The protocol handlers.</param>
        public void Register(IEnumerable<IProtocolHandler> handlers)
        {
            if (IsSessionOpen)
                throw new InvalidOperationException("Session Already Open");

            foreach (var handler in handlers)
            {
                InstanceParameters.SupportedProtocols.Add(handler);
                RegisterHandlerCore(handler);
            }
        }

        /// <summary>
        /// Does the internal the protocol handler registration.
        /// </summary>
        /// <param name="handler">The handlers by type.</param>
        private void RegisterHandlerCore(IProtocolHandler handler)
        {
            if (HandlersByProtocol.ContainsKey(handler.Protocol))
                Logger.Debug(Log($"[{SessionKey}] Replacing existing handler registered for Protocol {handler.Protocol}."));

            HandlersByProtocol[handler.Protocol] = handler;

            foreach (var @interface in handler.GetType().GetInterfaces().Where(t => typeof(IProtocolHandler).IsAssignableFrom(t) && t != typeof(IProtocolHandler)))
                HandlersByType[@interface] = handler;

            handler.SetSession(this);
        }

        /// <summary>
        /// Registers a supported data object.
        /// </summary>
        /// <param name="supportedDataObject">The supported data object.</param>
        public void Register(IEndpointSupportedDataObject supportedDataObject)
        {
            Register(new List<IEndpointSupportedDataObject> { supportedDataObject });
        }

        /// <summary>
        /// Registers supported data objects.
        /// </summary>
        /// <param name="supportedDataObjects">The supported data objects.</param>
        public void Register(IEnumerable<IEndpointSupportedDataObject> supportedDataObjects)
        {
            if (IsSessionOpen)
                throw new InvalidOperationException("Session Already Open");

            foreach (var supportedDataObject in supportedDataObjects)
                InstanceParameters.SupportedDataObjects.Add(new EtpSupportedDataObject(supportedDataObject));
        }

        /// <summary>
        /// Gets the registered handler for the specified protocol.
        /// </summary>
        /// <param name="protocol">The protocol.</param>
        /// <returns>The registered protocol handler instance.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        protected IProtocolHandler GetHandler(int protocol)
        {
            IProtocolHandler handler;
            if (HandlersByProtocol.TryGetValue(protocol, out handler))
                return handler;

            Logger.Debug(Log($"[{SessionKey}] Protocol handler not registered for protocol {EtpFactory.GetProtocolName(EtpVersion, protocol)}."));
            throw new NotSupportedException($"Protocol handler not registered for protocol {EtpFactory.GetProtocolName(EtpVersion, protocol)}.");
        }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <typeparam name="T">The type of message.</typeparam>
        /// <param name="message">The message.</param>
        protected void Sending<T>(EtpMessage<T> message)
            where T : ISpecificRecord
        {
            var now = DateTime.Now;

            if (Output != null)
            {
                Log("[{0}] Sending message at {1}: Name: {2}", SessionKey, now.ToString(TimestampFormat), message.MessageName);
                Log(EtpExtensions.Serialize(message.Header));
                if (message.Extension != null)
                    Log(EtpExtensions.Serialize(message.Extension));
                Log(EtpExtensions.Serialize(message.Body, true));
            }

            if (Logger.IsVerboseEnabled())
            {
                var extension = message.Extension == null ? string.Empty : $"{Environment.NewLine}{EtpExtensions.Serialize(message.Body, true)}";
                Logger.Verbose($"[{SessionKey}] Sending message at {now.ToString(TimestampFormat)}: Name: {message.MessageName}; Message: {EtpExtensions.Serialize(message.Header)}{Environment.NewLine}{EtpExtensions.Serialize(message.Body, true)}{extension}");
            }
        }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected void LogMessage(EtpMessage message, bool sending)
        {
            var now = DateTime.Now;

            var action = sending ? "Sending message" : "Message received";
            if (Output != null)
            {
                Log($"[{SessionKey}] {action} at {now.ToString(TimestampFormat)}");
                Log(EtpExtensions.Serialize(message.Header));
                if (message.Extension != null)
                    Log(EtpExtensions.Serialize(message.Header));
                Log(EtpExtensions.Serialize(message, true));
            }

            if (Logger.IsVerboseEnabled())
            {
                var extension = message.Extension != null
                    ? $"{EtpExtensions.Serialize(message.Extension)}{Environment.NewLine}"
                    : string.Empty;

                Logger.Verbose($"[{SessionKey}] {action} at {now.ToString(TimestampFormat)}: Name: {message.MessageName}; Message:{EtpExtensions.Serialize(message.Header)}{Environment.NewLine}{extension}{EtpExtensions.Serialize(message.Body, true)}");
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources;
        ///     <c>false</c> to release only unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Logger.Verbose($"[{SessionKey}] Disposing EtpSession for {GetType().Name}");

                IEnumerable<IProtocolHandler> handlers;

                handlers = HandlersByProtocol.Values.ToList();

                HandlersByType.Clear();
                HandlersByProtocol.Clear();

                foreach (var handler in handlers)
                {
                    handler.Dispose();
                }

                if (!_sendTokenSource.IsCancellationRequested)
                    _sendTokenSource.Cancel();

                try
                {
                    Logger.Verbose($"[{SessionKey}] Acquiring send lock in Dispose");

                    _sendLock.Wait();
                    IsSendingEnabled = false;
                }
                finally
                {
                    Logger.Verbose($"[{SessionKey}] Releasing send lock in Dispose");

                    _sendLock.Dispose();
                    _sendTokenSource.Cancel();
                }

                Logger.Verbose($"[{SessionKey}] Disposed EtpSession for {GetType().Name}");
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Validates the headers.
        /// </summary>
        protected virtual void ValidateHeaders()
        {
        }

        /// <summary>
        /// Updates the session key.
        /// </summary>
        private void UpdateSessionKey()
        {
            SessionKey = EtpFactory.CreateSessionKey(EtpVersion, SessionId, ClientInfo?.InstanceId ?? default(Guid), ServerInfo?.InstanceId ?? default(Guid));
        }
    }
}

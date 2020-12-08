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
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avro.IO;
using Avro.Specific;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Properties;
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
        private bool? _isJsonEncoding;
        private readonly List<object> _contextObjects = new List<object>();
        private string _sessionId;
        private string _clientInstanceId;
        private string _serverInstanceId;

        // Used to ensure only one thread at a time sends data over a websocket.
        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);
        // Used to ensure only one thread at a time manipulates the collection of handlers.
        private readonly ReaderWriterLockSlim _handlersLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpSession"/> class.
        /// </summary>
        /// <param name="etpVersion">The ETP version for the session.</param>
        /// <param name="application">The application name.</param>
        /// <param name="version">The application version.</param>
        /// <param name="instanceKey">The instance key to use in generating the instance identifier.</param>
        /// <param name="headers">The WebSocket or HTTP headers.</param>
        /// <param name="isClient">Whether or not this is the client-side of the session.</param>
        /// <param name="captureAsyncContext">Where or not the synchronization context should be captured for async tasks.</param>
        protected EtpSession(EtpVersion etpVersion, string application, string version, string instanceKey, IDictionary<string, string> headers, bool isClient, bool captureAsyncContext)
            : base(captureAsyncContext)
        {
            IsClient = isClient;

            Headers = headers ?? new Dictionary<string, string>();
            HandlersByType = new Dictionary<Type, IProtocolHandler>();
            HandlersByProtocol = new Dictionary<int, IProtocolHandler>();
            ValidateHeaders();

            Adapter = ResolveEtpAdapter(etpVersion);
            Adapter.RegisterCore(this);

            InstanceKey = instanceKey;

            if (IsClient)
            {
                ClientInstanceId = GuidUtility.CreateEnergisticsEtpGuid(instanceKey).ToString();
                ClientApplicationName = application;
                ClientApplicationVersion = version;
            }
            else
            {
                ServerInstanceId = GuidUtility.CreateEnergisticsEtpGuid(instanceKey).ToString();
                ServerApplicationName = application;
                ServerApplicationVersion = version;
            }

            InstanceMaxPartSize = EtpSettings.DefaultMaxPartSize;
            InstanceMaxMultipartMessageTimeInterval = EtpSettings.DefaultMaxMultipartMessageTimeInterval;
            InstanceMaxWebSocketFramePayloadSize = EtpSettings.DefaultMaxWebSocketFramePayloadSize;
            InstanceMaxWebSocketMessagePayloadSize = EtpSettings.DefaultMaxWebSocketMessagePayloadSize;
            InstanceSupportsAlternateRequestUris = EtpSettings.SupportsAlternateRequestUris;
        }

        /// <summary>
        /// Gets the ETP version supported by this session.
        /// </summary>
        public EtpVersion SupportedVersion => Adapter.SupportedVersion;

        /// <summary>
        /// Gets the version specific ETP adapter.
        /// </summary>
        public IEtpAdapter Adapter { get; private set; }

        /// <summary>
        /// Gets whether or not this is the client-side of the session.
        /// </summary>
        public bool IsClient { get; private set; }

        /// <summary>
        /// Gets the name of the client application.
        /// </summary>
        /// <value>The name of the client application.</value>
        public string ClientApplicationName { get; set; }

        /// <summary>
        /// Gets the client application version.
        /// </summary>
        /// <value>The client application version.</value>
        public string ClientApplicationVersion { get; set; }

        /// <summary>
        /// Gets or sets the client instance identifier.
        /// </summary>
        /// <value>The client instance identifier.</value>
        public string ClientInstanceId
        {
            get { return _clientInstanceId; }
            set
            {
                _clientInstanceId = value;
                UpdateSessionKey();
            }
        }

        /// <summary>
        /// Gets the client endpoint capabilities.
        /// </summary>
        /// <value>The client endpoint capabilities.</value>
        public EtpEndpointCapabilities ClientEndpointCapabilities { get; set; } = new EtpEndpointCapabilities();

        /// <summary>
        /// Gets the name of the server application.
        /// </summary>
        /// <value>The name of the server application.</value>
        public string ServerApplicationName { get; set; }

        /// <summary>
        /// Gets the server application version.
        /// </summary>
        /// <value>The server application version.</value>
        public string ServerApplicationVersion { get; set; }

        /// <summary>
        /// Gets or sets the server instance identifier.
        /// </summary>
        /// <value>The server instance identifier.</value>
        public string ServerInstanceId
        {
            get { return _serverInstanceId; }
            set
            {
                _serverInstanceId = value;
                UpdateSessionKey();
            }
        }

        /// <summary>
        /// Gets the server endpoint capabilities.
        /// </summary>
        /// <value>The server endpoint capabilities.</value>
        public EtpEndpointCapabilities ServerEndpointCapabilities { get; set; } = new EtpEndpointCapabilities();

        /// <summary>
        /// Gets the instance key.
        /// </summary>
        /// <value>The instance key, which is used to generate the client or server instance identifier.</value>
        public string InstanceKey { get; }

        /// <summary>
        /// Gets or sets the session key.
        /// </summary>
        /// <value>The session key.</value>
        public string SessionKey { get; private set; }

        /// <summary>
        /// Gets the session identifier.
        /// </summary>
        /// <value>The session identifier.</value>
        public string SessionId
        {
            get { return _sessionId; }
            set
            {
                _sessionId = value;
                UpdateSessionKey();
            }
        }

        /// <summary>
        /// This is the largest data object the store or customer can get or put. A store or customer can optionally specify these for protocols that handle data objects.
        /// </summary>
        public long InstanceMaxDataObjectSize { get; private set; }

        /// <summary>
        /// This is the largest part size the store or customer can get or put. A store can optionally specify this for protocols that handle object parts. Property of numberofbytes.
        /// </summary>
        public long InstanceMaxPartSize { get; private set; }

        /// <summary>
        /// Maximum time interval between subsequent messages in the SAME multipart request or response.
        /// </summary>
        public long InstanceMaxMultipartMessageTimeInterval { get; private set; }

        /// <summary>
        /// Maximum size allowed for a WebSocket frame (which is determined by the library you use to implement WebSocket). WebSocket is the transport protocol used by ETP.
        /// </summary>
        public long InstanceMaxWebSocketFramePayloadSize { get; private set; }

        /// <summary>
        /// Maximum size allowed for a WebSocket message (which is composed of multiple WebSocket frames, which is determined by the library you use to implement WebSocket). WebSocket is the transport protocol used by ETP.
        /// </summary>
        public long InstanceMaxWebSocketMessagePayloadSize { get; private set; }

        /// <summary>
        /// This is the largest part size the store or customer can get or put. A store can optionally specify this for protocols that handle object parts. Property of numberofbytes.
        /// </summary>
        public bool InstanceSupportsAlternateRequestUris { get; private set; }

        /// <summary>
        /// This is the largest data object the store or customer can get or put. A store or customer can optionally specify these for protocols that handle data objects.
        /// </summary>
        public long CounterpartMaxDataObjectSize { get; private set; }

        /// <summary>
        /// This is the largest part size the store or customer can get or put. A store can optionally specify this for protocols that handle object parts. Property of numberofbytes.
        /// </summary>
        public long CounterpartMaxPartSize { get; private set; }

        /// <summary>
        /// Maximum time interval between subsequent messages in the SAME multipart request or response.
        /// </summary>
        public long CounterpartMaxMultipartMessageTimeInterval { get; private set; }

        /// <summary>
        /// Maximum size allowed for a WebSocket frame (which is determined by the library you use to implement WebSocket). WebSocket is the transport protocol used by ETP.
        /// </summary>
        public long CounterpartMaxWebSocketFramePayloadSize { get; private set; }

        /// <summary>
        /// Maximum size allowed for a WebSocket message (which is composed of multiple WebSocket frames, which is determined by the library you use to implement WebSocket). WebSocket is the transport protocol used by ETP.
        /// </summary>
        public long CounterpartMaxWebSocketMessagePayloadSize { get; private set; }

        /// <summary>
        /// This is the largest part size the store or customer can get or put. A store can optionally specify this for protocols that handle object parts. Property of numberofbytes.
        /// </summary>
        public bool CounterpartSupportsAlternateRequestUris { get; private set; }

        /// <summary>
        /// Gets the protocols supported by this instance.
        /// </summary>
        /// <returns>A list of protocols supported by this instance.</returns>
        public IReadOnlyList<EtpSessionProtocol> InstanceSupportedProtocols { get; private set; } = new List<EtpSessionProtocol>();

        /// <summary>
        /// Gets or sets the types of objects supported by this instance.
        /// </summary>
        /// <returns>A list of object types supported by this instance.</returns>
        public IList<IDataObjectType> InstanceSupportedDataObjects { get; set; } = new List<IDataObjectType>();

        /// <summary>
        /// Gets the types of compression supported by this instance.
        /// </summary>
        /// <returns>A list of compression types supported by this instance.</returns>
        public IList<string> InstanceSupportedCompression { get; set; } = new List<string>();

        /// <summary>
        /// Gets the formats supported by this instance.
        /// </summary>
        /// <returns>A list of formats supported by this instance.</returns>
        public IList<string> InstanceSupportedFormats { get; set; } = new List<string>();

        /// <summary>
        /// Gets the endpoint capabilities supported by this instance.
        /// </summary>
        /// <returns>The endpoint capabilities supported by this instance.</returns>
        public EtpEndpointCapabilities InstanceEndpointCapabilities { get; set; } = new EtpEndpointCapabilities();

        /// <summary>
        /// Gets a value indicating whether the underlying websocket connection is open.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is open; otherwise, <c>false</c>.
        /// </value>
        public abstract bool IsOpen { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is json encoding.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is json encoding; otherwise, <c>false</c>.
        /// </value>
        public bool IsJsonEncoding
        {
            get
            {
                if (!_isJsonEncoding.HasValue)
                {
                    string header;
                    Headers.TryGetValue(Settings.Default.EtpEncodingHeader, out header);
                    _isJsonEncoding = Settings.Default.EtpEncodingJson.Equals(header);
                }

                return _isJsonEncoding.Value;
            }
        }

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
        public IReadOnlyList<EtpSessionProtocol> SessionSupportedProtocols { get; private set; }

        /// <summary>
        /// Gets or sets the negotiated compression type for this session.
        /// </summary>
        public string SessionSupportedCompression { get; private set; }

        /// <summary>
        /// Gets or sets the negotiated list of supported objects for this session.
        /// </summary>
        /// <value>The negotiated list of supported objects for this session.</value>
        public IReadOnlyList<IDataObjectType> SessionSupportedDataObjects { get; private set; }

        /// <summary>
        /// Gets or sets the negotiated list of formats supported for this session.
        /// </summary>
        public IReadOnlyList<string> SessionSupportedFormats { get; private set; }

        /// <summary>
        /// Gets the collection of registered protocol handlers by Type.
        /// </summary>
        /// <value>The handlers.</value>
        private IDictionary<Type, IProtocolHandler> HandlersByType { get; }

        /// <summary>
        /// Gets the collection of registered protocol handlers by protocol.
        /// </summary>
        /// <value>The handlers.</value>
        private IDictionary<int, IProtocolHandler> HandlersByProtocol { get; }

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
            try
            {
                _handlersLock.TryEnterReadLock(-1);

                IProtocolHandler handler;
                if (HandlersByType.TryGetValue(typeof(T), out handler) && handler is T)
                    return (T)handler;
            }
            finally
            {
                _handlersLock.ExitReadLock();
            }

            Logger.Debug(Log("[{0}] Protocol handler not registered for {1}.", SessionKey, typeof(T).FullName));
            throw new NotSupportedException($"Protocol handler not registered for { typeof(T).FullName }.");
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
            try
            {
                _handlersLock.TryEnterReadLock(-1);

                return HandlersByType.ContainsKey(typeof(T));
            }
            finally
            {
                _handlersLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Occurs when the WebSocket is opened.
        /// </summary>
        public event EventHandler SocketOpened;
        
        /// <summary>
        /// Raises the SocketOpened event.
        /// </summary>
        protected void InvokeSocketOpened()
        {
            var prevSocketOpenedEventCount = Interlocked.CompareExchange(ref _socketOpenedEventCount, 1, 0);

            if (prevSocketOpenedEventCount == 0)
                SocketOpened?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the WebSocket is closed.
        /// </summary>
        public event EventHandler SocketClosed;

        /// <summary>
        /// Raises the SocketClosed event.
        /// </summary>
        protected void InvokeSocketClosed()
        {
            var prevSocketOpenedEventCount = Interlocked.CompareExchange(ref _socketOpenedEventCount, 0, 1);

            if (prevSocketOpenedEventCount == 1)
                SocketClosed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the WebSocket has an error.
        /// </summary>
        public event EventHandler<Exception> SocketError;

        /// <summary>
        /// Raises the SocketError event.
        /// </summary>
        /// <param name="ex">The socket exception.</param>
        protected void InvokeSocketError(Exception ex)
        {
            var prevSocketOpenedEventCount = Interlocked.CompareExchange(ref _socketOpenedEventCount, 0, 1);

            if (prevSocketOpenedEventCount == 1)
                SocketError?.Invoke(this, ex);
        }


        /// <summary>
        /// Initializes the instance capabilities supported by the session.
        /// </summary>
        /// <param name="capabilities">The instances's capabilities.</param>
        public virtual void InitializeInstanceCapabilities(EtpEndpointCapabilities capabilities)
        {
            InstanceMaxDataObjectSize = capabilities.MaxDataObjectSize ?? InstanceMaxDataObjectSize;
            InstanceMaxPartSize = capabilities.MaxPartSize ?? InstanceMaxPartSize;
            InstanceMaxMultipartMessageTimeInterval = capabilities.MaxMultipartMessageTimeInterval ?? InstanceMaxMultipartMessageTimeInterval;
            InstanceMaxWebSocketFramePayloadSize = capabilities.MaxWebSocketFramePayloadSize ?? InstanceMaxWebSocketFramePayloadSize;
            InstanceMaxWebSocketMessagePayloadSize = capabilities.MaxWebSocketMessagePayloadSize ?? InstanceMaxWebSocketMessagePayloadSize;
            InstanceSupportsAlternateRequestUris = capabilities.SupportsAlternateRequestUris ?? InstanceSupportsAlternateRequestUris;
        }

        /// <summary>
        /// Gets the capabilities supported by the session.
        /// </summary>
        /// <param name="capabilities">The instances's capabilities.</param>
        public virtual void GetInstanceCapabilities(EtpEndpointCapabilities capabilities)
        {
            capabilities.MaxDataObjectSize = InstanceMaxDataObjectSize;
            capabilities.MaxPartSize = InstanceMaxPartSize;
            capabilities.MaxMultipartMessageTimeInterval = InstanceMaxMultipartMessageTimeInterval;
            capabilities.MaxWebSocketFramePayloadSize = InstanceMaxWebSocketFramePayloadSize;
            capabilities.MaxWebSocketMessagePayloadSize = InstanceMaxWebSocketMessagePayloadSize;
            capabilities.SupportsAlternateRequestUris = InstanceSupportsAlternateRequestUris;
        }

        /// <summary>
        /// Initialize the set of supported protocols from the registered handlers.
        /// </summary>
        public void InitializeInstanceSupportedProtocols()
        {
            try
            {
                _handlersLock.TryEnterReadLock(-1);

                InstanceSupportedProtocols = EtpExtensions.GetSupportedProtocols(HandlersByType.Values, SupportedVersion);
            }
            finally
            {
                _handlersLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Initialize the session based on details from the counterpart.
        /// After this, the protocols, objects, compression, formats and capabilities that will be used in the session will be initialized.
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <param name="counterpartApplicationName">The counterpart's application name.</param>
        /// <param name="counterpartApplicationVersion">The counterpart's application version.</param>
        /// <param name="counterpartInstanceId">The counterpart's instance ID.</param>
        /// <param name="counterpartSupportedProtocols">The counterpart's supported protocols.</param>
        /// <param name="counterpartSupportedDataObjects">The counterpart's supported data objects.</param>
        /// <param name="counterpartSupportedCompression">The counterpart's supported compression.</param>
        /// <param name="counterpartSupportedFormats">The counterpart's supported formats.</param>
        /// <param name="counterpartEndpointCapabilities">The counterpart's endpoint capabilities.</param>
        /// <returns><c>true</c> if the session was successfully initialized; <c>false</c> otherwise.</returns>
        public virtual bool InitializeSession(string sessionId, string counterpartApplicationName, string counterpartApplicationVersion, string counterpartInstanceId, IReadOnlyList<ISupportedProtocol> counterpartSupportedProtocols, IReadOnlyList<IDataObjectType> counterpartSupportedDataObjects, IReadOnlyList<string> counterpartSupportedCompression, IReadOnlyList<string> counterpartSupportedFormats, EtpEndpointCapabilities counterpartEndpointCapabilities)
        {
            var success = true;

            SessionId = sessionId;
            if (IsClient)
            {
                ServerApplicationName = counterpartApplicationName;
                ServerApplicationVersion = counterpartApplicationVersion;
                ServerInstanceId = counterpartInstanceId;
            }
            else
            {
                ClientApplicationName = counterpartApplicationName;
                ClientApplicationVersion = counterpartApplicationVersion;
                ClientInstanceId = counterpartInstanceId;
            }

            if (!InitializeSessionSupportedProtocols(counterpartSupportedProtocols))
                success = false;

            if (!InitializeSessionSupportedDataObjects(counterpartSupportedDataObjects))
                success = false;

            if (!InitializeSessionSupportedCompression(counterpartSupportedCompression))
                success = false;

            if (!InitializeSessionSupportedFormats(counterpartSupportedFormats))
                success = false;

            if (!InitializeCounterpartCapabilities(counterpartEndpointCapabilities))
                success = false;

            return success;
        }

        /// <summary>
        /// Initializes the protocols for use in this session.
        /// </summary>
        /// <param name="counterpartSupportedProtocols">The protocols supported by the counterpart</param>
        /// <returns><c>true</c> if initialization was successful; <c>false</c> otherwise.</returns>
        protected virtual bool InitializeSessionSupportedProtocols(IReadOnlyList<ISupportedProtocol> counterpartSupportedProtocols)
        {
            var success = true;

            var supportedProtocols = new List<EtpSessionProtocol>();
            foreach (var protocol in InstanceSupportedProtocols)
            {
                var counterpartProtocol = counterpartSupportedProtocols.FirstOrDefault(p => p.Protocol == protocol.Protocol && p.VersionString.Equals(protocol.VersionString, StringComparison.OrdinalIgnoreCase) && p.Role.Equals(protocol.Role, StringComparison.OrdinalIgnoreCase));
                if (counterpartProtocol == null)
                {
                    if (IsClient)
                    {
                        Logger.Debug($"[{SessionKey}] Requested protocol not supported by server. Protocol: {protocol.Protocol}; Role: {protocol.Role}; Version: {protocol.VersionString}");
                        success = false;
                    }

                    continue;
                }

                var role = IsClient ? EtpExtensions.GetCounterpartRole(counterpartProtocol.Role) : counterpartProtocol.Role;
                var counterpartRole = IsClient ? counterpartProtocol.Role : EtpExtensions.GetCounterpartRole(counterpartProtocol.Role);
                if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(counterpartRole))
                {
                    Logger.Debug($"[{SessionKey}] Invalid role or counterpart role. Role: {role}; Counterpart Role: {counterpartRole}");
                    success = false;
                    continue;
                }

                supportedProtocols.Add(new EtpSessionProtocol(protocol.Protocol, protocol.ProtocolVersion, role, counterpartRole, protocol.Capabilities, new EtpProtocolCapabilities(counterpartProtocol.ProtocolCapabilities)));
            }

            foreach (var counterpartProtocol in counterpartSupportedProtocols)
            {
                var supportedProtocol = supportedProtocols.FirstOrDefault(p => p.Protocol == counterpartProtocol.Protocol && p.VersionString.Equals(counterpartProtocol.VersionString, StringComparison.OrdinalIgnoreCase) && p.Role.Equals(counterpartProtocol.Role, StringComparison.OrdinalIgnoreCase));
                if (supportedProtocol == null)
                {
                    if (IsClient)
                        Logger.Debug($"[{SessionKey}] Ignoring unrequested protocol. Protocol: {counterpartProtocol.Protocol}; Role: {counterpartProtocol.Role}");
                    else
                        Logger.Debug($"[{SessionKey}] Requested protocol not supported. Protocol: {counterpartProtocol.Protocol}; Role: {counterpartProtocol.Role}");
                }
            }

            SessionSupportedProtocols = supportedProtocols;
            return success;
        }

        /// <summary>
        /// Initializes the supported objects for this session.
        /// </summary>
        /// <param name="counterpartSupportedDataObjects">The data objects supported by the counterpart.</param>
        /// <returns><c>true</c> if initialization was successful; <c>false</c> otherwise.</returns>
        protected virtual bool InitializeSessionSupportedDataObjects(IReadOnlyList<IDataObjectType> counterpartSupportedDataObjects)
        {
            var success = true;

            var supportedDataObjects = new List<IDataObjectType>();
            foreach (var supportedDataObject in InstanceSupportedDataObjects)
            {
                if (!supportedDataObject.IsValid || supportedDataObject.IsBaseType)
                {
                    if (SupportedVersion == EtpVersion.v11)
                        Logger.Warn($"[{SessionKey}] Invalid content type supported by this instance. Content Type: {supportedDataObject.ContentType}");
                    else
                        Logger.Warn($"[{SessionKey}] Invalid data object type supported by this instance. Data Object Type: {supportedDataObject.DataObjectType}");

                    continue;
                }

                foreach (var counterpartSupportedDataObject in counterpartSupportedDataObjects)
                {
                    if (!counterpartSupportedDataObject.IsValid || counterpartSupportedDataObject.IsBaseType)
                        continue;

                    if (supportedDataObject.IsBaseType ||
                        !supportedDataObject.Family.Equals(counterpartSupportedDataObject.Family, StringComparison.OrdinalIgnoreCase) ||
                        !supportedDataObject.Version.Equals(counterpartSupportedDataObject.Version, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (supportedDataObject.IsWildcard || counterpartSupportedDataObject.IsWildcard || supportedDataObject.ObjectType.Equals(counterpartSupportedDataObject.ObjectType, StringComparison.OrdinalIgnoreCase))
                    {
                        supportedDataObjects.Add(supportedDataObject);
                    }
                }
            }

            // TODO: Log unsupported types received from counterpart.

            return success;
        }

        /// <summary>
        /// Initializes the compression for use in this session.
        /// </summary>
        /// <param name="counterpartSupportedCompression">The compression supported by the counterpart</param>
        /// <returns><c>true</c> if initialization was successful; <c>false</c> otherwise.</returns>
        protected virtual bool InitializeSessionSupportedCompression(IReadOnlyList<string> counterpartSupportedCompression)
        {
            string supportedCompression = string.Empty;
            foreach (var compression in InstanceSupportedCompression)
            {
                if (!string.IsNullOrEmpty(compression) && !compression.Equals("gzip", StringComparison.OrdinalIgnoreCase))
                {
                    Logger.Warn($"[{SessionKey}] Unexpected compression supported by this instance: {compression}");
                }

                var counterpartCompression = counterpartSupportedCompression.FirstOrDefault(c => (string.IsNullOrEmpty(c) && string.IsNullOrEmpty(compression)) || compression.Equals(c, StringComparison.OrdinalIgnoreCase));
                if (counterpartCompression != null)
                {
                    supportedCompression = counterpartCompression;
                    break;
                }
            }

            // TODO: Log unsupported compression received from counterpart.

            SessionSupportedCompression = supportedCompression;
            return true;
        }

        /// <summary>
        /// Initializes the formats for use in this session.
        /// </summary>
        /// <param name="counterpartSupportedFormats">The formats supported by the counterpart</param>
        /// <returns><c>true</c> if initialization was successful; <c>false</c> otherwise.</returns>
        protected virtual bool InitializeSessionSupportedFormats(IReadOnlyList<string> counterpartSupportedFormats)
        {
            var supportedFormats = new List<string>();
            foreach (var format in InstanceSupportedFormats)
            {
                if (!format.Equals("xml", StringComparison.OrdinalIgnoreCase))
                {
                    Logger.Warn($"[{SessionKey}] Unexpected format supported by this instance: {format}");
                }

                var counterpartFormat = counterpartSupportedFormats.FirstOrDefault(f => format.Equals(f, StringComparison.OrdinalIgnoreCase));
                if (counterpartFormat != null)
                    supportedFormats.Add(counterpartFormat);
            }

            // TODO: Log unsupported formats received from counterpart.

            SessionSupportedFormats = supportedFormats;
            return true;
        }

        protected virtual bool InitializeCounterpartCapabilities(EtpEndpointCapabilities counterpartCapabilities)
        {
            CounterpartMaxDataObjectSize = counterpartCapabilities.MaxDataObjectSize ?? long.MaxValue;
            CounterpartMaxPartSize = counterpartCapabilities.MaxPartSize ?? long.MaxValue;
            CounterpartMaxMultipartMessageTimeInterval = counterpartCapabilities.MaxMultipartMessageTimeInterval ?? long.MaxValue;
            CounterpartMaxWebSocketFramePayloadSize = counterpartCapabilities.MaxWebSocketFramePayloadSize ?? long.MaxValue;
            CounterpartMaxWebSocketMessagePayloadSize = counterpartCapabilities.MaxWebSocketMessagePayloadSize ?? long.MaxValue;
            CounterpartSupportsAlternateRequestUris = counterpartCapabilities.SupportsAlternateRequestUris ?? false;

            SessionMaxWebSocketFramePayloadSize = Math.Min(CounterpartMaxWebSocketFramePayloadSize, InstanceMaxWebSocketFramePayloadSize);
            CounterpartMaxWebSocketMessagePayloadSize = Math.Min(CounterpartMaxWebSocketMessagePayloadSize, InstanceMaxWebSocketMessagePayloadSize);

            return true;
        }

        /// <summary>
        /// Called when the ETP session is opened.
        /// </summary>
        /// <param name="openedSuccessfully"><c>true</c> if the session opened without errors; <c>false</c> if there were errors when opening the session.</param>
        public virtual void OnSessionOpened(bool openedSuccessfully)
        {
            Logger.Trace($"[{SessionKey}] OnSessionOpened; Success: {openedSuccessfully}");
            HandleUnsupportedProtocols();

            if (openedSuccessfully)
            {
                try
                {
                    _handlersLock.TryEnterReadLock(-1);

                    var handlers = HandlersByType.Values.Where(x => x.SupportedVersion == SupportedVersion).ToList();

                    // notify protocol handlers about new session
                    foreach (var handler in handlers)
                    {
                        var protocol = SessionSupportedProtocols.FirstOrDefault(x => x.Protocol == handler.Protocol &&
                                        string.Equals(x.Role, handler.Role, StringComparison.InvariantCultureIgnoreCase));

                        if (protocol == null)
                            continue;

                        handler.OnSessionOpened(protocol.CounterpartCapabilities);
                    }
                }
                finally
                {
                    _handlersLock.ExitReadLock();
                }
            }

            SessionOpened?.Invoke(openedSuccessfully);
        }

        /// <summary>
        /// Event raised when the session is opened with a paremeters indicating whether there were errors when opening the session, the open session message header, the list of requested protocols and the list of supported protocols for the session.
        /// </summary>
        public event Action<bool> SessionOpened;

        /// <summary>
        /// Called when the ETP session is closed.
        /// </summary>
        /// <param name="closedSuccessfully"><c>true</c> if the session closed without errors; <c>false</c> if there were errors when closing the session.</param>
        public virtual void OnSessionClosed(bool closedSuccessfully)
        {
            Logger.Trace($"[{SessionKey}] OnSessionClosed; Success: {closedSuccessfully}");

            try
            {
                _handlersLock.TryEnterReadLock(-1);

                var handlers = HandlersByType.Values.ToList();

                // notify protocol handlers about new session
                foreach (var handler in handlers)
                {
                    handler.OnSessionClosed();
                }
            }
            finally
            {
                _handlersLock.ExitReadLock();
            }

            SessionClosed?.Invoke(closedSuccessfully);
        }

        /// <summary>
        /// Event raised when the session is closed with a paremeter indicating whether there were errors when closing the session.
        /// </summary>
        public event Action<bool> SessionClosed;

        /// <summary>
        /// Called when WebSocket data is received.
        /// </summary>
        /// <param name="data">The data.</param>
        public virtual void OnDataReceived(byte[] data)
        {
            Decode(data);
        }

        /// <summary>
        /// Called when a WebSocket message is received.
        /// </summary>
        /// <param name="message">The message.</param>
        public virtual void OnMessageReceived(string message)
        {
            Decode(message);
        }

        /// <summary>
        /// Synchronously sends the message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="header">The header.</param>
        /// <param name="body">The body.</param>
        /// <param name="onBeforeSend">Action called just before sending the message with the actual header having the definitive message ID.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public long SendMessage<T>(IMessageHeader header, T body, Action<IMessageHeader, T> onBeforeSend = null)
            where T : ISpecificRecord
        {
            return AsyncContext.Run(() => SendMessageAsync(header, body, onBeforeSend));
        }

        /// <summary>
        /// Asynchronously sends the message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="header">The header.</param>
        /// <param name="body">The body.</param>
        /// <param name="onBeforeSend">Action called just before sending the message with the actual header having the definitive message ID.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public async Task<long> SendMessageAsync<T>(IMessageHeader header, T body, Action<IMessageHeader, T> onBeforeSend = null) where T : ISpecificRecord
        {
            // Lock to ensure only one thread at a time attempts to send data and to ensure that messages are sent with sequential IDs
            try
            {
                try
                {
                    await _sendLock.WaitAsync().ConfigureAwait(CaptureAsyncContext);

                    if (!IsOpen)
                    {
                        Log("Warning: Sending on a session that is not open.");
                        Logger.Debug("Sending on a session that is not open.");
                        return -1;
                    }

                    header.MessageId = NewMessageId();

                    // Call the pre-send action in case any deterministic handling is needed with the actual message ID.
                    // Must be invoked before sending to ensure the response is not asynchronously processed before this method returns.
                    onBeforeSend?.Invoke(header, body);

                    // Log message just before it gets sent if needed.
                    Sending(header, body);

                    if (IsJsonEncoding)
                    {
                        var message = EtpExtensions.Serialize(new object[] {header, body});
                        if (!await SendAsync(message).ConfigureAwait(CaptureAsyncContext))
                            return -1;
                    }
                    else
                    {
                        var data = body.Encode(header, SessionSupportedCompression);
                        if (!await SendAsync(data, 0, data.Length).ConfigureAwait(CaptureAsyncContext))
                            return -1;
                    }
                }
                finally
                {
                    _sendLock.Release();
                }
            }
            catch (Exception ex)
            {
                // Send protocol exception unless the message being sent was already a protocol exception.
                if (header.MessageType != (int)v11.MessageTypes.Core.ProtocolException)
                    Handler(header.Protocol).InvalidState(ex.Message, header.MessageId);

                return -1;
            }

            return header.MessageId;
        }

        /// <summary>
        /// Generates a new unique message identifier for the current session.
        /// </summary>
        /// <returns>The new message identifier.</returns>
        public long NewMessageId()
        {
            return Interlocked.Increment(ref _messageId);
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
            // Closing sends messages over the websocket so need to ensure no other messages are being sent when closing
            try
            {
                await _sendLock.WaitAsync().ConfigureAwait(CaptureAsyncContext);
                Logger.Trace($"Closing Session: {reason}");

                await CloseWebSocketAsyncCore(reason).ConfigureAwait(CaptureAsyncContext);
            }
            finally
            {
                _sendLock.Release();
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
        /// Resolves the ETP adapter.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <returns>A new <see cref="IEtpAdapter"/> instance.</returns>
        private static IEtpAdapter ResolveEtpAdapter(EtpVersion version)
        {
            return version == EtpVersion.v12 ? (IEtpAdapter) new v12.Etp12Adapter() : new v11.Etp11Adapter();
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
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        protected abstract Task<bool> SendAsync(byte[] data, int offset, int length);

        /// <summary>
        /// Sends the specified messages.
        /// </summary>
        /// <param name="message">The message.</param>
        protected abstract Task<bool> SendAsync(string message);

        /// <summary>
        /// Decodes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        protected void Decode(byte[] data)
        {
            using (var inputStream = new MemoryStream(data))
            {
                // create avro binary decoder to read from memory stream
                var decoder = new BinaryDecoder(inputStream);
                // deserialize the header
                var header = Adapter.DecodeMessageHeader(decoder, null);

                // log message metadata
                if (Logger.IsVerboseEnabled())
                {
                    Logger.VerboseFormat("[{0}] Binary message received: {1}", SessionKey, EtpExtensions.Serialize(header));
                }

                Stream gzip = null;

                try
                {
                    // decompress message body if compression has been negotiated
                    if (header.CanCompressMessageBody(true))
                    {
                        if (EtpExtensions.GzipEncoding.Equals(SessionSupportedCompression, StringComparison.InvariantCultureIgnoreCase))
                        {
                            gzip = new GZipStream(inputStream, CompressionMode.Decompress, true);
                            decoder = new BinaryDecoder(gzip);
                        }
                    }

                    // call processing action
                    HandleMessage(header, decoder, null);
                }
                finally
                {
                    gzip?.Dispose();
                }
            }
        }

        /// <summary>
        /// Decodes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected void Decode(string message)
        {
            // split message header and body
            var array = JArray.Parse(message);
            var header = array[0].ToString();
            var body = array[1].ToString();

            // log message metadata
            if (Logger.IsVerboseEnabled())
            {
                Logger.VerboseFormat("[{0}] JSON message received: {1}", SessionKey, header);
            }
            
            // call processing action
            HandleMessage(Adapter.DeserializeMessageHeader(header), null, body);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="decoder">The decoder.</param>
        /// <param name="body">The body.</param>
        protected void HandleMessage(IMessageHeader header, Decoder decoder, string body)
        {
            try
            {
                IProtocolHandler handler;
                _handlersLock.TryEnterReadLock(-1);

                HandlersByProtocol.TryGetValue(header.Protocol, out handler);

                if (handler == null)
                {
                    HandlersByProtocol.TryGetValue((int)v11.Protocols.Core, out handler);

                    if (handler == null) // Socket has been closed
                    {
                        Logger.Trace($"Ignoring message on closed session: {EtpExtensions.Serialize(header)}");
                        return;
                    }

                    handler.UnsupportedProtocol(header.Protocol, header.MessageId);

                    return;
                }

                var message = Adapter.DecodeMessage(header.Protocol, header.MessageType, decoder, body);
                if (message == null)
                {
                    handler.InvalidMessage(header);
                    return;
                }

                Received(header, message);

                try
                {
                    // Handle global Acknowledge request
                    if (header.IsAcknowledgeRequested())
                    {
                        handler.Acknowledge(header.MessageId);
                    }

                    handler.HandleMessage(header, message);
                }
                catch (Exception ex)
                {
                    Logger.Debug(ex);
                    handler.InvalidState(ex.Message, header.MessageId);
                }
            }
            finally
            {
                _handlersLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Registers a protocol handler for the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="handlerType">Type of the handler.</param>
        protected override void Register(Type contractType, Type handlerType)
        {
            base.Register(contractType, handlerType);

            var handler = CreateInstance(contractType);

            if (handler != null)
            {
                handler.Session = this;
                HandlersByType[contractType] = handler;
                HandlersByProtocol[handler.Protocol] = handler;
            }
        }

        /// <summary>
        /// Gets the registered handler for the specified protocol.
        /// </summary>
        /// <param name="protocol">The protocol.</param>
        /// <returns>The registered protocol handler instance.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        protected IProtocolHandler GetHandler(int protocol)
        {
            try
            {
                _handlersLock.TryEnterReadLock(-1);

                IProtocolHandler handler;
                if (HandlersByProtocol.TryGetValue(protocol, out handler))
                    return handler;
            }
            finally
            {
                _handlersLock.ExitReadLock();
            }

            Logger.Debug(Log("[{0}] Protocol handler not registered for protocol {1}.", SessionKey, protocol));
            throw new NotSupportedException($"Protocol handler not registered for protocol { protocol }.");
        }

        /// <summary>
        /// Handles the unsupported protocols.
        /// </summary>
        protected virtual void HandleUnsupportedProtocols()
        {
            // remove unsupported handler mappings (excluding Core protocol)
            try
            {
                _handlersLock.TryEnterReadLock(-1);

                HandlersByType
                    .Where(x => x.Value.Protocol > 0 && !SessionSupportedProtocols.Contains(x.Value.Protocol, x.Value.Role))
                    .ToList()
                    .ForEach(x =>
                    {
                        x.Value.Session = null;
                        HandlersByType.Remove(x.Key);
                        HandlersByProtocol.Remove(x.Value.Protocol);
                    });

                // update remaining handler mappings by protocol
                foreach (var handler in HandlersByType.Values.ToArray())
                {
                    if (!HandlersByProtocol.ContainsKey(handler.Protocol))
                        HandlersByProtocol[handler.Protocol] = handler;
                }
            }
            finally
            {
                _handlersLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Logs the specified header and message body.
        /// </summary>
        /// <typeparam name="T">The type of message.</typeparam>
        /// <param name="header">The header.</param>
        /// <param name="body">The message body.</param>
        protected void Sending<T>(IMessageHeader header, T body)
        {
            var now = DateTime.Now;

            if (Output != null)
            {
                Log("[{0}] Sending message at {1}", SessionKey, now.ToString(TimestampFormat));
                Log(EtpExtensions.Serialize(header));
                Log(EtpExtensions.Serialize(body, true));
            }

            if (Logger.IsVerboseEnabled())
            {
                Logger.VerboseFormat("[{0}] Sending message at {1}: {2}{3}{4}",
                    SessionKey, now.ToString(TimestampFormat), EtpExtensions.Serialize(header), Environment.NewLine, EtpExtensions.Serialize(body, true));
            }
        }

        /// <summary>
        /// Logs the specified message header and body.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message body.</param>
        protected void Received(IMessageHeader header, ISpecificRecord message)
        {
            var now = DateTime.Now;

            if (Output != null)
            {
                Log("[{0}] Message received at {1}", SessionId, now.ToString(TimestampFormat));
                Log(EtpExtensions.Serialize(header));
                Log(EtpExtensions.Serialize(message, true));
            }

            if (Logger.IsVerboseEnabled())
            {
                Logger.VerboseFormat("[{0}] Message received at {1}: {2}{3}{4}",
                    SessionKey, now.ToString(TimestampFormat), EtpExtensions.Serialize(header), Environment.NewLine, EtpExtensions.Serialize(message, true));
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
                IEnumerable<IProtocolHandler> handlers;

                try
                {
                    _handlersLock.TryEnterWriteLock(-1);

                    handlers = HandlersByType.Values.ToList();

                    HandlersByType.Clear();
                    HandlersByProtocol.Clear();

                    foreach (var handler in handlers)
                    {
                        handler.Dispose();
                    }
                }
                finally
                {
                    _handlersLock.ExitWriteLock();
                }
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
            SessionKey = SupportedVersion == EtpVersion.v11
                ? SessionId
                : $"Session: {SessionId}; Client: {ClientInstanceId}; Server: {ServerInstanceId}";
        }
    }
}

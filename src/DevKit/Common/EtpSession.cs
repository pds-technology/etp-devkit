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
        private long _messageId;
        private bool? _isJsonEncoding;
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
        /// <param name="headers">The WebSocket or HTTP headers.</param>
        /// <param name="isClient">Whether or not this is the client-side of the session.</param>
        /// <param name="captureAsyncContext">Where or not the synchronization context should be captured for async tasks.</param>
        protected EtpSession(EtpVersion etpVersion, string application, string version, IDictionary<string, string> headers, bool isClient, bool captureAsyncContext)
            : base(captureAsyncContext)
        {
            IsClient = isClient;

            Headers = headers ?? new Dictionary<string, string>();
            HandlersByType = new Dictionary<Type, IProtocolHandler>();
            HandlersByProtocol = new Dictionary<int, IProtocolHandler>();
            ApplicationName = application;
            ApplicationVersion = version;
            ValidateHeaders();

            Adapter = ResolveEtpAdapter(etpVersion);
            Adapter.RegisterCore(this);
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
        /// Gets the name of the application.
        /// </summary>
        /// <value>The name of the application.</value>
        public string ApplicationName { get; }

        /// <summary>
        /// Gets the application version.
        /// </summary>
        /// <value>The application version.</value>
        public string ApplicationVersion { get; }

        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        /// <value>The session identifier.</value>
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets the supported compression type.
        /// </summary>
        public string SupportedCompression { get; set; }

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

            Logger.Debug(Log("[{0}] Protocol handler not registered for {1}.", SessionId, typeof(T).FullName));
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
        /// Called when the ETP session is opened.
        /// </summary>
        /// <param name="requestedProtocols">The requested protocols.</param>
        /// <param name="supportedProtocols">The supported protocols.</param>
        public override void OnSessionOpened(IList<ISupportedProtocol> requestedProtocols, IList<ISupportedProtocol> supportedProtocols)
        {
            Logger.Trace($"[{SessionId}] OnSessionOpened");
            HandleUnsupportedProtocols(supportedProtocols);

            try
            {
                _handlersLock.TryEnterReadLock(-1);

                var handlers = HandlersByType.Values.ToList();

                // notify protocol handlers about new session
                foreach (var handler in handlers)
                {
                    handler.OnSessionOpened(requestedProtocols, supportedProtocols);
                }
            }
            finally
            {
                _handlersLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Called when the ETP session is closed.
        /// </summary>
        public override void OnSessionClosed()
        {
            Logger.Trace($"[{SessionId}] OnSessionClosed");

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
        }

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
        /// <returns>The message identifier.</returns>
        public long SendMessage<T>(IMessageHeader header, T body, Action<IMessageHeader> onBeforeSend = null)
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
        /// <returns>The message identifier.</returns>
        public async Task<long> SendMessageAsync<T>(IMessageHeader header, T body, Action<IMessageHeader> onBeforeSend = null) where T : ISpecificRecord
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
                    onBeforeSend?.Invoke(header);

                    // Log message just before it gets sent if needed.
                    Sending(header, body);

                    if (IsJsonEncoding)
                    {
                        var message = EtpExtensions.Serialize(new object[] {header, body});
                        await SendAsync(message).ConfigureAwait(CaptureAsyncContext);
                    }
                    else
                    {
                        var data = body.Encode(header, SupportedCompression);
                        await SendAsync(data, 0, data.Length).ConfigureAwait(CaptureAsyncContext);
                    }
                }
                finally
                {
                    _sendLock.Release();
                }
            }
            catch (Exception ex)
            {
                // Handler already locked by the calling code...
                return Handler(header.Protocol)
                    .ProtocolException((int)EtpErrorCodes.InvalidState, ex.Message, header.MessageId);
            }

            return header.MessageId;
        }

        /// <summary>
        /// Gets the supported protocols.
        /// </summary>
        /// <returns>A list of supported protocols.</returns>
        public IList<ISupportedProtocol> GetSupportedProtocols()
        {
            try
            {
                _handlersLock.TryEnterReadLock(-1);

                return Adapter.GetSupportedProtocols(HandlersByType.Values.ToList(), IsClient);
            }
            finally
            {
                _handlersLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Generates a new unique message identifier for the current session.
        /// </summary>
        /// <returns>The message identifier.</returns>
        public long NewMessageId()
        {
            return Interlocked.Increment(ref _messageId);
        }

        /// <summary>
        /// Closes the WebSocket connection for the specified reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        public void Close(string reason)
        {
            CloseAsync(reason).Wait();
        }

        /// <summary>
        /// Asynchronously closes the WebSocket connection for the specified reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        public async Task CloseAsync(string reason)
        {
            // Closing sends messages over the websocket so need to ensure no other messages are being sent when closing
            try
            {
                await _sendLock.WaitAsync().ConfigureAwait(CaptureAsyncContext);
                Logger.Trace($"Closing Session: {reason}");

                await CloseAsyncCore(reason).ConfigureAwait(CaptureAsyncContext);
            }
            finally
            {
                _sendLock.Release();
            }
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
        protected abstract Task CloseAsyncCore(string reason);

        /// <summary>
        /// Sends the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        protected abstract Task SendAsync(byte[] data, int offset, int length);

        /// <summary>
        /// Sends the specified messages.
        /// </summary>
        /// <param name="message">The message.</param>
        protected abstract Task SendAsync(string message);

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
                    Logger.VerboseFormat("[{0}] Binary message received: {1}", SessionId, EtpExtensions.Serialize(header));
                }

                Stream gzip = null;

                try
                {
                    // decompress message body if compression has been negotiated
                    if (header.CanCompressMessageBody(true))
                    {
                        if (EtpExtensions.GzipEncoding.Equals(SupportedCompression, StringComparison.InvariantCultureIgnoreCase))
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
                Logger.VerboseFormat("[{0}] JSON message received: {1}", SessionId, header);
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

                    var msg = $"Protocol handler not registered for protocol { header.Protocol }.";
                    handler.ProtocolException((int)EtpErrorCodes.UnsupportedProtocol, msg, header.MessageId);

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
                    handler.ProtocolException((int)EtpErrorCodes.InvalidState, ex.Message, header.MessageId);
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

            Logger.Debug(Log("[{0}] Protocol handler not registered for protocol {1}.", SessionId, protocol));
            throw new NotSupportedException($"Protocol handler not registered for protocol { protocol }.");
        }

        /// <summary>
        /// Handles the unsupported protocols.
        /// </summary>
        /// <param name="supportedProtocols">The supported protocols.</param>
        protected virtual void HandleUnsupportedProtocols(IList<ISupportedProtocol> supportedProtocols)
        {
            // remove unsupported handler mappings (excluding Core protocol)
            try
            {
                _handlersLock.TryEnterReadLock(-1);

                HandlersByType
                    .Where(x => x.Value.Protocol > 0 && !supportedProtocols.Contains(x.Value.Protocol, x.Value.Role))
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
                Log("[{0}] Sending message at {1}", SessionId, now.ToString(TimestampFormat));
                Log(EtpExtensions.Serialize(header));
                Log(EtpExtensions.Serialize(body, true));
            }

            if (Logger.IsVerboseEnabled())
            {
                Logger.VerboseFormat("[{0}] Sending message at {1}: {2}{3}{4}",
                    SessionId, now.ToString(TimestampFormat), EtpExtensions.Serialize(header), Environment.NewLine, EtpExtensions.Serialize(body, true));
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
                    SessionId, now.ToString(TimestampFormat), EtpExtensions.Serialize(header), Environment.NewLine, EtpExtensions.Serialize(message, true));
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
    }
}

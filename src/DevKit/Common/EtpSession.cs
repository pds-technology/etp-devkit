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

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using Avro.IO;
using Avro.Specific;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Properties;
using Newtonsoft.Json.Linq;

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
        private readonly object _sendLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpSession"/> class.
        /// </summary>
        /// <param name="application">The application name.</param>
        /// <param name="version">The application version.</param>
        /// <param name="headers">The WebSocket or HTTP headers.</param>
        protected EtpSession(string application, string version, IDictionary<string, string> headers)
        {
            Headers = headers ?? new Dictionary<string, string>();
            Handlers = new Dictionary<object, IProtocolHandler>();
            ApplicationName = application;
            ApplicationVersion = version;
            ValidateHeaders();
        }

        /// <summary>
        /// Gets the version specific ETP adapter.
        /// </summary>
        public IEtpAdapter Adapter { get; private set; }

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
        protected IDictionary<string, string> Headers { get; }

        /// <summary>
        /// Gets the collection of registered protocol handlers.
        /// </summary>
        /// <value>The handlers.</value>
        protected IDictionary<object, IProtocolHandler> Handlers { get; }

        /// <summary>
        /// Gets the registered protocol handler for the specified ETP interface.
        /// </summary>
        /// <typeparam name="T">The protocol handler interface.</typeparam>
        /// <returns>The registered protocol handler instance.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public T Handler<T>() where T : IProtocolHandler
        {
            IProtocolHandler handler;

            if (Handlers.TryGetValue(typeof(T), out handler) && handler is T)
            {
                return (T)handler;
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
            return Handlers.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Called when the ETP session is opened.
        /// </summary>
        /// <param name="requestedProtocols">The requested protocols.</param>
        /// <param name="supportedProtocols">The supported protocols.</param>
        public override void OnSessionOpened(IList<ISupportedProtocol> requestedProtocols, IList<ISupportedProtocol> supportedProtocols)
        {
            Logger.Trace($"OnSessionOpened");
            HandleUnsupportedProtocols(supportedProtocols);

            // notify protocol handlers about new session
            foreach (var item in Handlers)
            {
                if (item.Key is Type)
                    item.Value.OnSessionOpened(requestedProtocols, supportedProtocols);
            }
        }

        /// <summary>
        /// Called when the ETP session is closed.
        /// </summary>
        public override void OnSessionClosed()
        {
            Logger.Trace($"OnSessionClosed");
            // notify protocol handlers about closed session
            foreach (var item in Handlers)
            {
                if (item.Key is Type)
                    item.Value.OnSessionClosed();
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
        /// Sends the message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="header">The header.</param>
        /// <param name="body">The body.</param>
        /// <param name="onBeforeSend">Action called just before sending the message with the actual header having the definitive message ID.</param>
        /// <returns>The message identifier.</returns>
        public long SendMessage<T>(IMessageHeader header, T body, Action<IMessageHeader> onBeforeSend = null) where T : ISpecificRecord
        {
            try
            {
                // Lock to ensure only one thread at a time attempts to send data and to ensure that messages are sent with sequential IDs
                lock (_sendLock)
                {
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

                    if (IsJsonEncoding)
                    {
                        var message = this.Serialize(new object[] {header, body});
                        Send(message);
                    }
                    else
                    {
                        var data = body.Encode(header, SupportedCompression);
                        Send(data, 0, data.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                return Handler(header.Protocol)
                    .ProtocolException((int) EtpErrorCodes.InvalidState, ex.Message, header.MessageId);
            }

            Sent(header, body);

            return header.MessageId;
        }

        /// <summary>
        /// Gets the supported protocols.
        /// </summary>
        /// <param name="isSender">if set to <c>true</c> the current session is the sender.</param>
        /// <returns>A list of supported protocols.</returns>
        public IList<ISupportedProtocol> GetSupportedProtocols(bool isSender = false)
        {
            var supportedProtocols = new List<ISupportedProtocol>();

            // Skip Core protocol (0)
            foreach (var handler in Handlers.Values.Where(x => x.Protocol > 0))
            {
                var role = isSender ? handler.RequestedRole : handler.Role;

                if (supportedProtocols.Contains(handler.Protocol, role))
                    continue;

                supportedProtocols.Add(Adapter.GetSupportedProtocol(handler, role));
            }

            return supportedProtocols;
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
            // Closing sends messages over the websocket so need to ensure no other messages are being sent when closing
            lock (_sendLock)
            {
                Logger.Trace($"Closing Session: {reason}");

                CloseCore(reason);
            }
        }

        /// <summary>
        /// Registers the core server handler.
        /// </summary>
        /// <param name="etpSubProtocol">The ETP sub protocol.</param>
        public void RegisterCoreServer(string etpSubProtocol)
        {
            Adapter = ResolveEtpAdapter(etpSubProtocol);
            Adapter.RegisterCoreServer(this);
        }

        /// <summary>
        /// Registers the core client handler.
        /// </summary>
        /// <param name="etpSubProtocol">The ETP sub protocol.</param>
        protected void RegisterCoreClient(string etpSubProtocol)
        {
            Adapter = ResolveEtpAdapter(etpSubProtocol);
            Adapter.RegisterCoreClient(this);
        }

        /// <summary>
        /// Resolves the ETP adapter.
        /// </summary>
        /// <param name="etpSubProtocol">The ETP sub protocol.</param>
        /// <returns>A new <see cref="IEtpAdapter"/> instance.</returns>
        protected IEtpAdapter ResolveEtpAdapter(string etpSubProtocol)
        {
            if (EtpSettings.Etp12SubProtocol.Equals(etpSubProtocol, StringComparison.InvariantCultureIgnoreCase))
                return new v12.Etp12Adapter();

            return new v11.Etp11Adapter();
        }

        /// <summary>
        /// Closes the WebSocket connection for the specified reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        protected abstract void CloseCore(string reason);

        /// <summary>
        /// Sends the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        protected abstract void Send(byte[] data, int offset, int length);

        /// <summary>
        /// Sends the specified messages.
        /// </summary>
        /// <param name="message">The message.</param>
        protected abstract void Send(string message);

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
                    Logger.VerboseFormat("[{0}] Binary message received: {1}", SessionId, this.Serialize(header));
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
            if (Handlers.ContainsKey(header.Protocol))
            {
                var handler = Handler(header.Protocol);

                try
                {
                    // Handle global Acknowledge request
                    if (header.IsAcknowledgeRequested())
                    {
                        handler.Acknowledge(header.MessageId);
                    }

                    handler.HandleMessage(header, decoder, body);
                }
                catch (Exception ex)
                {
                    Logger.Debug(ex);
                    handler.ProtocolException((int)EtpErrorCodes.InvalidState, ex.Message, header.MessageId);
                }
            }
            else
            {
                var message = $"Protocol handler not registered for protocol { header.Protocol }.";

                Handler((int)v11.Protocols.Core)
                    .ProtocolException((int)EtpErrorCodes.UnsupportedProtocol, message, header.MessageId);
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
                Handlers[contractType] = handler;
                Handlers[handler.Protocol] = handler;
            }
        }

        /// <summary>
        /// Get the registered handler for the specified protocol.
        /// </summary>
        /// <param name="protocol">The protocol.</param>
        /// <returns>The registered protocol handler instance.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        protected IProtocolHandler Handler(int protocol)
        {
            if (Handlers.ContainsKey(protocol))
            {
                return Handlers[protocol];
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
            Handlers
                .Where(x => x.Value.Protocol > 0 && !supportedProtocols.Contains(x.Value.Protocol, x.Value.Role))
                .ToList()
                .ForEach(x =>
                {
                    x.Value.Session = null;
                    Handlers.Remove(x.Key);
                    Handlers.Remove(x.Value.Protocol);
                });

            // update remaining handler mappings by protocol
            foreach (var handler in Handlers.Values.ToArray())
            {
                if (!Handlers.ContainsKey(handler.Protocol))
                    Handlers[handler.Protocol] = handler;
            }
        }

        /// <summary>
        /// Logs the specified header and message body.
        /// </summary>
        /// <typeparam name="T">The type of message.</typeparam>
        /// <param name="header">The header.</param>
        /// <param name="body">The message body.</param>
        protected void Sent<T>(IMessageHeader header, T body)
        {
            var now = DateTime.Now;
            if (Output != null)
            {
                Log("[{0}] Message sent at {1}", SessionId, now.ToString(TimestampFormat));
                Log(this.Serialize(header));
                Log(this.Serialize(body, true));
            }

            if (Logger.IsVerboseEnabled())
            {
                Logger.VerboseFormat("[{0}] Message sent at {1}: {2}{3}{4}",
                    SessionId, now.ToString(TimestampFormat), this.Serialize(header), Environment.NewLine, this.Serialize(body, true));
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
                var handlers = Handlers
                    .Where(x => x.Key is int)
                    .Select(x => x.Value)
                    .OfType<IDisposable>();

                foreach (var handler in handlers)
                    handler.Dispose();
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

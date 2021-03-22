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
using System.Threading.Tasks;
using Avro.Specific;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Protocol.Core;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Defines the properties and methods needed to manage an ETP session.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IEtpSession : IDisposable, IEtpSessionCapabilitiesRegistrar
    {
        /// <summary>
        /// Gets the ETP version supported by this session.
        /// </summary>
        EtpVersion EtpVersion { get; }

        /// <summary>
        /// Gets the encoding used by this session (binary or json).
        /// </summary>
        EtpEncoding Encoding { get; }

        /// <summary>
        /// Gets the version specific ETP adapter.
        /// </summary>
        IEtpAdapter Adapter { get; }

        /// <summary>
        /// Gets whether or not this is the client side of the session.
        /// </summary>
        bool IsClient { get; }

        /// <summary>
        /// Gets a value indicating whether the underlying websocket connection is open.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the underlying websocket is open; otherwise, <c>false</c>.
        /// </value>
        bool IsWebSocketOpen { get; }

        /// <summary>
        /// Gets a value indicating whether the session is open.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the session is open; otherwise, <c>false</c>.
        /// </value>
        bool IsSessionOpen { get; }

        /// <summary>
        /// Gets a value indicating whether the synchronization context should be captured for async tasks.
        /// </summary>
        bool CaptureAsyncContext { get; }

        /// <summary>
        /// Gets this instance's info.
        /// </summary>
        EtpEndpointInfo InstanceInfo { get; }

        /// <summary>
        /// Gets the counterpart's info.
        /// </summary>
        EtpEndpointInfo CounterpartInfo { get; }

        /// <summary>
        /// Gets the client info.
        /// </summary>
        EtpEndpointInfo ClientInfo { get; }

        /// <summary>
        /// Gets the server info.
        /// </summary>
        EtpEndpointInfo ServerInfo { get; }

        /// <summary>
        /// Gets this instance's details.
        /// </summary>
        IEndpointDetails InstanceDetails { get; }

        /// <summary>
        /// Gets the counterpart's details.
        /// </summary>
        IEndpointDetails CounterpartDetails { get; }

        /// <summary>
        /// Gets the client details.
        /// </summary>
        IEndpointDetails ClientDetails { get; }

        /// <summary>
        /// Gets the server details.
        /// </summary>
        IEndpointDetails ServerDetails { get; }

        /// <summary>
        /// Gets the session key.
        /// </summary>
        /// <value>The session key.</value>
        string SessionKey { get; }

        /// <summary>
        /// Gets the session identifier.
        /// </summary>
        /// <value>The session identifier.</value>
        Guid SessionId { get; }

        /// <summary>
        /// Gets the collection of WebSocket or HTTP headers.
        /// </summary>
        /// <value>The headers.</value>
        IDictionary<string, string> Headers { get; }

        /// <summary>
        /// Maximum size allowed for a WebSocket frame (which is determined by the library you use to implement WebSocket). WebSocket is the transport protocol used by ETP.
        /// </summary>
        long SessionMaxWebSocketFramePayloadSize { get; }

        /// <summary>
        /// Maximum size allowed for a WebSocket message (which is composed of multiple WebSocket frames, which is determined by the library you use to implement WebSocket). WebSocket is the transport protocol used by ETP.
        /// </summary>
        long SessionMaxWebSocketMessagePayloadSize { get; }

        /// <summary>
        /// Gets or sets the negotiated list of supported protocols for this session.
        /// </summary>
        /// <value>The negotiated list of supported protocols for this session.</value>
        IReadOnlyDictionary<int, ISessionProtocol> SessionSupportedProtocols { get; }

        /// <summary>
        /// Gets or sets the negotiated list of supported data objects for this session.
        /// </summary>
        /// <value>The negotiated list of supported data objects for this session.</value>
        ISessionSupportedDataObjectCollection SessionSupportedDataObjects { get; }

        /// <summary>
        /// Gets or sets the negotiated compression type for this session.
        /// </summary>
        string SessionSupportedCompression { get; }

        /// <summary>
        /// Gets or sets the negotiated list of formats supported for this session.
        /// </summary>
        IReadOnlyList<string> SessionSupportedFormats { get; }

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
        /// Occurs when the WebSocket is opened.
        /// </summary>
        event EventHandler SocketOpened;

        /// <summary>
        /// Occurs when the WebSocket is closed.
        /// </summary>
        event EventHandler SocketClosed;

        /// <summary>
        /// Occurs when the WebSocket has an error.
        /// </summary>
        event EventHandler<ErrorEventArgs> SocketError;

        /// <summary>
        /// Event raised when the session is opened.
        /// </summary>
        event EventHandler<SessionOpenedEventArgs> SessionOpened;

        /// <summary>
        /// Sends an Acknowledge message in response to the message associated with the correlation header.
        /// </summary>
        /// <param name="protocol">The protocol to send the acknowledge message on.</param>
        /// <param name="correlatedHeader">The message header the acknowledge message is correlated with.</param>
        /// <param name="isNoData">Whether or not the acknowledge message should have the NoData flag set.</param>
        /// <param name="extension">The message header extension to send with the message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<IAcknowledge> Acknowledge(int protocol, IMessageHeader correlatedHeader, bool isNoData = false, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Constructs a new <see cref="IErrorInfo"/> instance compatible with the session.
        /// </summary>
        /// <returns>The constructed error info.</returns>
        IErrorInfo ErrorInfo();

        /// <summary>
        /// Sends a ProtocolException message with the specified exception details.
        /// </summary>
        /// <param name="exception">The ETP exception.</param>
        /// <param name="isFinalPart">Whether or not the protocol exception is the final part in a multi-part message.</param>
        /// <param name="extension">The message header extension to send with the message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<IProtocolException> ProtocolException(EtpException exception, bool isFinalPart = false, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a ProtocolException message with the specified exception details.
        /// </summary>
        /// <param name="protocol">The protocol to send the protocol exception on.</param>
        /// <param name="error">The error in the protocol exception.</param>
        /// <param name="correlatedHeader">The message header the protocol exception is correlated with, if any.</param>
        /// <param name="isFinalPart">Whether or not the protocol exception is the final part in a multi-part message.</param>
        /// <param name="extension">The message header extension to send with the message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<IProtocolException> ProtocolException(int protocol, IErrorInfo error, IMessageHeader correlatedHeader = null, bool isFinalPart = false, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a ProtocolException message(s) with the specified exception details.
        /// </summary>
        /// <param name="protocol">The protocol to send the protocol exception on.</param>
        /// <param name="errors">The errors in the protocol exception.</param>
        /// <param name="correlatedHeader">The message header the protocol exception is correlated with, if any.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="extension">The message header extension to send with the message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        EtpMessage<IProtocolException> ProtocolException(int protocol, IDictionary<string, IErrorInfo> errors, IMessageHeader correlatedHeader = null, bool setFinalPart = true, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a ProtocolException message with the specified exception details.
        /// </summary>
        /// <param name="protocol">The protocol to send the protocol exception on.</param>
        /// <param name="exception">The protocol exception body to send.</param>
        /// <param name="correlatedHeader">The message header the protocol exception is correlated with, if any.</param>
        /// <param name="isFinalPart">Whether or not the protocol exception is the final part in a multi-part message.</param>
        /// <param name="extension">The message header extension to send with the message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<IProtocolException> ProtocolException(int protocol, IProtocolException exception, IMessageHeader correlatedHeader = null, bool isFinalPart = false, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Occurs when a ProtocolException message is received for any protocol.
        /// </summary>
        event EventHandler<MessageEventArgs<IProtocolException>> OnProtocolException;

        /// <summary>
        /// Sends a CloseSession message to the session's counterpart.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ICloseSession> CloseSession(string reason = null, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Event raised when the session is closed.
        /// </summary>
        event EventHandler<SessionClosedEventArgs> SessionClosed;

        /// <summary>
        /// Event raised when binary WebSocket data is received.
        /// </summary>
        event EventHandler<DataReceivedEventArgs> DataReceived;

        /// <summary>
        /// Event raised when text WebSocket data is received.
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// Synchronously sends the message.
        /// </summary>
        /// <typeparam name="T">The type of the message body</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="onBeforeSend">Action called just before sending the message with the actual header having the definitive message ID.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<T> SendMessage<T>(EtpMessage<T> message, Action<EtpMessage<T>> onBeforeSend = null) where T : ISpecificRecord;

        /// <summary>
        /// Asynchronously sends the message.
        /// </summary>
        /// <typeparam name="T">The type of the message body</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="onBeforeSend">Action called just before sending the message with the actual header having the definitive message ID.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        Task<EtpMessage<T>> SendMessageAsync<T>(EtpMessage<T> message, Action<EtpMessage<T>> onBeforeSend = null) where T : ISpecificRecord;

        /// <summary>
        /// Gets the registered handler for the specified protocol.
        /// </summary>
        /// <param name="protocol">The protocol.</param>
        /// <returns>The registered protocol handler instance.</returns>
        IProtocolHandler Handler(int protocol);

        /// <summary>
        /// Gets the registered protocol handler for the specified ETP interface.
        /// </summary>
        /// <typeparam name="T">The protocol handler interface.</typeparam>
        /// <returns>The registered protocol handler instance.</returns>
        T Handler<T>() where T : IProtocolHandler;

        /// <summary>
        /// Determines whether this instance can handle the specified protocol.
        /// </summary>
        /// <typeparam name="T">The protocol handler interface.</typeparam>
        /// <returns><c>true</c> if the specified protocol handler has been registered; otherwise, <c>false</c>.</returns>
        bool CanHandle<T>() where T : IProtocolHandler;

        /// <summary>
        /// Closes the WebSocket connection for the specified reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        void CloseWebSocket(string reason);

        /// <summary>
        /// Asynchronously closes the WebSocket connection for the specified reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        Task CloseWebSocketAsync(string reason);

        /// <summary>
        /// Sets the context object of type <typeparamref name="T"/> for this session.
        /// </summary>
        /// <typeparam name="T">The type of the context object.</typeparam>
        /// <param name="context">The context object.</param>
        void SetContext<T>(T context);

        /// <summary>
        /// Getsets the context object of type <typeparamref name="T"/> for this session.
        /// </summary>
        /// <typeparam name="T">The type of the context object.</typeparam>
        /// <returns>The context object.</returns>
        T GetContext<T>();
    }
}

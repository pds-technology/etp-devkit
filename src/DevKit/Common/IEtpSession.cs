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
using System.Threading.Tasks;
using Avro.Specific;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Datatypes.Object;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Defines the properties and methods needed to manage an ETP session.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IEtpSession : IDisposable
    {
        /// <summary>
        /// Gets the ETP version supported by this session.
        /// </summary>
        EtpVersion SupportedVersion { get; }

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
        ///   <c>true</c> if this instance is open; otherwise, <c>false</c>.
        /// </value>
        bool IsOpen { get; }

        /// <summary>
        /// Gets the name of the client application.
        /// </summary>
        /// <value>The name of the client application.</value>
        string ClientApplicationName { get; }

        /// <summary>
        /// Gets the client application version.
        /// </summary>
        /// <value>The client application version.</value>
        string ClientApplicationVersion { get; }

        /// <summary>
        /// Gets or sets the client instance identifier.
        /// </summary>
        /// <value>The client instance identifier.</value>
        string ClientInstanceId { get; }

        /// <summary>
        /// Gets the name of the server application.
        /// </summary>
        /// <value>The name of the server application.</value>
        string ServerApplicationName { get; }

        /// <summary>
        /// Gets the server application version.
        /// </summary>
        /// <value>The server application version.</value>
        string ServerApplicationVersion { get; }

        /// <summary>
        /// Gets or sets the server instance identifier.
        /// </summary>
        /// <value>The server instance identifier.</value>
        string ServerInstanceId { get; }

        /// <summary>
        /// Gets the instance key.
        /// </summary>
        /// <value>The instance key, which is used to generate the client or server instance identifier.</value>
        string InstanceKey { get; }

        /// <summary>
        /// Gets the session key.
        /// </summary>
        /// <value>The session key.</value>
        string SessionKey { get; }

        /// <summary>
        /// Gets the session identifier.
        /// </summary>
        /// <value>The session identifier.</value>
        string SessionId { get; }

        /// <summary>
        /// This is the largest data object the store or customer can get or put. A store or customer can optionally specify these for protocols that handle data objects.
        /// </summary>
        long InstanceMaxDataObjectSize { get; }

        /// <summary>
        /// This is the largest part size the store or customer can get or put. A store can optionally specify this for protocols that handle object parts. Property of numberofbytes.
        /// </summary>
        long InstanceMaxPartSize { get; }

        /// <summary>
        /// Maximum time interval between subsequent messages in the SAME multipart request or response.
        /// </summary>
        long InstanceMaxMultipartMessageTimeInterval { get; }

        /// <summary>
        /// Maximum size allowed for a WebSocket frame (which is determined by the library you use to implement WebSocket). WebSocket is the transport protocol used by ETP.
        /// </summary>
        long InstanceMaxWebSocketFramePayloadSize { get; }

        /// <summary>
        /// Maximum size allowed for a WebSocket message (which is composed of multiple WebSocket frames, which is determined by the library you use to implement WebSocket). WebSocket is the transport protocol used by ETP.
        /// </summary>
        long InstanceMaxWebSocketMessagePayloadSize { get; }

        /// <summary>
        /// This is the largest part size the store or customer can get or put. A store can optionally specify this for protocols that handle object parts. Property of numberofbytes.
        /// </summary>
        bool InstanceSupportsAlternateRequestUris { get; }

        /// <summary>
        /// This is the largest data object the store or customer can get or put. A store or customer can optionally specify these for protocols that handle data objects.
        /// </summary>
        long CounterpartMaxDataObjectSize { get; }

        /// <summary>
        /// This is the largest part size the store or customer can get or put. A store can optionally specify this for protocols that handle object parts. Property of numberofbytes.
        /// </summary>
        long CounterpartMaxPartSize { get; }

        /// <summary>
        /// Maximum time interval between subsequent messages in the SAME multipart request or response.
        /// </summary>
        long CounterpartMaxMultipartMessageTimeInterval { get; }

        /// <summary>
        /// Maximum size allowed for a WebSocket frame (which is determined by the library you use to implement WebSocket). WebSocket is the transport protocol used by ETP.
        /// </summary>
        long CounterpartMaxWebSocketFramePayloadSize { get; }

        /// <summary>
        /// Maximum size allowed for a WebSocket message (which is composed of multiple WebSocket frames, which is determined by the library you use to implement WebSocket). WebSocket is the transport protocol used by ETP.
        /// </summary>
        long CounterpartMaxWebSocketMessagePayloadSize { get; }

        /// <summary>
        /// This is the largest part size the store or customer can get or put. A store can optionally specify this for protocols that handle object parts. Property of numberofbytes.
        /// </summary>
        bool CounterpartSupportsAlternateRequestUris { get; }

        /// <summary>
        /// Gets the protocols supported by this instance.
        /// </summary>
        /// <returns>A list of protocols supported by this instance.</returns>
        IReadOnlyList<EtpSessionProtocol> InstanceSupportedProtocols { get; }

        /// <summary>
        /// Gets or sets the types of data objects supported by this instance.
        /// </summary>
        /// <returns>A list of data object types supported by this instance.</returns>
        IList<IDataObjectType> InstanceSupportedDataObjects { get; set; }

        /// <summary>
        /// Gets the types of compression supported by this instance.
        /// </summary>
        /// <returns>A list of compression types supported by this instance.</returns>
        IList<string> InstanceSupportedCompression { get; set; }

        /// <summary>
        /// Gets the formats supported by this instance.
        /// </summary>
        /// <returns>A list of formats supported by this instance.</returns>
        IList<string> InstanceSupportedFormats { get; set; }

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
        IReadOnlyList<EtpSessionProtocol> SessionSupportedProtocols { get; }

        /// <summary>
        /// Gets or sets the negotiated list of supported data objects for this session.
        /// </summary>
        /// <value>The negotiated list of supported data objects for this session.</value>
        IReadOnlyList<IDataObjectType> SessionSupportedDataObjects { get; }

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
        event EventHandler<Exception> SocketError;

        /// <summary>
        /// Initializes the instance capabilities supported by the session.
        /// </summary>
        /// <param name="capabilities">The instances's capabilities.</param>
        void InitializeInstanceCapabilities(EtpEndpointCapabilities capabilities);

        /// <summary>
        /// Gets the capabilities supported by the session.
        /// </summary>
        /// <param name="capabilities">The instances's capabilities.</param>
        void GetInstanceCapabilities(EtpEndpointCapabilities capabilities);

        /// <summary>
        /// Initialize the set of supported protocols from the registered handlers.
        /// </summary>
        void InitializeInstanceSupportedProtocols();

        /// <summary>
        /// Initialize the session based on details from the counterpart.
        /// After this, the protocols, objects, compression, formats and capabilities that will be used in the session will be initialized.
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <param name="counterpartApplicationName">The counterpart's application name.</param>
        /// <param name="counterpartApplicationVersion">The counterpart's application version.</param>
        /// <param name="counterpartInstanceId">The counterpart's instance ID.</param>
        /// <param name="counterpartSupportedProtocols">The counterpart's supported protocols.</param>
        /// <param name="counterpartSupportedDataObjects">The counterpart's supported objects.</param>
        /// <param name="counterpartSupportedCompression">The counterpart's supported compression.</param>
        /// <param name="counterpartSupportedFormats">The counterpart's supported formats.</param>
        /// <param name="counterpartEndpointCapabilities">The counterpart's endpoint capabilities.</param>
        /// <returns><c>true</c> if the session was successfully initialized; <c>false</c> otherwise.</returns>
        bool InitializeSession(string sessionId, string counterpartApplicationName, string counterpartApplicationVersion, string counterpartInstanceId, IReadOnlyList<ISupportedProtocol> counterpartSupportedProtocols, IReadOnlyList<IDataObjectType> counterpartSupportedDataObjects, IReadOnlyList<string> counterpartSupportedCompression, IReadOnlyList<string> counterpartSupportedFormats, EtpEndpointCapabilities counterpartEndpointCapabilities);

        /// <summary>
        /// Called when the ETP session is opened.
        /// </summary>
        /// <param name="openedSuccessfully"><c>true</c> if the session opened without errors; <c>false</c> if there were errors when opening the session.</param>
        void OnSessionOpened(bool openedSuccessfully);

        /// <summary>
        /// Event raised when the session is opened with a paremeters indicating whether there were errors when opening the session and the list of supported protocols for the session.
        /// </summary>
        event Action<bool> SessionOpened;

        /// <summary>
        /// Called when the ETP session is closed.
        /// </summary>
        /// <param name="closedSuccessfully"><c>true</c> if the session closed without errors; <c>false</c> if there were errors when closing the session.</param>
        void OnSessionClosed(bool closedSuccessfully);

        /// <summary>
        /// Event raised when the session is closed with a paremeter indicating whether there were errors when closing the session.
        /// </summary>
        event Action<bool> SessionClosed;

        /// <summary>
        /// Called when WebSocket data is received.
        /// </summary>
        /// <param name="data">The data.</param>
        void OnDataReceived(byte[] data);

        /// <summary>
        /// Called when a WebSocket message is received.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnMessageReceived(string message);

        /// <summary>
        /// Synchronously sends the message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="header">The header.</param>
        /// <param name="body">The body.</param>
        /// <param name="onBeforeSend">Action called just before sending the message with the actual header having the definitive message ID.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long SendMessage<T>(IMessageHeader header, T body, Action<IMessageHeader, T> onBeforeSend = null) where T : ISpecificRecord;

        /// <summary>
        /// Asynchronously sends the message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="header">The header.</param>
        /// <param name="body">The body.</param>
        /// <param name="onBeforeSend">Action called just before sending the message with the actual header having the definitive message ID.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        Task<long> SendMessageAsync<T>(IMessageHeader header, T body, Action<IMessageHeader, T> onBeforeSend = null) where T : ISpecificRecord;

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
        /// Registers a protocol handler for the specified contract type.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <typeparam name="THandler">The type of the handler.</typeparam>
        void Register<TContract, THandler>() where TContract : IProtocolHandler where THandler : TContract;

        /// <summary>
        /// Registers a protocol handler factory for the specified contract type.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="factory">The factory.</param>
        void Register<TContract>(Func<TContract> factory) where TContract : IProtocolHandler;

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

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
        /// Gets the name of the application.
        /// </summary>
        /// <value>The name of the application.</value>
        string ApplicationName { get; }

        /// <summary>
        /// Gets the application version.
        /// </summary>
        /// <value>The application version.</value>
        string ApplicationVersion { get; }

        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        /// <value>The session identifier.</value>
        string SessionId { get; set; }

        /// <summary>
        /// Gets or sets the supported compression type.
        /// </summary>
        string SupportedCompression { get; set; }

        /// <summary>
        /// Gets or sets the list of supported objects.
        /// </summary>
        /// <value>The supported objects.</value>
        IList<string> SupportedObjects { get; set; }

        /// <summary>
        /// Gets the collection of WebSocket or HTTP headers.
        /// </summary>
        /// <value>The headers.</value>
        IDictionary<string, string> Headers { get; }

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
        /// Called when the ETP session is opened.
        /// </summary>
        /// <param name="requestedProtocols">The requested protocols.</param>
        /// <param name="supportedProtocols">The supported protocols.</param>
        void OnSessionOpened(IList<ISupportedProtocol> requestedProtocols, IList<ISupportedProtocol> supportedProtocols);

        /// <summary>
        /// Called when the ETP session is closed.
        /// </summary>
        void OnSessionClosed();

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
        /// <returns>The message identifier.</returns>
        long SendMessage<T>(IMessageHeader header, T body, Action<IMessageHeader> onBeforeSend = null) where T : ISpecificRecord;

        /// <summary>
        /// Asynchronously sends the message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="header">The header.</param>
        /// <param name="body">The body.</param>
        /// <param name="onBeforeSend">Action called just before sending the message with the actual header having the definitive message ID.</param>
        /// <returns>The message identifier.</returns>
        Task<long> SendMessageAsync<T>(IMessageHeader header, T body, Action<IMessageHeader> onBeforeSend = null) where T : ISpecificRecord;

        /// <summary>
        /// Gets the supported protocols.
        /// </summary>
        /// <returns>A list of supported protocols.</returns>
        IList<ISupportedProtocol> GetSupportedProtocols();

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
        void Close(string reason);

        /// <summary>
        /// Asynchronously closes the WebSocket connection for the specified reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        Task CloseAsync(string reason);

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
    }
}

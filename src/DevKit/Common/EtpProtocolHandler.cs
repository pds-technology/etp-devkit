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
using Avro.Specific;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Protocol.Core;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Provides common functionality for ETP protocol handlers.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.EtpBase" />
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    public abstract class EtpProtocolHandler : EtpBase, IProtocolHandler
    {
        private readonly Dictionary<long, Action<IMessageHeader, ISpecificRecord>> MessageHandlers = new Dictionary<long, Action<IMessageHeader, ISpecificRecord>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpProtocolHandler"/> class.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <param name="protocol">The protocol.</param>
        /// <param name="role">The role.</param>
        /// <param name="requestedRole">The requested role.</param>
        protected EtpProtocolHandler(EtpVersion version, int protocol, string role, string requestedRole) : base(false)
        {
            SupportedVersion = version;
            Protocol = protocol;
            Role = role;
            RequestedRole = requestedRole;

            RegisterMessageHandler<IProtocolException>(v11.Protocols.Core, v11.MessageTypes.Core.ProtocolException, HandleProtocolException);
            RegisterMessageHandler<IAcknowledge>(v11.Protocols.Core, v11.MessageTypes.Core.Acknowledge, HandleAcknowledge);
        }

        /// <summary>
        /// The ETP version supported by this handler.
        /// </summary>
        public EtpVersion SupportedVersion { get; }

        /// <summary>
        /// Gets or sets the ETP session.
        /// </summary>
        /// <value>The session.</value>
        public IEtpSession Session { get; set; }

        /// <summary>
        /// Gets the protocol.
        /// </summary>
        /// <value>The protocol.</value>
        public int Protocol { get; }

        /// <summary>
        /// Gets the role.
        /// </summary>
        /// <value>The role.</value>
        public string Role { get; }

        /// <summary>
        /// Gets the requested role.
        /// </summary>
        /// <value>The requested role.</value>
        public string RequestedRole { get; }

        /// <summary>
        /// Gets the capabilities supported by the protocol handler.
        /// </summary>
        /// <returns>A collection of protocol capabilities.</returns>
        public virtual IDictionary<string, IDataValue> GetCapabilities()
        {
            return new Dictionary<string, IDataValue>();
        }

        /// <summary>
        /// Sends an Acknowledge message with the specified correlation identifier and message flag.
        /// </summary>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        public virtual long Acknowledge(long correlationId, MessageFlags messageFlag = MessageFlags.None)
        {
            var header = CreateMessageHeader(Protocol, v11.MessageTypes.Core.Acknowledge, correlationId, messageFlag);
            var acknowledge = Session.Adapter.CreateAcknowledge();

            return Session.SendMessage(header, acknowledge);
        }

        /// <summary>
        /// Sends a ProtocolException message with the specified error code, message and correlation identifier.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <returns>The message identifier.</returns>
        public virtual long ProtocolException(int errorCode, string errorMessage, long correlationId = 0)
        {
            var header = CreateMessageHeader(Protocol, v11.MessageTypes.Core.ProtocolException, correlationId);

            var error = Session.Adapter.CreateProtocolException();

            error.ErrorCode = errorCode;
            error.ErrorMessage = errorMessage;

            return Session.SendMessage(header, error);
        }

        /// <summary>
        /// Occurs when an Acknowledge message is received for the current protocol.
        /// </summary>
        public event ProtocolEventHandler<IAcknowledge> OnAcknowledge;

        /// <summary>
        /// Occurs when a ProtocolException message is received for the current protocol.
        /// </summary>
        public event ProtocolEventHandler<IProtocolException> OnProtocolException;

        /// <summary>
        /// Decodes the message based on the message type contained in the specified <see cref="IMessageHeader" />.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="body">The message body.</param>
        public void HandleMessage(IMessageHeader header, ISpecificRecord body)
        {
            if (!InvokeMessageHandler(header, body))
                this.InvalidMessage(header);

            // Additional processing if this message was the final message in response to a request:
            if (header.IsFinalResponse())
                HandleFinalResponse(header.CorrelationId);
        }

        /// <summary>
        /// Handles the Acknowledge message.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="acknowledge">The Acknowledge message.</param>
        protected virtual void HandleAcknowledge(IMessageHeader header, IAcknowledge acknowledge)
        {
            Notify(OnAcknowledge, header, acknowledge);
        }

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="protocolException">The ProtocolException message.</param>
        protected virtual void HandleProtocolException(IMessageHeader header, IProtocolException protocolException)
        {
            Notify(OnProtocolException, header, protocolException);
            Logger.DebugFormat("[{0}] Protocol exception: {1} - {2}", Session.SessionId, protocolException.ErrorCode, protocolException.ErrorMessage);
        }

        /// <summary>
        /// Handle any final cleanup related to the final message in response to a request.
        /// </summary>
        /// <param name="correlationId">The correlation ID of the request</param>
        protected virtual void HandleFinalResponse(long correlationId)
        {
        }

        /// <summary>
        /// Notifies subscribers of the specified event handler.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="handler">The message handler.</param>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message body.</param>
        /// <returns>The protocol event args.</returns>
        protected ProtocolEventArgs<T> Notify<T>(ProtocolEventHandler<T> handler, IMessageHeader header, T message) where T : ISpecificRecord
        {
            var args = new ProtocolEventArgs<T>(header, message);
            handler?.Invoke(this, args);
            return args;
        }

        /// <summary>
        /// Notifies subscribers of the specified event handler.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <typeparam name="TContext">The type of the context.</typeparam>
        /// <param name="handler">The message handler.</param>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message body.</param>
        /// <param name="context">The message context.</param>
        /// <returns>The protocol event args.</returns>
        protected ProtocolEventArgs<T, TContext> Notify<T, TContext>(ProtocolEventHandler<T, TContext> handler, IMessageHeader header, T message, TContext context) where T : ISpecificRecord
        {
            var args = new ProtocolEventArgs<T, TContext>(header, message, context);
            handler?.Invoke(this, args);
            return args;
        }

        /// <summary>
        /// Creates a message header for the specified protocol, message type, correlation identifier and message flag.
        /// </summary>
        /// <typeparam name="TProtocol">The protocol enum.</typeparam>
        /// <typeparam name="TMessageType">The message type enum.</typeparam>
        /// <param name="protocol">The protocol.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="messageFlags">The message flags.</param>
        /// <returns>A new message header instance.</returns>
        protected IMessageHeader CreateMessageHeader<TProtocol, TMessageType>(TProtocol protocol, TMessageType messageType, long correlationId = 0, MessageFlags messageFlags = MessageFlags.None) where TProtocol : IConvertible where TMessageType : IConvertible
        {
            return CreateMessageHeader(Convert.ToInt32(protocol), Convert.ToInt32(messageType), correlationId, messageFlags);
        }

        /// <summary>
        /// Creates a message header for the specified protocol, message type, correlation identifier and message flag.
        /// </summary>
        /// <typeparam name="TMessageType">The message type enum.</typeparam>
        /// <param name="protocol">The protocol.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="messageFlags">The message flags.</param>
        /// <returns>A new message header instance.</returns>
        protected IMessageHeader CreateMessageHeader<TMessageType>(int protocol, TMessageType messageType, long correlationId = 0, MessageFlags messageFlags = MessageFlags.None) where TMessageType : IConvertible
        {
            return CreateMessageHeader(protocol, Convert.ToInt32(messageType), correlationId, messageFlags);
        }

        /// <summary>
        /// Creates a message header for the specified protocol, message type, correlation identifier and message flag.
        /// </summary>
        /// <param name="protocol">The protocol.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="messageFlags">The message flags.</param>
        /// <returns>A new message header instance.</returns>
        protected IMessageHeader CreateMessageHeader(int protocol, int messageType, long correlationId = 0, MessageFlags messageFlags = MessageFlags.None)
        {
            var header = Session.Adapter.CreateMessageHeader();

            header.Protocol = protocol;
            header.MessageType = messageType;
            header.MessageId = 0; // MessageId needs to be set just before sending to ensure proper sequencing
            header.MessageFlags = (int) messageFlags;
            header.CorrelationId = correlationId;

            return header;
        }

        /// <summary>
        /// Invokes the message handler for the specified message type.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="body">The message body.</param>
        /// <returns><c>true</c> if a handler for the message was invoked; <c>false</c> otherwise.</returns>
        public bool InvokeMessageHandler(IMessageHeader header, ISpecificRecord body)
        {
            if (header == null)
                return false;

            var messageKey = EtpExtensions.CreateMessageKey(header.Protocol, header.MessageType);

            Action<IMessageHeader, ISpecificRecord> messageHandler;
            if (!MessageHandlers.TryGetValue(messageKey, out messageHandler))
                return false;

            messageHandler(header, body);
            return true;
        }

        /// <summary>
        /// Registers a handler for the specific protocol message.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="protocol">The message protocol.</param>
        /// <param name="messageType">The type of the message.</param>
        /// <param name="messageHandler">The protocol message handler.</param>
        /// <remarks>If more than one handler is registered for the same protocol message, the last registered handler will be used.</remarks>
        public void RegisterMessageHandler<T>(object protocol, object messageType, Action<IMessageHeader, T> messageHandler) where T : ISpecificRecord
        {
            var messageKey = EtpExtensions.CreateMessageKey(Convert.ToInt32(protocol), Convert.ToInt32(messageType));

            MessageHandlers[messageKey] = (h, b) => messageHandler(h, (T)b);
        }
    }
}

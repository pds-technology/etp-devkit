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
using log4net;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Provides common functionality for ETP protocol handlers.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.EtpBase" />
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    public abstract class EtpProtocolHandler : IProtocolHandler
    {
        private readonly Dictionary<long, Action<IMessageHeader, ISpecificRecord>> MessageHandlers = new Dictionary<long, Action<IMessageHeader, ISpecificRecord>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpProtocolHandler"/> class.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <param name="protocol">The protocol.</param>
        /// <param name="role">This handler's role in the protocol.</param>
        /// <param name="counterpartRole">The role for this handler's counterpart in the protocol.</param>
        protected EtpProtocolHandler(EtpVersion version, int protocol, string role, string counterpartRole)
        {
            SupportedVersion = version;
            Protocol = protocol;
            Role = role;
            CounterpartRole = counterpartRole;
            Logger = LogManager.GetLogger(GetType());

            RegisterMessageHandler<IProtocolException>(v11.Protocols.Core, v11.MessageTypes.Core.ProtocolException, HandleProtocolException);
            RegisterMessageHandler<IAcknowledge>(v11.Protocols.Core, v11.MessageTypes.Core.Acknowledge, HandleAcknowledge);
        }

        /// <summary>
        /// Gets the logger used by this instance.
        /// </summary>
        /// <value>The logger instance.</value>
        public ILog Logger { get; private set; }

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
        /// Gets this handler's role in the protocol.
        /// </summary>
        /// <value>This handler's role in the protocol.</value>
        public string Role { get; }

        /// <summary>
        /// Gets the role for this handler's counterpart in the protocol.
        /// </summary>
        /// <value>The role for this handler's counterpart in the protocol.</value>
        public string CounterpartRole { get; }

        /// <summary>
        /// Gets the capabilities supported by the protocol handler.
        /// </summary>
        /// <param name="capabilities">The protocol's capabilities.</param>
        public virtual void GetCapabilities(EtpProtocolCapabilities capabilities)
        {

        }

        /// <summary>
        /// Called when the ETP session is opened.
        /// </summary>
        /// <param name="counterpartCapabilities">The counterpart's protocol capabilities.</param>
        public virtual void OnSessionOpened(EtpProtocolCapabilities counterpartCapabilities)
        {
        }

        /// <summary>
        /// Called when the ETP session is closed.
        /// </summary>
        public virtual void OnSessionClosed()
        {
        }

        /// <summary>
        /// Sends an Acknowledge message with the specified correlation identifier and message flag.
        /// </summary>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long Acknowledge(long correlationId, MessageFlags messageFlag = MessageFlags.None)
        {
            var header = CreateMessageHeader(Protocol, v11.MessageTypes.Core.Acknowledge, correlationId, messageFlag);
            var acknowledge = Session.Adapter.CreateAcknowledge();

            return Session.SendMessage(header, acknowledge);
        }

        /// <summary>
        /// Sends a ProtocolException message with the specified exception details.
        /// </summary>
        /// <param name="exception">The ETP exception.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long ProtocolException(EtpException exception)
        {
            var header = CreateMessageHeader(Protocol, v11.MessageTypes.Core.ProtocolException, exception.CorrelationId);

            var error = Session.Adapter.CreateProtocolException(exception.ErrorInfo);

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
            Logger.DebugFormat("[{0}] Protocol exception: {1} - {2}", Session.SessionKey, protocolException.ErrorCode, protocolException.ErrorMessage);
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
        /// Notifies subscribers of the specified event handler.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <typeparam name="TErrorInfo">The error info type.</typeparam>
        /// <param name="handler">The message handler.</param>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message body.</param>
        /// <param name="errors">The errors, if any.</param>
        /// <returns>The protocol event args.</returns>
        protected ProtocolEventWithErrorsArgs<T, TErrorInfo> Notify<T, TErrorInfo>(ProtocolEventWithErrorsHandler<T, TErrorInfo> handler, IMessageHeader header, T message, IDictionary<string, TErrorInfo> errors)
            where T : ISpecificRecord
            where TErrorInfo : IErrorInfo
        {
            var args = new ProtocolEventWithErrorsArgs<T, TErrorInfo>(header, message, errors);
            handler?.Invoke(this, args);
            return args;
        }

        /// <summary>
        /// Notifies subscribers of the specified event handler.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <typeparam name="TContext">The type of the context.</typeparam>
        /// <typeparam name="TErrorInfo">The error info type.</typeparam>
        /// <param name="handler">The message handler.</param>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message body.</param>
        /// <param name="context">The message context.</param>
        /// <param name="errors">The errors, if any.</param>
        /// <returns>The protocol event args.</returns>
        protected ProtocolEventWithErrorsArgs<T, TContext, TErrorInfo> Notify<T, TContext, TErrorInfo>(ProtocolEventWithErrorsHandler<T, TContext, TErrorInfo> handler, IMessageHeader header, T message, IDictionary<string, TContext> context, IDictionary<string, TErrorInfo> errors)
            where T : ISpecificRecord
            where TErrorInfo : IErrorInfo
        {
            var args = new ProtocolEventWithErrorsArgs<T, TContext, TErrorInfo>(header, message, context, errors);
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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        private bool _disposedValue; // To detect redundant calls

        /// <summary>
        /// Checks whether the current instance has been disposed.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException"></exception>
        protected virtual void CheckDisposed()
        {
            if (_disposedValue)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                Logger.Trace($"Disposing {GetType().Name}");

                _disposedValue = true;
            }
        }
    }
}

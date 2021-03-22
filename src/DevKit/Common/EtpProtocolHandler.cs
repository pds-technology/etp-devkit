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
using System.Threading;
using System.Threading.Tasks;
using Avro.Specific;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Protocol.Core;
using log4net;
using Nito.AsyncEx;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Provides common functionality for ETP protocol handlers.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    public abstract class EtpProtocolHandler : IProtocolHandler
    {
        private readonly Dictionary<long, Action<EtpMessage>> MessageHandlers = new Dictionary<long, Action<EtpMessage>>();
        private Task _backgroundCleanUpTask;
        private CancellationTokenSource _source = new CancellationTokenSource();

        /// <summary>
        /// Initializes a new instance of the <see cref="EtpProtocolHandler"/> class.
        /// </summary>
        /// <param name="version">The ETP version.</param>
        /// <param name="protocol">The protocol.</param>
        /// <param name="role">This handler's role in the protocol.</param>
        /// <param name="counterpartRole">The role for this handler's counterpart in the protocol.</param>
        /// <param name="capabilities">The capabilities to use.</param>
        protected EtpProtocolHandler(EtpVersion version, int protocol, string role, string counterpartRole, EtpProtocolCapabilities capabilities = null)
        {
            EtpVersion = version;
            Protocol = protocol;
            Role = role;
            CounterpartRole = counterpartRole;
            Logger = LogManager.GetLogger(GetType());
            Capabilities = capabilities ?? new EtpProtocolCapabilities(version);

            RegisterMessageHandler<IProtocolException>(Protocols.Core, MessageTypes.Core.ProtocolException, HandleProtocolException);
            RegisterMessageHandler<IAcknowledge>(Protocols.Core, MessageTypes.Core.Acknowledge, HandleAcknowledge);
        }

        /// <summary>
        /// Gets the logger used by this instance.
        /// </summary>
        /// <value>The logger instance.</value>
        public ILog Logger { get; private set; }

        /// <summary>
        /// The ETP version supported by this handler.
        /// </summary>
        public EtpVersion EtpVersion { get; }

        /// <summary>
        /// Gets or sets the ETP session.
        /// </summary>
        /// <value>The session.</value>
        public IEtpSession Session { get; private set; }

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
        /// Gets this instances capabilities for this protocol.
        /// </summary>
        public EtpProtocolCapabilities Capabilities { get; }

        /// <summary>
        /// Gets the counterpart's capabilities for this protocol.
        /// </summary>
        IProtocolCapabilities IEndpointProtocol.Capabilities => Capabilities;

        /// <summary>
        /// Gets the counterpart's capabilities for this protocol.
        /// </summary>
        public IProtocolCapabilities CounterpartCapabilities { get; private set; }

        /// <summary>
        /// Gets the registered messagesfor this handler.
        /// </summary>
        public ConcurrentDictionary<long, EtpMessage> RegisteredMessages { get; } = new ConcurrentDictionary<long, EtpMessage>();

        /// <summary>
        /// Gets the registered requests for this handler.
        /// </summary>
        public ConcurrentDictionary<Guid, EtpMessage> RegisteredRequestsByUuid { get; } = new ConcurrentDictionary<Guid, EtpMessage>();

        /// <summary>
        /// Gets the registered subscriptions for this handler.
        /// </summary>
        public ConcurrentDictionary<Guid, EtpSubscription> RegisteredSubscriptions { get; } = new ConcurrentDictionary<Guid, EtpSubscription>();

        /// <summary>
        /// Gets the registered subscriptions for this handler by message correlation ID.
        /// </summary>
        public ConcurrentDictionary<long, EtpSubscription> RegisteredSubscriptionsByCorrelationId { get; } = new ConcurrentDictionary<long, EtpSubscription>();

        /// <summary>
        /// Creates a deep copy of this instance.
        /// </summary>
        /// <returns>A deep copy of this instance.</returns>
        public IProtocolHandler Clone()
        {
            var clone = CreateInstanceClone();
            clone.Capabilities.LoadFrom(Capabilities);

            return clone;
        }

        /// <summary>
        /// Creates a new instance of this type with the same ETP Version, Protocol, Role and CounterpartRole.
        /// </summary>
        /// <returns>The new instance.</returns>
        protected virtual EtpProtocolHandler CreateInstanceClone()
        {
            // Check for parameterless constructor:
            var constructor = GetType().GetConstructor(Type.EmptyTypes);
            if (constructor == null)
                throw new InvalidOperationException("No parameterless constructor available.  CreateInstance must be overridden.");

            return Activator.CreateInstance(GetType()) as EtpProtocolHandler;
        }

        /// <summary>
        /// Sets the protocol handler's session.
        /// </summary>
        /// <param name="session">The ETP session.</param>
        public void SetSession(IEtpSession session)
        {
            Session = session;
        }

        /// <summary>
        /// Sets the capabilities for the handler's counterpart.
        /// </summary>
        /// <param name="counterpartCapabilities">The counterpart's protocol capabilities.</param>
        public void SetCounterpartCapabilities(IProtocolCapabilities counterpartCapabilities)
        {
            CounterpartCapabilities = CreateCounterpartCapabilities(counterpartCapabilities);
        }

        /// <summary>
        /// Creates the actual counterpart capabilities to use.
        /// </summary>
        /// <param name="counterpartCapabilities">The capabilities received from the counterpart.</param>
        /// <returns>The coutnerpart capabilities to use.</returns>
        protected virtual IProtocolCapabilities CreateCounterpartCapabilities(IProtocolCapabilities counterpartCapabilities)
        {
            return counterpartCapabilities;
        }

        /// <summary>
        /// Start the protocol handler when the session has been opened.
        /// </summary>
        public void StartHandling()
        {
            StartBackgroundCleanUpLoop();
            StartCore();

            OnStarted?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event raised when the handler is started.
        /// </summary>
        public event EventHandler<EventArgs> OnStarted;

        /// <summary>
        /// Handles any additional initialization when the session has been opened, including rationalizing actual protocol capabilities.
        /// </summary>
        protected virtual void StartCore()
        {
        }

        /// <summary>
        /// Called when the ETP session is closed.
        /// </summary>
        public void StopHandling()
        {
            StopBackgroundCleanUpLoop();
            StopCore();

            OnStopped?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event raised when the handler is stopped.
        /// </summary>
        public event EventHandler<EventArgs> OnStopped;

        /// <summary>
        /// Handles any additional finalization when the session has been closed.
        /// </summary>
        protected virtual void StopCore()
        {

        }

        /// <summary>
        /// Sends an Acknowledge message in response to the message associated with the correlation header.
        /// </summary>
        /// <param name="correlatedHeader">The message header the acknowledge message is correlated with.</param>
        /// <param name="isNoData">Whether or not the acknowledge message should have the NoData flag set.</param>
        /// <param name="extension">The message header extension to send with the message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<IAcknowledge> Acknowledge(IMessageHeader correlatedHeader, bool isNoData = false, IMessageHeaderExtension extension = null)
        {
            return Session.Acknowledge(Protocol, correlatedHeader, isNoData: isNoData, extension: extension);
        }

        /// <summary>
        /// Constructs a new <see cref="IErrorInfo"/> instance compatible with the session.
        /// </summary>
        /// <returns>The constructed error info.</returns>
        public IErrorInfo ErrorInfo() => EtpFactory.CreateErrorInfo(EtpVersion);

        /// <summary>
        /// Sends a ProtocolException message.
        /// </summary>
        /// <param name="error">The error in the protocol exception.</param>
        /// <param name="correlatedHeader">The message header the protocol exception is correlated with, if any.</param>
        /// <param name="isFinalPart">Whether or not the protocol exception is the final part in a multi-part message.</param>
        /// <param name="extension">The message header extension to send with the message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<IProtocolException> ProtocolException(IErrorInfo error, IMessageHeader correlatedHeader = null, bool isFinalPart = false, IMessageHeaderExtension extension = null)
        {
            return Session.ProtocolException(Protocol, error, correlatedHeader: correlatedHeader, isFinalPart: isFinalPart, extension: extension);
        }

        /// <summary>
        /// Sends a ProtocolException message(s) with the specified exception details.
        /// </summary>
        /// <param name="errors">The errors in the protocol exception.</param>
        /// <param name="correlatedHeader">The message header the protocol exception is correlated with, if any.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="extension">The message header extension to send with the message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<IProtocolException> ProtocolException(IDictionary<string, IErrorInfo> errors, IMessageHeader correlatedHeader = null, bool setFinalPart = true, IMessageHeaderExtension extension = null)
        {
            return Session.ProtocolException(Protocol, errors, correlatedHeader: correlatedHeader, setFinalPart: setFinalPart, extension: extension);
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
            return Session.ProtocolException(exception, isFinalPart: isFinalPart, extension: extension);
        }

        /// <summary>
        /// Occurs when an Acknowledge message is received for the current protocol.
        /// </summary>
        public event EventHandler<MessageEventArgs<IAcknowledge>> OnAcknowledge;

        /// <summary>
        /// Occurs when a ProtocolException message is received for the current protocol that is not associated with a request.
        /// </summary>
        public event EventHandler<MessageEventArgs<IProtocolException>> OnProtocolException;

        /// <summary>
        /// Occurs when no response has been received in a timely fashion to a previously sent request message.
        /// </summary>
        public event EventHandler<MessageEventArgs> OnRequestTimedOut;
        
        /// <summary>
        /// Handles the message based on the message type contained in the specified <see cref="IMessageHeader" />.
        /// </summary>
        /// <param name="message">The message.</param>
        public void HandleMessage(EtpMessage message)
        {
            if (!InvokeMessageHandler(message))
                ProtocolException(ErrorInfo().InvalidMessageType(message.Header.Protocol, message.Header.MessageType), message.Header);

            // Additional processing if this message was the final message in response to a request:
            if (message.Header.IsFinalResponse(EtpVersion))
            {
                var request = TryUnregisterMessage(message.Header.CorrelationId);
                if (request != null)
                    HandleFinalResponse(request);
            }
        }

        /// <summary>
        /// Handles the Acknowledge message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected virtual void HandleAcknowledge(EtpMessage<IAcknowledge> message)
        {
            HandleMessage(message, OnAcknowledge);
        }

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected virtual void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            HandleMessage(message, OnProtocolException);
            Logger.DebugFormat("[{0}] Protocol exception: {1} - {2}", Session.SessionKey, message.Body.ErrorCode, message.Body.ErrorMessage);
        }

        /// <summary>
        /// Handle any final cleanup related to the final message in response to a request.
        /// </summary>
        /// <param name="request">The request</param>
        protected virtual void HandleFinalResponse(EtpMessage request)
        {
        }

        /// <summary>
        /// Handle any final cleanup related to a request timing out.
        /// </summary>
        /// <param name="request">The request that timed out.</param>
        protected virtual void HandleTimedOutRequest(EtpMessage request)
        {
            Logger.Trace($"[{Session.SessionKey}] Request Timed Out: Name: {request.MessageName}; Header: {EtpExtensions.Serialize(request.Header)}");
            HandleMessage(request, OnRequestTimedOut);
        }

        /// <summary>
        /// Handles a message of unspecified type.
        /// </summary>
        /// <typeparam name="TBody">The type of the message body.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="eventHandler">The event handler for the event associated with the message.</param>
        protected void HandleMessage<TBody>(EtpMessage<TBody> message, EventHandler<MessageEventArgs<TBody>> eventHandler)
            where TBody : ISpecificRecord
        {
            var args = new MessageEventArgs<TBody>(message);
            eventHandler?.Invoke(this, args);
        }

        /// <summary>
        /// Handles a message of unspecified type.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="eventHandler">The event handler for the event associated with the message.</param>
        protected void HandleMessage(EtpMessage message, EventHandler<MessageEventArgs> eventHandler)
        {
            var args = new MessageEventArgs(message);
            eventHandler?.Invoke(this, args);
        }

        /// <summary>
        /// Handles a fire and forget message.
        /// </summary>
        /// <typeparam name="TBody">The type of the message body.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="methodHandler">The class method for the event associated with the request.</param>
        /// <param name="eventHandler">The event handler for the event associated with the message.</param>
        protected void HandleFireAndForgetMessage<TBody>(EtpMessage<TBody> message, EventHandler<FireAndForgetEventArgs<TBody>> eventHandler, Action<FireAndForgetEventArgs<TBody>> methodHandler)
            where TBody : ISpecificRecord
        {
            var args = new FireAndForgetEventArgs<TBody>(message);
            eventHandler?.Invoke(this, args);
            methodHandler?.Invoke(args);
        }

        /// <summary>
        /// Handles a request message.
        /// </summary>
        /// <typeparam name="TArgs">The type of message event arguments.</typeparam>
        /// <typeparam name="TRequest">The type of the request message body.</typeparam>
        /// <param name="request">The request message.</param>
        /// <param name="eventHandler">The event handler for the event associated with the request.</param>
        /// <param name="methodHandler">The class method for the event associated with the request.</param>
        /// <param name="args">Optional arguments to use with the request handling.</param>
        /// <param name="responseMethod">The method to use to send a positive response.</param>
        protected void HandleRequestMessage<TRequest, TArgs>(EtpMessage<TRequest> request, EventHandler<TArgs> eventHandler, Action<TArgs> methodHandler, TArgs args = null, Action<TArgs> responseMethod = null)
            where TRequest : ISpecificRecord
            where TArgs : RequestEventArgsBase<TRequest>
        {
            args = args ?? (TArgs)Activator.CreateInstance(typeof(TArgs), request);
            eventHandler?.Invoke(this, args);
            if (!args.SendResponse)
                return;

            methodHandler?.Invoke(args);
            if (!args.SendResponse)
                return;
            
            if (args.HasNonErrorResponse)
                responseMethod?.Invoke(args);

            if (args.HasErrorMapErrors)
                ProtocolException(args.ErrorMap, correlatedHeader: args.Request.Header, setFinalPart: !args.HasFinalError, extension: args.ErrorMapExtension);

            if (args.HasFinalError)
                ProtocolException(args.FinalError, correlatedHeader: args.Request.Header, isFinalPart: true, extension: args.FinalErrorExtension);
        }

        /// <summary>
        /// Handles a cancellation message.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request message body.</typeparam>
        /// <typeparam name="TCancellation">The type of the cancellation message body.</typeparam>
        /// <param name="request">The original request message.</param>
        /// <param name="request">The cancellation request message.</param>
        /// <param name="eventHandler">The event handler for the event associated with the request.</param>
        /// <param name="methodHandler">The class method for the event associated with the request.</param>
        /// <param name="args">Optional arguments to use with the request handling.</param>
        protected void HandleCancellationMessage<TRequest, TCancellation>(EtpMessage<TRequest> request, EtpMessage<TCancellation> cancellation, EventHandler<CancellationRequestEventArgs<TRequest, TCancellation>> eventHandler, Action<CancellationRequestEventArgs<TRequest, TCancellation>> methodHandler, CancellationRequestEventArgs<TRequest, TCancellation> args = null)
            where TRequest : ISpecificRecord
            where TCancellation : ISpecificRecord
        {
            args = args ?? new CancellationRequestEventArgs<TRequest, TCancellation>(request, cancellation);
            eventHandler?.Invoke(this, args);
            if (!args.SendResponse)
                return;

            if (args.HasErrorMapErrors)
                ProtocolException(args.ErrorMap, correlatedHeader: args.Request.Header, setFinalPart: !args.HasFinalError, extension: args.ErrorMapExtension);

            if (args.HasFinalError)
                ProtocolException(args.FinalError, correlatedHeader: args.Request.Header, isFinalPart: true, extension: args.FinalErrorExtension);
        }

        /// <summary>
        /// Handles a response message.
        /// </summary>
        /// <typeparam name="TArgs">The type of message event arguments.</typeparam>
        /// <typeparam name="TRequest">The type of the original request message body.</typeparam>
        /// <typeparam name="TResponse">The type of the response message body received.</typeparam>
        /// <param name="request">The original request message.</param>
        /// <param name="request">The response message.</param>
        /// <param name="eventHandler">The event handler for the event associated with the request.</param>
        /// <param name="methodHandler">The class method for the event associated with the request.</param>
        protected void HandleResponseMessage<TRequest, TResponse, TArgs>(EtpMessage<TRequest> request, EtpMessage<TResponse> response, EventHandler<TArgs> eventHandler, Action<TArgs> methodHandler)
            where TRequest : ISpecificRecord
            where TResponse : ISpecificRecord
            where TArgs : ResponseEventArgsBase<TRequest>
        {
            var args = (TArgs)Activator.CreateInstance(typeof(TArgs), request, response);
            eventHandler?.Invoke(this, args);
            methodHandler?.Invoke(args);
        }

        /// <summary>
        /// Handles a notification message.
        /// </summary>
        /// <typeparam name="TArgs">The type of message event arguments.</typeparam>
        /// <typeparam name="TSubscription">The subscription type.</typeparam>
        /// <typeparam name="TNotification">The type of the notification message body received.</typeparam>
        /// <param name="subscription">The original subscription.</param>
        /// <param name="notification">The notification message.</param>
        /// <param name="eventHandler">The event handler for the event associated with the request.</param>
        /// <param name="methodHandler">The class method for the event associated with the request.</param>
        protected void HandleNotificationMessage<TSubscription, TNotification, TArgs>(EtpSubscription<TSubscription> subscription, EtpMessage<TNotification> notification, EventHandler<TArgs> eventHandler, Action<TArgs> methodHandler, TArgs args = null)
            where TNotification : ISpecificRecord
            where TArgs : NotificationEventArgs<TSubscription, TNotification>
        {
            args = args ?? (TArgs)Activator.CreateInstance(typeof(TArgs), subscription, notification);
            eventHandler?.Invoke(this, args);
            methodHandler?.Invoke(args);
        }

        /// <summary>
        /// Invokes the message handler for the specified message type.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns><c>true</c> if a handler for the message was invoked; <c>false</c> otherwise.</returns>
        public bool InvokeMessageHandler(EtpMessage message)
        {
            if (message?.Header == null)
                return false;

            var messageKey = EtpExtensions.CreateMessageKey(message.Header.Protocol, message.Header.MessageType);

            Action<EtpMessage> messageHandler;
            if (!MessageHandlers.TryGetValue(messageKey, out messageHandler))
                return false;

            messageHandler(message);
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
        public void RegisterMessageHandler<T>(object protocol, object messageType, Action<EtpMessage<T>> messageHandler) where T : ISpecificRecord
        {
            var messageKey = EtpExtensions.CreateMessageKey(Convert.ToInt32(protocol), Convert.ToInt32(messageType));

            MessageHandlers[messageKey] = (m) => messageHandler((EtpMessage<T>)m);
        }

        /// <summary>
        /// Creates a message header for the specified message body type.
        /// </summary>
        /// <typeparam name="T">The message body type.</typeparam>
        /// <returns>The created header.</returns>
        protected virtual IMessageHeader CreateMessageHeader<T>() where T : ISpecificRecord
        {
            var messageTypeNumber = Session.Adapter.TryGetMessageTypeNumber(typeof(T));
            if (messageTypeNumber == -1)
                Logger.Debug($"[{Session.SessionKey}] Message body type not registered: {typeof(T)}.");

            return EtpFactory.CreateMessageHeader(EtpVersion, Protocol, messageTypeNumber);
        }

        /// <summary>
        /// Sends a request message through the session.
        /// </summary>
        /// <typeparam name="T">The type of the message body.</typeparam>
        /// <param name="body">The message body.</param>
        /// <param name="extension">The message header extension.</param>
        /// <param name="isMultiPart">Whether or not this request is a multi-part message.</param>
        /// <param name="correlatedHeader">The message header of the message in the multi-part request that the message to be sent is correlated with.</param>
        /// <param name="isFinalPart">Whether or not this request is the final part in a multi-part request.</param>
        /// <param name="isLongLived">Whether this request is long lived or not.  A long lived request lives beyond the end of any FinalPart flag.</param>
        /// <param name="onBeforeSend">Action called just before sending the message with the actual header having the definitive message ID.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public EtpMessage<T> SendRequest<T>(T body, IMessageHeaderExtension extension = null, bool isMultiPart = false, IMessageHeader correlatedHeader = null, bool isFinalPart = true, bool isLongLived = false, Action<EtpMessage<T>> onBeforeSend = null) where T : ISpecificRecord
        {
            long messageId = 0;

            var request = SendMessage(body, correlatedHeader, extension, isMultiPart, isFinalPart, false, (m) =>
            {
                if (correlatedHeader == null)
                {
                    messageId = m.Header.MessageId;
                    TryRegisterMessage(m, isLongLived: isLongLived);
                }

                onBeforeSend?.Invoke(m);
            });

            if (request == null && correlatedHeader == null)
                TryUnregisterMessage(messageId, isLongLived: isLongLived);

            return request;
        }

        /// <summary>
        /// Sends a notification message through the session.
        /// </summary>
        /// <typeparam name="T">The type of the message body.</typeparam>
        /// <param name="body">The message body.</param>
        /// <param name="extension">The message header extension.</param>
        /// <param name="isMultiPart">Whether or not this notification is a multi-part message.</param>
        /// <param name="correlatedHeader">The message header of the message in the multi-part notification that the message to be sent is correlated with.</param>
        /// <param name="isFinalPart">Whether or not this notification is the final part in a multi-part notification.</param>
        /// <param name="isNoData">Whether or not this message should have the no data flag set.</param>
        /// <param name="onBeforeSend">Action called just before sending the message with the actual header having the definitive message ID.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public EtpMessage<T> SendNotification<T>(T body, IMessageHeaderExtension extension = null, bool isMultiPart = false, IMessageHeader correlatedHeader = null, bool isFinalPart = true, bool isNoData = false, Action<EtpMessage<T>> onBeforeSend = null) where T : ISpecificRecord
        {
            return SendMessage(body, correlatedHeader, extension, isMultiPart, isFinalPart, isNoData, onBeforeSend);
        }

        /// <summary>
        /// Sends a data message through the session.
        /// </summary>
        /// <typeparam name="T">The type of the message body.</typeparam>
        /// <param name="body">The message body.</param>
        /// <param name="extension">The message header extension.</param>
        /// <param name="isMultiPart">Whether or not this data message is a multi-part message.</param>
        /// <param name="isNoData">Whether or not this message should have the no data flag set.</param>
        /// <param name="onBeforeSend">Action called just before sending the message with the actual header having the definitive message ID.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public EtpMessage<T> SendData<T>(T body, IMessageHeaderExtension extension = null, bool isMultiPart = false, bool isNoData = false, Action<EtpMessage<T>> onBeforeSend = null) where T : ISpecificRecord
        {
            return SendMessage(body, null, extension, isMultiPart, false, isNoData, onBeforeSend);
        }

        /// <summary>
        /// Sends a response message through the session.
        /// </summary>
        /// <typeparam name="T">The type of the message body.</typeparam>
        /// <param name="body">The message body.</param>
        /// <param name="extension">The message header extension.</param>
        /// <param name="correlatedHeader">The message header of the request that the response is correlated with.</param>
        /// <param name="isMultiPart">Whether or not this reponse is a multi-part response.</param>
        /// <param name="isFinalPart">Whether or not this is the final part in a multi-part response.</param>
        /// <param name="isNoData">Whether or not this message should have the no data flag set.</param>
        /// <param name="onBeforeSend">Action called just before sending the message with the actual header having the definitive message ID.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public EtpMessage<T> SendResponse<T>(T body, IMessageHeader correlatedHeader, IMessageHeaderExtension extension = null, bool isMultiPart = false, bool isFinalPart = true, bool isNoData = false, Action<EtpMessage<T>> onBeforeSend = null) where T : ISpecificRecord
        {
            return SendMessage(body, correlatedHeader, extension, isMultiPart, isFinalPart, isNoData, onBeforeSend);
        }

        /// <summary>
        /// Sends a message through the session.
        /// </summary>
        /// <typeparam name="T">The type of the message body.</typeparam>
        /// <param name="body">The message body.</param>
        /// <param name="correlatedHeader">The message header that the message to be sent is correlated with, if any.</param>
        /// <param name="extension">The message header extension.</param>
        /// <param name="isMultiPart">Whether or not this is a multi-part message.</param>
        /// <param name="isFinalPart">Whether or not this is the final part multi-part message.</param>
        /// <param name="isNoData">Whether or not this message should have the no data flag set.</param>
        /// <param name="onBeforeSend">Action called just before sending the message with the actual header having the definitive message ID.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        private EtpMessage<T> SendMessage<T>(T body, IMessageHeader correlatedHeader, IMessageHeaderExtension extension, bool isMultiPart, bool isFinalPart, bool isNoData, Action<EtpMessage<T>> onBeforeSend) where T : ISpecificRecord
        {
            var header = CreateMessageHeader<T>();
            header.PrepareHeader(correlatedHeader, isMultiPart, isFinalPart);
            if (isNoData)
                header.SetNoData();

            var message = new EtpMessage<T>(header, body, extension: extension);

            return Session.SendMessage(message, onBeforeSend);
        }

        /// <summary>
        /// Tries to register the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="isLongLived">Whether the request is long lived or not.</param>
        /// <returns><c>true</c> if the request could be registered; <c>false</c> otherwise.</returns>
        public bool TryRegisterMessage(EtpMessage message, bool isLongLived = false)
        {
            return RegisteredMessages.TryAdd(isLongLived ? -message.Header.MessageId : message.Header.MessageId, message);
        }

        /// <summary>
        /// Tries to gets the message correlated with the specified message.
        /// </summary>
        /// <typeparam name="T">The correlated message body type.</typeparam>
        /// <param name="message">The message.</param>
        /// <returns>The correlated message if found; <c>null</c> otherwise.</returns>
        public EtpMessage<T> TryGetCorrelatedMessage<T>(EtpMessage message) where T : ISpecificRecord
        {
            var correlatedMessage = TryGetCorrelatedMessage(message);
            if (correlatedMessage is EtpMessage<T>)
                return correlatedMessage as EtpMessage<T>;

            Logger.Debug($"[{Session.SessionKey}] Unexpected Message Type for Correlation ID: Name: {message.MessageName}; Header: {EtpExtensions.Serialize(message.Header)}; Correlation ID: {message.Header.CorrelationId}; Expected Type: {typeof(T).Name}; Request Name: {correlatedMessage.MessageName}; Request Header:  {EtpExtensions.Serialize(correlatedMessage.Header)}");
            return null;
        }

        /// <summary>
        /// Tries to gets the message correlated with the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The correlated message if found; <c>null</c> otherwise.</returns>
        public EtpMessage TryGetCorrelatedMessage(EtpMessage message)
        {
            EtpMessage request;
            if (RegisteredMessages.TryGetValue(message.Header.CorrelationId, out request))
                return request;

            // Next try long-lived requests:
            if (RegisteredMessages.TryGetValue(-message.Header.CorrelationId, out request))
                return request;

            Logger.Debug($"[{Session.SessionKey}] Correlated Message Not Found: Name: {message.MessageName}; Header: {EtpExtensions.Serialize(message.Header)}; Correlation ID: {message.Header.CorrelationId}");
            return null;
        }

        /// <summary>
        /// Tries to unregister the message.
        /// </summary>
        /// <param name="messageId">The message ID.</param>
        /// <param name="isLongLived">Whether the request is long lived or not.</param>
        /// <returns>The request if it could be unregistered; <c>null</c> otherwise.</returns>
        public EtpMessage TryUnregisterMessage(long messageId, bool isLongLived = false)
        {
            EtpMessage message;
            if (RegisteredMessages.TryRemove(isLongLived ? -messageId : messageId, out message))
                return message;

            return null;
        }

        /// <summary>
        /// Tries to register the request.
        /// </summary>
        /// <param name="requestUuid">The request's UUID.</param>
        /// <param name="argumentName">The request's UUID argument name.</param>
        /// <param name="request">The request message.</param>
        /// <returns><c>null</c> if the request could be registered; an <see cref="IErrorInfo"/> instance otherwise.</returns>
        public IErrorInfo TryRegisterRequest(IRequestUuidGuidSource requestUuid, string argumentName, EtpMessage request)
        {
            return TryRegisterRequest(requestUuid?.RequestUuidGuid, argumentName, request);
        }

        /// <summary>
        /// Tries to register the request.
        /// </summary>
        /// <param name="requestUuid">The request's UUID.</param>
        /// <param name="argumentName">The request's UUID argument name.</param>
        /// <param name="request">The request message.</param>
        /// <returns><c>null</c> if the request could be registered; an <see cref="IErrorInfo"/> instance otherwise.</returns>
        public IErrorInfo TryRegisterRequest(IUuidGuidSource requestUuid, string argumentName, EtpMessage request)
        {
            if (requestUuid == null || !requestUuid.IsUuidValidGuid)
            {
                Logger.Debug($"[{Session.SessionKey}] Invalid Request Uuid: Name: {request.MessageName}; Header: {EtpExtensions.Serialize(request.Header)}; Uuid: '{requestUuid.RawUuid}'");
                return ErrorInfo().InvalidArgument(argumentName, requestUuid);
            }

            if (RegisteredRequestsByUuid.TryAdd(requestUuid.UuidGuid, request))
                return null;
            else
            {
                Logger.Debug($"[{Session.SessionKey}] Duplicate Subscription Uuid: Name: {request.MessageName}; Header: {EtpExtensions.Serialize(request.Header)}; Uuid: '{requestUuid}'");
                return ErrorInfo().RequestUuidRejected(requestUuid);
            }
        }

        /// <summary>
        /// Tries to get the request.
        /// </summary>
        /// <typeparam name="T">The request message type.</typeparam>
        /// <param name="requestUuid">The cancellation request's UUID.</param>
        /// <param name="argumentName">The cancellation request's UUID argument name.</param>
        /// <param name="cancellation">The cancellation request message.</param>
        /// <param name="request">The original request.</param>
        /// <returns><c>null</c> if the original request was found; an <see cref="IErrorInfo"/> instance otherwise.</returns>
        public IErrorInfo TryGetRequest<T>(IRequestUuidGuidSource requestUuid, string argumentName, EtpMessage cancellation, out EtpMessage<T> request) where T : ISpecificRecord
        {
            return TryGetRequest(requestUuid?.RequestUuidGuid, argumentName, cancellation, out request);
        }

        /// <summary>
        /// Tries to get the request.
        /// </summary>
        /// <typeparam name="T">The request message type.</typeparam>
        /// <param name="requestUuid">The cancellation request's UUID.</param>
        /// <param name="argumentName">The cancellation request's UUID argument name.</param>
        /// <param name="cancellation">The cancellation request message.</param>
        /// <param name="request">The original request.</param>
        /// <returns><c>null</c> if the original request was found; an <see cref="IErrorInfo"/> instance otherwise.</returns>
        public IErrorInfo TryGetRequest<T>(IUuidGuidSource requestUuid, string argumentName, EtpMessage cancellation, out EtpMessage<T> request) where T : ISpecificRecord
        {
            if (requestUuid == null || !requestUuid.IsUuidValidGuid)
            {
                request = null;
                Logger.Debug($"[{Session.SessionKey}] Invalid Request Uuid: Name: {cancellation.MessageName}; Header: {EtpExtensions.Serialize(cancellation.Header)}; Uuid: '{requestUuid.RawUuid}'");
                return ErrorInfo().InvalidArgument(argumentName, requestUuid);
            }

            EtpMessage r;
            if (RegisteredRequestsByUuid.TryGetValue(requestUuid.UuidGuid, out r))
            {
                request = r as EtpMessage<T>;
                if (request != null)
                    return null;
                else
                {
                    Logger.Debug($"[{Session.SessionKey}] Unexpected Request Type for Uuid: Name: {cancellation.MessageName}; Header: {EtpExtensions.Serialize(cancellation.Header)}; Uuid: '{requestUuid.DisplayUuid}'; Expected Type: {typeof(T).Name}; Request Name: {request.MessageName}; Request Header:  {EtpExtensions.Serialize(request.Header)}");
                    return ErrorInfo().InvalidState();
                }
            }
            else
            {
                request = null;
                Logger.Debug($"[{Session.SessionKey}] Request Uuid Not Found: Name: {cancellation.MessageName}; Header: {EtpExtensions.Serialize(cancellation.Header)}; Uuid: '{requestUuid.DisplayUuid}'");
                return ErrorInfo().NotFound(argumentName, requestUuid);
            }
        }

        /// <summary>
        /// Tries to unregister the request.
        /// </summary>
        /// <param name="requestUuid">The request's UUID.</param>
        /// <returns><c>true</c> if the subscription could be registered; <c>false</c> otherwise.</returns>
        public bool TryUnregisterRequest(Guid requestUuid)
        {
            if (RegisteredRequestsByUuid.TryRemove(requestUuid, out _))
                return true;

            Logger.Debug($"[{Session.SessionKey}] Request Uuid Not Found: '{requestUuid}'");
            return false;
        }

        /// <summary>
        /// Tries to register the subscriptions.  Any failed registrations are removed from the input subscription dictionary.
        /// </summary>
        /// <typeparam name="TSubscription">The subscription type.</typeparam>
        /// <param name="argumentName">The subscription UUID argument name.</param>
        /// <param name="message">The subscription message.</param>
        /// <param name="subscriptions">The subscription dictionary.</param>
        /// <returns><c>null</c> if the subscription could be registered; a dictionary of <see cref="IErrorInfo"/> error instances otherwise.</returns>
        public Dictionary<string, IErrorInfo> TryRegisterSubscriptions<TMessage, TSubscription>(EtpMessage<TMessage> message, IDictionary<string, TSubscription> subscriptions, string argumentName)
            where TSubscription : IRequestUuidGuidSource
            where TMessage : ISpecificRecord
        {
            Dictionary<string, IErrorInfo> errors = null;
            foreach (var kvp in subscriptions)
            {
                var error = TryRegisterSubscription(kvp.Value, argumentName, message, kvp.Value);
                if (error != null)
                {
                    if (errors == null)
                        errors = new Dictionary<string, IErrorInfo>();

                    errors[kvp.Key] = error;
                }
            }

            if (errors != null)
            {
                foreach (var key in errors.Keys)
                    subscriptions.Remove(key);
            }

            return errors;
        }

        /// <summary>
        /// Tries to register the subscription.
        /// </summary>
        /// <typeparam name="TSubscription">The subscription type.</typeparam>
        /// <param name="requestUuid">The subscription's UUID.</param>
        /// <param name="argumentName">The subscription's UUID argument name.</param>
        /// <param name="message">The subscription message.</param>
        /// <param name="subscription">The subscription.</param>
        /// <returns><c>null</c> if the subscription could be registered; an <see cref="IErrorInfo"/> instance otherwise.</returns>
        public IErrorInfo TryRegisterSubscription<TMessage, TSubscription>(IRequestUuidGuidSource requestUuid, string argumentName, EtpMessage<TMessage> message, TSubscription subscription)
            where TMessage : ISpecificRecord
        {
            return TryRegisterSubscription(requestUuid?.RequestUuidGuid, argumentName, message, subscription);
        }

        /// <summary>
        /// Tries to register the subscription.
        /// </summary>
        /// <typeparam name="TSubscription">The subscription type.</typeparam>
        /// <param name="requestUuid">The subscription's UUID.</param>
        /// <param name="argumentName">The subscription's UUID argument name.</param>
        /// <param name="message">The subscription message.</param>
        /// <param name="subscription">The subscription.</param>
        /// <returns><c>null</c> if the subscription could be registered; an <see cref="IErrorInfo"/> instance otherwise.</returns>
        public IErrorInfo TryRegisterSubscription<TMessage, TSubscription>(IUuidGuidSource requestUuid, string argumentName, EtpMessage<TMessage> message, TSubscription subscription)
            where TMessage : ISpecificRecord
        {
            if (requestUuid == null || !requestUuid.IsUuidValidGuid)
            {
                Logger.Debug($"[{Session.SessionKey}] Invalid Subscription Uuid: Name: {message.MessageName}; Header: {EtpExtensions.Serialize(message.Header)}; Uuid: '{requestUuid?.RawUuid}'");
                return ErrorInfo().InvalidArgument(argumentName, requestUuid);
            }

            var sub = new EtpSubscription<TMessage, TSubscription>(requestUuid.UuidGuid, message, subscription);
            if (RegisteredSubscriptions.TryAdd(sub.Uuid, sub))
            {
                RegisteredSubscriptionsByCorrelationId[message.Header.MessageId] = sub;
                return null;
            }
            else
            {
                Logger.Debug($"[{Session.SessionKey}] Duplicate Subscription Uuid: Name: {message.MessageName}; Header: {EtpExtensions.Serialize(message.Header)}; Uuid: '{requestUuid}'");
                return ErrorInfo().RequestUuidRejected(requestUuid);
            }
        }

        /// <summary>
        /// Gets the original subscription as a strongly typed message by the subscription's request UUID.
        /// </summary>
        /// <typeparam name="TSubscription">The subscription type.</typeparam>
        /// <param name="requestUuid">The subscription request UUID.</param>
        /// <returns>The original subscription.</returns>
        public EtpSubscription<TSubscription> TryGetSubscription<TSubscription>(Guid requestUuid)
        {
            return TryGetSubscription<TSubscription>(new GuidGuidSource(requestUuid));
        }

        /// <summary>
        /// Gets the original subscription as a strongly typed message by the subscription's request UUID.
        /// </summary>
        /// <typeparam name="TSubscription">The subscription type.</typeparam>
        /// <param name="requestUuid">The subscription request UUID.</param>
        /// <returns>The original subscription.</returns>
        public EtpSubscription<TSubscription> TryGetSubscription<TSubscription>(IRequestUuidGuidSource requestUuid)
        {
            return TryGetSubscription<TSubscription>(requestUuid?.RequestUuidGuid);
        }

        /// <summary>
        /// Gets the original subscription as a strongly typed message by the subscription's request UUID.
        /// </summary>
        /// <typeparam name="TSubscription">The subscription type.</typeparam>
        /// <param name="requestUuid">The subscription request UUID.</param>
        /// <returns>The original subscription.</returns>
        public EtpSubscription<TSubscription> TryGetSubscription<TSubscription>(IUuidGuidSource requestUuid)
        {
            EtpSubscription subscription;
            if (requestUuid != null && RegisteredSubscriptions.TryGetValue(requestUuid.UuidGuid, out subscription))
            {
                if (subscription is EtpSubscription<TSubscription>)
                    return subscription as EtpSubscription<TSubscription>;
                else
                {
                    Logger.Debug($"[{Session.SessionKey}] Unexpected Request Type for Uuid: Name: {subscription.MessageName}; Header: {EtpExtensions.Serialize(subscription.Header)}; Uuid: '{requestUuid.DisplayUuid}'; Expected Subscription Type: {typeof(TSubscription).Name}");
                    return null;
                }
            }
            else
            {
                Logger.Debug($"[{Session.SessionKey}] Request Uuid Not Found: '{requestUuid?.DisplayUuid}'");
                return null;
            }
        }

        /// <summary>
        /// Gets the original subscription as a strongly typed message by a message header correlated with the original subscription message.
        /// </summary>
        /// <typeparam name="TSubscription">The subscription type.</typeparam>
        /// <param name="header">A message header correlated with the subscription.</param>
        /// <returns>The original subscription.</returns>
        public EtpSubscription<TSubscription> TryGetSubscription<TSubscription>(IMessageHeader header)
        {
            EtpSubscription subscription;
            RegisteredSubscriptionsByCorrelationId.TryGetValue(header.CorrelationId, out subscription);
            return subscription as EtpSubscription<TSubscription>;
        }

        /// <summary>
        /// Tries to unregister the subscription.
        /// </summary>
        /// <param name="requestUuid">The subscription's UUID.</param>
        /// <param name="argumentName">The subscription's UUID argument name.</param>
        /// <param name="unsubscribeRequest">The unsubscription message.</param>
        /// <returns><c>null</c> if the subscription could be unregistered; an <see cref="IErrorInfo"/> instance otherwise.</returns>
        public IErrorInfo TryUnregisterSubscription(IRequestUuidGuidSource requestUuid, string argumentName, EtpMessage unsubscribeRequest)
        {
            return TryUnregisterSubscription(requestUuid?.RequestUuidGuid, argumentName, unsubscribeRequest);
        }

        /// <summary>
        /// Tries to unregister the subscription.
        /// </summary>
        /// <param name="requestUuid">The subscription's UUID.</param>
        /// <param name="argumentName">The subscription's UUID argument name.</param>
        /// <param name="unsubscribeRequest">The unsubscription message.</param>
        /// <returns><c>null</c> if the subscription could be unregistered; an <see cref="IErrorInfo"/> instance otherwise.</returns>
        public IErrorInfo TryUnregisterSubscription(IUuidGuidSource requestUuid, string argumentName, EtpMessage unsubscribeRequest)
        {
            if (requestUuid == null || !requestUuid.IsUuidValidGuid)
            {
                Logger.Debug($"[{Session.SessionKey}] Invalid Subscription Uuid: Name: {unsubscribeRequest.MessageName}; Header: {EtpExtensions.Serialize(unsubscribeRequest.Header)}; Uuid: '{requestUuid?.RawUuid}'");
                return ErrorInfo().InvalidArgument(argumentName, requestUuid);
            }

            if (TryUnregisterSubscription(requestUuid))
            {
                return null;
            }
            else
            {
                Logger.Debug($"[{Session.SessionKey}] Subscription Uuid Not Found: Name: {unsubscribeRequest.MessageName}; Header: {EtpExtensions.Serialize(unsubscribeRequest.Header)}; Uuid: '{requestUuid.DisplayUuid}'");
                return ErrorInfo().NotFound(argumentName, requestUuid);
            }
        }

        /// <summary>
        /// Tries to unregister the subscription.
        /// </summary>
        /// <param name="requestUuid">The subscription's UUID.</param>
        /// <returns><c>null</c> if the subscription could be unregistered; false otherwise.</returns>
        public bool TryUnregisterSubscription(IRequestUuidGuidSource requestUuid)
        {
            return TryUnregisterSubscription(requestUuid?.RequestUuidGuid);
        }

        /// <summary>
        /// Tries to unregister the subscription.
        /// </summary>
        /// <param name="requestUuid">The subscription's UUID.</param>
        /// <returns><c>null</c> if the subscription could be unregistered; false otherwise.</returns>
        public bool TryUnregisterSubscription(IUuidGuidSource requestUuid)
        {
            EtpSubscription subscription;
            if (requestUuid != null && RegisteredSubscriptions.TryRemove(requestUuid.UuidGuid, out subscription))
            {
                RegisteredSubscriptionsByCorrelationId.TryRemove(subscription.Header.MessageId, out _);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Starts the background cleanup loop.
        /// </summary>
        private void StartBackgroundCleanUpLoop()
        {
            _backgroundCleanUpTask = Task.Factory.StartNew(
                async () => await BackgroundCleanUpLoop(_source.Token).ConfigureAwait(Session.CaptureAsyncContext), _source.Token,
                TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default).Unwrap();
        }

        /// <summary>
        /// Stops the background cleanup loop.
        /// </summary>
        private void StopBackgroundCleanUpLoop()
        {
            _source?.Cancel();
            try
            {
                if (_backgroundCleanUpTask != null)
                    AsyncContext.Run(() => _backgroundCleanUpTask.ConfigureAwait(Session.CaptureAsyncContext));
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                _backgroundCleanUpTask = null;
                _source?.Dispose();
                _source = null;
            }

        }

        /// <summary>
        /// Method called once per second to handle any periodic background cleanup that needs to be done.
        /// </summary>
        protected virtual void BackgroundCleanUp()
        {
            var now = DateTime.UtcNow;
            var timeout = TimeSpan.FromSeconds(EtpSettings.DefaultResponseTimeoutPeriod ?? 600);

            foreach (var request in RegisteredMessages.Values)
            {
                var elapsedTime = now - request.Header.Timestamp;
                if (elapsedTime < timeout)
                    continue;

                if (!RegisteredMessages.TryRemove(request.Header.MessageId, out _))
                    continue;

                HandleTimedOutRequest(request);
            }

            foreach (var guid in RegisteredRequestsByUuid.Keys)
            {
                EtpMessage request;
                if (!RegisteredRequestsByUuid.TryGetValue(guid, out request))
                    continue;

                var elapsedTime = now - request.Header.Timestamp;
                if (elapsedTime < timeout)
                    continue;

                if (!RegisteredRequestsByUuid.TryRemove(guid, out _))
                    continue;

                HandleTimedOutRequest(request);
            }
        }

        /// <summary>
        /// Handles background clean up
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        private async Task BackgroundCleanUpLoop(CancellationToken token)
        {
            while (true)
            {
                try
                {
                    BackgroundCleanUp();
                    await Task.Delay(1000, token);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    Logger.Debug(ex);
                }
            }
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
                StopBackgroundCleanUpLoop();
                _disposedValue = true;
            }
        }
    }
}

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
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Protocol.Core;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Defines properties and methods that can be used to handle ETP messages.
    /// </summary>
    public interface IProtocolHandler : IDisposable, ISessionProtocol
    {
        /// <summary>
        /// Creates a deep copy of this instance.
        /// </summary>
        /// <returns>A deep copy of this instance.</returns>
        IProtocolHandler Clone();

        /// <summary>
        /// Gets or sets the ETP session.
        /// </summary>
        /// <value>The session.</value>
        IEtpSession Session { get; }

        /// <summary>
        /// Sets the protocol handler's session.
        /// </summary>
        /// <param name="session">The ETP session.</param>
        void SetSession(IEtpSession session);

        /// <summary>
        /// Sets the capabilities for the handler's counterpart.
        /// </summary>
        /// <param name="counterpartCapabilities">The counterpart's protocol capabilities.</param>
        void SetCounterpartCapabilities(IProtocolCapabilities counterpartCapabilities);

        /// <summary>
        /// Start the protocol handler when the session has been opened.
        /// </summary>
        void StartHandling();

        /// <summary>
        /// Event raised when the handler is started.
        /// </summary>
        event EventHandler<EventArgs> OnStarted;

        /// <summary>
        /// Stop the protocol handler when the ETP session has been closed.
        /// </summary>
        void StopHandling();

        /// <summary>
        /// Event raised when the handler is started.
        /// </summary>
        event EventHandler<EventArgs> OnStopped;

        /// <summary>
        /// Sends an Acknowledge message in response to the message associated with the correlation header.
        /// </summary>
        /// <param name="correlatedHeader">The message header the acknowledge message is correlated with.</param>
        /// <param name="isNoData">Whether or not the acknowledge message should have the NoData flag set.</param>
        /// <param name="extension">The message header extension to send with the message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<IAcknowledge> Acknowledge(IMessageHeader correlatedHeader, bool isNoData = false, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Constructs a new <see cref="IErrorInfo"/> instance compatible with the session.
        /// </summary>
        /// <returns>The constructed error info.</returns>
        IErrorInfo ErrorInfo();

        /// <summary>
        /// Sends a ProtocolException message with the specified exception details.
        /// </summary>
        /// <param name="error">The error in the protocol exception.</param>
        /// <param name="correlatedHeader">The message header the protocol exception is correlated with, if any.</param>
        /// <param name="isFinalPart">Whether or not the protocol exception is the final part in a multi-part message.</param>
        /// <param name="extension">The message header extension to send with the message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<IProtocolException> ProtocolException(IErrorInfo error, IMessageHeader correlatedHeader = null, bool isFinalPart = false, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a ProtocolException message(s) with the specified exception details.
        /// </summary>
        /// <param name="errors">The errors in the protocol exception.</param>
        /// <param name="correlatedHeader">The message header the protocol exception is correlated with, if any.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="extension">The message header extension to send with the message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        EtpMessage<IProtocolException> ProtocolException(IDictionary<string, IErrorInfo> errors, IMessageHeader correlatedHeader = null, bool setFinalPart = true, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a ProtocolException message with the specified exception details.
        /// </summary>
        /// <param name="exception">The ETP exception.</param>
        /// <param name="isFinalPart">Whether or not the protocol exception is the final part in a multi-part message.</param>
        /// <param name="extension">The message header extension to send with the message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<IProtocolException> ProtocolException(EtpException exception, bool isFinalPart = false, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Returns whether this handler can handle a message with the specified message body type.
        /// </summary>
        /// <param name="messageBodyType">The message body type</param>
        /// <returns><c>true</c> if this handler can handle messages with the specified message body type; <c>false</c> otherwise.</returns>
        bool CanHandleMessage(Type messageBodyType);

        /// <summary>
        /// Handles the message based on the message type contained in the specified <see cref="IMessageHeader" />.
        /// </summary>
        /// <param name="message">The message.</param>
        void HandleMessage(EtpMessage message);

        /// <summary>
        /// Occurs when an Acknowledge message is received for the current protocol.
        /// </summary>
        event EventHandler<MessageEventArgs<IAcknowledge>> OnAcknowledge;

        /// <summary>
        /// Occurs when a ProtocolException message is received for the current protocol.
        /// </summary>
        event EventHandler<MessageEventArgs<IProtocolException>> OnProtocolException;

        /// <summary>
        /// Occurs when no response has been received in a timely fashion to a previously sent request message.
        /// </summary>
        event EventHandler<MessageEventArgs> OnRequestTimedOut;
    }
}

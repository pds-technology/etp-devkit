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
using Avro.IO;
using Avro.Specific;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Protocol.Core;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Defines properties and methods that can be used to handle ETP messages.
    /// </summary>
    public interface IProtocolHandler : IDisposable
    {
        /// <summary>
        /// The ETP version supported by this handler.
        /// </summary>
        EtpVersion SupportedVersion { get; }

        /// <summary>
        /// Gets or sets the ETP session.
        /// </summary>
        /// <value>The session.</value>
        IEtpSession Session { get; set; }

        /// <summary>
        /// Gets the protocol.
        /// </summary>
        /// <value>The protocol.</value>
        int Protocol { get; }

        /// <summary>
        /// Gets the role.
        /// </summary>
        /// <value>The role.</value>
        string Role { get; }

        /// <summary>
        /// Gets the requested role.
        /// </summary>
        /// <value>The requested role.</value>
        string RequestedRole { get; }

        /// <summary>
        /// Gets the capabilities supported by the protocol handler.
        /// </summary>
        /// <returns>A collection of protocol capabilities.</returns>
        IDictionary<string, IDataValue> GetCapabilities();

        /// <summary>
        /// Called when the ETP session is opened.
        /// </summary>
        /// <param name="requestedProtocols">The requested protocols.</param>
        /// <param name="supportedProtocols">The supported protocols.</param>
        void OnSessionOpened(IList<ISupportedProtocol> requestedProtocols, IList<ISupportedProtocol> supportedProtocols);

        /// <summary>
        /// Called when the ETP session is opened.
        /// </summary>
        void OnSessionClosed();

        /// <summary>
        /// Sends an Acknowledge message with the specified correlation identifier and message flag.
        /// </summary>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        long Acknowledge(long correlationId, MessageFlags messageFlag = MessageFlags.None);

        /// <summary>
        /// Sends a ProtocolException message with the specified error code, message and correlation identifier.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <returns>The message identifier.</returns>
        long ProtocolException(int errorCode, string errorMessage, long correlationId = 0);

        /// <summary>
        /// Decodes the message based on the message type contained in the specified <see cref="IMessageHeader" />.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="body">The message body.</param>
        void HandleMessage(IMessageHeader header, ISpecificRecord body);

        /// <summary>
        /// Occurs when an Acknowledge message is received for the current protocol.
        /// </summary>
        event ProtocolEventHandler<IAcknowledge> OnAcknowledge;

        /// <summary>
        /// Occurs when a ProtocolException message is received for the current protocol.
        /// </summary>
        event ProtocolEventHandler<IProtocolException> OnProtocolException;
    }
}

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
        /// Gets this handler's role in the protocol.
        /// </summary>
        /// <value>This handler's role in the protocol.</value>
        string Role { get; }

        /// <summary>
        /// Gets the role for this handler's counterpart in the protocol.
        /// </summary>
        /// <value>The role for this handler's counterpart in the protocol.</value>
        string CounterpartRole { get; }

        /// <summary>
        /// Gets the capabilities supported by the protocol handler.
        /// </summary>
        /// <param name="capabilities">The protocol's capabilities.</param>
        void GetCapabilities(EtpProtocolCapabilities capabilities);

        /// <summary>
        /// Called when the ETP session is opened.
        /// </summary>
        /// <param name="counterpartCapabilities">The counterpart's protocol capabilities.</param>
        void OnSessionOpened(EtpProtocolCapabilities counterpartCapabilities);

        /// <summary>
        /// Called when the ETP session is opened.
        /// </summary>
        void OnSessionClosed();

        /// <summary>
        /// Sends an Acknowledge message with the specified correlation identifier and message flag.
        /// </summary>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long Acknowledge(long correlationId, MessageFlags messageFlag = MessageFlags.None);

        /// <summary>
        /// Sends a ProtocolException message with the specified exception details.
        /// </summary>
        /// <param name="exception">The ETP exception.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long ProtocolException(EtpException exception);

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

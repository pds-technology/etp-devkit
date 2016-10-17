//----------------------------------------------------------------------- 
// ETP DevKit, 1.1
//
// Copyright 2016 Energistics
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
using Energistics.Datatypes;
using Energistics.Protocol.Core;

namespace Energistics.Common
{
    /// <summary>
    /// Defines static helper methods that can be used to send <see cref="ProtocolException"/> messages.
    /// </summary>
    public static class EtpErrorMessages
    {
        /// <summary>
        /// Sends a ProtocolException message for an invalid message type.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="header">The message header.</param>
        /// <returns>The message identifier.</returns>
        public static long InvalidMessage(this IProtocolHandler handler, MessageHeader header)
        {
            return handler.ProtocolException((int)EtpErrorCodes.InvalidMessageType, "Invalid message type: " + header.MessageType, header.MessageId);
        }

        /// <summary>
        /// Sends a <see cref="ProtocolException"/> message for an invalid argument.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="ProtocolException"/> message identifier.</returns>
        public static long InvalidArgument(this EtpProtocolHandler handler, object value, long messageId = 0)
        {
            return handler.ProtocolException((int)EtpErrorCodes.InvalidArgument, "Invalid Argument: " + value, messageId);
        }

        /// <summary>
        /// Sends a <see cref="ProtocolException"/> message for an unsupported object.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="ex">The exception.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="ProtocolException"/> message identifier.</returns>
        public static long UnsupportedObject(this EtpProtocolHandler handler, Exception ex, string uri, long messageId = 0)
        {
            if (ex != null)
            {
                handler.Logger?.Error(ex);
            }

            return handler.ProtocolException((int)EtpErrorCodes.UnsupportedObject, "Data object not supported. URI: " + uri, messageId);
        }

        /// <summary>
        /// Sends a <see cref="ProtocolException"/> message for an invalid state.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="ProtocolException"/> message identifier.</returns>
        public static long InvalidState(this EtpProtocolHandler handler, object value, long messageId = 0)
        {
            return handler.ProtocolException((int)EtpErrorCodes.InvalidState, "Invalid State: " + value, messageId);
        }

        /// <summary>
        /// Sends a <see cref="ProtocolException"/> message for an invalid URI.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="ProtocolException"/> message identifier.</returns>
        public static long InvalidUri(this EtpProtocolHandler handler, object value, long messageId = 0)
        {
            return handler.ProtocolException((int)EtpErrorCodes.InvalidUri, "Invalid Uri: " + value, messageId);
        }

        /// <summary>
        /// Sends a <see cref="ProtocolException"/> message for an invalid ChannelId.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="ProtocolException"/> message identifier.</returns>
        public static long InvalidChannelId(this EtpProtocolHandler handler, object value, long messageId = 0)
        {
            return handler.ProtocolException((int)EtpErrorCodes.InvalidChannelId, "Invalid ChannelId: " + value, messageId);
        }
    }
}

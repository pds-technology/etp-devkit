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

using Energistics.Etp.Common.Datatypes;
using CoreMessageTypes = Energistics.Etp.v11.MessageTypes.Core;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Provides extension methods that can be used along with ETP message headers.
    /// </summary>
    public static class EtpMessageHeaderExtensions
    {
        /// <summary>
        /// Checks if this header is from a protocol exception message.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <returns><c>true</c> if the header is from a protocol exception; <c>false</c> otherwise.</returns>
        public static bool IsProtocolException(this IMessageHeader header)
        {
            return header.MessageType == (int)CoreMessageTypes.ProtocolException;
        }

        /// <summary>
        /// Checks if this header is from an acknowledge message.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <returns><c>true</c> if the header is from an acknowledge message; <c>false</c> otherwise.</returns>
        public static bool IsAcknowledge(this IMessageHeader header)
        {
            return header.MessageType == (int)CoreMessageTypes.Acknowledge;
        }

        /// <summary>
        /// Checks if this header is from a request session message.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <returns><c>true</c> if the header is from a request session message; <c>false</c> otherwise.</returns>
        public static bool IsRequestSession(this IMessageHeader header)
        {
            return header.MessageType == (int)CoreMessageTypes.RequestSession;
        }

        /// <summary>
        /// Checks if this header is from an open session message.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <returns><c>true</c> if the header is from an open session message; <c>false</c> otherwise.</returns>
        public static bool IsOpenSession(this IMessageHeader header)
        {
            return header.MessageType == (int)CoreMessageTypes.OpenSession;
        }

        /// <summary>
        /// Checks if this header has a correlation ID.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <returns><c>true</c> if the correlation ID is greater than 0; <c>false</c> otherwise.</returns>
        public static bool HasCorrelationId(this IMessageHeader header)
        {
            return header.CorrelationId > 0;
        }

        /// <summary>
        /// Checks if the header has the specified <see cref="MessageFlags"/> set.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="messageFlag">The ETP message flag.</param>
        /// <returns><c>true</c> if the header has the specified flag set; <c>false</c> otherwise.</returns>
        private static bool HasFlag(this IMessageHeader header, MessageFlags messageFlag)
        {
            return ((MessageFlags)header.MessageFlags).HasFlag(messageFlag);
        }

        /// <summary>
        /// Sets the specified <see cref="MessageFlags"/> on the header.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="messageFlag">The ETP message flag to set.</param>
        private static void SetFlag(this IMessageHeader header, MessageFlags messageFlag)
        {
            header.MessageFlags |= (int)messageFlag;
        }

        /// <summary>
        /// Checks if this header is part of a multi-part message.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <returns><c>true</c> if the header is part of a multi-part message; <c>false</c> otherwise.</returns>
        public static bool IsMultiPart(this IMessageHeader header)
        {
            return header.HasFlag(MessageFlags.MultiPart);
        }

        /// <summary>
        /// Sets the multi-part flag on the message header.
        /// </summary>
        /// <param name="header">The message header.</param>
        public static void SetMultiPart(this IMessageHeader header)
        {
            header.SetFlag(MessageFlags.MultiPart);
        }

        /// <summary>
        /// Checks if this header is the final part of a multi-part message.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <returns><c>true</c> if the header is the final part of a multi-part message; <c>false</c> otherwise.</returns>
        public static bool IsFinalPart(this IMessageHeader header)
        {
            return header.HasFlag(MessageFlags.FinalPart);
        }

        /// <summary>
        /// Sets the multi-part and final part flags on the message header.
        /// </summary>
        /// <param name="header">The message header.</param>
        public static void SetMultiPartAndFinalPart(this IMessageHeader header)
        {
            header.SetFlag(MessageFlags.MultiPartAndFinalPart);
        }

        /// <summary>
        /// Checks if there is no data.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <returns><c>true</c> if there is no data; <c>false</c> otherwise.</returns>
        public static bool IsNoData(this IMessageHeader header)
        {
            return header.HasFlag(MessageFlags.NoData);
        }

        /// <summary>
        /// Sets the no data flag on the message header.
        /// </summary>
        /// <param name="header">The message header.</param>
        public static void SetNoData(this IMessageHeader header)
        {
            header.SetFlag(MessageFlags.NoData);
        }

        /// <summary>
        /// Checks if the message body is compressed.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <returns><c>true</c> if the message body is compressed; <c>false</c> otherwise.</returns>
        public static bool IsBodyCompressed(this IMessageHeader header)
        {
            return header.HasFlag(MessageFlags.Compressed);
        }

        /// <summary>
        /// Sets the body compressed flag on the message header.
        /// </summary>
        /// <param name="header">The message header.</param>
        public static void SetBodyCompressed(this IMessageHeader header)
        {
            header.SetFlag(MessageFlags.Compressed);
        }

        /// <summary>
        /// Checks if an acknowledge message is requested.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <returns><c>true</c> if an acknowledge message is requested; <c>false</c> otherwise.</returns>
        public static bool IsAcknowledgeRequested(this IMessageHeader header)
        {
            return header.HasFlag(MessageFlags.Acknowledge);
        }

        /// <summary>
        /// Sets the acknowledge requested flag on the message header.
        /// </summary>
        /// <param name="header">The message header.</param>
        public static void SetAcknowledgeRequested(this IMessageHeader header)
        {
            header.SetFlag(MessageFlags.Acknowledge);
        }

        /// <summary>
        /// Checks if this message is the only or final message in response to a request.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <returns><c>true</c> if this is an acknowledge message with NoData set,
        /// a protocol exception, or a different message type that is either the final
        /// part of a multi-part message or not part of a multi-part message;
        /// <c>false</c> otherwise.</returns>
        public static bool IsFinalResponse(this IMessageHeader header)
        {
            if (!header.HasCorrelationId())
                return false;

            if (header.IsAcknowledge() && header.IsNoData())
                return true;

            if (header.IsProtocolException())
                return true;

            return !header.IsMultiPart() || header.IsFinalPart();
        }

        /// <summary>
        /// Determines whether the message body can be compressed based on the specified header.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="checkMessageFlags">A flag to check the message flags provided in the header.</param>
        /// <returns><c>true</c> if the message body can be comressed; otherwise, <c>false</c>.</returns>
        public static bool CanCompressMessageBody(this IMessageHeader header, bool checkMessageFlags = false)
        {
            // Never compress RequestSession or OpenSession in Core protocol
            if (header.Protocol == 0 && (header.IsRequestSession() || header.IsOpenSession()))
                return false;

            // Don't compress Acknowledge or ProtocolException when sent by any protocol
            if (header.IsAcknowledge() || header.IsProtocolException())
                return false;

            // Do the message flags indicate the body was compressed
            return !checkMessageFlags || header.HasFlag(MessageFlags.Compressed);
        }

    }
}

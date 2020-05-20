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
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Protocol.Core;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Defines static helper methods that can be used to send <see cref="IProtocolException"/> messages.
    /// </summary>
    public static class EtpProtocolExceptions
    {
        /// <summary>
        /// Sends a ProtocolException message for a unset type.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="header">The message header.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public static long Unset(this IProtocolHandler handler, IMessageHeader header)
        {
            try { handler.UnsetException(header); }
            catch (EtpException ex) { return handler.ProtocolException(ex); }
            return -1;
        }

        /// <summary>
        /// Sends a ProtocolException message for a no role type.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="header">The message header.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public static long NoRole(this IProtocolHandler handler, IMessageHeader header)
        {
            try { handler.NoRoleException(header); }
            catch (EtpException ex) { return handler.ProtocolException(ex); }
            return -1;
        }

        /// <summary>
        /// Sends a ProtocolException message for a no supported protocols type.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="header">The message header.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public static long NoSupportedProtocols(this IProtocolHandler handler, IMessageHeader header)
        {
            try { handler.NoSupportedProtocolsException(header); }
            catch (EtpException ex) { return handler.ProtocolException(ex); }
            return -1;
        }

        /// <summary>
        /// Sends a ProtocolException message for an invalid message type.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="header">The message header.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public static long InvalidMessage(this IProtocolHandler handler, IMessageHeader header)
        {
            try { handler.InvalidMessageException(header); }
            catch (EtpException ex) { return handler.ProtocolException(ex); }
            return -1;
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an unsupported protocol.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The positive <see cref="IProtocolException"/> message identifier on success; otherwise, a negative number.</returns>
        public static long UnsupportedProtocol(this IProtocolHandler handler, object value, long messageId = 0)
        {
            try { handler.UnsupportedProtocolException(value, messageId); }
            catch (EtpException ex) { return handler.ProtocolException(ex); }
            return -1;
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an invalid argument.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The positive <see cref="IProtocolException"/> message identifier on success; otherwise, a negative number.</returns>
        public static long InvalidArgument(this IProtocolHandler handler, object value, long messageId = 0)
        {
            try { handler.InvalidArgumentException(value, messageId); }
            catch (EtpException ex) { return handler.ProtocolException(ex); }
            return -1;
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for permission denied.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The positive <see cref="IProtocolException"/> message identifier on success; otherwise, a negative number.</returns>
        public static long PermissionDenied(this IProtocolHandler handler, object value, long messageId = 0)
        {
            try { handler.PermissionDeniedException(value, messageId); }
            catch (EtpException ex) { return handler.ProtocolException(ex); }
            return -1;
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for not supported.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The positive <see cref="IProtocolException"/> message identifier on success; otherwise, a negative number.</returns>
        public static long NotSupported(this IProtocolHandler handler, object value, long messageId = 0)
        {
            try { handler.NotSupportedException(value, messageId); }
            catch (EtpException ex) { return handler.ProtocolException(ex); }
            return -1;
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an invalid state.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The positive <see cref="IProtocolException"/> message identifier on success; otherwise, a negative number.</returns>
        public static long InvalidState(this IProtocolHandler handler, object value, long messageId = 0)
        {
            try { handler.InvalidStateException(value, messageId); }
            catch (EtpException ex) { return handler.ProtocolException(ex); }
            return -1;
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an invalid URI.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The positive <see cref="IProtocolException"/> message identifier on success; otherwise, a negative number.</returns>
        public static long InvalidUri(this IProtocolHandler handler, object value, long messageId = 0)
        {
            try { handler.InvalidUriException(value, messageId); }
            catch (EtpException ex) { return handler.ProtocolException(ex); }
            return -1;
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an expired token.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The positive <see cref="IProtocolException"/> message identifier on success; otherwise, a negative number.</returns>
        public static long ExpiredToken(this IProtocolHandler handler, object value, long messageId = 0)
        {
            try { handler.ExpiredTokenException(value, messageId); }
            catch (EtpException ex) { return handler.ProtocolException(ex); }
            return -1;
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for object not found.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The positive <see cref="IProtocolException"/> message identifier on success; otherwise, a negative number.</returns>
        public static long NotFound(this IProtocolHandler handler, object value, long messageId = 0)
        {
            try { handler.NotFoundException(value, messageId); }
            catch (EtpException ex) { return handler.ProtocolException(ex); }
            return -1;
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for limit exceeded.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The positive <see cref="IProtocolException"/> message identifier on success; otherwise, a negative number.</returns>
        public static long LimitExceeded(this IProtocolHandler handler, object value, long messageId = 0)
        {
            try { handler.LimitExceededException(value, messageId); }
            catch (EtpException ex) { return handler.ProtocolException(ex); }
            return -1;
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException" /> message for compression not supported.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="requestedCompression">The requested compression.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException" /> message identifier.</returns>
        public static long CompressionNotSupported(this IProtocolHandler handler, string requestedCompression, long messageId = 0)
        {
            try { handler.CompressionNotSupportedException(requestedCompression, messageId); }
            catch (EtpException ex) { return handler.ProtocolException(ex); }
            return -1;
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an invalid object.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="message">The optional error message.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The positive <see cref="IProtocolException"/> message identifier on success; otherwise, a negative number.</returns>
        public static long InvalidObject(this IProtocolHandler handler, Exception exception, string uri, string message = null, long messageId = 0)
        {
            try { handler.InvalidObjectException(exception, uri, message, messageId); }
            catch (EtpException ex)
            {
                if (exception != null)
                {
                    (handler as EtpProtocolHandler)?.Logger?.LogErrorInfo(ex.ErrorInfo, ex);
                }

                return handler.ProtocolException(ex);
            }
            return -1;
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an invalid ChannelId.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The positive <see cref="IProtocolException"/> message identifier on success; otherwise, a negative number.</returns>
        public static long InvalidChannelId(this IProtocolHandler handler, object value, long messageId = 0)
        {
            try { handler.InvalidChannelIdException(value, messageId); }
            catch (EtpException ex) { return handler.ProtocolException(ex); }
            return -1;
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an unsupported object.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The positive <see cref="IProtocolException"/> message identifier on success; otherwise, a negative number.</returns>
        public static long UnsupportedObject(this IProtocolHandler handler, Exception exception, string uri, long messageId = 0)
        {
            try { handler.UnsupportedObjectException(exception, uri, messageId); }
            catch (EtpException ex)
            {
                if (exception != null)
                {
                    (handler as EtpProtocolHandler)?.Logger?.LogErrorInfo(ex.ErrorInfo, ex);
                }

                return handler.ProtocolException(ex);
            }
            return -1;
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an invalid object X.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The positive <see cref="IProtocolException"/> message identifier on success; otherwise, a negative number.</returns>
        public static long InvalidObjectX(this IProtocolHandler handler, Exception exception, string uri, long messageId = 0)
        {
            try { handler.InvalidObjectXException(exception, uri, messageId); }
            catch (EtpException ex)
            {
                if (exception != null)
                {
                    (handler as EtpProtocolHandler)?.Logger?.LogErrorInfo(ex.ErrorInfo, ex);
                }

                return handler.ProtocolException(ex);
            }
            return -1;
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for no cascade delete.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The positive <see cref="IProtocolException"/> message identifier on success; otherwise, a negative number.</returns>
        public static long NoCascadeDelete(this IProtocolHandler handler, string uri, long messageId = 0)
        {
            try { handler.NoCascadeDeleteException(uri, messageId); }
            catch (EtpException ex) { return handler.ProtocolException(ex); }
            return -1;
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for no plural object.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The positive <see cref="IProtocolException"/> message identifier on success; otherwise, a negative number.</returns>
        public static long NoPluralObject(this IProtocolHandler handler, string uri, long messageId = 0)
        {
            try { handler.NoPluralObjectException(uri, messageId); }
            catch (EtpException ex) { return handler.ProtocolException(ex); }
            return -1;
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for ignoring the growing portion.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The positive <see cref="IProtocolException"/> message identifier on success; otherwise, a negative number.</returns>
        public static long GrowingPortionIgnored(this IProtocolHandler handler, string uri, long messageId = 0)
        {
            try { handler.GrowingPortionIgnoredException(uri, messageId); }
            catch (EtpException ex) { return handler.ProtocolException(ex); }
            return -1;
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for retention period exceeded.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The positive <see cref="IProtocolException"/> message identifier on success; otherwise, a negative number.</returns>
        public static long RetentionPeriodExceeded(this IProtocolHandler handler, long messageId = 0)
        {
            try { handler.RetentionPeriodExceededException(messageId); }
            catch (EtpException ex) { return handler.ProtocolException(ex); }
            return -1;
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for not growing object.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The positive <see cref="IProtocolException"/> message identifier on success; otherwise, a negative number.</returns>
        public static long NotGrowingObject(this IProtocolHandler handler, string uri, long messageId = 0)
        {
            try { handler.NotGrowingObjectException(uri, messageId); }
            catch (EtpException ex) { return handler.ProtocolException(ex); }
            return -1;
        }
    }
}

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
    /// Defines static helper methods that can be used to throw <see cref="EtpException"/> instances.
    /// </summary>
    public static class EtpExceptions
    {
        /// <summary>
        /// Sends a ProtocolException message for a unset type.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="header">The message header.</param>
        /// <returns>The message identifier.</returns>
        public static void UnsetException(this IProtocolHandler handler, IMessageHeader header)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().Unset(header.MessageType);
            throw new EtpException(errorInfo, header.MessageId);
        }

        /// <summary>
        /// Sends a ProtocolException message for a no role type.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="header">The message header.</param>
        /// <returns>The message identifier.</returns>
        public static void NoRoleException(this IProtocolHandler handler, IMessageHeader header)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().NoRole(header.MessageType);
            throw new EtpException(errorInfo, header.MessageId);
        }

        /// <summary>
        /// Sends a ProtocolException message for a no supported protocols type.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="header">The message header.</param>
        /// <returns>The message identifier.</returns>
        public static void NoSupportedProtocolsException(this IProtocolHandler handler, IMessageHeader header)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().NoSupportedProtocols(header.MessageType);
            throw new EtpException(errorInfo, header.MessageId);
        }

        /// <summary>
        /// Sends a ProtocolException message for an invalid message type.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="header">The message header.</param>
        /// <returns>The message identifier.</returns>
        public static void InvalidMessageException(this IProtocolHandler handler, IMessageHeader header)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().InvalidMessage(header.MessageType);
            throw new EtpException(errorInfo, header.MessageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an unsupported protocol.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static void UnsupportedProtocolException(this IProtocolHandler handler, object value, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().UnsupportedProtocol(value);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an invalid argument.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static void InvalidArgumentException(this IProtocolHandler handler, object value, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().InvalidArgument(value);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for permission denied.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static void PermissionDeniedException(this IProtocolHandler handler, object value, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().PermissionDenied(value);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for not supported.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static void NotSupportedException(this IProtocolHandler handler, object value, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().NotSupported(value);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an invalid state.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static void InvalidStateException(this IProtocolHandler handler, object value, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().InvalidState(value);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an invalid URI.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static void InvalidUriException(this IProtocolHandler handler, object value, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().InvalidUri(value);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an expired token.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static void ExpiredTokenException(this IProtocolHandler handler, object value, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().ExpiredToken(value);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for object not found.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static void NotFoundException(this IProtocolHandler handler, object value, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().NotFound(value);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for limit exceeded.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static void LimitExceededException(this IProtocolHandler handler, object value, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().LimitExceeded(value);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException" /> message for compression not supported.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="requestedCompression">The requested compression.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException" /> message identifier.</returns>
        public static void CompressionNotSupportedException(this IProtocolHandler handler, string requestedCompression, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().CompressionNotSupported(requestedCompression);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an invalid object.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="ex">The exception.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="message">The optional error message.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static void InvalidObjectException(this IProtocolHandler handler, Exception ex, string uri, string message = null, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().InvalidObject(uri, message);
            if (ex != null)
            {
                (handler as EtpProtocolHandler)?.Logger?.LogErrorInfo(errorInfo, ex);
            }

            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an invalid ChannelId.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static void InvalidChannelIdException(this IProtocolHandler handler, object value, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().InvalidChannelId(value);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an unsupported object.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="ex">The exception.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static void UnsupportedObjectException(this IProtocolHandler handler, Exception ex, string uri, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().UnsupportedObject(uri);
            if (ex != null)
            {
                (handler as EtpProtocolHandler)?.Logger?.LogErrorInfo(errorInfo, ex);
            }

            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an invalid object X.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="ex">The exception.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static void InvalidObjectXException(this IProtocolHandler handler, Exception ex, string uri, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().InvalidObjectX(uri);
            if (ex != null)
            {
                (handler as EtpProtocolHandler)?.Logger?.LogErrorInfo(errorInfo, ex);
            }

            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for no cascade delete.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static void NoCascadeDeleteException(this IProtocolHandler handler, string uri, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().NoCascadeDelete(uri);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for no plural object.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static void NoPluralObjectException(this IProtocolHandler handler, string uri, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().NoPluralObject(uri);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for ignoring the growing portion.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static void GrowingPortionIgnoredException(this IProtocolHandler handler, string uri, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().GrowingPortionIgnored(uri);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for retention period exceeded.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static void RetentionPeriodExceededException(this IProtocolHandler handler, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().RetentionPeriodExceeded();
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for not growing object.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static void NotGrowingObjectException(this IProtocolHandler handler, string uri, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().NotGrowingObject(uri);
            throw new EtpException(errorInfo, messageId);
        }
    }
}

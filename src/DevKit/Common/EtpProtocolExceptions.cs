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
        /// <returns>The message identifier.</returns>
        public static long Unset(this IProtocolHandler handler, IMessageHeader header)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().Unset(header.MessageType);
            return handler.ProtocolException(errorInfo, header.MessageId);
        }

        /// <summary>
        /// Sends a ProtocolException message for a no role type.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="header">The message header.</param>
        /// <returns>The message identifier.</returns>
        public static long NoRole(this IProtocolHandler handler, IMessageHeader header)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().NoRole(header.MessageType);
            return handler.ProtocolException(errorInfo, header.MessageId);
        }

        /// <summary>
        /// Sends a ProtocolException message for a no supported protocols type.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="header">The message header.</param>
        /// <returns>The message identifier.</returns>
        public static long NoSupportedProtocols(this IProtocolHandler handler, IMessageHeader header)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().NoSupportedProtocols(header.MessageType);
            return handler.ProtocolException(errorInfo, header.MessageId);
        }

        /// <summary>
        /// Sends a ProtocolException message for an invalid message type.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="header">The message header.</param>
        /// <returns>The message identifier.</returns>
        public static long InvalidMessage(this IProtocolHandler handler, IMessageHeader header)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().InvalidMessage(header.MessageType);
            return handler.ProtocolException(errorInfo, header.MessageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an unsupported protocol.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static long UnsupportedProtocol(this IProtocolHandler handler, object value, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().UnsupportedProtocol(value);
            return handler.ProtocolException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an invalid argument.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static long InvalidArgument(this IProtocolHandler handler, object value, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().InvalidArgument(value);
            return handler.ProtocolException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for permission denied.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static long PermissionDenied(this IProtocolHandler handler, object value, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().PermissionDenied(value);
            return handler.ProtocolException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for not supported.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static long NotSupported(this IProtocolHandler handler, object value, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().NotSupported(value);
            return handler.ProtocolException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an invalid state.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static long InvalidState(this IProtocolHandler handler, object value, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().InvalidState(value);
            return handler.ProtocolException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an invalid URI.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static long InvalidUri(this IProtocolHandler handler, object value, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().InvalidUri(value);
            return handler.ProtocolException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an expired token.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static long ExpiredToken(this IProtocolHandler handler, object value, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().ExpiredToken(value);
            return handler.ProtocolException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for object not found.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static long NotFound(this IProtocolHandler handler, object value, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().NotFound(value);
            return handler.ProtocolException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for limit exceeded.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static long LimitExceeded(this IProtocolHandler handler, object value, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().LimitExceeded(value);
            return handler.ProtocolException(errorInfo, messageId);
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
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().CompressionNotSupported(requestedCompression);
            return handler.ProtocolException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an invalid object.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="ex">The exception.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static long InvalidObject(this IProtocolHandler handler, Exception ex, string uri, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().InvalidObject(uri);
            if (ex != null)
            {
                (handler as EtpProtocolHandler)?.Logger?.Debug(errorInfo.Message, ex);
            }

            return handler.ProtocolException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an invalid ChannelId.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static long InvalidChannelId(this IProtocolHandler handler, object value, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().InvalidChannelId(value);
            return handler.ProtocolException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an unsupported object.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="ex">The exception.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static long UnsupportedObject(this IProtocolHandler handler, Exception ex, string uri, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().UnsupportedObject(uri);
            if (ex != null)
            {
                (handler as EtpProtocolHandler)?.Logger?.Debug(errorInfo.Message, ex);
            }

            return handler.ProtocolException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an invalid object X.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="ex">The exception.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static long InvalidObjectX(this IProtocolHandler handler, Exception ex, string uri, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().InvalidObjectX(uri);
            if (ex != null)
            {
                (handler as EtpProtocolHandler)?.Logger?.Debug(errorInfo.Message, ex);
            }

            return handler.ProtocolException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for no cascade delete.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static long NoCascadeDelete(this IProtocolHandler handler, string uri, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().NoCascadeDelete(uri);
            return handler.ProtocolException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for no plural object.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static long NoPluralObject(this IProtocolHandler handler, string uri, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().NoPluralObject(uri);
            return handler.ProtocolException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for ignoring the growing portion.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static long GrowingPortionIgnored(this IProtocolHandler handler, string uri, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().GrowingPortionIgnored(uri);
            return handler.ProtocolException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for retention period exceeded.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static long RetentionPeriodExceeded(this IProtocolHandler handler, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().RetentionPeriodExceeded();
            return handler.ProtocolException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for not growing object.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static long NotGrowingObject(this IProtocolHandler handler, string uri, long messageId = 0)
        {
            var errorInfo = handler.Session.Adapter.CreateErrorInfo().NotGrowingObject(uri);
            return handler.ProtocolException(errorInfo, messageId);
        }
    }
}

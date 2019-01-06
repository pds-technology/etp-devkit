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
    public static class EtpErrorMessages
    {
        /// <summary>
        /// Sends a ProtocolException message for a unset type.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="header">The message header.</param>
        /// <returns>The message identifier.</returns>
        public static long Unset(this IProtocolHandler handler, IMessageHeader header)
        {
            return handler.ProtocolException((int)EtpErrorCodes.Unset, "Unset: " + header.MessageType, header.MessageId);
        }

        /// <summary>
        /// Sends a ProtocolException message for a no role type.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="header">The message header.</param>
        /// <returns>The message identifier.</returns>
        public static long NoRole(this IProtocolHandler handler, IMessageHeader header)
        {
            return handler.ProtocolException((int)EtpErrorCodes.NoRole, "No Role: " + header.MessageType, header.MessageId);
        }

        /// <summary>
        /// Sends a ProtocolException message for a no supported protocols type.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="header">The message header.</param>
        /// <returns>The message identifier.</returns>
        public static long NoSupportedProtocols(this IProtocolHandler handler, IMessageHeader header)
        {
            return handler.ProtocolException((int)EtpErrorCodes.NoSupportedProtocols, "No supported protocols: " + header.MessageType, header.MessageId);
        }

        /// <summary>
        /// Sends a ProtocolException message for an invalid message type.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="header">The message header.</param>
        /// <returns>The message identifier.</returns>
        public static long InvalidMessage(this IProtocolHandler handler, IMessageHeader header)
        {
            return handler.ProtocolException((int)EtpErrorCodes.InvalidMessageType, "Invalid message type: " + header.MessageType, header.MessageId);
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
            return handler.ProtocolException((int)EtpErrorCodes.UnsupportedProtocol, "Unsupported Protocol: " + value, messageId);
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
            return handler.ProtocolException((int)EtpErrorCodes.InvalidArgument, "Invalid Argument: " + value, messageId);
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
            return handler.ProtocolException((int)EtpErrorCodes.PermissionDenied, "Permission Denied: " + value, messageId);
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
            return handler.ProtocolException((int)EtpErrorCodes.NotSupported, "Not Supported: " + value, messageId);
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
            return handler.ProtocolException((int)EtpErrorCodes.InvalidState, "Invalid State: " + value, messageId);
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
            return handler.ProtocolException((int)EtpErrorCodes.InvalidUri, "Invalid Uri: " + value, messageId);
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
            return handler.ProtocolException((int)EtpErrorCodes.ExpiredToken, "Expired Token: " + value, messageId);
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
            return handler.ProtocolException((int)EtpErrorCodes.NotFound, "Not Found: " + value, messageId);
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
            return handler.ProtocolException((int)EtpErrorCodes.LimitExceeded, "Limit Exceeded: " + value, messageId);
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
            return handler.ProtocolException((int)EtpErrorCodes.CompressionNotSupported, "Compression Not Supported: " + requestedCompression, messageId);
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
            if (ex != null)
            {
                (handler as EtpProtocolHandler)?.Logger?.Debug($"Invalid Object: {uri}", ex);
            }

            return handler.ProtocolException((int)EtpErrorCodes.InvalidObject, "Invalid Object. URI: " + uri, messageId);
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
            return handler.ProtocolException((int)EtpErrorCodes.InvalidChannelId, "Invalid ChannelId: " + value, messageId);
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
            if (ex != null)
            {
                (handler as EtpProtocolHandler)?.Logger?.Debug($"Unsupported Object: {uri}", ex);
            }

            return handler.ProtocolException((int)EtpErrorCodes.UnsupportedObject, "Data object not supported. URI: " + uri, messageId);
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
            if (ex != null)
            {
                (handler as EtpProtocolHandler)?.Logger?.Debug($"Invalid Object XML: {uri}", ex);
            }

            return handler.ProtocolException((int)EtpErrorCodes.InvalidObjectX, "Invalid Object. URI: " + uri, messageId);
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
            return handler.ProtocolException((int)EtpErrorCodes.NoCascadeDelete, "If cascading deletes are not invoked, a client must only request deletion of bottom level data-objects such that all child data - objects are deleted before the parent is deleted. URI: " + uri, messageId);
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
            return handler.ProtocolException((int)EtpErrorCodes.NoPluralObject, "Client attempted to put more than one object under the plural root of a 1.x Energistics data object. ETP only supports a single data object, one XML document. URI: " + uri, messageId);
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
            return handler.ProtocolException((int)EtpErrorCodes.GrowingPortionIgnored, "The object is upserted, but the growing portion is ignored. URI: " + uri, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for retention period exceeded.
        /// </summary>
        /// <param name="handler">The protocol handler.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The <see cref="IProtocolException"/> message identifier.</returns>
        public static long RetentionPeriodExceeded(this IProtocolHandler handler, long messageId = 0)
        {
            return handler.ProtocolException((int)EtpErrorCodes.RetentionPeriodExceeded, "Retention Period Exceeded", messageId);
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
            return handler.ProtocolException((int)EtpErrorCodes.NotGrowingObjct, "Not Growing Object. URI: " + uri, messageId);
        }
    }
}

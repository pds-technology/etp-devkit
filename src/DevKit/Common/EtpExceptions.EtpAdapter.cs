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
    public static partial class EtpExceptions
    {
        /// <summary>
        /// Sends a ProtocolException message for a unset type.
        /// </summary>
        /// <param name="etpAdapter">The ETP Adapter.</param>
        /// <param name="header">The message header.</param>
        public static void UnsetException(this IEtpAdapter etpAdapter, IMessageHeader header)
        {
            var errorInfo = etpAdapter.CreateErrorInfo().Unset(header.MessageType);
            throw new EtpException(errorInfo, header.MessageId);
        }

        /// <summary>
        /// Sends a ProtocolException message for a no role type.
        /// </summary>
        /// <param name="etpAdapter">The ETP Adapter.</param>
        /// <param name="header">The message header.</param>
        public static void NoRoleException(this IEtpAdapter etpAdapter, IMessageHeader header)
        {
            var errorInfo = etpAdapter.CreateErrorInfo().NoRole(header.MessageType);
            throw new EtpException(errorInfo, header.MessageId);
        }

        /// <summary>
        /// Sends a ProtocolException message for a no supported protocols type.
        /// </summary>
        /// <param name="etpAdapter">The ETP Adapter.</param>
        /// <param name="header">The message header.</param>
        public static void NoSupportedProtocolsException(this IEtpAdapter etpAdapter, IMessageHeader header)
        {
            var errorInfo = etpAdapter.CreateErrorInfo().NoSupportedProtocols(header.MessageType);
            throw new EtpException(errorInfo, header.MessageId);
        }

        /// <summary>
        /// Sends a ProtocolException message for an invalid message type.
        /// </summary>
        /// <param name="etpAdapter">The ETP Adapter.</param>
        /// <param name="header">The message header.</param>
        public static void InvalidMessageException(this IEtpAdapter etpAdapter, IMessageHeader header)
        {
            var errorInfo = etpAdapter.CreateErrorInfo().InvalidMessage(header.MessageType);
            throw new EtpException(errorInfo, header.MessageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an unsupported protocol.
        /// </summary>
        /// <param name="etpAdapter">The ETP Adapter.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        public static void UnsupportedProtocolException(this IEtpAdapter etpAdapter, object value, long messageId = 0)
        {
            var errorInfo = etpAdapter.CreateErrorInfo().UnsupportedProtocol(value);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an invalid argument.
        /// </summary>
        /// <param name="etpAdapter">The ETP Adapter.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        public static void InvalidArgumentException(this IEtpAdapter etpAdapter, object value, long messageId = 0)
        {
            var errorInfo = etpAdapter.CreateErrorInfo().InvalidArgument(value);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for permission denied.
        /// </summary>
        /// <param name="etpAdapter">The ETP Adapter.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        public static void PermissionDeniedException(this IEtpAdapter etpAdapter, object value, long messageId = 0)
        {
            var errorInfo = etpAdapter.CreateErrorInfo().PermissionDenied(value);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for not supported.
        /// </summary>
        /// <param name="etpAdapter">The ETP Adapter.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        public static void NotSupportedException(this IEtpAdapter etpAdapter, object value, long messageId = 0)
        {
            var errorInfo = etpAdapter.CreateErrorInfo().NotSupported(value);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an invalid state.
        /// </summary>
        /// <param name="etpAdapter">The ETP Adapter.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        public static void InvalidStateException(this IEtpAdapter etpAdapter, object value, long messageId = 0)
        {
            var errorInfo = etpAdapter.CreateErrorInfo().InvalidState(value);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an invalid URI.
        /// </summary>
        /// <param name="etpAdapter">The ETP Adapter.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        public static void InvalidUriException(this IEtpAdapter etpAdapter, object value, long messageId = 0)
        {
            var errorInfo = etpAdapter.CreateErrorInfo().InvalidUri(value);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an expired token.
        /// </summary>
        /// <param name="etpAdapter">The ETP Adapter.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        public static void ExpiredTokenException(this IEtpAdapter etpAdapter, object value, long messageId = 0)
        {
            var errorInfo = etpAdapter.CreateErrorInfo().ExpiredToken(value);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for object not found.
        /// </summary>
        /// <param name="etpAdapter">The ETP Adapter.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        public static void NotFoundException(this IEtpAdapter etpAdapter, object value, long messageId = 0)
        {
            var errorInfo = etpAdapter.CreateErrorInfo().NotFound(value);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for limit exceeded.
        /// </summary>
        /// <param name="etpAdapter">The ETP Adapter.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        public static void LimitExceededException(this IEtpAdapter etpAdapter, object value, long messageId = 0)
        {
            var errorInfo = etpAdapter.CreateErrorInfo().LimitExceeded(value);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException" /> message for compression not supported.
        /// </summary>
        /// <param name="etpAdapter">The ETP Adapter.</param>
        /// <param name="requestedCompression">The requested compression.</param>
        /// <param name="messageId">The message identifier.</param>
        public static void CompressionNotSupportedException(this IEtpAdapter etpAdapter, string requestedCompression, long messageId = 0)
        {
            var errorInfo = etpAdapter.CreateErrorInfo().CompressionNotSupported(requestedCompression);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an invalid object.
        /// </summary>
        /// <param name="etpAdapter">The ETP Adapter.</param>
        /// <param name="ex">The exception.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="message">The optional error message.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>The positive <see cref="IProtocolException"/> message identifier on success; otherwise, a negative number.</returns>
        public static void InvalidObjectException(this IEtpAdapter etpAdapter, Exception ex, string uri, string message = null, long messageId = 0)
        {
            var errorInfo = etpAdapter.CreateErrorInfo().InvalidObject(uri, message);
            if (ex != null)
            {
                (etpAdapter as EtpProtocolHandler)?.Logger?.LogErrorInfo(errorInfo, ex);
            }

            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an invalid ChannelId.
        /// </summary>
        /// <param name="etpAdapter">The ETP Adapter.</param>
        /// <param name="value">The argument value.</param>
        /// <param name="messageId">The message identifier.</param>
        public static void InvalidChannelIdException(this IEtpAdapter etpAdapter, object value, long messageId = 0)
        {
            var errorInfo = etpAdapter.CreateErrorInfo().InvalidChannelId(value);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an unsupported object.
        /// </summary>
        /// <param name="etpAdapter">The ETP Adapter.</param>
        /// <param name="ex">The exception.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="messageId">The message identifier.</param>
        public static void UnsupportedObjectException(this IEtpAdapter etpAdapter, Exception ex, string uri, long messageId = 0)
        {
            var errorInfo = etpAdapter.CreateErrorInfo().UnsupportedObject(uri);
            if (ex != null)
            {
                (etpAdapter as EtpProtocolHandler)?.Logger?.LogErrorInfo(errorInfo, ex);
            }

            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for an invalid object X.
        /// </summary>
        /// <param name="etpAdapter">The ETP Adapter.</param>
        /// <param name="ex">The exception.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="messageId">The message identifier.</param>
        public static void InvalidObjectXException(this IEtpAdapter etpAdapter, Exception ex, string uri, long messageId = 0)
        {
            var errorInfo = etpAdapter.CreateErrorInfo().InvalidObjectX(uri);
            if (ex != null)
            {
                (etpAdapter as EtpProtocolHandler)?.Logger?.LogErrorInfo(errorInfo, ex);
            }

            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for no cascade delete.
        /// </summary>
        /// <param name="etpAdapter">The ETP Adapter.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="messageId">The message identifier.</param>
        public static void NoCascadeDeleteException(this IEtpAdapter etpAdapter, string uri, long messageId = 0)
        {
            var errorInfo = etpAdapter.CreateErrorInfo().NoCascadeDelete(uri);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for no plural object.
        /// </summary>
        /// <param name="etpAdapter">The ETP Adapter.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="messageId">The message identifier.</param>
        public static void NoPluralObjectException(this IEtpAdapter etpAdapter, string uri, long messageId = 0)
        {
            var errorInfo = etpAdapter.CreateErrorInfo().NoPluralObject(uri);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for ignoring the growing portion.
        /// </summary>
        /// <param name="etpAdapter">The ETP Adapter.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="messageId">The message identifier.</param>
        public static void GrowingPortionIgnoredException(this IEtpAdapter etpAdapter, string uri, long messageId = 0)
        {
            var errorInfo = etpAdapter.CreateErrorInfo().GrowingPortionIgnored(uri);
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for retention period exceeded.
        /// </summary>
        /// <param name="etpAdapter">The ETP Adapter.</param>
        /// <param name="messageId">The message identifier.</param>
        public static void RetentionPeriodExceededException(this IEtpAdapter etpAdapter, long messageId = 0)
        {
            var errorInfo = etpAdapter.CreateErrorInfo().RetentionPeriodExceeded();
            throw new EtpException(errorInfo, messageId);
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException"/> message for not growing object.
        /// </summary>
        /// <param name="etpAdapter">The ETP Adapter.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="messageId">The message identifier.</param>
        public static void NotGrowingObjectException(this IEtpAdapter etpAdapter, string uri, long messageId = 0)
        {
            var errorInfo = etpAdapter.CreateErrorInfo().NotGrowingObject(uri);
            throw new EtpException(errorInfo, messageId);
        }
    }
}

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
using Energistics.Etp.Common.Protocol.Core;
using System;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Defines static helper methods to create <see cref="IErrorInfo"/> instances.
    /// </summary>
    public static class EtpErrorInfo
    {
        /// <summary>
        /// Converts the <see cref="IErrorInfo"/> instance into a <see cref="EtpException"/>.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to convert.</param>
        /// <param name="protocol">The protocol number.</param>
        /// <param name="correlatedHeader">The message header that the exception to send is correlated with, if any.</param>
        /// <param name="innerException">The inner exception, if any.</param>
        /// <returns>The exception.</returns>
        public static EtpException ToException(this IErrorInfo errorInfo, int protocol, IMessageHeader correlatedHeader = null, Exception innerException = null)
        {
            return new EtpException(protocol, errorInfo, correlatedHeader, innerException);
        }

        /// <summary>
        /// Converts the <see cref="IErrorInfo"/> instance into a <see cref="EtpException"/>.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to convert.</param>
        /// <param name="protocol">The protocol number.</param>
        /// <param name="correlatedHeader">The message header that the exception to send is correlated with, if any.</param>
        /// <returns>The exception.</returns>
        public static EtpException ToException(this IErrorInfo errorInfo, int protocol, IMessageHeader correlatedHeader = null)
        {
            return new EtpException(protocol, errorInfo, correlatedHeader);
        }

        /// <summary>
        /// Sets the properties of the specified <see cref="IErrorInfo"/> instance.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo Set(this IErrorInfo errorInfo, int errorCode, string errorMessage)
        {
            errorInfo.Code = errorCode;
            errorInfo.Message = errorMessage;
            return errorInfo;
        }

        /// <summary>
        /// Sets the properties of the specified <see cref="IErrorInfo"/> instance.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo Set(this IErrorInfo errorInfo, EtpErrorCodes errorCode, string errorMessage)
        {
            return errorInfo.Set((int)errorCode, errorMessage);
        }
        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for a unset type.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="messageType">The message type.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo Unset(this IErrorInfo errorInfo, long messageType)
        {
            return errorInfo.Set(EtpErrorCodes.Unset,
                                 $"Unset: {messageType}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for no role.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="protocol">The protocol the role was requested on.</param>
        /// <param name="role">The requested role.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo NoRole(this IErrorInfo errorInfo, int protocol, string role)
        {
            return errorInfo.Set(EtpErrorCodes.NoRole,
                                 $"No Role: Protocol: {protocol}; Role: {role}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for no supported protocols.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo NoSupportedProtocols(this IErrorInfo errorInfo)
        {
            return errorInfo.Set(EtpErrorCodes.NoSupportedProtocols,
                                 $"No Supported Protocols");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for an invalid message type.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="protocol">The message's protocol.</param>
        /// <param name="messageType">The message type.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo InvalidMessageType(this IErrorInfo errorInfo, int protocol, long messageType)
        {
            return errorInfo.Set(EtpErrorCodes.InvalidMessageType,
                                 $"Invalid Message Type: Protocol: {protocol}; Message Type: {messageType}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for an unsupported protocol.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="protocol">The unsupported protocol.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo UnsupportedProtocol(this IErrorInfo errorInfo, int protocol)
        {
            return errorInfo.Set(EtpErrorCodes.UnsupportedProtocol,
                                 $"Unsupported Protocol: {protocol}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for an invalid argument.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="argument">The argument name.</param>
        /// <param name="value">The argument value.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo InvalidArgument(this IErrorInfo errorInfo, string argument, object value)
        {
            return errorInfo.Set(EtpErrorCodes.InvalidArgument,
                                 $"Invalid Argument: Argument {argument}; Value '{value}'");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for an invalid argument.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="argument">The argument name.</param>
        /// <param name="value">The argument value.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo InvalidArgument(this IErrorInfo errorInfo, string argument, IUuidSource value)
            => errorInfo.InvalidArgument(argument, value?.Uuid);

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for an invalid argument.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="argument">The argument name.</param>
        /// <param name="value">The argument value.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo InvalidArgument(this IErrorInfo errorInfo, string argument, IRequestUuidSource value) => errorInfo.InvalidArgument(argument, value?.RequestUuid);

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for permission denied.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo RequestDenied(this IErrorInfo errorInfo) => errorInfo.RequestDenied(string.Empty);

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for permission denied.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="message">The message to provide.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo RequestDenied(this IErrorInfo errorInfo, string message)
        {
            return errorInfo.Set(EtpErrorCodes.RequestDenied,
                                 $"{(errorInfo.EtpVersion == EtpVersion.v11 ? "Permission Denied" : "Request Denied")}{(string.IsNullOrEmpty(message) ? string.Empty : $": {message}")}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for not supported.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo NotSupported(this IErrorInfo errorInfo)
        {
            return errorInfo.Set(EtpErrorCodes.NotSupported,
                                 $"Not Supported");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for an invalid state.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo InvalidState(this IErrorInfo errorInfo) => errorInfo.InvalidState(string.Empty);

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for an invalid state.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo InvalidState(this IErrorInfo errorInfo, string message)
        {
            return errorInfo.Set(EtpErrorCodes.InvalidState,
                                 $"Invalid State{(string.IsNullOrEmpty(message) ? string.Empty : $": {message}")}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for an invalid URI.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="uri">The invalid uri.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo InvalidUri(this IErrorInfo errorInfo, string uri)
        {
            return errorInfo.Set(EtpErrorCodes.InvalidUri,
                                 $"Invalid URI: {uri}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for an expired token.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo ExpiredToken(this IErrorInfo errorInfo)
        {
            return errorInfo.Set(EtpErrorCodes.ExpiredToken,
                                 $"Expired Token");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for object not found.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="value">The argument value.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo NotFound(this IErrorInfo errorInfo, object value)
        {
            return errorInfo.Set(EtpErrorCodes.NotFound,
                                 $"Not Found: {value}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for object not found.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="value">The argument value.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo NotFound(this IErrorInfo errorInfo, IUuidSource value) => errorInfo.NotFound(value?.Uuid);

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for object not found.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="value">The argument value.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo NotFound(this IErrorInfo errorInfo, IRequestUuidSource value) => errorInfo.NotFound(value?.RequestUuid);

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for object not found.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="argument">The argument name.</param>
        /// <param name="value">The argument value.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo NotFound(this IErrorInfo errorInfo, string argument, object value)
        {
            return errorInfo.Set(EtpErrorCodes.InvalidArgument,
                                 $"Not Found: Argument {argument}; Value '{value}'");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for object not found.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="argument">The argument name.</param>
        /// <param name="value">The argument value.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo NotFound(this IErrorInfo errorInfo, string argument, IUuidSource value) => errorInfo.NotFound(argument, value?.Uuid);

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for object not found.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="argument">The argument name.</param>
        /// <param name="value">The argument value.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo NotFound(this IErrorInfo errorInfo, string argument, IRequestUuidSource value)
        {
            return errorInfo.NotFound(argument, value?.RequestUuid);
        }
        
        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for limit exceeded.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="value">The argument value.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo LimitExceeded(this IErrorInfo errorInfo, object value)
        {
            return errorInfo.Set(EtpErrorCodes.LimitExceeded,
                                 $"Limit Exceeded: {value}");
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException" /> message for compression not supported.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <returns>The <see cref="IProtocolException" /> message identifier.</returns>
        public static IErrorInfo CompressionNotSupported(this IErrorInfo errorInfo)
        {
            return errorInfo.Set(EtpErrorCodes.CompressionNotSupported,
                                 $"Compression Not Supported");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for an invalid object.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="message">The optional error message.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo InvalidObject(this IErrorInfo errorInfo, string uri, string message = null)
        {
            return errorInfo.Set(EtpErrorCodes.InvalidObject,
                                 $"Invalid Object. URI: {uri}{(message == null ? string.Empty : $"; {message}")}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for max transactions exceeded.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo MaxTransactionsExceeded(this IErrorInfo errorInfo)
        {
            return errorInfo.Set(EtpErrorCodes.MaxTransactionsExceeded,
                                 $"Max Transactions Exceeded");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for content type not supported.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="contentType">The unsupported content type.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo ContentTypeNotSupported(this IErrorInfo errorInfo, string contentType)
        {
            return errorInfo.Set(EtpErrorCodes.ContentTypeNotSupported,
                                 $"Content Type Not Supported: {contentType}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for content type not supported.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo MaxSizeExceeded(this IErrorInfo errorInfo)
        {
            return errorInfo.Set(EtpErrorCodes.MaxSizeExceeded,
                                 $"Max Size Exceeded");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for multi-part cancelled.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo MultiPartCancelled(this IErrorInfo errorInfo)
        {
            return errorInfo.Set(EtpErrorCodes.MultiPartCancelled,
                                 $"Multi-Part Cancelled");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for invalid message.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="protocol">The message's protocol.</param>
        /// <param name="messageType">The message type.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo InvalidMessage(this IErrorInfo errorInfo, int protocol, long messageType)
        {
            return errorInfo.Set(EtpErrorCodes.InvalidMessage,
                                 $"Invalid Message: Protocol: {protocol}; Message Type: {messageType}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for invalid index kind.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="indexKind">The invalid index kind.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo InvalidIndexKind(this IErrorInfo errorInfo, string indexKind)
        {
            return errorInfo.Set(EtpErrorCodes.InvalidIndexKind,
                                 $"Invalid Index Kind: {indexKind}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for no supported formats.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="indexKind">The invalid index kind.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo NoSupportedFormats(this IErrorInfo errorInfo)
        {
            return errorInfo.Set(EtpErrorCodes.NoSupportedFormats,
                                 $"No Supported Formats");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for request UUID rejected.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="requestUuid">The rejected request UUID.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo RequestUuidRejected(this IErrorInfo errorInfo, Guid requestUuid)
        {
            return errorInfo.Set(EtpErrorCodes.RequestUuidRejected,
                                 $"Request UUID rejected: {requestUuid}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for request UUID rejected.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="requestUuid">The rejected request UUID.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo RequestUuidRejected(this IErrorInfo errorInfo, IUuidSource requestUuid)
        {
            return errorInfo.Set(EtpErrorCodes.RequestUuidRejected,
                                 $"Request UUID rejected: {requestUuid?.Uuid}");
        }


        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for request UUID rejected.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="requestUuid">The rejected request UUID.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo RequestUuidRejected(this IErrorInfo errorInfo, IRequestUuidSource requestUuid)
        {
            return errorInfo.RequestUuidRejected(requestUuid?.RequestUuid ?? default(Guid));
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for update growing object denied.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="uri">The object's URI.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo UpdateGrowingObjectDenied(this IErrorInfo errorInfo, string uri)
        {
            return errorInfo.Set(EtpErrorCodes.UpdateGrowingObjectDenied,
                                 $"Update Growing Object Denied: {uri}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for backpressure limit exceeded.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo BackpressureLimitExceeded(this IErrorInfo errorInfo)
        {
            return errorInfo.Set(EtpErrorCodes.BackpressureLimitExceeded,
                                 $"Backpressure Limit Exceeded");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for backpressure warning.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo BackpressureWarning(this IErrorInfo errorInfo)
        {
            return errorInfo.Set(EtpErrorCodes.BackpressureWarning,
                                 $"Backpressure Warning");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for an invalid ChannelId.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="value">The argument value.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo InvalidChannelId(this IErrorInfo errorInfo, object value)
        {
            return errorInfo.Set(EtpErrorCodes.InvalidChannelId,
                                 $"Invalid Channel ID: {value}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for an unsupported object.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="uri">The URI.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo UnsupportedObject(this IErrorInfo errorInfo, string uri)
        {
            return errorInfo.Set(EtpErrorCodes.UnsupportedObject,
                                 $"Unsupported Object: {uri}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for no cascade delete.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="uri">The URI.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo NoCascadeDelete(this IErrorInfo errorInfo, string uri)
        {
            return errorInfo.Set(EtpErrorCodes.NoCascadeDelete,
                                 $"If cascading deletes are not invoked, a client must only request deletion of bottom level data-objects such that all child data - objects are deleted before the parent is deleted. URI: {uri}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for plural object.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="uri">The URI.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo PluralObject(this IErrorInfo errorInfo, string uri)
        {
            return errorInfo.Set(EtpErrorCodes.PluralObject,
                                 $"Client attempted to put more than one object under the plural root of a 1.x Energistics data object. ETP only supports a single data object, one XML document. URI: {uri}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for ignoring the growing portion.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo GrowingPortionIgnored(this IErrorInfo errorInfo, string uri)
        {
            return errorInfo.Set(EtpErrorCodes.GrowingPortionIgnored,
                                 $"The object is upserted, but the growing portion is ignored. URI: {uri}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for retention period exceeded.
        /// </summary>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo RetentionPeriodExceeded(this IErrorInfo errorInfo)
        {
            return errorInfo.Set(EtpErrorCodes.RetentionPeriodExceeded,
                                 $"Retention Period Exceeded");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for not growing object.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static IErrorInfo NotGrowingObject(this IErrorInfo errorInfo, string uri)
        {
            return errorInfo.Set(EtpErrorCodes.NotGrowingObjct,
                                 $"Not Growing Object. URI: {uri}");
        }
    }
}

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

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Defines static helper methods to create <see cref="IErrorInfo"/> instances.
    /// </summary>
    public static class EtpErrorInfos
    {
        /// <summary>
        /// Sets the properties of the specified <see cref="IErrorInfo"/> instance.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static TErrorInfo Set<TErrorInfo>(this TErrorInfo errorInfo, int errorCode, string errorMessage)
            where TErrorInfo : IErrorInfo
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
        public static TErrorInfo Set<TErrorInfo>(this TErrorInfo errorInfo, EtpErrorCodes errorCode, string errorMessage)
            where TErrorInfo : IErrorInfo
        {
            return errorInfo.Set((int)errorCode, errorMessage);
        }
        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for a unset type.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="messageType">The message type.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static TErrorInfo Unset<TErrorInfo>(this TErrorInfo errorInfo, long messageType)
            where TErrorInfo : IErrorInfo
        {
            return errorInfo.Set(EtpErrorCodes.Unset,
                                 $"Unset: {messageType}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for a no role type.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="messageType">The message type.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static TErrorInfo NoRole<TErrorInfo>(this TErrorInfo errorInfo, long messageType)
            where TErrorInfo : IErrorInfo
        {
            return errorInfo.Set(EtpErrorCodes.NoRole,
                                 $"No Role: {messageType}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for a no supported protocols type.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="messageType">The message type.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static TErrorInfo NoSupportedProtocols<TErrorInfo>(this TErrorInfo errorInfo, long messageType)
            where TErrorInfo : IErrorInfo
        {
            return errorInfo.Set(EtpErrorCodes.NoSupportedProtocols,
                                 $"No supported protocols: {messageType}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for an invalid message type.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to update.</param>
        /// <param name="messageType">The message type.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static TErrorInfo InvalidMessage<TErrorInfo>(this TErrorInfo errorInfo, long messageType)
            where TErrorInfo : IErrorInfo
        {
            return errorInfo.Set(EtpErrorCodes.InvalidMessageType,
                                 $"Invalid message type: {messageType}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for an unsupported protocol.
        /// </summary>
        /// <param name="value">The argument value.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static TErrorInfo UnsupportedProtocol<TErrorInfo>(this TErrorInfo errorInfo, object value)
            where TErrorInfo : IErrorInfo
        {
            return errorInfo.Set(EtpErrorCodes.UnsupportedProtocol,
                                 $"Unsupported Protocol: {value}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for an invalid argument.
        /// </summary>
        /// <param name="value">The argument value.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static TErrorInfo InvalidArgument<TErrorInfo>(this TErrorInfo errorInfo, object value)
            where TErrorInfo : IErrorInfo
        {
            return errorInfo.Set(EtpErrorCodes.InvalidArgument,
                                 $"Invalid Argument: {value}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for permission denied.
        /// </summary>
        /// <param name="value">The argument value.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static TErrorInfo PermissionDenied<TErrorInfo>(this TErrorInfo errorInfo, object value)
            where TErrorInfo : IErrorInfo
        {
            return errorInfo.Set(EtpErrorCodes.PermissionDenied,
                                 $"Permission Denied: {value}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for not supported.
        /// </summary>
        /// <param name="value">The argument value.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static TErrorInfo NotSupported<TErrorInfo>(this TErrorInfo errorInfo, object value)
            where TErrorInfo : IErrorInfo
        {
            return errorInfo.Set(EtpErrorCodes.NotSupported,
                                 $"Not Supported: {value}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for an invalid state.
        /// </summary>
        /// <param name="value">The argument value.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static TErrorInfo InvalidState<TErrorInfo>(this TErrorInfo errorInfo, object value)
            where TErrorInfo : IErrorInfo
        {
            return errorInfo.Set(EtpErrorCodes.InvalidState,
                                 $"Invalid State: {value}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for an invalid URI.
        /// </summary>
        /// <param name="value">The argument value.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static TErrorInfo InvalidUri<TErrorInfo>(this TErrorInfo errorInfo, object value)
            where TErrorInfo : IErrorInfo
        {
            return errorInfo.Set(EtpErrorCodes.InvalidUri,
                                 $"Invalid Uri: {value}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for an expired token.
        /// </summary>
        /// <param name="value">The argument value.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static TErrorInfo ExpiredToken<TErrorInfo>(this TErrorInfo errorInfo, object value)
            where TErrorInfo : IErrorInfo
        {
            return errorInfo.Set(EtpErrorCodes.ExpiredToken,
                                 $"Expired Token: {value}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for object not found.
        /// </summary>
        /// <param name="value">The argument value.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static TErrorInfo NotFound<TErrorInfo>(this TErrorInfo errorInfo, object value)
            where TErrorInfo : IErrorInfo
        {
            return errorInfo.Set(EtpErrorCodes.NotFound,
                                 $"Not Found: {value}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for limit exceeded.
        /// </summary>
        /// <param name="value">The argument value.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static TErrorInfo LimitExceeded<TErrorInfo>(this TErrorInfo errorInfo, object value)
            where TErrorInfo : IErrorInfo
        {
            return errorInfo.Set(EtpErrorCodes.LimitExceeded,
                                 $"Limit Exceeded: {value}");
        }

        /// <summary>
        /// Sends a <see cref="IProtocolException" /> message for compression not supported.
        /// </summary>
        /// <param name="requestedCompression">The requested compression.</param>
        /// <returns>The <see cref="IProtocolException" /> message identifier.</returns>
        public static TErrorInfo CompressionNotSupported<TErrorInfo>(this TErrorInfo errorInfo, string requestedCompression)
            where TErrorInfo : IErrorInfo
        {
            return errorInfo.Set(EtpErrorCodes.CompressionNotSupported,
                                 $"Compression Not Supported: {requestedCompression}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for an invalid object.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static TErrorInfo InvalidObject<TErrorInfo>(this TErrorInfo errorInfo, string uri)
            where TErrorInfo : IErrorInfo
        {
            return errorInfo.Set(EtpErrorCodes.InvalidObject,
                                 $"Invalid Object. URI: {uri}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for an invalid ChannelId.
        /// </summary>
        /// <param name="value">The argument value.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static TErrorInfo InvalidChannelId<TErrorInfo>(this TErrorInfo errorInfo, object value)
            where TErrorInfo : IErrorInfo
        {
            return errorInfo.Set(EtpErrorCodes.InvalidChannelId,
                                 $"Invalid ChannelId: {value}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for an unsupported object.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static TErrorInfo UnsupportedObject<TErrorInfo>(this TErrorInfo errorInfo, string uri)
            where TErrorInfo : IErrorInfo
        {
            return errorInfo.Set(EtpErrorCodes.UnsupportedObject,
                                 $"Data object not supported. URI: {uri}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for an invalid object X.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static TErrorInfo InvalidObjectX<TErrorInfo>(this TErrorInfo errorInfo, string uri)
            where TErrorInfo : IErrorInfo
        {
            return errorInfo.Set(EtpErrorCodes.InvalidObjectX,
                                 $"Invalid Object. URI: {uri}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for no cascade delete.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static TErrorInfo NoCascadeDelete<TErrorInfo>(this TErrorInfo errorInfo, string uri)
            where TErrorInfo : IErrorInfo
        {
            return errorInfo.Set(EtpErrorCodes.NoCascadeDelete,
                                 $"If cascading deletes are not invoked, a client must only request deletion of bottom level data-objects such that all child data - objects are deleted before the parent is deleted. URI: {uri}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for no plural object.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static TErrorInfo NoPluralObject<TErrorInfo>(this TErrorInfo errorInfo, string uri)
            where TErrorInfo : IErrorInfo
        {
            return errorInfo.Set(EtpErrorCodes.NoPluralObject,
                                 $"Client attempted to put more than one object under the plural root of a 1.x Energistics data object. ETP only supports a single data object, one XML document. URI: {uri}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for ignoring the growing portion.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static TErrorInfo GrowingPortionIgnored<TErrorInfo>(this TErrorInfo errorInfo, string uri)
            where TErrorInfo : IErrorInfo
        {
            return errorInfo.Set(EtpErrorCodes.GrowingPortionIgnored,
                                 $"The object is upserted, but the growing portion is ignored. URI: {uri}");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for retention period exceeded.
        /// </summary>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static TErrorInfo RetentionPeriodExceeded<TErrorInfo>(this TErrorInfo errorInfo)
            where TErrorInfo : IErrorInfo
        {
            return errorInfo.Set(EtpErrorCodes.RetentionPeriodExceeded,
                                 $"Retention Period Exceeded");
        }

        /// <summary>
        /// Initializes a <see cref="IErrorInfo"/> instance for not growing object.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The initialized <see cref="IErrorInfo"/> instance.</returns>
        public static TErrorInfo NotGrowingObject<TErrorInfo>(this TErrorInfo errorInfo, string uri)
            where TErrorInfo : IErrorInfo
        {
            return errorInfo.Set(EtpErrorCodes.NotGrowingObjct,
                                 $"Not Growing Object. URI: {uri}");
        }
    }
}

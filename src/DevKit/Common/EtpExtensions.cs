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
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using Energistics.Avro;
using Energistics.Avro.Encoding;
using Energistics.Avro.Encoding.Binary;
using Energistics.Avro.Encoding.Json;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Datatypes.Object;
using Energistics.Etp.Common.Protocol.Core;
using log4net;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Provides extension methods that can be used along with ETP message types.
    /// </summary>
    public static class EtpExtensions
    {
        private static readonly char[] WhiteSpace = Enumerable.Range(0, 20).Select(Convert.ToChar).ToArray();
        private const long TicksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1000L;
        private const long UnixEpochTicks = 621355968000000000L; // new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks

        /// <summary>
        /// Converts a protocol and message type to a unique message key combination
        /// </summary>
        /// <param name="protocol">The message protocol.</param>
        /// <param name="messageType">The message type.</param>
        /// <returns>The message key.</returns>
        public static long CreateMessageKey(int protocol, int messageType)
        {
            // Special handling for Acknowledge and ProtocolException
            if (messageType == (int)MessageTypes.Core.Acknowledge || messageType == (int)MessageTypes.Core.ProtocolException)
                protocol = (int)Protocols.Core;

            return (((long)protocol) << 32) + messageType;
        }

        /// <summary>
        /// Converts an encoding to the string value used in headers.
        /// </summary>
        /// <param name="encoding">The encoding to get the string value for.</param>
        /// <returns>The string value.</returns>
        public static string ToHeaderValue(this EtpEncoding encoding)
        {
            FieldInfo fi = encoding.GetType().GetField(encoding.ToString());
            return fi.GetCustomAttributes(typeof(DescriptionAttribute), false)?.Cast<DescriptionAttribute>().FirstOrDefault()?.Description;
        }

        /// <summary>
        /// Sets the encoding header.
        /// </summary>
        /// <param name="headers">The header dictionary to set the encoding in.</param>
        /// <param name="encoding">The encoding to set.</param>
        public static void SetEncoding(this IDictionary<string, string> headers, EtpEncoding encoding)
        {
            if (headers == null)
                return;

            var value = encoding.ToHeaderValue();
            if (value == null)
                headers.Remove(EtpHeaders.Encoding);
            else
                headers[EtpHeaders.Encoding] = value;
        }

        /// <summary>
        /// Sets the encoding header.
        /// </summary>
        /// <param name="headers">The header dictionary to set the encoding in.</param>
        /// <param name="authorization">The authorization details.</param>
        public static void SetAuthorization(this IDictionary<string, string> headers, Security.Authorization authorization)
        {
            if (headers == null)
                return;

            if (authorization?.HasValue ?? false)
                headers[EtpHeaders.Authorization] = authorization.Value;
            else
                headers.Remove(EtpHeaders.Authorization);
        }

        /// <summary>
        /// Sets the encoding header.
        /// </summary>
        /// <param name="headers">The header dictionary to set the encoding in.</param>
        /// <param name="authorization">The authorization details.</param>
        public static void SetAuthorization(this NameValueCollection headers, Security.Authorization authorization)
        {
            if (headers == null)
                return;

            if (authorization?.HasValue ?? false)
                headers[EtpHeaders.Authorization] = authorization.Value;
            else
                headers.Remove(EtpHeaders.Authorization);
        }

        /// <summary>
        /// Converts an HTTP / HTTPS URI to a WebSocket URI.
        /// </summary>
        /// <param name="uri">The URI to convert.</param>
        /// <returns>A WebSocket URI.</returns>
        public static Uri ToWebSocketUri(this Uri uri)
        {
            if (uri.Scheme == "ws" || uri.Scheme == "wss")
                return uri;
            if (uri.Scheme != "http" && uri.Scheme != "https")
                throw new ArgumentException("Not an HTTP / HTTPS or WS / WSS URI", "uri");

            var scheme = uri.Scheme == "https" ? "wss" : "ws";
            return new Uri(scheme + uri.ToString().Substring(uri.Scheme.Length));
        }

        /// <summary>
        /// Serializes the specified message.
        /// </summary>
        /// <typeparam name="T">The type of the message body.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="includeExtension">Whether or not to include a message header extension.</param>
        /// <returns>The encoded byte array containing the message data.</returns>
        public static byte[] Serialize<T>(this EtpMessage<T> message, bool includeExtension = false) where T : IEtpMessageBody
        {
            var @objects = message.Extension != null && includeExtension
                ? new IEtpRecord[] { message.Header, message.Extension, message.Body }
                : new IEtpRecord[] { message.Header, message.Body };

            var content = Serialize(@objects);
            return System.Text.Encoding.UTF8.GetBytes(content);
        }

        /// <summary>
        /// Encodes the specified message.
        /// </summary>
        /// <typeparam name="T">The type of the message body.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="asBinary">Whether or not to encode as binary.</param>
        /// <param name="includeExtension">Whether or not to include a message header extension.</param>
        /// <param name="compression">The compression type.</param>
        /// <returns>The encoded byte array containing the message data.</returns>
        public static byte[] Encode<T>(this EtpMessage<T> message, bool asBinary = true, bool includeExtension = false, string compression = EtpCompression.None) where T : IEtpMessageBody
        {
            using (var stream = new MemoryStream())
            using (var compressionStream = message.Header.CanBeCompressed() ? EtpCompression.TryGetCompresionStream(compression, stream) : null)
            {
                // create avro binary encoder to write to memory stream
                var headerEncoder = asBinary ? (IAvroEncoder)new BinaryAvroEncoder(stream) : new JsonAvroEncoder(stream);
                var bodyEncoder = headerEncoder;

                if (compressionStream != null)
                {
                    // add Compressed flag to message flags before writing header
                    message.Header.SetCompressed();
                    bodyEncoder = asBinary ? (IAvroEncoder)new BinaryAvroEncoder(compressionStream) : new JsonAvroEncoder(compressionStream);
                }

                // serialize header
                message.Header.Encode(headerEncoder);

                // serialize header extension
                if (message.Extension != null && includeExtension)
                    message.Extension.Encode(bodyEncoder); // Use body encoder to handle compression

                message.Body.Encode(bodyEncoder);

                if (compressionStream != null)
                    compressionStream.Close();

                return stream.ToArray();
            }
        }

        /// <summary>
        /// Encodes the record to a byte array.
        /// </summary>
        /// <typeparam name="T">The record type.</typeparam>
        /// <param name="record">The record to encode.</param>
        /// <returns>The encoded byte array.</returns>
        public static byte[] EncodeToBytes<T>(this T record) where T : class, IEtpRecord, new()
        {
            using (var stream = new MemoryStream())
            {
                var encoder = new BinaryAvroEncoder(stream);

                if (record != null)
                    encoder.EncodeAvroObject<T>(record);

                return stream.ToArray();
            }
        }

        /// <summary>
        /// Decodes a record from a byte array.
        /// </summary>
        /// <typeparam name="T">The record type.</typeparam>
        /// <param name="bytes">The byte array to decode the record from.</param>
        /// <returns>The decoded record.</returns>
        public static T DecodeFromBytes<T>(byte[] bytes) where T : class, IEtpRecord, new()
        {
            if (bytes == null)
                return default(T);

            using (var stream = new MemoryStream(bytes))
            using (var decoder = new BinaryAvroDecoder(stream))
            {
                return decoder.DecodeAvroObject<T>();
            }
        }

        /// <summary>
        /// Serializes the specified object instance.
        /// </summary>
        /// <param name="instances">The objects to serialize.</param>
        /// <returns>The serialized JSON string.</returns>
        public static string Serialize(params IAvroRecord[] instances)
        {
            using (var writer = new StringWriter())
            {
                using (var encoder = new JsonAvroEncoder(writer))
                {
                    if (instances.Length == 1)
                        instances[0].Encode(encoder);
                    else
                    {
                        encoder.EncodeArrayStart(instances.Length, instances.Length);
                        var separator = false;
                        foreach (var instance in instances)
                        {
                            if (separator)
                                encoder.EncodeArrayItemSeparator();
                            instance.Encode(encoder);
                            separator = true;
                        }
                        encoder.EncodeArrayEnd();
                    }
                }
                return writer.ToString();
            }
        }

        /// <summary>
        /// Clears the specified <see cref="MemoryStream"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public static void Clear(this MemoryStream stream)
        {
            var buffer = stream.GetBuffer();
            Array.Clear(buffer, 0, buffer.Length);
            stream.Position = 0;
            stream.SetLength(0);
        }

        /// <summary>
        /// Converts an ETP timestamp to a UTC <see cref="DateTime"/>.
        /// </summary>
        /// <param name="timestamp">The ETP timestamp</param>
        /// <returns>The <see cref="DateTime"/>.</returns>
        public static DateTime ToUtcDateTime(this long timestamp)
        {
            var ticks = timestamp * TicksPerMicrosecond;
            return new DateTime(ticks + UnixEpochTicks, DateTimeKind.Utc);
        }

        /// <summary>
        /// Converts a <see cref="DateTime"/> to a UTC ETP timestamp.
        /// </summary>
        /// <param name="timestamp">The <see cref="DateTime"/> timestamp</param>
        /// <returns>The UTC ETP timestamp.</returns>
        public static long ToEtpTimestamp(this DateTime timestamp)
        {
            var ticks = timestamp.ToUniversalTime().Ticks - UnixEpochTicks;
            return ticks / TicksPerMicrosecond;
        }

        /// <summary>
        /// Converts an ETP timestamp to a UTC <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="timestamp">The ETP timestamp</param>
        /// <returns>The <see cref="DateTimeOffset"/>.</returns>
        public static DateTimeOffset ToUtcDateTimeOffset(this long timestamp)
        {
            var ticks = timestamp * TicksPerMicrosecond;
            return new DateTimeOffset(ticks + UnixEpochTicks, TimeSpan.Zero);
        }

        /// <summary>
        /// Converts a <see cref="DateTimeOffset"/> to a UTC ETP timestamp.
        /// </summary>
        /// <param name="timestamp">The <see cref="DateTimeOffset"/> timestamp</param>
        /// <returns>The UTC ETP timestamp.</returns>
        public static long ToEtpTimestamp(this DateTimeOffset timestamp)
        {
            var ticks = timestamp.ToUniversalTime().Ticks - UnixEpochTicks;
            return ticks / TicksPerMicrosecond;
        }

        /// <summary>
        /// Converts a list of items to a map where the keys are the list indexes.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="list">The list of items.</param>
        /// <returns>The list converted to a map.</returns>
        public static Dictionary<string, T> ToMap<T>(this IList<T> list)
        {
            var dictionary = new Dictionary<string, T>();
            if (list == null)
                return dictionary;

            for (int i = 0; i < list.Count; i++)
                dictionary[$"{i}"] = list[i];

            return dictionary;
        }

        /// <summary>
        /// Decodes the data contained by the <see cref="IDataObject"/> as a string.
        /// </summary>
        /// <param name="dataObject">The data object.</param>
        /// <returns>The decoded string.</returns>
        public static string GetString(this IDataObject dataObject)
        {
            //var data = System.Text.Encoding.Unicode.GetString(dataObject.GetData());
            var data = System.Text.Encoding.UTF8.GetString(dataObject.GetData());
            return data.Trim(WhiteSpace);
        }

        /// <summary>
        /// Encodes and optionally compresses the string for the <see cref="IDataObject"/> data.
        /// </summary>
        /// <param name="dataObject">The data object.</param>
        /// <param name="data">The data string.</param>
        /// <param name="compressionMethod">The compression method to use to compress the data, if any.</param>
        /// <returns>The data object with the data set.</returns>
        public static IDataObject SetString(this IDataObject dataObject, string data, string compressionMethod = EtpCompression.None)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                dataObject.SetData(new byte[0], compressionMethod);
                return dataObject;
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(data);

            //var bytes = System.Text.Encoding.Convert(
            //    System.Text.Encoding.UTF8,
            //    System.Text.Encoding.Unicode,
            //    System.Text.Encoding.UTF8.GetBytes(data));

            dataObject.SetData(bytes, compressionMethod);

            return dataObject;
        }

        /// <summary>
        /// Gets the data contained by the <see cref="IDataObject"/> and decompresses the byte array, if necessary.
        /// </summary>
        /// <param name="dataObject">The data object.</param>
        /// <returns>The decompressed data as a byte array.</returns>
        public static byte[] GetData(this IDataObject dataObject)
        {
            if (!EtpCompression.RequiresDecompression(dataObject.ContentEncoding) || dataObject.Data?.Length == 0)
                return dataObject.Data;

            if (!EtpCompression.CanDecompress(dataObject.ContentEncoding))
                throw new NotSupportedException($"Content encoding not supported: {dataObject.ContentEncoding}");

            using (var uncompressed = new MemoryStream())
            {
                using (var compressed = new MemoryStream(dataObject.Data))
                using (var decompressionStream = EtpCompression.TryGetDecompresionStream(dataObject.ContentEncoding, compressed))
                {
                    decompressionStream.CopyTo(uncompressed);
                }

                return uncompressed.ToArray();
            }
        }

        /// <summary>
        /// Sets and optionally compresses the data for the <see cref="IDataObject"/>.
        /// </summary>
        /// <param name="dataObject">The data object.</param>
        /// <param name="data">The data.</param>
        /// <param name="compressionMethod">The compression method to use if any.</param>
        /// <returns>The data object with the data set.</returns>
        public static IDataObject SetData(this IDataObject dataObject, byte[] data, string compressionMethod = EtpCompression.None)
        {
            var encoding = string.Empty;

            if (EtpCompression.RequiresCompression(compressionMethod) && data?.Length > 0)
            {
                if (!EtpCompression.CanCompress(compressionMethod))
                    throw new NotSupportedException($"Compression method not supported: {compressionMethod}");

                using (var compressed = new MemoryStream())
                {
                    using (var uncompressed = new MemoryStream(data))
                    using (var compressionStream = EtpCompression.TryGetCompresionStream(compressionMethod, compressed))
                    {
                        uncompressed.CopyTo(compressionStream);
                    }

                    data = compressed.ToArray();
                    encoding = compressionMethod;
                }
            }

            dataObject.ContentEncoding = encoding;
            dataObject.Data = data ?? new byte[0];

            return dataObject;
        }

        /// <summary>
        /// Converts this <see cref="IErrorInfo"/> to an error message.
        /// </summary>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to convert to an error message.</param>
        /// <param name="ex">The exception to include, if any.</param>
        public static string ToErrorMessage(this IErrorInfo errorInfo)
        {
            object error = $"Error {errorInfo.Code}";
            if (Enum.IsDefined(typeof(EtpErrorCodes), errorInfo.Code))
            {
                var value = (EtpErrorCodes)errorInfo.Code;
                FieldInfo fi = typeof(EtpErrorCodes).GetField(value.ToString());

                DescriptionAttribute description = (DescriptionAttribute)fi.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();
                error = description?.Description ?? error;
            }

            return $"{error}: {errorInfo.Message}";
        }

        /// <summary>
        /// Logs information from the specified <see cref="IErrorInfo"/> instance.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="errorInfo">The <see cref="IErrorInfo"/> instance to log to.</param>
        /// <param name="ex">The exception to log, if any.</param>
        public static void LogErrorInfo(this ILog logger, IErrorInfo errorInfo, Exception ex = null)
        {
            if (ex == null)
                logger.Debug(errorInfo.ToErrorMessage());
            else
                logger.Debug(errorInfo.ToErrorMessage(), ex);
        }

        /// <summary>
        /// Converts the specified <see cref="EtpEndpointParameters"/> instance to a <see cref="EtpEndpointDetails"/> instance.
        /// </summary>
        /// <param name="parameters">The parameters to convert.</param>
        /// <param name="version">The ETP version supported.</param>
        /// <returns>The converted parameters.</returns>
        public static EtpEndpointDetails ToEndpointDetails(this EtpEndpointParameters parameters, EtpVersion version)
        {
            var capabilities = new EtpEndpointCapabilities(version);
            capabilities.LoadFrom(parameters.Capabilities);

            var details = new EtpEndpointDetails(
                capabilities,
                new List<IEndpointProtocol>(parameters.SupportedProtocols),
                new List<IEndpointSupportedDataObject>(parameters.SupportedDataObjects),
                new List<string>(parameters.SupportedCompression),
                new List<string>(parameters.SupportedFormats)
            );

            return details;
        }

        /// <summary>
        /// Converts the RequestSession message into an <see cref="EtpEndpointInfo"/> instance.
        /// </summary>
        /// <param name="message">The message to convert.</param>
        /// <returns>The created instance.</returns>
        public static EtpEndpointInfo ToEndpointInfo(this EtpMessage<IRequestSession> message)
        {
            return EtpEndpointInfo.FromId(message.Body.ApplicationName, message.Body.ApplicationVersion, message.Body.ClientInstanceId);
        }

        /// <summary>
        /// Converts the RequestSession message into an <see cref="EtpEndpointDetails"/> instance.
        /// </summary>
        /// <param name="body">The message to convert.</param>
        /// <returns>The created instance.</returns>
        public static EtpEndpointDetails ToEndpointDetails(this EtpMessage<IRequestSession> message)
        {
            var capabilities = new EtpEndpointCapabilities(message.EtpVersion);
            capabilities.LoadFrom(message.Body.EndpointCapabilities);
            return new EtpEndpointDetails(
                capabilities,
                message.Body.RequestedProtocols.Select(p => new EtpEndpointProtocol(p, false)).ToList(),
                message.Body.SupportedDataObjects.Select(o => new EtpSupportedDataObject(o)).ToList(),
                message.Body.SupportedCompression.ToList() ?? new List<string>(),
                message.Body.SupportedFormats?.ToList() ?? new List<string> { Formats.Xml }
            );
        }

        /// <summary>
        /// Converts the OpenSession message into an <see cref="EtpEndpointInfo"/> instance.
        /// </summary>
        /// <param name="message">The message to convert.</param>
        /// <returns>The created instance.</returns>
        public static EtpEndpointInfo ToEndpointInfo(this EtpMessage<IOpenSession> message)
        {
            return EtpEndpointInfo.FromId(message.Body.ApplicationName, message.Body.ApplicationVersion, message.Body.ServerInstanceId);
        }

        /// <summary>
        /// Converts the OpenSession message into an <see cref="EtpEndpointDetails"/> instance.
        /// </summary>
        /// <param name="body">The message to convert.</param>
        /// <returns>The created instance.</returns>
        public static EtpEndpointDetails ToEndpointDetails(this EtpMessage<IOpenSession> message)
        {
            var capabilities = new EtpEndpointCapabilities(message.EtpVersion);
            capabilities.LoadFrom(message.Body.EndpointCapabilities);
            return new EtpEndpointDetails(
                capabilities,
                message.Body.SupportedProtocols.Select(p => new EtpEndpointProtocol(p, true)).ToList(),
                message.Body.SupportedDataObjects.Select(o => new EtpSupportedDataObject(o)).ToList(),
                message.Body.SupportedCompression == null ? new List<string>() : new List<string> { message.Body.SupportedCompression },
                message.Body.SupportedFormats?.ToList() ?? new List<string> { Formats.Xml }
            );
        }
        /// <summary>
        /// Gets a version string from the specified ETP version.
        /// </summary>
        /// <param name="version">The version to convert.</param>
        /// <returns>The version string.</returns>
        public static string ToVersionString(this EtpVersion version)
        {
            var v = version.ToSystemVersion();
            return $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
        }

        /// <summary>
        /// Gets a consistent, version-specific key for this data object type.
        /// </summary>
        /// <param name="dataObjectType">The data object type to get the key for.</param>
        /// <param name="version">The ETP version to get the key for.</param>
        /// <returns>The version-specific key.</returns>
        public static string ToVersionKey(this IDataObjectType dataObjectType, EtpVersion version)
        {
            switch (version)
            {
                case EtpVersion.v11: return dataObjectType.ContentType.ToString();
                case EtpVersion.v12: return dataObjectType.DataObjectType.ToString();
                default: return null;
            }
        }

        /// <summary>
        /// Sends a ProtocolException message with the specified exception details.
        /// </summary>
        /// <param name="exception">The ETP exception.</param>
        /// <param name="handler">The handler to send the exception on.</param>
        /// <param name="isFinalPart">Whether or not the protocol exception is the final part in a multi-part message.</param>
        /// <param name="extension">The message header extension to send with the message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public static EtpMessage<IProtocolException> Send(this EtpException exception, IProtocolHandler handler, bool isMultiPart = false, bool isFinalPart = false, IMessageHeaderExtension extension = null)
        {
            return handler.ProtocolException(exception, isFinalPart: isFinalPart, extension: extension);
        }

        /// <summary>
        /// Sends a ProtocolException message with the specified exception details.
        /// </summary>
        /// <param name="exception">The ETP exception.</param>
        /// <param name="session">The session to send the exception on.</param>
        /// <param name="isFinalPart">Whether or not the protocol exception is the final part in a multi-part message.</param>
        /// <param name="extension">The message header extension to send with the message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public static EtpMessage<IProtocolException> Send(this EtpException exception, IEtpSession session, bool isMultiPart = false, bool isFinalPart = false, IMessageHeaderExtension extension = null)
        {
            return session.ProtocolException(exception, isFinalPart: isFinalPart, extension: extension);
        }
    }
}

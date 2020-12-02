﻿//----------------------------------------------------------------------- 
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
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using Avro.IO;
using Avro.Specific;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Datatypes.Object;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Provides extension methods that can be used along with ETP message types.
    /// </summary>
    public static class EtpExtensions
    {
        private static readonly char[] WhiteSpace = Enumerable.Range(0, 20).Select(Convert.ToChar).ToArray();
        public const string GzipEncoding = "gzip";

        /// <summary>
        /// Converts a protocol and message type to a unique message key combination
        /// </summary>
        /// <param name="protocol">The message protocol.</param>
        /// <param name="messageType">The message type.</param>
        /// <returns>The message key.</returns>
        public static long CreateMessageKey(int protocol, int messageType)
        {
            // Special handling for Acknowledge and ProtocolException
            if (messageType == (int)v11.MessageTypes.Core.Acknowledge || messageType == (int)v11.MessageTypes.Core.ProtocolException)
                protocol = (int)v11.Protocols.Core;

            return (((long)protocol) << 32) + messageType;
        }

        private static IReadOnlyDictionary<string, string> RoleCounterparts = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["client"] = "server",
            ["server"] = "client",
            ["producer"] = "consumer",
            ["consumer"] = "producer",
            ["customer"] = "store",
            ["store"] = "customer",
        };

        /// <summary>
        /// Gets the name of the counterpart role for the specified role.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static string GetCounterpartRole(string role)
        {
            string counterpartRole;
            if (RoleCounterparts.TryGetValue(role, out counterpartRole))
                return counterpartRole;

            return null;
        }

        public static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings()
        {
            ContractResolver = new EtpContractResolver(),
            Converters = new List<JsonConverter>()
            {
                new ByteArrayConverter(),
                new NullableDoubleConverter(),
                new NullableIntConverter(),
                new NullableLongConverter(),
                new StringEnumConverter(),

                // TODO: new Etp11.Datatypes.DataValueConverter(),
                new v11.Datatypes.ChannelData.StreamingStartIndexConverter(),
                new v11.Datatypes.Object.GrowingObjectIndexConverter(),

                // TODO: new Etp12.Datatypes.DataValueConverter(),
                new v12.Datatypes.IndexValueConverter(),
                // new v12.Datatypes.Object.GrowingObjectIndexConverter()
            }
        };

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
        /// Encodes the specified message header and body.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="body">The message body.</param>
        /// <param name="header">The message header.</param>
        /// <param name="compression">The compression type.</param>
        /// <returns>The encoded byte array containing the message data.</returns>
        public static byte[] Encode<T>(this T body, IMessageHeader header, string compression) where T : ISpecificRecord
        {
            using (var stream = new MemoryStream())
            {
                // create avro binary encoder to write to memory stream
                var headerEncoder = new BinaryEncoder(stream);
                var bodyEncoder = headerEncoder;
                Stream gzip = null;

                try
                {
                    // compress message body if compression has been negotiated
                    if (header.CanCompressMessageBody())
                    {
                        if (GzipEncoding.Equals(compression, StringComparison.InvariantCultureIgnoreCase))
                        {
                            // add Compressed flag to message flags before writing header
                            header.SetBodyCompressed();

                            gzip = new GZipStream(stream, CompressionMode.Compress, true);
                            bodyEncoder = new BinaryEncoder(gzip);
                        }
                    }

                    // serialize header
                    var headerWriter = new SpecificWriter<IMessageHeader>(header.Schema);
                    headerWriter.Write(header, headerEncoder);

                    // serialize body
                    var bodyWriter = new SpecificWriter<T>(body.Schema);
                    bodyWriter.Write(body, bodyEncoder);
                }
                finally
                {
                    gzip?.Dispose();
                }

                return stream.ToArray();
            }
        }

        /// <summary>
        /// Decodes the message body using the specified decoder.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="decoder">The decoder.</param>
        /// <param name="body">The message body.</param>
        /// <returns>The decoded message body.</returns>
        public static T Decode<T>(this Decoder decoder, string body) where T : ISpecificRecord
        {
            if (!string.IsNullOrWhiteSpace(body))
                return Deserialize<T>(body);

            var record = Activator.CreateInstance<T>();
            var reader = new SpecificReader<T>(new EtpSpecificReader(record.Schema, record.Schema));

            reader.Read(record, decoder);

            return record;
        }

        /// <summary>
        /// Serializes the specified object instance.
        /// </summary>
        /// <param name="instance">The object to serialize.</param>
        /// <param name="indent">if set to <c>true</c> the JSON output should be indented; otherwise, <c>false</c>.</param>
        /// <returns>The serialized JSON string.</returns>
        public static string Serialize(object instance, bool indent = false)
        {
            var formatting = indent ? Formatting.Indented : Formatting.None;
            return JsonConvert.SerializeObject(instance, formatting, JsonSettings);
        }

        /// <summary>
        /// Deserializes the specified JSON string.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="json">The JSON string.</param>
        /// <returns></returns>
        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, JsonSettings);
        }

        /// <summary>
        /// Deserializes the specified JSON string.
        /// </summary>
        /// <param name="type">The type of object.</param>
        /// <param name="json">The JSON string.</param>
        /// <returns></returns>
        public static object Deserialize(Type type, string json)
        {
            return JsonConvert.DeserializeObject(json, type, JsonSettings);
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
        /// Determines whether the list of supported protocols contains the specified protocol and role combination.
        /// </summary>
        /// <param name="supportedProtocols">The supported protocols.</param>
        /// <param name="protocol">The requested protocol.</param>
        /// <param name="role">The requested role.</param>
        /// <returns>A value indicating whether the specified protocol and role combination is supported.</returns>
        public static bool Contains(this IReadOnlyList<ISupportedProtocol> supportedProtocols, int protocol, string role)
        {
            return supportedProtocols.Any(x => x.Protocol == protocol &&
                string.Equals(x.Role, role, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Gets the protocol capabilities for the requested protocol from the list of supported protocols.
        /// </summary>
        /// <param name="supportedProtocols">The supported protocols.</param>
        /// <param name="protocol">The protocol to get the capabilities for.</param>
        /// <returns>An <see cref="EtpProtocolCapabilities"/> instance containing the protocol's capabilities.</returns>
        public static EtpProtocolCapabilities ProtocolCapabilities(this IList<ISupportedProtocol> supportedProtocols, int protocol)
        {
            var capabilities = new Dictionary<string, IDataValue>();
            var protocolCapabilities = supportedProtocols?.FirstOrDefault(x => x.Protocol == protocol)?.ProtocolCapabilities;
            if (protocolCapabilities != null)
            {
                foreach (var key in protocolCapabilities.Keys)
                {
                    if (!(key is string))
                        continue;

                    var value = protocolCapabilities[key];
                    if (value == null || !(value is IDataValue))
                        continue;

                    capabilities[(string)key] = (IDataValue)value;
                }
            }

            return new EtpProtocolCapabilities(capabilities);
        }

        /// <summary>
        /// Converts the <see cref="Guid"/> to a UUID instance.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>A new UUID instance.</returns>
        public static v12.Datatypes.Uuid ToUuid(this Guid guid)
        {
            return new v12.Datatypes.Uuid
            {
                Value = GuidUtility.SwapByteOrder(guid.ToByteArray()),
            };
        }

        /// <summary>
        /// Converts the UUID to a <see cref="Guid"/> instance.
        /// </summary>
        /// <param name="uuid">The UUID.</param>
        /// <returns>A new <see cref="Guid"/> instance.</returns>
        public static Guid ToGuid(this v12.Datatypes.Uuid uuid)
        {
            return new Guid(GuidUtility.SwapByteOrder(uuid.Value));
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
        /// <param name="compress">if set to <c>true</c> the data will be compressed.</param>
        public static void SetString(this IDataObject dataObject, string data, bool compress = true)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                dataObject.SetData(new byte[0], compress);
                return;
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(data);

            //var bytes = System.Text.Encoding.Convert(
            //    System.Text.Encoding.UTF8,
            //    System.Text.Encoding.Unicode,
            //    System.Text.Encoding.UTF8.GetBytes(data));

            dataObject.SetData(bytes, compress);
        }

        /// <summary>
        /// Gets the data contained by the <see cref="IDataObject"/> and decompresses the byte array, if necessary.
        /// </summary>
        /// <param name="dataObject">The data object.</param>
        /// <returns>The decompressed data as a byte array.</returns>
        public static byte[] GetData(this IDataObject dataObject)
        {
            if (string.IsNullOrWhiteSpace(dataObject.ContentEncoding))
                return dataObject.Data;

            if (!GzipEncoding.Equals(dataObject.ContentEncoding, StringComparison.InvariantCultureIgnoreCase))
                throw new NotSupportedException("Content encoding not supported: " + dataObject.ContentEncoding);

            using (var uncompressed = new MemoryStream())
            {
                using (var compressed = new MemoryStream(dataObject.Data))
                using (var gzip = new GZipStream(compressed, CompressionMode.Decompress))
                {
                    gzip.CopyTo(uncompressed);
                }

                return uncompressed.ToArray();
            }
        }

        /// <summary>
        /// Sets and optionally compresses the data for the <see cref="IDataObject"/>.
        /// </summary>
        /// <param name="dataObject">The data object.</param>
        /// <param name="data">The data.</param>
        /// <param name="compress">if set to <c>true</c> the data will be compressed.</param>
        public static void SetData(this IDataObject dataObject, byte[] data, bool compress = true)
        {
            var encoding = string.Empty;

            if (compress)
            {
                using (var compressed = new MemoryStream())
                {
                    using (var uncompressed = new MemoryStream(data))
                    using (var gzip = new GZipStream(compressed, CompressionMode.Compress))
                    {
                        uncompressed.CopyTo(gzip);
                    }

                    data = compressed.ToArray();
                    encoding = GzipEncoding;
                }
            }

            dataObject.ContentEncoding = encoding;
            dataObject.Data = data;
        }

        /// <summary>
        /// Gets a list of protocols supported by the specified <see cref="IProtocolHandler"/>s.
        /// </summary>
        /// <param name="handlers">The <see cref="IProtocolHandler"/>s.</param>
        /// <param name="supportedVersion">The supported ETP version.</param>
        /// <returns>The list of supported protocols.</returns>
        public static IReadOnlyList<EtpSessionProtocol> GetSupportedProtocols(IEnumerable<IProtocolHandler> handlers, EtpVersion supportedVersion)
        {
            var supportedProtocols = new List<EtpSessionProtocol>();

            // Skip Core protocol (0)
            foreach (var handler in handlers.Where(x => x.Protocol > 0 && x.SupportedVersion == supportedVersion))
            {
                if (supportedProtocols.Contains(handler.Protocol, handler.Role))
                    continue;

                var capabilities = new EtpProtocolCapabilities();
                handler.GetCapabilities(capabilities);

                supportedProtocols.Add(new EtpSessionProtocol(handler.Protocol, handler.SupportedVersion.AsVersion(), handler.Role, handler.CounterpartRole, capabilities, null));
            }

            return supportedProtocols;
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
        /// Converts an <see cref="EtpVersion"/> to an <see cref="IVersion"/>.
        /// </summary>
        /// <param name="version">The version to convert.</param>
        /// <returns>The converted version.</returns>
        public static IVersion AsVersion(this EtpVersion version)
        {
            switch (version)
            {
                case EtpVersion.v11: return new v11.Datatypes.Version { Major = 1, Minor = 1 };
                case EtpVersion.v12: return new v12.Datatypes.Version { Major = 1, Minor = 2 };
                default: return null;
            }
        }
    }
}

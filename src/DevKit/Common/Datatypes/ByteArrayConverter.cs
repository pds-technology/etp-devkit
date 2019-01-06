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
using System.Linq;
using Newtonsoft.Json;

namespace Energistics.Etp.Common.Datatypes
{
    /// <summary>
    /// Converts a byte array to and from JSON.
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    public class ByteArrayConverter : JsonConverter
    {
        private static readonly IDictionary<byte, string> ByteToHexMap;
        private static readonly IDictionary<string, byte> HexToByteMap;
        private static readonly string[] Separator;

        /// <summary>
        /// Initializes the <see cref="ByteArrayConverter"/> class.
        /// </summary>
        static ByteArrayConverter()
        {
            ByteToHexMap = new Dictionary<byte, string>();
            HexToByteMap = new Dictionary<string, byte>();
            Separator = new[] { "\\" };

            InitializeByteMaps(ByteToHexMap, HexToByteMap);
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(byte[]) == objectType;
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null || reader.TokenType == JsonToken.None)
                return null;

            if (reader.TokenType != JsonToken.String)
                throw new Exception($"Unexpected token parsing unicode binary array. Expected String, got {reader.TokenType}");

            var hexString = $"{reader.Value}";
            var hexValues = hexString.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

            return hexValues.Select(x => HexToByteMap[x]).ToArray();
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var data = (byte[]) value;
            var propertyValue = string.Join(string.Empty, data.Select(x => ByteToHexMap[x]));

            writer.WriteValue(propertyValue);
        }

        private static void InitializeByteMaps(IDictionary<byte, string> byteToHexMap, IDictionary<string, byte> hexToByteMap)
        {
            var prefix = Separator[0];
            var index = byte.MinValue;

            while (true)
            {
                var hexValue = index.ToString("X2");
                var uintValue = hexValue[0] + ((uint)hexValue[1] << 16);
                var stringValue = $"u00{(char)uintValue}{(char)(uintValue >> 16)}";

                byteToHexMap[index] = prefix + stringValue;
                hexToByteMap[stringValue] = index;

                if (index == byte.MaxValue)
                    break;

                index++;
            }
        }
    }
}

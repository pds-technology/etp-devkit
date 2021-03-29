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
using Newtonsoft.Json.Linq;

namespace Energistics.Etp.v11.Datatypes
{
    /// <summary>
    /// Converts a <see cref="DataValue"/> to and from JSON.
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    public class DataValueConverter : JsonConverter
    {
        private const string ItemPropertyName = "item";

        private static readonly Dictionary<Type, string> TypeToName = new Dictionary<Type, string>
        {
            [typeof(bool)] = "boolean",
            [typeof(int)] = "int",
            [typeof(long)] = "long",
            [typeof(float)] = "float",
            [typeof(double)] = "double",
            [typeof(string)] = "string",
            [typeof(byte[])] = "bytes",
            [typeof(ArrayOfDouble)] = "Energistics.Etp.v12.Datatypes.ArrayOfDouble",
        };

        private static readonly Dictionary<string, Type> NameToType = new Dictionary<string, Type>
        {
            ["boolean"] = typeof(bool),
            ["int"] = typeof(int),
            ["long"] = typeof(long),
            ["float"] = typeof(float),
            ["double"] = typeof(double),
            ["string"] = typeof(string),
            ["bytes"] = typeof(byte[]),
            ["Energistics.Etp.v12.Datatypes.ArrayOfDouble"] = typeof(ArrayOfDouble),
        };

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(DataValue) == objectType;
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
            if (reader.TokenType == JsonToken.Null)
                return null;

            var dataValue = new DataValue();

            while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
            {
                var propertyName = reader.Value.ToString();
                reader.Read();

                if (!ItemPropertyName.Equals(propertyName))
                    continue;

                var value = serializer.Deserialize(reader);
                if (value == null)
                {
                    dataValue.Item = null;
                }
                else if (value is JObject)
                {
                    var itemValue = (value as JObject).Properties().FirstOrDefault(p => NameToType.ContainsKey(p.Name));
                    if (itemValue == null)
                        throw new JsonSerializationException($"Unsupported data type: {value}");

                    dataValue.Item = itemValue.ToObject(NameToType[itemValue.Name]);
                }
                else
                {
                    // Handle case where avro-json format is not used
                    dataValue.Item = value;
                }
            }

            return dataValue;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var index = value as DataValue;

            if (index == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartObject();
            writer.WritePropertyName(ItemPropertyName);

            if (index.Item == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteStartObject();

                string typeName;
                if (!TypeToName.TryGetValue(index.Item.GetType(), out typeName))
                    throw new JsonSerializationException($"Unsupported data type '{index.Item.GetType().FullName}' for value '{index.Item}'.");

                writer.WritePropertyName(typeName);

                serializer.Serialize(writer, index.Item, index.Item.GetType());
                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }
    }
}

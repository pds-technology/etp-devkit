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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Energistics.Etp.v12.Datatypes
{
    /// <summary>
    /// Converts a <see cref="IndexValueConverter"/> to and from JSON.
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    public class IndexValueConverter : JsonConverter
    {
        private const string ItemPropertyName = "item";
        private const string LongDataType = "long";
        private const string DoubleDataType = "double";

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(IndexValue) == objectType;
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

            var index = new IndexValue();

            while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
            {
                var propertyName = reader.Value.ToString();
                reader.Read();

                if (!ItemPropertyName.Equals(propertyName))
                    continue;

                var value = serializer.Deserialize(reader);

                // Handle case where avro-json format is not used
                if (value is long || value is double)
                {
                    index.Item = value;
                }
                // Handle number sent as string
                else if (value is string)
                {
                    long longValue;
                    double doubleValue;

                    if (long.TryParse(value.ToString(), out longValue))
                        index.Item = longValue;
                    else if (double.TryParse(value.ToString(), out doubleValue))
                        index.Item = doubleValue;
                }
                else if (value is JObject)
                {
                    var jObject = value as JObject;
                    var longProperty = jObject.Property(LongDataType);
                    var doubleProperty = jObject.Property(DoubleDataType);

                    if (longProperty != null)
                    {
                        index.Item = longProperty.Value.Value<long>();
                    }
                    else if (doubleProperty != null)
                    {
                        index.Item = doubleProperty.Value.Value<double>();
                    }
                    else
                    {
                        throw new JsonSerializationException($"Unsupported data type: {value}");
                    }
                }
                else if (value != null)
                {
                    throw new JsonSerializationException($"Unsupported data type: {value}");
                }
            }

            return index;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var index = value as IndexValue;

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

                if (index.Item is long)
                    writer.WritePropertyName(LongDataType);
                else if (index.Item is double)
                    writer.WritePropertyName(DoubleDataType);
                else
                    throw new JsonSerializationException($"Unsupported data type '{index.Item.GetType().FullName}' for value '{index.Item}'.");

                serializer.Serialize(writer, index.Item, index.Item.GetType());
                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }
    }
}

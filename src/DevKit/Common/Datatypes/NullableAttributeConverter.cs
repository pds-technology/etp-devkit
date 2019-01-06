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
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Energistics.Etp.Common.Datatypes
{
    /// <summary>
    /// Converts a nullable type to and from JSON.
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    public abstract class NullableAttributeConverter<T> : JsonConverter
    {
        private readonly string _propertyName;

        /// <summary>
        /// Initializes a new instance of the <see cref="NullableAttributeConverter{T}"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected NullableAttributeConverter(string propertyName)
        {
            _propertyName = propertyName;
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">The object type.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(T) == objectType;
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
            {
                if (!IsNullableType(objectType))
                    throw new JsonSerializationException($"Unsupported nullable type: {objectType.FullName}");

                return null;
            }

            // Read the Avro union format for nullable attributes, e.g. "uuid": { "string": "" }
            var value = serializer.Deserialize(reader);

            // Handle case where Avro JSON format is not used
            if (value is T)
            {
                return value;
            }
            // Handle possible primitive type conversion
            if (value is IConvertible && typeof(T).IsAssignableFrom(typeof(IConvertible)))
            {
                var convertible = value as IConvertible;
                return convertible.ToType(typeof(T), CultureInfo.InvariantCulture);
            }
            // Read logical type property value
            if (value is JObject)
            {
                var jObject = value as JObject;
                var property = jObject.Property(_propertyName);

                if (property != null)
                {
                    return property.Value.Value<T>();
                }
            }

            throw new JsonSerializationException($"Unsupported data type: {value}");
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

            writer.WriteStartObject();
            writer.WritePropertyName(_propertyName);
            writer.WriteValue(value);
            writer.WriteEndObject();
        }

        private bool IsNullableType(Type objectType)
        {
            return objectType == typeof(string) || Nullable.GetUnderlyingType(objectType) != null;
        }
    }
}

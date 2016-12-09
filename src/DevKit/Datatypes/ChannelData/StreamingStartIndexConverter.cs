//----------------------------------------------------------------------- 
// ETP DevKit, 1.1
//
// Copyright 2016 Energistics
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

namespace Energistics.Datatypes.ChannelData
{
    /// <summary>
    /// Converts a <see cref="StreamingStartIndex"/> to and from JSON.
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    public class StreamingStartIndexConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(StreamingStartIndex) == objectType;
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var index = new StreamingStartIndex();

            while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
            {
                var propertyName = reader.Value.ToString();

                if ("item".Equals(propertyName))
                {
                    reader.Read();

                    var value = serializer.Deserialize(reader) as JObject;

                    if (value != null)
                    {
                        var intProperty = value.Property("int");
                        var longProperty = value.Property("long");

                        if (intProperty != null)
                        {
                            index.Item = intProperty.Value.Value<int>();
                        }
                        else if (longProperty != null)
                        {
                            index.Item = longProperty.Value.Value<long>();
                        }
                    }
                }
            }

            return index;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var index = value as StreamingStartIndex;

            if (index == null)
            {
                writer.WriteNull();
                return;
            }


            writer.WriteStartObject();
            writer.WritePropertyName("item");

            if (index.Item == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteStartObject();

                if (index.Item is int)
                    writer.WritePropertyName("int");
                if (index.Item is long)
                    writer.WritePropertyName("long");

                serializer.Serialize(writer, index.Item, index.Item.GetType());
                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }
    }
}

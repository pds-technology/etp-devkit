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
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Avro;
using Avro.Specific;
using Energistics.Etp.Common.Datatypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Resolves member mappings for ETP message types.
    /// </summary>
    /// <seealso cref="CamelCasePropertyNamesContractResolver" />
    public class EtpContractResolver : CamelCasePropertyNamesContractResolver
    {
        private static readonly string[] NullStringTypes = { "null", "string" };

        /// <summary>
        /// Creates a <see cref="JsonProperty" /> for the given <see cref="MemberInfo" />.
        /// </summary>
        /// <param name="member">The member to create a <see cref="JsonProperty" /> for.</param>
        /// <param name="memberSerialization">The member's parent <see cref="MemberSerialization" />.</param>
        /// <returns>A created <see cref="JsonProperty" /> for the given <see cref="MemberInfo" />.</returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyType == typeof(Schema) || member.GetCustomAttributes(typeof(XmlIgnoreAttribute), true).Any())
            {
                property.ShouldSerialize = instance => false;
            }
            else if (property.PropertyType == typeof(string))
            {
                if (IsNullableString(member, property))
                {
                    property.Converter = new NullableStringConverter();
                    //property.MemberConverter = property.Converter;
                }
            }

            return property;
        }

        private bool IsNullableString(MemberInfo member, JsonProperty property)
        {
            if (member.DeclaringType == null) return false;
            if (!typeof(ISpecificRecord).IsAssignableFrom(member.DeclaringType)) return false;

            var instance = Activator.CreateInstance(member.DeclaringType) as ISpecificRecord;
            var schema = instance?.Schema as RecordSchema;

            var field = schema?[property.PropertyName];
            var union = field?.Schema as UnionSchema;

            if (union == null || union.Count != 2) return false;

            return NullStringTypes.Contains(union[0].Name)
                && NullStringTypes.Contains(union[1].Name);
        }
    }
}

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

using System.Collections.Concurrent;
using Avro;
using Avro.IO;
using Avro.Specific;

namespace Energistics.Etp.Common
{
    public class EtpSpecificReader : SpecificDefaultReader
    {
        private static readonly ConcurrentDictionary<string, string> Namespaces = new ConcurrentDictionary<string, string>();
        private const string RootNamespace = "Energistics.";

        public EtpSpecificReader(Schema writerSchema, Schema readerSchema) : base(writerSchema, readerSchema)
        {
        }

        protected override object ReadRecord(object reuse, RecordSchema writerSchema, Schema readerSchema, Decoder decoder)
        {
            reuse = reuse ?? CreateInstance(readerSchema, Schema.Type.Record);
            return base.ReadRecord(reuse, writerSchema, readerSchema, decoder);
        }

        protected override object ReadFixed(object reuse, FixedSchema writerSchema, Schema readerSchema, Decoder decoder)
        {
            reuse = reuse ?? CreateInstance(readerSchema, Schema.Type.Fixed);
            return base.ReadFixed(reuse, writerSchema, readerSchema, decoder);
        }

        protected override object ReadArray(object reuse, ArraySchema writerSchema, Schema readerSchema, Decoder decoder)
        {
            var arraySchema = readerSchema as ArraySchema;
            reuse = reuse ?? CreateInstance(arraySchema?.ItemSchema, Schema.Type.Array);
            return base.ReadArray(reuse, writerSchema, readerSchema, decoder);
        }

        protected override object ReadMap(object reuse, MapSchema writerSchema, Schema readerSchema, Decoder decoder)
        {
            var mapSchema = readerSchema as MapSchema;
            reuse = reuse ?? CreateInstance(mapSchema?.ValueSchema, Schema.Type.Map);
            return base.ReadMap(reuse, writerSchema, readerSchema, decoder);
        }

        private object CreateInstance(Schema schema, Schema.Type schemaType)
        {
            var recordSchema = schema as RecordSchema;
            var fixedSchema  = schema as FixedSchema;
            object instance = null;

            if (!string.IsNullOrWhiteSpace(recordSchema?.Fullname))
            {
                var typeName = GetTypeName(recordSchema.Fullname);
                instance = ObjectCreator.Instance.New(typeName, schemaType);
            }
            else if (!string.IsNullOrWhiteSpace(fixedSchema?.Fullname))
            {
                var typeName = GetTypeName(fixedSchema.Fullname);
                instance = ObjectCreator.Instance.New(typeName, schemaType);
            }

            return instance;
        }

        private string GetTypeName(string typeName)
        {
            return Namespaces.GetOrAdd(typeName, key =>
            {
                var result = key;

                // Map legacy namespaces to Etp.v11 namespace
                if (result.StartsWith(RootNamespace) && !result.StartsWith($"{RootNamespace}Etp."))
                    result = $"{RootNamespace}Etp.v11.{result.Substring(RootNamespace.Length)}";

                return result;
            });
        }
    }
}

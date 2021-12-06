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

using Energistics.Avro.Encoding.Converter;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Energistics.Etp.Data
{
    public class MockDataspace
    {
        public MockDataspace(string name = "")
        {
            Name = name;
        }

        public string Name { get; protected set; }
        public EtpUri Uri(EtpVersion version) => new EtpUri(version, Name);
        public static string ContentType => EtpContentType.DataspaceContentType;

        public Dictionary<Guid, MockObject> Objects { get; } = new Dictionary<Guid, MockObject>();

        public Dictionary<Guid, MockObject> DeletedObjects { get; } = new Dictionary<Guid, MockObject>();

        public List<MockFamily> Families { get; } = new List<MockFamily>();

        public v11.Datatypes.Object.Resource Resource11 => new v11.Datatypes.Object.Resource
        {
            Uuid = null,
            Uri = Uri(EtpVersion.v11),
            Name = string.IsNullOrEmpty(Name) ? "(Default Dataspace)" : Name,
            HasChildren = Families.Count, // Family
            ContentType = ContentType,
            ResourceType = ResourceTypes.Dataspace.ToString(),
            CustomData = new Dictionary<string, string>(),
            ChannelSubscribable = true,
            ObjectNotifiable = true,
            LastChanged = Objects.Count > 0 ? Objects.Values.Max(o => o.StoreLastWrite) : AvroConverter.UtcMinDateTime,
        };

        public v12.Datatypes.Object.Dataspace Dataspace12 => new v12.Datatypes.Object.Dataspace
        {
            Uri = Uri(EtpVersion.v12),
            Path = Name,
            StoreLastWrite = Objects.Count > 0 ? Objects.Values.Max(o => o.StoreLastWrite) : AvroConverter.UtcMinDateTime,
            StoreCreated = Objects.Count > 0 ? Objects.Values.Min(o => o.StoreCreated) : AvroConverter.UtcMinDateTime,
            CustomData = new Dictionary<string, v12.Datatypes.DataValue>(),
        };
    }
}

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
using System.Collections.Generic;

namespace Energistics.Etp.Data
{
    public class MockFamily
    {
        public MockDataspace Dataspace { get; set; }

        public string Title { get; set; }

        public IDataObjectType Type { get; set; }

        public int SupportedObjectCount { get; set; }

        public v11.Datatypes.Object.Resource Resource11 => new v11.Datatypes.Object.Resource
        {
            Uuid = null,
            Uri = new EtpUri(EtpVersion.v11, Type.Family, Type.Version, dataspace: Dataspace?.Name),
            Name = Title,
            HasChildren = SupportedObjectCount,
            ContentType = Type.ContentType,
            ResourceType = ResourceTypes.UriProtocol.ToString(),
            CustomData = new Dictionary<string, string>(),
            ChannelSubscribable = true,
            ObjectNotifiable = true,
            LastChanged = AvroConverter.UtcMinDateTime,
        };
    }
}

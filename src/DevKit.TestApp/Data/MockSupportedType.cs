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

using Energistics.Etp.Common.Datatypes;
using System.Collections.Generic;

namespace Energistics.Etp.Data
{
    public class MockSupportedType
    {
        public EtpUri Uri { get; set; }

        public IDataObjectType DataObjectType { get; set; }

        public int? ObjectCount { get; set; }

        public v11.Datatypes.Object.Resource Resource11 => new v11.Datatypes.Object.Resource
        {
            Uuid = null,
            Uri = Uri,
            Name = DataObjectType.ObjectType,
            HasChildren = ObjectCount ?? -1,
            ContentType = DataObjectType.ContentType,
            ResourceType = ResourceTypes.Folder.ToString(),
            CustomData = new Dictionary<string, string>(),
            ChannelSubscribable = true,
            ObjectNotifiable = true,
            LastChanged = 0L,
        };

        public v12.Datatypes.Object.SupportedType SupportedType12 => new v12.Datatypes.Object.SupportedType
        {
            DataObjectType = DataObjectType.DataObjectType,
            ObjectCount = ObjectCount,
        };
    }
}

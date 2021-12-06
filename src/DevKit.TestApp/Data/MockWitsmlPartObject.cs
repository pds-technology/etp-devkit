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

using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using System;
using System.Collections.Generic;

namespace Energistics.Etp.Data
{
    public abstract class MockWitsmlPartObject<TPart> : MockWitsmlGrowingObject, IMockPartObject<TPart>
    {
        public override bool IsTime => false;
        public List<TPart> Parts { get; } = new List<TPart>();

        public override object Value(int dataIndex) => Parts[dataIndex];

        public abstract string Xml(EtpVersion version, bool withParts, string indentation = "", bool embedded = false);

        public v12.Datatypes.Object.PartsMetadataInfo PartsMetadataInfo12(long channelId) => new v12.Datatypes.Object.PartsMetadataInfo
        {
            Name = Title,
            Uri = Uri(EtpVersion.v12),
            Index = IndexMetadataRecord12,
            CustomData = new Dictionary<string, v12.Datatypes.DataValue>(),
        };
    }
}

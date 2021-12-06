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

using System.Collections.Generic;

namespace Energistics.Etp.Data
{
    public struct MockDataItem
    {
        public long ChannelId;
        public long? LongIndex;
        public double? DoubleIndex;
        public object Value;

        public v11.Datatypes.ChannelData.DataItem DataItem11 => new v11.Datatypes.ChannelData.DataItem
        {
            ChannelId = ChannelId,
            Indexes = (LongIndex == null) ? new List<long>() : new List<long> { LongIndex.Value },
            Value = new v11.Datatypes.DataValue { Item = Value },
            ValueAttributes = new List<v11.Datatypes.DataAttribute>(),
        };

        public v12.Datatypes.ChannelData.DataItem DataItem12 => new v12.Datatypes.ChannelData.DataItem
        {
            ChannelId = ChannelId,
            Indexes = new List<v12.Datatypes.IndexValue> { new v12.Datatypes.IndexValue { Item = LongIndex ?? DoubleIndex } },
            Value = new v12.Datatypes.DataValue { Item = Value },
            ValueAttributes = new List<v12.Datatypes.DataAttribute>(),
        };
    };
}

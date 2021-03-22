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
using System;

namespace Energistics.Etp.Data
{
    public class MockIndex
    {
        public MockIndex(v11.Datatypes.ChannelData.StreamingStartIndex index)
        {
            Version = EtpVersion.v11;
            if (index.Item == null)
                IndexCount = 0;
            else if (index.Item is int)
                IndexCount = (int)index.Item;
            else
                Index = index.Item as IComparable;
        }

        public MockIndex(v12.Datatypes.ChannelData.ChannelSubscribeInfo info)
        {
            Version = EtpVersion.v12;
            if (info.StartIndex?.Item == null)
                IndexCount = info.RequestLatestIndexCount ?? 0;
            else
                Index = info.StartIndex.Item as IComparable;
        }

        public MockIndex(v12.Datatypes.IndexValue index)
        {
            Version = EtpVersion.v12;
            Index = index?.Item as IComparable;
        }

        public MockIndex(EtpVersion version, long index)
        {
            Version = version;
            Index = index;
        }

        public EtpVersion Version { get; }
        public int? IndexCount { get; }
        public IComparable Index { get; }
    };
}

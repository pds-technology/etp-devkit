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
using System.Collections.Generic;

namespace Energistics.Etp.Data
{
    public interface IMockActiveObject
    {
        bool IsActive { get; }

        DateTime LastActivatedTime { get; }

        DateTime ActiveChangeTime { get; }

        void SetActive(bool active, DateTime activeChangeTime);
    }

    public interface IMockGrowingObject : IMockActiveObject
    {
        bool IsTime { get; }
        string Mnemonic { get; set; }
        int DepthScale { get; }
        double DepthScaleValue { get; }
        string DataType { get; set; }
        string Uom { get; set; }
        MockPropertyKind ChannelPropertyKind { get; set; }
        MockPropertyKind IndexPropertyKind { get; set; }
        string IndexMnemonic { get; set; }
        string IndexUom { get; set; }
        IComparable StartIndex { get; }
        IComparable EndIndex { get; }
        double? DepthStartIndex { get; }
        double? DepthEndIndex { get; }
        DateTime? TimeStartIndex { get; }
        DateTime? TimeEndIndex { get; }
        int DataCount { get; }
        DateTime DataLastWrite { get; }

        int GetStartDataIndex(IComparable comparable, bool inclusive);
        int GetEndDataIndex(IComparable comparable, bool inclusive);
        IComparable Index(int dataIndex);
        double DepthIndex(int dataIndex);
        DateTime TimeIndex(int dataIndex);
        object Value(int dataIndex);
        v11.Datatypes.ChannelData.ChannelMetadataRecord ChannelMetadataRecord11(long channelId);
        double LongToDepthIndex(long value);
        long DepthIndexToLong(double value);
        MockDataItem DataItem(EtpVersion version, long channelId, int dataIndex, bool nullIndex);
        IComparable GetIndex(MockIndex index);
    }

    public interface IMockPartObject<TPart> : IMockGrowingObject
    {
        string Xml(EtpVersion version, bool withParts, string indentation = "", bool embedded = false);
        List<TPart> Parts { get; }
    }
}

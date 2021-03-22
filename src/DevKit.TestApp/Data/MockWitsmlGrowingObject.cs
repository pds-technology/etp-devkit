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
    public abstract class MockWitsmlGrowingObject : MockWitsmlActiveObject, IMockGrowingObject
    {
        public string Mnemonic { get; set; }
        public abstract bool IsTime { get; }
        public int DepthScale => 3;
        public double DepthScaleValue => 1000.0;
        public string DataType { get; set; }
        public string Uom { get; set; } = string.Empty;
        public MockPropertyKind ChannelClass { get; set; }
        public string IndexMnemonic { get; set; }
        public string IndexUom { get; set; }
        public IComparable StartIndex => IsTime ? (IComparable)TimeStartIndex : DepthStartIndex;
        public IComparable EndIndex => IsTime ? (IComparable)TimeEndIndex : DepthEndIndex;
        public List<double> DepthIndexes { get; } = new List<double>();
        public List<DateTime> TimeIndexes { get; } = new List<DateTime>();
        public DateTime AppendTime { get; private set; }
        public double? DepthStartIndex => DepthIndexes.Count > 0 ? (double?)DepthIndexes[0] : null;
        public double? DepthEndIndex => DepthIndexes.Count > 0 ? (double?)DepthIndexes[DepthIndexes.Count - 1] : null;
        public DateTime? TimeStartIndex => TimeIndexes.Count > 0 ? (DateTime?)TimeIndexes[0] : null;
        public DateTime? TimeEndIndex => TimeIndexes.Count > 0 ? (DateTime?)TimeIndexes[TimeIndexes.Count - 1] : null;
        public int DataCount => IsTime ? TimeIndexes.Count : DepthIndexes.Count;
        public IComparable Index(int dataIndex) => IsTime ? (IComparable)TimeIndex(dataIndex) : DepthIndex(dataIndex);
        public double DepthIndex(int dataIndex) => DepthIndexes[dataIndex];
        public DateTime TimeIndex(int dataIndex) => TimeIndexes[dataIndex];
        public abstract object Value(int dataIndex);
        public double LongToDepthIndex(long value) => value / DepthScaleValue;
        public long DepthIndexToLong(double value) => (long)(value * DepthScaleValue);
        protected void SetAppendTime(DateTime appendTime)
        {
            AppendTime = appendTime;
            UpdateDataLastWrite(appendTime);
            SetActive(true, appendTime);
        }

        public MockDataItem DataItem(EtpVersion version, long channelId, int dataIndex, bool nullIndex)
        {
            if (IsTime)
            {
                return new MockDataItem
                {
                    ChannelId = channelId,
                    LongIndex = nullIndex ? (long?)null : TimeIndex(dataIndex).ToEtpTimestamp(),
                    Value = Value(dataIndex),
                };
            }
            else
            {
                return new MockDataItem
                {
                    ChannelId = channelId,
                    LongIndex = version == EtpVersion.v11 ? (nullIndex ? (long?)null : DepthIndexToLong(DepthIndex(dataIndex))) : null,
                    DoubleIndex = version == EtpVersion.v12 ? (nullIndex ? (double?)null : DepthIndex(dataIndex)) : null,
                    Value = Value(dataIndex),
                };
            }
        }

        public IComparable GetIndex(MockIndex index)
        {
            if (index.Index == null)
            {
                if (DataCount == 0)
                    return IsTime ? (IComparable)0L.ToUtcDateTime() : double.MinValue;

                var i = DataCount - (index.IndexCount ?? 0);
                if (i < 0)
                    return StartIndex;
                else if (i >= DataCount)
                    return EndIndex;
                else
                    return Index(i);
            }
            else
            {
                if (IsTime && index.Index is long)
                    return ((long)index.Index).ToUtcDateTime();
                else if (!IsTime && index.Version == EtpVersion.v11 && index.Index is long)
                    return LongToDepthIndex((long)index.Index);
                else if (!IsTime && index.Version == EtpVersion.v12 && index.Index is double)
                    return (double)index.Index;
                else
                    return null;
            }
        }

        public int GetStartDataIndex(IComparable comparable, bool inclusive)
        {
            return GetDataIndex(comparable, inclusive, true);
        }

        public int GetEndDataIndex(IComparable comparable, bool inclusive)
        {
            return GetDataIndex(comparable, inclusive, false);
        }

        private int GetDataIndex(IComparable comparable, bool inclusive, bool isStart)
        {
            if (DataCount == 0)
                return 0;

            if (comparable == null)
                return DataCount - 1;

            if ((inclusive && comparable.CompareTo(EndIndex) > 0) || (!inclusive && comparable.CompareTo(EndIndex) >= 0))
                return DataCount;
            else if ((inclusive && comparable.CompareTo(StartIndex) <= 0) || (!inclusive && comparable.CompareTo(StartIndex) < 0))
                return 0;
            else
            {
                int dataIndex;
                if (IsTime)
                    dataIndex = TimeIndexes.BinarySearch((DateTime)comparable);
                else
                    dataIndex = DepthIndexes.BinarySearch((double)comparable);

                if (dataIndex >= 0)
                    return inclusive ? (isStart ? dataIndex : ++dataIndex) : (isStart ? ++dataIndex : dataIndex);
                else
                    return ~dataIndex;
            }
        }

        public v11.Datatypes.ChannelData.ChannelMetadataRecord ChannelMetadataRecord11(long channelId) => new v11.Datatypes.ChannelData.ChannelMetadataRecord
        {
            ChannelId = channelId,
            Uuid = Uuid.ToString(),
            ChannelUri = Uri(EtpVersion.v11),
            ChannelName = Mnemonic,
            ContentType = ContentType,
            Description = Title,
            DataType = DataType,
            Status = IsActive ? v11.Datatypes.ChannelData.ChannelStatuses.Active : v11.Datatypes.ChannelData.ChannelStatuses.Inactive,
            Uom = Uom,
            MeasureClass = ChannelClass?.Title ?? string.Empty,
            Indexes = new List<v11.Datatypes.ChannelData.IndexMetadataRecord>
            {
                new v11.Datatypes.ChannelData.IndexMetadataRecord
                {
                    Uri = string.Concat(Uri(EtpVersion.v11), IndexMnemonic),
                    IndexType = IsTime ? v11.Datatypes.ChannelData.ChannelIndexTypes.Time : v11.Datatypes.ChannelData.ChannelIndexTypes.Depth,
                    Uom = IndexUom,
                    Mnemonic = IndexMnemonic,
                    Direction = v11.Datatypes.ChannelData.IndexDirections.Increasing,
                    Scale = 3,
                    DepthDatum = IsTime ? string.Empty : "KB",
                    TimeDatum = string.Empty,
                    Description = string.Empty,
                    CustomData = new Dictionary<string, v11.Datatypes.DataValue>(),
                }
            },
            StartIndex = IsTime ? TimeStartIndex?.ToEtpTimestamp() : (DepthStartIndex == null ? (long?)null : DepthIndexToLong(DepthStartIndex.Value)),
            EndIndex = IsTime ? TimeEndIndex?.ToEtpTimestamp() : (DepthEndIndex == null ? (long?)null : DepthIndexToLong(DepthEndIndex.Value)),
            Source = string.Empty,
            DomainObject = DataObject11(true),
            CustomData = new Dictionary<string, v11.Datatypes.DataValue>(),
        };
    }
}

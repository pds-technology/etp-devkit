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
using System.Globalization;

namespace Energistics.Etp.Data
{
    public class MockChannel : MockWitsmlGrowingObject
    {
        new public static EtpDataObjectType Type { get; } = new EtpDataObjectType(MockWitsmlObject.Type, "Channel");

        public static HashSet<EtpDataObjectType> SourceTypes { get; } = new HashSet<EtpDataObjectType> { };
        public static HashSet<EtpDataObjectType> TargetTypes { get; } = new HashSet<EtpDataObjectType> { MockWellbore.Type, MockChannelSet.Type };
        public static HashSet<EtpDataObjectType> SecondarySourceTypes { get; } = new HashSet<EtpDataObjectType> { };
        public static HashSet<EtpDataObjectType> SecondaryTargetTypes { get; } = new HashSet<EtpDataObjectType> { MockPropertyKind.Type };

        public override HashSet<EtpDataObjectType> SupportedSourceTypes => SourceTypes;
        public override HashSet<EtpDataObjectType> SupportedTargetTypes => TargetTypes;
        public override HashSet<EtpDataObjectType> SupportedSecondarySourceTypes => SecondarySourceTypes;
        public override HashSet<EtpDataObjectType> SupportedSecondaryTargetTypes => SecondaryTargetTypes;


        public MockChannel(bool isTime)
        {
            DataObjectType = Type;
            IsTime = isTime;
            DataType = "double";
            IndexMnemonic = IsTime ? "Time" : "Depth";
            IndexUom = IsTime ? "s" : "m";
        }

        public override void Link()
        {
            base.Link();

            if (!IsDeleted && ChannelPropertyKind != null && !ChannelPropertyKind.IsDeleted)
            {
                SecondaryTargets[ChannelPropertyKind.Uuid] = ChannelPropertyKind;
                ChannelPropertyKind.SecondarySources[Uuid] = this;
            }
        }

        public void AppendTimeData(DateTime index, double value, DateTime appendTime)
        {
            if (IsDeleted || !IsTime || (TimeEndIndex != null && index <= TimeEndIndex))
                return;

            TimeIndexes.Add(index);
            Data.Add(value);
            UpdateDataLastWrite(appendTime);
        }

        public void AppendDepthData(double index, double value, DateTime appendTime)
        {
            if (IsDeleted || IsTime || (DepthEndIndex != null && index <= DepthEndIndex))
                return;

            DepthIndexes.Add(index);
            Data.Add(value);
            UpdateDataLastWrite(appendTime);
        }

        public override bool IsTime { get; }
        public List<double> Data { get; } = new List<double>();

        public v12.Datatypes.ChannelData.ChannelMetadataRecord ChannelMetadataRecord12(long channelId) => new v12.Datatypes.ChannelData.ChannelMetadataRecord
        {
            Id = channelId,
            ChannelName = Mnemonic,
            Uri = Uri(EtpVersion.v12),
            DataKind = v12.Datatypes.ChannelData.ChannelDataKind.typeDouble,
            Status = IsActive ? v12.Datatypes.Object.ActiveStatusKind.Active : v12.Datatypes.Object.ActiveStatusKind.Inactive,
            Uom = Uom,
            ChannelClassUri = ChannelPropertyKind?.Uri(EtpVersion.v12),
            Indexes = new List<v12.Datatypes.ChannelData.IndexMetadataRecord>
            {
                IndexMetadataRecord12,
            },
            AxisVectorLengths = new List<int>(),
            AttributeMetadata = new List<v12.Datatypes.AttributeMetadataRecord>(),
            Source = string.Empty,
            CustomData = new Dictionary<string, v12.Datatypes.DataValue>(),
        };
        public v12.Datatypes.ChannelData.FrameChannelMetadataRecord FrameChannelMetadataRecord12 => new v12.Datatypes.ChannelData.FrameChannelMetadataRecord
        {
            ChannelName = Mnemonic,
            Uri = Uri(EtpVersion.v12),
            DataKind = v12.Datatypes.ChannelData.ChannelDataKind.typeDouble,
            Status = IsActive ? v12.Datatypes.Object.ActiveStatusKind.Active : v12.Datatypes.Object.ActiveStatusKind.Inactive,
            Uom = Uom,
            ChannelPropertyKindUri = ChannelPropertyKind?.Uri(EtpVersion.v12),
            AxisVectorLengths = new List<int>(),
            AttributeMetadata = new List<v12.Datatypes.AttributeMetadataRecord>(),
            Source = string.Empty,
            CustomData = new Dictionary<string, v12.Datatypes.DataValue>(),
        };

        public override object Value(int dataIndex) => Data[dataIndex];

        public override string Xml(EtpVersion version, string indentation = "", bool embedded = false) =>
$@"{indentation}<Channel{Namespaces(embedded)} schemaVersion=""2.0"" uuid=""{Uuid}""{DefaultNamespace(embedded)}>
{indentation}  <Citation xmlns=""http://www.energistics.org/energyml/data/commonv2"">
{indentation}    <Title>{Title}</Title>
{indentation}    <Originator>ETP DevKit</Originator>
{indentation}    <Creation>{Creation.ToUniversalTime():O}</Creation>
{indentation}    <Format>Energistics:ETP DevKit {typeof(IEtpSession).Assembly.GetName().Version}</Format>
{indentation}    <LastUpdate>{LastUpdate.ToUniversalTime():O}</LastUpdate>
{indentation}  </Citation>
{indentation}  <Mnemonic>{Mnemonic}</Mnemonic>
{indentation}  <DataType>{DataType}</DataType>
{indentation}  <Uom>{Uom}</Uom>
{indentation}  <GrowingStatus>{(IsActive ? "active" : "inactive")}</GrowingStatus>
{indentation}  <Wellbore>
{indentation}    <ContentType xmlns=""http://www.energistics.org/energyml/data/commonv2"">{Wellbore.ContentType}</ContentType>
{indentation}    <Title xmlns=""http://www.energistics.org/energyml/data/commonv2"">{Wellbore.Title}</Title>
{indentation}    <Uuid xmlns=""http://www.energistics.org/energyml/data/commonv2"">{Wellbore.Uuid}</Uuid>
{indentation}    <Uri xmlns=""http://www.energistics.org/energyml/data/commonv2"">{Wellbore.Uri(version)}</Uri>
{indentation}  </Wellbore>
{indentation}  <TimeDepth>{(IsTime ? "time" : "depth")}</TimeDepth>
{indentation}  <ChannelClass>
{indentation}    <ContentType xmlns=""http://www.energistics.org/energyml/data/commonv2"">{ChannelPropertyKind.ContentType}</ContentType>
{indentation}    <Title xmlns=""http://www.energistics.org/energyml/data/commonv2"">{ChannelPropertyKind.Title}</Title>
{indentation}    <Uuid xmlns=""http://www.energistics.org/energyml/data/commonv2"">{ChannelPropertyKind.Uuid}</Uuid>
{indentation}    <Uri xmlns=""http://www.energistics.org/energyml/data/commonv2"">{ChannelPropertyKind.Uri(version)}</Uri>
{indentation}  </ChannelClass>
{indentation}  <StartIndex xsi:type=""{(IsTime ? "TimeIndexValue" : "DepthIndexValue")}"">
{indentation}    <{(IsTime ? "Time" : "Depth")}>{(IsTime ? TimeStartIndex?.ToString("O", CultureInfo.InvariantCulture) : DepthStartIndex?.ToString(CultureInfo.InvariantCulture))}</{(IsTime ? "Time" : "Depth")}>
{indentation}  </StartIndex>
{indentation}  <EndIndex xsi:type=""{(IsTime ? "TimeIndexValue" : "DepthIndexValue")}"">
{indentation}    <{(IsTime ? "Time" : "Depth")}>{(IsTime ? TimeEndIndex?.ToString("O", CultureInfo.InvariantCulture) : DepthEndIndex?.ToString(CultureInfo.InvariantCulture))}</{(IsTime ? "Time" : "Depth")}>
{indentation}  </EndIndex>
{indentation}  <LoggingCompanyName>Energistics</LoggingCompanyName>
{indentation}  <LoggingCompanyCode>1000</LoggingCompanyCode>
{indentation}  <Index>
{indentation}    <IndexType>{(IsTime ? "date time" : "measured depth")}</IndexType>
{indentation}    <Uom>{IndexUom}</Uom>
{indentation}    <Direction>increasing</Direction>
{indentation}    <Mnemonic>{IndexMnemonic}</Mnemonic>
{indentation}  </Index>
{indentation}</Channel>";
    }
}

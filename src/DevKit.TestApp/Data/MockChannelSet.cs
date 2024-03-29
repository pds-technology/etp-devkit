﻿//----------------------------------------------------------------------- 
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
using System.Linq;

namespace Energistics.Etp.Data
{
    public class MockChannelSet : MockWitsmlWellboreObject
    {
        new public static EtpDataObjectType Type { get; } = new EtpDataObjectType(MockWitsmlObject.Type, "ChannelSet");

        public static HashSet<EtpDataObjectType> SourceTypes { get; } = new HashSet<EtpDataObjectType> { MockChannel.Type };
        public static HashSet<EtpDataObjectType> TargetTypes { get; } = new HashSet<EtpDataObjectType> { MockWellbore.Type };
        public static HashSet<EtpDataObjectType> SecondarySourceTypes { get; } = new HashSet<EtpDataObjectType> { };
        public static HashSet<EtpDataObjectType> SecondaryTargetTypes { get; } = new HashSet<EtpDataObjectType> { };

        public override HashSet<EtpDataObjectType> SupportedSourceTypes => SourceTypes;
        public override HashSet<EtpDataObjectType> SupportedTargetTypes => TargetTypes;
        public override HashSet<EtpDataObjectType> SupportedSecondarySourceTypes => SecondarySourceTypes;
        public override HashSet<EtpDataObjectType> SupportedSecondaryTargetTypes => SecondaryTargetTypes;

        public MockChannelSet()
        {
            DataObjectType = Type;
        }

        protected override void UpdateDataLastWrite(DateTime lastWrite)
        {
            base.UpdateDataLastWrite(lastWrite);
            SetActive(true, lastWrite);
            if (Wellbore != null)
                Wellbore.SetActive(true, lastWrite);
        }

        protected override void JoinContainee(MockObject child, DateTime unjoinTime)
        {
            base.JoinContainee(child, unjoinTime);
            if (child is MockChannel)
                Channels.Add(child as MockChannel);
        }

        protected override void UnjoinContainee(MockObject child, DateTime unjoinTime)
        {
            base.UnjoinContainee(child, unjoinTime);
            if (child is MockChannel)
                Channels.Remove(child as MockChannel);
        }

        private List<MockChannel> Channels { get; set; } = new List<MockChannel>();
        public bool IsTime => Channels.FirstOrDefault()?.IsTime ?? false;
        public MockPropertyKind IndexPropertyKind => Channels.FirstOrDefault()?.IndexPropertyKind;
        public string IndexMnemonic => Channels.FirstOrDefault()?.Mnemonic ?? string.Empty;
        public string IndexUom => Channels.FirstOrDefault()?.IndexUom ?? string.Empty;

        public double? DepthStartIndex
        {
            get
            {
                double? start = null;
                foreach (var c in Channels)
                {
                    if (start == null || (c.DepthStartIndex != null && c.DepthStartIndex < start))
                        start = c.DepthStartIndex;
                }
                return start;
            }
        }

        public double? DepthEndIndex
        {
            get
            {
                double? end = null;
                foreach (var c in Channels)
                {
                    if (end == null || (c.DepthEndIndex != null && c.DepthEndIndex > end))
                        end = c.DepthEndIndex;
                }
                return end;
            }
        }

        public DateTime? TimeStartIndex
        {
            get
            {
                DateTime? start = null;
                foreach (var c in Channels)
                {
                    if (start == null || (c.TimeStartIndex != null && c.TimeStartIndex < start))
                        start = c.TimeStartIndex;
                }
                return start;
            }
        }

        public DateTime? TimeEndIndex
        {
            get
            {
                DateTime? end = null;
                foreach (var c in Channels)
                {
                    if (end == null || (c.TimeEndIndex != null && c.TimeEndIndex > end))
                        end = c.TimeEndIndex;
                }
                return end;
            }
        }

        public v12.Datatypes.ChannelData.IndexMetadataRecord IndexMetadataRecord12 => new v12.Datatypes.ChannelData.IndexMetadataRecord
        {
            Name = IndexMnemonic ?? string.Empty,
            Uom = IndexUom ?? string.Empty,
            DepthDatum = IsTime ? string.Empty : "KB",
            Direction = v12.Datatypes.ChannelData.IndexDirection.Increasing,
            IndexKind = IsTime ? v12.Datatypes.ChannelData.ChannelIndexKind.DateTime : v12.Datatypes.ChannelData.ChannelIndexKind.MeasuredDepth,
            IndexPropertyKindUri = IndexPropertyKind?.Uri(EtpVersion.v12) ?? string.Empty,
            Interval = new v12.Datatypes.Object.IndexInterval
            {
                StartIndex = new v12.Datatypes.IndexValue { Item = IsTime ? TimeStartIndex?.ToEtpTimestamp() : DepthStartIndex },
                EndIndex = new v12.Datatypes.IndexValue { Item = IsTime ? TimeEndIndex?.ToEtpTimestamp() : DepthEndIndex },
                DepthDatum = IsTime ? string.Empty : "KB",
                Uom = IndexUom ?? string.Empty,
            }
        };

        public override string Xml(EtpVersion version, string indentation = "", bool embedded = false) =>
$@"{indentation}<ChannelSet{Namespaces(embedded)} schemaVersion=""2.0"" uuid=""{Uuid}""{DefaultNamespace(embedded)}>
{indentation}  <Citation xmlns=""http://www.energistics.org/energyml/data/commonv2"">
{indentation}    <Title>{Title}</Title>
{indentation}    <Originator>ETP DevKit</Originator>
{indentation}    <Creation>{Creation.ToUniversalTime():O}</Creation>
{indentation}    <Format>Energistics:ETP DevKit {typeof(IEtpSession).Assembly.GetName().Version}</Format>
{indentation}    <LastUpdate>{LastUpdate.ToUniversalTime():O}</LastUpdate>
{indentation}  </Citation>
{indentation}  <Index>
{indentation}    <IndexType>{(IsTime ? "date time" : "measured depth")}</IndexType>
{indentation}    <Uom>{(IsTime ? "s" : "m")}</Uom>
{indentation}    <Direction>increasing</Direction>
{indentation}    <Mnemonic>{(IsTime ? "Time" : "Depth")}</Mnemonic>
{indentation}  </Index>
{string.Concat(Channels.Select(c => c.Xml(version, indentation = "  ", embedded = true)))}
{indentation}  <TimeDepth>{(IsTime ? "time" : "depth")}</TimeDepth>
{indentation}  <StartIndex xsi:type=""{(IsTime ? "TimeIndexValue" : "DepthIndexValue")}"">
{indentation}    <{(IsTime ? "Time" : "Depth")}>{(IsTime ? TimeStartIndex?.ToString("O", CultureInfo.InvariantCulture) : DepthStartIndex?.ToString(CultureInfo.InvariantCulture))}</{(IsTime ? "Time" : "Depth")}>
{indentation}  </StartIndex>
{indentation}  <EndIndex xsi:type=""{(IsTime ? "TimeIndexValue" : "DepthIndexValue")}"">
{indentation}    <{(IsTime ? "Time" : "Depth")}>{(IsTime ? TimeEndIndex?.ToString("O", CultureInfo.InvariantCulture) : DepthEndIndex?.ToString(CultureInfo.InvariantCulture))}</{(IsTime ? "Time" : "Depth")}>
{indentation}  </EndIndex>
{indentation}  <LoggingCompanyName>ETP DevKit</LoggingCompanyName>
{indentation}  <Wellbore>
{indentation}    <ContentType xmlns=""http://www.energistics.org/energyml/data/commonv2"">{Wellbore.ContentType}</ContentType>
{indentation}    <Title xmlns=""http://www.energistics.org/energyml/data/commonv2"">{Wellbore.Title}</Title>
{indentation}    <Uuid xmlns=""http://www.energistics.org/energyml/data/commonv2"">{Wellbore.Uuid}</Uuid>
{indentation}    <Uri xmlns=""http://www.energistics.org/energyml/data/commonv2"">{Wellbore.Uri(version)}</Uri>
{indentation}  </Wellbore>
{indentation}</ChannelSet>";
    }
}

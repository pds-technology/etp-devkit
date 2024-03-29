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

using System;
using System.Collections.Generic;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v12.Datatypes;
using Energistics.Etp.v12.Datatypes.ChannelData;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.ChannelStreaming
{
    public class ChannelStreamingProducer12MockHandler : ChannelStreamingProducerHandler
    {
        public ChannelStreamingProducer12MockHandler()
        {
        }

        protected override void HandleStartStreaming(VoidRequestEventArgs<StartStreaming> args)
        {
            base.HandleStartStreaming(args);

            var channelMetaData = new List<ChannelMetadataRecord>()
            {
                new ChannelMetadataRecord()
                {
                    Uri = "eml:///witsml20.Channel(18c518fa-1a11-43a1-9a86-36d651ddbf7d)",
                    Id = 1,
                    Indexes = new List<IndexMetadataRecord>()
                    {
                        new IndexMetadataRecord()
                        {
                            IndexKind = ChannelIndexKind.MeasuredDepth,
                            Interval = new Datatypes.Object.IndexInterval
                            {
                                StartIndex = new IndexValue { Item = 0L },
                                EndIndex = new IndexValue { Item = 0L },
                                Uom = "m",
                                DepthDatum = string.Empty,
                            },
                            Direction = IndexDirection.Increasing,
                            Name = "Depth",
                        },
                    },
                    ChannelName = "test",
                    DataKind = ChannelDataKind.typeDouble,
                    Uom = "m",
                    Status = ActiveStatusKind.Active,
                    AxisVectorLengths = new int[0],
                    Source = "ETP DevKit",
                    ChannelClassUri = "eml:///eml21.PropertyKind(44964f15-701a-48b0-aec2-281186bddc24)",
                    CustomData = new Dictionary<string, DataValue>(),
                    AttributeMetadata = new AttributeMetadataRecord[0],
                },
            };

            ChannelMetadata(channelMetaData);

            var dataItems = new List<DataItem>
            {
                new DataItem()
                {
                    Indexes = new List<IndexValue> { new IndexValue { Item = 0L } },
                    ChannelId = 1,
                    Value = new DataValue { Item = 1 },
                    ValueAttributes = new DataAttribute[0],
                }
            };

            ChannelData(dataItems);
        }
    }
}

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
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v11.Datatypes;
using Energistics.Etp.v11.Datatypes.ChannelData;

namespace Energistics.Etp.v11.Protocol.ChannelStreaming
{
    public class ChannelStreamingProducer11MockHandler : ChannelStreamingProducerHandler
    {
        public ChannelStreamingProducer11MockHandler()
        {
            IsSimpleStreamer = true;
        }

        protected override void HandleStart(IMessageHeader header, Start start)
        {
            base.HandleStart(header, start);

            var channelMetaData = new List<ChannelMetadataRecord>()
            {
                new ChannelMetadataRecord()
                {
                    ChannelUri = "eml://witsml20/Channel(test)",
                    ChannelId = 1,
                    Indexes = new List<IndexMetadataRecord>()
                    {
                        new IndexMetadataRecord()
                        {
                            IndexType = ChannelIndexTypes.Depth,
                            Uom = "m",
                            DepthDatum = null,
                            Direction = IndexDirections.Increasing,
                            Mnemonic = "Depth",
                            Description = "Depth",
                            Uri = "eml://witsml20/Channel(depth)",
                            CustomData = new Dictionary<string, DataValue>(),
                            Scale = 1,
                            TimeDatum = null,
                        }
                    },
                    ChannelName = "test",
                    DataType = "double",
                    Uom = "m",
                    StartIndex = 1,
                    EndIndex = 2,
                    Description = "Test Channel",
                    Status = ChannelStatuses.Active,
                    ContentType = null,
                    Source = "ETP DevKit",
                    MeasureClass = "TestClass",
                    Uuid = "test",
                    CustomData = new Dictionary<string, DataValue>(),
                    DomainObject = null,
                },
            };

            ChannelMetadata(header, channelMetaData);

            var dataItems = new List<DataItem>
            {
                new DataItem()
                {
                    Indexes = new long[] { 0 },
                    ChannelId = 1,
                    Value = new DataValue { Item = 1 },
                    ValueAttributes = new DataAttribute[0],
                }
            };
            ChannelData(header, dataItems, MessageFlags.MultiPartAndFinalPart);
        }
    }
}

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

using System;
using Energistics.Etp.Common;
using System.Collections.Generic;
using System.Linq;
using Energistics.Etp.Common.Datatypes.ChannelData;
using System.Collections.Concurrent;

namespace Energistics.Etp.Handlers
{
    public class CustomerChannelHandler : Handler
    {
        private ConcurrentDictionary<long, IChannelMetadataRecord> ChannelMetadata = new ConcurrentDictionary<long, IChannelMetadataRecord>();

        protected override void InitializeRegistrarCore()
        {
            if (Registrar.IsEtpVersionSupported(EtpVersion.v11))
            {
                Registrar.Register(new v11.Protocol.ChannelStreaming.ChannelStreamingConsumerHandler());
            }
            if (Registrar.IsEtpVersionSupported(EtpVersion.v12))
            {
                Registrar.Register(new v12.Protocol.ChannelStreaming.ChannelStreamingConsumerHandler());
                Registrar.Register(new v12.Protocol.ChannelSubscribe.ChannelSubscribeCustomerHandler());
            }
        }

        protected override void InitializeSessionCore()
        {
            if (Session.EtpVersion == EtpVersion.v11)
            {
                Session.Handler<v11.Protocol.ChannelStreaming.IChannelStreamingConsumer>().OnSimpleStreamerChannelMetadata += OnSimpleStreamerChannelMetadata;
                Session.Handler<v11.Protocol.ChannelStreaming.IChannelStreamingConsumer>().OnBasicStreamerChannelMetadata += OnBasicStreamerChannelMetadata;
                Session.Handler<v11.Protocol.ChannelStreaming.IChannelStreamingConsumer>().OnStreamingChannelData += OnStreamingChannelData;
                Session.Handler<v11.Protocol.ChannelStreaming.IChannelStreamingConsumer>().OnChannelRemove += OnChannelRemove;
                Session.Handler<v11.Protocol.ChannelStreaming.IChannelStreamingConsumer>().OnChannelStatusChange += OnChannelStatusChange;
                Session.Handler<v11.Protocol.ChannelStreaming.IChannelStreamingConsumer>().OnChannelDataChange += OnChannelDataChange;
            }
            else if (Session.EtpVersion == EtpVersion.v12)
            {
                Session.Handler<v12.Protocol.ChannelStreaming.IChannelStreamingConsumer>().OnChannelMetadata += OnChannelMetadata;
                Session.Handler<v12.Protocol.ChannelStreaming.IChannelStreamingConsumer>().OnChannelData += OnChannelData;
                Session.Handler<v12.Protocol.ChannelSubscribe.IChannelSubscribeCustomer>().OnGetChannelMetadataResponse += OnGetChannelMetadataResponse;
                Session.Handler<v12.Protocol.ChannelSubscribe.IChannelSubscribeCustomer>().OnChannelData += OnChannelData;
            }
        }

        public override void PrintConsoleOptions()
        {
            Console.WriteLine(" N - Channels");
        }

        public override bool HandleConsoleInput(ConsoleKeyInfo info)
        {
            if (!IsKey(info, "N"))
                return false;

            var version = Session.EtpVersion;

            Console.WriteLine($" G - {(version == EtpVersion.v11 ? "ChannelStreaming - Start" : "ChannelStreaming - StartStreaming")}");
            if (version == EtpVersion.v12)
                Console.WriteLine($" P - ChannelStreaming - StopStreaming");
            Console.WriteLine($" M - {(version == EtpVersion.v11 ? "ChannelStreaming - ChannelDescribe" : "ChannelSubscribe - GetChannelMetadata")}");
            Console.WriteLine($" S - {(version == EtpVersion.v11 ? "ChannelStreaming - ChannelStreamingStart" : "ChannelSubscribe - SubscribeChannels")}");
            Console.WriteLine($" U - {(version == EtpVersion.v11 ? "ChannelStreaming - ChannelStreamingStop" : "ChannelSubscribe - UnsubscribeChannels")}");
            Console.WriteLine($" R - {(version == EtpVersion.v11 ? "ChannelStreaming - ChannelRangeRequest" : "ChannelSubscribe - GetRanges")}");
            Console.WriteLine();

            info = Console.ReadKey();

            Console.WriteLine(" - processing...");
            Console.WriteLine();

            if (IsKey(info, "G"))
            {
                if (version == EtpVersion.v11)
                {
                    Console.WriteLine("Sending Start...");
                    Session.Handler<v11.Protocol.ChannelStreaming.IChannelStreamingConsumer>().Start(maxMessageRate: 2000);
                }
                else if (version == EtpVersion.v12)
                {
                    Console.WriteLine("Sending StartStreaming...");
                    Session.Handler<v12.Protocol.ChannelStreaming.IChannelStreamingConsumer>().StartStreaming();
                }
            }
            else if (IsKey(info, "P"))
            {
                if (version == EtpVersion.v12)
                {
                    Console.WriteLine("Sending StopStreaming...");
                    Session.Handler<v12.Protocol.ChannelStreaming.IChannelStreamingConsumer>().StopStreaming();
                }
            }
            else if (IsKey(info, "M"))
            {
                Console.WriteLine("Enter channel metadata URI:");
                var uri = Console.ReadLine();
                Console.WriteLine();

                if (version == EtpVersion.v11)
                    Session.Handler<v11.Protocol.ChannelStreaming.IChannelStreamingConsumer>().ChannelDescribe(new List<string> { uri });
                else if (version == EtpVersion.v12)
                    Session.Handler<v12.Protocol.ChannelSubscribe.IChannelSubscribeCustomer>().GetChannelMetadata(new List<string> { uri });
            }
            else if (IsKey(info, "S"))
            {
                if (version == EtpVersion.v11)
                {
                    Console.WriteLine("Sending ChannelStreamingStart...");
                    Session.Handler<v11.Protocol.ChannelStreaming.IChannelStreamingConsumer>().ChannelStreamingStart(
                        ChannelMetadata.Values.Select(cmd => new v11.Datatypes.ChannelData.ChannelStreamingInfo
                        {
                            ChannelId = cmd.ChannelId,
                            ReceiveChangeNotification = false,
                            StartIndex = new v11.Datatypes.ChannelData.StreamingStartIndex { Item = null },
                        }).ToList()
                    );
                }
                else if (version == EtpVersion.v12)
                {
                    Console.WriteLine("Sending SubscribeChannels...");
                    Session.Handler<v12.Protocol.ChannelSubscribe.IChannelSubscribeCustomer>().SubscribeChannels(
                        ChannelMetadata.Values.Select(cmd => new v12.Datatypes.ChannelData.ChannelSubscribeInfo
                        {
                            ChannelId = cmd.ChannelId,
                            DataChanges = false,
                            RequestLatestIndexCount = null,
                            StartIndex = new v12.Datatypes.IndexValue { Item = null },
                        }).ToList()
                    );
                }
            }
            else if (IsKey(info, "U"))
            {
                if (version == EtpVersion.v11)
                {
                    Console.WriteLine("Sending ChannelStreamingStop...");
                    Session.Handler<v11.Protocol.ChannelStreaming.IChannelStreamingConsumer>().ChannelStreamingStop(
                        ChannelMetadata.Keys.ToList()
                    );
                }
                else if (version == EtpVersion.v12)
                {
                    Console.WriteLine("Sending UnsubscribeChannels...");
                    Session.Handler<v12.Protocol.ChannelSubscribe.IChannelSubscribeCustomer>().UnsubscribeChannels(
                        ChannelMetadata.Keys.ToList()
                    );
                }
            }
            else if (IsKey(info, "R"))
            {
                Console.WriteLine(" T - Time");
                Console.WriteLine(" D - Depth");
                Console.WriteLine();

                info = Console.ReadKey();

                Console.WriteLine(" - processing...");
                Console.WriteLine();
                IComparable startIndex = null;
                IComparable endIndex = null;
                var time = false;
                if (IsKey(info, "T"))
                {
                    Console.WriteLine("Enter start time in ISO 8601 format:");
                    var start = Console.ReadLine();
                    Console.WriteLine("Enter end time in ISO 8601 format:");
                    var end = Console.ReadLine();

                    DateTime startTime;
                    DateTime endTime;
                    if (!DateTime.TryParse(start, out startTime) || !DateTime.TryParse(end, out endTime))
                        return true;
                    startIndex = startTime;
                    endIndex = endTime;
                    time = true;
                }
                else if (IsKey(info, "D"))
                {
                    Console.WriteLine("Enter start depth:");
                    var start = Console.ReadLine();
                    Console.WriteLine("Enter end depth:");
                    var end = Console.ReadLine();

                    double startDepth;
                    double endDepth;
                    if (!double.TryParse(start, out startDepth) || !double.TryParse(end, out endDepth))
                        return true;
                    startIndex = startDepth;
                    endIndex = endDepth;
                }
                else
                {
                    return true;
                }

                Console.WriteLine("Enter channel IDs:");
                var channels = Console.ReadLine();
                var channelIds = channels.Split(',').Where(c => long.TryParse(c, out _)).Select(c => long.Parse(c)).Where(id => ChannelMetadata.ContainsKey(id)).ToList();
                if (channelIds.Count == 0)
                    return true;

                if (version == EtpVersion.v11)
                {
                    Console.WriteLine("Sending ChannelRangeRequest...");
                    Session.Handler<v11.Protocol.ChannelStreaming.IChannelStreamingConsumer>().ChannelRangeRequest(
                        new List<v11.Datatypes.ChannelData.ChannelRangeInfo>
                        {
                            new v11.Datatypes.ChannelData.ChannelRangeInfo
                            {
                                ChannelId = channelIds,
                                StartIndex = time ? ((DateTime)startIndex).ToEtpTimestamp() : (long)(((double)startIndex) / 1000.0),
                                EndIndex = time ? ((DateTime)endIndex).ToEtpTimestamp() : (long)(((double)endIndex) / 1000.0),
                                DepthDatum = string.Empty,
                                Uom = string.Empty,
                            }
                        });
                }
                else if (version == EtpVersion.v12)
                {
                    Console.WriteLine("Sending GetRanges...");
                    Session.Handler<v12.Protocol.ChannelSubscribe.IChannelSubscribeCustomer>().GetRanges(
                        new List<v12.Datatypes.ChannelData.ChannelRangeInfo>
                        {
                            new v12.Datatypes.ChannelData.ChannelRangeInfo
                            {
                                ChannelIds = channelIds,
                                Interval = new v12.Datatypes.Object.IndexInterval
                                {
                                    StartIndex = new v12.Datatypes.IndexValue { Item = time ? (object)((DateTime)startIndex).ToEtpTimestamp() : ((double)startIndex) },
                                    EndIndex = new v12.Datatypes.IndexValue { Item = time ? (object)((DateTime)endIndex).ToEtpTimestamp() : ((double)endIndex) },
                                    DepthDatum = string.Empty,
                                    Uom = string.Empty,
                                },
                            }
                        }, Guid.NewGuid());
                }
            }

            return true;
        }

        private void OnSimpleStreamerChannelMetadata(object sender, FireAndForgetEventArgs<v11.Protocol.ChannelStreaming.ChannelMetadata> args)
        {
            Console.WriteLine(string.Join(Environment.NewLine, args.Message.Body.Channels.Select(d => EtpExtensions.Serialize(d))));
        }

        private void OnBasicStreamerChannelMetadata(object sender, ResponseEventArgs<v11.Protocol.ChannelStreaming.ChannelDescribe, v11.Protocol.ChannelStreaming.ChannelMetadata> args)
        {
            foreach (var metadata in args.Response.Body.Channels)
                ChannelMetadata[metadata.ChannelId] = metadata;

            var domainObjects = new List<string>();
            foreach (var channel in args.Response.Body.Channels)
            {
                domainObjects.Add(channel.DomainObject.GetString());
                channel.DomainObject.SetString(string.Empty);
            }
            for (int i = 0; i < args.Response.Body.Channels.Count; i++)
            {
                Console.WriteLine(EtpExtensions.Serialize(args.Response.Body.Channels[i], true));
                Console.WriteLine(domainObjects[i]);
                args.Response.Body.Channels[i].DomainObject.SetString(domainObjects[i]);
            }
        }

        private void OnStreamingChannelData(object sender, FireAndForgetEventArgs<v11.Protocol.ChannelStreaming.ChannelData> args)
        {
            Console.WriteLine(EtpExtensions.Serialize(args.Message.Body.Data, true));
        }

        private void OnChannelRemove(object sender, FireAndForgetEventArgs<v11.Protocol.ChannelStreaming.ChannelRemove> args)
        {
            ChannelMetadata.TryRemove(args.Message.Body.ChannelId, out _);
            Console.WriteLine(EtpExtensions.Serialize(args.Message.Body, true));
        }

        private void OnChannelStatusChange(object sender, FireAndForgetEventArgs<v11.Protocol.ChannelStreaming.ChannelStatusChange> args)
        {
            Console.WriteLine(EtpExtensions.Serialize(args.Message.Body, true));
        }

        private void OnChannelDataChange(object sender, FireAndForgetEventArgs<v11.Protocol.ChannelStreaming.ChannelDataChange> args)
        {
            Console.WriteLine(EtpExtensions.Serialize(args.Message.Body, true));
        }

        private void OnChannelMetadata(object sender, FireAndForgetEventArgs<v12.Protocol.ChannelStreaming.ChannelMetadata> args)
        {
            Console.WriteLine(string.Join(Environment.NewLine, args.Message.Body.Channels.Select(d => EtpExtensions.Serialize(d, true))));
        }

        private void OnChannelData(object sender, FireAndForgetEventArgs<v12.Protocol.ChannelStreaming.ChannelData> args)
        {
            Console.WriteLine(EtpExtensions.Serialize(args.Message.Body.Data, true));
        }

        private void OnGetChannelMetadataResponse(object sender, ResponseEventArgs<v12.Protocol.ChannelSubscribe.GetChannelMetadata, v12.Protocol.ChannelSubscribe.GetChannelMetadataResponse> args)
        {
            if (args.Response == null)
                return;

            foreach (var metadata in args.Response.Body.Metadata.Values)
                ChannelMetadata[metadata.Id] = metadata;
            Console.WriteLine(string.Join(Environment.NewLine, args.Response.Body.Metadata.Select(d => EtpExtensions.Serialize(d, true))));
        }

        private void OnChannelData(object sender, FireAndForgetEventArgs<v12.Protocol.ChannelSubscribe.ChannelData> args)
        {
            Console.WriteLine(EtpExtensions.Serialize(args.Message.Body.Data, true));
        }
    }
}

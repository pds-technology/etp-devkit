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
using Energistics.Etp.Common.Datatypes.ChannelData;
using Energistics.Etp.Data;
using Energistics.Etp.Store;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Energistics.Etp.Handlers
{
    public class StoreChannelHandler : StoreHandlerBase
    {
        private BackgroundLoop ChannelUpdateLoop { get; } = new BackgroundLoop();

        private Random Random { get; } = new Random();
        private ConcurrentDictionary<Guid, Guid> SessionIds { get; } = new ConcurrentDictionary<Guid, Guid>();
        private double DepthIndex = 0.0;
        private DateTime TimeIndex = DateTime.UtcNow;
        private v11.Protocol.ChannelStreaming.ChannelStreamingProducerHandler v11ChannelStreamingHandler = new v11.Protocol.ChannelStreaming.ChannelStreamingProducerHandler();

        public StoreChannelHandler(TestDataStore store)
            : base(store)
        {
        }

        protected override void InitializeRegistrarCore()
        {
            // Register protocol handlers
            if (Registrar.IsEtpVersionSupported(EtpVersion.v11))
            {
                Registrar.Register(v11ChannelStreamingHandler);
                //Registrar.Register(new v11.Protocol.ChannelDataFrame.ChannelDataFrameProducerHandler());
            }
            if (Registrar.IsEtpVersionSupported(EtpVersion.v12))
            {
                Registrar.Register(new v12.Protocol.ChannelStreaming.ChannelStreamingProducerHandler());
                //Registrar.Register(new v12.Protocol.ChannelDataFrame.ChannelDataFrameStoreHandler());
                Registrar.Register(new v12.Protocol.ChannelSubscribe.ChannelSubscribeStoreHandler());
            }
        }

        protected override void InitializeSessionCore()
        {
            Session.SessionClosed += OnServerSessionClosed;
            if (Session.EtpVersion == EtpVersion.v11)
            {
                Session.Handler<v11.Protocol.ChannelStreaming.IChannelStreamingProducer>().OnStart += OnStart;
                Session.Handler<v11.Protocol.ChannelStreaming.IChannelStreamingProducer>().OnChannelDescribe += OnChannelDescribe;
                Session.Handler<v11.Protocol.ChannelStreaming.IChannelStreamingProducer>().OnChannelStreamingStart += OnChannelStreamingStart;
                Session.Handler<v11.Protocol.ChannelStreaming.IChannelStreamingProducer>().OnChannelStreamingStop += OnChannelStreamingStop;
                Session.Handler<v11.Protocol.ChannelStreaming.IChannelStreamingProducer>().OnChannelRangeRequest += OnChannelRangeRequest;
            }
            else if (Session.EtpVersion == EtpVersion.v12)
            {
                Session.Handler<v12.Protocol.ChannelStreaming.IChannelStreamingProducer>().OnStartStreaming += OnStartStreaming;
                Session.Handler<v12.Protocol.ChannelStreaming.IChannelStreamingProducer>().OnStopStreaming += OnStopStreaming;
                Session.Handler<v12.Protocol.ChannelSubscribe.IChannelSubscribeStore>().OnGetChannelMetadata += OnGetChannelMetadata;
                Session.Handler<v12.Protocol.ChannelSubscribe.IChannelSubscribeStore>().OnSubscribeChannels += OnSubscribeChannels;
                Session.Handler<v12.Protocol.ChannelSubscribe.IChannelSubscribeStore>().OnUnsubscribeChannels += OnUnsubscribeChannels;
                Session.Handler<v12.Protocol.ChannelSubscribe.IChannelSubscribeStore>().OnGetRanges += OnGetRanges;
            }
        }

        public override void PrintConsoleOptions()
        {
            Console.WriteLine(" D - Set store primary dataspace name");
            if (Registrar?.IsEtpVersionSupported(EtpVersion.v11) ?? false && Session?.IsSessionOpen == false)
            {
                if (v11ChannelStreamingHandler.Capabilities.SimpleStreamer ?? false)
                    Console.WriteLine(" B - Set ETP 1.1 channel streaming to simple streaming");
                else
                    Console.WriteLine(" B - Set ETP 1.1 channel streaming to basic streaming");
            }
            Console.WriteLine(" G - Start / stop background channel data generation");
        }

        public override bool HandleConsoleInput(ConsoleKeyInfo info)
        {
            if (IsKey(info, "B"))
            {
                if (v11ChannelStreamingHandler.Capabilities.SimpleStreamer ?? false)
                {
                    Console.WriteLine("Setting store to basic streamer for new ETP 1.1 sessions.");
                    SetBasicStreamer();
                }
                else
                {
                    Console.WriteLine("Setting store to simple streamer for new ETP 1.1 sessions.");
                    SetSimpleStreamer();
                }

                return true;
            }
            else if (IsKey(info, "G"))
            {
                if (ChannelUpdateLoop.IsStarted)
                {
                    Console.WriteLine("Stopping channel data updates.");
                    ChannelUpdateLoop.Stop();
                }
                else
                {
                    var now = DateTime.UtcNow;
                    TimeIndex = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Utc);
                    Console.WriteLine("Starting channel data updates.");
                    ChannelUpdateLoop.Start(UpdateChannelData, TimeSpan.FromSeconds(1));
                }
                return true;
            }

            return false;
        }

        private void SetSimpleStreamer()
        {
            if (Registrar?.IsEtpVersionSupported(EtpVersion.v11) ?? false && Session?.IsSessionOpen == false)
                v11ChannelStreamingHandler.Capabilities.SimpleStreamer = true;
        }

        private void SetBasicStreamer()
        {
            if (Registrar?.IsEtpVersionSupported(EtpVersion.v11) ?? false && Session?.IsSessionOpen == false)
                v11ChannelStreamingHandler.Capabilities.SimpleStreamer = false;
        }

        private void UpdateChannelData(CancellationToken token)
        {
            Store.ExecuteWithLock(() =>
            {
                RandomlyUpdateChannelData();
                Store.RefreshAll();
            });
        }

        private void RandomlyUpdateChannelData()
        {
            var rop = Random.NextDouble() * 3.0 + 15.0;
            var hkld = Random.NextDouble() * 20.0 + 15.0;
            var bdep = DepthIndex;
            var hdep = DepthIndex;

            Store.AppendTimeChannelData(Dataspace.TimeChannel01, TimeIndex, rop);
            if (Random.NextDouble() > 0.5)
                Store.AppendTimeChannelData(Dataspace.TimeChannel02, TimeIndex, hkld);
            Store.AppendTimeChannelData(Dataspace.TimeChannel03, TimeIndex, bdep);
            Store.AppendTimeChannelData(Dataspace.TimeChannel04, TimeIndex, hdep);

            if (Random.NextDouble() > 0.75)
                Store.AppendDepthChannelData(Dataspace.DepthChannel01, DepthIndex, rop);
            if (Random.NextDouble() > 0.9)
                Store.AppendDepthChannelData(Dataspace.DepthChannel02, DepthIndex, hkld);

            TimeIndex = TimeIndex.AddSeconds(1);
            DepthIndex = DepthIndex + 0.1;
        }

        private void OnServerSessionClosed(object sender, SessionClosedEventArgs args)
        {
            var server = (IEtpSession)sender;

            Store.ExecuteWithLock(() =>
            {
                foreach (var sessionId in SessionIds.Keys)
                    Store.StopChannelSubscription(sessionId);
            });
        }

        private void OnStart(object sender, VoidRequestEventArgs<v11.Protocol.ChannelStreaming.Start> args)
        {
            Store.ExecuteWithLock(() =>
            {
                OnStartCore(sender, args);
            });
        }

        private void OnStartCore(object sender, VoidRequestEventArgs<v11.Protocol.ChannelStreaming.Start> args)
        {
            var handler = (v11.Protocol.ChannelStreaming.IChannelStreamingProducer)sender;
            var sessionId = GetSessionId(handler);
            var callbacks = CreateChannelStreamingCallbacks(handler);
            if (Store.StartChannelSubscription(EtpVersion.v11, sessionId, args.Request.Body.MaxDataItems, TimeSpan.FromMilliseconds(args.Request.Body.MaxMessageRate), handler.Capabilities.SimpleStreamer ?? false, callbacks))
            {
                if (handler.Capabilities.SimpleStreamer ?? false)
                {
                    var describe = new v11.Protocol.ChannelStreaming.ChannelDescribe
                    {
                        Uris = new List<string> { EtpUri.RootUri11 },
                    };
                    foreach (var subscription in describe.GetSubscriptions(sessionId)) // Register global metadata subscriptions
                    {
                        var subscriptionInfo = new MockSubscriptionInfo(subscription);
                        if (Store.HasChannelSubscriptionScope(sessionId, subscriptionInfo))
                            continue;

                        IList<MockObject> addedChannels;
                        if (Store.AddChannelSubscriptionChannelScope(sessionId, subscriptionInfo, out addedChannels))
                        {
                            var channels = addedChannels
                                .Where(o => o is IMockGrowingObject)
                                .Select(o => ((IMockGrowingObject)o).ChannelMetadataRecord11(Store.GetChannelId(sessionId, o)))
                                .ToList();
                            handler.ChannelMetadata(subscription.RequestUuidGuid.UuidGuid, channels);
                        }
                    }
                }
            }
            else
            {
                args.FinalError = handler.ErrorInfo().RequestDenied();
            }
        }

        private void OnChannelDescribe(object sender, ListRequestEventArgs<v11.Protocol.ChannelStreaming.ChannelDescribe, v11.Datatypes.ChannelData.ChannelMetadataRecord> args)
        {
            Store.ExecuteWithLock(() =>
            {
                OnChannelDescribeCore(sender, args);
            });
        }

        private void OnChannelDescribeCore(object sender, ListRequestEventArgs<v11.Protocol.ChannelStreaming.ChannelDescribe, v11.Datatypes.ChannelData.ChannelMetadataRecord> args)
        {
            var handler = (v11.Protocol.ChannelStreaming.IChannelStreamingProducer)sender;
            var sessionId = GetSessionId(handler);
            if (handler.Capabilities.SimpleStreamer ?? false)
            {
                args.FinalError = handler.ErrorInfo().RequestDenied();
                return;
            }

            var allAddedChannels = new List<MockObject>();
            foreach (var subscription in args.Request.Body.GetSubscriptions(sessionId)) // Register metadata subscriptions
            {
                var subscriptionInfo = new MockSubscriptionInfo(subscription);
                if (Store.HasChannelSubscriptionScope(sessionId, subscriptionInfo))
                    continue;

                IList<MockObject> addedChannels;
                if (Store.AddChannelSubscriptionChannelScope(sessionId, subscriptionInfo, out addedChannels))
                    allAddedChannels.AddRange(addedChannels);
                else
                    args.ErrorMap[subscriptionInfo.RequestUuid.ToString()] = handler.ErrorInfo().RequestDenied($"URI: {subscription.Uri}");
            }
            args.Responses = allAddedChannels
                .Where(o => o is IMockGrowingObject)
                .Select(o => ((IMockGrowingObject)o).ChannelMetadataRecord11(Store.GetChannelId(sessionId, o)))
                .ToList();
        }

        private void OnChannelStreamingStart(object sender, VoidRequestEventArgs<v11.Protocol.ChannelStreaming.ChannelStreamingStart> args)
        {
            Store.ExecuteWithLock(() =>
            {
                OnChannelStreamingStartCore(sender, args);
            });
        }

        private void OnChannelStreamingStartCore(object sender, VoidRequestEventArgs<v11.Protocol.ChannelStreaming.ChannelStreamingStart> args)
        {
            var handler = (v11.Protocol.ChannelStreaming.IChannelStreamingProducer)sender;
            var sessionId = GetSessionId(handler);
            if (handler.Capabilities.SimpleStreamer ?? false)
            {
                args.FinalError = handler.ErrorInfo().RequestDenied();
                return;
            }
            HashSet<long> startedChannels, stoppedChannels, closedChannels, invalidChannels;
            if (!Store.ValidateChannelIds(sessionId, args.Request.Body.Channels.Select(si => si.ChannelId), out startedChannels, out stoppedChannels, out closedChannels, out invalidChannels))
            {
                args.FinalError = handler.ErrorInfo().RequestDenied();
                return;
            }
            foreach (var streamingInfo in args.Request.Body.Channels)
            {
                var channelId = streamingInfo.ChannelId;
                if (invalidChannels.Contains(channelId))
                {
                        args.ErrorMap[channelId.ToString()] = handler.ErrorInfo().InvalidChannelId(channelId);
                        continue;
                }
                if (closedChannels.Contains(channelId)) // Will send ChannelRemove
                    continue;

                var channel = Store.GetChannel(sessionId, channelId);
                if (channel == null || !(channel is IMockGrowingObject))
                {
                    args.ErrorMap[channelId.ToString()] = handler.ErrorInfo().NotFound($"Missing channel for Channel ID {channelId}");
                    continue;
                }
                var growingObject = channel as IMockGrowingObject;
                if (startedChannels.Contains(channelId))
                {
                    args.ErrorMap[channelId.ToString()] = handler.ErrorInfo().NotFound($"Channel {channelId} ({growingObject.Mnemonic}) is already streaming.");
                    continue;
                }

                if (!Store.StartChannelStreaming(sessionId, channelId, streamingInfo.ReceiveChangeNotification, new MockIndex(streamingInfo.StartIndex)))
                    args.ErrorMap[channelId.ToString()] = handler.ErrorInfo().RequestDenied($"Could not start streaming for Channel {streamingInfo.ChannelId} ({growingObject.Mnemonic}).");
            }
            // Send removed channel IDs
            foreach (var channelId in closedChannels)
                handler.ChannelRemove(channelId);
        }

        private void OnChannelStreamingStop(object sender, VoidRequestEventArgs<v11.Protocol.ChannelStreaming.ChannelStreamingStop> args)
        {
            Store.ExecuteWithLock(() =>
            {
                OnChannelStreamingStopCore(sender, args);
            });
        }

        private void OnChannelStreamingStopCore(object sender, VoidRequestEventArgs<v11.Protocol.ChannelStreaming.ChannelStreamingStop> args)
        {
            var handler = (v11.Protocol.ChannelStreaming.IChannelStreamingProducer)sender;
            var sessionId = GetSessionId(handler);
            if (handler.Capabilities.SimpleStreamer ?? false)
            {
                args.FinalError = handler.ErrorInfo().RequestDenied();
                return;
            }
            HashSet<long> startedChannels, stoppedChannels, closedChannels, invalidChannels;
            if (!Store.ValidateChannelIds(sessionId, args.Request.Body.Channels, out startedChannels, out stoppedChannels, out closedChannels, out invalidChannels))
            {
                args.FinalError = handler.ErrorInfo().RequestDenied();
                return;
            }
            foreach (var invalidId in invalidChannels.Concat(closedChannels).Concat(stoppedChannels))
                args.ErrorMap[invalidId.ToString()] = handler.ErrorInfo().InvalidState($"Channel {invalidId} is not streaming.");

            foreach (var channelId in startedChannels)
            {
                if (!Store.StopChannelStreaming(sessionId, channelId))
                    args.ErrorMap[channelId.ToString()] = handler.ErrorInfo().RequestDenied($"Could not stop streaming for Channel {channelId}.");
            }
        }

        private void OnChannelRangeRequest(object sender, ListRequestEventArgs<v11.Protocol.ChannelStreaming.ChannelRangeRequest, v11.Datatypes.ChannelData.DataItem> args)
        {
            Store.ExecuteWithLock(() =>
            {
                OnChannelRangeRequestCore(sender, args);
            });
        }

        private void OnChannelRangeRequestCore(object sender, ListRequestEventArgs<v11.Protocol.ChannelStreaming.ChannelRangeRequest, v11.Datatypes.ChannelData.DataItem> args)
        {
            var handler = (v11.Protocol.ChannelStreaming.IChannelStreamingProducer)sender;
            var sessionId = GetSessionId(handler);
            if (handler.Capabilities.SimpleStreamer ?? false)
            {
                args.FinalError = handler.ErrorInfo().RequestDenied();
                return;
            }
            HashSet<long> startedChannels, stoppedChannels, closedChannels, invalidChannels;
            if (!Store.ValidateChannelIds(sessionId, args.Request.Body.ChannelRanges.SelectMany(cr => cr.ChannelId), out startedChannels, out stoppedChannels, out closedChannels, out invalidChannels))
            {
                args.FinalError = handler.ErrorInfo().RequestDenied();
                return;
            }
            foreach (var invalidId in invalidChannels.Concat(closedChannels))
                args.ErrorMap[invalidId.ToString()] = handler.ErrorInfo().InvalidState($"Channel {invalidId} is not available.");
            if (args.ErrorMap.Count > 0)
                return;

            var queries = args.Request.Body.ChannelRanges.SelectMany(cr => cr.ChannelId.Select(id => new MockRangeQuery(cr, id)));
            args.Responses = Store.GetChannelDataRanges(sessionId, queries)
                .Select(di => di.DataItem11).ToList();
        }

        private void OnStartStreaming(object sender, VoidRequestEventArgs<v12.Protocol.ChannelStreaming.StartStreaming> args)
        {
            Store.ExecuteWithLock(() =>
            {
                OnStartStreamingCore(sender, args);
            });
        }

        private void OnStartStreamingCore(object sender, VoidRequestEventArgs<v12.Protocol.ChannelStreaming.StartStreaming> args)
        {
            var handler = (v12.Protocol.ChannelStreaming.IChannelStreamingProducer)sender;
            var sessionId = GetSessionId(handler);
            var callbacks = CreateChannelStreamingCallbacks(handler);
            if (Store.StartChannelSubscription(EtpVersion.v11, sessionId, int.MaxValue, TimeSpan.FromMilliseconds(0), true, callbacks))
            {
                var describe = new v11.Protocol.ChannelStreaming.ChannelDescribe
                {
                    Uris = new List<string> { EtpUri.RootUri11 },
                };
                foreach (var subscription in describe.GetSubscriptions(sessionId)) // Register global metadata subscriptions
                {
                    var subscriptionInfo = new MockSubscriptionInfo(subscription);
                    if (Store.HasChannelSubscriptionScope(sessionId, subscriptionInfo))
                        continue;

                    IList<MockObject> addedChannels;
                    if (Store.AddChannelSubscriptionChannelScope(sessionId, subscriptionInfo, out addedChannels))
                    {
                        var channels = addedChannels
                            .Where(o => o is MockChannel)
                            .Select(o => ((MockChannel)o).ChannelMetadataRecord12(Store.GetChannelId(sessionId, o)))
                            .ToList();
                        handler.ChannelMetadata(channels);
                    }
                }
            }
            else
            {
                args.FinalError = handler.ErrorInfo().RequestDenied();
            }
        }

        private void OnStopStreaming(object sender, VoidRequestEventArgs<v12.Protocol.ChannelStreaming.StopStreaming> args)
        {
            Store.ExecuteWithLock(() =>
            {
                OnStopStreamingCore(sender, args);
            });
        }

        private void OnStopStreamingCore(object sender, VoidRequestEventArgs<v12.Protocol.ChannelStreaming.StopStreaming> args)
        {
            var handler = (v12.Protocol.ChannelStreaming.IChannelStreamingProducer)sender;
            var sessionId = GetSessionId(handler);
            Store.StopChannelSubscription(sessionId);
        }

        private void OnGetChannelMetadata(object sender, MapRequestEventArgs<v12.Protocol.ChannelSubscribe.GetChannelMetadata, v12.Datatypes.ChannelData.ChannelMetadataRecord> args)
        {
            Store.ExecuteWithLock(() =>
            {
                OnGetChannelMetadataCore(sender, args);
            });
        }

        private void OnGetChannelMetadataCore(object sender, MapRequestEventArgs<v12.Protocol.ChannelSubscribe.GetChannelMetadata, v12.Datatypes.ChannelData.ChannelMetadataRecord> args)
        {
            var handler = (v12.Protocol.ChannelSubscribe.IChannelSubscribeStore)sender;
            var sessionId = GetSessionId(handler);

            if (!Store.HasChannelSubscription(sessionId))
            {
                var callbacks = CreateChannelStreamingCallbacks(handler);
                if (!Store.StartChannelSubscription(EtpVersion.v12, sessionId, int.MaxValue, TimeSpan.FromSeconds(0), false, callbacks))
                {
                    args.FinalError = handler.ErrorInfo().RequestDenied();
                }
            }

            foreach (var kvp in args.Request.Body.Uris) // Register metadata subscriptions
            {
                var subscriptionInfo = new MockSubscriptionInfo(kvp.Value, handler.Session.SessionId); // Use the actual sesison ID as a namespace.
                if (Store.HasChannelSubscriptionScope(sessionId, subscriptionInfo))
                    continue;

                IList<MockObject> addedChannels;
                if (Store.AddChannelSubscriptionChannelScope(sessionId, subscriptionInfo, out addedChannels) && addedChannels.Count == 1)
                    args.ResponseMap[kvp.Key] = ((MockChannel)addedChannels[0]).ChannelMetadataRecord12(Store.GetChannelId(sessionId, addedChannels[0]));
                else
                    args.ErrorMap[subscriptionInfo.RequestUuid.ToString()] = handler.ErrorInfo().RequestDenied($"URI: {kvp.Value}");
            }
        }

        private void OnSubscribeChannels(object sender, MapRequestEventArgs<v12.Protocol.ChannelSubscribe.SubscribeChannels, string> args)
        {
            Store.ExecuteWithLock(() =>
            {
                OnSubscribeChannelsCore(sender, args);
            });
        }

        private void OnSubscribeChannelsCore(object sender, MapRequestEventArgs<v12.Protocol.ChannelSubscribe.SubscribeChannels, string> args)
        {
            var handler = (v12.Protocol.ChannelSubscribe.IChannelSubscribeStore)sender;
            var sessionId = GetSessionId(handler);

            HashSet<long> startedChannels, stoppedChannels, closedChannels, invalidChannels;
            if (!Store.ValidateChannelIds(sessionId, args.Request.Body.Channels.Values.Select(csi => csi.ChannelId), out startedChannels, out stoppedChannels, out closedChannels, out invalidChannels))
            {
                args.FinalError = handler.ErrorInfo().RequestDenied();
                return;
            }
            foreach (var kvp in args.Request.Body.Channels)
            {
                var streamingInfo = kvp.Value;
                var channelId = streamingInfo.ChannelId;
                if (invalidChannels.Contains(channelId) || closedChannels.Contains(channelId))
                {
                    args.ErrorMap[kvp.Key] = handler.ErrorInfo().InvalidChannelId(channelId);
                    continue;
                }

                var channel = Store.GetChannel(sessionId, channelId);
                if (channel == null || !(channel is IMockGrowingObject))
                {
                    args.ErrorMap[kvp.Key] = handler.ErrorInfo().NotFound($"Missing channel for Channel ID {channelId}");
                    continue;
                }
                var growingObject = channel as IMockGrowingObject;
                if (startedChannels.Contains(channelId))
                {
                    args.ErrorMap[kvp.Key] = handler.ErrorInfo().NotFound($"Channel {channelId} ({growingObject.Mnemonic}) is already streaming.");
                    continue;
                }

                if (Store.StartChannelStreaming(sessionId, channelId, streamingInfo.DataChanges, new MockIndex(streamingInfo)))
                    args.ResponseMap[kvp.Key] = string.Empty;
                else
                    args.ErrorMap[kvp.Key] = handler.ErrorInfo().RequestDenied($"Could not start streaming for Channel {streamingInfo.ChannelId} ({growingObject.Mnemonic}).");
            }
        }

        private void OnUnsubscribeChannels(object sender, MapRequestEventArgs<v12.Protocol.ChannelSubscribe.UnsubscribeChannels, long> args)
        {
            Store.ExecuteWithLock(() =>
            {
                OnUnsubscribeChannelsCore(sender, args);
            });
        }

        private void OnUnsubscribeChannelsCore(object sender, MapRequestEventArgs<v12.Protocol.ChannelSubscribe.UnsubscribeChannels, long> args)
        {
            var handler = (v12.Protocol.ChannelSubscribe.IChannelSubscribeStore)sender;
            var sessionId = GetSessionId(handler);

            HashSet<long> startedChannels, stoppedChannels, closedChannels, invalidChannels;
            if (!Store.ValidateChannelIds(sessionId, args.Request.Body.ChannelIds.Values, out startedChannels, out stoppedChannels, out closedChannels, out invalidChannels))
            {
                args.FinalError = handler.ErrorInfo().RequestDenied();
                return;
            }
            foreach (var kvp in args.Request.Body.ChannelIds)
            {
                var channelId = kvp.Value;
                if (invalidChannels.Contains(channelId) || closedChannels.Contains(channelId))
                {
                    args.ErrorMap[kvp.Key] = handler.ErrorInfo().InvalidState($"Channel {channelId} is not streaming.");
                    continue;
                }
                if (Store.StopChannelStreaming(sessionId, channelId))
                    args.ResponseMap[kvp.Key] = channelId;
                else
                    args.ErrorMap[kvp.Key] = handler.ErrorInfo().RequestDenied($"Could not stop streaming for Channel {channelId}.");
            }
        }

        private void OnGetRanges(object sender, ListRequestEventArgs<v12.Protocol.ChannelSubscribe.GetRanges, v12.Datatypes.ChannelData.DataItem> args)
        {
            Store.ExecuteWithLock(() =>
            {
                OnGetRangesCore(sender, args);
            });
        }

        private void OnGetRangesCore(object sender, ListRequestEventArgs<v12.Protocol.ChannelSubscribe.GetRanges, v12.Datatypes.ChannelData.DataItem> args)
        {
            var handler = (v12.Protocol.ChannelSubscribe.IChannelSubscribeStore)sender;
            var sessionId = GetSessionId(handler);

            HashSet<long> startedChannels, stoppedChannels, closedChannels, invalidChannels;
            if (!Store.ValidateChannelIds(sessionId, args.Request.Body.ChannelRanges.SelectMany(cr => cr.ChannelIds), out startedChannels, out stoppedChannels, out closedChannels, out invalidChannels))
            {
                args.FinalError = handler.ErrorInfo().RequestDenied();
                return;
            }
            if (invalidChannels.Count > 0)
            {
                args.FinalError = handler.ErrorInfo().RequestDenied($"Channel {invalidChannels.First()} is not available.");
                return;
            }
            if (closedChannels.Count > 0)
            {
                args.FinalError = handler.ErrorInfo().RequestDenied($"Channel {closedChannels.First()} is not available.");
                return;
            }

            foreach (var invalidId in invalidChannels.Concat(closedChannels))
                args.ErrorMap[invalidId.ToString()] = handler.ErrorInfo().InvalidState($"Channel {invalidId} is not available.");
            if (args.ErrorMap.Count > 0)
                return;

            var queries = args.Request.Body.ChannelRanges.SelectMany(cr => cr.ChannelIds.Select(id => new MockRangeQuery(cr, id)));
            args.Responses = Store.GetChannelDataRanges(sessionId, queries)
                .Select(di => di.DataItem12).ToList();
        }

        private MockGrowingObjectCallbacks CreateChannelStreamingCallbacks(v11.Protocol.ChannelStreaming.IChannelStreamingProducer handler)
        {
            var sessionId = GetSessionId(handler);
            return new MockGrowingObjectCallbacks
            {
                Created = (subscriptionUuid, @object, includeData) =>
                {
                    var growingObject = @object as IMockGrowingObject;
                    handler.ChannelMetadata(subscriptionUuid, new List<v11.Datatypes.ChannelData.ChannelMetadataRecord> { growingObject.ChannelMetadataRecord11(Store.GetChannelId(sessionId, @object)) });
                },
                Updated = null,
                Joined = (subscriptionUuid, @object, includeData) =>
                {
                    var growingObject = @object as IMockGrowingObject;
                    handler.ChannelMetadata(subscriptionUuid, new List<v11.Datatypes.ChannelData.ChannelMetadataRecord> { growingObject.ChannelMetadataRecord11(Store.GetChannelId(sessionId, @object)) });
                },
                Unjoined = (subscriptionUuid, @object, includeData) =>
                {
                    handler.ChannelRemove(Store.GetChannelId(sessionId, @object));
                },
                ActiveStatusChanged = (subscriptionUuid, @object, isActive) =>
                {
                    handler.ChannelStatusChange(Store.GetChannelId(sessionId, @object), isActive ? v11.Datatypes.ChannelData.ChannelStatuses.Active : v11.Datatypes.ChannelData.ChannelStatuses.Inactive);
                },
                Deleted = (subscriptionUuid, @object) =>
                {
                    handler.ChannelRemove(Store.GetChannelId(sessionId, @object));
                },
                SubscriptionEnded = null,
                DataAppended = (subscriptionUuid, dataItems) =>
                {
                    handler.StreamingChannelData(dataItems.Select(d => d.DataItem11).ToList());
                }
            };
        }

        private MockGrowingObjectCallbacks CreateChannelStreamingCallbacks(v12.Protocol.ChannelStreaming.IChannelStreamingProducer handler)
        {
            var sessionId = GetSessionId(handler);
            return new MockGrowingObjectCallbacks
            {
                Created = (subscriptionUuid, @object, includeData) =>
                {
                    var channel = @object as MockChannel;
                    handler.ChannelMetadata(new List<v12.Datatypes.ChannelData.ChannelMetadataRecord> { channel.ChannelMetadataRecord12(Store.GetChannelId(sessionId, @object)) });
                },
                Updated = null,
                Joined = (subscriptionUuid, @object, includeData) =>
                {
                    var channel = @object as MockChannel;
                    handler.ChannelMetadata(new List<v12.Datatypes.ChannelData.ChannelMetadataRecord> { channel.ChannelMetadataRecord12(Store.GetChannelId(sessionId, @object)) });
                },
                Unjoined = null,
                ActiveStatusChanged = null,
                Deleted = null,
                SubscriptionEnded = null,
                DataAppended = (subscriptionUuid, dataItems) =>
                {
                    handler.ChannelData(dataItems.Select(d => d.DataItem12).ToList());
                }
            };
        }

        private MockGrowingObjectCallbacks CreateChannelStreamingCallbacks(v12.Protocol.ChannelSubscribe.IChannelSubscribeStore handler)
        {
            var sessionId = GetSessionId(handler);
            return new MockGrowingObjectCallbacks
            {
                Created = null,
                Updated = null,
                Joined = null,
                Unjoined = (subscriptionUuid, @object, includeData) =>
                {
                    handler.NotificationSubscriptionsStopped(new List<long> { Store.GetChannelId(sessionId, @object) });
                },
                ActiveStatusChanged = null,
                Deleted = (subscriptionUuid, @object) =>
                {
                    handler.NotificationSubscriptionsStopped(new List<long> { Store.GetChannelId(sessionId, @object) });
                },
                SubscriptionEnded = null,
                DataAppended = (subscriptionUuid, dataItems) =>
                {
                    handler.ChannelData(dataItems.Select(d => d.DataItem12).ToList());
                }
            };
        }

        private Guid GetSessionId(IProtocolHandler handler)
        {
            var guid = handler.EtpVersion == EtpVersion.v11
                ? GuidUtility.Create(handler.Session.SessionId, v11.ProtocolNames.GetProtocolName(handler.Protocol))
                : GuidUtility.Create(handler.Session.SessionId, v12.ProtocolNames.GetProtocolName(handler.Protocol));

            SessionIds[guid] = guid;
            return guid;
        }

    }
}

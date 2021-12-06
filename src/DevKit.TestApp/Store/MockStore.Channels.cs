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
using Energistics.Etp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Energistics.Etp.Store
{
    public partial class MockStore
    {
        private Dictionary<Guid, ChannelSubscription> ChannelSubscriptionsBySessionId { get; } = new Dictionary<Guid, ChannelSubscription>();

        private BackgroundLoop ChannelNotificationLoop { get; } = new BackgroundLoop();

        private void StartChannelNotificationLoop()
        {
            ChannelNotificationLoop.Start(SendChannelNotifications, TimeSpan.FromSeconds(0.1));
        }

        private void RefreshChannelSubscriptions()
        {
            foreach (var subscription in ChannelSubscriptionsBySessionId.Values)
            {
                RefreshChannelSubscription(subscription);
            }
        }

        private void RefreshChannelSubscription(ChannelSubscription subscription)
        {
            var allObjects = new Dictionary<Guid, MockObject>();
            subscription.ChannelScopeUuidByChannelUuid.Clear();
            subscription.ValidChannelIds.Clear();
            foreach (var subscriptionInfo in subscription.ChannelScopesByChannelScopeUuid.Values)
            {
                foreach (var @object in GetObjects(subscription.Version, subscriptionInfo.Context))
                {
                    allObjects[@object.Uuid] = @object;
                    if (!subscription.ChannelScopeUuidByChannelUuid.ContainsKey(@object.Uuid))
                    {
                        subscription.ChannelScopeUuidByChannelUuid[@object.Uuid] = subscriptionInfo.RequestUuid;
                        ChannelSubscriptionInfo info;
                        if (!subscription.ChannelSubscriptionsByChannelUuid.TryGetValue(@object.Uuid, out info))
                        {
                            info = new ChannelSubscriptionInfo
                            {
                                Channel = @object,
                                ChannelId = subscription.NextChannelId++,
                                ChannelScopeUuid = subscriptionInfo.RequestUuid,
                                IsStarted = false,
                                SendChanges = false,
                            };
                            if (subscription.AutoStart)
                            {
                                StartChannelSubscriptionInfo(info, null, false);
                            }
                            subscription.ChannelSubscriptionsByChannelUuid[@object.Uuid] = info;
                        }
                        subscription.ValidChannelIds[info.ChannelId] = info;
                    }
                }
            }

            RefreshSubscriptionObjects(subscription, allObjects.Values);
        }

        public bool HasChannelSubscription(Guid sessionId)
        {
            CheckLocked();

            return ChannelSubscriptionsBySessionId.ContainsKey(sessionId);
        }

        public bool StartChannelSubscription(EtpVersion version, Guid sessionId, int maxDataItems, TimeSpan maxMessageRate, bool autoStart, MockGrowingObjectCallbacks callbacks)
        {
            CheckLocked();

            if (ChannelSubscriptionsBySessionId.ContainsKey(sessionId))
                return false;

            ChannelSubscriptionsBySessionId[sessionId] = new ChannelSubscription
            {
                Version = version,
                Uuid = sessionId,
                LastNotificationTime = StoreLastWrite,
                AutoStart = autoStart,
                MaxDataItems = maxDataItems,
                MaxMessageRate = maxMessageRate,
                Callbacks = callbacks,
            };

            return true;
        }

        public bool HasChannelSubscriptionScope(Guid sessionId, MockSubscriptionInfo channelScope)
        {
            CheckLocked();

            ChannelSubscription channelSubscription;
            if (!ChannelSubscriptionsBySessionId.TryGetValue(sessionId, out channelSubscription))
                return false;

            return channelSubscription.ChannelScopesByChannelScopeUuid.ContainsKey(channelScope.RequestUuid);
        }

        public bool AddChannelSubscriptionChannelScope(Guid sessionId, MockSubscriptionInfo channelScope, out IList<MockObject> addedChannels)
        {
            addedChannels = new List<MockObject>();

            CheckLocked();

            ChannelSubscription channelSubscription;
            if (!ChannelSubscriptionsBySessionId.TryGetValue(sessionId, out channelSubscription))
                return false;

            var channelScopeUuid = channelScope.RequestUuid;
            if (channelSubscription.ChannelScopesByChannelScopeUuid.ContainsKey(channelScopeUuid))
                return false;

            channelSubscription.ChannelScopesByChannelScopeUuid[channelScopeUuid] = channelScope;

            RefreshChannelSubscription(channelSubscription);

            foreach (var channel in channelSubscription.Objects.Values)
                addedChannels.Add(channel);

            return true;
        }

        public bool RemoveChannelSubscriptionChannelScope(Guid sessionId, IRequestUuidSource request)
        {
            CheckLocked();

            ChannelSubscription channelSubscription;
            if (!ChannelSubscriptionsBySessionId.TryGetValue(sessionId, out channelSubscription))
                return false;

            MockSubscriptionInfo subscriptionInfo;
            if (!channelSubscription.ChannelScopesByChannelScopeUuid.TryGetValue(request.RequestUuid, out subscriptionInfo))
                return false;

            if (!channelSubscription.ChannelScopesByChannelScopeUuid.Remove(request.RequestUuid))
                return false;

            return UnsubscribeObjectNotifications(subscriptionInfo.RequestUuid);
        }

        public bool StopChannelSubscription(Guid sessionId)
        {
            CheckLocked();

            ChannelSubscription channelSubscription;
            if (!ChannelSubscriptionsBySessionId.TryGetValue(sessionId, out channelSubscription))
                return false;

            var success = true;
            foreach (var subscriptionInfo in channelSubscription.ChannelScopesByChannelScopeUuid.Values)
            {
                if (!UnsubscribeObjectNotifications(subscriptionInfo.RequestUuid))
                    success = false;
            }

            if (!ChannelSubscriptionsBySessionId.Remove(sessionId))
                success = false;

            return success;
        }

        public long GetChannelId(Guid sessionId, MockObject @object)
        {
            CheckLocked();

            ChannelSubscription channelSubscription;
            if (!ChannelSubscriptionsBySessionId.TryGetValue(sessionId, out channelSubscription))
                return -1;

            ChannelSubscriptionInfo info;
            if (!channelSubscription.ChannelSubscriptionsByChannelUuid.TryGetValue(@object.Uuid, out info))
                return -1;

            return info.ChannelId;
        }

        public MockObject GetChannel(Guid sessionId, long channelId)
        {
            CheckLocked();

            ChannelSubscription channelSubscription;
            if (!ChannelSubscriptionsBySessionId.TryGetValue(sessionId, out channelSubscription))
                return null;

            ChannelSubscriptionInfo info;
            if (!channelSubscription.ValidChannelIds.TryGetValue(channelId, out info))
                return null;

            return info.Channel;
        }

        public bool IsChannelStreaming(Guid sessionId, long channelId)
        {
            CheckLocked();

            ChannelSubscription channelSubscription;
            if (!ChannelSubscriptionsBySessionId.TryGetValue(sessionId, out channelSubscription))
                return false;

            ChannelSubscriptionInfo info;
            if (!channelSubscription.ValidChannelIds.TryGetValue(channelId, out info))
                return false;

            return info.IsStarted;
        }

        public bool ValidateChannelIds(Guid sessionId, IEnumerable<long> channelIds, out HashSet<long> startedchannels, out HashSet<long> stoppedChannels, out HashSet<long> closedChannels, out HashSet<long> invalidChannels)
        {
            CheckLocked();

            startedchannels = new HashSet<long>();
            stoppedChannels = new HashSet<long>();
            closedChannels = new HashSet<long>();
            invalidChannels = new HashSet<long>();

            ChannelSubscription channelSubscription;
            if (!ChannelSubscriptionsBySessionId.TryGetValue(sessionId, out channelSubscription))
                return false;

            foreach (var channelId in channelIds)
            {
                ChannelSubscriptionInfo info;
                if (channelSubscription.ValidChannelIds.TryGetValue(channelId, out info))
                {
                    if (info.IsStarted)
                        startedchannels.Add(channelId);
                    else
                        stoppedChannels.Add(channelId);
                }
                else
                {
                    if (channelId >= channelSubscription.NextChannelId)
                        invalidChannels.Add(channelId);
                    else
                        closedChannels.Add(channelId);
                }
            }

            return true;
        }

        public bool StartChannelStreaming(Guid sessionId, long channelId, bool sendChanges, MockIndex startIndex)
        {
            CheckLocked();

            ChannelSubscription channelSubscription;
            if (!ChannelSubscriptionsBySessionId.TryGetValue(sessionId, out channelSubscription))
                return false;

            ChannelSubscriptionInfo info;
            if (!channelSubscription.ValidChannelIds.TryGetValue(channelId, out info))
                return false;

            var channel = GetChannel(sessionId, channelId) as IMockGrowingObject;
            if (channel == null)
                return false;

            StartChannelSubscriptionInfo(info, startIndex, sendChanges);

            return true;
        }

        private void StartChannelSubscriptionInfo(ChannelSubscriptionInfo info, MockIndex startIndex, bool sendChanges)
        {
            info.IsStarted = true;
            info.Query = new ChannelDataQuery
            {
                Channel = info.Channel as IMockGrowingObject,
                ChannelId = info.ChannelId,
                IsStartIndexInclusive = true,
                EndIndex = null,
            };
            info.SendChanges = sendChanges;

            info.Query.StartIndex = startIndex == null
                ? info.Query.Channel.EndIndex
                : info.Query.Channel.GetIndex(startIndex);
        }

        public bool StopChannelStreaming(Guid sessionId, long channelId)
        {
            CheckLocked();

            ChannelSubscription channelSubscription;
            if (!ChannelSubscriptionsBySessionId.TryGetValue(sessionId, out channelSubscription))
                return false;

            ChannelSubscriptionInfo info;
            if (!channelSubscription.ValidChannelIds.TryGetValue(channelId, out info))
                return false;

            info.IsStarted = false;

            return true;
        }

        public IList<MockDataItem> GetChannelDataRanges(Guid sessionId, IEnumerable<MockRangeQuery> rangeQueries, int maxDataItems = int.MaxValue)
        {
            CheckLocked();

            ChannelSubscription channelSubscription;
            if (!ChannelSubscriptionsBySessionId.TryGetValue(sessionId, out channelSubscription))
                return new List<MockDataItem>();

            var queries = new List<ChannelDataQuery>();

            foreach (var rangeQuery in rangeQueries)
            {
                var channel = GetChannel(sessionId, rangeQuery.ChannelId) as IMockGrowingObject;
                if (channel == null)
                    continue;

                queries.Add(new ChannelDataQuery
                {
                    Channel = channel,
                    ChannelId = rangeQuery.ChannelId,
                    StartIndex = channel.GetIndex(rangeQuery.StartIndex),
                    IsStartIndexInclusive = true,
                    EndIndex = channel.GetIndex(rangeQuery.EndIndex),
                });
            }

            return GetChannelData(channelSubscription.Version, queries, maxDataItems);
        }

        private void SendChannelNotifications(CancellationToken token)
        {
            ExecuteWithLock(() =>
            {
                SendAllChannelNotifications();
            });
        }

        private void SendAllChannelNotifications()
        {
            CheckLocked();

            foreach (var subscription in ChannelSubscriptionsBySessionId.Values.ToList())
            {
                SendChannelSubscriptionNotifications(subscription);
            }
        }

        private void SendChannelSubscriptionNotifications(ChannelSubscription subscription)
        {
            CheckLocked();

            RefreshSubscriptions();

            SendJoinedSubscriptionNotifications(subscription);
            SendCreatedNotifications(subscription);
            SendUpdatedNotifications(subscription);
            SendActivatedNotifications(subscription);

            SendChannelDataNotifications(subscription);

            SendDeactivatedNotifications(subscription);
            SendUnjoinedSubscriptionNotifications(subscription);

            foreach (var @object in subscription.GetAllRemovedObjects())
            {
                subscription.ChannelScopeUuidByChannelUuid.Remove(@object.Uuid);
                subscription.ChannelSubscriptionsByChannelUuid.Remove(@object.Uuid);
                subscription.ChannelScopeUuidByChannelUuid.Remove(@object.Uuid);
            }

            subscription.PreviousObjects = subscription.Objects;
            subscription.Objects = new Dictionary<Guid, MockObject>();

            subscription.LastNotificationTime = StoreLastWrite;
        }

        private void SendChannelDataNotifications(ChannelSubscription subscription)
        {
            if (StoreLastWrite - subscription.LastSendTime < subscription.MaxMessageRate)
                return;

            var streamingInfos = subscription.ValidChannelIds.Values.Where(s => s.IsStarted && !s.Channel.IsDeleted && s.Channel is IMockGrowingObject);

            var dataItems = GetChannelData(subscription.Version, streamingInfos, subscription.MaxDataItems);

            if (dataItems.Count > 0)
            {
                subscription.Callbacks.DataAppended(subscription.Uuid, dataItems);
                subscription.LastSendTime = StoreLastWrite;
            }
        }

        private static IList<int> GetStartingDataIndexes(IList<ChannelDataQuery> queries, IComparable minIndex)
        {
            var dataIndexes = new List<int>(queries.Count);
            for (int i = 0; i < queries.Count; i++)
            {
                dataIndexes.Add(queries[i].Channel.GetStartDataIndex(queries[i].StartIndex ?? minIndex, queries[i].IsStartIndexInclusive));
            }

            return dataIndexes;
        }

        private static IList<int> GetEndingDataIndexes(IList<ChannelDataQuery> queries, IComparable maxIndex)
        {
            var dataIndexes = new List<int>(queries.Count);
            for (int i = 0; i < queries.Count; i++)
            {
                dataIndexes.Add(queries[i].Channel.GetEndDataIndex(queries[i].EndIndex ?? maxIndex, true));
            }

            return dataIndexes;
        }

        private static IComparable GetStartingIndex(IList<ChannelDataQuery> queries, IList<int> dataIndexes, IComparable maxIndex)
        {
            var currentIndex = maxIndex;
            for (int i = 0; i < queries.Count; i++)
            {
                if (dataIndexes[i] >= queries[i].Channel.DataCount)
                    continue;

                var timeIndex = queries[i].Channel.Index(dataIndexes[i]);
                if (timeIndex.CompareTo(currentIndex) < 0)
                {
                    currentIndex = timeIndex;
                }
            }

            return currentIndex;
        }

        private static IList<MockDataItem> GetChannelData(EtpVersion version, IEnumerable<ChannelSubscriptionInfo> infos, int maxDataItems)
        {
            var queries = infos.Select(info => info.Query).ToList();

            return GetChannelData(version, queries, maxDataItems);
        }

        private static IList<MockDataItem> GetChannelData(EtpVersion version, IList<ChannelDataQuery> queries, int maxDataItems)
        {
            var timeQueries = new List<ChannelDataQuery>();
            var depthQueries = new List<ChannelDataQuery>();

            for (int i = 0; i < queries.Count; i++)
            {
                if (queries[i].Channel.IsTime)
                    timeQueries.Add(queries[i]);
                else
                    depthQueries.Add(queries[i]);
            }

            var dataItems = new List<MockDataItem>();
            GetChannelData(dataItems, version, timeQueries, DateTime.MinValue, DateTime.MaxValue, maxDataItems);
            GetChannelData(dataItems, version, depthQueries, double.MinValue, double.MaxValue, maxDataItems);

            return dataItems;
        }

        private static void GetChannelData(IList<MockDataItem> dataItems, EtpVersion version, IList<ChannelDataQuery> queries, IComparable minIndex, IComparable maxIndex, int maxDataItems)
        {
            var dataIndexes = GetStartingDataIndexes(queries, minIndex);
            var currentIndex = GetStartingIndex(queries, dataIndexes, maxIndex);

            if (currentIndex.CompareTo(maxIndex) == 0) // No new data.
                return;

            var endIndexes = GetEndingDataIndexes(queries, maxIndex);
            IComparable nextIndex;
            do
            {
                nextIndex = maxIndex;
                var nullIndex = false;
                for (int i = 0; i < queries.Count; i++)
                {
                    if (dataIndexes[i] >= endIndexes[i])
                        continue;

                    var channel = queries[i].Channel;
                    var index = channel.Index(dataIndexes[i]);
                    if (index.CompareTo(currentIndex) == 0)
                    {
                        queries[i].StartIndex = index;
                        queries[i].IsStartIndexInclusive = false;
                        dataItems.Add(channel.DataItem(version, queries[i].ChannelId, dataIndexes[i]++, nullIndex));

                        nullIndex = true;
                        if (dataItems.Count == maxDataItems)
                            return;

                        if (dataIndexes[i] < endIndexes[i])
                        {
                            var idx = channel.Index(dataIndexes[i]);
                            if (idx.CompareTo(nextIndex) < 0)
                                nextIndex = idx;
                        }
                    }
                }

                currentIndex = nextIndex;
            }
            while (dataItems.Count < maxDataItems && nextIndex.CompareTo(maxIndex) != 0);
        }
    }
}

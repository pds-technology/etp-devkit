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
using Energistics.Etp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Energistics.Etp.Store
{
    public partial class MockStore
    {
        private Dictionary<Guid, ObjectSubscription> ObjectSubscriptionsByRequestUuid { get; } = new Dictionary<Guid, ObjectSubscription>();
        private BackgroundLoop ObjectNotificationLoop { get; } = new BackgroundLoop();

        private void StartObjectNotificationLoop()
        {
            ObjectNotificationLoop.Start(SendObjectNotifications, TimeSpan.FromSeconds(5));
        }

        private void RefreshObjectSubscriptions()
        {
            foreach (var subscription in ObjectSubscriptionsByRequestUuid.Values)
            {
                RefreshObjectSubscription(subscription);
            }
        }

        private void RefreshObjectSubscription(ObjectSubscription subscription)
        {
            var @objects = GetObjects(subscription.Version, subscription.SubscriptionInfo.Context);
            RefreshSubscriptionObjects(subscription, objects);
        }

        public bool SubscribeObjectNotifications(EtpVersion version, bool sendHistoricalChanges, DateTime historicalChangesStartTime, bool endIfRootDeleted, Guid sessionId, MockSubscriptionInfo subscriptionInfo, MockObjectCallbacks callbacks)
        {
            CheckLocked();

            if (ObjectSubscriptionsByRequestUuid.ContainsKey(subscriptionInfo.RequestUuid))
                return false;

            MockObject root = null;
            var uri = subscriptionInfo.Context.Uri;
            if (uri.IsObjectUri)
            {
                root = GetObject(version, uri);
                if (root == null)
                    return false;
            }

            ObjectSubscriptionsByRequestUuid[subscriptionInfo.RequestUuid] = new ObjectSubscription
            {
                SessionId = sessionId,
                Version = version,
                Uuid = subscriptionInfo.RequestUuid,
                LastNotificationTime = sendHistoricalChanges ? historicalChangesStartTime : StoreLastWrite,
                SubscriptionInfo = subscriptionInfo,
                Callbacks = callbacks,
                Root = root,
                SendHistoricalChanges = sendHistoricalChanges,
                EndIfRootDeleted = endIfRootDeleted,
            };

            return true;
        }

        public bool UnsubscribeObjectNotifications(Guid requestUuid)
        {
            CheckLocked();

            return ObjectSubscriptionsByRequestUuid.Remove(requestUuid);
        }

        public void CancelAllObjectNotifications(Guid sessionId)
        {
            CheckLocked();

            var requestUuids = ObjectSubscriptionsByRequestUuid.Values.Where(s => s.SessionId == sessionId).Select(s => s.Uuid).ToList();

            foreach (var requestUuid in requestUuids)
                UnsubscribeObjectNotifications(requestUuid);

        }

        private void SendObjectNotifications(CancellationToken token)
        {
            ExecuteWithLock(() =>
            {
                SendAllObjectNotifications();
            });
        }

        private void SendAllObjectNotifications()
        {
            CheckLocked();

            foreach (var subscription in ObjectSubscriptionsByRequestUuid.Values.ToList())
            {
                SendObjectSubscriptionNotifications(subscription);
            }
        }

        private void SendObjectSubscriptionNotifications(ObjectSubscription subscription)
        {
            CheckLocked();

            RefreshSubscriptions();

            SendJoinedSubscriptionNotifications(subscription);
            SendCreatedNotifications(subscription);
            SendUpdatedNotifications(subscription);
            SendJoinedAndUnjoinedNotifications(subscription);
            SendActivatedNotifications(subscription);
            SendDeactivatedNotifications(subscription);
            SendUnjoinedSubscriptionNotifications(subscription);

            subscription.PreviousObjects = subscription.Objects;
            subscription.Objects = new Dictionary<Guid, MockObject>();

            if (subscription.Root?.IsDeleted ?? false && subscription.EndIfRootDeleted)
            {
                subscription.Callbacks.SubscriptionEnded?.Invoke(subscription.Uuid, $"Object Deleted: {subscription.Root.Uri(subscription.Version)}");
                ObjectSubscriptionsByRequestUuid.Remove(subscription.Uuid);
            }

            subscription.LastNotificationTime = StoreLastWrite;
            subscription.SendHistoricalChanges = false; // Only send historical changes the first time.
        }
    }
}

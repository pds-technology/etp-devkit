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
        private class ObjectSubscription : Subscription<MockObject>
        {
            public MockObject Root { get; set; }
            public bool EndIfRootDeleted { get; set; }
            public MockSubscriptionInfo SubscriptionInfo { get; set; }
            public bool SendAllChanges { get; set; }
            public override MockObject GetContext(MockObject @object) => @object;
            public override bool IncludeObjectData(MockObject context) => SubscriptionInfo?.IncludeObjectData ?? false;
            public override IEnumerable<MockObject> GetCandidateObjects()
            {
                if (SendAllChanges)
                    return Objects.Values.FilterByStoreLastWrite(LastStoreWriteTime);
                else
                    return Objects.Values.FilterByObjectLastWrite(LastStoreWriteTime);
            }
        }

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

        public bool SubscribeObjectNotifications(EtpVersion version, DateTime startTime, bool endIfRootDeleted, MockSubscriptionInfo subscriptionInfo, MockObjectCallbacks callbacks)
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
                Version = version,
                Uuid = subscriptionInfo.RequestUuid,
                LastStoreWriteTime = startTime,
                SubscriptionInfo = subscriptionInfo,
                Callbacks = callbacks,
                Root = root,
                SendAllChanges = startTime < StoreLastWrite,
                EndIfRootDeleted = endIfRootDeleted,
            };

            return true;
        }

        public bool UnsubscribeObjectNotifications(Guid requestUuid)
        {
            CheckLocked();

            return ObjectSubscriptionsByRequestUuid.Remove(requestUuid);
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

            SendNotificationsForObjectsAddedToScope(subscription);
            SendUpdatedNotificationsForObjectsInScope(subscription);
            SendActivatedNotificationsForObjectsInScope(subscription);
            SendDeactivatedNotificationsForObjectsInScope(subscription);
            SendNotificationsForObjectsRemovedFromScope(subscription);

            subscription.AddedObjects.Clear();
            subscription.RemovedObjects.Clear();

            if (subscription.Root?.IsDeleted ?? false && subscription.EndIfRootDeleted)
            {
                subscription.Callbacks.SubscriptionEnded?.Invoke(subscription.Uuid);
                ObjectSubscriptionsByRequestUuid.Remove(subscription.Uuid);
            }
            subscription.SendAllChanges = false; // Only send all changes the first time

            subscription.LastStoreWriteTime = StoreLastWrite;
        }
    }
}

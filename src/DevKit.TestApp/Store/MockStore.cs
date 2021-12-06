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
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Energistics.Etp.Store
{
    public partial class MockStore
    {
        private readonly object _lock = new object();
        private ILog Logger { get; }

        public DateTime Creation { get; } = DateTime.UtcNow;

        private Dictionary<Guid, MockObject> Objects { get; } = new Dictionary<Guid, MockObject>();
        private Dictionary<Guid, MockObject> DeletedObjects { get; } = new Dictionary<Guid, MockObject>();
        private Dictionary<EtpVersion, Dictionary<string, MockObject>> ObjectsByUri { get; } = new Dictionary<EtpVersion, Dictionary<string, MockObject>>
        {
            [EtpVersion.v11] = new Dictionary<string, MockObject>(),
            [EtpVersion.v12] = new Dictionary<string, MockObject>(),
        };
        public static Dictionary<EtpDataObjectType, HashSet<EtpDataObjectType>> SupportedSourceTypes { get; } = new Dictionary<EtpDataObjectType, HashSet<EtpDataObjectType>>
        {
            [MockWell.Type] = MockWell.SourceTypes,
            [MockWellbore.Type] = MockWellbore.SourceTypes,
            [MockTrajectory.Type] = MockTrajectory.SourceTypes,
            [MockChannelSet.Type] = MockChannelSet.SourceTypes,
            [MockChannel.Type] = MockChannel.SourceTypes,
            [MockPropertyKind.Type] = MockPropertyKind.SourceTypes,
        };
        public static Dictionary<EtpDataObjectType, HashSet<EtpDataObjectType>> SupportedTargetTypes { get; } = new Dictionary<EtpDataObjectType, HashSet<EtpDataObjectType>>
        {
            [MockWell.Type] = MockWell.TargetTypes,
            [MockWellbore.Type] = MockWellbore.TargetTypes,
            [MockTrajectory.Type] = MockTrajectory.TargetTypes,
            [MockChannelSet.Type] = MockChannelSet.TargetTypes,
            [MockChannel.Type] = MockChannel.TargetTypes,
            [MockPropertyKind.Type] = MockPropertyKind.TargetTypes,
        };
        public static Dictionary<EtpDataObjectType, HashSet<EtpDataObjectType>> SupportedSecondarySourceTypes { get; } = new Dictionary<EtpDataObjectType, HashSet<EtpDataObjectType>>
        {
            [MockWell.Type] = MockWell.SecondarySourceTypes,
            [MockWellbore.Type] = MockWellbore.SecondarySourceTypes,
            [MockTrajectory.Type] = MockTrajectory.SecondarySourceTypes,
            [MockChannelSet.Type] = MockChannelSet.SecondarySourceTypes,
            [MockChannel.Type] = MockChannel.SecondarySourceTypes,
            [MockPropertyKind.Type] = MockPropertyKind.SecondarySourceTypes,
        };
        public static Dictionary<EtpDataObjectType, HashSet<EtpDataObjectType>> SupportedSecondaryTargetTypes { get; } = new Dictionary<EtpDataObjectType, HashSet<EtpDataObjectType>>
        {
            [MockWell.Type] = MockWell.SecondaryTargetTypes,
            [MockWellbore.Type] = MockWellbore.SecondaryTargetTypes,
            [MockTrajectory.Type] = MockTrajectory.SecondaryTargetTypes,
            [MockChannelSet.Type] = MockChannelSet.SecondaryTargetTypes,
            [MockChannel.Type] = MockChannel.SecondaryTargetTypes,
            [MockPropertyKind.Type] = MockPropertyKind.SecondaryTargetTypes,
        };
        public List<MockDataspace> Dataspaces { get; } = new List<MockDataspace>();

        public EtpSupportedDataObjectCollection SupportedDataObjects { get; } = new EtpSupportedDataObjectCollection();

        public DateTime StoreLastWrite { get; private set; }
        private BackgroundLoop ActiveStatusLoop { get; } = new BackgroundLoop();
        private bool Locked { get; set; }

        public MockStore()
        {
            Logger = LogManager.GetLogger(GetType());

            InitializeSupportedDataObjects();
            StartBackgroundLoops();
        }

        public void ExecuteWithLock(Action action)
        {
            lock (_lock)
            {
                Locked = true;

                StoreLastWrite = DateTime.UtcNow;
                action();

                Locked = false;
            }
        }

        public T ExecuteWithLock<T>(Func<T> func)
        {
            lock (_lock)
            {
                StoreLastWrite = DateTime.UtcNow;
                return func();
            }
        }

        private void StartBackgroundLoops()
        {
            ExecuteWithLock(() =>
            {
                Logger.Info("Starting background loops");
                StartObjectNotificationLoop();
                StartChannelNotificationLoop();
                ActiveStatusLoop.Start(RefreshActiveStatus, TimeSpan.FromSeconds(1.0));
            });
        }

        public void AddDataspace(MockDataspace dataspace)
        {
            ExecuteWithLock(() =>
            {
                Dataspaces.Add(dataspace);
                RefreshAll();
            });
        }

        public void RemoveDataspace(MockDataspace dataspace)
        {
            ExecuteWithLock(() =>
            {
                Dataspaces.Remove(dataspace);
                RefreshAll();
            });
        }

        public void RefreshAll()
        {
            RefreshGraph();
            RefreshObjectLookUps();
            RefreshFamilies();
        }

        private void RefreshGraph()
        {
            CheckLocked();

            foreach (var dataspace in Dataspaces)
            {
                foreach (var @object in dataspace.Objects.Values)
                    @object.ClearLinks();

                foreach (var @object in dataspace.Objects.Values)
                    @object.Link();
            }
        }

        private void RefreshObjectLookUps()
        {
            CheckLocked();

            Objects.Clear();
            DeletedObjects.Clear();
            ObjectsByUri[EtpVersion.v11].Clear();
            ObjectsByUri[EtpVersion.v12].Clear();

            foreach (var dataspace in Dataspaces)
            {
                foreach (var @object in dataspace.Objects.Values)
                {
                    if (@object.IsDeleted)
                        DeletedObjects[@object.Uuid] = @object;
                    else
                        Objects[@object.Uuid] = @object;

                    ObjectsByUri[EtpVersion.v11][@object.Uri(EtpVersion.v11)] = @object;
                    foreach (var alternateUri in @object.AlternateUris(EtpVersion.v11))
                        ObjectsByUri[EtpVersion.v11][alternateUri] = @object;

                    ObjectsByUri[EtpVersion.v12][@object.Uri(EtpVersion.v12)] = @object;
                    foreach (var alternateUri in @object.AlternateUris(EtpVersion.v12))
                        ObjectsByUri[EtpVersion.v12][alternateUri] = @object;
                }
            }
        }

        private void RefreshFamilies()
        {
            CheckLocked();

            foreach (var dataspace in Dataspaces)
            {
                foreach (var family in dataspace.Families)
                    family.SupportedObjectCount = SupportedDataObjects.Count(sdo => sdo.QualifiedType.MatchesFamilyAndVersion(family.Type));
            }
        }

        private void RefreshSubscriptions()
        {
            CheckLocked();

            RefreshObjectSubscriptions();
            RefreshChannelSubscriptions();
        }

        private void InitializeSupportedDataObjects()
        {
            SupportedDataObjects.AddSupportedDataObject(new EtpSupportedDataObject(MockWell.Type));
            SupportedDataObjects.AddSupportedDataObject(new EtpSupportedDataObject(MockWellbore.Type));
            SupportedDataObjects.AddSupportedDataObject(new EtpSupportedDataObject(MockChannelSet.Type));
            SupportedDataObjects.AddSupportedDataObject(new EtpSupportedDataObject(MockChannel.Type));
            SupportedDataObjects.AddSupportedDataObject(new EtpSupportedDataObject(MockTrajectory.Type));

            SupportedDataObjects.AddSupportedDataObject(new EtpSupportedDataObject(MockPropertyKind.Type));
        }

        private void CheckLocked()
        {
            if (!Locked)
                throw new InvalidOperationException("Store must be locked");
        }

        public void TouchObject(MockObject @object)
        {
            CheckLocked();
            if (@object == null)
                return;

            Logger.Debug($"Touching {@object.Uri(EtpVersion.v12)}");
            @object.Touch(StoreLastWrite);
            foreach (var parent in @object.Containers.Values)
            {
                Logger.Debug($"Updating container object {parent.Uri(EtpVersion.v12)}");
                foreach (var grandParent in parent.Containers.Values)
                    Logger.Debug($"Updating container object {grandParent.Uri(EtpVersion.v12)}");
            }
        }

        public void DeleteObject(MockObject @object, bool pruneContainedDataObjects = false)
        {
            CheckLocked();
            if (@object == null || @object.IsDeleted)
                return;

            Logger.Debug($"Deleting {@object.Uri(EtpVersion.v12)}");

            var deleted = @object.Delete(StoreLastWrite, pruneContainedDataObjects: pruneContainedDataObjects);
            foreach (var parent in @object.Containers.Values)
            {
                Logger.Debug($"Updating container object {parent.Uri(EtpVersion.v12)}");
                foreach (var grandParent in parent.Containers.Values)
                    Logger.Debug($"Updating container object {grandParent.Uri(EtpVersion.v12)}");
            }

            foreach (var deletedObject in deleted.Values)
            {
                @object.Dataspace.Objects.Remove(deletedObject.Uuid);
                @object.Dataspace.DeletedObjects[deletedObject.Uuid] = deletedObject;
            }
            RefreshAll();
        }

        public void RestoreObject(MockObject @object)
        {
            CheckLocked();
            if (@object == null || !@object.IsDeleted)
                return;

            Logger.Debug($"Restoring {@object.Uri(EtpVersion.v12)}");
            foreach (var parent in @object.Containers.Values)
            {
                Logger.Debug($"Updating container object {parent.Uri(EtpVersion.v12)}");
                foreach (var grandParent in parent.Containers.Values)
                    Logger.Debug($"Updating container object {grandParent.Uri(EtpVersion.v12)}");
            }

            var restored = @object.Restore(StoreLastWrite);

            foreach (var restoredObject in restored.Values)
            {
                @object.Dataspace.DeletedObjects.Remove(restoredObject.Uuid);
                @object.Dataspace.Objects[restoredObject.Uuid] = restoredObject;
            }

            RefreshAll();
        }

        public void UnjoinObject(MockObject container, MockObject containee)
        {
            CheckLocked();
            if (container == null || containee == null || !container.Containees.ContainsKey(containee.Uuid))
                return;

            Logger.Debug($"Unjoining {containee.Uri(EtpVersion.v12)} from {container.Uri(EtpVersion.v12)}");
            containee.UnjoinContainer(container, StoreLastWrite);

            RefreshAll();
        }

        public void JoinObject(MockObject container, MockObject containee)
        {
            CheckLocked();
            if (container == null || containee == null || container.Containees.ContainsKey(containee.Uuid))
                return;

            Logger.Debug($"Joining {containee.Uri(EtpVersion.v12)} to {container.Uri(EtpVersion.v12)}");
            containee.JoinContainer(container, StoreLastWrite);

            RefreshAll();
        }

        public void AppendDepthChannelData(MockObject @object, double index, double value)
        {
            CheckLocked();

            var channel = @object as MockChannel;
            if (channel == null)
                return;

            channel.AppendDepthData(index, value, StoreLastWrite);
        }

        public void AppendTimeChannelData(MockObject @object, DateTime index, double value)
        {
            CheckLocked();

            var channel = @object as MockChannel;
            if (channel == null)
                return;

            channel.AppendTimeData(index, value, StoreLastWrite);
        }

        private void RefreshActiveStatus(CancellationToken token)
        {
            ExecuteWithLock(() =>
            {
                RefreshActiveObjectActiveStatus();
            });
        }

        private void RefreshActiveObjectActiveStatus()
        {
            foreach (var @object in Objects.Values)
            {
                if (!(@object is IMockActiveObject))
                    continue;

                var growingObject = @object as IMockActiveObject;
                if (growingObject.IsActive && StoreLastWrite - growingObject.LastActivatedTime > TimeSpan.FromSeconds(3.0))
                    growingObject.SetActive(false, StoreLastWrite);
            }
        }

        private void RefreshSubscriptionObjects<TContext>(Subscription<TContext> subscription, IEnumerable<MockObject> objects)
        {
            // This method may be run multiple times before notifications are sent out.
            subscription.Objects = new Dictionary<Guid, MockObject>();
            foreach (var @object in objects)
                subscription.Objects[@object.Uuid] = @object;
        }

        private void SendJoinedSubscriptionNotifications<TContext>(Subscription<TContext> subscription)
        {
            CheckLocked();

            if (subscription.Callbacks.JoinedSubscription == null)
                return;

            foreach (var @object in subscription.GetJoinedScopeObjects())
            {
                var context = subscription.GetContext(@object);
                if (subscription.CanSendJoinedScope(context))
                    subscription.Callbacks.JoinedSubscription.Invoke(subscription.SubscriptionUuid(context), @object, subscription.IncludeObjectData(context));
            }
        }

        private void SendCreatedNotifications<TContext>(Subscription<TContext> subscription)
        {
            CheckLocked();

            if (subscription.Callbacks.Created == null)
                return;

            foreach (var @object in subscription.GetCreatedObjects())
            {
                var context = subscription.GetContext(@object);
                if (subscription.CanSendCreated(context))
                    subscription.Callbacks.Created.Invoke(subscription.SubscriptionUuid(context), @object, subscription.IncludeObjectData(context));
            }
        }

        private void SendUpdatedNotifications<TContext>(Subscription<TContext> subscription)
        {
            CheckLocked();

            if (subscription.Callbacks.Updated == null)
                return;

            foreach (var @object in subscription.GetUpdatedObjects())
            {
                var context = subscription.GetContext(@object);
                if (subscription.CanSendUpdated(context))
                    subscription.Callbacks.Updated.Invoke(subscription.SubscriptionUuid(context), @object, subscription.IncludeObjectData(context));
            }
        }

        private void SendJoinedAndUnjoinedNotifications<TContext>(Subscription<TContext> subscription)
        {
            CheckLocked();

            foreach (var @object in subscription.GetJoinedAndUnjoinedObjects())
            {
                var context = subscription.GetContext(@object);

                if (@object.LastJoinedTime > subscription.LastNotificationTime && @object.LastUnjoinedTime > subscription.LastNotificationTime)
                {
                    if (@object.LastJoinedTime > @object.LastUnjoinedTime)
                    {
                        if (subscription.Callbacks.Joined != null && subscription.CanSendJoined(context))
                            subscription.Callbacks.Joined.Invoke(subscription.SubscriptionUuid(context), @object, subscription.IncludeObjectData(context));
                        if (subscription.Callbacks.Unjoined != null && subscription.CanSendUnjoined(context))
                            subscription.Callbacks.Unjoined.Invoke(subscription.SubscriptionUuid(context), @object, subscription.IncludeObjectData(context));
                    }
                    else
                    {
                        if (subscription.Callbacks.Unjoined != null && subscription.CanSendUnjoined(context))
                            subscription.Callbacks.Unjoined.Invoke(subscription.SubscriptionUuid(context), @object, subscription.IncludeObjectData(context));
                        if (subscription.Callbacks.Joined != null && subscription.CanSendJoined(context))
                            subscription.Callbacks.Joined.Invoke(subscription.SubscriptionUuid(context), @object, subscription.IncludeObjectData(context));
                    }
                }
                else if (@object.LastJoinedTime > subscription.LastNotificationTime)
                {
                    if (subscription.Callbacks.Joined != null && subscription.CanSendJoined(context))
                        subscription.Callbacks.Joined.Invoke(subscription.SubscriptionUuid(context), @object, subscription.IncludeObjectData(context));
                }
                else if (@object.LastUnjoinedTime > subscription.LastNotificationTime)
                {
                    if (subscription.Callbacks.Unjoined != null && subscription.CanSendUnjoined(context))
                        subscription.Callbacks.Unjoined.Invoke(subscription.SubscriptionUuid(context), @object, subscription.IncludeObjectData(context));
                }
            }
        }


        private void SendActivatedNotifications<TContext>(Subscription<TContext> subscription)
        {
            CheckLocked();

            if (subscription.Callbacks.ActiveStatusChanged == null)
                return;

            foreach (var @object in subscription.GetActivedObjects())
            {
                var context = subscription.GetContext(@object);
                if (subscription.CanSendActiveStatusChanged(context))
                    subscription.Callbacks.ActiveStatusChanged.Invoke(subscription.SubscriptionUuid(context), @object, subscription.IncludeObjectData(context));
            }
        }

        private void SendDeactivatedNotifications<TContext>(Subscription<TContext> subscription)
        {
            CheckLocked();

            if (subscription.Callbacks.ActiveStatusChanged == null)
                return;

            foreach (var @object in subscription.GetDeactivedObjects())
            {
                var context = subscription.GetContext(@object);
                if (subscription.CanSendActiveStatusChanged(context))
                    subscription.Callbacks.ActiveStatusChanged.Invoke(subscription.SubscriptionUuid(context), @object, subscription.IncludeObjectData(context));
            }
        }

        private void SendDeletedNotifications<TContext>(Subscription<TContext> subscription)
        {
            CheckLocked();

            if (subscription.Callbacks.Deleted == null)
                return;

            foreach (var @object in subscription.GetDeletedObjects())
            {
                var context = subscription.GetContext(@object);
                if (subscription.CanSendDeleted(context))
                    subscription.Callbacks.Deleted.Invoke(subscription.SubscriptionUuid(context), @object);
            }
        }

        private void SendUnjoinedSubscriptionNotifications<TContext>(Subscription<TContext> subscription)
        {
            CheckLocked();

            if (subscription.Callbacks.UnjoinedSubscription == null)
                return;

            foreach (var @object in subscription.GetUnjoinedScopeObjects())
            {
                var context = subscription.GetContext(@object);
                if (subscription.CanSendUnjoinedScope(context))
                    subscription.Callbacks.UnjoinedSubscription.Invoke(subscription.SubscriptionUuid(context), @object, subscription.IncludeObjectData(context));
            }


        }
    }
}

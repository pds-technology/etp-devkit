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
using System.Threading.Tasks;

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

        private DateTime StoreLastWrite { get; set; }
        private BackgroundLoop ActiveStatusLoop { get; } = new BackgroundLoop();
        private bool Locked { get; set; }

        private abstract class Subscription<TContext>
        {
            public EtpVersion Version { get; set; }
            public Guid Uuid { get; set; }
            public DateTime LastStoreWriteTime { get; set; }
            public Dictionary<Guid, MockObject> Objects { get; } = new Dictionary<Guid, MockObject>();
            public Dictionary<Guid, MockObject> AddedObjects { get; } = new Dictionary<Guid, MockObject>();
            public Dictionary<Guid, MockObject> RemovedObjects { get; } = new Dictionary<Guid, MockObject>();
            public MockObjectCallbacks Callbacks { get; set; }
            public abstract TContext GetContext(MockObject @object);
            public virtual IEnumerable<MockObject> GetCandidateObjects() => Objects.Values.FilterByStoreLastWrite(LastStoreWriteTime);
            public virtual Guid SubscriptionUuid(TContext context) => Uuid;
            public virtual bool IncludeObjectData(TContext context) => true;
            public virtual bool CanSendCreated(TContext context) => true;
            public virtual bool CanSendUpdated(TContext context) => true;
            public virtual bool CanSendJoined(TContext context) => true;
            public virtual bool CanSendUnjoined(TContext context) => true;
            public virtual bool CanSendActiveStatusChanged(TContext context) => true;
            public virtual bool CanSendDeleted(TContext context) => true;
        }


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
            RefreshSubscriptions();
        }

        private void RefreshGraph()
        {
            CheckLocked();

            foreach (var dataspace in Dataspaces)
            {
                foreach (var @object in dataspace.Objects)
                    @object.ClearLinks();

                foreach (var @object in dataspace.Objects)
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
                foreach (var @object in dataspace.Objects)
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
            if (@object.Container != null)
            {
                Logger.Debug($"Updating container object {@object.Container.Uri(EtpVersion.v12)}");
                if (@object.Container.Container != null)
                    Logger.Debug($"Updating container object {@object.Container.Container.Uri(EtpVersion.v12)}");
            }
        }

        public void DeleteObject(MockObject @object)
        {
            CheckLocked();
            if (@object == null || @object.IsDeleted)
                return;

            Logger.Debug($"Deleting {@object.Uri(EtpVersion.v12)}");

            @object.Delete(StoreLastWrite);
            if (@object.Container != null)
            {
                Logger.Debug($"Updating container object {@object.Container.Uri(EtpVersion.v12)}");
                if (@object.Container.Container != null)
                    Logger.Debug($"Updating container object {@object.Container.Container.Uri(EtpVersion.v12)}");
            }

            RefreshSubscriptions();
            @object.Dataspace.Objects.Remove(@object);
            @object.Dataspace.DeletedObjects.Add(@object);
            RefreshAll();
        }

        public void RestoreObject(MockObject @object)
        {
            CheckLocked();
            if (@object == null || !@object.IsDeleted)
                return;

            Logger.Debug($"Restoring {@object.Uri(EtpVersion.v12)}");
            if (@object.Container != null)
            {
                Logger.Debug($"Updating container object {@object.Container.Uri(EtpVersion.v12)}");
                if (@object.Container.Container != null)
                    Logger.Debug($"Updating container object {@object.Container.Container.Uri(EtpVersion.v12)}");
            }

            @object.Restore(StoreLastWrite);

            @object.Dataspace.DeletedObjects.Remove(@object);
            @object.Dataspace.Objects.Add(@object);
            RefreshAll();
        }

        public void UnjoinObject(MockObject @object)
        {
            CheckLocked();
            if (@object?.Container == null)
                return;

            Logger.Debug($"Unjoining {@object.Uri(EtpVersion.v12)} from {@object.Container.Uri(EtpVersion.v12)}");
            @object.Unjoin(StoreLastWrite);

            RefreshAll();
        }

        public void JoinObject(MockObject @object, MockObject container)
        {
            CheckLocked();
            if (container == null || @object.Container != null)
                return;

            Logger.Debug($"Joining {@object.Uri(EtpVersion.v12)} to {container.Uri(EtpVersion.v12)}");
            @object.Join(container, StoreLastWrite);

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
                RefreshGrowingObjectActiveStatus();
                RefreshWellboreActiveStatus();
            });
        }

        private void RefreshGrowingObjectActiveStatus()
        {
            foreach (var @object in Objects.Values)
            {
                if (!(@object is IMockGrowingObject))
                    continue;

                var growingObject = @object as IMockGrowingObject;
                if (growingObject.IsActive && StoreLastWrite - growingObject.AppendTime > TimeSpan.FromSeconds(3.0))
                    growingObject.SetActive(false, StoreLastWrite);
            }
        }

        private void RefreshWellboreActiveStatus()
        {
            foreach (var @object in Objects.Values)
            {
                if (!(@object is MockWellbore))
                    continue;

                var wellbore = @object as MockWellbore;
                var isActive = @object.Sources.Any(s => (s is IMockGrowingObject) && ((IMockGrowingObject)s).IsActive);
                wellbore.SetActive(isActive, StoreLastWrite);
            }
        }

        private void RefreshSubscriptionObjects<TContext>(Subscription<TContext> subscription, IEnumerable<MockObject> objects)
        {
            var removedObjects = new Dictionary<Guid, MockObject>();
            var addedObjects = new Dictionary<Guid, MockObject>();

            var newObjects = new Dictionary<Guid, MockObject>();
            // Set up initial set of objects and deleted objects based on current graph.
            // Current graph may include data objects flagged as deleted that are in the process of being deleted.
            // Also detect and any added objects.
            foreach (var @object in objects)
            {
                if (@object.IsDeleted)
                    removedObjects[@object.Uuid] = @object;
                else
                {
                    newObjects[@object.Uuid] = @object;
                    if (!subscription.Objects.ContainsKey(@object.Uuid) || @object.StoreCreated > subscription.LastStoreWriteTime)
                        addedObjects[@object.Uuid] = @object;
                }
            }

            // Compare against previously known objects to find any unjoined or other deleted objects.
            foreach (var @object in subscription.Objects.Values)
            {
                if (!newObjects.ContainsKey(@object.Uuid))
                    removedObjects[@object.Uuid] = @object;
            }

            // Replace set of current objects.
            subscription.Objects.Clear();
            foreach (var kvp in newObjects)
                subscription.Objects[kvp.Key] = kvp.Value;

            // Clean up current added / removed state.
            foreach (var uuid in subscription.AddedObjects.Keys.ToList())
            {
                if (!subscription.Objects.ContainsKey(uuid))
                    subscription.AddedObjects.Remove(uuid);
            }
            foreach (var uuid in subscription.RemovedObjects.Keys.ToList())
            {
                if (subscription.Objects.ContainsKey(uuid))
                    subscription.RemovedObjects.Remove(uuid);
            }

            // Merge in new added / removed objects.
            foreach (var kvp in addedObjects)
                subscription.AddedObjects[kvp.Key] = kvp.Value;
            foreach (var kvp in removedObjects)
                subscription.RemovedObjects[kvp.Key] = kvp.Value;
        }

        private void SendNotificationsForObjectsAddedToScope<TContext>(Subscription<TContext> subscription)
        {
            CheckLocked();

            foreach (var @object in subscription.AddedObjects.Values)
            {
                var context = subscription.GetContext(@object);
                if (@object.StoreCreated > subscription.LastStoreWriteTime)
                {
                    if (subscription.CanSendCreated(context))
                        subscription.Callbacks.Created?.Invoke(subscription.SubscriptionUuid(context), @object, subscription.IncludeObjectData(context));
                }
                else
                {
                    if (subscription.CanSendJoined(context))
                        subscription.Callbacks.Joined?.Invoke(subscription.SubscriptionUuid(context), @object, subscription.IncludeObjectData(context));
                }
            }
        }

        private void SendUpdatedNotificationsForObjectsInScope<TContext>(Subscription<TContext> subscription)
        {
            CheckLocked();

            foreach (var @object in subscription.GetCandidateObjects())
            {
                if (subscription.AddedObjects.ContainsKey(@object.Uuid)) // Skip created / joined objects.
                    continue;

                var context = subscription.GetContext(@object);
                if (subscription.CanSendUpdated(context))
                    subscription.Callbacks.Updated?.Invoke(subscription.SubscriptionUuid(context), @object, subscription.IncludeObjectData(context));
            }
        }


        private void SendActivatedNotificationsForObjectsInScope<TContext>(Subscription<TContext> subscription)
        {
            CheckLocked();

            foreach (var @object in subscription.GetCandidateObjects())
            {
                if (subscription.AddedObjects.ContainsKey(@object.Uuid)) // Skip created / joined objects.
                    continue;

                if (@object is IMockActiveObject)
                {
                    var context = subscription.GetContext(@object);
                    var activeObject = @object as IMockActiveObject;
                    if (activeObject.ActiveChangeTime > subscription.LastStoreWriteTime && subscription.CanSendActiveStatusChanged(context) && activeObject.IsActive)
                        subscription.Callbacks.ActiveStatusChanged?.Invoke(subscription.SubscriptionUuid(context), @object, activeObject.IsActive);
                }
            }
        }

        private void SendDeactivatedNotificationsForObjectsInScope<TContext>(Subscription<TContext> subscription)
        {
            CheckLocked();

            foreach (var @object in subscription.GetCandidateObjects())
            {
                if (subscription.AddedObjects.ContainsKey(@object.Uuid)) // Skip created / joined objects.
                    continue;

                if (@object is IMockActiveObject)
                {
                    var context = subscription.GetContext(@object);
                    var activeObject = @object as IMockActiveObject;
                    if (activeObject.ActiveChangeTime > subscription.LastStoreWriteTime && subscription.CanSendActiveStatusChanged(context) && !activeObject.IsActive)
                        subscription.Callbacks.ActiveStatusChanged?.Invoke(subscription.SubscriptionUuid(context), @object, activeObject.IsActive);
                }
            }
        }

        private void SendNotificationsForObjectsRemovedFromScope<TContext>(Subscription<TContext> subscription)
        {
            CheckLocked();

            foreach (var @object in subscription.RemovedObjects.Values)
            {
                var context = subscription.GetContext(@object);
                if (@object.IsDeleted)
                {
                    if (subscription.CanSendDeleted(context))
                        subscription.Callbacks.Deleted?.Invoke(subscription.SubscriptionUuid(context), @object);
                }
                else
                {
                    if (subscription.CanSendUnjoined(context))
                        subscription.Callbacks.Unjoined?.Invoke(subscription.SubscriptionUuid(context), @object, subscription.IncludeObjectData(context));
                }
            }
        }
    }
}

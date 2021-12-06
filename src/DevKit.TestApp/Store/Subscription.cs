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

namespace Energistics.Etp.Store
{
    internal abstract class Subscription<TContext>
    {
        public bool IsNewSubscription { get; set; } = true;
        public EtpVersion Version { get; set; }
        public Guid Uuid { get; set; }
        public DateTime LastNotificationTime { get; set; }
        public Dictionary<Guid, MockObject> PreviousObjects { get; set; } = new Dictionary<Guid, MockObject>();
        public Dictionary<Guid, MockObject> Objects { get; set; } = new Dictionary<Guid, MockObject>();
        public MockObjectCallbacks Callbacks { get; set; }
        public abstract TContext GetContext(MockObject @object);
        public virtual IEnumerable<MockObject> GetCandidateObjects() => Objects.Values.FilterByStoreLastWrite(LastNotificationTime);
        public virtual IEnumerable<MockObject> GetJoinedScopeObjects() => Objects.Values.Where(o => !PreviousObjects.ContainsKey(o.Uuid) && o.StoreCreated <= LastNotificationTime);
        public virtual IEnumerable<MockObject> GetCreatedObjects() => GetCandidateObjects().Where(o => o.StoreCreated > LastNotificationTime);
        public virtual IEnumerable<MockObject> GetUpdatedObjects() => GetCandidateObjects().Where(o => o.StoreCreated <= LastNotificationTime);
        public virtual IEnumerable<MockObject> GetJoinedAndUnjoinedObjects() => GetCandidateObjects().Where(o => o.LastJoinedTime > LastNotificationTime || o.LastUnjoinedTime > LastNotificationTime);
        public virtual IEnumerable<MockObject> GetActivedObjects() => GetCandidateObjects().Where(o => o.ActiveChangeTime > LastNotificationTime && o.IsActive);
        public virtual IEnumerable<MockObject> GetDeactivedObjects() => GetCandidateObjects().Where(o => o.ActiveChangeTime > LastNotificationTime && !o.IsActive);
        public virtual IEnumerable<MockObject> GetDeletedObjects() => PreviousObjects.Values.Where(o => !Objects.ContainsKey(o.Uuid) && o.IsDeleted);
        public virtual IEnumerable<MockObject> GetUnjoinedScopeObjects() => PreviousObjects.Values.Where(o => !Objects.ContainsKey(o.Uuid) && !o.IsDeleted);
        public virtual IEnumerable<MockObject> GetAllRemovedObjects() => PreviousObjects.Values.Where(o => !Objects.ContainsKey(o.Uuid));
        public virtual Guid SubscriptionUuid(TContext context) => Uuid;
        public virtual bool IncludeObjectData(TContext context) => true;
        public virtual bool CanSendJoinedScope(TContext context) => true;
        public virtual bool CanSendCreated(TContext context) => true;
        public virtual bool CanSendUpdated(TContext context) => true;
        public virtual bool CanSendJoined(TContext context) => true;
        public virtual bool CanSendUnjoined(TContext context) => true;
        public virtual bool CanSendActiveStatusChanged(TContext context) => true;
        public virtual bool CanSendDeleted(TContext context) => true;
        public virtual bool CanSendUnjoinedScope(TContext context) => true;
    }
}

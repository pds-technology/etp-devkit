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

using Energistics.Etp.Data;
using System;
using System.Collections.Generic;

namespace Energistics.Etp.Store
{
    internal class ChannelSubscription : Subscription<ChannelSubscriptionInfo>
    {
        public bool AutoStart { get; set; }
        public int MaxDataItems { get; set; }
        public TimeSpan MaxMessageRate { get; set; }
        public long NextChannelId { get; set; } = 0;
        public Dictionary<long, ChannelSubscriptionInfo> ValidChannelIds { get; } = new Dictionary<long, ChannelSubscriptionInfo>();
        public Dictionary<Guid, MockSubscriptionInfo> ChannelScopesByChannelScopeUuid { get; } = new Dictionary<Guid, MockSubscriptionInfo>();
        public Dictionary<Guid, ChannelSubscriptionInfo> ChannelSubscriptionsByChannelUuid { get; } = new Dictionary<Guid, ChannelSubscriptionInfo>();
        public Dictionary<Guid, Guid> ChannelScopeUuidByChannelUuid { get; } = new Dictionary<Guid, Guid>();
        new public MockGrowingObjectCallbacks Callbacks { get { return base.Callbacks as MockGrowingObjectCallbacks; } set { base.Callbacks = value; } }
        public override ChannelSubscriptionInfo GetContext(MockObject @object)
        {
            ChannelSubscriptionInfo info;
            if (@object != null && ChannelSubscriptionsByChannelUuid.TryGetValue(@object.Uuid, out info))
                return info;
            else
                return null;
        }
        public override Guid SubscriptionUuid(ChannelSubscriptionInfo context) => context?.ChannelScopeUuid ?? default(Guid);
        public override bool CanSendJoinedScope(ChannelSubscriptionInfo context) => true;
        public override bool CanSendUpdated(ChannelSubscriptionInfo context) => false;
        public override bool CanSendActiveStatusChanged(ChannelSubscriptionInfo context) => context != null && context.IsStarted && context.SendChanges;
        public override bool CanSendDeleted(ChannelSubscriptionInfo context) => context?.IsStarted ?? false;
        public override bool CanSendUnjoinedScope(ChannelSubscriptionInfo context) => context?.IsStarted ?? false;
        public DateTime LastSendTime { get; set; } = DateTime.MinValue;
    }
}

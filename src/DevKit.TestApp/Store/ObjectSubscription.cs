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
    internal class ObjectSubscription : Subscription<MockObject>
    {
        public Guid SessionId { get; set; }
        public MockObject Root { get; set; }
        public bool EndIfRootDeleted { get; set; }
        public MockSubscriptionInfo SubscriptionInfo { get; set; }
        public bool SendHistoricalChanges { get; set; }
        public override MockObject GetContext(MockObject @object) => @object;
        public override bool IncludeObjectData(MockObject context) => SubscriptionInfo?.IncludeObjectData ?? false;
        public override IEnumerable<MockObject> GetCandidateObjects()
        {
            if (SendHistoricalChanges)
                return Objects.Values.FilterByStoreLastWrite(LastNotificationTime);
            else
                return Objects.Values.FilterByObjectLastWrite(LastNotificationTime);
        }
        public override bool CanSendJoinedScope(MockObject context) => !PreviousObjects.ContainsKey(context.Uuid);
        public override bool CanSendUnjoinedScope(MockObject context) => !context.IsDeleted;
    }
}

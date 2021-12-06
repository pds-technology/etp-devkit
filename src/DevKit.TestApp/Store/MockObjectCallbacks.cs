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
    public class MockObjectCallbacks
    {
        public delegate void JoinedSubscriptionCallback(Guid subscriptionUuid, MockObject @object, bool includeData);
        public delegate void CreatedCallback(Guid subscriptionUuid, MockObject @object, bool includeData);
        public delegate void UpdatedCallback(Guid subscriptionUuid, MockObject @object, bool includeData);
        public delegate void JoinedCallback(Guid subscriptionUuid, MockObject @object, bool includeData);
        public delegate void UnjoinedCallback(Guid subscriptionUuid, MockObject @object, bool includeData);
        public delegate void ActiveStatusChangedCallback(Guid subscriptionUuid, MockObject @object, bool isActive);
        public delegate void DeletedCallback(Guid subscriptionUuid, MockObject @object);
        public delegate void SubscriptionEndedCallback(Guid subscriptionUuid, string reason);
        public delegate void UnjoinedSubscriptionCallback(Guid subscriptionUuid, MockObject @object, bool includeData);

        public JoinedSubscriptionCallback JoinedSubscription { get; set; }
        public CreatedCallback Created { get; set; }
        public UpdatedCallback Updated { get; set; }
        public JoinedCallback Joined { get; set; }
        public UnjoinedCallback Unjoined { get; set; }
        public ActiveStatusChangedCallback ActiveStatusChanged { get; set; }
        public DeletedCallback Deleted { get; set; }
        public UnjoinedSubscriptionCallback UnjoinedSubscription { get; set; }
        public SubscriptionEndedCallback SubscriptionEnded { get; set; }
    }

    public class MockGrowingObjectCallbacks : MockObjectCallbacks
    {
        public delegate void DataAppendedCallback(Guid subscriptionUuid, IList<MockDataItem> dataItems);

        public delegate void DepthDataReplaced(Guid subscriptionUuid, MockObject @object, double start, double end, IEnumerable<KeyValuePair<double, double>> replacementData);
        public delegate void TimeDataReplaced(Guid subscriptionUuid, MockObject @object, DateTime start, DateTime end, IEnumerable<KeyValuePair<double, double>> replacementData);

        public delegate void DepthDataTruncated(Guid subscriptionUuid, MockObject @object, double end);
        public delegate void TimeDataTruncated(Guid subscriptionUuid, MockObject @object, DateTime end);

        public DataAppendedCallback DataAppended { get; set; }
    }
}

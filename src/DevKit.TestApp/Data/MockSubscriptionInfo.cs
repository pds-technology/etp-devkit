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
using System;

namespace Energistics.Etp.Data
{
    public class MockSubscriptionInfo
    {
        public MockSubscriptionInfo(Common.Datatypes.ChannelData.IChannelDescribeSubscription subscription)
        {
            RequestUuid = subscription.RequestUuidGuid.UuidGuid;
            IncludeObjectData = false;
            Context = new MockGraphContext(subscription);
        }

        public MockSubscriptionInfo(v11.Datatypes.Object.NotificationRequestRecord request)
        {
            RequestUuid = request.RequestUuidGuid.UuidGuid;
            IncludeObjectData = request.IncludeObjectData;
            Context = new MockGraphContext(request);
        }

        public MockSubscriptionInfo(v12.Datatypes.Object.SubscriptionInfo subscriptionInfo)
        {
            RequestUuid = subscriptionInfo.RequestUuidGuid.UuidGuid;
            IncludeObjectData = subscriptionInfo.IncludeObjectData;
            Format = subscriptionInfo.Format;
            Context = new MockGraphContext(subscriptionInfo);
        }

        public MockSubscriptionInfo(string uri, Guid namespaceId)
        {
            RequestUuid = GuidUtility.Create(namespaceId, uri);
            IncludeObjectData = false;
            Context = new MockGraphContext(new EtpUri(uri));
        }

        public MockSubscriptionInfo(EtpVersion version, MockObject mockObject, Guid requestUuid)
        {
            RequestUuid = requestUuid;
            IncludeObjectData = true;
            Context = new MockGraphContext(mockObject.Uri(version));
        }


        public Guid RequestUuid { get; }
        public string Format { get; } = Formats.Xml;
        public bool IncludeObjectData { get; }
        public MockGraphContext Context { get; }

        public v12.Datatypes.Object.SubscriptionInfo SubsriptionInfo12 => new v12.Datatypes.Object.SubscriptionInfo
        {
            Format = Format,
            Context = Context.ContextInfo12,
            Scope = Context.ContextScopeKind12,
            IncludeObjectData = IncludeObjectData,
            RequestUuid = RequestUuid.ToUuid<v12.Datatypes.Uuid>(),
        };
    };
}

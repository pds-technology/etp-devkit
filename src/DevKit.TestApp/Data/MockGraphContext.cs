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

using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Datatypes.ChannelData;
using Energistics.Etp.Store;
using System.Collections.Generic;
using System.Linq;

namespace Energistics.Etp.Data
{
    public class MockGraphContext
    {
        public MockGraphContext()
        {
        }

        public MockGraphContext(IChannelDescribeSubscription subscription)
            : this(subscription.Uri, MockObject.GrowingObjectTypes)
        {
        }

        public MockGraphContext(v11.Datatypes.Object.NotificationRequestRecord request)
            : this(request.Uri, request.ObjectTypes?.ToContentTypes().Select(ct => ct.ToDataObjectType()))
        {
        }

        protected MockGraphContext(string uri, IEnumerable<EtpDataObjectType> dataObjectTypes)
        {
            Uri = new EtpUri(uri);
            IncludeSelf = true;
            IncludeSources = true;
            Depth = 999;
            NavigatePrimaryEdges = true;
            DataObjectTypes = new HashSet<EtpDataObjectType>(dataObjectTypes);
        }

        public MockGraphContext(v12.Datatypes.Object.SubscriptionInfo subscriptionInfo)
            : this(subscriptionInfo.Context, subscriptionInfo.Scope)
        {
        }

        public MockGraphContext(v12.Protocol.Discovery.GetResources getResources)
            : this(getResources.Context, getResources.Scope)
        {
        }

        public MockGraphContext(v12.Datatypes.Object.ContextInfo context, v12.Datatypes.Object.ContextScopeKind scope)
        {
            Uri = new EtpUri(context.Uri);
            IncludeSelf = scope == v12.Datatypes.Object.ContextScopeKind.self || scope == v12.Datatypes.Object.ContextScopeKind.sourcesOrSelf || scope == v12.Datatypes.Object.ContextScopeKind.targetsOrSelf;
            IncludeSources = scope == v12.Datatypes.Object.ContextScopeKind.sources || scope == v12.Datatypes.Object.ContextScopeKind.sourcesOrSelf;
            IncludeTargets = scope == v12.Datatypes.Object.ContextScopeKind.targets || scope == v12.Datatypes.Object.ContextScopeKind.targetsOrSelf;
            Depth = context.Depth;
            DataObjectTypes = context.DataObjectTypes.ToDataObjectTypes();
            NavigatePrimaryEdges = context.NavigableEdges == v12.Datatypes.Object.RelationshipKind.Primary || context.NavigableEdges == v12.Datatypes.Object.RelationshipKind.Both;
            NavigateSecondaryEdges = context.NavigableEdges == v12.Datatypes.Object.RelationshipKind.Secondary || context.NavigableEdges == v12.Datatypes.Object.RelationshipKind.Both;
            IncludeSecondarySources = context.IncludeSecondarySources;
            IncludeSecondaryTargets = context.IncludeSecondaryTargets;
        }

        public MockGraphContext(EtpUri uri, EtpDataObjectType dataObjectType)
        {
            Uri = uri;
            IncludeSelf = false;
            IncludeSources = true;
            Depth = 1;
            NavigatePrimaryEdges = true;
            NavigateSecondaryEdges = true;
            if (dataObjectType != null)
                DataObjectTypes = new HashSet<EtpDataObjectType> { dataObjectType };
        }

        public MockGraphContext(EtpUri uri)
        {
            Uri = uri;
            IncludeSelf = true;
            Depth = 1;
        }

        public MockGraphContext(EtpUri uri, bool includeSources, bool includeTargets)
        {
            Uri = uri;
            IncludeSources = includeSources;
            IncludeTargets = includeTargets;
            Depth = 1;
            NavigatePrimaryEdges = true;
            NavigateSecondaryEdges = true;
        }

        public MockGraphContext(v12.Protocol.SupportedTypes.GetSupportedTypes getSupportedTypes)
            : this(new EtpUri(getSupportedTypes.Uri), getSupportedTypes.Scope)
        {
        }

        public MockGraphContext(EtpUri uri, v12.Datatypes.Object.ContextScopeKind scope)
        {
            Uri = uri;
            IncludeSelf = scope == v12.Datatypes.Object.ContextScopeKind.sources || scope == v12.Datatypes.Object.ContextScopeKind.sourcesOrSelf;
            IncludeSources = scope == v12.Datatypes.Object.ContextScopeKind.targets || scope == v12.Datatypes.Object.ContextScopeKind.targetsOrSelf;
            Depth = 1;
            NavigatePrimaryEdges = true;
            NavigateSecondaryEdges = true;
        }

        public EtpUri Uri { get; set; }
        public bool IncludeSelf { get; set; }
        public bool IncludeSources { get; set; }
        public bool IncludeTargets { get; set; }
        public bool IsSelfOnly => IncludeSelf && !IncludeSources && !IncludeTargets;
        public int Depth { get; set; }
        public ISet<EtpDataObjectType> DataObjectTypes { get; set; }
        public bool NavigatePrimaryEdges { get; set; }
        public bool NavigateSecondaryEdges { get; set; }
        public bool IncludeSecondaryTargets { get; set; }
        public bool IncludeSecondarySources { get; set; }

        public v12.Datatypes.Object.ContextInfo ContextInfo12 => new v12.Datatypes.Object.ContextInfo
        {
            Uri = Uri,
            Depth = Depth,
            IncludeSecondarySources = IncludeSecondarySources,
            IncludeSecondaryTargets = IncludeSecondaryTargets,
            NavigableEdges = (NavigatePrimaryEdges && NavigateSecondaryEdges)
                ? v12.Datatypes.Object.RelationshipKind.Both
                : (NavigateSecondaryEdges ? v12.Datatypes.Object.RelationshipKind.Secondary : v12.Datatypes.Object.RelationshipKind.Primary),
            DataObjectTypes = DataObjectTypes?.Select(dt => dt.ToString()).ToList() ?? new List<string>(),
        };

        public v12.Datatypes.Object.ContextScopeKind ContextScopeKind12 =>
            IsSelfOnly
                ? v12.Datatypes.Object.ContextScopeKind.self
                : (IncludeSources
                    ? (IncludeSelf ? v12.Datatypes.Object.ContextScopeKind.sourcesOrSelf : v12.Datatypes.Object.ContextScopeKind.sources)
                    : (IncludeSelf ? v12.Datatypes.Object.ContextScopeKind.targetsOrSelf : v12.Datatypes.Object.ContextScopeKind.targets));
    };
}

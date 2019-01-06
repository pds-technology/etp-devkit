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

using System;
using System.Collections.Generic;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v11.Datatypes.Object;
using Energistics.Etp.v11.Protocol.Discovery;

namespace Energistics.Etp.Providers
{
    public class MockResourceProvider : DiscoveryStoreHandler
    {
        private const string Witsml141 = "application/x-witsml+xml;version=1.4.1.1;";
        private const string BaseUri = "eml://witsml14";

        protected override void HandleGetResources(ProtocolEventArgs<GetResources, IList<Resource>> args)
        {
            if (EtpUri.IsRoot(args.Message.Uri))
            {
                args.Context.Add(New(
                    x => BaseUri,
                    contentType: Witsml141,
                    resourceType: ResourceTypes.UriProtocol,
                    name: "WITSML Store (1.4.1.1)"));
            }
            else if (BaseUri.Equals(args.Message.Uri, StringComparison.InvariantCultureIgnoreCase))
            {
                args.Context.Add(New(
                    uuid => String.Format("{0}/well({1})", args.Message.Uri, uuid),
                    contentType: Witsml141 + "type=well",
                    resourceType: ResourceTypes.DataObject,
                    name: "Well 01"));

                args.Context.Add(New(
                    uuid => String.Format("{0}/well({1})", args.Message.Uri, uuid),
                    contentType: Witsml141 + "type=well",
                    resourceType: ResourceTypes.DataObject,
                    name: "Well 02"));
            }
            else if (args.Message.Uri.Contains("/well(") && !args.Message.Uri.Contains("/wellbore("))
            {
                args.Context.Add(New(
                    uuid => String.Format("{0}/wellbore({1})", args.Message.Uri, uuid),
                    contentType: Witsml141 + "type=wellbore",
                    resourceType: ResourceTypes.DataObject,
                    name: "Wellbore 01-01"));

                args.Context.Add(New(
                    uuid => String.Format("{0}/wellbore({1})", args.Message.Uri, uuid),
                    contentType: Witsml141 + "type=wellbore",
                    resourceType: ResourceTypes.DataObject,
                    name: "Wellbore 01-02"));
            }
            else if (args.Message.Uri.Contains("/wellbore("))
            {
                args.Context.Add(New(
                    uuid => String.Format("{0}/log({1})", args.Message.Uri, uuid),
                    contentType: Witsml141 + "type=log",
                    resourceType: ResourceTypes.DataObject,
                    name: "Depth Log 01",
                    count: 0));

                args.Context.Add(New(
                    uuid => String.Format("{0}/log({1})", args.Message.Uri, uuid),
                    contentType: Witsml141 + "type=log",
                    resourceType: ResourceTypes.DataObject,
                    name: "Time Log 01",
                    count: 0));
            }
        }

        private Resource New(Func<string, string> formatUri, ResourceTypes resourceType, string contentType, string name, int count = 1)
        {
            var uuid = Guid.NewGuid().ToString();

            return new Resource()
            {
                Uuid = uuid,
                Uri = formatUri(uuid),
                Name = name,
                HasChildren = count,
                ContentType = contentType,
                ResourceType = resourceType.ToString(),
                CustomData = new Dictionary<string, string>()
            };
        }
    }
}

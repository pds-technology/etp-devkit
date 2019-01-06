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

using System.Collections.Generic;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v11.Datatypes.Object;

namespace Energistics.Etp.v11.Protocol.Discovery
{
    public class DiscoveryStore11MockHandler : DiscoveryStoreHandler
    {
        public DiscoveryStore11MockHandler()
        {

        }

        protected override void HandleGetResources(ProtocolEventArgs<GetResources, IList<Resource>> args)
        {
            var witsml20 = new EtpUri("eml://witsml20");

            if (args.Message.Uri == EtpUri.RootUri)
            {
                args.Context.Add(new Resource
                {
                    Uuid = null,
                    Uri = witsml20,
                    Name = "WITSML Store (2.0)",
                    HasChildren = 1,
                    ContentType = witsml20.ContentType,
                    ResourceType = ResourceTypes.UriProtocol.ToString(),
                    CustomData = new Dictionary<string, string>(),
                    LastChanged = 0,
                    ChannelSubscribable = false,
                    ObjectNotifiable = false,
                });
            }
            else if (args.Message.Uri == witsml20)
            {
                var witsml20well = new EtpUri("eml://witsml20/well");
                args.Context.Add(new Resource
                {
                    Uuid = null,
                    Uri = witsml20well,
                    Name = "Well",
                    HasChildren = 0,
                    ContentType = witsml20well.ContentType,
                    ResourceType = ResourceTypes.Folder.ToString(),
                    CustomData = new Dictionary<string, string>(),
                    LastChanged = 0,
                    ChannelSubscribable = false,
                    ObjectNotifiable = false,
                });
            }
            else
            {
                args.Cancel = true;
            }
        }
    }
}

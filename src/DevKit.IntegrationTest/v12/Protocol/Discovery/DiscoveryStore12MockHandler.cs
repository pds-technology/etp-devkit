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
using Energistics.Etp.v12.Datatypes;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.Discovery
{
    public class DiscoveryStore12MockHandler : DiscoveryStoreHandler
    {
        public DiscoveryStore12MockHandler()
        {

        }

        protected override void HandleGetResources(DualListRequestEventArgs<GetResources, Resource, Edge> args)
        {
            if (args.Request.Body.Context.Uri == EtpUri.RootUri12)
            {
                var now = DateTime.UtcNow;
                var uri = new EtpUri("eml:///witsml20.Well(5c365045-3a12-49b3-9276-356b90fff03b)");
                args.Responses1.Add(new Resource
                {
                    Uri = uri,
                    Name = "Test Well",
                    DataObjectType = uri.DataObjectType,
                    AlternateUris = new List<string>(),
                    CustomData = new Dictionary<string, DataValue>(),
                    LastChanged = now.ToEtpTimestamp(),
                    StoreLastWrite = now.ToEtpTimestamp(),
                    SourceCount = 0,
                    TargetCount = 0,
                });
            }
            else
            {
                args.FinalError = ErrorInfo().NotSupported();
            }
        }
    }
}

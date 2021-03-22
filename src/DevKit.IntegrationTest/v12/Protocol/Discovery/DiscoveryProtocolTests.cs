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
using System.Linq;
using System.Threading.Tasks;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v12.Datatypes.Object;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Energistics.Etp.v12.Protocol.Discovery
{
    [TestClass]
    public class DiscoveryProtocolTests : IntegrationTestBase
    {
        [TestInitialize]
        public void TestSetUp()
        {
            SetUp(TestSettings.WebSocketType, EtpVersion.v12);

            // Register protocol handler
            _server.ServerManager.Register(new DiscoveryStore12MockHandler());

            _server.Start();
        }

        [TestCleanup]
        public void TestTearDown()
        {
            CleanUp();
        }

        [TestMethod]
        public async Task IDiscoveryCustomer_v12_GetResource_Request_Default_Uri()
        {
            var handler = _client.Handler<IDiscoveryCustomer>();

            // Wait for Open connection
            var isOpen = await _client.OpenAsyncWithTimeout();
            Assert.IsTrue(isOpen);

            // Register event handler for root URI
            var onGetRootResourcesResponse = HandleAsync<DualResponseEventArgs<GetResources, GetResourcesResponse, GetResourcesEdgesResponse>>(
                x => handler.OnGetResourcesResponse += x);

            // Send GetResources message for root URI
            var context = new ContextInfo
            {
                Uri = EtpUri.RootUri12,
                DataObjectTypes = new List<string>(),
            };
            handler.GetResources(context, ContextScopeKind.self);

            // Wait for GetResourcesResponse for top level resources
            var argsRoot = await onGetRootResourcesResponse.WaitAsync();

            Assert.IsNotNull(argsRoot);
            Assert.IsTrue(argsRoot.Response1.Body.Resources?.Count > 0);
            Assert.IsNotNull(argsRoot.Response1.Body.Resources[0].Uri);
        }
    }
}

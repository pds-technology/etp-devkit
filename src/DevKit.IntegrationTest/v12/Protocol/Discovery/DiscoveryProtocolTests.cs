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
            SetUp(TestSettings.WebSocketType, EtpSettings.Etp12SubProtocol);

            // Register protocol handler
            _server.Register<IDiscoveryStore, DiscoveryStore12MockHandler>();

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
            var onGetRootResourcesResponse = HandleAsync<GetResourcesResponse, string>(
                x => handler.OnGetResourcesResponse += x);

            // Send GetResources message for root URI
            handler.GetTreeResources(new ContextInfo
            {
                Uri = EtpUri.RootUri,
                ContentTypes = new List<string>()
            });

            // Wait for GetResourcesResponse for top level resources
            var argsRoot = await onGetRootResourcesResponse.WaitAsync();

            Assert.IsNotNull(argsRoot);
            Assert.IsNotNull(argsRoot.Message.Resources?.FirstOrDefault());
            Assert.IsNotNull(argsRoot.Message.Resources[0].Uri);

            // Register event handler for child resources
            var onGetChildResourcesResponse = HandleAsync<GetResourcesResponse, string>(
                x => handler.OnGetResourcesResponse += x);

            // Send GetResources message for child resources
            var resource = argsRoot.Message.Resources[0];
            handler.GetTreeResources(new ContextInfo
            {
                Uri = resource.Uri,
                ContentTypes = new List<string>()
            });

            // Wait for GetResourcesResponse for child resources
            var argsChild = await onGetChildResourcesResponse.WaitAsync();

            Assert.IsNotNull(argsChild);

            if (argsChild.Header.IsNoData())
            {
                Assert.IsNull(argsChild.Message.Resources?.FirstOrDefault());
            }
            else
            {
                Assert.IsNotNull(argsChild.Message.Resources?.FirstOrDefault());
                Assert.AreNotEqual(resource.Uri, argsChild.Message.Resources[0].Uri);
            }
        }
    }
}

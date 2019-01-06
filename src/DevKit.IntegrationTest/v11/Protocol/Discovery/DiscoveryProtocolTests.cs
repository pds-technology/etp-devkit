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

using System.Threading.Tasks;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Energistics.Etp.v11.Protocol.Discovery
{
    [TestClass]
    public class DiscoveryProtocolTests : IntegrationTestBase
    {
        [TestInitialize]
        public void TestSetUp()
        {
            SetUp(TestSettings.WebSocketType, EtpSettings.Etp11SubProtocol);

            // Register protocol handler
            _server.Register<IDiscoveryStore, DiscoveryStore11MockHandler>();

            _server.Start();
        }

        [TestCleanup]
        public void TestTearDown()
        {
            CleanUp();
        }

        [TestMethod]
        public async Task IDiscoveryCustomer_v11_GetResource_Request_Default_Uri()
        {
            var handler = _client.Handler<IDiscoveryCustomer>();

            // Wait for Open connection
            var isOpen = await _client.OpenAsyncWithTimeout();
            Assert.IsTrue(isOpen);

            // Register event handler for root URI
            var onGetRootResourcesResponse = HandleAsync<GetResourcesResponse, string>(
                x => handler.OnGetResourcesResponse += x);

            // Send GetResources message for root URI
            handler.GetResources(EtpUri.RootUri);

            // Wait for GetResourcesResponse for top level resources
            var argsRoot = await onGetRootResourcesResponse.WaitAsync();

            Assert.IsNotNull(argsRoot);
            Assert.IsNotNull(argsRoot.Message.Resource);
            Assert.IsNotNull(argsRoot.Message.Resource.Uri);

            // Register event handler for child resources
            var onGetChildResourcesResponse = HandleAsync<GetResourcesResponse, string>(
                x => handler.OnGetResourcesResponse += x);

            // Send GetResources message for child resources
            var resource = argsRoot.Message.Resource;
            handler.GetResources(resource.Uri);

            // Wait for GetResourcesResponse for child resources
            var argsChild = await onGetChildResourcesResponse.WaitAsync();

            Assert.IsNotNull(argsChild);

            if (argsChild.Header.IsNoData())
            {
                Assert.IsNull(argsChild.Message.Resource);
            }
            else
            {
                Assert.IsNotNull(argsChild.Message.Resource);
                Assert.AreNotEqual(resource.Uri, argsChild.Message.Resource.Uri);
            }
        }
    }
}

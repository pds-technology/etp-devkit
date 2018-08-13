//----------------------------------------------------------------------- 
// ETP DevKit, 1.2
//
// Copyright 2018 Energistics
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
using Energistics.Etp.Properties;
using Energistics.Etp.Security;
using Energistics.Etp.v11.Protocol.ChannelStreaming;
using Energistics.Etp.v11.Protocol.Core;
using Energistics.Etp.v11.Protocol.Discovery;
using Energistics.Etp.v11.Protocol.Store;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Energistics.Etp
{
    [TestClass]
    public class EtpClientTests
    {
        private const string AppName = "EtpClientTests";
        private const string AppVersion = "1.0";

        [TestMethod]
        public async Task EtpClient_Opens_WebSocket_Connection()
        {
            // Create a Basic authorization header dictionary
            var auth = Authorization.Basic(TestSettings.Username, TestSettings.Password);

            // Initialize an EtpClient with a valid Uri, app name and version, and auth header
            using (var client = new EtpClient(TestSettings.ServerUrl, AppName, AppVersion, TestSettings.EtpSubProtocol, auth))
            {
                // Register protocol handlers to be used in later tests
                client.Register<IChannelStreamingConsumer, ChannelStreamingConsumerHandler>();
                client.Register<IDiscoveryCustomer, DiscoveryCustomerHandler>();
                client.Register<IStoreCustomer, StoreCustomerHandler>();

                // Open the connection (uses an async extension method)
                await client.OpenAsync();

                // Assert the current state of the connection
                Assert.IsTrue(client.IsOpen);

                // Explicit Close not needed as the WebSocket connection will be closed
                // automatically after leaving the scope of the using statement
                //client.Close("reason");
            }
        }

        [TestMethod]
        public async Task EtpClient_Connects_Using_Json_Encoding()
        {
            // Create a Basic authorization header dictionary
            var headers = Authorization.Basic(TestSettings.Username, TestSettings.Password);

            // Specify preference for JSON encoding
            headers[EtpSettings.EtpEncodingHeader] = Settings.Default.EtpEncodingJson;

            // Initialize an EtpClient with a valid Uri, app name and version, and headers
            using (var client = new EtpClient(TestSettings.ServerUrl, AppName, AppVersion, TestSettings.EtpSubProtocol, headers))
            {
                // Open the connection (uses an async extension method)
                await client.OpenAsync();

                // Assert the current state of the connection
                Assert.IsTrue(client.IsOpen);
            }
        }
    }
}

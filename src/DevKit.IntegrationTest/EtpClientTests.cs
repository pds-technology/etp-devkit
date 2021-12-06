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
using Energistics.Etp.Properties;
using Energistics.Etp.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Energistics.Etp
{
    [TestClass]
    public class EtpClientTests : IntegrationTestBase
    {
        private const string AppName = "EtpClientTests";
        private const string AppVersion = "1.0";

        [TestMethod]
        public async Task EtpClient_Connects_Using_Json_Encoding()
        {
            _server = CreateServer(TestSettings.WebSocketType);
            _server.Start();

            // Create a Basic authorization header dictionary
            var authorization = Authorization.Basic(TestSettings.Username, TestSettings.Password);

            // Initialize an EtpClient with a valid Uri, app name and version, and headers
            using (_client = CreateClient(TestSettings.WebSocketType, TestSettings.EtpVersion, _server.Uri.ToWebSocketUri().ToString(), authorization: authorization, etpEncoding: EtpEncoding.Json))
            {
                // Open the connection (uses an async extension method)
                await _client.OpenAsyncWithTimeout().ConfigureAwait(false);

                // Assert the current state of the connection
                Assert.IsTrue(_client.IsWebSocketOpen);
            }

            _server.Dispose();
        }
    }
}

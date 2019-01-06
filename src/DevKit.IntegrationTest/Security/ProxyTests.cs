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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Energistics.Etp.Security
{
    [TestClass]
    public class ProxyTests : IntegrationTestBase
    {
        [TestMethod]
        [Description("Native EtpClient connects to web socket server")]
        public async Task ProxyTests_EtpClient_Connects_To_WebSocket_Server_Through_Proxy_Native()
        {
            SetUpWithProxy(Common.Datatypes.WebSocketType.Native, EtpSettings.Etp11SubProtocol);
            _server.Start();

            var proxyServer = _server as EtpSelfHostedProxyWebServer;

            _client.SetProxy(proxyServer.ProxyUri.Host, proxyServer.ProxyUri.Port, TestSettings.ProxyUsername, TestSettings.ProxyPassword);

            var task = new Task<bool>(() => _client.IsOpen);

            var result = await _client.OpenAsync().WaitAsync();

            Assert.IsTrue(result, "EtpClient connection not opened");
            Assert.IsTrue(proxyServer.ProxyAuthenticationSuccessful, "Failed to authenticate with proxy");

            CleanUp();
        }

        [TestMethod]
        [Description("Native EtpClient connects to web socket server")]
        public async Task ProxyTests_EtpClient_Connects_To_WebSocket_Server_Through_Proxy_WebSocket4Net()
        {
            SetUpWithProxy(Common.Datatypes.WebSocketType.WebSocket4Net, EtpSettings.Etp11SubProtocol);
            _server.Start();

            var proxyServer = _server as EtpSelfHostedProxyWebServer;

            _client.SetProxy(proxyServer.ProxyUri.Host, proxyServer.ProxyUri.Port, TestSettings.ProxyUsername, TestSettings.ProxyPassword);

            var task = new Task<bool>(() => _client.IsOpen);

            var result = await _client.OpenAsync().WaitAsync();

            Assert.IsTrue(result, "EtpClient connection not opened");
            Assert.IsTrue(proxyServer.ProxyAuthenticationSuccessful, "Failed to authenticate with proxy");

            CleanUp();
        }
    }
}

//----------------------------------------------------------------------- 
// ETP DevKit, 1.0
//
// Copyright 2016 Petrotechnical Data Systems
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

using System.Linq;
using Energistics.IntegrationTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Energistics
{
    [TestClass]
    public class JsonClientTests
    {
        private static readonly string CapServerUrl = Settings.Default.ServerCapabilitiesUrl;
        private static readonly string AuthTokenUrl = Settings.Default.AuthTokenUrl;
        private static readonly string Username = Settings.Default.Username;
        private static readonly string Password = Settings.Default.Password;

        [TestMethod]
        public void JsonClient_GetServerCapabilities_Using_Basic_Authentication()
        {
            var client = new JsonClient(Username, Password);
            var capServer = client.GetServerCapabilities(CapServerUrl);

            Assert.IsNotNull(capServer);
            Assert.IsNotNull(capServer.SupportedObjects);
            Assert.IsTrue(capServer.SupportedObjects.Any());

            Assert.IsNotNull(capServer.SupportedProtocols);
            Assert.IsTrue(capServer.SupportedProtocols.Any());
        }

        [TestMethod]
        public void JsonClient_GetServerCapabilities_Using_Json_Web_Token()
        {
            var client = new JsonClient(Username, Password);

            var token = client.GetJsonWebToken(AuthTokenUrl);
            Assert.IsNotNull(token);

            var capServer = client.GetServerCapabilities(CapServerUrl);

            Assert.IsNotNull(capServer);
            Assert.IsNotNull(capServer.SupportedObjects);
            Assert.IsTrue(capServer.SupportedObjects.Any());

            Assert.IsNotNull(capServer.SupportedProtocols);
            Assert.IsTrue(capServer.SupportedProtocols.Any());
        }
    }
}

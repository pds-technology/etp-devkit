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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Energistics.Etp
{
    [TestClass]
    public class JsonClientTests
    {
        [TestMethod]
        public void JsonClient_GetServerCapabilities_Using_Basic_Authentication()
        {
            var client = new JsonClient(TestSettings.Username, TestSettings.Password);
            var capServer = client.GetServerCapabilities(TestSettings.ServerCapabilitiesUrl) as v11.Datatypes.ServerCapabilities;

            Assert.IsNotNull(capServer);
            Assert.IsNotNull(capServer.SupportedObjects);
            Assert.IsTrue(capServer.SupportedObjects.Any());

            Assert.IsNotNull(capServer.SupportedProtocols);
            Assert.IsTrue(capServer.SupportedProtocols.Any());
        }

        [TestMethod]
        public void JsonClient_GetServerCapabilities_Using_Json_Web_Token()
        {
            var client = new JsonClient(TestSettings.Username, TestSettings.Password);

            var token = client.GetJsonWebToken(TestSettings.AuthTokenUrl);
            Assert.IsNotNull(token);

            var capServer = client.GetServerCapabilities(TestSettings.ServerCapabilitiesUrl) as v11.Datatypes.ServerCapabilities;

            Assert.IsNotNull(capServer);
            Assert.IsNotNull(capServer.SupportedObjects);
            Assert.IsTrue(capServer.SupportedObjects.Any());

            Assert.IsNotNull(capServer.SupportedProtocols);
            Assert.IsTrue(capServer.SupportedProtocols.Any());
        }
    }
}

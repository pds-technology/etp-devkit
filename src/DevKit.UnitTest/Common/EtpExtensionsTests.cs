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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Energistics.Etp.Common
{
    [TestClass]
    public class EtpExtensionsTests
    {
        [TestMethod]
        public void EtpExtensions_Can_Convert_Uri_To_WebSocket_Uri()
        {
            var expectedWS = new Uri("ws://localhost:8080/api/etp");
            var expectedWSS = new Uri("wss://localhost:8080/api/etp");

            var http = new Uri("http://localhost:8080/api/etp");
            var https = new Uri("https://localhost:8080/api/etp");
            var hTTp = new Uri("hTTp://localhost:8080/api/etp");
            var hTtPs = new Uri("hTtPs://localhost:8080/api/etp");

            Assert.AreEqual(expectedWS, expectedWS.ToWebSocketUri());
            Assert.AreEqual(expectedWSS, expectedWSS.ToWebSocketUri());
            Assert.AreEqual(expectedWS, http.ToWebSocketUri());
            Assert.AreEqual(expectedWSS, https.ToWebSocketUri());
            Assert.AreEqual(expectedWS, hTTp.ToWebSocketUri());
            Assert.AreEqual(expectedWSS, hTtPs.ToWebSocketUri());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EtpExtensions_Rejects_Unexpected_Uri_Scheme()
        {
            var ftp = new Uri("ftp://localhost:8080/api/etp").ToWebSocketUri();
        }

        [TestMethod]
        public void EtpExtensions_CreateMessageKey_Creates_Core_Key()
        {
            long expectedKey, actualKey;

            expectedKey = 1L;
            actualKey = EtpExtensions.CreateMessageKey((int)v11.Protocols.Core, (int)v11.MessageTypes.Core.RequestSession);
            Assert.AreEqual(expectedKey, actualKey);

            expectedKey = 1L;
            actualKey = EtpExtensions.CreateMessageKey((int)v12.Protocols.Core, (int)v12.MessageTypes.Core.RequestSession);
            Assert.AreEqual(expectedKey, actualKey);

            expectedKey = 1000L;
            actualKey = EtpExtensions.CreateMessageKey((int)v11.Protocols.Core, (int)v11.MessageTypes.Core.ProtocolException);
            Assert.AreEqual(expectedKey, actualKey);

            expectedKey = 1000L;
            actualKey = EtpExtensions.CreateMessageKey((int)v12.Protocols.Core, (int)v12.MessageTypes.Core.ProtocolException);
            Assert.AreEqual(expectedKey, actualKey);

            expectedKey = 1001L;
            actualKey = EtpExtensions.CreateMessageKey((int)v11.Protocols.Core, (int)v11.MessageTypes.Core.Acknowledge);
            Assert.AreEqual(expectedKey, actualKey);

            expectedKey = 1001L;
            actualKey = EtpExtensions.CreateMessageKey((int)v12.Protocols.Core, (int)v12.MessageTypes.Core.Acknowledge);
            Assert.AreEqual(expectedKey, actualKey);
        }

        [TestMethod]
        public void EtpExtensions_CreateMessageKey_Creates_Discovery_Key()
        {
            long expectedKey, actualKey;

            expectedKey = (3L << 32) + 1;
            actualKey = EtpExtensions.CreateMessageKey((int)v11.Protocols.Discovery, (int)v11.MessageTypes.Discovery.GetResources);
            Assert.AreEqual(expectedKey, actualKey);

            expectedKey = (3L << 32) + 5;
            actualKey = EtpExtensions.CreateMessageKey((int)v12.Protocols.Discovery, (int)v12.MessageTypes.Discovery.GetTreeResources);
            Assert.AreEqual(expectedKey, actualKey);

            expectedKey = (3L << 32) + 6;
            actualKey = EtpExtensions.CreateMessageKey((int)v12.Protocols.Discovery, (int)v12.MessageTypes.Discovery.GetGraphResources);
            Assert.AreEqual(expectedKey, actualKey);

            expectedKey = 1000L;
            actualKey = EtpExtensions.CreateMessageKey((int)v11.Protocols.Discovery, (int)v11.MessageTypes.Core.ProtocolException);
            Assert.AreEqual(expectedKey, actualKey);

            expectedKey = 1000L;
            actualKey = EtpExtensions.CreateMessageKey((int)v12.Protocols.Discovery, (int)v12.MessageTypes.Core.ProtocolException);
            Assert.AreEqual(expectedKey, actualKey);

            expectedKey = 1001L;
            actualKey = EtpExtensions.CreateMessageKey((int)v11.Protocols.Discovery, (int)v11.MessageTypes.Core.Acknowledge);
            Assert.AreEqual(expectedKey, actualKey);

            expectedKey = 1001L;
            actualKey = EtpExtensions.CreateMessageKey((int)v12.Protocols.Discovery, (int)v12.MessageTypes.Core.Acknowledge);
            Assert.AreEqual(expectedKey, actualKey);
        }
    }
}

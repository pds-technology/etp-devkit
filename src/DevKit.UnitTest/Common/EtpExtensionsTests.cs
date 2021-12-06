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

using Energistics.Avro.Encoding;
using Energistics.Avro.Encoding.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

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

            expectedKey = (3L << 32) + 1;
            actualKey = EtpExtensions.CreateMessageKey((int)v12.Protocols.Discovery, (int)v12.MessageTypes.Discovery.GetResources);
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

        [TestMethod]
        public void EtpExtensions_Encode_And_Decode_Can_Round_Trip()
        {
            var expectedVersion = new v11.Datatypes.Version { Major = 1, Minor = 2, Revision = 3, Patch = 4 };
            var bytes = expectedVersion.EncodeToBytes();
            var actualVersion = EtpExtensions.DecodeFromBytes<v11.Datatypes.Version>(bytes);

            Assert.AreEqual(expectedVersion.Major, actualVersion.Major, nameof(expectedVersion.Major));
            Assert.AreEqual(expectedVersion.Minor, actualVersion.Minor, nameof(expectedVersion.Minor));
            Assert.AreEqual(expectedVersion.Revision, actualVersion.Revision, nameof(expectedVersion.Revision));
            Assert.AreEqual(expectedVersion.Patch, actualVersion.Patch, nameof(expectedVersion.Patch));

            var expectedValue = new v11.Datatypes.DataValue { Item = 1 };
            bytes = expectedValue.EncodeToBytes();
            var actualValue = EtpExtensions.DecodeFromBytes<v11.Datatypes.DataValue>(bytes);

            Assert.AreEqual(expectedValue.Item, actualValue.Item, nameof(expectedValue.Item));

            expectedValue = new v11.Datatypes.DataValue { Item = 1.2 };
            bytes = expectedValue.EncodeToBytes();
            actualValue = EtpExtensions.DecodeFromBytes<v11.Datatypes.DataValue>(bytes);

            Assert.AreEqual(expectedValue.Item, actualValue.Item, nameof(expectedValue.Item));

            expectedValue = new v11.Datatypes.DataValue { Item = "test" };
            bytes = expectedValue.EncodeToBytes();
            actualValue = EtpExtensions.DecodeFromBytes<v11.Datatypes.DataValue>(bytes);

            Assert.AreEqual(expectedValue.Item, actualValue.Item, nameof(expectedValue.Item));

            var expectedProtocol = new v11.Datatypes.SupportedProtocol { Protocol = 1, ProtocolVersion = expectedVersion, ProtocolCapabilities = new Dictionary<string, v11.Datatypes.DataValue> { ["test"] = expectedValue }, Role = "store" };
            bytes = expectedProtocol.EncodeToBytes();
            var actualProtocol = EtpExtensions.DecodeFromBytes<v11.Datatypes.SupportedProtocol>(bytes);

            Assert.AreEqual(expectedProtocol.Protocol, actualProtocol.Protocol, nameof(expectedProtocol.Protocol));
            Assert.AreEqual(expectedProtocol.ProtocolVersion.Major, actualProtocol.ProtocolVersion.Major, nameof(expectedProtocol.ProtocolVersion.Major));
            Assert.AreEqual(expectedProtocol.ProtocolVersion.Minor, actualProtocol.ProtocolVersion.Minor, nameof(expectedProtocol.ProtocolVersion.Minor));
            Assert.AreEqual(expectedProtocol.ProtocolVersion.Revision, actualProtocol.ProtocolVersion.Revision, nameof(expectedProtocol.ProtocolVersion.Revision));
            Assert.AreEqual(expectedProtocol.ProtocolVersion.Patch, actualProtocol.ProtocolVersion.Patch, nameof(expectedProtocol.ProtocolVersion.Patch));
            Assert.AreEqual(expectedProtocol.Role, actualProtocol.Role, nameof(expectedProtocol.Role));
            Assert.AreEqual(1, actualProtocol.ProtocolCapabilities.Count, nameof(expectedProtocol.ProtocolCapabilities.Count));
            Assert.AreEqual(expectedValue.Item, actualProtocol.ProtocolCapabilities["test"].Item);

            var expectedRequestSession = new v11.Protocol.Core.RequestSession
            {
                ApplicationName = "Application",
                ApplicationVersion = "Version",
                RequestedProtocols = new List<v11.Datatypes.SupportedProtocol> { expectedProtocol },
                SupportedObjects = new List<string> { "object" },
            };
            bytes = expectedRequestSession.EncodeToBytes();
            var actualRequestSession = EtpExtensions.DecodeFromBytes<v11.Protocol.Core.RequestSession>(bytes);

            Assert.AreEqual(expectedRequestSession.ApplicationName, actualRequestSession.ApplicationName, nameof(expectedRequestSession.ApplicationName));
            Assert.AreEqual(expectedRequestSession.ApplicationVersion, actualRequestSession.ApplicationVersion, nameof(expectedRequestSession.ApplicationVersion));
            Assert.AreEqual(expectedRequestSession.RequestedProtocols.Count, actualRequestSession.RequestedProtocols.Count, nameof(expectedRequestSession.RequestedProtocols.Count));
            Assert.AreEqual(expectedRequestSession.RequestedProtocols[0].Protocol, actualRequestSession.RequestedProtocols[0].Protocol);
            Assert.AreEqual(expectedRequestSession.RequestedProtocols[0].ProtocolVersion.Major, actualRequestSession.RequestedProtocols[0].ProtocolVersion.Major);
            Assert.AreEqual(expectedRequestSession.RequestedProtocols[0].ProtocolVersion.Minor, actualRequestSession.RequestedProtocols[0].ProtocolVersion.Minor);
            Assert.AreEqual(expectedRequestSession.RequestedProtocols[0].ProtocolVersion.Revision, actualRequestSession.RequestedProtocols[0].ProtocolVersion.Revision);
            Assert.AreEqual(expectedRequestSession.RequestedProtocols[0].ProtocolVersion.Patch, actualRequestSession.RequestedProtocols[0].ProtocolVersion.Patch);
            Assert.AreEqual(expectedRequestSession.RequestedProtocols[0].Role, actualRequestSession.RequestedProtocols[0].Role);
            Assert.AreEqual(1, actualRequestSession.RequestedProtocols[0].ProtocolCapabilities.Count);
            Assert.AreEqual(expectedValue.Item, actualProtocol.ProtocolCapabilities["test"].Item);
            Assert.AreEqual(expectedRequestSession.SupportedObjects.Count, actualRequestSession.SupportedObjects.Count, nameof(expectedRequestSession.SupportedObjects.Count));
            Assert.AreEqual(expectedRequestSession.SupportedObjects[0], actualRequestSession.SupportedObjects[0]);

            var expectedHeader = new v11.Datatypes.MessageHeader { MessageFlags = 0, Protocol = 1, MessageId = 2, CorrelationId = 3, MessageType = 4 };
            var expectedMessage = new EtpMessage<v11.Protocol.Core.RequestSession>(expectedHeader, expectedRequestSession);
            bytes = expectedMessage.Encode();

            using (var inputStream = new MemoryStream(bytes))
            using (var decoder = new BinaryAvroDecoder(inputStream))
            {
                var actualHeader = decoder.DecodeAvroObject<v11.Datatypes.MessageHeader>();
                var actualBody = decoder.DecodeAvroObject<v11.Protocol.Core.RequestSession>();

                Assert.AreEqual(expectedHeader.MessageFlags, actualHeader.MessageFlags, nameof(expectedHeader.MessageFlags));
                Assert.AreEqual(expectedHeader.Protocol, actualHeader.Protocol, nameof(expectedHeader.Protocol));
                Assert.AreEqual(expectedHeader.MessageId, actualHeader.MessageId, nameof(expectedHeader.MessageId));
                Assert.AreEqual(expectedHeader.CorrelationId, actualHeader.CorrelationId, nameof(expectedHeader.CorrelationId));
                Assert.AreEqual(expectedHeader.MessageType, actualHeader.MessageType, nameof(expectedHeader.MessageType));

                Assert.AreEqual(expectedRequestSession.ApplicationName, actualBody.ApplicationName, nameof(expectedRequestSession.ApplicationName));
                Assert.AreEqual(expectedRequestSession.ApplicationVersion, actualBody.ApplicationVersion, nameof(expectedRequestSession.ApplicationVersion));
                Assert.AreEqual(expectedRequestSession.RequestedProtocols.Count, actualBody.RequestedProtocols.Count, nameof(expectedRequestSession.RequestedProtocols.Count));
                Assert.AreEqual(expectedRequestSession.RequestedProtocols[0].Protocol, actualBody.RequestedProtocols[0].Protocol);
                Assert.AreEqual(expectedRequestSession.RequestedProtocols[0].ProtocolVersion.Major, actualBody.RequestedProtocols[0].ProtocolVersion.Major);
                Assert.AreEqual(expectedRequestSession.RequestedProtocols[0].ProtocolVersion.Minor, actualBody.RequestedProtocols[0].ProtocolVersion.Minor);
                Assert.AreEqual(expectedRequestSession.RequestedProtocols[0].ProtocolVersion.Revision, actualBody.RequestedProtocols[0].ProtocolVersion.Revision);
                Assert.AreEqual(expectedRequestSession.RequestedProtocols[0].ProtocolVersion.Patch, actualBody.RequestedProtocols[0].ProtocolVersion.Patch);
                Assert.AreEqual(expectedRequestSession.RequestedProtocols[0].Role, actualBody.RequestedProtocols[0].Role);
                Assert.AreEqual(1, actualBody.RequestedProtocols[0].ProtocolCapabilities.Count);
                Assert.AreEqual(expectedValue.Item, actualProtocol.ProtocolCapabilities["test"].Item);
                Assert.AreEqual(expectedRequestSession.SupportedObjects.Count, actualBody.SupportedObjects.Count, nameof(expectedRequestSession.SupportedObjects.Count));
                Assert.AreEqual(expectedRequestSession.SupportedObjects[0], actualBody.SupportedObjects[0]);
            }
        }
    }
}

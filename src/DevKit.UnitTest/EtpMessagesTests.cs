﻿//----------------------------------------------------------------------- 
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

using Energistics.Etp.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Energistics.Etp
{
    [TestClass]
    public class EtpMessagesTests
    {
        [TestMethod]
        public void EtpMessages_v11_MessageTypes_And_Protocols_Match_Avro_Schemas()
        {
            /*var assembly = typeof(v11.Protocols).Assembly;

            // Get the message types defined in the appropriate namespace.
            var messageTypes = assembly.GetExportedTypes().Where(type =>
                typeof(IEtpMessageBody).IsAssignableFrom(type) && type.Namespace.StartsWith(typeof(v11.Protocol.IEtp11ProtocolHandler).Namespace)).ToList();

            var protocols = typeof(v11.Protocols);
            var messageTypesType = typeof(v11.MessageTypes);
            var messageTypeMap = messageTypesType.GetNestedTypes().ToDictionary(t => t.Name);

            foreach (var messageType in messageTypes)
            {
                var protocolName = messageType.Namespace.Split('.').Last();

                var protocolNumber = Convert.ToInt32(Enum.Parse(protocols, protocolName));
                var messageTypeNumber = Convert.ToInt32(Enum.Parse(messageTypeMap[protocolName], messageType.Name));

                var schema = (Schema)messageType.GetField("_SCHEMA").GetValue(null);
                Assert.AreEqual(schema.GetProperty("protocol"), $"\"{protocolNumber}\"");
                Assert.AreEqual(schema.GetProperty("messageType"), $"\"{messageTypeNumber}\"");

                var names = schema.GetProperty("fullName").Replace("\"", string.Empty).Split('.');

                // Check type protocol and message name.
                Assert.AreEqual(names[names.Length - 2], protocolName);
                Assert.AreEqual(names[names.Length - 1], messageType.Name);

                // Check dictionary lookup of names and values.
                Assert.AreEqual(names[names.Length - 2], v11.ProtocolNames.GetProtocolName(protocolNumber));
                Assert.AreEqual(names[names.Length - 1], v11.MessageNames.GetMessageName(protocolNumber, messageTypeNumber));

                // Check dictionary lookup of names and values.
                Assert.AreEqual(protocolNumber, v11.MessageReflection.TryGetProtocolNumber(messageType));
                Assert.AreEqual(messageTypeNumber, v11.MessageReflection.TryGetMessageTypeNumber(messageType));
            }*/
        }

        [TestMethod]
        public void EtpMessages_v12_MessageTypes_And_Protocols_Match_Avro_Schemas()
        {
            /*var assembly = typeof(v12.Protocols).Assembly;

            // Get the message types defined in the appropriate namespace.
            var messageTypes = assembly.GetExportedTypes().Where(type =>
                typeof(IEtpMessageBody).IsAssignableFrom(type) && type.Namespace.StartsWith(typeof(v12.Protocol.IEtp12ProtocolHandler).Namespace)).ToList();

            var protocols = typeof(v12.Protocols);
            var messageTypesType = typeof(v12.MessageTypes);
            var messageTypeMap = messageTypesType.GetNestedTypes().ToDictionary(t => t.Name);

            foreach (var messageType in messageTypes)
            {
                var protocolName = messageType.Namespace.Split('.').Last();

                var protocolNumber = Convert.ToInt32(Enum.Parse(protocols, protocolName));
                var messageTypeNumber = Convert.ToInt32(Enum.Parse(messageTypeMap[protocolName], messageType.Name));

                var schema = (Schema)messageType.GetField("_SCHEMA").GetValue(null);
                Assert.AreEqual(schema.GetProperty("protocol"), $"\"{protocolNumber}\"");
                Assert.AreEqual(schema.GetProperty("messageType"), $"\"{messageTypeNumber}\"");

                var names = schema.GetProperty("fullName").Replace("\"", string.Empty).Split('.');

                // Check type protocol and message name.
                Assert.AreEqual(names[names.Length - 2], protocolName);
                Assert.AreEqual(names[names.Length - 1], messageType.Name);

                // Check dictionary lookup of names and values.
                Assert.AreEqual(names[names.Length - 2], v12.ProtocolNames.GetProtocolName(protocolNumber));
                Assert.AreEqual(names[names.Length - 1], v12.MessageNames.GetMessageName(protocolNumber, messageTypeNumber));

                // Check dictionary lookup of names and values.
                Assert.AreEqual(protocolNumber, v12.MessageReflection.TryGetProtocolNumber(messageType));
                Assert.AreEqual(messageTypeNumber, v12.MessageReflection.TryGetMessageTypeNumber(messageType));
            }*/
        }
    }
}

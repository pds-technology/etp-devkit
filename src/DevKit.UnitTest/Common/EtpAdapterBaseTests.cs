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

using Avro.Specific;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Energistics.Etp.Common
{
    [TestClass]
    public class EtpAdapterBaseTests
    {
        [TestMethod]
        public void EtpAdapterBase_Registers_All_Message_Types_By_Type()
        {
            var etp11 = new v11.Etp11Adapter();
            var etp12 = new v12.Etp12Adapter();

            var assembly = typeof(v11.Protocols).Assembly;

            // Get the message types defined in the appropriate namespace.
            var messageTypes = assembly.GetExportedTypes().Where(type =>
                typeof(ISpecificRecord).IsAssignableFrom(type)).ToList();

            var register11 = ((Func<bool>)etp11.IsMessageDecoderRegistered<ISpecificRecord>).Method.GetGenericMethodDefinition();
            var register12 = ((Func<bool>)etp12.IsMessageDecoderRegistered<ISpecificRecord>).Method.GetGenericMethodDefinition();

            var unregistered = new List<string>();

            int count = 0;
            foreach (var messageType in messageTypes)
            {
                if (messageType.Namespace.StartsWith(typeof(v11.Protocol.IEtp11ProtocolHandler).Namespace))
                {
                    count++;
                    var genericRegister11 = register11.MakeGenericMethod(messageType);
                    var registered11 = (bool)genericRegister11.Invoke(etp11, new object[] { });
                    if (!registered11)
                        unregistered.Add(messageType.FullName);
                }
                else if (messageType.Namespace.StartsWith(typeof(v12.Protocol.IEtp12ProtocolHandler).Namespace))
                {
                    count++;
                    var genericRegister12 = register12.MakeGenericMethod(messageType);
                    var registered12 = (bool)genericRegister12.Invoke(etp12, new object[] { });

                    if (!registered12)
                        unregistered.Add(messageType.FullName);
                }
            }

            Assert.IsTrue(count > 0, "Did not find any message types");
            Assert.IsTrue(unregistered.Count == 0, string.Join(", ", unregistered));
        }

        [TestMethod]
        public void EtpAdapterBase_Registers_All_v11_Message_Types_By_Protocol_And_Message_ID()
        {
            var etp11 = new v11.Etp11Adapter();

            var assembly = typeof(v11.Protocols).Assembly;

            var protocols = typeof(v11.Protocols);
            var messageTypeEnumTypes = typeof(v11.MessageTypes).GetNestedTypes();

            var unregistered = new List<string>();

            foreach (var messageTypeEnumType in messageTypeEnumTypes)
            {
                var protocol = Enum.Parse(protocols, messageTypeEnumType.Name);

                foreach (var name in Enum.GetNames(messageTypeEnumType))
                {
                    var messageType = Enum.Parse(messageTypeEnumType, name);

                    if (!etp11.IsMessageDecoderRegistered(protocol, messageType))
                        unregistered.Add(messageTypeEnumType.FullName + "." + name);
                }

            }

            Assert.IsTrue(unregistered.Count == 0, string.Join(", ", unregistered));
        }

        [TestMethod]
        public void EtpAdapterBase_Registers_All_v12_Message_Types_By_Protocol_And_Message_ID()
        {
            var etp12 = new v12.Etp12Adapter();

            var assembly = typeof(v12.Protocols).Assembly;

            var protocols = typeof(v12.Protocols);
            var messageTypeEnumTypes = typeof(v12.MessageTypes).GetNestedTypes();

            var unregistered = new List<string>();

            foreach (var messageTypeEnumType in messageTypeEnumTypes)
            {
                var protocol = Enum.Parse(protocols, messageTypeEnumType.Name);

                foreach (var name in Enum.GetNames(messageTypeEnumType))
                {
                    var messageType = Enum.Parse(messageTypeEnumType, name);

                    if (!etp12.IsMessageDecoderRegistered(protocol, messageType))
                        unregistered.Add(messageTypeEnumType.FullName + "." + name);
                }

            }

            Assert.IsTrue(unregistered.Count == 0, string.Join(", ", unregistered));
        }
    }
}

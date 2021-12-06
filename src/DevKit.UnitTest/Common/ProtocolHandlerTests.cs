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
using System.Linq;
using System.Collections.Generic;
using Energistics.Etp.Common.Datatypes;
using System.Reflection;

namespace Energistics.Etp.Common
{
    [TestClass]
    public class ProtocolHandlerTests
    {
        [TestMethod]
        public void ProtocolHandlerTests_v11_Handler_Sanity_Check()
        {
            var handlersAssembly = typeof(EtpSession).Assembly;

            var handlerInterfaceTypes = handlersAssembly.GetExportedTypes().Where(type =>
                typeof(IProtocolHandler).IsAssignableFrom(type) && type.IsInterface && type.Namespace.StartsWith(typeof(v11.Protocol.IEtp11ProtocolHandler).Namespace + ".")).ToList();

            var protocolCount = Enum.GetNames(typeof(v11.Protocols)).Length;
            Assert.AreEqual(protocolCount * 2, handlerInterfaceTypes.Count);

            var implementationTypes = new Dictionary<Type, Type>();

            var interfacesByProtocol = new Dictionary<int, List<Type>>();
            foreach (var value in Enum.GetValues(typeof(v11.Protocols)))
                interfacesByProtocol[(int)value] = new List<Type>();

            foreach (var handlerInterfaceType in handlerInterfaceTypes)
            {
                var attributes = handlerInterfaceType.GetCustomAttributes(typeof(ProtocolRoleAttribute), false);
                Assert.AreEqual(1, attributes.Length);
                Assert.IsTrue(attributes[0] is ProtocolRoleAttribute);

                var attribute = attributes[0] as ProtocolRoleAttribute;
                Assert.IsTrue(interfacesByProtocol.ContainsKey(attribute.Protocol));

                Assert.AreEqual($"I{v11.ProtocolNames.GetProtocolName(attribute.Protocol)}{attribute.Role.Substring(0, 1).ToUpperInvariant()}{attribute.Role.Substring(1)}", handlerInterfaceType.Name);
                Assert.IsTrue(Roles.IsRoleRegistered(attribute.Role));
                Assert.IsTrue(Roles.IsRoleRegistered(attribute.CounterpartRole));
                Assert.AreEqual(Roles.GetCounterpartRole(attribute.Role), attribute.CounterpartRole);

                interfacesByProtocol[attribute.Protocol].Add(handlerInterfaceType);

                var implementationTypeList = handlersAssembly.GetExportedTypes().Where(type => handlerInterfaceType.IsAssignableFrom(type) && !type.IsInterface).ToList();
                Assert.AreEqual(1, implementationTypeList.Count);

                var implementationType = implementationTypeList[0];
                Assert.AreEqual($"{v11.ProtocolNames.GetProtocolName(attribute.Protocol)}{attribute.Role.Substring(0, 1).ToUpperInvariant()}{attribute.Role.Substring(1)}Handler", implementationType.Name);

                var instance = Activator.CreateInstance(implementationType) as IProtocolHandler;
                Assert.IsNotNull(instance);

                Assert.AreEqual(attribute.Protocol, instance.Protocol);
                Assert.AreEqual(attribute.Role, instance.Role);
                Assert.AreEqual(attribute.CounterpartRole, instance.CounterpartRole);
            }

            Assert.AreEqual(protocolCount, interfacesByProtocol.Count);
            foreach (var list in interfacesByProtocol.Values)
            {
                Assert.IsTrue(list.Count == 2);

                var first = list[0];
                var second = list[1];

                var firstAttribute = (ProtocolRoleAttribute)first.GetCustomAttributes(typeof(ProtocolRoleAttribute), false)[0];
                var secondAttribute = (ProtocolRoleAttribute)second.GetCustomAttributes(typeof(ProtocolRoleAttribute), false)[0];

                Assert.AreEqual(Roles.GetCounterpartRole(firstAttribute.Role), secondAttribute.Role);
                Assert.AreEqual(Roles.GetCounterpartRole(firstAttribute.CounterpartRole), secondAttribute.CounterpartRole);
                Assert.AreEqual(Roles.GetCounterpartRole(secondAttribute.Role), firstAttribute.Role);
                Assert.AreEqual(Roles.GetCounterpartRole(secondAttribute.CounterpartRole), firstAttribute.CounterpartRole);
            }
        }

        [TestMethod]
        public void ProtocolHandlerTests_v11_Handler_Message_Sanity_Check()
        {
            /*var messagesAssembly = typeof(v11.Protocols).Assembly;

            // Get the message types defined in the appropriate namespace.
            var messageTypes = messagesAssembly.GetExportedTypes().Where(type =>
                typeof(IEtpMessageBody).IsAssignableFrom(type) && type.Namespace.StartsWith(typeof(v11.Protocol.IEtp11ProtocolHandler).Namespace)).ToList();

            var protocols = typeof(v11.Protocols);
            var messageTypesType = typeof(v11.MessageTypes);
            var messageTypeMap = messageTypesType.GetNestedTypes().ToDictionary(t => t.Name);

            var handlersAssembly = typeof(EtpSession).Assembly;

            var handlerInterfaceTypes = handlersAssembly.GetExportedTypes().Where(type =>
                typeof(IProtocolHandler).IsAssignableFrom(type) && type.IsInterface && type.Namespace.StartsWith(typeof(v11.Protocol.IEtp11ProtocolHandler).Namespace + ".")).ToList();

            var handlerImplementationTypes = handlerInterfaceTypes.Select(t => handlersAssembly.GetExportedTypes().Where(type => t.IsAssignableFrom(type) && !type.IsInterface).First()).ToList();

            var handlerInstances = new Dictionary<int, Dictionary<string, IProtocolHandler>>();
            foreach (var handlerImplementationType in handlerImplementationTypes)
            {
                var instance = Activator.CreateInstance(handlerImplementationType) as IProtocolHandler;
                Dictionary<string, IProtocolHandler> handlersByRole;
                if (!handlerInstances.TryGetValue(instance.Protocol, out handlersByRole))
                {
                    handlersByRole = new Dictionary<string, IProtocolHandler>();
                    handlerInstances[instance.Protocol] = handlersByRole;
                }

                handlersByRole[instance.Role] = instance;
            }

            var ignoredMessages = Enum.GetNames(typeof(MessageTypes.Core)); // messages handled by session itself.

            foreach (var messageType in messageTypes)
            {
                var protocolName = messageType.Namespace.Split('.').Last();

                var protocolNumber = Convert.ToInt32(Enum.Parse(protocols, protocolName));
                var messageTypeNumber = Convert.ToInt32(Enum.Parse(messageTypeMap[protocolName], messageType.Name));

                var schema = (Schema)messageType.GetField("_SCHEMA").GetValue(null);
                var senderRoles = schema.GetProperty("senderRole").Replace("\"", string.Empty).Split(',');
                foreach (var role in senderRoles)
                {
                    if (role == "*") // Acknowledge / ProtocolException
                    {
                        foreach (var sender in handlerInstances.Values.SelectMany(d => d.Values))
                        {
                            var receiver = sender;
                            Assert.IsTrue(receiver.CanHandleMessage(messageType), $"{receiver.GetType().Name}: {protocolName}.{messageType.Name}");
                            Assert.IsTrue(sender.GetType().GetMethods().Any(mi => mi.Name.EndsWith(messageType.Name)), $"Sender send methods: {sender.GetType().Name}: {protocolName}.{messageType.Name}");
                            Assert.IsTrue(receiver.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Any(mi => mi.Name.StartsWith("Handle") && mi.Name.EndsWith(messageType.Name)), $"Receiver Handle methods: {receiver.GetType().Name}: {protocolName}.{messageType.Name}");
                            if (messageType.Name != "Acknowledge" && messageType.Name != "ProtocolException")
                                Assert.IsTrue(receiver.GetType().GetEvents().Any(ei => ei.Name.StartsWith("On") && ei.EventHandlerType.GenericTypeArguments[0].GenericTypeArguments.Any(ta => ta == messageType)), $"Receiver events: {receiver.GetType().Name}: {protocolName}.{messageType.Name}");
                        }
                    }
                    else
                    {
                        if (ignoredMessages.Contains(messageType.Name))
                            continue;

                        Assert.IsTrue(handlerInstances.ContainsKey(protocolNumber));
                        Assert.IsTrue(handlerInstances[protocolNumber].ContainsKey(role));
                        Assert.IsTrue(handlerInstances[protocolNumber].ContainsKey(Roles.GetCounterpartRole(role)));
                        var sender = handlerInstances[protocolNumber][role];
                        var receiver = handlerInstances[protocolNumber][Roles.GetCounterpartRole(role)];

                        Assert.IsTrue(receiver.CanHandleMessage(messageType), $"{receiver.GetType().Name}: {protocolName}.{messageType.Name}");
                        Assert.IsTrue(sender.GetType().GetMethods().Any(mi => mi.Name.EndsWith(messageType.Name)), $"Sender send methods: {sender.GetType().Name}: {protocolName}.{messageType.Name}");
                        Assert.IsTrue(receiver.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Any(mi => mi.Name.StartsWith("Handle") && mi.Name.EndsWith(messageType.Name)), $"Receiver Handle methods: {receiver.GetType().Name}: {protocolName}.{messageType.Name}");
                        Assert.IsTrue(receiver.GetType().GetEvents().Any(ei => ei.Name.StartsWith("On") && ei.EventHandlerType.GenericTypeArguments[0].GenericTypeArguments.Any(ta => ta == messageType)), $"Receiver events: {receiver.GetType().Name}: {protocolName}.{messageType.Name}");
                    }
                }
            }*/
        }
        
        [TestMethod]
        public void ProtocolHandlerTests_v12_Handler_Sanity_Check()
        {
            var handlersAssembly = typeof(EtpSession).Assembly;

            var handlerInterfaceTypes = handlersAssembly.GetExportedTypes().Where(type =>
                typeof(IProtocolHandler).IsAssignableFrom(type) && type.IsInterface && type.Namespace.StartsWith(typeof(v12.Protocol.IEtp12ProtocolHandler).Namespace + ".")).ToList();

            var protocolCount = Enum.GetNames(typeof(v12.Protocols)).Length;
            Assert.AreEqual(protocolCount * 2, handlerInterfaceTypes.Count);

            var implementationTypes = new Dictionary<Type, Type>();

            var interfacesByProtocol = new Dictionary<int, List<Type>>();
            foreach (var value in Enum.GetValues(typeof(v12.Protocols)))
                interfacesByProtocol[(int)value] = new List<Type>();

            foreach (var handlerInterfaceType in handlerInterfaceTypes)
            {
                var attributes = handlerInterfaceType.GetCustomAttributes(typeof(ProtocolRoleAttribute), false);
                Assert.AreEqual(1, attributes.Length);
                Assert.IsTrue(attributes[0] is ProtocolRoleAttribute);

                var attribute = attributes[0] as ProtocolRoleAttribute;
                Assert.IsTrue(interfacesByProtocol.ContainsKey(attribute.Protocol));

                Assert.AreEqual($"I{v12.ProtocolNames.GetProtocolName(attribute.Protocol)}{attribute.Role.Substring(0, 1).ToUpperInvariant()}{attribute.Role.Substring(1)}", handlerInterfaceType.Name);
                Assert.IsTrue(Roles.IsRoleRegistered(attribute.Role));
                Assert.IsTrue(Roles.IsRoleRegistered(attribute.CounterpartRole));
                Assert.AreEqual(Roles.GetCounterpartRole(attribute.Role), attribute.CounterpartRole);

                interfacesByProtocol[attribute.Protocol].Add(handlerInterfaceType);

                var implementationTypeList = handlersAssembly.GetExportedTypes().Where(type => handlerInterfaceType.IsAssignableFrom(type) && !type.IsInterface).ToList();
                Assert.AreEqual(1, implementationTypeList.Count);

                var implementationType = implementationTypeList[0];
                Assert.AreEqual($"{v12.ProtocolNames.GetProtocolName(attribute.Protocol)}{attribute.Role.Substring(0, 1).ToUpperInvariant()}{attribute.Role.Substring(1)}Handler", implementationType.Name);

                var instance = Activator.CreateInstance(implementationType) as IProtocolHandler;
                Assert.IsNotNull(instance);

                Assert.AreEqual(attribute.Protocol, instance.Protocol);
                Assert.AreEqual(attribute.Role, instance.Role);
                Assert.AreEqual(attribute.CounterpartRole, instance.CounterpartRole);
            }

            Assert.AreEqual(protocolCount, interfacesByProtocol.Count);
            foreach (var list in interfacesByProtocol.Values)
            {
                Assert.IsTrue(list.Count == 2);

                var first = list[0];
                var second = list[1];

                var firstAttribute = (ProtocolRoleAttribute)first.GetCustomAttributes(typeof(ProtocolRoleAttribute), false)[0];
                var secondAttribute = (ProtocolRoleAttribute)second.GetCustomAttributes(typeof(ProtocolRoleAttribute), false)[0];

                Assert.AreEqual(Roles.GetCounterpartRole(firstAttribute.Role), secondAttribute.Role);
                Assert.AreEqual(Roles.GetCounterpartRole(firstAttribute.CounterpartRole), secondAttribute.CounterpartRole);
                Assert.AreEqual(Roles.GetCounterpartRole(secondAttribute.Role), firstAttribute.Role);
                Assert.AreEqual(Roles.GetCounterpartRole(secondAttribute.CounterpartRole), firstAttribute.CounterpartRole);
            }
        }

        [TestMethod]
        public void ProtocolHandlerTests_v12_Handler_Message_Sanity_Check()
        {
            /*var messagesAssembly = typeof(v12.Protocols).Assembly;

            // Get the message types defined in the appropriate namespace.
            var messageTypes = messagesAssembly.GetExportedTypes().Where(type =>
                typeof(IEtpMessageBody).IsAssignableFrom(type) && type.Namespace.StartsWith(typeof(v12.Protocol.IEtp12ProtocolHandler).Namespace)).ToList();

            var protocols = typeof(v12.Protocols);
            var messageTypesType = typeof(v12.MessageTypes);
            var messageTypeMap = messageTypesType.GetNestedTypes().ToDictionary(t => t.Name);

            var handlersAssembly = typeof(EtpSession).Assembly;

            var handlerInterfaceTypes = handlersAssembly.GetExportedTypes().Where(type =>
                typeof(IProtocolHandler).IsAssignableFrom(type) && type.IsInterface && type.Namespace.StartsWith(typeof(v12.Protocol.IEtp12ProtocolHandler).Namespace + ".")).ToList();

            var handlerImplementationTypes = handlerInterfaceTypes.Select(t => handlersAssembly.GetExportedTypes().Where(type => t.IsAssignableFrom(type) && !type.IsInterface).First()).ToList();

            var handlerInstances = new Dictionary<int, Dictionary<string, IProtocolHandler>>();
            foreach (var handlerImplementationType in handlerImplementationTypes)
            {
                var instance = Activator.CreateInstance(handlerImplementationType) as IProtocolHandler;
                Dictionary<string, IProtocolHandler> handlersByRole;
                if (!handlerInstances.TryGetValue(instance.Protocol, out handlersByRole))
                {
                    handlersByRole = new Dictionary<string, IProtocolHandler>();
                    handlerInstances[instance.Protocol] = handlersByRole;
                }

                handlersByRole[instance.Role] = instance;
            }

            var ignoredMessages = Enum.GetNames(typeof(MessageTypes.Core)); // messages handled by session itself.

            foreach (var messageType in messageTypes)
            {
                var protocolName = messageType.Namespace.Split('.').Last();

                var protocolNumber = Convert.ToInt32(Enum.Parse(protocols, protocolName));
                var messageTypeNumber = Convert.ToInt32(Enum.Parse(messageTypeMap[protocolName], messageType.Name));

                var schema = (Schema)messageType.GetField("_SCHEMA").GetValue(null);
                var senderRoles = schema.GetProperty("senderRole").Replace("\"", string.Empty).Split(',');
                foreach (var role in senderRoles)
                {
                    if (role == "*") // Acknowledge / ProtocolException
                    {
                        foreach (var sender in handlerInstances.Values.SelectMany(d => d.Values))
                        {
                            var receiver = sender;
                            Assert.IsTrue(receiver.CanHandleMessage(messageType), $"{receiver.GetType().Name}: {protocolName}.{messageType.Name}");
                            Assert.IsTrue(sender.GetType().GetMethods().Any(mi => mi.Name.EndsWith(messageType.Name)), $"Sender send methods: {sender.GetType().Name}: {protocolName}.{messageType.Name}");
                            Assert.IsTrue(receiver.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Any(mi => mi.Name.StartsWith("Handle") && mi.Name.EndsWith(messageType.Name)), $"Receiver Handle methods: {receiver.GetType().Name}: {protocolName}.{messageType.Name}");
                            if (messageType.Name != "Acknowledge" && messageType.Name != "ProtocolException")
                                Assert.IsTrue(receiver.GetType().GetEvents().Any(ei => ei.Name.StartsWith("On") && ei.EventHandlerType.GenericTypeArguments[0].GenericTypeArguments.Any(ta => ta == messageType)), $"Receiver events: {receiver.GetType().Name}: {protocolName}.{messageType.Name}");
                        }
                    }
                    else
                    {
                        if (ignoredMessages.Contains(messageType.Name))
                            continue;

                        Assert.IsTrue(handlerInstances.ContainsKey(protocolNumber));
                        Assert.IsTrue(handlerInstances[protocolNumber].ContainsKey(role));
                        Assert.IsTrue(handlerInstances[protocolNumber].ContainsKey(Roles.GetCounterpartRole(role)));
                        var sender = handlerInstances[protocolNumber][role];
                        var receiver = handlerInstances[protocolNumber][Roles.GetCounterpartRole(role)];

                        Assert.IsTrue(receiver.CanHandleMessage(messageType), $"{receiver.GetType().Name}: {protocolName}.{messageType.Name}");
                        Assert.IsTrue(sender.GetType().GetMethods().Any(mi => mi.Name.EndsWith(messageType.Name)), $"Sender send methods: {sender.GetType().Name}: {protocolName}.{messageType.Name}");
                        Assert.IsTrue(receiver.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Any(mi => mi.Name.StartsWith("Handle") && mi.Name.EndsWith(messageType.Name)), $"Receiver Handle methods: {receiver.GetType().Name}: {protocolName}.{messageType.Name}");

                        Assert.IsTrue(receiver.GetType().GetEvents().Any(ei => ei.Name.StartsWith("On") && ei.EventHandlerType.GenericTypeArguments[0].GenericTypeArguments.Any(ta => ta == messageType)), $"Receiver events: {receiver.GetType().Name}: {protocolName}.{messageType.Name}");
                    }
                }
            }*/
        }
    }
}

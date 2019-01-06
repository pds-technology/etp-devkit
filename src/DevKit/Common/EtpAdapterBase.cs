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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avro.IO;
using Avro.Specific;
using Energistics.Etp.Common.Datatypes;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// A base class for <see cref="IEtpAdapter"/> implementations that handles registering message decoders and resolving messages.
    /// </summary>
    public class EtpAdapterBase
    {
        private readonly Dictionary<long, Func<Decoder, string, ISpecificRecord>> MessageDecoders = new Dictionary<long, Func<Decoder, string, ISpecificRecord>>();
        private readonly Dictionary<Type, Func<Decoder, string, ISpecificRecord>> MessageDecodersByType = new Dictionary<Type, Func<Decoder, string, ISpecificRecord>>();

        /// <summary>
        /// Constructs a new <see cref="EtpAdapterBase"/> instance.
        /// </summary>
        /// <param name="version">The supported ETP version.</param>
        protected EtpAdapterBase(EtpVersion version)
        {
            SupportedVersion = version;
            RegisterStandardMessageDecoders();
        }

        public EtpVersion SupportedVersion { get; }

        /// <summary>
        /// Decodes the message type from the specified protocol.
        /// </summary>
        /// <param name="protocol">The message's protocol.</param>
        /// <param name="messageType">The type of the message.</param>
        /// <param name="decoder">The decoder with binary message data.</param>
        /// <param name="body">The string with json message data.</param>
        /// <returns>The message.</returns>
        public ISpecificRecord DecodeMessage(int protocol, int messageType, Decoder decoder, string body)
        {
            var messageKey = EtpExtensions.CreateMessageKey(protocol, messageType);

            Func<Decoder, string, ISpecificRecord> messageDecoder;
            if (!MessageDecoders.TryGetValue(messageKey, out messageDecoder))
                return null;

            return messageDecoder(decoder, body);
        }

        /// <summary>
        /// Decodes the message type from the specified protocol.
        /// </summary>
        /// <param name="decoder">The decoder with binary message data.</param>
        /// <param name="body">The string with json message data.</param>
        /// <returns>The message.</returns>
        public T DecodeMessage<T>(Decoder decoder, string body) where T : ISpecificRecord
        {
            Func<Decoder, string, ISpecificRecord> messageDecoder;
            if (!MessageDecodersByType.TryGetValue(typeof(T), out messageDecoder))
                return default(T);

            return (T)messageDecoder(decoder, body);
        }

        /// <summary>
        /// Registers a function that decodes a particular message type.
        /// </summary>
        /// <typeparam name="T">The type of the decoded message.</typeparam>
        /// <param name="protocol">The message's protocol.</param>
        /// <param name="messageType">The type of the message.</param>
        /// <exception cref="ArgumentException">More than one decoder for the same protocol message is added.</exception>
        public void RegisterMessageDecoder<T>(object protocol, object messageType) where T : ISpecificRecord
        {
            var messageKey = EtpExtensions.CreateMessageKey(Convert.ToInt32(protocol), Convert.ToInt32(messageType));
            if (MessageDecoders.ContainsKey(messageKey))
                throw new ArgumentException($"Duplicate decoder: Protocol: {protocol}; Message Type: {messageType}", "messageType");

            MessageDecoders[messageKey] = (d, b) => EtpExtensions.Decode<T>(d, b);
            MessageDecodersByType[typeof(T)] = MessageDecoders[messageKey];
        }

        /// <summary>
        /// Checks whether a decoder is registered for the specified protocol message.
        /// </summary>
        /// <param name="protocol">The message protocol.</param>
        /// <param name="messageType">The message type.</param>
        /// <returns><c>true</c> if there is a message decoder registered; <c>false</c> otherwise.</returns>
        public bool IsMessageDecoderRegistered(object protocol, object messageType)
        {
            var messageKey = EtpExtensions.CreateMessageKey(Convert.ToInt32(protocol), Convert.ToInt32(messageType));
            return MessageDecoders.ContainsKey(messageKey);
        }

        /// <summary>
        /// Checks whether a decoder is registered for the specified protocol message.
        /// </summary>
        /// <returns><c>true</c> if there is a message decoder registered; <c>false</c> otherwise.</returns>
        public bool IsMessageDecoderRegistered<T>() where T : ISpecificRecord
        {
            return MessageDecodersByType.ContainsKey(typeof(T));
        }
        /// <summary>
        /// Register message decoders for the standard messages that follow the pattern:
        /// Message protocol:    Energistics.Etp.[VERSION].Protocols.[PROTOCOL]
        /// Message type enum:   Energistics.Etp.[VERSION].MessageTypes.[PROTOCOL].[MESSAGE]
        /// Message object type: Energistics.Etp.[VERSION].Protocol.[PROTOCOL].[MESSAGE]
        /// </summary>
        protected void RegisterStandardMessageDecoders()
        {
            var messageNamespacePrefix = $"Energistics.Etp.{SupportedVersion}.Protocol";

            var protocolsEnumType = SupportedVersion == EtpVersion.v11 ? typeof(v11.Protocols) : typeof(v12.Protocols);
            var protocolNames = Enum.GetNames(protocolsEnumType);

            var messageTypesType = SupportedVersion == EtpVersion.v11 ? typeof(v11.MessageTypes) : typeof(v12.MessageTypes);
            var messageTypeMap = messageTypesType.GetNestedTypes().ToDictionary(t => t.Name);
            var messageTypeEnumValuesMap = messageTypeMap.Values.ToDictionary(t => t.Name, t => Enum.GetNames(t));

            var assembly = protocolsEnumType.Assembly;

            // Get the message types defined in the appropriate namespace.
            var messageTypes = assembly.GetExportedTypes().Where(type =>
                type.Namespace.StartsWith(messageNamespacePrefix) &&
                typeof(ISpecificRecord).IsAssignableFrom(type)).ToList();

            // Loops through the message types to find the corresponding Protocols enum value and MessageTypes enum value
            foreach (var messageType in messageTypes)
            {
                // Get the message's protocol enum value Energistics.Etp.[VERSION].Protocols.[PROTOCOL]
                var protocolName = messageType.Namespace.Split('.').Last();
                if (!protocolNames.Contains(protocolName))
                    continue;
                object protocolsEnumValue = Enum.Parse(protocolsEnumType, protocolName);

                // Get the type of the protocol message type enumeration: Energistics.Etp.[VERSION].MessageTypes.[PROTOCOL]
                Type messageTypeEnumType;
                if (!messageTypeMap.TryGetValue(protocolName, out messageTypeEnumType))
                    continue;

                // Get the enumeration value for the message: Energistics.Etp.[VERSION].MessageTypes.[PROTOCOL].[MESSAGE]
                if (!messageTypeEnumValuesMap.ContainsKey(protocolName) || !messageTypeEnumValuesMap[protocolName].Contains(messageType.Name))
                    continue;
                object messageTypeEnumValue = Enum.Parse(messageTypeEnumType, messageType.Name);

                // Call RegisterMessageDecoder with the appropriate parameters.
                var register = ((Action<object, object>)RegisterMessageDecoder<ISpecificRecord>).Method.GetGenericMethodDefinition();
                var genericRegister = register.MakeGenericMethod(messageType);
                genericRegister.Invoke(this, new object[] { protocolsEnumValue, messageTypeEnumValue });
            }
        }
    }
}

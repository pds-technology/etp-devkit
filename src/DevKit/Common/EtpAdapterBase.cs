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
using Energistics.Etp.Common.Protocol.Core;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// A base class for <see cref="IEtpAdapter"/> implementations that handles registering message decoders and resolving messages.
    /// </summary>
    public abstract class EtpAdapterBase : IEtpAdapter
    {
        private readonly Dictionary<long, Func<Decoder, IMessageHeader, IMessageHeaderExtension, EtpMessage>> MessageDecoders = new Dictionary<long, Func<Decoder, IMessageHeader, IMessageHeaderExtension, EtpMessage>>();
        private readonly Dictionary<long, Func<string, IMessageHeader, IMessageHeaderExtension, EtpMessage>> MessageDeserializers = new Dictionary<long, Func<string, IMessageHeader, IMessageHeaderExtension, EtpMessage>>();
        private readonly Dictionary<Type, Func<Decoder, IMessageHeader, IMessageHeaderExtension, EtpMessage>> MessageDecodersByType = new Dictionary<Type, Func<Decoder, IMessageHeader, IMessageHeaderExtension, EtpMessage>>();

        /// <summary>
        /// Constructs a new <see cref="EtpAdapterBase"/> instance.
        /// </summary>
        /// <param name="version">The supported ETP version.</param>
        protected EtpAdapterBase(EtpVersion version)
        {
            EtpVersion = version;
            RegisterStandardMessageDecoders();
        }

        /// <summary>
        /// The ETP version supported by this adapter.
        /// </summary>
        public EtpVersion EtpVersion { get; }

        /// <summary>
        /// Whether or not Protocol Exceptions are multi-part messages in the ETP version supported by this adapter.
        /// </summary>
        public abstract bool IsProtocolExceptionMultiPart { get; }

        /// <summary>
        /// Whether or not supported data objects are negotiated (i.e. are the intersection) or not (i.e. are the union)
        /// </summary>
        public abstract bool AreSupportedDataObjectsNegotiated { get; }

        /// <summary>
        /// Decode the binary message header.
        /// </summary>
        /// <param name="decoder">The decoder with the binary data.</param>
        /// <returns>The decoded message header.</returns>
        public abstract IMessageHeader DecodeMessageHeader(Decoder decoder);

        /// <summary>
        /// Deserialize the json message header.
        /// </summary>
        /// <param name="json">The json data.</param>
        /// <returns>The deserialized message header.</returns>
        public abstract IMessageHeader DeserializeMessageHeader(string json);

        /// <summary>
        /// Decode the binary message header extension.
        /// </summary>
        /// <param name="decoder">The decoder with the binary data.</param>
        /// <returns>The decoded message header extension.</returns>
        public abstract IMessageHeaderExtension DecodeMessageHeaderExtension(Decoder decoder);

        /// <summary>
        /// Deserialize the json message header extension.
        /// </summary>
        /// <param name="json">The json data.</param>
        /// <returns>The deserialized message header extension.</returns>
        public abstract IMessageHeaderExtension DeserializeMessageHeaderExtension(string json);

        /// <summary>
        /// Decodes the message.
        /// </summary>
        /// <param name="header">The message header</param>
        /// <param name="extension">The header extension</param>
        /// <param name="decoder">The decoder with binary message data.</param>
        /// <returns>The message if successful; <c>null</c> otherwise.</returns>
        public EtpMessage DecodeMessage(IMessageHeader header, IMessageHeaderExtension extension, Decoder decoder)
        {
            var messageKey = EtpExtensions.CreateMessageKey(header.Protocol, header.MessageType);

            Func<Decoder, IMessageHeader, IMessageHeaderExtension, EtpMessage> messageDecoder;
            if (!MessageDecoders.TryGetValue(messageKey, out messageDecoder))
                return null;

            return messageDecoder(decoder, header, extension);
        }

        /// <summary>
        /// Deserializes the message type from the specified protocol.
        /// </summary>
        /// <param name="header">The message header</param>
        /// <param name="extension">The header extension</param>
        /// <param name="content">The string with json message data.</param>
        /// <returns>The message.</returns>
        public EtpMessage DeserializeMessage(IMessageHeader header, IMessageHeaderExtension extension, string content)
        {
            var messageKey = EtpExtensions.CreateMessageKey(header.Protocol, header.MessageType);

            Func<string, IMessageHeader, IMessageHeaderExtension, EtpMessage> messageDeserializer;
            if (!MessageDeserializers.TryGetValue(messageKey, out messageDeserializer))
                return null;

            return messageDeserializer(content, header, extension);
        }

        /// <summary>
        /// Creates a default handler for the core protocol.
        /// </summary>
        /// <param name="clientHandler">Whether to create a client or server handler.</param>
        /// <returns>The default handler.</returns>
        public abstract IProtocolHandler CreateDefaultCoreHandler(bool clientHandler);

        /// <summary>
        /// Checks if the specified message type is valid.
        /// </summary>
        /// <param name="protocol">The message type's protocol.</param>
        /// <param name="messageType">The message type.</param>
        /// <returns><c>true</c> if the message type is valid; <c>false</c> otherwise.</returns>
        public abstract bool IsValidMessageType(int protocol, int messageType);

        /// <summary>
        /// Tries to get the protocol number for the specified message body type.
        /// </summary>
        /// <param name="messageBodyType">The message body type.</param>
        /// <returns>The protocol number on success; -1 otherwise.</returns>
        public abstract int TryGetProtocolNumber(Type messageBodyType);

        /// <summary>
        /// Tries to get the message type number for the specified message body type.
        /// </summary>
        /// <param name="messageBodyType">The message body type.</param>
        /// <returns>The message type number on success; -1 otherwise.</returns>
        public abstract int TryGetMessageTypeNumber(Type messageBodyType);

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

            RegisterMessageDecoderOverride<T, T>(protocol, messageType);
        }

        /// <summary>
        /// Overrides an existing message decoder so that it can return a generic interface.
        /// </summary>
        /// <typeparam name="TInterface">The type of the message body interface.</typeparam>
        /// <typeparam name="TBody">The type of the decoded message.</typeparam>
        /// <param name="protocol"></param>
        /// <param name="messageType"></param>
        protected void RegisterMessageDecoderOverride<TInterface, TBody>(object protocol, object messageType) where TInterface : ISpecificRecord where TBody : TInterface
        {
            var messageKey = EtpExtensions.CreateMessageKey(Convert.ToInt32(protocol), Convert.ToInt32(messageType));

            MessageDecoders[messageKey] = (d, h, x) => CreateMessage<TInterface, TBody>(h, d.Decode<TBody>(), extension: x);
            MessageDeserializers[messageKey] = (b, h, x) => CreateMessage<TInterface, TBody>(h, EtpExtensions.Deserialize<TBody>(b), extension: x);
            MessageDecodersByType[typeof(TInterface)] = MessageDecoders[messageKey];
        }

        /// <summary>
        /// Creates an ETP message for use elsewhere in the DevKit.
        /// </summary>
        /// <typeparam name="TBody">The type of the message body.</typeparam>
        /// <typeparam name="TInterface">The type of the message body interface.</typeparam>
        /// <param name="header">The message header.</param>
        /// <param name="extension">The message header extension, if any.</param>
        /// <param name="body">The message body.</param>
        /// <returns>The message.</returns>
        private EtpMessage CreateMessage<TInterface, TBody>(IMessageHeader header, TBody body, IMessageHeaderExtension extension = null) where TInterface : ISpecificRecord where TBody : TInterface
        {
            return new EtpMessage<TInterface>(header, body, extension: extension);
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
            var mainMessageNamespacePrefix = $"Energistics.Etp.{EtpVersion}.Protocol";
            var privateMessageNamespacePrefix = $"Energistics.Etp.{EtpVersion}.PrivateProtocols";

            var protocolsEnumType = EtpVersion == EtpVersion.v11 ? typeof(v11.Protocols) : typeof(v12.Protocols);
            var protocolNames = Enum.GetNames(protocolsEnumType);

            var messageTypesType = EtpVersion == EtpVersion.v11 ? typeof(v11.MessageTypes) : typeof(v12.MessageTypes);
            var messageTypeMap = messageTypesType.GetNestedTypes().ToDictionary(t => t.Name);
            var messageTypeEnumValuesMap = messageTypeMap.Values.ToDictionary(t => t.Name, t => Enum.GetNames(t));

            var assembly = protocolsEnumType.Assembly;

            // Get the message types defined in the appropriate namespace.
            var mainMessageTypes = assembly.GetExportedTypes().Where(type =>
                type.Namespace.StartsWith(mainMessageNamespacePrefix) &&
                typeof(ISpecificRecord).IsAssignableFrom(type));
            var privateMessageTypes = assembly.GetExportedTypes().Where(type =>
                type.Namespace.StartsWith(privateMessageNamespacePrefix) &&
                typeof(ISpecificRecord).IsAssignableFrom(type));

            var messageTypes = mainMessageTypes.Concat(privateMessageTypes).ToList();

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

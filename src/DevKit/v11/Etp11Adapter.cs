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

using Avro.IO;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Protocol.Core;
using Energistics.Etp.v11.Datatypes;
using Energistics.Etp.v11.Protocol.Core;
using System;

namespace Energistics.Etp.v11
{
    public class Etp11Adapter : EtpAdapterBase
    {
        public Etp11Adapter() : base(EtpVersion.v11)
        {
            // Register message decoder overrides for the core interfaces for messages handled by the session.
            RegisterMessageDecoderOverride<IRequestSession, RequestSession>(Protocols.Core, MessageTypes.Core.RequestSession);
            RegisterMessageDecoderOverride<IOpenSession, OpenSession>(Protocols.Core, MessageTypes.Core.OpenSession);
            RegisterMessageDecoderOverride<ICloseSession, CloseSession>(Protocols.Core, MessageTypes.Core.CloseSession);
            RegisterMessageDecoderOverride<IProtocolException, ProtocolException>(Protocols.Core, MessageTypes.Core.ProtocolException);
            RegisterMessageDecoderOverride<IAcknowledge, Acknowledge>(Protocols.Core, MessageTypes.Core.Acknowledge);
        }

        public override bool IsProtocolExceptionMultiPart => false;

        public override bool AreSupportedDataObjectsNegotiated => false;

        public override IMessageHeader DecodeMessageHeader(Decoder decoder)
        {
            return decoder.Decode<MessageHeader>();
        }

        public override IMessageHeader DeserializeMessageHeader(string json)
        {
            return EtpExtensions.Deserialize<MessageHeader>(json);
        }

        public override IMessageHeaderExtension DecodeMessageHeaderExtension(Decoder decoder)
        {
            return null;
        }

        public override IMessageHeaderExtension DeserializeMessageHeaderExtension(string json)
        {
            return null;
        }

        /// <summary>
        /// Creates a default handler for the core protocol.
        /// </summary>
        /// <param name="clientHandler">Whether to create a client or server handler.</param>
        /// <returns>The default handler.</returns>
        public override IProtocolHandler CreateDefaultCoreHandler(bool clientHandler)
        {
            return clientHandler ? (IProtocolHandler)new CoreClientHandler() : new CoreServerHandler();
        }

        /// <summary>
        /// Checks if the specified message type is valid.
        /// </summary>
        /// <param name="protocol">The message type's protocol.</param>
        /// <param name="messageType">The message type.</param>
        /// <returns><c>true</c> if the message type is valid; <c>false</c> otherwise.</returns>
        public override bool IsValidMessageType(int protocol, int messageType)
        {
            return MessageNames.IsMessageNameRegistered(protocol, messageType);
        }

        /// <summary>
        /// Tries to get the protocol number for the specified message body type.
        /// </summary>
        /// <param name="messageBodyType">The message body type.</param>
        /// <returns>The protocol number on success; -1 otherwise.</returns>
        public override int TryGetProtocolNumber(Type messageBodyType)
        {
            return MessageReflection.TryGetProtocolNumber(messageBodyType);
        }

        /// <summary>
        /// Tries to get the message type number for the specified message body type.
        /// </summary>
        /// <param name="messageBodyType">The message body type.</param>
        /// <returns>The message type number on success; -1 otherwise.</returns>
        public override int TryGetMessageTypeNumber(Type messageBodyType)
        {
            return MessageReflection.TryGetMessageTypeNumber(messageBodyType);
        }
    }
}

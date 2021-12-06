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
using Energistics.Etp.Common.Datatypes;
using System;

namespace Energistics.Etp.Common
{
    public interface IEtpAdapter
    {
        EtpVersion EtpVersion { get; }

        bool IsProtocolExceptionMultiPart { get; }

        bool AreSupportedDataObjectsNegotiated { get; }

        IMessageHeader DecodeMessageHeader(IAvroDecoder decoder);

        IMessageHeaderExtension DecodeMessageHeaderExtension(IAvroDecoder decoder);

        void RegisterMessageDecoder<T>(object protocol, object messageType) where T : class, IEtpMessageBody, new();

        bool IsMessageDecoderRegistered(object protocol, object messageType);

        bool IsMessageDecoderRegistered<T>() where T : IEtpMessageBody;

        EtpMessage DecodeMessage(IMessageHeader header, IMessageHeaderExtension extension, IAvroDecoder decoder);

        IProtocolHandler CreateDefaultCoreHandler(bool clientHandler);

        bool IsValidMessageType(int protocol, int messageType);

        int TryGetProtocolNumber(Type messageBodyType);

        int TryGetMessageTypeNumber(Type messageBodyType);
    }
}
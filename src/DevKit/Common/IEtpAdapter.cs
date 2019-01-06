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
using Avro.Specific;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Protocol.Core;

namespace Energistics.Etp.Common
{
    public interface IEtpAdapter
    {
        EtpVersion SupportedVersion { get; }

        void RegisterCore(IEtpSession session);

        void RequestSession(IEtpSession session, string applicationName, string applicationVersion, string requestedCompression);

        ISupportedProtocol GetSupportedProtocol(IProtocolHandler handler, string role);

        IMessageHeader CreateMessageHeader();

        IMessageHeader DecodeMessageHeader(Decoder decoder, string body);

        IMessageHeader DeserializeMessageHeader(string body);

        void RegisterMessageDecoder<T>(object protocol, object messageType) where T : ISpecificRecord;

        bool IsMessageDecoderRegistered(object protocol, object messageType);

        bool IsMessageDecoderRegistered<T>() where T : ISpecificRecord;

        ISpecificRecord DecodeMessage(int protocol, int messageType, Decoder decoder, string body);

        T DecodeMessage<T>(Decoder decoder, string body) where T : ISpecificRecord;

        IAcknowledge CreateAcknowledge();

        IAcknowledge DecodeAcknowledge(ISpecificRecord body);

        IProtocolException CreateProtocolException();

        IProtocolException DecodeProtocolException(ISpecificRecord body);
    }
}
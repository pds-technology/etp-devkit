//----------------------------------------------------------------------- 
// ETP DevKit, 1.2
//
// Copyright 2018 Energistics
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
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Protocol.Core;

namespace Energistics.Etp.Common
{
    public interface IEtpAdapter
    {
        void RegisterCoreClient(EtpSession session);

        void RegisterCoreServer(EtpSession session);

        void RequestSession(string applicationName, string applicationVersion, string requestedCompression);

        ISupportedProtocol GetSupportedProtocol(IProtocolHandler handler, string role);

        IMessageHeader CreateMessageHeader();

        IMessageHeader DecodeMessageHeader(Decoder decoder, string body);

        IMessageHeader DeserializeMessageHeader(string body);

        IAcknowledge CreateAcknowledge();

        IAcknowledge DecodeAcknowledge(Decoder decoder, string body);

        IAcknowledge DeserializeAcknowledge(string body);

        IProtocolException CreateProtocolException();

        IProtocolException DecodeProtocolException(Decoder decoder, string body);

        IProtocolException DeserializeProtocolException(string body);
    }
}
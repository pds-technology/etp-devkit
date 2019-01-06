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

using System.Linq;
using System.Reflection;
using Avro.IO;
using Avro.Specific;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Protocol.Core;
using Energistics.Etp.v12.Datatypes;
using Energistics.Etp.v12.Protocol.Core;

namespace Energistics.Etp.v12
{
    public class Etp12Adapter : EtpAdapterBase, IEtpAdapter
    {
        public Etp12Adapter() : base(EtpVersion.v12)
        {
        }

        public void RegisterCore(IEtpSession session)
        {
            if (session.IsClient)
                session.Register<ICoreClient, CoreClientHandler>();
            else
                session.Register<ICoreServer, CoreServerHandler>();
        }

        public void RequestSession(IEtpSession session, string applicationName, string applicationVersion, string requestedCompression)
        {
            var requestedProtocols = session.GetSupportedProtocols();

            session.Handler<ICoreClient>()
                .RequestSession(applicationName, applicationVersion, requestedProtocols, requestedCompression);
        }

        public ISupportedProtocol GetSupportedProtocol(IProtocolHandler handler, string role)
        {
            if (handler.SupportedVersion != SupportedVersion)
                return null;

            return new SupportedProtocol
            {
                Protocol = handler.Protocol,
                ProtocolVersion = new Version
                {
                    Major = 1,
                    Minor = 2
                },
                ProtocolCapabilities = handler
                    .GetCapabilities()
                    .ToDictionary(
                        x => x.Key,
                        x => (DataValue) x.Value),
                Role = role
            };
        }

        public IMessageHeader CreateMessageHeader()
        {
            return new MessageHeader();
        }

        public IMessageHeader DecodeMessageHeader(Decoder decoder, string content)
        {
            return decoder.Decode<MessageHeader>(content);
        }

        public IMessageHeader DeserializeMessageHeader(string content)
        {
            return EtpExtensions.Deserialize<MessageHeader>(content);
        }

        public IAcknowledge CreateAcknowledge()
        {
            return new Acknowledge();
        }

        public IAcknowledge DecodeAcknowledge(ISpecificRecord body)
        {
            return (Acknowledge)body;
        }

        public IProtocolException CreateProtocolException()
        {
            return new ProtocolException();
        }

        public IProtocolException DecodeProtocolException(ISpecificRecord body)
        {
            return (ProtocolException)body;
        }
    }
}

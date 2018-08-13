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

using System.Linq;
using Avro.IO;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Protocol.Core;
using Energistics.Etp.v11.Datatypes;
using Energistics.Etp.v11.Protocol.Core;

namespace Energistics.Etp.v11
{
    public class Etp11Adapter : IEtpAdapter
    {
        private EtpSession Session { get; set; }

        public void RegisterCoreClient(EtpSession session)
        {
            Session = session;
            Session.Register<ICoreClient, CoreClientHandler>();
        }

        public void RegisterCoreServer(EtpSession session)
        {
            Session = session;
            Session.Register<ICoreServer, CoreServerHandler>();
        }

        public void RequestSession(string applicationName, string applicationVersion)
        {
            var requestedProtocols = Session.GetSupportedProtocols(true);

            Session.Handler<ICoreClient>()
                .RequestSession(applicationName, applicationVersion, requestedProtocols);
        }

        public ISupportedProtocol GetSupportedProtocol(IProtocolHandler handler, string role)
        {
            return new SupportedProtocol
            {
                Protocol = handler.Protocol,
                ProtocolVersion = new Version
                {
                    Major = 1,
                    Minor = 1
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
            return Session.Deserialize<MessageHeader>(content);
        }

        public IAcknowledge CreateAcknowledge()
        {
            return new Acknowledge();
        }

        public IAcknowledge DecodeAcknowledge(Decoder decoder, string content)
        {
            return decoder.Decode<Acknowledge>(content);
        }

        public IAcknowledge DeserializeAcknowledge(string content)
        {
            return Session.Deserialize<Acknowledge>(content);
        }

        public IProtocolException CreateProtocolException()
        {
            return new ProtocolException();
        }

        public IProtocolException DecodeProtocolException(Decoder decoder, string content)
        {
            return decoder.Decode<ProtocolException>(content);
        }

        public IProtocolException DeserializeProtocolException(string content)
        {
            return Session.Deserialize<ProtocolException>(content);
        }
    }
}

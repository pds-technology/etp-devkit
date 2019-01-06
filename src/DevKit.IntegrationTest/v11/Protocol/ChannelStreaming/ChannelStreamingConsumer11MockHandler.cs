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

using System.Collections.Generic;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v11.Protocol.Core;

namespace Energistics.Etp.v11.Protocol.ChannelStreaming
{
    public class ChannelStreamingConsumer11MockHandler : ChannelStreamingConsumerHandler
    {
        public ChannelStreamingConsumer11MockHandler()
        {
        }

        public bool ProducerIsSimpleStreamer { get; private set; }

        public event ProtocolEventHandler<OpenSession> OnOpenSession;

        public override void OnSessionOpened(IList<ISupportedProtocol> requestedProtocols, IList<ISupportedProtocol> supportedProtocols)
        {
            base.OnSessionOpened(requestedProtocols, supportedProtocols);

            ProducerIsSimpleStreamer = supportedProtocols.IsSimpleStreamer();

            OnOpenSession?.Invoke(this, new ProtocolEventArgs<OpenSession>(null, null));
        }
    }
}

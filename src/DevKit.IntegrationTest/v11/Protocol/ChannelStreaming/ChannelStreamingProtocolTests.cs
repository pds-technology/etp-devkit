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
using System.Linq;
using System.Threading.Tasks;
using Energistics.Etp.Common;
using Energistics.Etp.v11.Protocol.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Energistics.Etp.v11.Protocol.ChannelStreaming
{
    [TestClass]
    public class ChannelStreamingProtocolTests : IntegrationTestBase
    {
        [TestInitialize]
        public void TestSetUp()
        {
            SetUp(TestSettings.WebSocketType, EtpVersion.v11);

            // Register protocol handler
            _server.ServerManager.Register(new ChannelStreamingProducer11MockHandler());

            _server.Start();
        }

        [TestCleanup]
        public void TestTearDown()
        {
            CleanUp();
        }

        [TestMethod]
        public async Task IChannelStreamingConsumer_v11_Start_Connected_To_Simple_Producer()
        {
            _client.Register(new ChannelStreamingConsumer11MockHandler());
            var handler = _client.Handler<IChannelStreamingConsumer>() as ChannelStreamingConsumer11MockHandler;

            // Register event handlers
            var onChannelMetadata = HandleAsync<FireAndForgetEventArgs<ChannelMetadata>>(x => handler.OnSimpleStreamerChannelMetadata += x);
            var onChannelData = HandleAsync<FireAndForgetEventArgs<ChannelData>>(x => handler.OnStreamingChannelData += x);
            var onStarted = HandleAsync<EventArgs>(x => handler.OnStarted += x);

            // Wait for Open connection
            var isOpen = await _client.OpenAsyncWithTimeout();
            Assert.IsTrue(isOpen);

            // Wait for OpenSession to check if the producer is a simple streamer
            await onStarted.WaitAsync();
            Assert.IsTrue(handler.CounterpartCapabilities.SimpleStreamer ?? false);

            // Send Start message
            handler.Start();

            // Wait for ChannelMetadata message
            var argsMetadata = await onChannelMetadata.WaitAsync();

            Assert.IsNotNull(argsMetadata);
            Assert.IsNotNull(argsMetadata.Message.Body.Channels);
            Assert.IsTrue(argsMetadata.Message.Body.Channels.Any());

            // Wait for ChannelData message
            var argsData = await onChannelData.WaitAsync();

            Assert.IsNotNull(argsData);
            Assert.IsNotNull(argsData.Message.Body.Data);
            Assert.IsTrue(argsData.Message.Body.Data.Any());
        }
    }
}

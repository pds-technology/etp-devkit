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
using System.Threading.Tasks;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Protocol.Core;
using Energistics.Etp.Security;
using Energistics.Etp.v11.Datatypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Energistics.Etp.v11.Protocol.Core
{
    [TestClass]
    public class CoreProtocolTests : IntegrationTestBase
    {
        [TestInitialize]
        public void TestSetUp()
        {
            SetUp(TestSettings.WebSocketType, EtpVersion.v11);
            _server.Start();
        }

        [TestCleanup]
        public void TestTearDown()
        {
            CleanUp();
        }

        [TestMethod]
        [Description("EtpClient connects to web socket server")]
        public async Task EtpClient_v11_Open_Connects_To_WebSocket_Server()
        {
            var result = await _client.OpenAsync().WaitAsync();

            Assert.IsTrue(result, "EtpClient connection not opened");
        }

        [TestMethod]
        [Description("EtpClient sends RequestSession and receives OpenSession with a valid Session ID")]
        public async Task EtpClient_v11_RequestSession_Receive_OpenSession_After_Requesting_No_Protocols()
        {
            var onSessionOpened = HandleAsync<SessionOpenedEventArgs>(x => _client.SessionOpened += x);

            var opened = await _client.OpenAsyncWithTimeout();
            Assert.IsTrue(opened, "EtpClient connection not opened");

            var args = await onSessionOpened.WaitAsync();

            Assert.IsNotNull(args);
            Assert.IsTrue(args.OpenedSuccessfully);
            Assert.AreNotEqual(default(Guid), _client.SessionId);
        }

        [Ignore]
        [TestMethod]
        [Description("EtpClient authenticates using JWT retrieved from supported token provider")]
        public async Task EtpClient_v11_OpenSession_Can_Authenticate_Using_Json_Web_Token()
        {
            var authorization = Authorization.Basic(TestSettings.Username, TestSettings.Password);
            var etpSubProtocol = EtpVersion.v11;
            string token;

            using (var client = new System.Net.WebClient())
            {
                client.Headers.SetAuthorization(authorization);

                var response = await client.UploadStringTaskAsync(TestSettings.AuthTokenUrl, "grant_type=password");
                var json = JObject.Parse(response);

                token = json["access_token"].Value<string>();
            }

            _client.Dispose();
            _client = CreateClient(TestSettings.WebSocketType, etpSubProtocol, _server.Uri.ToWebSocketUri().ToString(), Authorization.Bearer(token));

            var onSessionOpened = HandleAsync<SessionOpenedEventArgs>(x => _client.SessionOpened += x);

            var opened = await _client.OpenAsyncWithTimeout();
            Assert.IsTrue(opened, "EtpClient connection not opened");

            var args = await onSessionOpened.WaitAsync();

            Assert.IsNotNull(args);
            Assert.IsNotNull(args.OpenedSuccessfully);
            Assert.AreNotEqual(default(Guid), _client.SessionId);
        }

        [TestMethod]
        [Description("EtpClient sends an invalid message and receives ProtocolException with the correct error code")]
        public async Task EtpClient_v11_SendMessage_Receive_Protocol_Exception_After_Sending_Invalid_Message()
        {
            var onProtocolException = HandleAsync<MessageEventArgs<IProtocolException>>(x => _client.Handler<ICoreClient>().OnProtocolException += x);

            var opened = await _client.OpenAsyncWithTimeout();
            Assert.IsTrue(opened, "EtpClient connection not opened");

            var message = new EtpMessage<IAcknowledge>(
                new MessageHeader() { Protocol = (int)Protocols.Core, MessageType = -999, MessageId = -999 },
                new Acknowledge());
            _client.SendMessage(message);

            var args = await onProtocolException.WaitAsync();

            Assert.IsNotNull(args);
            Assert.IsNotNull(args.Message);
            Assert.AreEqual((int)EtpErrorCodes.InvalidMessageType, args.Message.Body.ErrorCode);
        }
    }
}

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
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Energistics.Etp
{
    [TestClass]
    public class ClientServerStressTest : IntegrationTestBase
    {
        private const string AppName = "EtpClientTests";
        private const string AppVersion = "1.0";

        private readonly ILog Logger = log4net.LogManager.GetLogger(typeof(ClientServerStressTest));

        private readonly int nativeIterations = EtpFactory.IsNativeSupported ? 1000 : 10;
        private const int webSocket4NetIterations = 10;

        protected void SetUp(WebSocketType webSocketType)
        {
            SetUp(webSocketType, EtpSettings.Etp11SubProtocol);
        }

        protected void SetupStart(WebSocketType webSocketType)
        {
            SetUp(webSocketType, EtpSettings.Etp11SubProtocol);
            _server.Start();
        }

        protected void SetupStartOpen(WebSocketType webSocketType)
        {
            SetUp(webSocketType, EtpSettings.Etp11SubProtocol);
            _server.Start();
            _client.Open();
        }

        protected void StopCleanUp()
        {
            _server?.Stop();
            CleanUp();
        }

        protected void CloseStopCleanUp()
        {
            _client?.Close("Closing");
            _server?.Stop();
            CleanUp();
        }

        protected void StopCloseCleanUp()
        {
            _server?.Stop();
            _client?.Close("Closing");
            CleanUp();
        }

        protected void DisposeCloseCleanUp()
        {
            _server?.Dispose();
            _server = null;

            _client?.Close("Closing");
            CleanUp();
        }

        protected void SetUpOnceCleanUpTwice(WebSocketType webSocketType)
        {
            SetupStartOpen(webSocketType);

            _server?.Stop();
            CleanUp();

            _server?.Stop();
            CleanUp();
        }

        protected void RunStressTest(WebSocketType webSocketType, Action<WebSocketType> setup, Action cleanup)
        {
            int iterations = webSocketType == WebSocketType.Native ? nativeIterations : webSocket4NetIterations;

            for (int i = 0; i < iterations; i++)
            {
                Logger.Debug($"Starting iteration {i}");
                setup(webSocketType);

                cleanup();
            }
        }

        [TestMethod]
        public void ClientServerStressTest_Setup_Once_Cleanup_Twice_Native()
        {
            SetUpOnceCleanUpTwice(WebSocketType.Native);
        }

        [TestMethod]
        public void ClientServerStressTest_Setup_Once_Cleanup_Twice_WebSocket4Net()
        {
            SetUpOnceCleanUpTwice(WebSocketType.WebSocket4Net);
        }

        [TestMethod]
        public void ClientServerStressTest_Setup_Cleanup_Native()
        {
            RunStressTest(WebSocketType.Native,
                SetUp, CleanUp);
        }

        [TestMethod]
        public void ClientServerStressTest_Setup_Cleanup_WebSocket4Net()
        {
            RunStressTest(WebSocketType.WebSocket4Net,
                SetUp, CleanUp);
        }

        [TestMethod]
        public void ClientServerStressTest_SetupStart_Cleanup_Native()
        {
            RunStressTest(WebSocketType.Native,
                SetupStart, CleanUp);
        }

        [TestMethod]
        public void ClientServerStressTest_SetupStart_Cleanup_WebSocket4Net()
        {
            RunStressTest(WebSocketType.WebSocket4Net,
                SetupStart, CleanUp);
        }

        [TestMethod]
        public void ClientServerStressTest_SetupStart_StopCleanup_Native()
        {
            RunStressTest(WebSocketType.Native,
                SetupStart, StopCleanUp);
        }

        [TestMethod]
        public void ClientServerStressTest_SetupStart_StopCleanup_WebSocket4Net()
        {
            RunStressTest(WebSocketType.WebSocket4Net,
                SetupStart, StopCleanUp);
        }

        [TestMethod]
        public void ClientServerStressTest_SetupStartOpen_StopCleanup_Native()
        {
            RunStressTest(WebSocketType.Native,
                SetupStartOpen, StopCleanUp);
        }

        [TestMethod]
        public void ClientServerStressTest_SetupStartOpen_StopCleanup_WebSocket4Net()
        {
            RunStressTest(WebSocketType.WebSocket4Net,
                SetupStartOpen, StopCleanUp);
        }

        [TestMethod]
        public void ClientServerStressTest_SetupStartOpen_StopCloseCleanup_Native()
        {
            RunStressTest(WebSocketType.Native,
                SetupStartOpen, StopCloseCleanUp);
        }

        [TestMethod]
        public void ClientServerStressTest_SetupStartOpen_StopCloseCleanup_WebSocket4Net()
        {
            RunStressTest(WebSocketType.WebSocket4Net,
                SetupStartOpen, StopCloseCleanUp);
        }

        [TestMethod]
        public void ClientServerStressTest_SetupStartOpen_CoseStopCleanup_Native()
        {
            RunStressTest(WebSocketType.Native,
                SetupStartOpen, CloseStopCleanUp);
        }

        [TestMethod]
        public void ClientServerStressTest_SetupStartOpen_CloseStopCleanup_WebSocket4Net()
        {
            RunStressTest(WebSocketType.WebSocket4Net,
                SetupStartOpen, CloseStopCleanUp);
        }

        [TestMethod]
        public void ClientServerStressTest_SetupStartOpen_DisposeCloseCleanup_Native()
        {
            RunStressTest(WebSocketType.Native,
                SetupStartOpen, DisposeCloseCleanUp);
        }

        [TestMethod]
        public void ClientServerStressTest_SetupStartOpen_DisposeCloseCleanup_WebSocket4Net()
        {
            RunStressTest(WebSocketType.WebSocket4Net,
                SetupStartOpen, DisposeCloseCleanUp);
        }
    }
}

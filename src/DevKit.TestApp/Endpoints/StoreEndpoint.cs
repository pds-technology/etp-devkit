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

using Energistics.Etp.Common;
using Energistics.Etp.Handlers;

namespace Energistics.Etp.Endpoints
{
    public class StoreEndpoint : Endpoint
    {
        public TestDataStore TestDataStore { get; } = new TestDataStore();

        protected override void InitializeWebServer(IEtpSelfHostedWebServer webServer)
        {
            var webServerHandler = new StoreHandler(TestDataStore);
            InitializeRegistrar(webServer.ServerManager, webServerHandler);
            Handlers.Add(webServerHandler);
        }

        protected override void InitializeServer(IEtpServer server)
        {
            var serverHandler = new StoreHandler(TestDataStore);
            serverHandler.InitializeSession(server);
        }

        protected override void InitializeClient(IEtpClient client)
        {
            var clientHandler = new StoreHandler(TestDataStore);
            InitializeRegistrar(client, clientHandler);
            clientHandler.InitializeSession(client);
            Handlers.Add(clientHandler);
        }

        protected void InitializeRegistrar(IEtpSessionCapabilitiesRegistrar registrar, StoreHandler storeHandler)
        {
            storeHandler.InitializeRegistrar(registrar);
            registrar.Register(TestDataStore.SupportedDataObjects.SupportedTypes);
        }
    }
}

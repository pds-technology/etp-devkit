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

namespace Energistics.Etp.Handlers
{
    public class CustomerHandler : Handler
    {
        public CustomerHandler()
        {
            DiscoveryHandler = new CustomerDiscoveryHandler();
            ChannelHandler = new CustomerChannelHandler();
            ObjectHandler = new CustomerObjectHandler();
        }

        public CustomerDiscoveryHandler DiscoveryHandler { get; }
        public CustomerChannelHandler ChannelHandler { get; }
        public CustomerObjectHandler ObjectHandler { get; }

        protected override void InitializeRegistrarCore()
        {
            DiscoveryHandler.InitializeRegistrar(Registrar);
            ChannelHandler.InitializeRegistrar(Registrar);
            ObjectHandler.InitializeRegistrar(Registrar);
        }

        protected override void InitializeSessionCore()
        {
            DiscoveryHandler.InitializeSession(Session);
            ChannelHandler.InitializeSession(Session);
            ObjectHandler.InitializeSession(Session);
        }

        public override void PrintConsoleOptions()
        {
            Console.WriteLine(" D - Set store primary dataspace name");
            DiscoveryHandler.PrintConsoleOptions();
            ChannelHandler.PrintConsoleOptions();
            ObjectHandler.PrintConsoleOptions();
        }

        public override bool HandleConsoleInput(ConsoleKeyInfo info)
        {
            return DiscoveryHandler.HandleConsoleInput(info) || ChannelHandler.HandleConsoleInput(info) || ObjectHandler.HandleConsoleInput(info);
        }
    }
}

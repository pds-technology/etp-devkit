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

namespace Energistics.Etp.Handlers
{
    public abstract class Handler
    {
        public IEtpSessionCapabilitiesRegistrar Registrar { get; private set; }
        public IEtpSession Session { get; private set; }

        public void InitializeRegistrar(IEtpSessionCapabilitiesRegistrar registrar)
        {
            Registrar = registrar;
            InitializeRegistrarCore();
        }
        protected virtual void InitializeRegistrarCore()
        {
        }

        public void InitializeSession(IEtpSession session)
        {
            Session = session;
            InitializeSessionCore();
        }
        protected virtual void InitializeSessionCore()
        {
        }

        public virtual void PrintConsoleOptions()
        {
        }

        public virtual bool HandleConsoleInput(ConsoleKeyInfo info)
        {
            return false;
        }

        protected bool IsKey(ConsoleKeyInfo info, string key)
        {
            return string.Equals(info.KeyChar.ToString(), key, StringComparison.OrdinalIgnoreCase);
        }
    }
}

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
using System.IO;
using System.Reflection;
using Energistics.Etp.Endpoints;
using log4net.Config;

namespace Energistics.Etp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            XmlConfigurator.ConfigureAndWatch(log4net.LogManager.GetRepository(Assembly.GetExecutingAssembly()), new FileInfo("log4net.config"));
            Start();
        }

        private static void Start()
        {
            while (true)
            {
                Console.Write("Press 'S' to start a store,");
                Console.WriteLine(" or press 'C' to start a customer...");

                var key = Console.ReadKey();

                Console.WriteLine(" - processing...");
                Console.WriteLine();

                if (IsKey(key, "S"))
                {
                    new StoreEndpoint().Run();
                    break;
                }
                else if (IsKey(key, "C"))
                {
                    new CustomerEndpoint().Run();
                    break;
                }
            }
        }

        private static bool IsKey(ConsoleKeyInfo info, string key)
        {
            return string.Equals(info.KeyChar.ToString(), key, StringComparison.OrdinalIgnoreCase);
        }
    }
}

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
using Energistics.Etp.Common.Datatypes;
using System;
using System.Collections.Generic;

namespace Energistics.Etp.Data
{
    public abstract class MockResqmlObject : MockObject
    {
        public static EtpDataObjectType Type { get; } = new EtpDataObjectType("resqml", "2.0.1");

        protected override string DefaultNamespace(bool embedded)
        {
            if (embedded)
                return string.Empty;

            return @" xmlns=""http://www.energistics.org/energyml/data/resqmlv2""";
        }
    }
}

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
using System;

namespace Energistics.Etp.Data
{
    public abstract class MockWitsmlActiveObject : MockWitsmlObject, IMockActiveObject
    {
        public bool IsActive { get; private set; }

        public DateTime ActiveChangeTime { get; private set; } = 0L.ToUtcDateTime();

        public override void Create(DateTime createdTime)
        {
            base.Create(createdTime);
            IsActive = false;
            ActiveChangeTime = StoreCreated;
        }

        public void SetActive(bool active, DateTime activeChangeTime)
        {
            if (IsActive == active)
                return;

            IsActive = active;
            ActiveChangeTime = activeChangeTime;
            UpdateObjectLastWrite(activeChangeTime);
        }
    }
}

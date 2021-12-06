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

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Provides common information about a supported data object in a session
    /// </summary>
    public interface ISessionSupportedDataObject : IEndpointSupportedDataObject
    {
        /// <summary>
        /// Gets the counterpart's capabilities supported for this object.
        /// </summary>
        IDataObjectCapabilities CounterpartCapabilities { get; }
    }
}

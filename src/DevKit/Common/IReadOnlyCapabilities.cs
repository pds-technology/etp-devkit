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

using Energistics.Etp.Common.Datatypes;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Read-only interface for ETP capabilities.
    /// </summary>
    public interface IReadOnlyCapabilities : IReadOnlyDataValueDictionary
    {
        /// <summary>
        /// Saves the property values to the specified capabilities dictionary.
        /// </summary>
        /// <param name="capabilities">The capabilities dictionary to save values to.</param>
        void SaveTo(IDataValueDictionary capabilities);

        /// <summary>
        /// Creates a deep copy of capabilities stored in this instance.
        /// </summary>
        /// <returns>A deep copy of capabilities stored in this instance.  The returned instance is a <see cref="IDataValueDictionary"/> but is not guaranteed to be an <see cref="ICapabilities"/> instance.</returns>
        IDataValueDictionary CloneCapabilities();
    }
}

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
using System.Collections.Generic;

namespace Energistics.Etp.Common
{
    /// <summary>
    /// Provides common interface for a collection of supported data objects
    /// </summary>
    public interface ISessionSupportedDataObjectCollection : IReadOnlyList<ISessionSupportedDataObject>
    {
        /// <summary>
        /// Checks whether or not the specificied <see cref="IDataObjectType"/> is supported or not.
        /// </summary>
        /// <param name="dataObjectType">The type to check.</param>
        /// <returns><c>true</c> if the data object type is supported; <c>false</c> otherwise.</returns>
        bool IsSupported(IDataObjectType dataObjectType);

        /// <summary>
        /// Tries to get the <see cref="ISessionDataObject"/> matching the specified <see cref="IDataObjectType"/>.
        /// </summary>
        /// <param name="dataObjectType">The type to try to get the matching <see cref="ISessionDataObject"/> for.</param>
        /// <returns>
        /// If there is a specific type matching the specified data object type, it is returned.
        /// Otherwise, if there is a family that matches the specified data object type, it is returned.
        /// Otherwise, <c>null</c> is returned.
        /// </returns>
        ISessionSupportedDataObject TryGetMatchingDataObject(IDataObjectType dataObjectType);

        /// <summary>
        /// An enumerable of all <see cref="ISessionSupportedDataObject"/> instances that represent data object families (i.e. are wildcards).
        /// </summary>
        IEnumerable<ISessionSupportedDataObject> SupportedFamilies { get; }

        /// <summary>
        /// An enumerable of all <see cref="ISessionSupportedDataObject"/> instances that represent specific data object types (i.e. are NOT wildcards).
        /// </summary>
        IEnumerable<ISessionSupportedDataObject> SupportedTypes { get; }
    }
}

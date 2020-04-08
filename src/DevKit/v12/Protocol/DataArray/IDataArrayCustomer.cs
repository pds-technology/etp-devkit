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
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v12.Datatypes.DataArrayTypes;

namespace Energistics.Etp.v12.Protocol.DataArray
{
    /// <summary>
    /// Defines the interface that must be implemented by the customer role of the DataArray protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.DataArray, "customer", "store")]
    public interface IDataArrayCustomer : IProtocolHandler
    {
        /// <summary>
        /// Sends a GetDataArrays message to a store.
        /// </summary>
        /// <param name="dataArrays">The data arrays.</param>
        /// <returns>The message identifier.</returns>
        long GetDataArrays(IList<DataArrayIdentifier> dataArrays);

        /// <summary>
        /// Handles the GetDataArraysResponse event from a store.
        /// </summary>
        event ProtocolEventHandler<GetDataArraysResponse> OnGetDataArraysResponse;

        /// <summary>
        /// Sends a GetDataSubarrays message to a store.
        /// </summary>
        /// <param name="dataSubarrays">The data subarrays.</param>
        /// <returns>The message identifier.</returns>
        long GetDataSubarrays(IList<GetDataSubarraysType> dataSubarrays);

        /// <summary>
        /// Handles the GetDataSubarraysResponse event from a store.
        /// </summary>
        event ProtocolEventHandler<GetDataSubarraysResponse> OnGetDataSubarraysResponse;

        /// <summary>
        /// Sends a PutDataArrays message to a store.
        /// </summary>
        /// <param name="dataArrays">The data arrays.</param>
        /// <returns>The message identifier.</returns>
        long PutDataArrays(IList<PutDataArraysType> dataArrays);

        /// <summary>
        /// Sends a PutUninitializedDataArray message to a store.
        /// </summary>
        /// <param name="dataArrays">The data arrays.</param>
        /// <returns>The message identifier.</returns>
        long PutUninitializedDataArray(IList<PutUninitializedDataArrayType> dataArrays);

        /// <summary>
        /// Sends a PutDataSubarrays message to a store.
        /// </summary>
        /// <param name="dataSubarrays">The data subarrays.</param>
        /// <returns>The message identifier.</returns>
        long PutDataSubarrays(IList<PutDataSubarraysType> dataSubarrays);

        /// <summary>
        /// Sends a GetDataArrayMetadata message to a store.
        /// </summary>
        /// <param name="dataArrays">The data arrays.</param>
        /// <returns>The message identifier.</returns>
        long GetDataArrayMetadata(IList<DataArrayIdentifier> dataArrays);

        /// <summary>
        /// Handles the GetDataArrayMetadataResponse event from a store.
        /// </summary>
        event ProtocolEventHandler<GetDataArrayMetadataResponse> OnGetDataArrayMetadataResponse;
    }
}

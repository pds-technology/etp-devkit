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
using Energistics.Etp.v12.Datatypes;
using Energistics.Etp.v12.Datatypes.DataArrayTypes;

namespace Energistics.Etp.v12.Protocol.DataArray
{
    /// <summary>
    /// Defines the interface that must be implemented by the store role of the store protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.DataArray, "store", "customer")]
    public interface IDataArrayStore : IProtocolHandler
    {
        /// <summary>
        /// Handles the GetDataArrays event from a customer.
        /// </summary>
        event ProtocolEventWithErrorsHandler<GetDataArrays, Datatypes.DataArrayTypes.DataArray, ErrorInfo> OnGetDataArrays;

        /// <summary>
        /// Sends an GetDataArraysResponse message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="dataArrays">The data arrays.</param>
        /// <param name="errors">The errors.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long GetDataArraysResponse(IMessageHeader request, IDictionary<string, Datatypes.DataArrayTypes.DataArray> dataArrays, IDictionary<string, ErrorInfo> errors);

        /// <summary>
        /// Handles the GetDataSubarrays event from a customer.
        /// </summary>
        event ProtocolEventWithErrorsHandler<GetDataSubarrays, Datatypes.DataArrayTypes.DataArray, ErrorInfo> OnGetDataSubarrays;

        /// <summary>
        /// Sends an GetDataSubarraysResponse message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="dataSubarrays">The data subarrays.</param>
        /// <param name="errors">The errors.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long GetDataSubarraysResponse(IMessageHeader request, IDictionary<string, Datatypes.DataArrayTypes.DataArray> dataSubarrays, IDictionary<string, ErrorInfo> errors);

        /// <summary>
        /// Handles the PutDataArrays event from a customer.
        /// </summary>
        event ProtocolEventWithErrorsHandler<PutDataArrays, ErrorInfo> OnPutDataArrays;

        /// <summary>
        /// Handles the PutUninitializedDataArrays event from a customer.
        /// </summary>
        event ProtocolEventWithErrorsHandler<PutUninitializedDataArrays, ErrorInfo> OnPutUninitializedDataArrays;

        /// <summary>
        /// Handles the PutDataSubarrays event from a customer.
        /// </summary>
        event ProtocolEventWithErrorsHandler<PutDataSubarrays, ErrorInfo> OnPutDataSubarrays;

        /// <summary>
        /// Handles the GetDataArrayMetadata event from a customer.
        /// </summary>
        event ProtocolEventWithErrorsHandler<GetDataArrayMetadata, DataArrayMetadata, ErrorInfo> OnGetDataArrayMetadata;

        /// <summary>
        /// Sends an GetDataArrayMetadataResponse message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="arrayMetadata">The array metadata.</param>
        /// <param name="errors">The errors.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        long GetDataArrayMetadataResponse(IMessageHeader request, IDictionary<string, DataArrayMetadata> arrayMetadata, IDictionary<string, ErrorInfo> errors);
    }
}

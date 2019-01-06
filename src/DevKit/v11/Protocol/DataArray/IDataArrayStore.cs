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
using Energistics.Etp.v11.Datatypes;

namespace Energistics.Etp.v11.Protocol.DataArray
{
    [ProtocolRole((int)Protocols.DataArray, "store", "customer")]
    public interface IDataArrayStore : IProtocolHandler
    {
        /// <summary>
        /// Sends a data array as a response for GetDataArray and GetDataArraySlice.
        /// </summary>
        /// <param name="dimensions">The dimensions.</param>
        /// <param name="data">The data array.</param>
        /// <returns>The message identifier.</returns>
        long DataArray(IList<long> dimensions, AnyArray data);

        /// <summary>
        /// Handles the GetDataArray event from a customer.
        /// </summary>
        event ProtocolEventHandler<GetDataArray> OnGetDataArray;

        /// <summary>
        /// Handles the GetDataArraySlice event from a customer.
        /// </summary>
        event ProtocolEventHandler<GetDataArraySlice> OnGetDataArraySlice;

        /// <summary>
        /// Handles the PutDataArray event from a customer.
        /// </summary>
        event ProtocolEventHandler<PutDataArray> OnPutDataArray;

        /// <summary>
        /// Handles the PutDataArraySlice event from a customer.
        /// </summary>
        event ProtocolEventHandler<PutDataArraySlice> OnPutDataArraySlice;
    }
}

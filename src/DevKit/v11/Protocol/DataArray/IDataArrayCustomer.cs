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
    [ProtocolRole((int)Protocols.DataArray, "customer", "store")]
    public interface IDataArrayCustomer : IProtocolHandler
    {
        /// <summary>
        /// Gets the data array by URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The message identifier.</returns>
        long GetDataArray(string uri);

        /// <summary>
        /// Gets the data array slice by URI, start and count.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns>The message identifier.</returns>
        long GetDataArraySlice(string uri, IList<long> start, IList<long> count);

        /// <summary>
        /// Puts a data array in the data store.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="data">The data array.</param>
        /// <param name="dimensions">The dimensions.</param>
        /// <returns>The message identifier.</returns>
        long PutDataArray(string uri, AnyArray data, IList<long> dimensions);

        /// <summary>
        /// Puts a data array slice in the data store.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="data">The data array.</param>
        /// <param name="dimensions">The dimensions.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns>The message identifier.</returns>
        long PutDataArraySlice(string uri, AnyArray data, IList<long> dimensions, IList<long> start, IList<long> count);

        /// <summary>
        /// Handles the DataArray event from a store.
        /// </summary>
        event ProtocolEventHandler<DataArray> OnDataArray; 
    }
}

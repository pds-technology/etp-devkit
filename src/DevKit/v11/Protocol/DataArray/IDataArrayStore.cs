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
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;

namespace Energistics.Etp.v11.Protocol.DataArray
{
    [ProtocolRole((int)Protocols.DataArray, Roles.Store, Roles.Customer)]
    public interface IDataArrayStore : IProtocolHandler
    {
        /// <summary>
        /// Sends a data array as a response for GetDataArray and GetDataArraySlice.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="array">The data array.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<DataArray> DataArray(IMessageHeader correlatedHeader, DataArray array);

        /// <summary>
        /// Handles the GetDataArray event from a customer.
        /// </summary>
        event EventHandler<RequestEventArgs<GetDataArray, DataArray>> OnGetDataArray;

        /// <summary>
        /// Handles the GetDataArraySlice event from a customer.
        /// </summary>
        event EventHandler<RequestEventArgs<GetDataArraySlice, DataArray>> OnGetDataArraySlice;

        /// <summary>
        /// Handles the PutDataArray event from a customer.
        /// </summary>
        event EventHandler<VoidRequestEventArgs<PutDataArray>> OnPutDataArray;

        /// <summary>
        /// Handles the PutDataArraySlice event from a customer.
        /// </summary>
        event EventHandler<VoidRequestEventArgs<PutDataArraySlice>> OnPutDataArraySlice;
    }
}

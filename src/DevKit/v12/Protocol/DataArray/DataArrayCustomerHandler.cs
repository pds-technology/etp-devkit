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
    /// Base implementation of the <see cref="IDataArrayCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.DataArray.IDataArrayCustomer" />
    public class DataArrayCustomerHandler : Etp12ProtocolHandler, IDataArrayCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataArrayCustomerHandler"/> class.
        /// </summary>
        public DataArrayCustomerHandler() : base((int)Protocols.DataArray, "customer", "store")
        {
            RegisterMessageHandler<GetDataArraysResponse>(Protocols.DataArray, MessageTypes.DataArray.GetDataArraysResponse, HandleGetDataArraysResponse);
            RegisterMessageHandler<GetDataSubarraysResponse>(Protocols.DataArray, MessageTypes.DataArray.GetDataSubarraysResponse, HandleGetDataSubarraysResponse);
            RegisterMessageHandler<GetDataArrayMetadataResponse>(Protocols.DataArray, MessageTypes.DataArray.GetDataArrayMetadataResponse, HandleGetDataArrayMetadataResponse);
        }

        /// <summary>
        /// Sends a GetDataArrays message to a store.
        /// </summary>
        /// <param name="dataArrays">The data arrays.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long GetDataArrays(IList<DataArrayIdentifier> dataArrays)
        {
            var header = CreateMessageHeader(Protocols.DataArray, MessageTypes.DataArray.GetDataArrays);

            var message = new GetDataArrays
            {
                DataArrays = dataArrays.ToMap(),
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Handles the GetDataArraysResponse event from a store.
        /// </summary>
        public event ProtocolEventHandler<GetDataArraysResponse> OnGetDataArraysResponse;

        /// <summary>
        /// Sends a GetDataSubarrays message to a store.
        /// </summary>
        /// <param name="dataSubarrays">The data subarrays.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long GetDataSubarrays(IList<GetDataSubarraysType> dataSubarrays)
        {
            var header = CreateMessageHeader(Protocols.DataArray, MessageTypes.DataArray.GetDataSubarrays);

            var message = new GetDataSubarrays
            {
                DataSubarrays = dataSubarrays.ToMap(),
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Handles the GetDataSubarraysResponse event from a store.
        /// </summary>
        public event ProtocolEventHandler<GetDataSubarraysResponse> OnGetDataSubarraysResponse;

        /// <summary>
        /// Sends a PutDataArrays message to a store.
        /// </summary>
        /// <param name="dataArrays">The data arrays.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long PutDataArrays(IList<PutDataArraysType> dataArrays)
        {
            var header = CreateMessageHeader(Protocols.DataArray, MessageTypes.DataArray.PutDataArrays);

            var message = new PutDataArrays
            {
                DataArrays = dataArrays.ToMap(),
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Sends a PutUninitializedDataArray message to a store.
        /// </summary>
        /// <param name="dataArrays">The data arrays.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long PutUninitializedDataArray(IList<PutUninitializedDataArrayType> dataArrays)
        {
            var header = CreateMessageHeader(Protocols.DataArray, MessageTypes.DataArray.PutUninitializedDataArray);

            var message = new PutUninitializedDataArray
            {
                DataArrays = dataArrays.ToMap(),
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Sends a PutDataSubarrays message to a store.
        /// </summary>
        /// <param name="dataSubarrays">The data subarrays.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long PutDataSubarrays(IList<PutDataSubarraysType> dataSubarrays)
        {
            var header = CreateMessageHeader(Protocols.DataArray, MessageTypes.DataArray.PutDataSubarrays);

            var message = new PutDataSubarrays
            {
                DataSubarrays = dataSubarrays.ToMap(),
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Sends a GetDataArrayMetadata message to a store.
        /// </summary>
        /// <param name="dataArrays">The data arrays.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long GetDataArrayMetadata(IList<DataArrayIdentifier> dataArrays)
        {
            var header = CreateMessageHeader(Protocols.DataArray, MessageTypes.DataArray.GetDataArrayMetadata);

            var message = new GetDataArrayMetadata
            {
                DataArrays = dataArrays.ToMap(),
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Handles the GetDataArrayMetadataResponse event from a store.
        /// </summary>
        public event ProtocolEventHandler<GetDataArrayMetadataResponse> OnGetDataArrayMetadataResponse;

        /// <summary>
        /// Handles the GetDataArraysResponse message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="response">The GetDataArraysResponse message.</param>
        protected virtual void HandleGetDataArraysResponse(IMessageHeader header, GetDataArraysResponse response)
        {
            Notify(OnGetDataArraysResponse, header, response);
        }

        /// <summary>
        /// Handles the GetDataSubarraysResponse message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="response">The GetDataSubarraysResponse message.</param>
        protected virtual void HandleGetDataSubarraysResponse(IMessageHeader header, GetDataSubarraysResponse response)
        {
            Notify(OnGetDataSubarraysResponse, header, response);
        }

        /// <summary>
        /// Handles the GetDataArrayMetadataResponse message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="response">The GetDataArrayMetadataResponse message.</param>
        protected virtual void HandleGetDataArrayMetadataResponse(IMessageHeader header, GetDataArrayMetadataResponse response)
        {
            Notify(OnGetDataArrayMetadataResponse, header, response);
        }
    }
}

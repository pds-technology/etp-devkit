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
    [ProtocolRole((int)Protocols.DataArray, Roles.Customer, Roles.Store)]
    public interface IDataArrayCustomer : IProtocolHandlerWithCounterpartCapabilities<ICapabilitiesStore>
    {
        /// <summary>
        /// Sends a GetDataArrayMetadata message to a store.
        /// </summary>
        /// <param name="dataArrays">The data arrays.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetDataArrayMetadata> GetDataArrayMetadata(IDictionary<string, DataArrayIdentifier> dataArrays, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a GetDataArrayMetadata message to a store.
        /// </summary>
        /// <param name="dataArrays">The data arrays.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetDataArrayMetadata> GetDataArrayMetadata(IList<DataArrayIdentifier> dataArrays, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the GetDataArrayMetadataResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<GetDataArrayMetadata, GetDataArrayMetadataResponse>> OnGetDataArrayMetadataResponse;

        /// <summary>
        /// Sends a GetDataArrays message to a store.
        /// </summary>
        /// <param name="dataArrays">The data arrays.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetDataArrays> GetDataArrays(IDictionary<string, DataArrayIdentifier> dataArrays, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a GetDataArrays message to a store.
        /// </summary>
        /// <param name="dataArrays">The data arrays.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetDataArrays> GetDataArrays(IList<DataArrayIdentifier> dataArrays, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the GetDataArraysResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<GetDataArrays, GetDataArraysResponse>> OnGetDataArraysResponse;

        /// <summary>
        /// Sends a GetDataSubarrays message to a store.
        /// </summary>
        /// <param name="dataSubarrays">The data subarrays.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetDataSubarrays> GetDataSubarrays(IDictionary<string, GetDataSubarraysType> dataSubarrays, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a GetDataSubarrays message to a store.
        /// </summary>
        /// <param name="dataSubarrays">The data subarrays.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetDataSubarrays> GetDataSubarrays(IList<GetDataSubarraysType> dataSubarrays, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the GetDataSubarraysResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<GetDataSubarrays, GetDataSubarraysResponse>> OnGetDataSubarraysResponse;

        /// <summary>
        /// Sends a PutUninitializedDataArrays message to a store.
        /// </summary>
        /// <param name="dataArrays">The data arrays.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<PutUninitializedDataArrays> PutUninitializedDataArrays(IDictionary<string, PutUninitializedDataArrayType> dataArrays, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a PutUninitializedDataArrays message to a store.
        /// </summary>
        /// <param name="dataArrays">The data arrays.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<PutUninitializedDataArrays> PutUninitializedDataArrays(IList<PutUninitializedDataArrayType> dataArrays, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the PutUninitializedDataArraysResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<PutUninitializedDataArrays, PutUninitializedDataArraysResponse>> OnPutUninitializedDataArraysResponse;

        /// <summary>
        /// Sends a PutDataArrays message to a store.
        /// </summary>
        /// <param name="dataArrays">The data arrays.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<PutDataArrays> PutDataArrays(IDictionary<string, PutDataArraysType> dataArrays, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a PutDataArrays message to a store.
        /// </summary>
        /// <param name="dataArrays">The data arrays.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<PutDataArrays> PutDataArrays(IList<PutDataArraysType> dataArrays, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the PutDataArraysResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<PutDataArrays, PutDataArraysResponse>> OnPutDataArraysResponse;

        /// <summary>
        /// Sends a PutDataSubarrays message to a store.
        /// </summary>
        /// <param name="dataSubarrays">The data subarrays.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<PutDataSubarrays> PutDataSubarrays(IDictionary<string, PutDataSubarraysType> dataSubarrays, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a PutDataSubarrays message to a store.
        /// </summary>
        /// <param name="dataSubarrays">The data subarrays.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<PutDataSubarrays> PutDataSubarrays(IList<PutDataSubarraysType> dataSubarrays, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the PutDataSubarraysResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<PutDataSubarrays, PutDataSubarraysResponse>> OnPutDataSubarraysResponse;

    }
}

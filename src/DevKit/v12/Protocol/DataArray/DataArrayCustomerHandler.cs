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
using Energistics.Etp.Common.Protocol.Core;
using Energistics.Etp.v12.Datatypes.DataArrayTypes;

namespace Energistics.Etp.v12.Protocol.DataArray
{
    /// <summary>
    /// Base implementation of the <see cref="IDataArrayCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.DataArray.IDataArrayCustomer" />
    public class DataArrayCustomerHandler : Etp12ProtocolHandlerWithCounterpartCapabilities<CapabilitiesStore, ICapabilitiesStore>, IDataArrayCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataArrayCustomerHandler"/> class.
        /// </summary>
        public DataArrayCustomerHandler() : base((int)Protocols.DataArray, Roles.Customer, Roles.Store)
        {
            RegisterMessageHandler<GetDataArrayMetadataResponse>(Protocols.DataArray, MessageTypes.DataArray.GetDataArrayMetadataResponse, HandleGetDataArrayMetadataResponse);
            RegisterMessageHandler<GetDataArraysResponse>(Protocols.DataArray, MessageTypes.DataArray.GetDataArraysResponse, HandleGetDataArraysResponse);
            RegisterMessageHandler<GetDataSubarraysResponse>(Protocols.DataArray, MessageTypes.DataArray.GetDataSubarraysResponse, HandleGetDataSubarraysResponse);
            RegisterMessageHandler<PutUninitializedDataArraysResponse>(Protocols.DataArray, MessageTypes.DataArray.PutUninitializedDataArraysResponse, HandlePutUninitializedDataArraysResponse);
            RegisterMessageHandler<PutDataArraysResponse>(Protocols.DataArray, MessageTypes.DataArray.PutDataArraysResponse, HandlePutDataArraysResponse);
            RegisterMessageHandler<PutDataSubarraysResponse>(Protocols.DataArray, MessageTypes.DataArray.PutDataSubarraysResponse, HandlePutDataSubarraysResponse);
        }

        /// <summary>
        /// Sends a GetDataArrayMetadata message to a store.
        /// </summary>
        /// <param name="dataArrays">The data arrays.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetDataArrayMetadata> GetDataArrayMetadata(IDictionary<string, DataArrayIdentifier> dataArrays, IMessageHeaderExtension extension = null)
        {
            var body = new GetDataArrayMetadata
            {
                DataArrays = dataArrays ?? new Dictionary<string, DataArrayIdentifier>(),
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Sends a GetDataArrayMetadata message to a store.
        /// </summary>
        /// <param name="dataArrays">The data arrays.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetDataArrayMetadata> GetDataArrayMetadata(IList<DataArrayIdentifier> dataArrays, IMessageHeaderExtension extension = null) => GetDataArrayMetadata(dataArrays.ToMap(), extension: extension);

        /// <summary>
        /// Handles the GetDataArrayMetadataResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<GetDataArrayMetadata, GetDataArrayMetadataResponse>> OnGetDataArrayMetadataResponse;

        /// <summary>
        /// Sends a GetDataArrays message to a store.
        /// </summary>
        /// <param name="dataArrays">The data arrays.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetDataArrays> GetDataArrays(IDictionary<string, DataArrayIdentifier> dataArrays, IMessageHeaderExtension extension = null)
        {
            var body = new GetDataArrays
            {
                DataArrays = dataArrays ?? new Dictionary<string, DataArrayIdentifier>(),
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Sends a GetDataArrays message to a store.
        /// </summary>
        /// <param name="dataArrays">The data arrays.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetDataArrays> GetDataArrays(IList<DataArrayIdentifier> dataArrays, IMessageHeaderExtension extension = null) => GetDataArrays(dataArrays.ToMap(), extension: extension);

        /// <summary>
        /// Handles the GetDataArraysResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<GetDataArrays, GetDataArraysResponse>> OnGetDataArraysResponse;

        /// <summary>
        /// Sends a GetDataSubarrays message to a store.
        /// </summary>
        /// <param name="dataSubarrays">The data subarrays.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetDataSubarrays> GetDataSubarrays(IDictionary<string, GetDataSubarraysType> dataSubarrays, IMessageHeaderExtension extension = null)
        {
            var body = new GetDataSubarrays
            {
                DataSubarrays = dataSubarrays ?? new Dictionary<string, GetDataSubarraysType>(),
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Sends a GetDataSubarrays message to a store.
        /// </summary>
        /// <param name="dataSubarrays">The data subarrays.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetDataSubarrays> GetDataSubarrays(IList<GetDataSubarraysType> dataSubarrays, IMessageHeaderExtension extension = null) => GetDataSubarrays(dataSubarrays.ToMap(), extension: extension);

        /// <summary>
        /// Handles the GetDataSubarraysResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<GetDataSubarrays, GetDataSubarraysResponse>> OnGetDataSubarraysResponse;

        /// <summary>
        /// Sends a PutUninitializedDataArrays message to a store.
        /// </summary>
        /// <param name="dataArrays">The data arrays.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutUninitializedDataArrays> PutUninitializedDataArrays(IDictionary<string, PutUninitializedDataArrayType> dataArrays, IMessageHeaderExtension extension = null)
        {
            var body = new PutUninitializedDataArrays
            {
                DataArrays = dataArrays ?? new Dictionary<string, PutUninitializedDataArrayType>(),
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Sends a PutUninitializedDataArrays message to a store.
        /// </summary>
        /// <param name="dataArrays">The data arrays.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutUninitializedDataArrays> PutUninitializedDataArrays(IList<PutUninitializedDataArrayType> dataArrays, IMessageHeaderExtension extension = null) => PutUninitializedDataArrays(dataArrays.ToMap(), extension: extension);

        /// <summary>
        /// Handles the PutUninitializedDataArraysResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<PutUninitializedDataArrays, PutUninitializedDataArraysResponse>> OnPutUninitializedDataArraysResponse;

        /// <summary>
        /// Sends a PutDataArrays message to a store.
        /// </summary>
        /// <param name="dataArrays">The data arrays.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutDataArrays> PutDataArrays(IDictionary<string, PutDataArraysType> dataArrays, IMessageHeaderExtension extension = null)
        {
            var body = new PutDataArrays
            {
                DataArrays = dataArrays ?? new Dictionary<string, PutDataArraysType>(),
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Sends a PutDataArrays message to a store.
        /// </summary>
        /// <param name="dataArrays">The data arrays.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutDataArrays> PutDataArrays(IList<PutDataArraysType> dataArrays, IMessageHeaderExtension extension = null) => PutDataArrays(dataArrays.ToMap(), extension: extension);

        /// <summary>
        /// Handles the PutDataArraysResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<PutDataArrays, PutDataArraysResponse>> OnPutDataArraysResponse;

        /// <summary>
        /// Sends a PutDataSubarrays message to a store.
        /// </summary>
        /// <param name="dataSubarrays">The data subarrays.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutDataSubarrays> PutDataSubarrays(IDictionary<string, PutDataSubarraysType> dataSubarrays, IMessageHeaderExtension extension = null)
        {
            var body = new PutDataSubarrays
            {
                DataSubarrays = dataSubarrays ?? new Dictionary<string, PutDataSubarraysType>(),
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Sends a PutDataSubarrays message to a store.
        /// </summary>
        /// <param name="dataSubarrays">The data subarrays.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutDataSubarrays> PutDataSubarrays(IList<PutDataSubarraysType> dataSubarrays, IMessageHeaderExtension extension = null) => PutDataSubarrays(dataSubarrays.ToMap(), extension: extension);

        /// <summary>
        /// Handles the PutDataSubarraysResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<PutDataSubarrays, PutDataSubarraysResponse>> OnPutDataSubarraysResponse;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<GetDataArrayMetadata>)
                HandleResponseMessage(request as EtpMessage<GetDataArrayMetadata>, message, OnGetDataArrayMetadataResponse, HandleGetDataArrayMetadataResponse);
            else if (request is EtpMessage<GetDataArrays>)
                HandleResponseMessage(request as EtpMessage<GetDataArrays>, message, OnGetDataArraysResponse, HandleGetDataArraysResponse);
            else if (request is EtpMessage<GetDataSubarrays>)
                HandleResponseMessage(request as EtpMessage<GetDataSubarrays>, message, OnGetDataSubarraysResponse, HandleGetDataSubarraysResponse);
            else if (request is EtpMessage<PutUninitializedDataArrays>)
                HandleResponseMessage(request as EtpMessage<PutUninitializedDataArrays>, message, OnPutUninitializedDataArraysResponse, HandlePutUninitializedDataArraysResponse);
            else if (request is EtpMessage<PutDataArrays>)
                HandleResponseMessage(request as EtpMessage<PutDataArrays>, message, OnPutDataArraysResponse, HandlePutDataArraysResponse);
            else if (request is EtpMessage<PutDataSubarrays>)
                HandleResponseMessage(request as EtpMessage<PutDataSubarrays>, message, OnPutDataSubarraysResponse, HandlePutDataSubarraysResponse);
        }

        /// <summary>
        /// Handles the GetDataArrayMetadataResponse message from a store.
        /// </summary>
        /// <param name="message">The GetDataArrayMetadataResponse message.</param>
        protected virtual void HandleGetDataArrayMetadataResponse(EtpMessage<GetDataArrayMetadataResponse> message)
        {
            var request = TryGetCorrelatedMessage<GetDataArrayMetadata>(message);
            HandleResponseMessage(request, message, OnGetDataArrayMetadataResponse, HandleGetDataArrayMetadataResponse);
        }

        /// <summary>
        /// Handles the GetDataArrayMetadataResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{GetDataArrayMetadata, GetDataArrayMetadataResponse}"/> instance containing the event data.</param>
        protected virtual void HandleGetDataArrayMetadataResponse(ResponseEventArgs<GetDataArrayMetadata, GetDataArrayMetadataResponse> args)
        {
        }

        /// <summary>
        /// Handles the GetDataArraysResponse message from a store.
        /// </summary>
        /// <param name="message">The GetDataArraysResponse message.</param>
        protected virtual void HandleGetDataArraysResponse(EtpMessage<GetDataArraysResponse> message)
        {
            var request = TryGetCorrelatedMessage<GetDataArrays>(message);
            HandleResponseMessage(request, message, OnGetDataArraysResponse, HandleGetDataArraysResponse);
        }

        /// <summary>
        /// Handles the GetDataArraysResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{GetDataArrays, GetDataArraysResponse}"/> instance containing the event data.</param>
        protected virtual void HandleGetDataArraysResponse(ResponseEventArgs<GetDataArrays, GetDataArraysResponse> args)
        {
        }

        /// <summary>
        /// Handles the GetDataSubarraysResponse message from a store.
        /// </summary>
        /// <param name="message">The GetDataSubarraysResponse message.</param>
        protected virtual void HandleGetDataSubarraysResponse(EtpMessage<GetDataSubarraysResponse> message)
        {
            var request = TryGetCorrelatedMessage<GetDataSubarrays>(message);
            HandleResponseMessage(request, message, OnGetDataSubarraysResponse, HandleGetDataSubarraysResponse);
        }

        /// <summary>
        /// Handles the GetDataSubarraysResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{GetDataSubarrays, GetDataSubarraysResponse}"/> instance containing the event data.</param>
        protected virtual void HandleGetDataSubarraysResponse(ResponseEventArgs<GetDataSubarrays, GetDataSubarraysResponse> args)
        {
        }

        /// <summary>
        /// Handles the PutUninitializedDataArraysResponse message from a store.
        /// </summary>
        /// <param name="message">The PutUninitializedDataArraysResponse message.</param>
        protected virtual void HandlePutUninitializedDataArraysResponse(EtpMessage<PutUninitializedDataArraysResponse> message)
        {
            var request = TryGetCorrelatedMessage<PutUninitializedDataArrays>(message);
            HandleResponseMessage(request, message, OnPutUninitializedDataArraysResponse, HandlePutUninitializedDataArraysResponse);
        }

        /// <summary>
        /// Handles the PutUninitializedDataArraysResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{PutUninitializedDataArrays, PutUninitializedDataArraysResponse}"/> instance containing the event data.</param>
        protected virtual void HandlePutUninitializedDataArraysResponse(ResponseEventArgs<PutUninitializedDataArrays, PutUninitializedDataArraysResponse> args)
        {
        }

        /// <summary>
        /// Handles the PutDataArraysResponse message from a store.
        /// </summary>
        /// <param name="message">The PutDataArraysResponse message.</param>
        protected virtual void HandlePutDataArraysResponse(EtpMessage<PutDataArraysResponse> message)
        {
            var request = TryGetCorrelatedMessage<PutDataArrays>(message);
            HandleResponseMessage(request, message, OnPutDataArraysResponse, HandlePutDataArraysResponse);
        }

        /// <summary>
        /// Handles the PutDataArraysResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{PutDataArrays, PutDataArraysResponse}"/> instance containing the event data.</param>
        protected virtual void HandlePutDataArraysResponse(ResponseEventArgs<PutDataArrays, PutDataArraysResponse> args)
        {
        }

        /// <summary>
        /// Handles the PutDataSubarraysResponse message from a store.
        /// </summary>
        /// <param name="message">The PutDataSubarraysResponse message.</param>
        protected virtual void HandlePutDataSubarraysResponse(EtpMessage<PutDataSubarraysResponse> message)
        {
            var request = TryGetCorrelatedMessage<PutDataSubarrays>(message);
            HandleResponseMessage(request, message, OnPutDataSubarraysResponse, HandlePutDataSubarraysResponse);
        }

        /// <summary>
        /// Handles the PutDataSubarraysResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{PutDataSubarrays, PutDataSubarraysResponse}"/> instance containing the event data.</param>
        protected virtual void HandlePutDataSubarraysResponse(ResponseEventArgs<PutDataSubarrays, PutDataSubarraysResponse> args)
        {
        }
    }
}

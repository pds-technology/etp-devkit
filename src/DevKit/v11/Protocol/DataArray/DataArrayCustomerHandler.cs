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
using Energistics.Etp.Common.Protocol.Core;
using Energistics.Etp.v11.Datatypes;

namespace Energistics.Etp.v11.Protocol.DataArray
{
    /// <summary>
    /// Base implementation of the <see cref="IDataArrayCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp11ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v11.Protocol.DataArray.IDataArrayCustomer" />
    public class DataArrayCustomerHandler : Etp11ProtocolHandler, IDataArrayCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataArrayCustomerHandler"/> class.
        /// </summary>
        public DataArrayCustomerHandler() : base((int)Protocols.DataArray, Roles.Customer, Roles.Store)
        {
            RegisterMessageHandler<DataArray>(Protocols.DataArray, MessageTypes.DataArray.DataArray, HandleDataArray);
        }

        /// <summary>
        /// Gets the data array by URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual EtpMessage<GetDataArray> GetDataArray(string uri)
        {
            var body = new GetDataArray
            {
                Uri = uri ?? string.Empty
            };

            return SendRequest(body);
        }

        /// <summary>
        /// Gets the data array slice by URI, start and count.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual EtpMessage<GetDataArraySlice> GetDataArraySlice(string uri, IList<long> start, IList<long> count)
        {
            var body = new GetDataArraySlice
            {
                Uri = uri ?? string.Empty,
                Start = start ?? new List<long>(),
                Count = count ?? new List<long>(),
            };

            return SendRequest(body);
        }

        /// <summary>
        /// Puts a data array in the data store.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="data">The data array.</param>
        /// <param name="dimensions">The dimensions.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual EtpMessage<PutDataArray> PutDataArray(string uri, AnyArray data, IList<long> dimensions)
        {
            var body = new PutDataArray
            {
                Uri = uri ?? string.Empty,
                Data = data,
                Dimensions = dimensions ?? new List<long>(),
            };

            return SendRequest(body);
        }

        /// <summary>
        /// Puts a data array slice in the data store.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="data">The data array.</param>
        /// <param name="dimensions">The dimensions.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual EtpMessage<PutDataArraySlice> PutDataArraySlice(string uri, AnyArray data, IList<long> dimensions, IList<long> start, IList<long> count)
        {
            var body = new PutDataArraySlice
            {
                Uri = uri ?? string.Empty,
                Data = data,
                Dimensions = dimensions ?? new List<long>(),
                Start = start ?? new List<long>(),
                Count = count ?? new List<long>(),
            };

            return SendRequest(body);
        }

        /// <summary>
        /// Handles the DataArray event from a store when sent in response to a GetDataArray.
        /// </summary>
        public event EventHandler<ResponseEventArgs<GetDataArray, DataArray>> OnGetDataArrayDataArray;

        /// <summary>
        /// Handles the DataArray event from a store when sent in response to a GetDataArraySlice.
        /// </summary>
        public event EventHandler<ResponseEventArgs<GetDataArraySlice, DataArray>> OnGetDataArraySliceDataArray;

        /// <summary>
        /// Event raised when there is an exception received in response to a PutDataArray message.
        /// </summary>
        public event EventHandler<VoidResponseEventArgs<PutDataArray>> OnPutDataArrayException;

        /// <summary>
        /// Event raised when there is an exception received in response to a PutDataArraySlice message.
        /// </summary>
        public event EventHandler<VoidResponseEventArgs<PutDataArraySlice>> OnPutDataArraySliceException;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<GetDataArray>)
                HandleResponseMessage(request as EtpMessage<GetDataArray>, message, OnGetDataArrayDataArray, HandleGetDataArrayDataArray);
            else if (request is EtpMessage<GetDataArraySlice>)
                HandleResponseMessage(request as EtpMessage<GetDataArraySlice>, message, OnGetDataArraySliceDataArray, HandleGetDataArraySliceDataArray);
            else if (request is EtpMessage<PutDataArray>)
                HandleResponseMessage(request as EtpMessage<PutDataArray>, message, OnPutDataArrayException, HandlePutDataArrayException);
            else if (request is EtpMessage<PutDataArraySlice>)
                HandleResponseMessage(request as EtpMessage<PutDataArraySlice>, message, OnPutDataArraySliceException, HandlePutDataArraySliceException);
        }

        /// <summary>
        /// Handles the DataArray message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        protected virtual void HandleDataArray(EtpMessage<DataArray> message)
        {
            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<GetDataArray>)
                HandleResponseMessage(request as EtpMessage<GetDataArray>, message, OnGetDataArrayDataArray, HandleGetDataArrayDataArray);
            else if (request is EtpMessage<GetDataArraySlice>)
                HandleResponseMessage(request as EtpMessage<GetDataArraySlice>, message, OnGetDataArraySliceDataArray, HandleGetDataArraySliceDataArray);
        }

        /// <summary>
        /// Handles the DataArray message when sent in response to a GetDataArray message.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{GetDataArray, DataArray}"/> instance containing the event data.</param>
        protected virtual void HandleGetDataArrayDataArray(ResponseEventArgs<GetDataArray, DataArray> args)
        {
        }

        /// <summary>
        /// Handles the DataArray message when sent in response to a GetDataArraySlice message.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{GetDataArraySlice, DataArray}"/> instance containing the event data.</param>
        protected virtual void HandleGetDataArraySliceDataArray(ResponseEventArgs<GetDataArraySlice, DataArray> args)
        {
        }

        /// <summary>
        /// Handles exceptions to the PutDataArray message from a store.
        /// </summary>
        /// <param name="args">The <see cref="VoidResponseEventArgs{PutDataArray}"/> instance containing the event data.</param>
        protected virtual void HandlePutDataArrayException(VoidResponseEventArgs<PutDataArray> args)
        {
        }

        /// <summary>
        /// Handles exceptions to the PutDataArraySlice message from a store.
        /// </summary>
        /// <param name="args">The <see cref="VoidResponseEventArgs{PutDataArraySlice}"/> instance containing the event data.</param>
        protected virtual void HandlePutDataArraySliceException(VoidResponseEventArgs<PutDataArraySlice> args)
        {
        }
    }
}

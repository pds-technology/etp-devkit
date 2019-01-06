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
        public DataArrayCustomerHandler() : base((int)Protocols.DataArray, "customer", "store")
        {
            RegisterMessageHandler<DataArray>(Protocols.DataArray, MessageTypes.DataArray.DataArray, HandleDataArray);
        }

        /// <summary>
        /// Gets the data array by URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The message identifier.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public long GetDataArray(string uri)
        {
            var header = CreateMessageHeader(Protocols.DataArray, MessageTypes.DataArray.GetDataArray);

            var message = new GetDataArray
            {
                Uri = uri
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Gets the data array slice by URI, start and count.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns>The message identifier.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public long GetDataArraySlice(string uri, IList<long> start, IList<long> count)
        {
            var header = CreateMessageHeader(Protocols.DataArray, MessageTypes.DataArray.GetDataArraySlice);

            var message = new GetDataArraySlice
            {
                Uri = uri,
                Start = start,
                Count = count
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Puts a data array in the data store.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="data">The data array.</param>
        /// <param name="dimensions">The dimensions.</param>
        /// <returns>The message identifier.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public long PutDataArray(string uri, AnyArray data, IList<long> dimensions)
        {
            var header = CreateMessageHeader(Protocols.DataArray, MessageTypes.DataArray.PutDataArray);

            var message = new PutDataArray
            {
                Uri = uri,
                Data = data,
                Dimensions = dimensions
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Puts a data array slice in the data store.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="data">The data array.</param>
        /// <param name="dimensions">The dimensions.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns>The message identifier.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public long PutDataArraySlice(string uri, AnyArray data, IList<long> dimensions, IList<long> start, IList<long> count)
        {
            var header = CreateMessageHeader(Protocols.DataArray, MessageTypes.DataArray.PutDataArraySlice);

            var message = new PutDataArraySlice
            {
                Uri = uri,
                Data = data,
                Dimensions = dimensions,
                Start = start,
                Count = count
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Handles the DataArray event from a store.
        /// </summary>
        public event ProtocolEventHandler<DataArray> OnDataArray;

        /// <summary>
        /// Handles the DataArray message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The DataArray message.</param>
        protected virtual void HandleDataArray(IMessageHeader header, DataArray message)
        {
            Notify(OnDataArray, header, message);
        }
    }
}

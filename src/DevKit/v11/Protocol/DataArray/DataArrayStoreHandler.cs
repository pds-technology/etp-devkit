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
    /// Base implementation of the <see cref="IDataArrayStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp11ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v11.Protocol.DataArray.IDataArrayStore" />
    public class DataArrayStoreHandler : Etp11ProtocolHandler, IDataArrayStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataArrayStoreHandler"/> class.
        /// </summary>
        public DataArrayStoreHandler() : base((int)Protocols.DataArray, "store", "customer")
        {
            RegisterMessageHandler<GetDataArray>(Protocols.DataArray, MessageTypes.DataArray.GetDataArray, HandleGetDataArray);
            RegisterMessageHandler<GetDataArraySlice>(Protocols.DataArray, MessageTypes.DataArray.GetDataArraySlice, HandleGetDataArraySlice);
            RegisterMessageHandler<PutDataArray>(Protocols.DataArray, MessageTypes.DataArray.PutDataArray, HandlePutDataArray);
            RegisterMessageHandler<PutDataArraySlice>(Protocols.DataArray, MessageTypes.DataArray.PutDataArraySlice, HandlePutDataArraySlice);
        }

        /// <summary>
        /// Sends a data array as a response for GetDataArray and GetDataArraySlice.
        /// </summary>
        /// <param name="dimensions">The dimensions.</param>
        /// <param name="data">The data array.</param>
        /// <returns>The message identifier.</returns>
        public long DataArray(IList<long> dimensions, AnyArray data)
        {
            var header = CreateMessageHeader(Protocols.DataArray, MessageTypes.DataArray.DataArray);

            var message = new DataArray
            {
                Dimensions = dimensions,
                Data = data
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Handles the GetDataArray event from a store.
        /// </summary>
        public event ProtocolEventHandler<GetDataArray> OnGetDataArray;

        /// <summary>
        /// Handles the GetDataArraySlice event from a store.
        /// </summary>
        public event ProtocolEventHandler<GetDataArraySlice> OnGetDataArraySlice;

        /// <summary>
        /// Handles the PutDataArray event from a store.
        /// </summary>
        public event ProtocolEventHandler<PutDataArray> OnPutDataArray;

        /// <summary>
        /// Handles the PutDataArraySlice event from a store.
        /// </summary>
        public event ProtocolEventHandler<PutDataArraySlice> OnPutDataArraySlice;

        /// <summary>
        /// Handles the GetDataArray message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GetDataArray message.</param>
        protected virtual void HandleGetDataArray(IMessageHeader header, GetDataArray message)
        {
            Notify(OnGetDataArray, header, message);
        }

        /// <summary>
        /// Handles the GetDataArraySlice message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GetDataArraySlice message.</param>
        protected virtual void HandleGetDataArraySlice(IMessageHeader header, GetDataArraySlice message)
        {
            Notify(OnGetDataArraySlice, header, message);
        }

        /// <summary>
        /// Handles the PutDataArray message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The PutDataArray message.</param>
        protected virtual void HandlePutDataArray(IMessageHeader header, PutDataArray message)
        {
            Notify(OnPutDataArray, header, message);
        }

        /// <summary>
        /// Handles the PutDataArraySlice message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The PutDataArraySlice message.</param>
        protected virtual void HandlePutDataArraySlice(IMessageHeader header, PutDataArraySlice message)
        {
            Notify(OnPutDataArraySlice, header, message);
        }
    }
}

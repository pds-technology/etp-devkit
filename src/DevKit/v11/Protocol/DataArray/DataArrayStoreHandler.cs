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
        public DataArrayStoreHandler() : base((int)Protocols.DataArray, Roles.Store, Roles.Customer)
        {
            RegisterMessageHandler<GetDataArray>(Protocols.DataArray, MessageTypes.DataArray.GetDataArray, HandleGetDataArray);
            RegisterMessageHandler<GetDataArraySlice>(Protocols.DataArray, MessageTypes.DataArray.GetDataArraySlice, HandleGetDataArraySlice);
            RegisterMessageHandler<PutDataArray>(Protocols.DataArray, MessageTypes.DataArray.PutDataArray, HandlePutDataArray);
            RegisterMessageHandler<PutDataArraySlice>(Protocols.DataArray, MessageTypes.DataArray.PutDataArraySlice, HandlePutDataArraySlice);
        }

        /// <summary>
        /// Sends a data array as a response for GetDataArray and GetDataArraySlice.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="array">The data array.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<DataArray> DataArray(IMessageHeader correlatedHeader, DataArray array)
        {
            var body = array;

            return SendResponse(body, correlatedHeader);
        }

        /// <summary>
        /// Handles the GetDataArray event from a customer.
        /// </summary>
        public event EventHandler<RequestEventArgs<GetDataArray, DataArray>> OnGetDataArray;

        /// <summary>
        /// Handles the GetDataArraySlice event from a customer.
        /// </summary>
        public event EventHandler<RequestEventArgs<GetDataArraySlice, DataArray>> OnGetDataArraySlice;

        /// <summary>
        /// Handles the PutDataArray event from a customer.
        /// </summary>
        public event EventHandler<VoidRequestEventArgs<PutDataArray>> OnPutDataArray;

        /// <summary>
        /// Handles the PutDataArraySlice event from a customer.
        /// </summary>
        public event EventHandler<VoidRequestEventArgs<PutDataArraySlice>> OnPutDataArraySlice;

        /// <summary>
        /// Handles the GetDataArray message from a customer.
        /// </summary>
        /// <param name="message">The GetDataArray message.</param>
        protected virtual void HandleGetDataArray(EtpMessage<GetDataArray> message)
        {
            HandleRequestMessage(message, OnGetDataArray, HandleGetDataArray,
                responseMethod: (args) => DataArray(args.Request?.Header, args.Response));
        }

        /// <summary>
        /// Handles the GetDataArray message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="RequestEventArgs{GetDataArray, DataArray}"/> instance containing the event data.</param>
        protected virtual void HandleGetDataArray(RequestEventArgs<GetDataArray, DataArray> args)
        {
        }

        /// <summary>
        /// Handles the GetDataArraySlice message from a customer.
        /// </summary>
        /// <param name="message">The GetDataArraySlice message.</param>
        protected virtual void HandleGetDataArraySlice(EtpMessage<GetDataArraySlice> message)
        {
            HandleRequestMessage(message, OnGetDataArraySlice,  HandleGetDataArraySlice,
                responseMethod: (args) => DataArray(args.Request?.Header, args.Response));
        }

        /// <summary>
        /// Handles the GetDataArraySlice message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="RequestEventArgs{GetDataArraySlice, DataArray}"/> instance containing the event data.</param>
        protected virtual void HandleGetDataArraySlice(RequestEventArgs<GetDataArraySlice, DataArray> args)
        {
        }

        /// <summary>
        /// Handles the PutDataArray message from a customer.
        /// </summary>
        /// <param name="message">The PutDataArray message.</param>
        protected virtual void HandlePutDataArray(EtpMessage<PutDataArray> message)
        {
            HandleRequestMessage(message, OnPutDataArray, HandlePutDataArray);
        }

        /// <summary>
        /// Handles the PutDataArray message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="VoidRequestEventArgs{PutDataArray}"/> instance containing the event data.</param>
        protected virtual void HandlePutDataArray(VoidRequestEventArgs<PutDataArray> args)
        {
        }

        /// <summary>
        /// Handles the PutDataArraySlice message from a customer.
        /// </summary>
        /// <param name="message">The PutDataArraySlice message.</param>
        protected virtual void HandlePutDataArraySlice(EtpMessage<PutDataArraySlice> message)
        {
            HandleRequestMessage( message, OnPutDataArraySlice, HandlePutDataArraySlice);
        }

        /// <summary>
        /// Handles the PutDataArraySlice message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="VoidRequestEventArgs{PutDataArraySlice}"/> instance containing the event data.</param>
        protected virtual void HandlePutDataArraySlice(VoidRequestEventArgs<PutDataArraySlice> args)
        {
        }
    }
}

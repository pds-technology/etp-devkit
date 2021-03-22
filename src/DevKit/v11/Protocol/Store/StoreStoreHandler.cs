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

using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v11.Datatypes.Object;
using System;

namespace Energistics.Etp.v11.Protocol.Store
{
    /// <summary>
    /// Base implementation of the <see cref="IStoreStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp11ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v11.Protocol.Store.IStoreStore" />
    public class StoreStoreHandler : Etp11ProtocolHandler, IStoreStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreStoreHandler"/> class.
        /// </summary>
        public StoreStoreHandler() : base((int)Protocols.Store, Roles.Store, Roles.Customer)
        {
            RegisterMessageHandler<GetObject>(Protocols.Store, MessageTypes.Store.GetObject, HandleGetObject);
            RegisterMessageHandler<PutObject>(Protocols.Store, MessageTypes.Store.PutObject, HandlePutObject);
            RegisterMessageHandler<DeleteObject>(Protocols.Store, MessageTypes.Store.DeleteObject, HandleDeleteObject);
        }

        /// <summary>
        /// Sends an Object message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="dataObject">The data object.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<Object> Object(IMessageHeader correlatedHeader, DataObject dataObject)
        {
            var body = new Object()
            {
                DataObject = dataObject
            };

            return SendResponse(body, correlatedHeader, isMultiPart: true, isFinalPart: true, isNoData: dataObject?.Data?.Length == 0);
        }

        /// <summary>
        /// Handles the GetObject event from a customer.
        /// </summary>
        public event EventHandler<RequestEventArgs<GetObject, DataObject>> OnGetObject;

        /// <summary>
        /// Handles the PutObject event from a customer.
        /// </summary>
        public event EventHandler<VoidRequestEventArgs<PutObject>> OnPutObject;

        /// <summary>
        /// Handles the DeleteObject event from a customer.
        /// </summary>
        public event EventHandler<VoidRequestEventArgs<DeleteObject>> OnDeleteObject;

        /// <summary>
        /// Handles the GetObject message from a customer.
        /// </summary>
        /// <param name="message">The GetObject message.</param>
        protected virtual void HandleGetObject(EtpMessage<GetObject> message)
        {
            HandleRequestMessage(message, OnGetObject, HandleGetObject,
                responseMethod: (args) => Object(args.Request?.Header, args.Response));
        }

        /// <summary>
        /// Handles the GetObject message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="RequestEventArgs{GetObject, DataObject}"/> instance containing the event data.</param>
        protected virtual void HandleGetObject(RequestEventArgs<GetObject, DataObject> args)
        {
        }

        /// <summary>
        /// Handles the PutObject message from a customer.
        /// </summary>
        /// <param name="message">The PutObject message.</param>
        protected virtual void HandlePutObject(EtpMessage<PutObject> message)
        {
            HandleRequestMessage(message, OnPutObject, HandlePutObject);
        }

        /// <summary>
        /// Handles the PutObject message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="VoidRequestEventArgs{PutObject}"/> instance containing the event data.</param>
        protected virtual void HandlePutObject(VoidRequestEventArgs<PutObject> args)
        {
        }

        /// <summary>
        /// Handles the DeleteObject message from a customer.
        /// </summary>
        /// <param name="message">The DeleteObject message.</param>
        protected virtual void HandleDeleteObject(EtpMessage<DeleteObject> message)
        {
            HandleRequestMessage(message, OnDeleteObject, HandleDeleteObject);
        }

        /// <summary>
        /// Handles the DeleteObject message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="VoidRequestEventArgs{DeleteObject}"/> instance containing the event data.</param>
        protected virtual void HandleDeleteObject(VoidRequestEventArgs<DeleteObject> args)
        {
        }
    }
}

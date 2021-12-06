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
using Energistics.Etp.Common.Protocol.Core;
using Energistics.Etp.v11.Datatypes.Object;
using System;

namespace Energistics.Etp.v11.Protocol.Store
{
    /// <summary>
    /// Base implementation of the <see cref="IStoreCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp11ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v11.Protocol.Store.IStoreCustomer" />
    public class StoreCustomerHandler : Etp11ProtocolHandler, IStoreCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreCustomerHandler"/> class.
        /// </summary>
        public StoreCustomerHandler() : base((int)Protocols.Store, Roles.Customer, Roles.Store)
        {
            RegisterMessageHandler<Object>(Protocols.Store, MessageTypes.Store.Object, HandleObject);
        }

        /// <summary>
        /// Sends a GetObject message to a store.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetObject> GetObject(string uri)
        {
            var body = new GetObject()
            {
                Uri = uri ?? string.Empty,
            };

            return SendRequest(body, isMultiPart: true, isFinalPart: true);
        }

        /// <summary>
        /// Sends a PutObject message to a store.
        /// </summary>
        /// <param name="dataObject">The data object.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutObject> PutObject(DataObject dataObject)
        {
            var body = new PutObject()
            {
                DataObject = dataObject,
            };

            return SendNotification(body);
        }

        /// <summary>
        /// Sends a DeleteObject message to a store.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<DeleteObject> DeleteObject(string uri)
        {
            var body = new DeleteObject()
            {
                Uri = uri ?? string.Empty,
            };

            return SendNotification(body);
        }

        /// <summary>
        /// Handles the Object event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<GetObject, Object>> OnObject;

        /// <summary>
        /// Event raised when there is an exception received in response to a PutObject message.
        /// </summary>
        public event EventHandler<VoidResponseEventArgs<PutObject>> OnPutObjectException;

        /// <summary>
        /// Event raised when there is an exception received in response to a DeleteObject message.
        /// </summary>
        public event EventHandler<VoidResponseEventArgs<DeleteObject>> OnDeleteObjectException;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<GetObject>)
                HandleResponseMessage(request as EtpMessage<GetObject>, message, OnObject, HandleObject);
            else if (request is EtpMessage<PutObject>)
                HandleResponseMessage(request as EtpMessage<PutObject>, message, OnPutObjectException, HandlePutObjectException);
            else if (request is EtpMessage<DeleteObject>)
                HandleResponseMessage(request as EtpMessage<DeleteObject>, message, OnDeleteObjectException, HandleDeleteObjectException);
        }

        /// <summary>
        /// Handles the Object message from a store.
        /// </summary>
        /// <param name="message">The Object message.</param>
        protected virtual void HandleObject(EtpMessage<Object> message)
        {
            var request = TryGetCorrelatedMessage<GetObject>(message);
            HandleResponseMessage(request, message, OnObject, HandleObject);
        }

        /// <summary>
        /// Handles the Object message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{GetObject, Object}"/> instance containing the event data.</param>
        protected virtual void HandleObject(ResponseEventArgs<GetObject, Object> args)
        {
        }

        /// <summary>
        /// Handles exceptions to the PutObject message from a store.
        /// </summary>
        /// <param name="args">The <see cref="VoidResponseEventArgs{PutObject}"/> instance containing the event data.</param>
        protected virtual void HandlePutObjectException(VoidResponseEventArgs<PutObject> args)
        {
        }

        /// <summary>
        /// Handles exceptions to the DeleteObject message from a store.
        /// </summary>
        /// <param name="args">The <see cref="VoidResponseEventArgs{DeleteObject}"/> instance containing the event data.</param>
        protected virtual void HandleDeleteObjectException(VoidResponseEventArgs<DeleteObject> args)
        {
        }
    }
}

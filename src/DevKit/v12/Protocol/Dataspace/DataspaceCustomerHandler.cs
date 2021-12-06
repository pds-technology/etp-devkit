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

namespace Energistics.Etp.v12.Protocol.Dataspace
{
    /// <summary>
    /// Base implementation of the <see cref="IDataspaceCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.Dataspace.IDataspaceCustomer" />
    public class DataspaceCustomerHandler : Etp12ProtocolHandler<CapabilitiesCustomer, ICapabilitiesCustomer, CapabilitiesStore, ICapabilitiesStore>, IDataspaceCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataspaceCustomerHandler"/> class.
        /// </summary>
        public DataspaceCustomerHandler() : base((int)Protocols.Dataspace, Roles.Customer, Roles.Store)
        {
            RegisterMessageHandler<GetDataspacesResponse>(Protocols.Dataspace, MessageTypes.Dataspace.GetDataspacesResponse, HandleGetDataspacesResponse);
            RegisterMessageHandler<PutDataspacesResponse>(Protocols.Dataspace, MessageTypes.Dataspace.PutDataspacesResponse, HandlePutDataspacesResponse);
            RegisterMessageHandler<DeleteDataspacesResponse>(Protocols.Dataspace, MessageTypes.Dataspace.DeleteDataspacesResponse, HandleDeleteDataspacesResponse);
        }

        /// <summary>
        /// Sends a GetDataspaces message to a store.
        /// </summary>
        /// <param name="storeLastWriteFilter">An optional filter to limit the dataspaces returned by store last write.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetDataspaces> GetDataspaces(DateTime? storeLastWriteFilter = null, IMessageHeaderExtension extension = null)
        {
            var body = new GetDataspaces
            {
                StoreLastWriteFilter = storeLastWriteFilter,
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Handles the GetDataspacesResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<GetDataspaces, GetDataspacesResponse>> OnGetDataspacesResponse;

        /// <summary>
        /// Sends a PutDataspaces message to a store.
        /// </summary>
        /// <param name="dataspaces">The dataspaces.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutDataspaces> PutDataspaces(IDictionary<string, Datatypes.Object.Dataspace> dataspaces, IMessageHeaderExtension extension = null)
        {
            var body = new PutDataspaces
            {
                Dataspaces = dataspaces ?? new Dictionary<string, Datatypes.Object.Dataspace>(),
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Sends a PutDataspaces message to a store.
        /// </summary>
        /// <param name="dataspaces">The dataspaces.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutDataspaces> PutDataspaces(IList<Datatypes.Object.Dataspace> dataspaces, IMessageHeaderExtension extension = null) => PutDataspaces(dataspaces.ToMap(), extension: extension);

        /// <summary>
        /// Handles the PutDataspacesResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<PutDataspaces, PutDataspacesResponse>> OnPutDataspacesResponse;

        /// <summary>
        /// Sends a DeleteDataspaces message to a store.
        /// </summary>
        /// <param name="uris">The URIs.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<DeleteDataspaces> DeleteDataspaces(IDictionary<string, string> uris, IMessageHeaderExtension extension = null)
        {
            var body = new DeleteDataspaces
            {
                Uris = uris ?? new Dictionary<string, string>(),
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Sends a DeleteDataspaces message to a store.
        /// </summary>
        /// <param name="uris">The URIs.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<DeleteDataspaces> DeleteDataspaces(IList<string> uris, IMessageHeaderExtension extension = null) => DeleteDataspaces(uris.ToMap(), extension: extension);

        /// <summary>
        /// Handles the DeleteDataspacesResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<DeleteDataspaces, DeleteDataspacesResponse>> OnDeleteDataspacesResponse;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<GetDataspaces>)
                HandleResponseMessage(request as EtpMessage<GetDataspaces>, message, OnGetDataspacesResponse, HandleGetDataspacesResponse);
            else if (request is EtpMessage<PutDataspaces>)
                HandleResponseMessage(request as EtpMessage<PutDataspaces>, message, OnPutDataspacesResponse, HandlePutDataspacesResponse);
            else if (request is EtpMessage<DeleteDataspaces>)
                HandleResponseMessage(request as EtpMessage<DeleteDataspaces>, message, OnDeleteDataspacesResponse, HandleDeleteDataspacesResponse);
        }

        /// <summary>
        /// Handles the GetDataspacesResponse message from a store.
        /// </summary>
        /// <param name="message">The GetDataspacesResponse message.</param>
        protected virtual void HandleGetDataspacesResponse(EtpMessage<GetDataspacesResponse> message)
        {
            var request = TryGetCorrelatedMessage<GetDataspaces>(message);
            HandleResponseMessage(request, message, OnGetDataspacesResponse, HandleGetDataspacesResponse);
        }

        /// <summary>
        /// Handles the GetDataspacesResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{GetDataspaces, GetDataspacesResponse}"/> instance containing the event data.</param>
        protected virtual void HandleGetDataspacesResponse(ResponseEventArgs<GetDataspaces, GetDataspacesResponse> args)
        {
        }

        /// <summary>
        /// Handles the PutDataspacesResponse message from a store.
        /// </summary>
        /// <param name="message">The PutDataspacesResponse message.</param>
        protected virtual void HandlePutDataspacesResponse(EtpMessage<PutDataspacesResponse> message)
        {
            var request = TryGetCorrelatedMessage<PutDataspaces>(message);
            HandleResponseMessage(request, message, OnPutDataspacesResponse, HandlePutDataspacesResponse);
        }

        /// <summary>
        /// Handles the PutDataspacesResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{PutDataspaces, PutDataspacesResponse}"/> instance containing the event data.</param>
        protected virtual void HandlePutDataspacesResponse(ResponseEventArgs<PutDataspaces, PutDataspacesResponse> args)
        {
        }

        /// <summary>
        /// Handles the DeleteDataspacesResponse message from a store.
        /// </summary>
        /// <param name="message">The DeleteDataspacesResponse message.</param>
        protected virtual void HandleDeleteDataspacesResponse(EtpMessage<DeleteDataspacesResponse> message)
        {
            var request = TryGetCorrelatedMessage<DeleteDataspaces>(message);
            HandleResponseMessage(request, message, OnDeleteDataspacesResponse, HandleDeleteDataspacesResponse);
        }

        /// <summary>
        /// Handles the DeleteDataspacesResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{DeleteDataspaces, DeleteDataspacesResponse}"/> instance containing the event data.</param>
        protected virtual void HandleDeleteDataspacesResponse(ResponseEventArgs<DeleteDataspaces, DeleteDataspacesResponse> args)
        {
        }
    }
}

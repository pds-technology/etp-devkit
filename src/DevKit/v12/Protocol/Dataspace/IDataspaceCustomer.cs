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

namespace Energistics.Etp.v12.Protocol.Dataspace
{
    /// <summary>
    /// Defines the interface that must be implemented by the customer role of the Dataspace protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.Dataspace, Roles.Customer, Roles.Store)]
    public interface IDataspaceCustomer : IProtocolHandler<ICapabilitiesCustomer, ICapabilitiesStore>
    {
        /// <summary>
        /// Sends a GetDataspaces message to a store.
        /// </summary>
        /// <param name="storeLastWriteFilter">An optional filter to limit the dataspaces returned by store last write.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetDataspaces> GetDataspaces(DateTime? storeLastWriteFilter = null, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the GetDataspacesResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<GetDataspaces, GetDataspacesResponse>> OnGetDataspacesResponse;

        /// <summary>
        /// Sends a PutDataspaces message to a store.
        /// </summary>
        /// <param name="dataspaces">The dataspaces.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<PutDataspaces> PutDataspaces(IDictionary<string, Datatypes.Object.Dataspace> dataspaces, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a PutDataspaces message to a store.
        /// </summary>
        /// <param name="dataspaces">The dataspaces.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<PutDataspaces> PutDataspaces(IList<Datatypes.Object.Dataspace> dataspaces, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the PutDataspacesResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<PutDataspaces, PutDataspacesResponse>> OnPutDataspacesResponse;

        /// <summary>
        /// Sends a DeleteDataspaces message to a store.
        /// </summary>
        /// <param name="uris">The URIs.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<DeleteDataspaces> DeleteDataspaces(IDictionary<string, string> uris, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a DeleteDataspaces message to a store.
        /// </summary>
        /// <param name="uris">The URIs.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<DeleteDataspaces> DeleteDataspaces(IList<string> uris, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the DeleteDataspacesResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<DeleteDataspaces, DeleteDataspacesResponse>> OnDeleteDataspacesResponse;
    }
}

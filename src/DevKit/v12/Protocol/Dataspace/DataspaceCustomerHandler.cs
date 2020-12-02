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

namespace Energistics.Etp.v12.Protocol.Dataspace
{
    /// <summary>
    /// Base implementation of the <see cref="IDataspaceCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.Dataspace.IDataspaceCustomer" />
    public class DataspaceCustomerHandler : Etp12ProtocolHandler, IDataspaceCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataspaceCustomerHandler"/> class.
        /// </summary>
        public DataspaceCustomerHandler() : base((int)Protocols.Dataspace, "customer", "store")
        {
            RegisterMessageHandler<GetDataspacesResponse>(Protocols.Dataspace, MessageTypes.Dataspace.GetDataspacesResponse, HandleGetDataspacesResponse);
        }

        /// <summary>
        /// Sends a GetDataspaces message to a store.
        /// </summary>
        /// <param name="lastChangedFilter">An optional filter to limit the dataspaces returned by date last changed.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long GetDataspaces(long? lastChangedFilter)
        {
            var header = CreateMessageHeader(Protocols.Dataspace, MessageTypes.Dataspace.GetDataspaces);

            var message = new GetDataspaces
            {
                LastChangedFilter = lastChangedFilter,
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Handles the GetDataspacesResponse event from a store.
        /// </summary>
        public event ProtocolEventHandler<GetDataspacesResponse> OnGetDataspacesResponse;

        /// <summary>
        /// Sends a PutDataspaces message to a store.
        /// </summary>
        /// <param name="dataspaces">The dataspaces.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long PutDataspaces(IList<Datatypes.Object.Dataspace> dataspaces)
        {
            var header = CreateMessageHeader(Protocols.Dataspace, MessageTypes.Dataspace.PutDataspaces);

            var message = new PutDataspaces
            {
                Dataspaces = dataspaces.ToMap(),
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Sends a DeleteDataspaces message to a store.
        /// </summary>
        /// <param name="uris">The URIs.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long DeleteDataspaces(IList<string> uris)
        {
            var header = CreateMessageHeader(Protocols.Dataspace, MessageTypes.Dataspace.DeleteDataspaces);

            var deleteObject = new DeleteDataspaces
            {
                Uris = uris.ToMap(),
            };

            return Session.SendMessage(header, deleteObject);
        }
                
        /// <summary>
        /// Handles the GetDataspacesResponse message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="response">The GetDataspacesResponse message.</param>
        protected virtual void HandleGetDataspacesResponse(IMessageHeader header, GetDataspacesResponse response)
        {
            Notify(OnGetDataspacesResponse, header, response);
        }
    }
}

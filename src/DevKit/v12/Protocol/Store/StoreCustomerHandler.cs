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
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.Store
{
    /// <summary>
    /// Base implementation of the <see cref="IStoreCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.Store.IStoreCustomer" />
    public class StoreCustomerHandler : Etp12ProtocolHandler, IStoreCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreCustomerHandler"/> class.
        /// </summary>
        public StoreCustomerHandler() : base((int)Protocols.Store, "customer", "store")
        {
            RegisterMessageHandler<GetDataObjectsResponse>(Protocols.Store, MessageTypes.Store.GetDataObjectsResponse, HandleGetDataObjectsResponse);
        }

        /// <summary>
        /// Sends a GetDataObjects message to a store.
        /// </summary>
        /// <param name="uris">The URIs.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        public virtual long GetDataObjects(IList<string> uris, MessageFlags messageFlag = MessageFlags.MultiPartAndFinalPart)
        {
            var header = CreateMessageHeader(Protocols.Store, MessageTypes.Store.GetDataObjects, messageFlags: messageFlag);

            var getObject = new GetDataObjects
            {
                Uris = uris
            };

            return Session.SendMessage(header, getObject);
        }

        /// <summary>
        /// Sends a PutDataObjects message to a store.
        /// </summary>
        /// <param name="dataObjects">The data objects.</param>
        /// <returns>The message identifier.</returns>
        public virtual long PutDataObjects(IList<DataObject> dataObjects)
        {
            var header = CreateMessageHeader(Protocols.Store, MessageTypes.Store.PutDataObjects);

            var putObject = new PutDataObjects
            {
                DataObjects = dataObjects
            };

            return Session.SendMessage(header, putObject);
        }

        /// <summary>
        /// Sends a DeleteDataObjects message to a store.
        /// </summary>
        /// <param name="uris">The URIs.</param>
        /// <returns>The message identifier.</returns>
        public virtual long DeleteDataObjects(IList<string> uris)
        {
            var header = CreateMessageHeader(Protocols.Store, MessageTypes.Store.DeleteDataObjects);

            var deleteObject = new DeleteDataObjects
            {
                Uris = uris
            };

            return Session.SendMessage(header, deleteObject);
        }

        /// <summary>
        /// Handles the GetDataObjectsResponse event from a store.
        /// </summary>
        public event ProtocolEventHandler<GetDataObjectsResponse> OnGetDataObjectsResponse;

        /// <summary>
        /// Handles the GetDataObjectsResponse message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="response">The GetDataObjectsResponse message.</param>
        protected virtual void HandleGetDataObjectsResponse(IMessageHeader header, GetDataObjectsResponse response)
        {
            Notify(OnGetDataObjectsResponse, header, response);
        }
    }
}

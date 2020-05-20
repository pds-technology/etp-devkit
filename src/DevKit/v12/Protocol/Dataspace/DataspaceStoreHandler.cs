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
using Energistics.Etp.v12.Datatypes;

namespace Energistics.Etp.v12.Protocol.Dataspace
{
    /// <summary>
    /// Base implementation of the <see cref="IDataspaceStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.Dataspace.IDataspaceStore" />
    public class DataspaceStoreHandler : Etp12ProtocolHandler, IDataspaceStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataspaceStoreHandler"/> class.
        /// </summary>
        public DataspaceStoreHandler() : base((int)Protocols.Dataspace, "store", "customer")
        {
            RegisterMessageHandler<GetDataspaces>(Protocols.Dataspace, MessageTypes.Dataspace.GetDataspaces, HandleGetDataspaces);
            RegisterMessageHandler<PutDataspaces>(Protocols.Dataspace, MessageTypes.Dataspace.PutDataspaces, HandlePutDataspaces);
            RegisterMessageHandler<DeleteDataspaces>(Protocols.Dataspace, MessageTypes.Dataspace.DeleteDataspaces, HandleDeleteDataspaces);
        }

        /// <summary>
        /// Handles the GetDataspaces event from a customer.
        /// </summary>
        public event ProtocolEventHandler<GetDataspaces, IList<Datatypes.Object.Dataspace>> OnGetDataspaces;

        /// <summary>
        /// Sends an GetDataspacesResponse message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="dataspaces">The dataspaces.</param>
        /// <param name="errors">The errors.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long GetDataspacesResponse(IMessageHeader request, IList<Datatypes.Object.Dataspace> dataspaces)
        {
            var header = CreateMessageHeader(Protocols.Dataspace, MessageTypes.Dataspace.GetDataspacesResponse, request.MessageId);

            var response = new GetDataspacesResponse
            {
            };

            return SendMultipartResponse(header, response, dataspaces, (m, i) => m.Dataspaces = i);
        }

        /// <summary>
        /// Handles the PutDataspaces event from a customer.
        /// </summary>
        public event ProtocolEventWithErrorsHandler<PutDataspaces, ErrorInfo> OnPutDataspaces;

        /// <summary>
        /// Handles the DeleteDataspaces event from a customer.
        /// </summary>
        public event ProtocolEventWithErrorsHandler<DeleteDataspaces, ErrorInfo> OnDeleteDataspaces;

        /// <summary>
        /// Handles the GetDataspaces message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GetDataspaces message.</param>
        protected virtual void HandleGetDataspaces(IMessageHeader header, GetDataspaces message)
        {
            var args = Notify(OnGetDataspaces, header, message, new List<Datatypes.Object.Dataspace>());
            if (args.Cancel)
                return;

            if (!HandleGetDataspaces(header, message, args.Context))
                return;

            GetDataspacesResponse(header, args.Context);
        }

        /// <summary>
        /// Handles the GetDataspaces message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="response">The response.</param>
        protected virtual bool HandleGetDataspaces(IMessageHeader header, GetDataspaces message, IList<Datatypes.Object.Dataspace> response)
        {
            return true;
        }

        /// <summary>
        /// Handles the PutDataspaces message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The PutDataspaces message.</param>
        protected virtual void HandlePutDataspaces(IMessageHeader header, PutDataspaces message)
        {
            var args = Notify(OnPutDataspaces, header, message, new Dictionary<string, ErrorInfo>());
            if (args.Cancel)
                return;

            if (!HandlePutDataspaces(header, message, args.Errors))
                return;

            SendMultipartResponse(header, message, args.Errors);
        }

        /// <summary>
        /// Handles the PutDataspaces message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="errors">The errors.</param>
        protected virtual bool HandlePutDataspaces(IMessageHeader header, PutDataspaces message, IDictionary<string, ErrorInfo> errors)
        {
            return true;
        }

        /// <summary>
        /// Handles the DeleteDataspaces message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The DeleteDataspaces message.</param>
        protected virtual void HandleDeleteDataspaces(IMessageHeader header, DeleteDataspaces message)
        {
            var args = Notify(OnDeleteDataspaces, header, message, new Dictionary<string, ErrorInfo>());
            if (args.Cancel)
                return;

            if (!HandleDeleteDataspaces(header, message, args.Errors))
                return;

            SendMultipartResponse(header, message, args.Errors);
        }

        /// <summary>
        /// Handles the DeleteDataspaces message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="errors">The errors.</param>
        protected virtual bool HandleDeleteDataspaces(IMessageHeader header, DeleteDataspaces message, IDictionary<string, ErrorInfo> errors)
        {
            return true;
        }
    }
}

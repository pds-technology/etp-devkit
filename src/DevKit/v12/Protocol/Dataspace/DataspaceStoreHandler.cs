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
using Energistics.Etp.v12.Datatypes;

namespace Energistics.Etp.v12.Protocol.Dataspace
{
    /// <summary>
    /// Base implementation of the <see cref="IDataspaceStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.Dataspace.IDataspaceStore" />
    public class DataspaceStoreHandler : Etp12ProtocolHandler<CapabilitiesStore, ICapabilitiesStore, CapabilitiesCustomer, ICapabilitiesCustomer>, IDataspaceStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataspaceStoreHandler"/> class.
        /// </summary>
        public DataspaceStoreHandler() : base((int)Protocols.Dataspace, Roles.Store, Roles.Customer)
        {
            RegisterMessageHandler<GetDataspaces>(Protocols.Dataspace, MessageTypes.Dataspace.GetDataspaces, HandleGetDataspaces);
            RegisterMessageHandler<PutDataspaces>(Protocols.Dataspace, MessageTypes.Dataspace.PutDataspaces, HandlePutDataspaces);
            RegisterMessageHandler<DeleteDataspaces>(Protocols.Dataspace, MessageTypes.Dataspace.DeleteDataspaces, HandleDeleteDataspaces);
        }

        /// <summary>
        /// Handles the GetDataspaces event from a customer.
        /// </summary>
        public event EventHandler<ListRequestEventArgs<GetDataspaces, Datatypes.Object.Dataspace>> OnGetDataspaces;

        /// <summary>
        /// Sends an GetDataspacesResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="dataspaces">The dataspaces.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetDataspacesResponse> GetDataspacesResponse(IMessageHeader correlatedHeader, IList<Datatypes.Object.Dataspace> dataspaces, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new GetDataspacesResponse
            {
                Dataspaces = dataspaces ?? new List<Datatypes.Object.Dataspace>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Handles the PutDataspaces event from a customer.
        /// </summary>
        public event EventHandler<MapRequestEventArgs<PutDataspaces, string>> OnPutDataspaces;

        /// <summary>
        /// Sends a PutDataspacesResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutDataspacesResponse> PutDataspacesResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new PutDataspacesResponse
            {
                Success = success ?? new Dictionary<string, string>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a complete multi-part set of PutDataspacesResponse and ProtocolException messages to a customer.
        /// If there are no successes, an empty PutDataspacesResponse message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the PutDataspacesResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutDataspacesResponse> PutDataspacesResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
        {
            return SendMapResponse(PutDataspacesResponse, correlatedHeader, success, errors, setFinalPart: setFinalPart, responseExtension: responseExtension, exceptionExtension: exceptionExtension);
        }

        /// <summary>
        /// Handles the DeleteDataspaces event from a customer.
        /// </summary>
        public event EventHandler<MapRequestEventArgs<DeleteDataspaces, string>> OnDeleteDataspaces;

        /// <summary>
        /// Sends a DeleteDataspacesResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<DeleteDataspacesResponse> DeleteDataspacesResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new DeleteDataspacesResponse
            {
                Success = success ?? new Dictionary<string, string>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a complete multi-part set of DeleteDataspacesResponse and ProtocolException messages to a customer.
        /// If there are no successes, an empty DeleteDataspacesResponse message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the DeleteDataspacesResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<DeleteDataspacesResponse> DeleteDataspacesResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
        {
            return SendMapResponse(DeleteDataspacesResponse, correlatedHeader, success, errors, setFinalPart: setFinalPart, responseExtension: responseExtension, exceptionExtension: exceptionExtension);
        }

        /// <summary>
        /// Handles the GetDataspaces message from a customer.
        /// </summary>
        /// <param name="message">The GetDataspaces message.</param>
        protected virtual void HandleGetDataspaces(EtpMessage<GetDataspaces> message)
        {
            HandleRequestMessage(message, OnGetDataspaces, HandleGetDataspaces,
                responseMethod: (args) => GetDataspacesResponse(args.Request?.Header, args.Responses, isFinalPart: !args.HasErrors, extension: args.ResponseExtension));
        }

        /// <summary>
        /// Handles the GetDataspaces message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="ListRequestEventArgs{GetDataspaces, Datatypes.Object.Dataspace}"/> instance containing the event data.</param>
        protected virtual void HandleGetDataspaces(ListRequestEventArgs<GetDataspaces, Datatypes.Object.Dataspace> args)
        {
        }

        /// <summary>
        /// Handles the PutDataspaces message from a customer.
        /// </summary>
        /// <param name="message">The PutDataspaces message.</param>
        protected virtual void HandlePutDataspaces(EtpMessage<PutDataspaces> message)
        {
            HandleRequestMessage(message, OnPutDataspaces, HandlePutDataspaces,
                responseMethod: (args) => PutDataspacesResponse(args.Request?.Header, args.ResponseMap, isFinalPart: !args.HasErrors, extension: args.ResponseMapExtension));
        }

        /// <summary>
        /// Handles the response to an PutDataspaces message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="MapRequestEventArgs{PutDataspaces, string, ErrorInfo}"/> instance containing the event data.</param>
        protected virtual void HandlePutDataspaces(MapRequestEventArgs<PutDataspaces, string> args)
        {
        }

        /// <summary>
        /// Handles the DeleteDataspaces message from a customer.
        /// </summary>
        /// <param name="message">The DeleteDataspaces message.</param>
        protected virtual void HandleDeleteDataspaces(EtpMessage<DeleteDataspaces> message)
        {
            HandleRequestMessage(message, OnDeleteDataspaces, HandleDeleteDataspaces,
                responseMethod: (args) => DeleteDataspacesResponse(args.Request?.Header, args.ResponseMap, isFinalPart: !args.HasErrors, extension: args.ResponseMapExtension));
        }

        /// <summary>
        /// Handles the response to an DeleteDataspaces message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="MapRequestEventArgs{DeleteDataspaces, string, ErrorInfo}"/> instance containing the event data.</param>
        protected virtual void HandleDeleteDataspaces(MapRequestEventArgs<DeleteDataspaces, string> args)
        {
        }
    }
}

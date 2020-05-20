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
using Energistics.Etp.v12.Datatypes.DataArrayTypes;

namespace Energistics.Etp.v12.Protocol.DataArray
{
    /// <summary>
    /// Base implementation of the <see cref="IDataArrayStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.DataArray.IDataArrayStore" />
    public class DataArrayStoreHandler : Etp12ProtocolHandler, IDataArrayStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataArrayStoreHandler"/> class.
        /// </summary>
        public DataArrayStoreHandler() : base((int)Protocols.DataArray, "store", "customer")
        {
            RegisterMessageHandler<GetDataArrays>(Protocols.DataArray, MessageTypes.DataArray.GetDataArrays, HandleGetDataArrays);
            RegisterMessageHandler<GetDataSubarrays>(Protocols.DataArray, MessageTypes.DataArray.GetDataSubarrays, HandleGetDataSubarrays);
            RegisterMessageHandler<PutDataArrays>(Protocols.DataArray, MessageTypes.DataArray.PutDataArrays, HandlePutDataArrays);
            RegisterMessageHandler<PutUninitializedDataArray>(Protocols.DataArray, MessageTypes.DataArray.PutUninitializedDataArray, HandlePutUninitializedDataArray);
            RegisterMessageHandler<PutDataSubarrays>(Protocols.DataArray, MessageTypes.DataArray.PutDataSubarrays, HandlePutDataSubarrays);
            RegisterMessageHandler<GetDataArrayMetadata>(Protocols.DataArray, MessageTypes.DataArray.GetDataArrayMetadata, HandleGetDataArrayMetadata);
        }

        /// <summary>
        /// Handles the GetDataArrays event from a customer.
        /// </summary>
        public event ProtocolEventWithErrorsHandler<GetDataArrays, Datatypes.DataArrayTypes.DataArray, ErrorInfo> OnGetDataArrays;

        /// <summary>
        /// Sends an GetDataArraysResponse message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="dataArrays">The data arrays.</param>
        /// <param name="errors">The errors.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long GetDataArraysResponse(IMessageHeader request, IDictionary<string, Datatypes.DataArrayTypes.DataArray> dataArrays, IDictionary<string, ErrorInfo> errors)
        {
            var header = CreateMessageHeader(Protocols.DataArray, MessageTypes.DataArray.GetDataArraysResponse, request.MessageId);

            var response = new GetDataArraysResponse
            {
            };

            return SendMultipartResponse(header, response, dataArrays, errors, (m, i) => m.DataArrays = i);
        }

        /// <summary>
        /// Handles the GetDataSubarrays event from a customer.
        /// </summary>
        public event ProtocolEventWithErrorsHandler<GetDataSubarrays, Datatypes.DataArrayTypes.DataArray, ErrorInfo> OnGetDataSubarrays;

        /// <summary>
        /// Sends an GetDataSubarraysResponse message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="dataSubarrays">The data subarrays.</param>
        /// <param name="errors">The errors.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long GetDataSubarraysResponse(IMessageHeader request, IDictionary<string, Datatypes.DataArrayTypes.DataArray> dataSubarrays, IDictionary<string, ErrorInfo> errors)
        {
            var header = CreateMessageHeader(Protocols.DataArray, MessageTypes.DataArray.GetDataSubarraysResponse, request.MessageId);

            var response = new GetDataSubarraysResponse
            {
            };

            return SendMultipartResponse(header, response, dataSubarrays, errors, (m, i) => m.DataSubarrays = i);
        }

        /// <summary>
        /// Handles the PutDataArrays event from a customer.
        /// </summary>
        public event ProtocolEventWithErrorsHandler<PutDataArrays, ErrorInfo> OnPutDataArrays;

        /// <summary>
        /// Handles the PutUninitializedDataArray event from a customer.
        /// </summary>
        public event ProtocolEventWithErrorsHandler<PutUninitializedDataArray, ErrorInfo> OnPutUninitializedDataArray;

        /// <summary>
        /// Handles the PutDataSubarrays event from a customer.
        /// </summary>
        public event ProtocolEventWithErrorsHandler<PutDataSubarrays, ErrorInfo> OnPutDataSubarrays;

        /// <summary>
        /// Handles the GetDataArrayMetadata event from a customer.
        /// </summary>
        public event ProtocolEventWithErrorsHandler<GetDataArrayMetadata, DataArrayMetadata, ErrorInfo> OnGetDataArrayMetadata;

        /// <summary>
        /// Sends an GetDataArrayMetadataResponse message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="arrayMetadata">The array metadata.</param>
        /// <param name="errors">The errors.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long GetDataArrayMetadataResponse(IMessageHeader request, IDictionary<string, DataArrayMetadata> arrayMetadata, IDictionary<string, ErrorInfo> errors)
        {
            var header = CreateMessageHeader(Protocols.DataArray, MessageTypes.DataArray.GetDataArrayMetadataResponse, request.MessageId);

            var response = new GetDataArrayMetadataResponse
            {
            };

            return SendMultipartResponse(header, response, arrayMetadata, errors, (m, i) => m.ArrayMetadata = i);
        }

        /// <summary>
        /// Handles the GetDataArrays message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GetDataArrays message.</param>
        protected virtual void HandleGetDataArrays(IMessageHeader header, GetDataArrays message)
        {
            var args = Notify(OnGetDataArrays, header, message, new Dictionary<string, Datatypes.DataArrayTypes.DataArray>(), new Dictionary<string, ErrorInfo>());
            if (args.Cancel)
                return;

            if (!HandleGetDataArrays(header, message, args.Context, args.Errors))
                return;

            GetDataArraysResponse(header, args.Context, args.Errors);
        }

        /// <summary>
        /// Handles the GetDataArrays message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="response">The response.</param>
        /// <param name="errors">The errors.</param>
        protected virtual bool HandleGetDataArrays(IMessageHeader header, GetDataArrays message, IDictionary<string, Datatypes.DataArrayTypes.DataArray> response, IDictionary<string, ErrorInfo> errors)
        {
            return true;
        }

        /// <summary>
        /// Handles the GetDataSubarrays message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GetDataSubarrays message.</param>
        protected virtual void HandleGetDataSubarrays(IMessageHeader header, GetDataSubarrays message)
        {
            var args = Notify(OnGetDataSubarrays, header, message, new Dictionary<string, Datatypes.DataArrayTypes.DataArray>(), new Dictionary<string, ErrorInfo>());
            if (args.Cancel)
                return;

            if (!HandleGetDataSubarrays(header, message, args.Context, args.Errors))
                return;

            GetDataSubarraysResponse(header, args.Context, args.Errors);
        }

        /// <summary>
        /// Handles the GetDataSubarrays message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="response">The response.</param>
        /// <param name="errors">The errors.</param>
        protected virtual bool HandleGetDataSubarrays(IMessageHeader header, GetDataSubarrays message, IDictionary<string, Datatypes.DataArrayTypes.DataArray> response, IDictionary<string, ErrorInfo> errors)
        {
            return true;
        }

        /// <summary>
        /// Handles the PutDataArrays message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The PutDataArrays message.</param>
        protected virtual void HandlePutDataArrays(IMessageHeader header, PutDataArrays message)
        {
            var args = Notify(OnPutDataArrays, header, message, new Dictionary<string, ErrorInfo>());
            if (args.Cancel)
                return;

            if (!HandlePutDataArrays(header, message, args.Errors))
                return;

            SendMultipartResponse(header, message, args.Errors);
        }

        /// <summary>
        /// Handles the PutDataArrays message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="errors">The errors.</param>
        protected virtual bool HandlePutDataArrays(IMessageHeader header, PutDataArrays message, IDictionary<string, ErrorInfo> errors)
        {
            return true;
        }

        /// <summary>
        /// Handles the PutUninitializedDataArray message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The PutUninitializedDataArray message.</param>
        protected virtual void HandlePutUninitializedDataArray(IMessageHeader header, PutUninitializedDataArray message)
        {
            var args = Notify(OnPutUninitializedDataArray, header, message, new Dictionary<string, ErrorInfo>());
            if (args.Cancel)
                return;

            if (!HandlePutUninitializedDataArray(header, message, args.Errors))
                return;

            SendMultipartResponse(header, message, args.Errors);
        }

        /// <summary>
        /// Handles the PutUninitializedDataArray message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="errors">The errors.</param>
        protected virtual bool HandlePutUninitializedDataArray(IMessageHeader header, PutUninitializedDataArray message, IDictionary<string, ErrorInfo> errors)
        {
            return true;
        }

        /// <summary>
        /// Handles the PutDataSubarrays message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The PutDataSubarrays message.</param>
        protected virtual void HandlePutDataSubarrays(IMessageHeader header, PutDataSubarrays message)
        {
            var args = Notify(OnPutDataSubarrays, header, message, new Dictionary<string, ErrorInfo>());
            if (args.Cancel)
                return;

            if (!HandlePutDataSubarrays(header, message, args.Errors))
                return;

            SendMultipartResponse(header, message, args.Errors);
        }

        /// <summary>
        /// Handles the PutDataSubarrays message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="errors">The errors.</param>
        protected virtual bool HandlePutDataSubarrays(IMessageHeader header, PutDataSubarrays message, IDictionary<string, ErrorInfo> errors)
        {
            return true;
        }

        /// <summary>
        /// Handles the GetDataArrayMetadata message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GetDataArrayMetadata message.</param>
        protected virtual void HandleGetDataArrayMetadata(IMessageHeader header, GetDataArrayMetadata message)
        {
            var args = Notify(OnGetDataArrayMetadata, header, message, new Dictionary<string, DataArrayMetadata>(), new Dictionary<string, ErrorInfo>());
            if (args.Cancel)
                return;

            if (!HandleGetDataArrayMetadata(header, message, args.Context, args.Errors))
                return;

            GetDataArrayMetadataResponse(header, args.Context, args.Errors);
        }

        /// <summary>
        /// Handles the GetDataArrayMetadata message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="response">The response.</param>
        /// <param name="errors">The errors.</param>
        protected virtual bool HandleGetDataArrayMetadata(IMessageHeader header, GetDataArrayMetadata message, IDictionary<string, DataArrayMetadata> response, IDictionary<string, ErrorInfo> errors)
        {
            return true;
        }
    }
}

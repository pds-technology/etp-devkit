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
using Energistics.Etp.v12.Datatypes.DataArrayTypes;

namespace Energistics.Etp.v12.Protocol.DataArray
{
    /// <summary>
    /// Base implementation of the <see cref="IDataArrayStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.DataArray.IDataArrayStore" />
    public class DataArrayStoreHandler : Etp12ProtocolHandlerWithCapabilities<CapabilitiesStore, ICapabilitiesStore>, IDataArrayStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataArrayStoreHandler"/> class.
        /// </summary>
        public DataArrayStoreHandler() : base((int)Protocols.DataArray, Roles.Store, Roles.Customer)
        {
            RegisterMessageHandler<GetDataArrayMetadata>(Protocols.DataArray, MessageTypes.DataArray.GetDataArrayMetadata, HandleGetDataArrayMetadata);
            RegisterMessageHandler<GetDataArrays>(Protocols.DataArray, MessageTypes.DataArray.GetDataArrays, HandleGetDataArrays);
            RegisterMessageHandler<GetDataSubarrays>(Protocols.DataArray, MessageTypes.DataArray.GetDataSubarrays, HandleGetDataSubarrays);
            RegisterMessageHandler<PutUninitializedDataArrays>(Protocols.DataArray, MessageTypes.DataArray.PutUninitializedDataArrays, HandlePutUninitializedDataArrays);
            RegisterMessageHandler<PutDataArrays>(Protocols.DataArray, MessageTypes.DataArray.PutDataArrays, HandlePutDataArrays);
            RegisterMessageHandler<PutDataSubarrays>(Protocols.DataArray, MessageTypes.DataArray.PutDataSubarrays, HandlePutDataSubarrays);
        }

        /// <summary>
        /// Handles the GetDataArrayMetadata event from a customer.
        /// </summary>
        public event EventHandler<MapRequestEventArgs<GetDataArrayMetadata, DataArrayMetadata>> OnGetDataArrayMetadata;

        /// <summary>
        /// Sends a GetDataArrayMetadataResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="arrayMetadata">The array metadata.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetDataArrayMetadataResponse> GetDataArrayMetadataResponse(IMessageHeader correlatedHeader, IDictionary<string, DataArrayMetadata> arrayMetadata, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new GetDataArrayMetadataResponse
            {
                ArrayMetadata = arrayMetadata ?? new Dictionary<string, DataArrayMetadata>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a complete multi-part set of GetDataArrayMetadataResponse and ProtocolException messages to a customer.
        /// If there are no array metadata, an empty DataArrayMetadataRecord message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="arrayMetadata">The array metadata.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the GetDataArrayMetadataResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetDataArrayMetadataResponse> GetDataArrayMetadataResponse(IMessageHeader correlatedHeader, IDictionary<string, DataArrayMetadata> arrayMetadata, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
        {
            return SendMapResponse(GetDataArrayMetadataResponse, correlatedHeader, arrayMetadata, errors, setFinalPart: setFinalPart, responseExtension: responseExtension, exceptionExtension: exceptionExtension);
        }

        /// <summary>
        /// Handles the GetDataArrays event from a customer.
        /// </summary>
        public event EventHandler<MapRequestEventArgs<GetDataArrays, Datatypes.DataArrayTypes.DataArray>> OnGetDataArrays;

        /// <summary>
        /// Sends a GetDataArraysResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="dataArrays">The data arrays.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetDataArraysResponse> GetDataArraysResponse(IMessageHeader correlatedHeader, IDictionary<string, Datatypes.DataArrayTypes.DataArray> dataArrays, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new GetDataArraysResponse
            {
                DataArrays = dataArrays ?? new Dictionary<string, Datatypes.DataArrayTypes.DataArray>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a complete multi-part set of GetDataArraysResponse and ProtocolException messages to a customer.
        /// If there are no data arrays, an empty DataArraysRecord message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="dataArrays">The data arrays.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the GetDataArraysResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetDataArraysResponse> GetDataArraysResponse(IMessageHeader correlatedHeader, IDictionary<string, Datatypes.DataArrayTypes.DataArray> dataArrays, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
        {
            return SendMapResponse(GetDataArraysResponse, correlatedHeader, dataArrays, errors, setFinalPart: setFinalPart, responseExtension: responseExtension, exceptionExtension: exceptionExtension);
        }

        /// <summary>
        /// Handles the GetDataSubarrays event from a customer.
        /// </summary>
        public event EventHandler<MapRequestEventArgs<GetDataSubarrays, Datatypes.DataArrayTypes.DataArray>> OnGetDataSubarrays;

        /// <summary>
        /// Sends a GetDataSubarraysResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="dataSubarrays">The data subarrays.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetDataSubarraysResponse> GetDataSubarraysResponse(IMessageHeader correlatedHeader, IDictionary<string, Datatypes.DataArrayTypes.DataArray> dataSubarrays, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new GetDataSubarraysResponse
            {
                DataSubarrays = dataSubarrays ?? new Dictionary<string, Datatypes.DataArrayTypes.DataArray>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a complete multi-part set of GetDataSubarraysResponse and ProtocolException messages to a customer.
        /// If there are no data subarrays, an empty DataSubarraysRecord message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="dataSubarrays">The data subarrays.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the GetDataSubarraysResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetDataSubarraysResponse> GetDataSubarraysResponse(IMessageHeader correlatedHeader, IDictionary<string, Datatypes.DataArrayTypes.DataArray> dataSubarrays, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
        {
            return SendMapResponse(GetDataSubarraysResponse, correlatedHeader, dataSubarrays, errors, setFinalPart: setFinalPart, responseExtension: responseExtension, exceptionExtension: exceptionExtension);
        }

        /// <summary>
        /// Handles the PutUninitializedDataArrays event from a customer.
        /// </summary>
        public event EventHandler<MapRequestEventArgs<PutUninitializedDataArrays, string>> OnPutUninitializedDataArrays;

        /// <summary>
        /// Sends a PutUninitializedDataArraysResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutUninitializedDataArraysResponse> PutUninitializedDataArraysResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new PutUninitializedDataArraysResponse
            {
                Success = success ?? new Dictionary<string, string>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a complete multi-part set of PutUninitializedDataArraysResponse and ProtocolException messages to a customer.
        /// If there are no successes, an empty PutUninitializedDataArraysResponse message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the PutUninitializedDataArraysResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutUninitializedDataArraysResponse> PutUninitializedDataArraysResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
        {
            return SendMapResponse(PutUninitializedDataArraysResponse, correlatedHeader, success, errors, setFinalPart: setFinalPart, responseExtension: responseExtension, exceptionExtension: exceptionExtension);
        }

        /// <summary>
        /// Handles the PutDataArrays event from a customer.
        /// </summary>
        public event EventHandler<MapRequestEventArgs<PutDataArrays, string>> OnPutDataArrays;

        /// <summary>
        /// Sends a PutDataArraysResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutDataArraysResponse> PutDataArraysResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new PutDataArraysResponse
            {
                Success = success ?? new Dictionary<string, string>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a complete multi-part set of PutDataArraysResponse and ProtocolException messages to a customer.
        /// If there are no successes, an empty PutDataArraysResponse message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the PutDataArraysResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutDataArraysResponse> PutDataArraysResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
        {
            return SendMapResponse(PutDataArraysResponse, correlatedHeader, success, errors, setFinalPart: setFinalPart, responseExtension: responseExtension, exceptionExtension: exceptionExtension);
        }

        /// <summary>
        /// Handles the PutDataSubarrays event from a customer.
        /// </summary>
        public event EventHandler<MapRequestEventArgs<PutDataSubarrays, string>> OnPutDataSubarrays;

        /// <summary>
        /// Sends a PutDataSubarraysResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutDataSubarraysResponse> PutDataSubarraysResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new PutDataSubarraysResponse
            {
                Success = success ?? new Dictionary<string, string>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a complete multi-part set of PutDataSubarraysResponse and ProtocolException messages to a customer.
        /// If there are no successes, an empty PutDataSubarraysResponse message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the PutDataSubarraysResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutDataSubarraysResponse> PutDataSubarraysResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
        {
            return SendMapResponse(PutDataSubarraysResponse, correlatedHeader, success, errors, setFinalPart: setFinalPart, responseExtension: responseExtension, exceptionExtension: exceptionExtension);
        }

        /// <summary>
        /// Handles the GetDataArrayMetadata message from a customer.
        /// </summary>
        /// <param name="message">The GetDataArrayMetadata message.</param>
        protected virtual void HandleGetDataArrayMetadata(EtpMessage<GetDataArrayMetadata> message)
        {
            HandleRequestMessage(message, OnGetDataArrayMetadata, HandleGetDataArrayMetadata,
                responseMethod: (args) => GetDataArrayMetadataResponse(args.Request?.Header, args.ResponseMap, isFinalPart: !args.HasErrors, extension: args.ResponseMapExtension));
        }

        /// <summary>
        /// Handles the response to an GetDataArrayMetadata message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="MapRequestEventArgs{GetDataArrayMetadata, DataArrayMetadata}"/> instance containing the event data.</param>
        protected virtual void HandleGetDataArrayMetadata(MapRequestEventArgs<GetDataArrayMetadata, DataArrayMetadata> args)
        {
        }

        /// <summary>
        /// Handles the GetDataArrays message from a customer.
        /// </summary>
        /// <param name="message">The GetDataArrays message.</param>
        protected virtual void HandleGetDataArrays(EtpMessage<GetDataArrays> message)
        {
            HandleRequestMessage(message, OnGetDataArrays, HandleGetDataArrays,
                responseMethod: (args) => GetDataArraysResponse(args.Request?.Header, args.ResponseMap, isFinalPart: !args.HasErrors, extension: args.ResponseMapExtension));
        }

        /// <summary>
        /// Handles the response to an GetDataArrays message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="MapRequestEventArgs{GetDataArrays, Datatypes.DataArrayTypes.DataArray}"/> instance containing the event data.</param>
        protected virtual void HandleGetDataArrays(MapRequestEventArgs<GetDataArrays, Datatypes.DataArrayTypes.DataArray> args)
        {
        }

        /// <summary>
        /// Handles the GetDataSubarrays message from a customer.
        /// </summary>
        /// <param name="message">The GetDataSubarrays message.</param>
        protected virtual void HandleGetDataSubarrays(EtpMessage<GetDataSubarrays> message)
        {
            HandleRequestMessage(message, OnGetDataSubarrays, HandleGetDataSubarrays,
                responseMethod: (args) => GetDataSubarraysResponse(args.Request?.Header, args.ResponseMap, isFinalPart: !args.HasErrors, extension: args.ResponseMapExtension));
        }

        /// <summary>
        /// Handles the response to an GetDataSubarrays message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="MapRequestEventArgs{GetDataSubarrays, Datatypes.DataArrayTypes.DataArray}"/> instance containing the event data.</param>
        protected virtual void HandleGetDataSubarrays(MapRequestEventArgs<GetDataSubarrays, Datatypes.DataArrayTypes.DataArray> args)
        {
        }

        /// <summary>
        /// Handles the PutUninitializedDataArrays message from a customer.
        /// </summary>
        /// <param name="message">The PutUninitializedDataArrays message.</param>
        protected virtual void HandlePutUninitializedDataArrays(EtpMessage<PutUninitializedDataArrays> message)
        {
            HandleRequestMessage(message, OnPutUninitializedDataArrays, HandlePutUninitializedDataArrays,
                responseMethod: (args) => PutUninitializedDataArraysResponse(args.Request?.Header, args.ResponseMap, isFinalPart: !args.HasErrors, extension: args.ResponseMapExtension));
        }

        /// <summary>
        /// Handles the response to an PutUninitializedDataArrays message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="MapRequestEventArgs{PutUninitializedDataArrays, string}"/> instance containing the event data.</param>
        protected virtual void HandlePutUninitializedDataArrays(MapRequestEventArgs<PutUninitializedDataArrays, string> args)
        {
        }

        /// <summary>
        /// Handles the PutDataArrays message from a customer.
        /// </summary>
        /// <param name="message">The PutDataArrays message.</param>
        protected virtual void HandlePutDataArrays(EtpMessage<PutDataArrays> message)
        {
            HandleRequestMessage(message, OnPutDataArrays, HandlePutDataArrays,
                responseMethod: (args) => PutDataArraysResponse(args.Request?.Header, args.ResponseMap, isFinalPart: !args.HasErrors, extension: args.ResponseMapExtension));
        }

        /// <summary>
        /// Handles the response to an PutDataArrays message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="MapRequestEventArgs{PutDataArrays, string}"/> instance containing the event data.</param>
        protected virtual void HandlePutDataArrays(MapRequestEventArgs<PutDataArrays, string> args)
        {
        }

        /// <summary>
        /// Handles the PutDataSubarrays message from a customer.
        /// </summary>
        /// <param name="message">The PutDataSubarrays message.</param>
        protected virtual void HandlePutDataSubarrays(EtpMessage<PutDataSubarrays> message)
        {
            HandleRequestMessage(message, OnPutDataSubarrays, HandlePutDataSubarrays,
                responseMethod: (args) => PutDataSubarraysResponse(args.Request?.Header, args.ResponseMap, isFinalPart: !args.HasErrors, extension: args.ResponseMapExtension));
        }

        /// <summary>
        /// Handles the response to an PutDataSubarrays message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="MapRequestEventArgs{PutDataSubarrays, string}"/> instance containing the event data.</param>
        protected virtual void HandlePutDataSubarrays(MapRequestEventArgs<PutDataSubarrays, string> args)
        {
        }
    }
}

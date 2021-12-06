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
using System.Linq;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v12.Datatypes;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.GrowingObject
{
    /// <summary>
    /// Base implementation of the <see cref="IGrowingObjectStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.GrowingObject.IGrowingObjectStore" />
    public class GrowingObjectStoreHandler : Etp12ProtocolHandler<CapabilitiesStore, ICapabilitiesStore, CapabilitiesCustomer, ICapabilitiesCustomer>, IGrowingObjectStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrowingObjectStoreHandler"/> class.
        /// </summary>
        public GrowingObjectStoreHandler() : base((int)Protocols.GrowingObject, Roles.Store, Roles.Customer)
        {
            RegisterMessageHandler<GetGrowingDataObjectsHeader>(Protocols.GrowingObject, MessageTypes.GrowingObject.GetGrowingDataObjectsHeader, HandleGetGrowingDataObjectsHeader);
            RegisterMessageHandler<PutGrowingDataObjectsHeader>(Protocols.GrowingObject, MessageTypes.GrowingObject.PutGrowingDataObjectsHeader, HandlePutGrowingDataObjectsHeader);
            RegisterMessageHandler<GetPartsMetadata>(Protocols.GrowingObject, MessageTypes.GrowingObject.GetPartsMetadata, HandleGetPartsMetadata);
            RegisterMessageHandler<GetChangeAnnotations>(Protocols.GrowingObject, MessageTypes.GrowingObject.GetChangeAnnotations, HandleGetChangeAnnotations);
            RegisterMessageHandler<GetParts>(Protocols.GrowingObject, MessageTypes.GrowingObject.GetParts, HandleGetParts);
            RegisterMessageHandler<GetPartsByRange>(Protocols.GrowingObject, MessageTypes.GrowingObject.GetPartsByRange, HandleGetPartsByRange);
            RegisterMessageHandler<PutParts>(Protocols.GrowingObject, MessageTypes.GrowingObject.PutParts, HandlePutParts);
            RegisterMessageHandler<DeleteParts>(Protocols.GrowingObject, MessageTypes.GrowingObject.DeleteParts, HandleDeleteParts);
            RegisterMessageHandler<ReplacePartsByRange>(Protocols.GrowingObject, MessageTypes.GrowingObject.ReplacePartsByRange, HandleReplacePartsByRange);
        }

        /// <summary>
        /// Handles the GetGrowingDataObjectsHeader event from a customer.
        /// </summary>
        public event EventHandler<MapRequestEventArgs<GetGrowingDataObjectsHeader, DataObject>> OnGetGrowingDataObjectsHeader;

        /// <summary>
        /// Sends a GetGrowingDataObjectsHeaderResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="dataObjects">The data objects.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetGrowingDataObjectsHeaderResponse> GetGrowingDataObjectsHeaderResponse(IMessageHeader correlatedHeader, IDictionary<string, DataObject> dataObjects, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new GetGrowingDataObjectsHeaderResponse
            {
                DataObjects = dataObjects ?? new Dictionary<string, DataObject>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a complete multi-part set of GetGrowingDataObjectsHeaderResponse and ProtocolException messages to a customer.
        /// If there are no data objects, an empty GetGrowingDataObjectsHeaderResponse message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="dataObjects">The data objects.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the GetGrowingDataObjectsHeaderResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetGrowingDataObjectsHeaderResponse> GetGrowingDataObjectsHeaderResponse(IMessageHeader correlatedHeader, IDictionary<string, DataObject> dataObjects, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
        {
            return SendMapResponse(GetGrowingDataObjectsHeaderResponse, correlatedHeader, dataObjects, errors, setFinalPart: setFinalPart, responseExtension: responseExtension, exceptionExtension: exceptionExtension);
        }

        /// <summary>
        /// Handles the PutGrowingDataObjectsHeader event from a customer.
        /// </summary>
        public event EventHandler<MapRequestEventArgs<PutGrowingDataObjectsHeader, string>> OnPutGrowingDataObjectsHeader;

        /// <summary>
        /// Sends a PutGrowingDataObjectsHeaderResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutGrowingDataObjectsHeaderResponse> PutGrowingDataObjectsHeaderResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new PutGrowingDataObjectsHeaderResponse
            {
                Success = success ?? new Dictionary<string, string>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a complete multi-part set of PutGrowingDataObjectsHeaderResponse and ProtocolException messages to a customer.
        /// If there are no URIs, an empty PutGrowingDataObjectsHeaderResponse message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the PutGrowingDataObjectsHeaderResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutGrowingDataObjectsHeaderResponse> PutGrowingDataObjectsHeaderResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
        {
            return SendMapResponse(PutGrowingDataObjectsHeaderResponse, correlatedHeader, success, errors, setFinalPart: setFinalPart, responseExtension: responseExtension, exceptionExtension: exceptionExtension);
        }

        /// <summary>
        /// Handles the GetPartsMetadata event from a customer.
        /// </summary>
        public event EventHandler<MapRequestEventArgs<GetPartsMetadata, PartsMetadataInfo>> OnGetPartsMetadata;

        /// <summary>
        /// Sends a GetPartsMetadataResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="metadata">The parts metadata.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetPartsMetadataResponse> GetPartsMetadataResponse(IMessageHeader correlatedHeader, IDictionary<string, PartsMetadataInfo> metadata, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new GetPartsMetadataResponse
            {
                Metadata = metadata ?? new Dictionary<string, PartsMetadataInfo>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a complete multi-part set of GetPartsMetadataResponse and ProtocolException messages to a customer.
        /// If there are no parts metadata, an empty GetPartsMetadataResponse message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="metadata">The metadata.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the GetPartsMetadataResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetPartsMetadataResponse> GetPartsMetadataResponse(IMessageHeader correlatedHeader, IDictionary<string, PartsMetadataInfo> metadata, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
        {
            return SendMapResponse(GetPartsMetadataResponse, correlatedHeader, metadata, errors, setFinalPart: setFinalPart, responseExtension: responseExtension, exceptionExtension: exceptionExtension);
        }

        /// <summary>
        /// Handles the GetChangeAnnotations event from a customer.
        /// </summary>
        public event EventHandler<MapRequestEventArgs<GetChangeAnnotations, ChangeResponseInfo>> OnGetChangeAnnotations;

        /// <summary>
        /// Sends a GetChangeAnnotationsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="changes">The changes.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetChangeAnnotationsResponse> GetChangeAnnotationsResponse(IMessageHeader correlatedHeader, IDictionary<string, ChangeResponseInfo> changes, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new GetChangeAnnotationsResponse
            {
                Changes = changes ?? new Dictionary<string, ChangeResponseInfo>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a complete multi-part set of GetChangeAnnotationsResponse and ProtocolException messages to a customer.
        /// If there are no changes, an empty ChangeAnnotationsRecord message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="changes">The changes.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the GetChangeAnnotationsResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetChangeAnnotationsResponse> GetChangeAnnotationsResponse(IMessageHeader correlatedHeader, IDictionary<string, ChangeResponseInfo> changes, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
        {
            return SendMapResponse(GetChangeAnnotationsResponse, correlatedHeader, changes, errors, setFinalPart: setFinalPart, responseExtension: responseExtension, exceptionExtension: exceptionExtension);
        }

        /// <summary>
        /// Handles the GetParts event from a customer.
        /// </summary>
        public event EventHandler<MapRequestWithContextEventArgs<GetParts, ObjectPart, ResponseContext>> OnGetParts;

        /// <summary>
        /// Sends a GetPartsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="parts">The parts.</param>
        /// <param name="context">The response context.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetPartsResponse> GetPartsResponse(IMessageHeader correlatedHeader, IDictionary<string, ObjectPart> parts, ResponseContext context, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new GetPartsResponse
            {
                Uri = context?.Uri ?? string.Empty,
                Format = context?.Format ?? Formats.Xml,
                Parts = parts ?? new Dictionary<string, ObjectPart>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a GetPartsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="parts">The parts.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetPartsResponse> GetPartsResponse(IMessageHeader correlatedHeader, string uri, IDictionary<string, ObjectPart> parts, string format = Formats.Xml, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            return GetPartsResponse(correlatedHeader, parts, new ResponseContext { Uri = uri, Format = format }, isFinalPart: isFinalPart, extension: extension);
        }

        /// <summary>
        /// Sends a complete multi-part set of GetPartsResponse and ProtocolException messages to a customer.
        /// If there are no parts, an empty GetPartsResponse message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="parts">The parts.</param>
        /// <param name="context">The response context.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the GetPartsResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetPartsResponse> GetPartsResponse(IMessageHeader correlatedHeader, IDictionary<string, ObjectPart> parts, ResponseContext context, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
        {
            return SendMapResponse(GetPartsResponse, correlatedHeader, parts, context, errors, setFinalPart: setFinalPart, responseExtension: responseExtension, exceptionExtension: exceptionExtension);
        }

        /// <summary>
        /// Sends a complete multi-part set of GetPartsResponse and ProtocolException messages to a customer.
        /// If there are no parts, an empty GetPartsResponse message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="parts">The parts.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the GetPartsResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetPartsResponse> GetPartsResponse(IMessageHeader correlatedHeader, string uri, IDictionary<string, ObjectPart> parts, IDictionary<string, IErrorInfo> errors, string format = Formats.Xml, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
        {
            return GetPartsResponse(correlatedHeader, parts, new ResponseContext { Uri = uri, Format = format }, errors, setFinalPart: setFinalPart, responseExtension: responseExtension, exceptionExtension: exceptionExtension);
        }

        /// <summary>
        /// Handles the GetPartsByRange event from a customer.
        /// </summary>
        public event EventHandler<ListRequestWithContextEventArgs<GetPartsByRange, ObjectPart, ResponseContext>> OnGetPartsByRange;

        /// <summary>
        /// Sends a GetPartsByRangeResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="parts">The UIDs and data of the parts being returned.</param>
        /// <param name="context">The response context.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetPartsByRangeResponse> GetPartsByRangeResponse(IMessageHeader correlatedHeader, IList<ObjectPart> parts, ResponseContext context, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new GetPartsByRangeResponse
            {
                Uri = context?.Uri ?? string.Empty,
                Format = context?.Format ?? Formats.Xml,
                Parts = parts ?? new List<ObjectPart>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a GetPartsByRangeResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="parts">The UIDs and data of the parts being returned.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetPartsByRangeResponse> GetPartsByRangeResponse(IMessageHeader correlatedHeader, string uri, IList<ObjectPart> parts, string format = Formats.Xml, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            return GetPartsByRangeResponse(correlatedHeader, parts, new ResponseContext { Uri = uri, Format = format }, isFinalPart: isFinalPart, extension: extension);
        }

        /// <summary>
        /// Handles the PutParts event from a customer.
        /// </summary>
        public event EventHandler<MapRequestEventArgs<PutParts, string>> OnPutParts;

        /// <summary>
        /// Sends a PutPartsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutPartsResponse> PutPartsResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new PutPartsResponse
            {
                Success = success ?? new Dictionary<string, string>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a complete multi-part set of PutPartsResponse and ProtocolException messages to a customer.
        /// If there are no successes, an empty PutPartsResponse message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the PutPartsResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutPartsResponse> PutPartsResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
        {
            return SendMapResponse(PutPartsResponse, correlatedHeader, success, errors, setFinalPart: setFinalPart, responseExtension: responseExtension, exceptionExtension: exceptionExtension);
        }

        /// <summary>
        /// Handles the DeleteParts event from a customer.
        /// </summary>
        public event EventHandler<MapRequestEventArgs<DeleteParts, string>> OnDeleteParts;

        /// <summary>
        /// Sends a DeletePartsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<DeletePartsResponse> DeletePartsResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new DeletePartsResponse
            {
                Success = success ?? new Dictionary<string, string>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a complete multi-part set of DeletePartsResponse and ProtocolException messages to a customer.
        /// If there are no successes, an empty DeletePartsResponse message is sent.
        /// If there are no errors, no ProtocolException message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="success">The successes.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="responseExtension">The message header extension for the DeletePartsResponse message.</param>
        /// <param name="exceptionExtension">The message header extension for the ProtocolException message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<DeletePartsResponse> DeletePartsResponse(IMessageHeader correlatedHeader, IDictionary<string, string> success, IDictionary<string, IErrorInfo> errors, bool setFinalPart = true, IMessageHeaderExtension responseExtension = null, IMessageHeaderExtension exceptionExtension = null)
        {
            return SendMapResponse(DeletePartsResponse, correlatedHeader, success, errors, setFinalPart: setFinalPart, responseExtension: responseExtension, exceptionExtension: exceptionExtension);
        }

        /// <summary>
        /// Handles the ReplacePartsByRange event from a customer.
        /// </summary>
        public event EventHandler<EmptyRequestEventArgs<ReplacePartsByRange>> OnReplacePartsByRange;

        /// <summary>
        /// Sends a ReplacePartsByRangeResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ReplacePartsByRangeResponse> ReplacePartsByRangeResponse(IMessageHeader correlatedHeader, IMessageHeaderExtension extension = null)
        {
            var body = new ReplacePartsByRangeResponse
            {
            };

            return SendResponse(body, correlatedHeader, extension: extension);
        }

        /// <summary>
        /// Handles the GetGrowingDataObjectsHeader message from a customer.
        /// </summary>
        /// <param name="message">The GetGrowingDataObjectsHeader message.</param>
        protected virtual void HandleGetGrowingDataObjectsHeader(EtpMessage<GetGrowingDataObjectsHeader> message)
        {
            HandleRequestMessage(message, OnGetGrowingDataObjectsHeader, HandleGetGrowingDataObjectsHeader,
                responseMethod: (args) => GetGrowingDataObjectsHeaderResponse(args.Request?.Header, args.ResponseMap, isFinalPart: !args.HasErrors, extension: args.ResponseMapExtension));
        }

        /// <summary>
        /// Handles the GetGrowingDataObjectsHeader message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="MapRequestEventArgs{GetGrowingDataObjectsHeader, DataObject}"/> instance containing the event data.</param>
        protected virtual void HandleGetGrowingDataObjectsHeader(MapRequestEventArgs<GetGrowingDataObjectsHeader, DataObject> args)
        {
        }

        /// <summary>
        /// Handles the PutGrowingDataObjectsHeader message from a customer.
        /// </summary>
        /// <param name="message">The PutGrowingDataObjectsHeader message.</param>
        protected virtual void HandlePutGrowingDataObjectsHeader(EtpMessage<PutGrowingDataObjectsHeader> message)
        {
            HandleRequestMessage(message, OnPutGrowingDataObjectsHeader, HandlePutGrowingDataObjectsHeader,
                responseMethod: (args) => PutGrowingDataObjectsHeaderResponse(args.Request?.Header, args.ResponseMap, isFinalPart: !args.HasErrors, extension: args.ResponseMapExtension));
        }

        /// <summary>
        /// Handles the PutGrowingDataObjectsHeader message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="MapRequestEventArgs{PutGrowingDataObjectsHeader, string}"/> instance containing the event data.</param>
        protected virtual void HandlePutGrowingDataObjectsHeader(MapRequestEventArgs<PutGrowingDataObjectsHeader, string> args)
        {
        }

        /// <summary>
        /// Handles the GetPartsMetadata message from a customer.
        /// </summary>
        /// <param name="message">The GetPartsMetadata message.</param>
        protected virtual void HandleGetPartsMetadata(EtpMessage<GetPartsMetadata> message)
        {
            HandleRequestMessage(message, OnGetPartsMetadata, HandleGetPartsMetadata,
                responseMethod: (args) => GetPartsMetadataResponse(args.Request?.Header, args.ResponseMap, isFinalPart: !args.HasErrors, extension: args.ResponseMapExtension));
        }

        /// <summary>
        /// Handles the GetPartsMetadata message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="MapRequestEventArgs{GetPartsMetadata, PartsMetadataInfo}"/> instance containing the event data.</param>
        protected virtual void HandleGetPartsMetadata(MapRequestEventArgs<GetPartsMetadata, PartsMetadataInfo> args)
        {
        }

        /// <summary>
        /// Handles the GetChangeAnnotations message from a customer.
        /// </summary>
        /// <param name="message">The GetChangeAnnotations message.</param>
        protected virtual void HandleGetChangeAnnotations(EtpMessage<GetChangeAnnotations> message)
        {
            HandleRequestMessage(message, OnGetChangeAnnotations, HandleGetChangeAnnotations,
                responseMethod: (args) => GetChangeAnnotationsResponse(args.Request?.Header, args.ResponseMap, isFinalPart: !args.HasErrors, extension: args.ResponseMapExtension));
        }

        /// <summary>
        /// Handles the response to an GetChangeAnnotations message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="MapRequestEventArgs{GetChangeAnnotations, ChangeResponseInfo}"/> instance containing the event data.</param>
        protected virtual void HandleGetChangeAnnotations(MapRequestEventArgs<GetChangeAnnotations, ChangeResponseInfo> args)
        {
        }

        /// <summary>
        /// Handles the GetParts message from a customer.
        /// </summary>
        /// <param name="message">The GetParts message.</param>
        protected virtual void HandleGetParts(EtpMessage<GetParts> message)
        {
            HandleRequestMessage(message, OnGetParts, HandleGetParts,
                responseMethod: (args) => GetPartsResponse(args.Request?.Header, args.ResponseMap, args.Context, isFinalPart: !args.HasErrors, extension: args.ResponseMapExtension));
        }

        /// <summary>
        /// Handles the GetParts message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="MapRequestEventArgs{GetParts, ObjectPart}"/> instance containing the event data.</param>
        protected virtual void HandleGetParts(MapRequestEventArgs<GetParts, ObjectPart> args)
        {
        }

        /// <summary>
        /// Handles the GetPartsByRange message from a customer.
        /// </summary>
        /// <param name="message">The GetPartsByRange message.</param>
        protected virtual void HandleGetPartsByRange(EtpMessage<GetPartsByRange> message)
        {
            HandleRequestMessage(message, OnGetPartsByRange, HandleGetPartsByRange,
                responseMethod: (args) => GetPartsByRangeResponse(args.Request?.Header, args.Responses, args.Context, isFinalPart: !args.HasErrors, extension: args.ResponseExtension));
        }

        /// <summary>
        /// Handles the GetPartsByRange message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="ListRequestWithContextEventArgs{GetPartsByRange, ObjectPart, ResponseContext}"/> instance containing the event data.</param>
        protected virtual void HandleGetPartsByRange(ListRequestWithContextEventArgs<GetPartsByRange, ObjectPart, ResponseContext> args)
        {
        }

        /// <summary>
        /// Handles the PutParts message from a customer.
        /// </summary>
        /// <param name="message">The PutParts message.</param>
        protected virtual void HandlePutParts(EtpMessage<PutParts> message)
        {
            HandleRequestMessage(message, OnPutParts, HandlePutParts,
                responseMethod: (args) => PutPartsResponse(args.Request?.Header, args.ResponseMap, isFinalPart: !args.HasErrors, extension: args.ResponseMapExtension));
        }

        /// <summary>
        /// Handles the PutParts message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="MapRequestEventArgs{PutParts, string}"/> instance containing the event data.</param>
        protected virtual void HandlePutParts(MapRequestEventArgs<PutParts, string> args)
        {
        }

        /// <summary>
        /// Handles the DeleteParts message from a customer.
        /// </summary>
        /// <param name="message">The DeleteParts message.</param>
        protected virtual void HandleDeleteParts(EtpMessage<DeleteParts> message)
        {
            HandleRequestMessage(message, OnDeleteParts, HandleDeleteParts,
                responseMethod: (args) => DeletePartsResponse(args.Request?.Header, args.ResponseMap, isFinalPart: !args.HasErrors, extension: args.ResponseMapExtension));
        }

        /// <summary>
        /// Handles the DeleteParts message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="MapRequestEventArgs{DeleteParts, string}"/> instance containing the event data.</param>
        protected virtual void HandleDeleteParts(MapRequestEventArgs<DeleteParts, string> args)
        {
        }

        /// <summary>
        /// Handles the ReplacePartsByRange message from a customer.
        /// </summary>
        /// <param name="message">The ReplacePartsByRange message.</param>
        protected virtual void HandleReplacePartsByRange(EtpMessage<ReplacePartsByRange> message)
        {
            HandleRequestMessage(message, OnReplacePartsByRange, HandleReplacePartsByRange,
                responseMethod: (args) => ReplacePartsByRangeResponse(args.Request?.Header));
        }

        /// <summary>
        /// Handles the ReplacePartsByRange message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="EmptyRequestEventArgs{ReplacePartsByRange}"/> instance containing the event data.</param>
        protected virtual void HandleReplacePartsByRange(EmptyRequestEventArgs<ReplacePartsByRange> args)
        {
        }
    }
}

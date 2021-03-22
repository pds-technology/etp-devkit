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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Protocol.Core;
using Energistics.Etp.v12.Datatypes;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.GrowingObject
{
    /// <summary>
    /// Base implementation of the <see cref="IGrowingObjectCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.GrowingObject.IGrowingObjectCustomer" />
    public class GrowingObjectCustomerHandler : Etp12ProtocolHandler<CapabilitiesCustomer, ICapabilitiesCustomer, CapabilitiesStore, ICapabilitiesStore>, IGrowingObjectCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrowingObjectCustomerHandler"/> class.
        /// </summary>
        public GrowingObjectCustomerHandler() : base((int)Protocols.GrowingObject, Roles.Customer, Roles.Store)
        {
            RegisterMessageHandler<GetGrowingDataObjectsHeaderResponse>(Protocols.GrowingObject, MessageTypes.GrowingObject.GetGrowingDataObjectsHeaderResponse, HandleGetGrowingDataObjectsHeaderResponse);
            RegisterMessageHandler<PutGrowingDataObjectsHeaderResponse>(Protocols.GrowingObject, MessageTypes.GrowingObject.PutGrowingDataObjectsHeaderResponse, HandlePutGrowingDataObjectsHeaderResponse);
            RegisterMessageHandler<GetPartsMetadataResponse>(Protocols.GrowingObject, MessageTypes.GrowingObject.GetPartsMetadataResponse, HandleGetPartsMetadataResponse);
            RegisterMessageHandler<GetPartsResponse>(Protocols.GrowingObject, MessageTypes.GrowingObject.GetPartsResponse, HandleGetPartsResponse);
            RegisterMessageHandler<GetPartsByRangeResponse>(Protocols.GrowingObject, MessageTypes.GrowingObject.GetPartsByRangeResponse, HandleGetPartsByRangeResponse);
            RegisterMessageHandler<PutPartsResponse>(Protocols.GrowingObject, MessageTypes.GrowingObject.PutPartsResponse, HandlePutPartsResponse);
            RegisterMessageHandler<DeletePartsResponse>(Protocols.GrowingObject, MessageTypes.GrowingObject.DeletePartsResponse, HandleDeletePartsResponse);
            RegisterMessageHandler<ReplacePartsByRangeResponse>(Protocols.GrowingObject, MessageTypes.GrowingObject.ReplacePartsByRangeResponse, HandleReplacePartsByRangeResponse);
        }

        /// <summary>
        /// Sends a GetGrowingDataObjectsHeader message to a store.
        /// </summary>
        /// <param name="uris">The collection of growing object URIs.</param>
        /// <param name="format">The format of the response (XML or JSON).</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetGrowingDataObjectsHeader> GetGrowingDataObjectsHeader(IDictionary<string, string> uris, string format = Formats.Xml, IMessageHeaderExtension extension = null)
        {
            var body = new GetGrowingDataObjectsHeader
            {
                Uris = uris ?? new Dictionary<string, string>(),
                Format = format ?? Formats.Xml,
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Sends a GetGrowingDataObjectsHeader message to a store.
        /// </summary>
        /// <param name="uris">The collection of growing object URIs.</param>
        /// <param name="format">The format of the response (XML or JSON).</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetGrowingDataObjectsHeader> GetGrowingDataObjectsHeader(IList<string> uris, string format = Formats.Xml, IMessageHeaderExtension extension = null) => GetGrowingDataObjectsHeader(uris.ToMap(), format: format, extension: extension);

        /// <summary>
        /// Handles the GetGrowingDataObjectsHeaderResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<GetGrowingDataObjectsHeader, GetGrowingDataObjectsHeaderResponse>> OnGetGrowingDataObjectsHeaderResponse;

        /// <summary>
        /// Sends a PutGrowingDataObjectsHeader message to a store.
        /// </summary>
        /// <param name="dataObjects">The collection of data object headers.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutGrowingDataObjectsHeader> PutGrowingDataObjectsHeader(IDictionary<string, DataObject> dataObjects, IMessageHeaderExtension extension = null)
        {
            var body = new PutGrowingDataObjectsHeader
            {
                DataObjects = dataObjects ?? new Dictionary<string, DataObject>(),
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Sends a PutGrowingDataObjectsHeader message to a store.
        /// </summary>
        /// <param name="dataObjects">The collection of data object headers.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutGrowingDataObjectsHeader> PutGrowingDataObjectsHeader(IList<DataObject> dataObjects, IMessageHeaderExtension extension = null) => PutGrowingDataObjectsHeader(dataObjects.ToMap(), extension: extension);

        /// <summary>
        /// Handles the PutGrowingDataObjectsHeaderResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<PutGrowingDataObjectsHeader, PutGrowingDataObjectsHeaderResponse>> OnPutGrowingDataObjectsHeaderResponse;

        /// <summary>
        /// Sends a GetPartsMetadata message to a store.
        /// </summary>
        /// <param name="uris">The collection of growing object URIs.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetPartsMetadata> GetPartsMetadata(IDictionary<string, string> uris, IMessageHeaderExtension extension = null)
        {
            var body = new GetPartsMetadata
            {
                Uris = uris ?? new Dictionary<string, string>(),
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Sends a GetPartsMetadata message to a store.
        /// </summary>
        /// <param name="uris">The collection of growing object URIs.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetPartsMetadata> GetPartsMetadata(IList<string> uris, IMessageHeaderExtension extension = null) => GetPartsMetadata(uris.ToMap(), extension: extension);

        /// <summary>
        /// Handles the GetPartsMetadataResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<GetPartsMetadata, GetPartsMetadataResponse>> OnGetPartsMetadataResponse;

        /// <summary>
        /// Sends a GetParts message to a store.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="uids">The UIDs of the elements within the growing object to get.</param>
        /// <param name="format">The format of the response (XML or JSON).</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetParts> GetParts(string uri, IDictionary<string, string> uids, string format = Formats.Xml, IMessageHeaderExtension extension = null)
        {
            var body = new GetParts
            {
                Uri = uri,
                Format = format ?? Formats.Xml,
                Uids = uids ?? new Dictionary<string, string>(),
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Sends a GetParts message to a store.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="uids">The UIDs of the elements within the growing object to get.</param>
        /// <param name="format">The format of the response (XML or JSON).</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetParts> GetParts(string uri, IList<string> uids, string format = Formats.Xml, IMessageHeaderExtension extension = null) => GetParts(uri, uids.ToMap(), format: format, extension: extension);

        /// <summary>
        /// Handles the GetPartsResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<GetParts, GetPartsResponse>> OnGetPartsResponse;

        /// <summary>
        /// Sends a GetPartsByRange message to a store.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="indexInterval">The index interval.</param>
        /// <param name="includeOverlappingIntervals"><c>true</c> if overlapping intervals should be included; otherwise, <c>false</c>.</param>
        /// <param name="format">The format of the response (XML or JSON).</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetPartsByRange> GetPartsByRange(string uri, IndexInterval indexInterval, bool includeOverlappingIntervals = false, string format = Formats.Xml, IMessageHeaderExtension extension = null)
        {
            var body = new GetPartsByRange
            {
                Uri = uri,
                Format = format ?? Formats.Xml,
                IndexInterval = indexInterval,
                IncludeOverlappingIntervals = includeOverlappingIntervals,
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Handles the GetPartsByRangeResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<GetPartsByRange, GetPartsByRangeResponse>> OnGetPartsByRangeResponse;

        /// <summary>
        /// Sends a PutParts message to a store.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="parts">The UIDs and data of the parts being put.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutParts> PutParts(string uri, IDictionary<string, ObjectPart> parts, string format = Formats.Xml, IMessageHeaderExtension extension = null)
        {
            var body = new PutParts
            {
                Uri = uri,
                Format = format ?? Formats.Xml,
                Parts = parts ?? new Dictionary<string, ObjectPart>(),
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Sends a PutParts message to a store.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="parts">The UIDs and data of the parts being put.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<PutParts> PutParts(string uri, IList<ObjectPart> parts, string format = Formats.Xml, IMessageHeaderExtension extension = null) => PutParts(uri, parts.ToMap(), format: format, extension: extension);

        /// <summary>
        /// Handles the PutPartsResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<PutParts, PutPartsResponse>> OnPutPartsResponse;

        /// <summary>
        /// Sends a DeleteParts message to a store.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="uids">The UIDs of the parts within the growing object to delete.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<DeleteParts> DeleteParts(string uri, IDictionary<string, string> uids, IMessageHeaderExtension extension = null)
        {
            var body = new DeleteParts
            {
                Uri = uri,
                Uids = uids ?? new Dictionary<string, string>(),
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Sends a DeleteParts message to a store.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="uids">The UIDs of the parts within the growing object to delete.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<DeleteParts> DeleteParts(string uri, IList<string> uids, IMessageHeaderExtension extension = null) => DeleteParts(uri, uids.ToMap(), extension: extension);

        /// <summary>
        /// Handles the DeletePartsResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<DeleteParts, DeletePartsResponse>> OnDeletePartsResponse;

        /// <summary>
        /// Sends a ReplacePartsByRange message to a store.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="deleteInterval">The index interval to delete.</param>
        /// <param name="includeOverlappingIntervals"><c>true</c> if overlapping intervals should be included; otherwise, <c>false</c>.</param>
        /// <param name="parts">The map of UIDs and data of the parts being put.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<ReplacePartsByRange> ReplacePartsByRange(string uri, IndexInterval deleteInterval, bool includeOverlappingIntervals, IList<ObjectPart> parts, string format = Formats.Xml, bool isFinalPart = true, IMessageHeader correlatedHeader = null, IMessageHeaderExtension extension = null)
        {
            var body = new ReplacePartsByRange
            {
                Uri = uri,
                Format = format ?? Formats.Xml,
                DeleteInterval = deleteInterval,
                IncludeOverlappingIntervals = includeOverlappingIntervals,
                Parts = parts ?? new List<ObjectPart>(),
            };

            return SendRequest(body, extension: extension, isMultiPart: true, correlatedHeader: correlatedHeader, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Handles the ReplacePartsByRangeResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<ReplacePartsByRange, ReplacePartsByRangeResponse>> OnReplacePartsByRangeResponse;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<GetGrowingDataObjectsHeader>)
                HandleResponseMessage(request as EtpMessage<GetGrowingDataObjectsHeader>, message, OnGetGrowingDataObjectsHeaderResponse, HandleGetGrowingDataObjectsHeaderResponse);
            else if (request is EtpMessage<PutGrowingDataObjectsHeader>)
                HandleResponseMessage(request as EtpMessage<PutGrowingDataObjectsHeader>, message, OnPutGrowingDataObjectsHeaderResponse, HandlePutGrowingDataObjectsHeaderResponse);
            else if (request is EtpMessage<GetPartsMetadata>)
                HandleResponseMessage(request as EtpMessage<GetPartsMetadata>, message, OnGetPartsMetadataResponse, HandleGetPartsMetadataResponse);
            else if (request is EtpMessage<GetParts>)
                HandleResponseMessage(request as EtpMessage<GetParts>, message, OnGetPartsResponse, HandleGetPartsResponse);
            else if (request is EtpMessage<GetPartsByRange>)
                HandleResponseMessage(request as EtpMessage<GetPartsByRange>, message, OnGetPartsByRangeResponse, HandleGetPartsByRangeResponse);
            else if (request is EtpMessage<PutParts>)
                HandleResponseMessage(request as EtpMessage<PutParts>, message, OnPutPartsResponse, HandlePutPartsResponse);
            else if (request is EtpMessage<DeleteParts>)
                HandleResponseMessage(request as EtpMessage<DeleteParts>, message, OnDeletePartsResponse, HandleDeletePartsResponse);
            else if (request is EtpMessage<ReplacePartsByRange>)
                HandleResponseMessage(request as EtpMessage<ReplacePartsByRange>, message, OnReplacePartsByRangeResponse, HandleReplacePartsByRangeResponse);
        }

        /// <summary>
        /// Handles the GetGrowingDataObjectsHeaderResponse message from a store.
        /// </summary>
        /// <param name="message">The GetGrowingDataObjectsHeaderResponse message.</param>
        protected virtual void HandleGetGrowingDataObjectsHeaderResponse(EtpMessage<GetGrowingDataObjectsHeaderResponse> message)
        {
            var request = TryGetCorrelatedMessage<GetGrowingDataObjectsHeader>(message);
            HandleResponseMessage(request, message, OnGetGrowingDataObjectsHeaderResponse, HandleGetGrowingDataObjectsHeaderResponse);
        }

        /// <summary>
        /// Handles the GetGrowingDataObjectsHeaderResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{GetGrowingDataObjectsHeader, GetGrowingDataObjectsHeaderResponse}"/> instance containing the event data.</param>
        protected virtual void HandleGetGrowingDataObjectsHeaderResponse(ResponseEventArgs<GetGrowingDataObjectsHeader, GetGrowingDataObjectsHeaderResponse> args)
        {
        }

        /// <summary>
        /// Handles the PutGrowingDataObjectsHeaderResponse message from a store.
        /// </summary>
        /// <param name="message">The PutGrowingDataObjectsHeaderResponse message.</param>
        protected virtual void HandlePutGrowingDataObjectsHeaderResponse(EtpMessage<PutGrowingDataObjectsHeaderResponse> message)
        {
            var request = TryGetCorrelatedMessage<PutGrowingDataObjectsHeader>(message);
            HandleResponseMessage(request, message, OnPutGrowingDataObjectsHeaderResponse, HandlePutGrowingDataObjectsHeaderResponse);
        }

        /// <summary>
        /// Handles the PutGrowingDataObjectsHeaderResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{PutGrowingDataObjectsHeader, PutGrowingDataObjectsHeaderResponse}"/> instance containing the event data.</param>
        protected virtual void HandlePutGrowingDataObjectsHeaderResponse(ResponseEventArgs<PutGrowingDataObjectsHeader, PutGrowingDataObjectsHeaderResponse> args)
        {
        }

        /// <summary>
        /// Handles the GetPartsMetadataResponse message from a store.
        /// </summary>
        /// <param name="message">The GetPartsMetadataResponse message.</param>
        protected virtual void HandleGetPartsMetadataResponse(EtpMessage<GetPartsMetadataResponse> message)
        {
            var request = TryGetCorrelatedMessage<GetPartsMetadata>(message);
            HandleResponseMessage(request, message, OnGetPartsMetadataResponse, HandleGetPartsMetadataResponse);
        }

        /// <summary>
        /// Handles the GetPartsMetadataResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{GetPartsMetadata, GetPartsMetadataResponse}"/> instance containing the event data.</param>
        protected virtual void HandleGetPartsMetadataResponse(ResponseEventArgs<GetPartsMetadata, GetPartsMetadataResponse> args)
        {
        }

        /// <summary>
        /// Handles the GetPartsResponse message from a store.
        /// </summary>
        /// <param name="message">The GetPartsResponse message.</param>
        protected virtual void HandleGetPartsResponse(EtpMessage<GetPartsResponse> message)
        {
            var request = TryGetCorrelatedMessage<GetParts>(message);
            HandleResponseMessage(request, message, OnGetPartsResponse, HandleGetPartsResponse);
        }

        /// <summary>
        /// Handles the GetPartsResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{GetParts, GetPartsResponse}"/> instance containing the event data.</param>
        protected virtual void HandleGetPartsResponse(ResponseEventArgs<GetParts, GetPartsResponse> args)
        {
        }

        /// <summary>
        /// Handles the GetPartsByRangeResponse message from a store.
        /// </summary>
        /// <param name="message">The GetPartsByRangeResponse message.</param>
        protected virtual void HandleGetPartsByRangeResponse(EtpMessage<GetPartsByRangeResponse> message)
        {
            var request = TryGetCorrelatedMessage<GetPartsByRange>(message);
            HandleResponseMessage(request, message, OnGetPartsByRangeResponse, HandleGetPartsByRangeResponse);
        }

        /// <summary>
        /// Handles the GetPartsByRangeResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{GetPartsByRange, GetPartsByRangeResponse}"/> instance containing the event data.</param>
        protected virtual void HandleGetPartsByRangeResponse(ResponseEventArgs<GetPartsByRange, GetPartsByRangeResponse> args)
        {
        }

        /// <summary>
        /// Handles the PutPartsResponse message from a store.
        /// </summary>
        /// <param name="message">The PutPartsResponse message.</param>
        protected virtual void HandlePutPartsResponse(EtpMessage<PutPartsResponse> message)
        {
            var request = TryGetCorrelatedMessage<PutParts>(message);
            HandleResponseMessage(request, message, OnPutPartsResponse, HandlePutPartsResponse);
        }

        /// <summary>
        /// Handles the PutPartsResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{PutParts, PutPartsResponse}"/> instance containing the event data.</param>
        protected virtual void HandlePutPartsResponse(ResponseEventArgs<PutParts, PutPartsResponse> args)
        {
        }

        /// <summary>
        /// Handles the DeletePartsResponse message from a store.
        /// </summary>
        /// <param name="message">The DeletePartsResponse message.</param>
        protected virtual void HandleDeletePartsResponse(EtpMessage<DeletePartsResponse> message)
        {
            var request = TryGetCorrelatedMessage<DeleteParts>(message);
            HandleResponseMessage(request, message, OnDeletePartsResponse, HandleDeletePartsResponse);
        }

        /// <summary>
        /// Handles the DeletePartsResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{DeleteParts, DeletePartsResponse}"/> instance containing the event data.</param>
        protected virtual void HandleDeletePartsResponse(ResponseEventArgs<DeleteParts, DeletePartsResponse> args)
        {
        }

        /// <summary>
        /// Handles the ReplacePartsByRangeResponse message from a store.
        /// </summary>
        /// <param name="message">The ReplacePartsByRangeResponse message.</param>
        protected virtual void HandleReplacePartsByRangeResponse(EtpMessage<ReplacePartsByRangeResponse> message)
        {
            var request = TryGetCorrelatedMessage<ReplacePartsByRange>(message);
            HandleResponseMessage(request, message, OnReplacePartsByRangeResponse, HandleReplacePartsByRangeResponse);
        }

        /// <summary>
        /// Handles the ReplacePartsByRangeResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{ReplacePartsByRange, ReplacePartsByRangeResponse}"/> instance containing the event data.</param>
        protected virtual void HandleReplacePartsByRangeResponse(ResponseEventArgs<ReplacePartsByRange, ReplacePartsByRangeResponse> args)
        {
        }
    }
}

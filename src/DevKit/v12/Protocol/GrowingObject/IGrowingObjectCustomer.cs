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
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.GrowingObject
{
    /// <summary>
    /// Defines the interface that must be implemented by the customer role of the growing object protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.GrowingObject, Roles.Customer, Roles.Store)]
    public interface IGrowingObjectCustomer : IProtocolHandler<ICapabilitiesCustomer, ICapabilitiesStore>
    {
        /// <summary>
        /// Sends a GetGrowingDataObjectsHeader message to a store.
        /// </summary>
        /// <param name="uris">The collection of growing object URIs.</param>
        /// <param name="format">The format of the response (XML or JSON).</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetGrowingDataObjectsHeader> GetGrowingDataObjectsHeader(IDictionary<string, string> uris, string format = Formats.Xml, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a GetGrowingDataObjectsHeader message to a store.
        /// </summary>
        /// <param name="uris">The collection of growing object URIs.</param>
        /// <param name="format">The format of the response (XML or JSON).</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetGrowingDataObjectsHeader> GetGrowingDataObjectsHeader(IList<string> uris, string format = Formats.Xml, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the GetGrowingDataObjectsHeaderResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<GetGrowingDataObjectsHeader, GetGrowingDataObjectsHeaderResponse>> OnGetGrowingDataObjectsHeaderResponse;

        /// <summary>
        /// Sends a PutGrowingDataObjectsHeader message to a store.
        /// </summary>
        /// <param name="dataObjects">The collection of data object headers.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<PutGrowingDataObjectsHeader> PutGrowingDataObjectsHeader(IDictionary<string, DataObject> dataObjects, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a PutGrowingDataObjectsHeader message to a store.
        /// </summary>
        /// <param name="dataObjects">The collection of data object headers.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<PutGrowingDataObjectsHeader> PutGrowingDataObjectsHeader(IList<DataObject> dataObjects, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the PutGrowingDataObjectsHeaderResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<PutGrowingDataObjectsHeader, PutGrowingDataObjectsHeaderResponse>> OnPutGrowingDataObjectsHeaderResponse;

        /// <summary>
        /// Sends a GetPartsMetadata message to a store.
        /// </summary>
        /// <param name="uris">The collection of growing object URIs.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetPartsMetadata> GetPartsMetadata(IDictionary<string, string> uris, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a GetPartsMetadata message to a store.
        /// </summary>
        /// <param name="uris">The collection of growing object URIs.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetPartsMetadata> GetPartsMetadata(IList<string> uris, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the GetPartsMetadataResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<GetPartsMetadata, GetPartsMetadataResponse>> OnGetPartsMetadataResponse;

        /// <summary>
        /// Sends a GetChangeAnnotations message to a store.
        /// </summary>
        /// <param name="uris">The URIs.</param>
        /// <param name="latestOnly">Whether or not to only get the latest change annotation for each growing object.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetChangeAnnotations> GetChangeAnnotations(IDictionary<string, string> uris, bool latestOnly = false, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a GetChangeAnnotations message to a store.
        /// </summary>
        /// <param name="uris">The URIs.</param>
        /// <param name="latestOnly">Whether or not to only get the latest change annotation for each growing object.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetChangeAnnotations> GetChangeAnnotations(IList<string> uris, bool latestOnly = false, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the GetChangeAnnotationsResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<GetChangeAnnotations, GetChangeAnnotationsResponse>> OnGetChangeAnnotationsResponse;

        /// <summary>
        /// Sends a GetParts message to a store.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="uids">The UIDs of the elements within the growing object to get.</param>
        /// <param name="format">The format of the response (XML or JSON).</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetParts> GetParts(string uri, IDictionary<string, string> uids, string format = Formats.Xml, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a GetParts message to a store.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="uids">The UIDs of the elements within the growing object to get.</param>
        /// <param name="format">The format of the response (XML or JSON).</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetParts> GetParts(string uri, IList<string> uids, string format = Formats.Xml, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the GetPartsResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<GetParts, GetPartsResponse>> OnGetPartsResponse;

        /// <summary>
        /// Sends a GetPartsByRange message to a store.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="indexInterval">The index interval.</param>
        /// <param name="includeOverlappingIntervals"><c>true</c> if overlapping intervals should be included; otherwise, <c>false</c>.</param>
        /// <param name="format">The format of the response (XML or JSON).</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetPartsByRange> GetPartsByRange(string uri, IndexInterval indexInterval, bool includeOverlappingIntervals = false, string format = Formats.Xml, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the GetPartsByRangeResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<GetPartsByRange, GetPartsByRangeResponse>> OnGetPartsByRangeResponse;

        /// <summary>
        /// Sends a PutParts message to a store.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="parts">The UIDs and data of the parts being put.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<PutParts> PutParts(string uri, IDictionary<string, ObjectPart> parts, string format = Formats.Xml, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a PutParts message to a store.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="parts">The UIDs and data of the parts being put.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<PutParts> PutParts(string uri, IList<ObjectPart> parts, string format = Formats.Xml, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the PutPartsResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<PutParts, PutPartsResponse>> OnPutPartsResponse;

        /// <summary>
        /// Sends a DeleteParts message to a store.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="uids">The UIDs of the parts within the growing object to delete.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<DeleteParts> DeleteParts(string uri, IDictionary<string, string> uids, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a DeleteParts message to a store.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="uids">The UIDs of the parts within the growing object to delete.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<DeleteParts> DeleteParts(string uri, IList<string> uids, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the DeletePartsResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<DeleteParts, DeletePartsResponse>> OnDeletePartsResponse;

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
        EtpMessage<ReplacePartsByRange> ReplacePartsByRange(string uri, IndexInterval deleteInterval, bool includeOverlappingIntervals, IList<ObjectPart> parts, string format = Formats.Xml, bool isFinalPart = true, IMessageHeader correlatedHeader = null, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the ReplacePartsByRangeResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<ReplacePartsByRange, ReplacePartsByRangeResponse>> OnReplacePartsByRangeResponse;
    }
}

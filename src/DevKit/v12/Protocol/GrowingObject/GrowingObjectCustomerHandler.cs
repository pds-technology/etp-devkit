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

using System.Collections.Concurrent;
using System.Collections.Generic;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v12.Datatypes;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.GrowingObject
{
    /// <summary>
    /// Base implementation of the <see cref="IGrowingObjectCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.GrowingObject.IGrowingObjectCustomer" />
    public class GrowingObjectCustomerHandler : Etp12ProtocolHandler, IGrowingObjectCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrowingObjectCustomerHandler"/> class.
        /// </summary>
        public GrowingObjectCustomerHandler() : base((int)Protocols.GrowingObject, "customer", "store")
        {
            Metadata = new ConcurrentBag<PartsMetadataInfo>();
            Errors = new ConcurrentBag<ErrorInfo>();

            RegisterMessageHandler<GetPartsResponse>(Protocols.GrowingObject, MessageTypes.GrowingObject.GetPartsResponse, HandleGetPartsResponse);
            RegisterMessageHandler<GetPartsMetadataResponse>(Protocols.GrowingObject, MessageTypes.GrowingObject.GetPartsMetadataResponse, HandleGetPartsMetadataResponse);
        }

        /// <summary>
        /// Gets the collection of parts metadata infos.
        /// </summary>
        protected ConcurrentBag<PartsMetadataInfo> Metadata { get; }

        /// <summary>
        /// Gets the collection of errors.
        /// </summary>
        protected ConcurrentBag<ErrorInfo> Errors { get; }

        /// <summary>
        /// Gets the metadata for growing object parts.
        /// </summary>
        /// <param name="uris">The collection of growing object URIs.</param>
        /// <returns>The message identifier.</returns>
        public long GetPartsMetadata(IList<string> uris)
        {
            var header = CreateMessageHeader(Protocols.GrowingObject, MessageTypes.GrowingObject.GetPartsMetadata);

            var message = new GetPartsMetadata
            {
                Uris = uris
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Gets a single list item in a growing object, by its ID.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="uid">The ID of the element within the list.</param>
        /// <returns>The message identifier.</returns>
        public long GetPart(string uri, string uid)
        {
            var header = CreateMessageHeader(Protocols.GrowingObject, MessageTypes.GrowingObject.GetPart);

            var message = new GetPart
            {
                Uri = uri,
                Uid = uid
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Gets all list items in a growing object within an index range.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="uom">The unit of measure.</param>
        /// <param name="depthDatum">The depth datum.</param>
        /// <param name="includeOverlappingIntervals"><c>true</c> if overlapping intervals should be included; otherwise, <c>false</c>.</param>
        /// <returns>The message identifier.</returns>
        public long GetPartsByRange(string uri, object startIndex, object endIndex, string uom, string depthDatum, bool includeOverlappingIntervals = false)
        {
            var header = CreateMessageHeader(Protocols.GrowingObject, MessageTypes.GrowingObject.GetPartsByRange);

            var message = new GetPartsByRange
            {
                Uri = uri,
                IndexInterval = new IndexInterval
                {
                    StartIndex = new IndexValue { Item = startIndex },
                    EndIndex = new IndexValue { Item = endIndex },
                    Uom = uom ?? string.Empty,
                    DepthDatum = depthDatum ?? string.Empty
                },
                IncludeOverlappingIntervals = includeOverlappingIntervals
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Adds or updates a list item in a growing object.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="uid">The ID of the element within the list.</param>
        /// <param name="contentType">The content type string for the parent object.</param>
        /// <param name="data">The data (list items) to be added to the growing object.</param>
        /// <returns>The message identifier.</returns>
        public long PutPart(string uri, string uid, string contentType, byte[] data)
        {
            var header = CreateMessageHeader(Protocols.GrowingObject, MessageTypes.GrowingObject.PutPart);

            var message = new PutPart
            {
                Uri = uri,
                Uid = uid, 
                ContentType = contentType,
                Data = data
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Deletes one list item in a growing object.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="uid">The ID of the element within the list.</param>
        /// <returns>The message identifier.</returns>
        public long DeletePart(string uri, string uid)
        {
            var header = CreateMessageHeader(Protocols.GrowingObject, MessageTypes.GrowingObject.DeletePart);

            var message = new DeletePart
            {
                Uri = uri,
                Uid = uid
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Deletes all list items in a range of index values.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="uom">The unit of measure.</param>
        /// <param name="depthDatum">The depth datum.</param>
        /// <param name="includeOverlappingIntervals"><c>true</c> if overlapping intervals should be included; otherwise, <c>false</c>.</param>
        /// <returns>The message identifier.</returns>
        public long DeletePartsByRange(string uri, object startIndex, object endIndex, string uom, string depthDatum, bool includeOverlappingIntervals = false)
        {
            var header = CreateMessageHeader(Protocols.GrowingObject, MessageTypes.GrowingObject.DeletePartsByRange);

            var message = new DeletePartsByRange
            {
                Uri = uri,
                DeleteInterval = new IndexInterval
                {
                    StartIndex = new IndexValue { Item = startIndex },
                    EndIndex = new IndexValue { Item = endIndex },
                    Uom = uom ?? string.Empty,
                    DepthDatum = depthDatum ?? string.Empty
                },
                IncludeOverlappingIntervals = includeOverlappingIntervals
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Replaces a list item in a growing object.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="uid">The ID of the element within the list.</param>
        /// <param name="contentType">The content type string for the parent object.</param>
        /// <param name="data">The data (list items) to be added to the growing object.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="uom">The unit of measure.</param>
        /// <param name="depthDatum">The depth datum.</param>
        /// <param name="includeOverlappingIntervals"><c>true</c> if overlapping intervals should be included; otherwise, <c>false</c>.</param>
        /// <returns>The message identifier.</returns>
        public long ReplacePartsByRange(string uri, string uid, string contentType, byte[] data, object startIndex, object endIndex, string uom, string depthDatum, bool includeOverlappingIntervals)
        {
            var header = CreateMessageHeader(Protocols.GrowingObject, MessageTypes.GrowingObject.ReplacePartsByRange);

            var message = new ReplacePartsByRange
            {
                Uri = uri,
                Uid = uid,
                ContentType = contentType,
                Data = data,
                DeleteInterval = new IndexInterval
                {
                    StartIndex = new IndexValue { Item = startIndex },
                    EndIndex = new IndexValue { Item = endIndex },
                    Uom = uom,
                    DepthDatum = depthDatum
                },
                IncludeOverlappingIntervals = includeOverlappingIntervals
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Handles the GetPartsResponse event from a store.
        /// </summary>
        public event ProtocolEventHandler<GetPartsResponse> OnGetPartsResponse;

        /// <summary>
        /// Handles the GetPartsMetadataResponse event from a store.
        /// </summary>
        public event ProtocolEventHandler<GetPartsMetadataResponse> OnGetPartsMetadataResponse;

        /// <summary>
        /// Handles the GetPartsResponse message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GetPartsResponse message.</param>
        protected virtual void HandleGetPartsResponse(IMessageHeader header, GetPartsResponse message)
        {
            Notify(OnGetPartsResponse, header, message);
        }

        /// <summary>
        /// Handles the GetPartsMetadataResponse message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GetPartsMetadataResponse message.</param>
        protected virtual void HandleGetPartsMetadataResponse(IMessageHeader header, GetPartsMetadataResponse message)
        {
            foreach (var metadata in message.Metadata)
                Metadata.Add(metadata);

            foreach (var error in message.Errors)
                Errors.Add(error);

            Notify(OnGetPartsMetadataResponse, header, message);
        }
    }
}

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
using System.Linq;
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
            RegisterMessageHandler<GetPartsResponse>(Protocols.GrowingObject, MessageTypes.GrowingObject.GetPartsResponse, HandleGetPartsResponse);
            RegisterMessageHandler<GetPartsByRangeResponse>(Protocols.GrowingObject, MessageTypes.GrowingObject.GetPartsByRangeResponse, HandleGetPartsByRangeResponse);
            RegisterMessageHandler<GetPartsMetadataResponse>(Protocols.GrowingObject, MessageTypes.GrowingObject.GetPartsMetadataResponse, HandleGetPartsMetadataResponse);
        }

        /// <summary>
        /// Gets parts in a growing object by UID from a store.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="uids">The UIDs of the elements within the growing object to get.</param>
        /// <param name="format">The format of the response (XML or JSON).</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long GetParts(string uri, IList<string> uids, string format = "xml")
        {
            var header = CreateMessageHeader(Protocols.GrowingObject, MessageTypes.GrowingObject.GetParts);
            var list = new List<string>();
            var message = new GetParts
            {
                Uri = uri,
                Uids = uids.ToMap(),
                Format = format ?? "xml",
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Handles the GetPartsResponse event from a store.
        /// </summary>
        public event ProtocolEventHandler<GetPartsResponse> OnGetPartsResponse;

        /// <summary>
        /// Gets all parts in a growing object within an index range from a store.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="indexInterval">The index interval.</param>
        /// <param name="includeOverlappingIntervals"><c>true</c> if overlapping intervals should be included; otherwise, <c>false</c>.</param>
        /// <param name="format">The format of the response (XML or JSON).</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long GetPartsByRange(string uri, IndexInterval indexInterval, bool includeOverlappingIntervals = false, string format = "xml")
        {
            var header = CreateMessageHeader(Protocols.GrowingObject, MessageTypes.GrowingObject.GetPartsByRange);

            var message = new GetPartsByRange
            {
                Uri = uri,
                IndexInterval = indexInterval,
                IncludeOverlappingIntervals = includeOverlappingIntervals,
                Format = format ?? "xml",
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Handles the GetPartsByRangeResponse event from a store.
        /// </summary>
        public event ProtocolEventHandler<GetPartsByRangeResponse> OnGetPartsByRangeResponse;

        /// <summary>
        /// Adds or updates parts in a growing object in a store.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="parts">The UIDs and data of the parts being put.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long PutParts(string uri, IList<ObjectPart> parts, string format = "xml")
        {
            var header = CreateMessageHeader(Protocols.GrowingObject, MessageTypes.GrowingObject.PutParts);

            var message = new PutParts
            {
                Uri = uri,
                Parts = parts.ToMap(),
                Format = format ?? "xml",
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Deletes parts from a growing object from a store.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="uids">The UIDs of the parts within the growing object to delete.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long DeleteParts(string uri, IList<string> uids)
        {
            var header = CreateMessageHeader(Protocols.GrowingObject, MessageTypes.GrowingObject.DeleteParts);

            var message = new DeleteParts
            {
                Uri = uri,
                Uids = uids.ToMap(),
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Replaces all parts in a range of index values in a growing object with new parts in a store.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="deleteInterval">The index interval to delete.</param>
        /// <param name="includeOverlappingIntervals"><c>true</c> if overlapping intervals should be included; otherwise, <c>false</c>.</param>
        /// <param name="parts">The map of UIDs and data of the parts being put.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long ReplacePartsByRange(string uri, IndexInterval deleteInterval, bool includeOverlappingIntervals, IList<ObjectPart> parts, string format = "xml")
        {
            var header = CreateMessageHeader(Protocols.GrowingObject, MessageTypes.GrowingObject.ReplacePartsByRange);

            var message = new ReplacePartsByRange
            {
                Uri = uri,
                Parts = parts,
                DeleteInterval = deleteInterval,
                IncludeOverlappingIntervals = includeOverlappingIntervals,
                Format = format ?? "xml",
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Gets the metadata for growing object parts from a store.
        /// </summary>
        /// <param name="uris">The collection of growing object URIs.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long GetPartsMetadata(IList<string> uris)
        {
            var header = CreateMessageHeader(Protocols.GrowingObject, MessageTypes.GrowingObject.GetPartsMetadata);

            var message = new GetPartsMetadata
            {
                Uris = uris.ToDictionary(uri => uri, uri => string.Empty),
            };

            return Session.SendMessage(header, message);
        }

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
        /// Handles the GetPartsByRangeResponse message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GetPartsResponse message.</param>
        protected virtual void HandleGetPartsByRangeResponse(IMessageHeader header, GetPartsByRangeResponse message)
        {
            Notify(OnGetPartsByRangeResponse, header, message);
        }

        /// <summary>
        /// Handles the GetPartsMetadataResponse message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GetPartsMetadataResponse message.</param>
        protected virtual void HandleGetPartsMetadataResponse(IMessageHeader header, GetPartsMetadataResponse message)
        {
            Notify(OnGetPartsMetadataResponse, header, message);
        }

    }
}

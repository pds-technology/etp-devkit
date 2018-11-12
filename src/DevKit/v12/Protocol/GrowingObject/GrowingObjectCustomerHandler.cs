//----------------------------------------------------------------------- 
// ETP DevKit, 1.2
//
// Copyright 2018 Energistics
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

using Avro.IO;
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
        }

        /// <summary>
        /// Gets or sets the parts metadata.
        /// </summary>
        protected PartsMetadata Metadata { get; set; }

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
        /// Gets the metadata for all list items in a growing object.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <returns>The message identifier.</returns>
        public long DescribeParts(string uri)
        {
            var header = CreateMessageHeader(Protocols.GrowingObject, MessageTypes.GrowingObject.DescribeParts);

            var message = new DescribeParts
            {
                Uri = uri
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Handles the ObjectPart event from a store.
        /// </summary>
        public event ProtocolEventHandler<ObjectPart> OnObjectPart;

        /// <summary>
        /// Handles the PartsMetadata event from a store.
        /// </summary>
        public event ProtocolEventHandler<PartsMetadata> OnPartsMetadata;

        /// <summary>
        /// Decodes the message based on the message type contained in the specified <see cref="IMessageHeader" />.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="decoder">The message decoder.</param>
        /// <param name="body">The message body.</param>
        protected override void HandleMessage(IMessageHeader header, Decoder decoder, string body)
        {
            switch (header.MessageType)
            {
                case (int)MessageTypes.GrowingObject.ObjectPart:
                    HandleObjectPart(header, decoder.Decode<ObjectPart>(body));
                    break;

                case (int)MessageTypes.GrowingObject.PartsMetadata:
                    HandlePartsMetadata(header, decoder.Decode<PartsMetadata>(body));
                    break;

                default:
                    base.HandleMessage(header, decoder, body);
                    break;
            }
        }

        /// <summary>
        /// Handles the ObjectPart message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The ObjectPart message.</param>
        protected virtual void HandleObjectPart(IMessageHeader header, ObjectPart message)
        {
            Notify(OnObjectPart, header, message);
        }

        /// <summary>
        /// Handles the PartsMetadata message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The PartsMetadata message.</param>
        protected virtual void HandlePartsMetadata(IMessageHeader header, PartsMetadata message)
        {
            Metadata = message;
            Notify(OnPartsMetadata, header, message);
        }
    }
}

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

using Energistics.Etp.Common;
using Energistics.Etp.Common.Protocol.Core;
using Energistics.Etp.v11.Datatypes.Object;
using System;

namespace Energistics.Etp.v11.Protocol.GrowingObject
{
    /// <summary>
    /// Base implementation of the <see cref="IGrowingObjectCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp11ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v11.Protocol.GrowingObject.IGrowingObjectCustomer" />
    public class GrowingObjectCustomerHandler : Etp11ProtocolHandler, IGrowingObjectCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrowingObjectCustomerHandler"/> class.
        /// </summary>
        public GrowingObjectCustomerHandler() : base((int)Protocols.GrowingObject, Roles.Customer, Roles.Store)
        {
            RegisterMessageHandler<ObjectFragment>(Protocols.GrowingObject, MessageTypes.GrowingObject.ObjectFragment, HandleObjectFragment);
        }

        /// <summary>
        /// Gets a single list item in a growing object, by its ID.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="uid">The ID of the element within the list.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GrowingObjectGet> GrowingObjectGet(string uri, string uid)
        {
            var body = new GrowingObjectGet
            {
                Uri = uri ?? string.Empty,
                Uid = uid ?? string.Empty,
            };

            return SendRequest(body);
        }

        /// <summary>
        /// Gets all list items in a growing object within an index range.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="uom">The unit of measure.</param>
        /// <param name="depthDatum">The depth datum.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GrowingObjectGetRange> GrowingObjectGetRange(string uri, object startIndex, object endIndex, string uom, string depthDatum)
        {
            var body = new GrowingObjectGetRange
            {
                Uri = uri ?? string.Empty,
                StartIndex = new GrowingObjectIndex { Item = startIndex },
                EndIndex = new GrowingObjectIndex { Item = endIndex },
                Uom = uom ?? string.Empty,
                DepthDatum = depthDatum ?? string.Empty
            };

            return SendRequest(body);
        }

        /// <summary>
        /// Adds or updates a list item in a growing object.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="contentType">The content type string for the parent object.</param>
        /// <param name="data">The data (list items) to be added to the growing object.</param>
        /// <param name="contentEncoding">The content encoding the data.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GrowingObjectPut> GrowingObjectPut(string uri, string contentType, byte[] data, string contentEncoding = ContentEncodings.TextXml)
        {
            var body = new GrowingObjectPut
            {
                Uri = uri ?? string.Empty,
                ContentType = contentType ?? string.Empty,
                ContentEncoding = contentEncoding ?? ContentEncodings.TextXml,
                Data = data ?? new byte[0],
            };

            return SendRequest(body);
        }

        /// <summary>
        /// Deletes one list item in a growing object.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="uid">The ID of the element within the list.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GrowingObjectDelete> GrowingObjectDelete(string uri, string uid)
        {
            var body = new GrowingObjectDelete
            {
                Uri = uri ?? string.Empty,
                Uid = uid ?? string.Empty,
            };

            return SendRequest(body);
        }

        /// <summary>
        /// Deletes all list items in a range of index values. Range is inclusive of the limits.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GrowingObjectDeleteRange> GrowingObjectDeleteRange(string uri, object startIndex, object endIndex, string uom, string depthDatum)
        {
            var body = new GrowingObjectDeleteRange
            {
                Uri = uri ?? string.Empty,
                StartIndex = new GrowingObjectIndex { Item = startIndex },
                EndIndex = new GrowingObjectIndex { Item = endIndex },
                Uom = uom ?? string.Empty,
                DepthDatum = depthDatum ?? string.Empty,
            };

            return SendRequest(body);
        }

        /// <summary>
        /// Handles the ObjectFragment event from a store in response to GrowingObjectGet.
        /// </summary>
        public event EventHandler<ResponseEventArgs<GrowingObjectGet, ObjectFragment>> OnGrowingObjectGetObjectFragment;

        /// <summary>
        /// Handles the ObjectFragment event from a store in response to GrowingObjectGetRange.
        /// </summary>
        public event EventHandler<ResponseEventArgs<GrowingObjectGetRange, ObjectFragment>> OnGrowingObjectGetRangeObjectFragment;

        /// <summary>
        /// Event raised when there is an exception received in response to a GrowingObjectPut message.
        /// </summary>
        public event EventHandler<VoidResponseEventArgs<GrowingObjectPut>> OnGrowingObjectPutException;

        /// <summary>
        /// Event raised when there is an exception received in response to a GrowingObjectDelete message.
        /// </summary>
        public event EventHandler<VoidResponseEventArgs<GrowingObjectDelete>> OnGrowingObjectDeleteException;

        /// <summary>
        /// Event raised when there is an exception received in response to a GrowingObjectDeleteRange message.
        /// </summary>
        public event EventHandler<VoidResponseEventArgs<GrowingObjectDeleteRange>> OnGrowingObjectDeleteRangeException;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<GrowingObjectGet>)
                HandleResponseMessage(request as EtpMessage<GrowingObjectGet>, message, OnGrowingObjectGetObjectFragment, HandleGrowingObjectGetObjectFragment);
            else if (request is EtpMessage<GrowingObjectGetRange>)
                HandleResponseMessage(request as EtpMessage<GrowingObjectGetRange>, message, OnGrowingObjectGetRangeObjectFragment, HandleGrowingObjectGetRangeObjectFragment);
            else if (request is EtpMessage<GrowingObjectPut>)
                HandleResponseMessage(request as EtpMessage<GrowingObjectPut>, message, OnGrowingObjectPutException, HandleGrowingObjectPutException);
            else if (request is EtpMessage<GrowingObjectDelete>)
                HandleResponseMessage(request as EtpMessage<GrowingObjectDelete>, message, OnGrowingObjectDeleteException, HandleGrowingObjectDeleteException);
            else if (request is EtpMessage<GrowingObjectDeleteRange>)
                HandleResponseMessage(request as EtpMessage<GrowingObjectDeleteRange>, message, OnGrowingObjectDeleteRangeException, HandleGrowingObjectDeleteRangeException);
        }

        /// <summary>
        /// Handles the ObjectFragment message from a store.
        /// </summary>
        /// <param name="message">The ObjectFragment message.</param>
        protected virtual void HandleObjectFragment(EtpMessage<ObjectFragment> message)
        {
            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<GrowingObjectGet>)
                HandleResponseMessage(request as EtpMessage<GrowingObjectGet>, message, OnGrowingObjectGetObjectFragment, HandleGrowingObjectGetObjectFragment);
            else if (request is EtpMessage<GrowingObjectGetRange>)
                HandleResponseMessage(request as EtpMessage<GrowingObjectGetRange>, message, OnGrowingObjectGetRangeObjectFragment, HandleGrowingObjectGetRangeObjectFragment);
        }

        /// <summary>
        /// Handles the ObjectFragment message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{GrowingObjectGet, ObjectFragment}"/> instance containing the event data.</param>
        protected virtual void HandleGrowingObjectGetObjectFragment(ResponseEventArgs<GrowingObjectGet, ObjectFragment> args)
        {
        }

        /// <summary>
        /// Handles the ObjectFragment message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{GrowingObjectGetRange, ObjectFragment}"/> instance containing the event data.</param>
        protected virtual void HandleGrowingObjectGetRangeObjectFragment(ResponseEventArgs<GrowingObjectGetRange, ObjectFragment> args)
        {
        }

        /// <summary>
        /// Handles exceptions to the GrowingObjectPut message from a store.
        /// </summary>
        /// <param name="args">The <see cref="VoidResponseEventArgs{GrowingObjectPut}"/> instance containing the event data.</param>
        protected virtual void HandleGrowingObjectPutException(VoidResponseEventArgs<GrowingObjectPut> args)
        {
        }

        /// <summary>
        /// Handles exceptions to the GrowingObjectDelete message from a store.
        /// </summary>
        /// <param name="args">The <see cref="VoidResponseEventArgs{GrowingObjectDelete}"/> instance containing the event data.</param>
        protected virtual void HandleGrowingObjectDeleteException(VoidResponseEventArgs<GrowingObjectDelete> args)
        {
        }

        /// <summary>
        /// Handles exceptions to the GrowingObjectDeleteRange message from a store.
        /// </summary>
        /// <param name="args">The <see cref="VoidResponseEventArgs{GrowingObjectDeleteRange}"/> instance containing the event data.</param>
        protected virtual void HandleGrowingObjectDeleteRangeException(VoidResponseEventArgs<GrowingObjectDeleteRange> args)
        {
        }
    }
}

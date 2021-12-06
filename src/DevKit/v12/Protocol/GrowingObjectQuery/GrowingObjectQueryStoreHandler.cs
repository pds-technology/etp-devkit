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
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.GrowingObjectQuery
{
    /// <summary>
    /// Base implementation of the <see cref="IGrowingObjectQueryStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.GrowingObjectQuery.IGrowingObjectQueryStore" />
    public class GrowingObjectQueryStoreHandler : Etp12ProtocolHandler<CapabilitiesStore, ICapabilitiesStore, CapabilitiesCustomer, ICapabilitiesCustomer>, IGrowingObjectQueryStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrowingObjectQueryStoreHandler"/> class.
        /// </summary>
        public GrowingObjectQueryStoreHandler() : base((int)Protocols.GrowingObjectQuery, Roles.Store, Roles.Customer)
        {
            RegisterMessageHandler<FindParts>(Protocols.GrowingObjectQuery, MessageTypes.GrowingObjectQuery.FindParts, HandleFindParts);
        }

        /// <summary>
        /// Handles the FindParts event from a customer.
        /// </summary>
        public event EventHandler<ListRequestWithContextEventArgs<FindParts, ObjectPart, ResponseContext>> OnFindParts;

        /// <summary>
        /// Sends a FindPartsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="parts">The list of <see cref="ObjectPart"/> objects.</param>
        /// <param name="context">The response context.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<FindPartsResponse> FindPartsResponse(IMessageHeader correlatedHeader, IList<ObjectPart> parts, ResponseContext context, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new FindPartsResponse
            {
                Uri = context?.Uri ?? string.Empty,
                Format = context?.Format ?? Formats.Xml,
                ServerSortOrder = context?.ServerSortOrder ?? string.Empty,
                Parts = parts ?? new List<ObjectPart>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }
        /// <summary>
        /// Sends a FindPartsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="uri">The URI from the request.</param>
        /// <param name="parts">The list of <see cref="ObjectPart"/> objects.</param>
        /// <param name="serverSortOrder">The sort order.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<FindPartsResponse> FindPartsResponse(IMessageHeader correlatedHeader, string uri, IList<ObjectPart> parts, string serverSortOrder, string format = Formats.Xml, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            return FindPartsResponse(correlatedHeader, parts, new ResponseContext { Uri = uri, ServerSortOrder = serverSortOrder, Format = format }, isFinalPart: isFinalPart, extension: extension);
        }

        /// <summary>
        /// Handles the FindParts message from a customer.
        /// </summary>
        /// <param name="message">The FindParts message.</param>
        protected virtual void HandleFindParts(EtpMessage<FindParts> message)
        {
            HandleRequestMessage(message, OnFindParts, HandleFindParts,
                responseMethod: (args) => FindPartsResponse(args.Request?.Header, args.Responses, args.Context, isFinalPart: !args.HasErrors, extension: args.ResponseExtension));
        }

        /// <summary>
        /// Handles the FindParts message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="ListRequestWithContextEventArgs{FindParts, ObjectPart, ResponseContext}"/> instance containing the event data.</param>
        protected virtual void HandleFindParts(ListRequestWithContextEventArgs<FindParts, ObjectPart, ResponseContext> args)
        {
        }
    }
}

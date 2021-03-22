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

namespace Energistics.Etp.v12.Protocol.GrowingObjectQuery
{
    /// <summary>
    /// Describes the interface that must be implemented by the store role of the GrowingObjectQuery protocol.
    /// </summary>
    /// <seealso cref="IProtocolHandler" />
    [ProtocolRole((int)Protocols.GrowingObjectQuery, Roles.Store, Roles.Customer)]
    public interface IGrowingObjectQueryStore : IProtocolHandler<ICapabilitiesStore, ICapabilitiesCustomer>
    {
        /// <summary>
        /// Handles the FindParts event from a customer.
        /// </summary>
        event EventHandler<ListRequestWithContextEventArgs<FindParts, ObjectPart, ResponseContext>> OnFindParts;

        /// <summary>
        /// Sends a FindPartsResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="parts">The list of <see cref="ObjectPart"/> objects.</param>
        /// <param name="context">The response context.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<FindPartsResponse> FindPartsResponse(IMessageHeader correlatedHeader, IList<ObjectPart> parts, ResponseContext context, bool isFinalPart = true, IMessageHeaderExtension extension = null);

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
        EtpMessage<FindPartsResponse> FindPartsResponse(IMessageHeader correlatedHeader, string uri, IList<ObjectPart> parts, string serverSortOrder, string format = Formats.Xml, bool isFinalPart = true, IMessageHeaderExtension extension = null);
    }

    /// <summary>
    /// Encapsulates the context of a growing object query response.
    /// </summary>
    public class ResponseContext
    {
        /// <summary>
        /// Gets or sets teh URI.
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets the server sort order.
        /// </summary>
        public string ServerSortOrder { get; set; }

        /// <summary>
        /// Gets or sets the format of the data.
        /// </summary>
        public string Format { get; set; }
    }
}

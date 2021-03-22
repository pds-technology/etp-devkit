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

namespace Energistics.Etp.v12.Protocol.DiscoveryQuery
{
    /// <summary>
    /// Describes the interface that must be implemented by the store role of the DiscoveryQuery protocol.
    /// </summary>
    /// <seealso cref="IProtocolHandler" />
    [ProtocolRole((int)Protocols.DiscoveryQuery, Roles.Store, Roles.Customer)]
    public interface IDiscoveryQueryStore : IProtocolHandler<ICapabilitiesStore, ICapabilitiesCustomer>
    {
        /// <summary>
        /// Handles the FindResources event from a customer.
        /// </summary>
        event EventHandler<ListRequestWithContextEventArgs<FindResources, Resource, ResponseContext>> OnFindResources;

        /// <summary>
        /// Sends a FindResourcesResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="dataObjects">The list of <see cref="Resource"/> objects.</param>
        /// <param name="serverSortOrder">The server sort order.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<FindResourcesResponse> FindResourcesResponse(IMessageHeader correlatedHeader, IList<Resource> dataObjects, string serverSortOrder, bool isFinalPart = true, IMessageHeaderExtension extension = null);
    }

    /// <summary>
    /// Encapsulates the context of a discovery query response.
    /// </summary>
    public class ResponseContext
    {
        /// <summary>
        /// Gets or sets the server sort order.
        /// </summary>
        public string ServerSortOrder { get; set; }
    }
}

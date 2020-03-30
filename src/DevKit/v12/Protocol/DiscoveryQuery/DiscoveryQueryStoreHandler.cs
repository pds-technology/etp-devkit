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

using System.Collections.Generic;
using System.Linq;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v12.Datatypes;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.DiscoveryQuery
{
    /// <summary>
    /// Base implementation of the <see cref="IDiscoveryQueryStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.DiscoveryQuery.IDiscoveryQueryStore" />
    public class DiscoveryQueryStoreHandler : Etp12ProtocolHandler, IDiscoveryQueryStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscoveryQueryStoreHandler"/> class.
        /// </summary>
        public DiscoveryQueryStoreHandler() : base((int)Protocols.DiscoveryQuery, "store", "customer")
        {
            MaxResponseCount = EtpSettings.DefaultMaxResponseCount;

            RegisterMessageHandler<FindResources>(Protocols.DiscoveryQuery, MessageTypes.DiscoveryQuery.FindResources, HandleFindResources);
        }

        /// <summary>
        /// Gets the maximum response count.
        /// </summary>
        public int MaxResponseCount { get; set; }

        /// <summary>
        /// Gets the capabilities supported by the protocol handler.
        /// </summary>
        /// <returns>A collection of protocol capabilities.</returns>
        public override IDictionary<string, IDataValue> GetCapabilities()
        {
            var capabilities = base.GetCapabilities();

            capabilities[EtpSettings.MaxResponseCountKey] = new DataValue { Item = MaxResponseCount };

            return capabilities;
        }

        /// <summary>
        /// Sends a FindResourcesResponse message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="resources">The list of <see cref="Resource" /> objects.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns>The message identifier.</returns>
        public virtual long FindResourcesResponse(IMessageHeader request, IList<Resource> resources, string sortOrder)
        {

            var header = CreateMessageHeader(Protocols.DiscoveryQuery, MessageTypes.DiscoveryQuery.FindResourcesResponse, request.MessageId);
            var response = new FindResourcesResponse
            {
                ServerSortOrder = string.Empty,
            };

            return Session.Send12MultipartResponse(header, response, resources, (m, i) => m.Resources = i);
        }

        /// <summary>
        /// Handles the FindResources event from a customer.
        /// </summary>
        public event ProtocolEventHandler<FindResources, ResourceResponse> OnFindResources;

        /// <summary>
        /// Handles the FindResources message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The FindResources message.</param>
        protected virtual void HandleFindResources(IMessageHeader header, FindResources message)
        {
            var args = Notify(OnFindResources, header, message, new ResourceResponse());
            if (args.Cancel)
                return;

            HandleFindResources(header, message, args.Context);

            if (!args.Cancel)
            {
                FindResourcesResponse(header, args.Context.Resources, args.Context.ServerSortOrder);
            }
        }

        /// <summary>
        /// Handles the FindResources message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="response">The response.</param>
        protected virtual void HandleFindResources(IMessageHeader header, FindResources message, ResourceResponse response)
        {
        }
    }
}

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
            if (!resources.Any())
            {
                return Acknowledge(request.MessageId, MessageFlags.NoData);
            }

            long messageId = 0;

            for (var i=0; i<resources.Count; i++)
            {
                var messageFlags = i < resources.Count - 1
                    ? MessageFlags.MultiPart
                    : MessageFlags.MultiPartAndFinalPart;

                var header = CreateMessageHeader(Protocols.DiscoveryQuery, MessageTypes.DiscoveryQuery.FindResourcesResponse, request.MessageId, messageFlags);

                var response = new FindResourcesResponse
                {
                    Resources = new[] { resources[i] },
                    ServerSortOrder = sortOrder ?? string.Empty
                };

                messageId = Session.SendMessage(header, response);
                sortOrder = string.Empty; // Only needs to be set in the first message
            }

            return messageId;
        }

        /// <summary>
        /// Handles the FindResources event from a customer.
        /// </summary>
        public event ProtocolEventHandler<FindResources, ResourceResponse> OnFindResources;

        /// <summary>
        /// Handles the FindResources message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="findResources">The FindResources message.</param>
        protected virtual void HandleFindResources(IMessageHeader header, FindResources findResources)
        {
            var args = Notify(OnFindResources, header, findResources, new ResourceResponse());
            HandleFindResources(args);

            if (!args.Cancel)
            {
                FindResourcesResponse(header, args.Context.Resources, args.Context.ServerSortOrder);
            }
        }

        /// <summary>
        /// Handles the FindResources message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="ProtocolEventArgs{FindResources}"/> instance containing the event data.</param>
        protected virtual void HandleFindResources(ProtocolEventArgs<FindResources, ResourceResponse> args)
        {
        }
    }
}

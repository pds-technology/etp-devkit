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

using System.Collections.Generic;
using System.Linq;
using Avro.IO;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.DiscoveryQuery
{
    /// <summary>
    /// Base implementation of the <see cref="IDiscoveryQueryStore"/> interface.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.EtpProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.DiscoveryQuery.IDiscoveryQueryStore" />
    public class DiscoveryQueryStoreHandler : EtpProtocolHandler, IDiscoveryQueryStore
    {
        /// <summary>
        /// The MaxGetResourcesResponse protocol capability key.
        /// </summary>
        public const string MaxGetResourcesResponse = "MaxGetResourcesResponse";

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscoveryQueryStoreHandler"/> class.
        /// </summary>
        public DiscoveryQueryStoreHandler() : base((int)Protocols.DiscoveryQuery, "store", "customer")
        {
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

            for (int i=0; i<resources.Count; i++)
            {
                var messageFlags = i < resources.Count - 1
                    ? MessageFlags.MultiPart
                    : MessageFlags.FinalPart;

                var header = CreateMessageHeader(Protocols.DiscoveryQuery, MessageTypes.DiscoveryQuery.FindResourcesResponse, request.MessageId, messageFlags);

                var findResourcesResponse = new FindResourcesResponse()
                {
                    Resource = resources[i],
                    ServerSortOrder = sortOrder
                };

                messageId = Session.SendMessage(header, findResourcesResponse);
            }

            return messageId;
        }

        /// <summary>
        /// Handles the FindResources event from a customer.
        /// </summary>
        public event ProtocolEventHandler<FindResources, IList<Resource>> OnFindResources;

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
                case (int)MessageTypes.DiscoveryQuery.FindResources:
                    HandleFindResources(header, decoder.Decode<FindResources>(body));
                    break;

                default:
                    base.HandleMessage(header, decoder, body);
                    break;
            }
        }

        /// <summary>
        /// Handles the FindResources message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="findResources">The FindResources message.</param>
        protected virtual void HandleFindResources(IMessageHeader header, FindResources findResources)
        {
            var args = Notify(OnFindResources, header, findResources, new List<Resource>());
            HandleFindResources(args);

            if (!args.Cancel)
            {
                FindResourcesResponse(header, args.Context, string.Empty);
            }
        }

        /// <summary>
        /// Handles the FindResources message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="ProtocolEventArgs{FindResources}"/> instance containing the event data.</param>
        protected virtual void HandleFindResources(ProtocolEventArgs<FindResources, IList<Resource>> args)
        {
        }
    }
}

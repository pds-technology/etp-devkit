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
using Energistics.Etp.v11.Datatypes.Object;

namespace Energistics.Etp.v11.Protocol.Discovery
{
    /// <summary>
    /// Base implementation of the <see cref="IDiscoveryStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp11ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v11.Protocol.Discovery.IDiscoveryStore" />
    public class DiscoveryStoreHandler : Etp11ProtocolHandler, IDiscoveryStore
    {
        /// <summary>
        /// The MaxGetResourcesResponse protocol capability key.
        /// </summary>
        public const string MaxGetResourcesResponse = "MaxGetResourcesResponse";

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscoveryStoreHandler"/> class.
        /// </summary>
        public DiscoveryStoreHandler() : base((int)Protocols.Discovery, "store", "customer")
        {
            RegisterMessageHandler<GetResources>(Protocols.Discovery, MessageTypes.Discovery.GetResources, HandleGetResources);
        }

        /// <summary>
        /// Sends a GetResourcesResponse message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="resources">The list of <see cref="Resource" /> objects.</param>
        /// <returns>The message identifier.</returns>
        public virtual long GetResourcesResponse(IMessageHeader request, IList<Resource> resources)
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
                    : MessageFlags.MultiPartAndFinalPart;

                var header = CreateMessageHeader(Protocols.Discovery, MessageTypes.Discovery.GetResourcesResponse, request.MessageId, messageFlags);

                var getResourcesResponse = new GetResourcesResponse()
                {
                    Resource = resources[i]
                };

                messageId = Session.SendMessage(header, getResourcesResponse);
            }

            return messageId;
        }

        /// <summary>
        /// Handles the GetResources event from a customer.
        /// </summary>
        public event ProtocolEventHandler<GetResources, IList<Resource>> OnGetResources;

        /// <summary>
        /// Handles the GetResources message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="getResources">The GetResources message.</param>
        protected virtual void HandleGetResources(IMessageHeader header, GetResources getResources)
        {
            var args = Notify(OnGetResources, header, getResources, new List<Resource>());
            HandleGetResources(args);

            if (!args.Cancel)
            {
                GetResourcesResponse(header, args.Context);
            }
        }

        /// <summary>
        /// Handles the GetResources message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="ProtocolEventArgs{GetResources}"/> instance containing the event data.</param>
        protected virtual void HandleGetResources(ProtocolEventArgs<GetResources, IList<Resource>> args)
        {
        }
    }
}

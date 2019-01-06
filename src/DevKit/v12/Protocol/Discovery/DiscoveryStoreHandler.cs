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

namespace Energistics.Etp.v12.Protocol.Discovery
{
    /// <summary>
    /// Base implementation of the <see cref="IDiscoveryStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.Discovery.IDiscoveryStore" />
    public class DiscoveryStoreHandler : Etp12ProtocolHandler, IDiscoveryStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscoveryStoreHandler"/> class.
        /// </summary>
        public DiscoveryStoreHandler() : base((int)Protocols.Discovery, "store", "customer")
        {
            MaxResponseCount = EtpSettings.DefaultMaxResponseCount;

            RegisterMessageHandler<GetTreeResources>(Protocols.Discovery, MessageTypes.Discovery.GetTreeResources, HandleGetTreeResources);
            RegisterMessageHandler<GetGraphResources>(Protocols.Discovery, MessageTypes.Discovery.GetGraphResources, HandleGetGraphResources);
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

            for (var i=0; i<resources.Count; i++)
            {
                var messageFlags = i < resources.Count - 1
                    ? MessageFlags.MultiPart
                    : MessageFlags.MultiPartAndFinalPart;

                var header = CreateMessageHeader(Protocols.Discovery, MessageTypes.Discovery.GetResourcesResponse, request.MessageId, messageFlags);

                // TODO: Optimize reponse by sending multiple Resource at a time
                var response = new GetResourcesResponse
                {
                    Resources = new[] { resources[i] }
                };

                messageId = Session.SendMessage(header, response);
            }

            return messageId;
        }

        /// <summary>
        /// Handles the GetTreeResources event from a customer.
        /// </summary>
        public event ProtocolEventHandler<GetTreeResources, IList<Resource>> OnGetTreeResources;

        /// <summary>
        /// Handles the GetGraphResources event from a customer.
        /// </summary>
        public event ProtocolEventHandler<GetGraphResources, IList<Resource>> OnGetGraphResources;

        /// <summary>
        /// Handles the GetTreeResources message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="getTreeResources">The GetTreeResources message.</param>
        protected virtual void HandleGetTreeResources(IMessageHeader header, GetTreeResources getTreeResources)
        {
            var args = Notify(OnGetTreeResources, header, getTreeResources, new List<Resource>());
            HandleGetTreeResources(args);

            if (!args.Cancel)
            {
                GetResourcesResponse(header, args.Context);
            }
        }

        /// <summary>
        /// Handles the GetTreeResources message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="ProtocolEventArgs{GetTreeResources}"/> instance containing the event data.</param>
        protected virtual void HandleGetTreeResources(ProtocolEventArgs<GetTreeResources, IList<Resource>> args)
        {
        }

        /// <summary>
        /// Handles the GetGraphResources message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="getGraphResources">The GetGraphResources message.</param>
        protected virtual void HandleGetGraphResources(IMessageHeader header, GetGraphResources getGraphResources)
        {
            var args = Notify(OnGetGraphResources, header, getGraphResources, new List<Resource>());
            HandleGetGraphResources(args);

            if (!args.Cancel)
            {
                GetResourcesResponse(header, args.Context);
            }
        }

        /// <summary>
        /// Handles the GetGraphResources message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="ProtocolEventArgs{GetGraphResources}"/> instance containing the event data.</param>
        protected virtual void HandleGetGraphResources(ProtocolEventArgs<GetGraphResources, IList<Resource>> args)
        {
        }
    }
}

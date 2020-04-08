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

            RegisterMessageHandler<GetResources>(Protocols.Discovery, MessageTypes.Discovery.GetResources, HandleGetResources);
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
        /// Handles the GetResources event from a customer.
        /// </summary>
        public event ProtocolEventHandler<GetResources, IList<Resource>> OnGetResources;

        /// <summary>
        /// Sends a GetResourcesResponse message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="resources">The list of <see cref="Resource" /> objects.</param>
        /// <returns>The message identifier.</returns>
        public virtual long GetResourcesResponse(IMessageHeader request, IList<Resource> resources)
        {
            var header = CreateMessageHeader(Protocols.Discovery, MessageTypes.Discovery.GetResourcesResponse, request.MessageId);
            var response = new GetResourcesResponse();

            return SendMultipartResponse(header, response, resources, (m, i) => m.Resources = i);
        }

        /// <summary>
        /// Handles the GetResources message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GetResources message.</param>
        protected virtual void HandleGetResources(IMessageHeader header, GetResources message)
        {
            var args = Notify(OnGetResources, header, message, new List<Resource>());
            if (args.Cancel)
                return;

            if (!HandleGetResources(header, message, args.Context))
                return;

            GetResourcesResponse(header, args.Context);
        }

        /// <summary>
        /// Handles the GetResources message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="response">The response.</param>
        protected virtual bool HandleGetResources(IMessageHeader header, GetResources message, IList<Resource> response)
        {
            return true;
        }
    }
}

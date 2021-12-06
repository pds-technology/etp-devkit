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
using Energistics.Etp.Common;
using Energistics.Etp.Common.Protocol.Core;

namespace Energistics.Etp.v11.Protocol.Discovery
{
    /// <summary>
    /// Base implementation of the <see cref="IDiscoveryCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp11ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v11.Protocol.Discovery.IDiscoveryCustomer" />
    public class DiscoveryCustomerHandler : Etp11ProtocolHandlerWithCounterpartCapabilities<CapabilitiesStore, ICapabilitiesStore>, IDiscoveryCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscoveryCustomerHandler"/> class.
        /// </summary>
        public DiscoveryCustomerHandler() : base((int)Protocols.Discovery, Roles.Customer, Roles.Store)
        {
            RegisterMessageHandler<GetResourcesResponse>(Protocols.Discovery, MessageTypes.Discovery.GetResourcesResponse, HandleGetResourcesResponse);
        }

        /// <summary>
        /// Sends a GetResources message to a store.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetResources> GetResources(string uri)
        {
            var body = new GetResources()
            {
                Uri = uri ?? string.Empty,
            };

            return SendRequest(body);
        }

        /// <summary>
        /// Handles the GetResourcesResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<GetResources, GetResourcesResponse>> OnGetResourcesResponse;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage<GetResources>(message);
            if (request != null)
                HandleResponseMessage(request, message, OnGetResourcesResponse, HandleGetResourcesResponse);
        }

        /// <summary>
        /// Handles the GetResourcesResponse message from a store.
        /// </summary>
        /// <param name="message">The GetResourcesResponse message.</param>
        protected virtual void HandleGetResourcesResponse(EtpMessage<GetResourcesResponse> message)
        {
            var request = TryGetCorrelatedMessage<GetResources>(message);
            HandleResponseMessage(request, message, OnGetResourcesResponse, HandleGetResourcesResponse);
        }

        /// <summary>
        /// Handles the GetResourcesResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{GetResources, GetResourcesResponse}"/> instance containing the event data.</param>
        protected virtual void HandleGetResourcesResponse(ResponseEventArgs<GetResources, GetResourcesResponse> args)
        {
        }
    }
}

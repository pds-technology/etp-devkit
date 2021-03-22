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
using System.Linq;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Datatypes.Object;
using Energistics.Etp.v11.Datatypes.Object;

namespace Energistics.Etp.v11.Protocol.Discovery
{
    /// <summary>
    /// Base implementation of the <see cref="IDiscoveryStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp11ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v11.Protocol.Discovery.IDiscoveryStore" />
    public class DiscoveryStoreHandler : Etp11ProtocolHandlerWithCapabilities<CapabilitiesStore, ICapabilitiesStore>, IDiscoveryStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscoveryStoreHandler"/> class.
        /// </summary>
        public DiscoveryStoreHandler() : base((int)Protocols.Discovery, Roles.Store, Roles.Customer)
        {
            RegisterMessageHandler<GetResources>(Protocols.Discovery, MessageTypes.Discovery.GetResources, HandleGetResources);
        }

        /// <summary>
        /// Handles the GetResources event from a customer.
        /// </summary>
        public event EventHandler<ListRequestEventArgs<GetResources, Resource>> OnGetResources;

        /// <summary>
        /// Sends a GetResourcesResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="resource">The list of <see cref="Resource" /> objects.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetResourcesResponse> GetResourcesResponse(IMessageHeader correlatedHeader, Resource resource, bool isFinalPart = true)
        {
            var body = new GetResourcesResponse()
            {
                Resource = resource,
            };

            return SendResponse(body, correlatedHeader, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a complete multi-part set of GetResourcesResponse messages to a customer for the list of <see cref="Resource"/> objects.
        /// If there are no resources in the list, an Acknowledge message is sent with the NoData flag sent.
        /// If there are resources in the list and acknowledge is requested, an Acknowledge message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="resources">The list of <see cref="Resource" /> objects.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.  If there are no resources in the list, a placeholder message with a header matching the sent Acknowledge is returned.</returns>
        public virtual EtpMessage<GetResourcesResponse> GetResourcesResponses(IMessageHeader correlatedHeader, IList<Resource> resources, bool setFinalPart = true)
        {
            if (resources == null || resources.Count == 0)
            {
                var ack = Acknowledge(correlatedHeader, true);
                if (ack == null)
                    return null;

                var header = CreateMessageHeader<GetResourcesResponse>();
                header.MessageFlags = ack.Header.MessageFlags;
                header.MessageId = ack.Header.MessageId;
                header.CorrelationId = ack.Header.CorrelationId;
                header.Timestamp = ack.Header.Timestamp;
                return new EtpMessage<GetResourcesResponse>(header, new GetResourcesResponse());
            }
            else if (correlatedHeader.IsAcknowledgeRequested())
            {
                Acknowledge(correlatedHeader);
            }

            EtpMessage<GetResourcesResponse> message = null;

            for (int i = 0; i < resources?.Count; i++)
            {
                var ret = GetResourcesResponse(correlatedHeader, resources[i], isFinalPart: (i == resources.Count - 1 && setFinalPart));
                if (ret == null)
                    return null;
                message = message ?? ret;
            }

            return message;
        }

        /// <summary>
        /// Handles the GetResources message from a customer.
        /// </summary>
        /// <param name="message">The GetResources message.</param>
        protected virtual void HandleGetResources(EtpMessage<GetResources> message)
        {
            HandleRequestMessage(message, OnGetResources, HandleGetResources,
                responseMethod: (args) => GetResourcesResponses(args.Request?.Header, args.Responses, setFinalPart: !args.HasErrors));
        }

        /// <summary>
        /// Handles the GetResources message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="ListRequestEventArgs{GetResources, Resource}"/> instance containing the event data.</param>
        protected virtual void HandleGetResources(ListRequestEventArgs<GetResources, Resource> args)
        {
        }
    }
}

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

using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v11.Datatypes.Object;
using System;
using System.Collections.Generic;

namespace Energistics.Etp.v11.Protocol.Discovery
{
    /// <summary>
    /// Describes the interface that must be implemented by the store role of the Discovery protocol.
    /// </summary>
    /// <seealso cref="IProtocolHandler" />
    [ProtocolRole((int)Protocols.Discovery, Roles.Store, Roles.Customer)]
    public interface IDiscoveryStore : IProtocolHandlerWithCapabilities<ICapabilitiesStore>
    {
        /// <summary>
        /// Handles the GetResources event from a customer.
        /// </summary>
        event EventHandler<ListRequestEventArgs<GetResources, Resource>> OnGetResources;

        /// <summary>
        /// Sends a GetResourcesResponse message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="resource">The <see cref="Resource"/> object.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetResourcesResponse> GetResourcesResponse(IMessageHeader request, Resource resource, bool isFinalPart = true);

        /// <summary>
        /// Sends a complete multi-part set of GetResourcesResponse messages to a customer for the list of <see cref="Resource"/> objects.
        /// If there are no resources in the list, an Acknowledge message is sent with the NoData flag sent.
        /// If there are resources in the list and acknowledge is requested, an Acknowledge message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="resources">The list of <see cref="Resource" /> objects.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last GetResourcesResponse message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.  If there are no resources in the list, a placeholder message with a header matching the sent Acknowledge is returned.</returns>
        EtpMessage<GetResourcesResponse> GetResourcesResponses(IMessageHeader correlatedHeader, IList<Resource> resources, bool setFinalPart = true);
    }
}

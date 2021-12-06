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

namespace Energistics.Etp.v12.Protocol.Discovery
{
    /// <summary>
    /// Describes the interface that must be implemented by the store role of the Discovery protocol.
    /// </summary>
    /// <seealso cref="IProtocolHandler" />
    [ProtocolRole((int)Protocols.Discovery, Roles.Store, Roles.Customer)]
    public interface IDiscoveryStore : IProtocolHandler<ICapabilitiesStore, ICapabilitiesCustomer>
    {
        /// <summary>
        /// Handles the GetResources event from a customer.
        /// </summary>
        event EventHandler<DualListRequestEventArgs<GetResources, Resource, Edge>> OnGetResources;

        /// <summary>
        /// Sends a GetResourcesResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="resources">The list of <see cref="Resource"/> objects.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetResourcesResponse> GetResourcesResponse(IMessageHeader correlatedHeader, IList<Resource> resources, bool isFinalPart = true, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a GetResourcesEdgeResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="edges">The list of <see cref="Edge"/> objects.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetResourcesEdgesResponse> GetResourcesEdgesResponse(IMessageHeader correlatedHeader, IList<Edge> edges, bool isFinalPart = true, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Sends a complete multi-part set of GetResourcesResponse and GetResourcesEdgesResponse messagess to a customer.
        /// If there are no resources, an empty GetResourcesResponse message is sent.
        /// If there are no edges, no GetResourcesEdgesResponse message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="resources">The list of <see cref="Resource"/> objects.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <param name="resourcesExtension">The message header extension for the GetResourcesResponse message.</param>
        /// <param name="edgesExtension">The message header extension for the GetResourcesEdgesResponse message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetResourcesResponse> GetResourcesResponse(IMessageHeader correlatedHeader, IList<Resource> resources, IList<Edge> edges, bool setFinalPart = true, IMessageHeaderExtension resourcesExtension = null, IMessageHeaderExtension edgesExtension = null);

        /// <summary>
        /// Handles the GetDeletedResources event from a customer.
        /// </summary>
        event EventHandler<ListRequestEventArgs<GetDeletedResources, DeletedResource>> OnGetDeletedResources;

        /// <summary>
        /// Sends a GetDeletedResourcesResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="deletedResources">The list of <see cref="DeletedResource"/> objects.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetDeletedResourcesResponse> GetDeletedResourcesResponse(IMessageHeader correlatedHeader, IList<DeletedResource> deletedResources, bool isFinalPart = true, IMessageHeaderExtension extension = null);
    }
}

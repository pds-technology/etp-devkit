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
using Energistics.Etp.v12.Datatypes.Object;
using System;
using System.Collections.Generic;

namespace Energistics.Etp.v12.Protocol.Discovery
{
    /// <summary>
    /// Describes the interface that must be implemented by the customer role of the Discovery protocol.
    /// </summary>
    /// <seealso cref="IProtocolHandler" />
    [ProtocolRole((int)Protocols.Discovery, Roles.Customer, Roles.Store)]
    public interface IDiscoveryCustomer : IProtocolHandler<ICapabilitiesCustomer, ICapabilitiesStore>
    {
        /// <summary>
        /// Sends a GetResources message to a store.
        /// </summary>
        /// <param name="context">The context information.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="storeLastWriteFilter">An optional parameter to filter discovery on a date when an object last changed.</param>
        /// <param name="activeStatusFilter">if not <c>null</c>, request only objects with a matching active status.</param>
        /// <param name="includeEdges">if set to <c>true</c>, request edges.</param>
        /// <param name="countObjects">if set to <c>true</c>, request object counts.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetResources> GetResources(ContextInfo context, ContextScopeKind scope, long? storeLastWriteFilter = null, ActiveStatusKind? activeStatusFilter = null, bool includeEdges = false, bool countObjects = false, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the GetResourcesResponse and GetResourcesEdgesResponse events from a store.
        /// </summary>
        event EventHandler<DualResponseEventArgs<GetResources, GetResourcesResponse, GetResourcesEdgesResponse>> OnGetResourcesResponse;

        /// <summary>
        /// Sends a GetDeletedResources message to a store.
        /// </summary>
        /// <param name="dataspaceUri">The dataspace URI.</param>
        /// <param name="deleteTimeFilter">An optional parameter to filter discovery on a date when an object was deleted.</param>
        /// <param name="dataObjectTypes">if not <c>null</c> or empty, requests only deleted resources for objects of types found in the list.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetDeletedResources> GetDeletedResources(string dataspaceUri, long? deleteTimeFilter = null, IList<string> dataObjectTypes = null, IMessageHeaderExtension extension = null);

        /// <summary>
        /// Handles the GetDeletedResourcesResponse event from a store.
        /// </summary>
        event EventHandler<ResponseEventArgs<GetDeletedResources, GetDeletedResourcesResponse>> OnGetDeletedResourcesResponse;
    }
}

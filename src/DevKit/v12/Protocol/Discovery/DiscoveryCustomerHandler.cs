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
using System.Collections.Concurrent;
using System.Collections.Generic;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Protocol.Core;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.Discovery
{
    /// <summary>
    /// Base implementation of the <see cref="IDiscoveryCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.Discovery.IDiscoveryCustomer" />
    public class DiscoveryCustomerHandler : Etp12ProtocolHandler<CapabilitiesCustomer, ICapabilitiesCustomer, CapabilitiesStore, ICapabilitiesStore>, IDiscoveryCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscoveryCustomerHandler"/> class.
        /// </summary>
        public DiscoveryCustomerHandler() : base((int)Protocols.Discovery, Roles.Customer, Roles.Store)
        {
            RegisterMessageHandler<GetResourcesResponse>(Protocols.Discovery, MessageTypes.Discovery.GetResourcesResponse, HandleGetResourcesResponse);
            RegisterMessageHandler<GetResourcesEdgesResponse>(Protocols.Discovery, MessageTypes.Discovery.GetResourcesEdgesResponse, HandleGetResourcesEdgesResponse);
            RegisterMessageHandler<GetDeletedResourcesResponse>(Protocols.Discovery, MessageTypes.Discovery.GetDeletedResourcesResponse, HandleGetDeletedResourcesResponse);
        }

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
        public virtual EtpMessage<GetResources> GetResources(ContextInfo context, ContextScopeKind scope, DateTime? storeLastWriteFilter = null, ActiveStatusKind? activeStatusFilter = null, bool includeEdges = false, bool countObjects = false, IMessageHeaderExtension extension = null)
        {
            var body = new GetResources
            {
                Context = context,
                Scope = scope,
                StoreLastWriteFilter = storeLastWriteFilter,
                ActiveStatusFilter = activeStatusFilter,
                IncludeEdges = includeEdges,
                CountObjects = countObjects,
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Handles the GetResourcesResponse and GetResourcesEdgesResponse events from a store.
        /// </summary>
        public event EventHandler<DualResponseEventArgs<GetResources, GetResourcesResponse, GetResourcesEdgesResponse>> OnGetResourcesResponse;

        /// <summary>
        /// Sends a GetDeletedResources message to a store.
        /// </summary>
        /// <param name="dataspaceUri">The dataspace URI.</param>
        /// <param name="deleteTimeFilter">An optional parameter to filter discovery on a date when an object was deleted.</param>
        /// <param name="dataObjectTypes">if not <c>null</c> or empty, requests only deleted resources for objects of types found in the list.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetDeletedResources> GetDeletedResources(string dataspaceUri, DateTime? deleteTimeFilter = null, IList<string> dataObjectTypes = null, IMessageHeaderExtension extension = null)
        {
            var body = new GetDeletedResources
            {
                DataspaceUri = dataspaceUri ?? string.Empty,
                DeleteTimeFilter = deleteTimeFilter,
                DataObjectTypes = dataObjectTypes ?? new List<string>(),
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Handles the GetDeletedResourcesResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<GetDeletedResources, GetDeletedResourcesResponse>> OnGetDeletedResourcesResponse;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<GetResources>)
                HandleResponseMessage(request as EtpMessage<GetResources>, message, OnGetResourcesResponse, HandleGetResourcesResponse);
            else if (request is EtpMessage<GetDeletedResources>)
                HandleResponseMessage(request as EtpMessage<GetDeletedResources>, message, OnGetDeletedResourcesResponse, HandleGetDeletedResourcesResponse);
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
        /// Handles the GetResourcesEdgesResponse message from a store.
        /// </summary>
        /// <param name="message">The GetResourcesEdgesResponse message.</param>
        protected virtual void HandleGetResourcesEdgesResponse(EtpMessage<GetResourcesEdgesResponse> message)
        {
            var request = TryGetCorrelatedMessage<GetResources>(message);
            HandleResponseMessage(request, message, OnGetResourcesResponse, HandleGetResourcesResponse);
        }

        /// <summary>
        /// Handles the GetResourcesResponse and GetResourcesEdgesResponse messages from a store.
        /// </summary>
        /// <param name="args">The <see cref="DualResponseEventArgs{GetResources, GetResourcesResponse, GetResourcesEdgesResponse}"/> instance containing the event data.</param>
        protected virtual void HandleGetResourcesResponse(DualResponseEventArgs<GetResources, GetResourcesResponse, GetResourcesEdgesResponse> args)
        {
        }

        /// <summary>
        /// Handles the GetDeletedResourcesResponse message from a store.
        /// </summary>
        /// <param name="message">The GetDeletedResourcesResponse message.</param>
        protected virtual void HandleGetDeletedResourcesResponse(EtpMessage<GetDeletedResourcesResponse> message)
        {
            var request = TryGetCorrelatedMessage<GetDeletedResources>(message);
            HandleResponseMessage(request, message, OnGetDeletedResourcesResponse, HandleGetDeletedResourcesResponse);
        }

        /// <summary>
        /// Handles the GetDeletedResourcesResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{GetDeletedResources, GetDeletedResourcesResponse}"/> instance containing the event data.</param>
        protected virtual void HandleGetDeletedResourcesResponse(ResponseEventArgs<GetDeletedResources, GetDeletedResourcesResponse> args)
        {
        }
    }
}

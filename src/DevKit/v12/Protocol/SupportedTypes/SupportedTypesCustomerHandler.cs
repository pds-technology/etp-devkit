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
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Protocol.Core;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.SupportedTypes
{
    /// <summary>
    /// Base implementation of the <see cref="ISupportedTypesCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.SupportedTypes.ISupportedTypesCustomer" />
    public class SupportedTypesCustomerHandler : Etp12ProtocolHandler<CapabilitiesCustomer, ICapabilitiesCustomer, CapabilitiesStore, ICapabilitiesStore>, ISupportedTypesCustomer
    {
        private readonly ConcurrentDictionary<long, GetSupportedTypes> _requests = new ConcurrentDictionary<long, GetSupportedTypes>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedTypesCustomerHandler"/> class.
        /// </summary>
        public SupportedTypesCustomerHandler() : base((int)Protocols.SupportedTypes, Roles.Customer, Roles.Store)
        {
            RegisterMessageHandler<GetSupportedTypesResponse>(Protocols.SupportedTypes, MessageTypes.SupportedTypes.GetSupportedTypesResponse, HandleGetSupportedTypesResponse);
        }

        /// <summary>
        /// Sends a GetSupportedTypes message to a store.
        /// </summary>
        /// <param name="uri">The uri to to discover instantiated or supported data types from.</param>
        /// <param name="scope">The scope to return supported types for.</param>
        /// <param name="returnEmptyTypes">Whether the store should return data types that it supports but for which it currently has no data.</param>
        /// <param name="countObjects">if set to <c>true</c>, request object counts.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetSupportedTypes> GetSupportedTypes(string uri, ContextScopeKind scope, bool returnEmptyTypes = false, bool countObjects = false, IMessageHeaderExtension extension = null)
        {
            var body = new GetSupportedTypes
            {
                Uri = uri ?? string.Empty,
                Scope = scope,
                ReturnEmptyTypes = returnEmptyTypes,
                CountObjects = countObjects,
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Handles the GetSupportedTypesResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<GetSupportedTypes, GetSupportedTypesResponse>> OnGetSupportedTypesResponse;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<GetSupportedTypes>)
                HandleResponseMessage(request as EtpMessage<GetSupportedTypes>, message, OnGetSupportedTypesResponse, HandleGetSupportedTypesResponse);
        }

        /// <summary>
        /// Handles the GetSupportedTypesResponse message from a store.
        /// </summary>
        /// <param name="message">The GetSupportedTypesResponse message.</param>
        protected virtual void HandleGetSupportedTypesResponse(EtpMessage<GetSupportedTypesResponse> message)
        {
            var request = TryGetCorrelatedMessage<GetSupportedTypes>(message);
            HandleResponseMessage(request, message, OnGetSupportedTypesResponse, HandleGetSupportedTypesResponse);
        }

        /// <summary>
        /// Handles the GetSupportedTypesResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{GetSupportedTypes, GetSupportedTypesResponse}"/> instance containing the event data.</param>
        protected virtual void HandleGetSupportedTypesResponse(ResponseEventArgs<GetSupportedTypes, GetSupportedTypesResponse> args)
        {
        }
    }
}

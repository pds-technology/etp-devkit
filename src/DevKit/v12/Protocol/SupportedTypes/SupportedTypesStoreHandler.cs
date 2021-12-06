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

namespace Energistics.Etp.v12.Protocol.SupportedTypes
{
    /// <summary>
    /// Base implementation of the <see cref="ISupportedTypesStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.SupportedTypes.ISupportedTypesStore" />
    public class SupportedTypesStoreHandler : Etp12ProtocolHandler<CapabilitiesStore, ICapabilitiesStore, CapabilitiesCustomer, ICapabilitiesCustomer>, ISupportedTypesStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedTypesStoreHandler"/> class.
        /// </summary>
        public SupportedTypesStoreHandler() : base((int)Protocols.SupportedTypes, Roles.Store, Roles.Customer)
        {
            RegisterMessageHandler<GetSupportedTypes>(Protocols.SupportedTypes, MessageTypes.SupportedTypes.GetSupportedTypes, HandleGetSupportedTypes);
        }

        /// <summary>
        /// Handles the GetSupportedTypes event from a customer.
        /// </summary>
        public event EventHandler<ListRequestEventArgs<GetSupportedTypes, SupportedType>> OnGetSupportedTypes;

        /// <summary>
        /// Sends a GetSupportedTypesResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="supportedTypes">The list of <see cref="SupportedType" /> objects.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<GetSupportedTypesResponse> GetSupportedTypesResponse(IMessageHeader correlatedHeader, IList<SupportedType> supportedTypes, bool isFinalPart = true, IMessageHeaderExtension extension = null)
        {
            var body = new GetSupportedTypesResponse
            {
                SupportedTypes = supportedTypes ?? new List<SupportedType>(),
            };

            return SendResponse(body, correlatedHeader, extension: extension, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Handles the GetSupportedTypes message from a customer.
        /// </summary>
        /// <param name="message">The GetSupportedTypes message.</param>
        protected virtual void HandleGetSupportedTypes(EtpMessage<GetSupportedTypes> message)
        {
            HandleRequestMessage(message, OnGetSupportedTypes, HandleGetSupportedTypes,
                responseMethod: (args) => GetSupportedTypesResponse(args.Request?.Header, args.Responses, isFinalPart: !args.HasErrors, extension: args.ResponseExtension));
        }

        /// <summary>
        /// Handles the GetSupportedTypes message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="ListRequestEventArgs{GetSupportedTypes, SupportedType}"/> instance containing the event data.</param>
        protected virtual void HandleGetSupportedTypes(ListRequestEventArgs<GetSupportedTypes, SupportedType> args)
        {
        }
    }
}

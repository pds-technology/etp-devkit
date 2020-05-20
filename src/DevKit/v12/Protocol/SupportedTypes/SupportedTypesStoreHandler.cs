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
    public class SupportedTypesStoreHandler : Etp12ProtocolHandler, ISupportedTypesStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedTypesStoreHandler"/> class.
        /// </summary>
        public SupportedTypesStoreHandler() : base((int)Protocols.SupportedTypes, "store", "customer")
        {
            RegisterMessageHandler<GetSupportedTypes>(Protocols.SupportedTypes, MessageTypes.SupportedTypes.GetSupportedTypes, HandleGetSupportedTypes);
        }

        /// <summary>
        /// Handles the GetSupportedTypes event from a customer.
        /// </summary>
        public event ProtocolEventHandler<GetSupportedTypes, IList<SupportedType>> OnGetSupportedTypes;

        /// <summary>
        /// Sends a GetSupportedTypesResponse message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="supportedTypes">The list of <see cref="SupportedType" /> objects.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long GetSupportedTypesResponse(IMessageHeader request, IList<SupportedType> supportedTypes)
        {
            var header = CreateMessageHeader(Protocols.SupportedTypes, MessageTypes.SupportedTypes.GetSupportedTypesResponse, request.MessageId);
            var response = new GetSupportedTypesResponse();

            return SendMultipartResponse(header, response, supportedTypes, (m, i) => m.SupportedTypes = i);
        }

        /// <summary>
        /// Handles the GetSupportedTypes message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GetSupportedTypes message.</param>
        protected virtual void HandleGetSupportedTypes(IMessageHeader header, GetSupportedTypes message)
        {
            var args = Notify(OnGetSupportedTypes, header, message, new List<SupportedType>());
            if (args.Cancel)
                return;

            if (!HandleGetSupportedTypes(header, message, args.Context))
                return;

            GetSupportedTypesResponse(header, args.Context);
        }

        /// <summary>
        /// Handles the GetSupportedTypes message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="response">The response.</param>
        protected virtual bool HandleGetSupportedTypes(IMessageHeader header, GetSupportedTypes message, IList<SupportedType> response)
        {
            return true;
        }
    }
}

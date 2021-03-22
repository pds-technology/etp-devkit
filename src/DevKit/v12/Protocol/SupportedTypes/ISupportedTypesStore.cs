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
    /// Describes the interface that must be implemented by the store role of the SupportedTypes protocol.
    /// </summary>
    /// <seealso cref="IProtocolHandler" />
    [ProtocolRole((int)Protocols.SupportedTypes, Roles.Store, Roles.Customer)]
    public interface ISupportedTypesStore : IProtocolHandler<ICapabilitiesStore, ICapabilitiesCustomer>
    {
        /// <summary>
        /// Handles the GetSupportedTypes event from a customer.
        /// </summary>
        event EventHandler<ListRequestEventArgs<GetSupportedTypes, SupportedType>> OnGetSupportedTypes;

        /// <summary>
        /// Sends a GetSupportedTypesResponse message to a customer.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="supportedTypes">The list of <see cref="SupportedType"/> objects.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<GetSupportedTypesResponse> GetSupportedTypesResponse(IMessageHeader correlatedHeader, IList<SupportedType> supportedTypes, bool isFinalPart = true, IMessageHeaderExtension extension = null);
    }
}

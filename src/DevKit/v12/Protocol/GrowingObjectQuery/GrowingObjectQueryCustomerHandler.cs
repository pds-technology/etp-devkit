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
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.Common.Protocol.Core;

namespace Energistics.Etp.v12.Protocol.GrowingObjectQuery
{
    /// <summary>
    /// Base implementation of the <see cref="IGrowingObjectQueryCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.GrowingObjectQuery.IGrowingObjectQueryCustomer" />
    public class GrowingObjectQueryCustomerHandler : Etp12ProtocolHandler<CapabilitiesCustomer, ICapabilitiesCustomer, CapabilitiesStore, ICapabilitiesStore>, IGrowingObjectQueryCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrowingObjectQueryCustomerHandler"/> class.
        /// </summary>
        public GrowingObjectQueryCustomerHandler() : base((int)Protocols.GrowingObjectQuery, Roles.Customer, Roles.Store)
        {
            RegisterMessageHandler<FindPartsResponse>(Protocols.GrowingObjectQuery, MessageTypes.GrowingObjectQuery.FindPartsResponse, HandleFindPartsResponse);
        }

        /// <summary>
        /// Sends a FindParts message to a store.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <param name="extension">The message header extension.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public virtual EtpMessage<FindParts> FindParts(string uri, string format = Formats.Xml, IMessageHeaderExtension extension = null)
        {
            var body = new FindParts
            {
                Uri = uri ?? string.Empty,
                Format = format ?? Formats.Xml,
            };

            return SendRequest(body, extension: extension);
        }

        /// <summary>
        /// Handles the FindPartsResponse event from a store.
        /// </summary>
        public event EventHandler<ResponseEventArgs<FindParts, FindPartsResponse>> OnFindPartsResponse;

        /// <summary>
        /// Handles the ProtocolException message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void HandleProtocolException(EtpMessage<IProtocolException> message)
        {
            base.HandleProtocolException(message);

            var request = TryGetCorrelatedMessage(message);
            if (request is EtpMessage<FindParts>)
                HandleResponseMessage(request as EtpMessage<FindParts>, message, OnFindPartsResponse, HandleFindPartsResponse);
        }

        /// <summary>
        /// Handles the FindPartsResponse message from a store.
        /// </summary>
        /// <param name="message">The FindPartsResponse message.</param>
        protected virtual void HandleFindPartsResponse(EtpMessage<FindPartsResponse> message)
        {
            var request = TryGetCorrelatedMessage<FindParts>(message);
            HandleResponseMessage(request, message, OnFindPartsResponse, HandleFindPartsResponse);
        }

        /// <summary>
        /// Handles the FindPartsResponse message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ResponseEventArgs{FindParts, FindPartsResponse}"/> instance containing the event data.</param>
        protected virtual void HandleFindPartsResponse(ResponseEventArgs<FindParts, FindPartsResponse> args)
        {
        }
    }
}

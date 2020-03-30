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
using Energistics.Etp.v12.Datatypes;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.GrowingObjectQuery
{
    /// <summary>
    /// Base implementation of the <see cref="IGrowingObjectQueryStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.GrowingObjectQuery.IGrowingObjectQueryStore" />
    public class GrowingObjectQueryStoreHandler : Etp12ProtocolHandler, IGrowingObjectQueryStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrowingObjectQueryStoreHandler"/> class.
        /// </summary>
        public GrowingObjectQueryStoreHandler() : base((int)Protocols.GrowingObjectQuery, "store", "customer")
        {
            MaxResponseCount = EtpSettings.DefaultMaxResponseCount;

            RegisterMessageHandler<FindParts>(Protocols.GrowingObjectQuery, MessageTypes.GrowingObjectQuery.FindParts, HandleFindParts);
        }

        /// <summary>
        /// Gets the maximum response count.
        /// </summary>
        public int MaxResponseCount { get; set; }

        /// <summary>
        /// Gets the capabilities supported by the protocol handler.
        /// </summary>
        /// <returns>A collection of protocol capabilities.</returns>
        public override IDictionary<string, IDataValue> GetCapabilities()
        {
            var capabilities = base.GetCapabilities();

            capabilities[EtpSettings.MaxResponseCountKey] = new DataValue { Item = MaxResponseCount };

            return capabilities;
        }

        /// <summary>
        /// Sends a FindPartsResponse message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="uri">The URI from the request.</param>
        /// <param name="parts">The list of <see cref="ObjectPart"/> objects.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <returns>The message identifier.</returns>
        public virtual long FindPartsResponse(IMessageHeader request, string uri, IList<ObjectPart> parts, string sortOrder, string format = "xml")
        {
            var header = CreateMessageHeader(Protocols.DiscoveryQuery, MessageTypes.GrowingObjectQuery.FindPartsResponse, request.MessageId);
            var response = new FindPartsResponse
            {
                Uri = uri,
                ServerSortOrder = string.Empty,
                Format = format ?? "xml",
            };

            return Session.Send12MultipartResponse(header, response, parts, (m, i) => m.Parts = i);
        }

        /// <summary>
        /// Handles the FindParts event from a customer.
        /// </summary>
        public event ProtocolEventHandler<FindParts, PartsResponse> OnFindParts;

        /// <summary>
        /// Handles the FindParts message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The FindParts message.</param>
        protected virtual void HandleFindParts(IMessageHeader header, FindParts message)
        {
            var args = Notify(OnFindParts, header, message, new PartsResponse());
            if (args.Cancel)
                return;

            if (!HandleFindParts(header, message, args.Context))
                return;

            FindPartsResponse(header, message.Uri, args.Context.Parts, args.Context.ServerSortOrder, args.Context.Format);
        }

        /// <summary>
        /// Handles the FindParts message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="response">The response.</param>
        protected virtual bool HandleFindParts(IMessageHeader header, FindParts message, PartsResponse response)
        {
            return true;
        }
    }
}

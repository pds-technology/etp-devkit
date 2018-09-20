//----------------------------------------------------------------------- 
// ETP DevKit, 1.2
//
// Copyright 2018 Energistics
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
using System.Linq;
using Avro.IO;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v12.Protocol.GrowingObject;

namespace Energistics.Etp.v12.Protocol.GrowingObjectQuery
{
    /// <summary>
    /// Base implementation of the <see cref="IGrowingObjectQueryStore"/> interface.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.EtpProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.GrowingObjectQuery.IGrowingObjectQueryStore" />
    public class GrowingObjectQueryStoreHandler : EtpProtocolHandler, IGrowingObjectQueryStore
    {
        /// <summary>
        /// The MaxResponseCount protocol capability key.
        /// </summary>
        public const string MaxResponseCount = "MaxResponseCount";

        /// <summary>
        /// Initializes a new instance of the <see cref="GrowingObjectQueryStoreHandler"/> class.
        /// </summary>
        public GrowingObjectQueryStoreHandler() : base((int)Protocols.GrowingObjectQuery, "store", "customer")
        {
        }

        /// <summary>
        /// Sends a FindPartsResponse message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="parts">The list of <see cref="ObjectPart" /> objects.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns>The message identifier.</returns>
        public virtual long FindPartsResponse(IMessageHeader request, IList<ObjectPart> parts, string sortOrder)
        {
            if (!parts.Any())
            {
                return Acknowledge(request.MessageId, MessageFlags.NoData);
            }

            long messageId = 0;

            for (var i=0; i<parts.Count; i++)
            {
                var messageFlags = i < parts.Count - 1
                    ? MessageFlags.MultiPart
                    : MessageFlags.FinalPart;

                var header = CreateMessageHeader(Protocols.GrowingObjectQuery, MessageTypes.GrowingObjectQuery.FindPartsResponse, request.MessageId, messageFlags);

                var part = parts[i];

                var findPartsResponse = new FindPartsResponse
                {
                    Uri = part.Uri,
                    Uid = part.Uid,
                    ContentType = part.ContentType,
                    Data = part.Data,
                    ServerSortOrder = sortOrder ?? string.Empty
                };

                messageId = Session.SendMessage(header, findPartsResponse);
                sortOrder = string.Empty; // Only needs to be set in the first message
            }

            return messageId;
        }

        /// <summary>
        /// Handles the FindParts event from a customer.
        /// </summary>
        public event ProtocolEventHandler<FindParts, ObjectPartResponse> OnFindParts;

        /// <summary>
        /// Decodes the message based on the message type contained in the specified <see cref="IMessageHeader" />.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="decoder">The message decoder.</param>
        /// <param name="body">The message body.</param>
        protected override void HandleMessage(IMessageHeader header, Decoder decoder, string body)
        {
            switch (header.MessageType)
            {
                case (int)MessageTypes.GrowingObjectQuery.FindParts:
                    HandleFindParts(header, decoder.Decode<FindParts>(body));
                    break;

                default:
                    base.HandleMessage(header, decoder, body);
                    break;
            }
        }

        /// <summary>
        /// Handles the FindParts message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="findParts">The FindParts message.</param>
        protected virtual void HandleFindParts(IMessageHeader header, FindParts findParts)
        {
            var args = Notify(OnFindParts, header, findParts, new ObjectPartResponse());
            HandleFindParts(args);

            if (!args.Cancel)
            {
                FindPartsResponse(header, args.Context.ObjectParts, args.Context.ServerSortOrder);
            }
        }

        /// <summary>
        /// Handles the FindParts message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="ProtocolEventArgs{FindParts}"/> instance containing the event data.</param>
        protected virtual void HandleFindParts(ProtocolEventArgs<FindParts, ObjectPartResponse> args)
        {
        }
    }
}

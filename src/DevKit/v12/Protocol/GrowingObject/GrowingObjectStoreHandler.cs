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

namespace Energistics.Etp.v12.Protocol.GrowingObject
{
    /// <summary>
    /// Base implementation of the <see cref="IGrowingObjectStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp12ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.GrowingObject.IGrowingObjectStore" />
    public class GrowingObjectStoreHandler : Etp12ProtocolHandler, IGrowingObjectStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrowingObjectStoreHandler"/> class.
        /// </summary>
        public GrowingObjectStoreHandler() : base((int)Protocols.GrowingObject, "store", "customer")
        {
            RegisterMessageHandler<GetPart>(Protocols.GrowingObject, MessageTypes.GrowingObject.GetPart, HandleGetPart);
            RegisterMessageHandler<GetPartsByRange>(Protocols.GrowingObject, MessageTypes.GrowingObject.GetPartsByRange, HandleGetPartsByRange);
            RegisterMessageHandler<PutPart>(Protocols.GrowingObject, MessageTypes.GrowingObject.PutPart, HandlePutPart);
            RegisterMessageHandler<DeletePart>(Protocols.GrowingObject, MessageTypes.GrowingObject.DeletePart, HandleDeletePart);
            RegisterMessageHandler<DeletePartsByRange>(Protocols.GrowingObject, MessageTypes.GrowingObject.DeletePartsByRange, HandleDeletePartsByRange);
            RegisterMessageHandler<ReplacePartsByRange>(Protocols.GrowingObject, MessageTypes.GrowingObject.ReplacePartsByRange, HandleReplacePartsByRange);
            RegisterMessageHandler<GetPartsMetadata>(Protocols.GrowingObject, MessageTypes.GrowingObject.GetPartsMetadata, HandleGetPartsMetadata);
        }

        /// <summary>
        /// Sends a single list item as a response for GetPart and GetPartsByRange.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="uid">The ID of the element within the list.</param>
        /// <param name="contentType">The content type string.</param>
        /// <param name="data">The data.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        public long GetPartsResponse(string uri, string uid, string contentType, byte[] data, long correlationId, MessageFlags messageFlag = MessageFlags.MultiPartAndFinalPart)
        {
            var header = CreateMessageHeader(Protocols.GrowingObject, MessageTypes.GrowingObject.GetPartsResponse, correlationId, messageFlag);

            var message = new GetPartsResponse
            {
                ObjectPart = new ObjectPart
                {
                    Uri = uri,
                    Uid = uid,
                    ContentType = contentType,
                    Data = data
                }
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Sends the metadata describing the list items in the requested growing objects.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="metadata">The parts metadata.</param>
        /// <param name="errors">The errors.</param>
        /// <returns>The message identifier.</returns>
        public long GetPartsMetadataResponse(IMessageHeader request, IList<PartsMetadataInfo> metadata, IList<ErrorInfo> errors)
        {
            var header = CreateMessageHeader(Protocols.GrowingObject, MessageTypes.GrowingObject.GetPartsMetadataResponse, request.MessageId);

            if (metadata != null)
            {
                foreach (var info in metadata)
                {
                    if (info.CustomData == null)
                        info.CustomData = new Dictionary<string, DataValue>();
                }
            }

            var message = new GetPartsMetadataResponse
            {
                Metadata = metadata ?? new List<PartsMetadataInfo>(),
                Errors = errors ?? new List<ErrorInfo>()
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Handles the GetPart event from a customer.
        /// </summary>
        public event ProtocolEventHandler<GetPart> OnGetPart;

        /// <summary>
        /// Handles the GetPartsByRange event from a customer.
        /// </summary>
        public event ProtocolEventHandler<GetPartsByRange> OnGetPartsByRange;

        /// <summary>
        /// Handles the PutPart event from a customer.
        /// </summary>
        public event ProtocolEventHandler<PutPart> OnPutPart;

        /// <summary>
        /// Handles the DeletePart event from a customer.
        /// </summary>
        public event ProtocolEventHandler<DeletePart> OnDeletePart;

        /// <summary>
        /// Handles the DeletePartsByRange event from a customer.
        /// </summary>
        public event ProtocolEventHandler<DeletePartsByRange> OnDeletePartsByRange;

        /// <summary>
        /// Handles the ReplacePartsByRange event from a customer.
        /// </summary>
        public event ProtocolEventHandler<ReplacePartsByRange> OnReplacePartsByRange;

        /// <summary>
        /// Handles the GetPartsMetadata event from a customer.
        /// </summary>
        public event ProtocolEventHandler<GetPartsMetadata> OnGetPartsMetadata;

        /// <summary>
        /// Handles the GetPart message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GetPart message.</param>
        protected virtual void HandleGetPart(IMessageHeader header, GetPart message)
        {
            Notify(OnGetPart, header, message);
        }

        /// <summary>
        /// Handles the GetPartsByRange message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GetPartsByRange message.</param>
        protected virtual void HandleGetPartsByRange(IMessageHeader header, GetPartsByRange message)
        {
            Notify(OnGetPartsByRange, header, message);
        }

        /// <summary>
        /// Handles the PutPart message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The PutPart message.</param>
        protected virtual void HandlePutPart(IMessageHeader header, PutPart message)
        {
            Notify(OnPutPart, header, message);
        }

        /// <summary>
        /// Handles the DeletePart message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The DeletePart message.</param>
        protected virtual void HandleDeletePart(IMessageHeader header, DeletePart message)
        {
            Notify(OnDeletePart, header, message);
        }

        /// <summary>
        /// Handles the DeletePartsByRange message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The DeletePartsByRange message.</param>
        protected virtual void HandleDeletePartsByRange(IMessageHeader header, DeletePartsByRange message)
        {
            Notify(OnDeletePartsByRange, header, message);
        }

        /// <summary>
        /// Handles the ReplacePartsByRange message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The ReplacePartsByRange message.</param>
        protected virtual void HandleReplacePartsByRange(IMessageHeader header, ReplacePartsByRange message)
        {
            Notify(OnReplacePartsByRange, header, message);
        }

        /// <summary>
        /// Handles the GetPartsMetadata message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GetPartsMetadata message.</param>
        protected virtual void HandleGetPartsMetadata(IMessageHeader header, GetPartsMetadata message)
        {
            var args = Notify(OnGetPartsMetadata, header, message);
            var metadata = new List<PartsMetadataInfo>();
            var errors = new List<ErrorInfo>();

            HandleGetPartsMetadata(args, metadata, errors);

            if (!args.Cancel)
            {
                GetPartsMetadataResponse(header, metadata, errors);
            }
        }

        /// <summary>
        /// Handles the GetPartsMetadata message from a store.
        /// </summary>
        /// <param name="args">The <see cref="ProtocolEventArgs{GetPartsMetadata}" /> instance containing the event data.</param>
        /// <param name="metadata">The metadata.</param>
        /// <param name="errors">The errors.</param>
        protected virtual void HandleGetPartsMetadata(ProtocolEventArgs<GetPartsMetadata> args, IList<PartsMetadataInfo> metadata, IList<ErrorInfo> errors)
        {
        }
    }
}

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
using System.Linq;
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
            RegisterMessageHandler<GetParts>(Protocols.GrowingObject, MessageTypes.GrowingObject.GetParts, HandleGetParts);
            RegisterMessageHandler<GetPartsByRange>(Protocols.GrowingObject, MessageTypes.GrowingObject.GetPartsByRange, HandleGetPartsByRange);
            RegisterMessageHandler<PutParts>(Protocols.GrowingObject, MessageTypes.GrowingObject.PutParts, HandlePutParts);
            RegisterMessageHandler<DeleteParts>(Protocols.GrowingObject, MessageTypes.GrowingObject.DeleteParts, HandleDeleteParts);
            RegisterMessageHandler<DeletePartsByRange>(Protocols.GrowingObject, MessageTypes.GrowingObject.DeletePartsByRange, HandleDeletePartsByRange);
            RegisterMessageHandler<ReplacePartsByRange>(Protocols.GrowingObject, MessageTypes.GrowingObject.ReplacePartsByRange, HandleReplacePartsByRange);
            RegisterMessageHandler<GetPartsMetadata>(Protocols.GrowingObject, MessageTypes.GrowingObject.GetPartsMetadata, HandleGetPartsMetadata);
        }

        /// <summary>
        /// Handles the GetParts event from a customer.
        /// </summary>
        public event ProtocolEventWithErrorsHandler<GetParts, ObjectPart, ErrorInfo> OnGetParts;

        /// <summary>
        /// Sends a a list of parts as a response for GetParts to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="parts">The UIDs and data of the parts being returned.</param>
        /// <param name="errors">The errors, if any.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long GetPartsResponse(IMessageHeader request, string uri, IDictionary<string, ObjectPart> parts, IDictionary<string, ErrorInfo> errors, string format = "xml")
        {
            var header = CreateMessageHeader(Protocols.GrowingObject, MessageTypes.GrowingObject.GetPartsResponse, request.MessageId);
            var message = new GetPartsResponse
            {
                Uri = uri,
                Format = format ?? "xml",
            };

            return SendMultipartResponse(header, message, parts, errors, (m, i) => m.Parts = i);
        }

        /// <summary>
        /// Handles the PutParts event from a customer.
        /// </summary>
        public event ProtocolEventWithErrorsHandler<PutParts, ErrorInfo> OnPutParts;

        /// <summary>
        /// Handles the DeleteParts event from a customer.
        /// </summary>
        public event ProtocolEventWithErrorsHandler<DeleteParts, ErrorInfo> OnDeleteParts;

        /// <summary>
        /// Handles the GetPartsByRange event from a customer.
        /// </summary>
        public event ProtocolEventHandler<GetPartsByRange, IList<ObjectPart>> OnGetPartsByRange;

        /// <summary>
        /// Sends a a list of parts as a response for GetPartsByRange to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="parts">The UIDs and data of the parts being returned.</param>
        /// <param name="format">The format of the data (XML or JSON).</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long GetPartsByRangeResponse(IMessageHeader request, string uri, IList<ObjectPart> parts, string format = "xml")
        {
            var header = CreateMessageHeader(Protocols.GrowingObject, MessageTypes.GrowingObject.GetPartsByRangeResponse, request.MessageId);
            var message = new GetPartsByRangeResponse
            {
                Uri = uri,
                Format = format ?? "xml",
            };

            return SendMultipartResponse(header, message, parts, (m, i) => m.Parts = i);
        }

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
        public event ProtocolEventWithErrorsHandler<GetPartsMetadata, PartsMetadataInfo, ErrorInfo> OnGetPartsMetadata;

        /// <summary>
        /// Sends the metadata describing the parts in the requested growing objects to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="metadata">The parts metadata.</param>
        /// <param name="errors">The errors, if any.</param>
        /// <returns>The positive message identifier on success; otherwise, a negative number.</returns>
        public virtual long GetPartsMetadataResponse(IMessageHeader request, IDictionary<string, PartsMetadataInfo> metadata, IDictionary<string, ErrorInfo> errors)
        {
            var header = CreateMessageHeader(Protocols.GrowingObject, MessageTypes.GrowingObject.GetPartsMetadataResponse, request.MessageId);
            var message = new GetPartsMetadataResponse
            {
            };

            if (metadata == null)
                metadata = new Dictionary<string, PartsMetadataInfo>();

            foreach (var info in metadata.Values)
            {
                if (info.CustomData == null)
                    info.CustomData = new Dictionary<string, DataValue>();
            }

            return SendMultipartResponse(header, message, metadata, errors, (m, i) => m.Metadata = i);
        }

        /// <summary>
        /// Handles the GetParts message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GetParts message.</param>
        protected virtual void HandleGetParts(IMessageHeader header, GetParts message)
        {
            var args = Notify(OnGetParts, header, message, new Dictionary<string, ObjectPart>(), new Dictionary<string, ErrorInfo>());

            if (args.Cancel)
                return;

            if (!HandleGetParts(header, message, args.Context, args.Errors))
                return;

            GetPartsResponse(header, message.Uri, args.Context, args.Errors);
        }

        /// <summary>
        /// Handles the GetParts message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="response">The response.</param>
        /// <param name="errors">The errors.</param>
        protected virtual bool HandleGetParts(IMessageHeader header, GetParts message, IDictionary<string, ObjectPart> response, IDictionary<string, ErrorInfo> errors)
        {
            return true;
        }

        /// <summary>
        /// Handles the PutParts message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The PutParts message.</param>
        protected virtual void HandlePutParts(IMessageHeader header, PutParts message)
        {
            var args = Notify(OnPutParts, header, message, new Dictionary<string, ErrorInfo>());
            if (args.Cancel)
                return;

            if (!HandlePutParts(header, message, args.Errors))
                return;

            SendMultipartResponse(header, message, args.Errors);
        }

        /// <summary>
        /// Handles the PutParts message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="errors">The errors.</param>
        protected virtual bool HandlePutParts(IMessageHeader header, PutParts message, IDictionary<string, ErrorInfo> errors)
        {
            return true;
        }

        /// <summary>
        /// Handles the DeleteParts message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The DeleteParts message.</param>
        protected virtual void HandleDeleteParts(IMessageHeader header, DeleteParts message)
        {
            var args = Notify(OnDeleteParts, header, message, new Dictionary<string, ErrorInfo>());
            if (args.Cancel)
                return;
            if (!HandleDeleteParts(header, message, args.Errors))
                return;

            SendMultipartResponse(header, message, args.Errors);
        }

        /// <summary>
        /// Handles the DeleteParts message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="errors">The errors.</param>
        protected virtual bool HandleDeleteParts(IMessageHeader header, DeleteParts message, IDictionary<string, ErrorInfo> errors)
        {
            return true;
        }


        /// <summary>
        /// Handles the GetPartsByRange message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GetPartsByRange message.</param>
        protected virtual void HandleGetPartsByRange(IMessageHeader header, GetPartsByRange message)
        {
            var args = Notify(OnGetPartsByRange, header, message, new List<ObjectPart>());
            if (args.Cancel)
                return;

            if (!HandleGetPartsByRange(header, message, args.Context))
                return;

            GetPartsByRangeResponse(header, message.Uri, args.Context);
        }

        /// <summary>
        /// Handles the GetPartsByRange message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="response">The response.</param>
        protected virtual bool HandleGetPartsByRange(IMessageHeader header, GetPartsByRange message, IList<ObjectPart> response)
        {
            return true;
        }

        /// <summary>
        /// Handles the DeletePartsByRange message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The DeletePartsByRange message.</param>
        protected virtual void HandleDeletePartsByRange(IMessageHeader header, DeletePartsByRange message)
        {
            Notify(OnDeletePartsByRange, header, message);
        }

        /// <summary>
        /// Handles the ReplacePartsByRange message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The ReplacePartsByRange message.</param>
        protected virtual void HandleReplacePartsByRange(IMessageHeader header, ReplacePartsByRange message)
        {
            Notify(OnReplacePartsByRange, header, message);
        }


        /// <summary>
        /// Handles the GetPartsMetadata message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GetPartsMetadata message.</param>
        protected virtual void HandleGetPartsMetadata(IMessageHeader header, GetPartsMetadata message)
        {
            var args = Notify(OnGetPartsMetadata, header, message, new Dictionary<string, PartsMetadataInfo>(), new Dictionary<string, ErrorInfo>());
            if (args.Cancel)
                return;

            if (!HandleGetPartsMetadata(header, message, args.Context, args.Errors))
                return;

            GetPartsMetadataResponse(header, args.Context, args.Errors);
        }

        /// <summary>
        /// Handles the GetPartsMetadata message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The message.</param>
        /// <param name="response">The response.</param>
        /// <param name="errors">The errors.</param>
        protected virtual bool HandleGetPartsMetadata(IMessageHeader header, GetPartsMetadata message, IDictionary<string, PartsMetadataInfo> response, IDictionary<string, ErrorInfo> errors)
        {
            return true;
        }
    }
}

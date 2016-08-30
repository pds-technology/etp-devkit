﻿//----------------------------------------------------------------------- 
// ETP DevKit, 1.1
//
// Copyright 2016 Energistics
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

using Avro.IO;
using Energistics.Common;
using Energistics.Datatypes;
using Energistics.Datatypes.Object;

namespace Energistics.Protocol.Store
{
    /// <summary>
    /// Base implementation of the <see cref="IStoreStore"/> interface.
    /// </summary>
    /// <seealso cref="Energistics.Common.EtpProtocolHandler" />
    /// <seealso cref="Energistics.Protocol.Store.IStoreStore" />
    public class StoreStoreHandler : EtpProtocolHandler, IStoreStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreStoreHandler"/> class.
        /// </summary>
        public StoreStoreHandler() : base(Protocols.Store, "store", "customer")
        {
        }

        /// <summary>
        /// Sends an Object message to a customer.
        /// </summary>
        /// <param name="dataObject">The data object.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        public virtual long Object(DataObject dataObject, long correlationId, MessageFlags messageFlag = MessageFlags.FinalPart)
        {
            var header = CreateMessageHeader(Protocols.Store, MessageTypes.Store.Object, correlationId, messageFlag);

            var @object = new Object()
            {
                DataObject = dataObject
            };

            return Session.SendMessage(header, @object);
        }

        /// <summary>
        /// Handles the GetObject event from a customer.
        /// </summary>
        public event ProtocolEventHandler<GetObject, DataObject> OnGetObject;

        /// <summary>
        /// Handles the PutObject event from a customer.
        /// </summary>
        public event ProtocolEventHandler<PutObject> OnPutObject;

        /// <summary>
        /// Handles the DeleteObject event from a customer.
        /// </summary>
        public event ProtocolEventHandler<DeleteObject> OnDeleteObject;

        /// <summary>
        /// Decodes the message based on the message type contained in the specified <see cref="MessageHeader" />.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="decoder">The message decoder.</param>
        /// <param name="body">The message body.</param>
        protected override void HandleMessage(MessageHeader header, Decoder decoder, string body)
        {
            switch (header.MessageType)
            {
                case (int)MessageTypes.Store.GetObject:
                    HandleGetObject(header, decoder.Decode<GetObject>(body));
                    break;

                case (int)MessageTypes.Store.PutObject:
                    HandlePutObject(header, decoder.Decode<PutObject>(body));
                    break;

                case (int)MessageTypes.Store.DeleteObject:
                    HandleDeleteObject(header, decoder.Decode<DeleteObject>(body));
                    break;

                default:
                    base.HandleMessage(header, decoder, body);
                    break;
            }
        }

        /// <summary>
        /// Handles the GetObject message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="getObject">The GetObject message.</param>
        protected virtual void HandleGetObject(MessageHeader header, GetObject getObject)
        {
            var args = Notify(OnGetObject, header, getObject, new DataObject());
            HandleGetObject(args);

            if (args.Cancel)
                return;

            if (args.Context.Data == null || args.Context.Data.Length == 0)
                Object(args.Context, header.MessageId, MessageFlags.NoData);
            else
                Object(args.Context, header.MessageId);
        }

        /// <summary>
        /// Handles the GetObject message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="ProtocolEventArgs{GetObject, DataObject}"/> instance containing the event data.</param>
        protected virtual void HandleGetObject(ProtocolEventArgs<GetObject, DataObject> args)
        {
        }

        /// <summary>
        /// Handles the PutObject message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="putObject">The PutObject message.</param>
        protected virtual void HandlePutObject(MessageHeader header, PutObject putObject)
        {
            Notify(OnPutObject, header, putObject);
        }

        /// <summary>
        /// Handles the DeleteObject message from a customer.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="deleteObject">The DeleteObject message.</param>
        protected virtual void HandleDeleteObject(MessageHeader header, DeleteObject deleteObject)
        {
            Notify(OnDeleteObject, header, deleteObject);
        }
    }
}

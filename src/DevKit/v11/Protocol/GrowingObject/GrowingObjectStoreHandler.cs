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

using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;

namespace Energistics.Etp.v11.Protocol.GrowingObject
{
    /// <summary>
    /// Base implementation of the <see cref="IGrowingObjectStore"/> interface.
    /// </summary>
    /// <seealso cref="Etp11ProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v11.Protocol.GrowingObject.IGrowingObjectStore" />
    public class GrowingObjectStoreHandler : Etp11ProtocolHandler, IGrowingObjectStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrowingObjectStoreHandler"/> class.
        /// </summary>
        public GrowingObjectStoreHandler() : base((int)Protocols.GrowingObject, "store", "customer")
        {
            RegisterMessageHandler<GrowingObjectGet>(Protocols.GrowingObject, MessageTypes.GrowingObject.GrowingObjectGet, HandleGrowingObjectGet);
            RegisterMessageHandler<GrowingObjectGetRange>(Protocols.GrowingObject, MessageTypes.GrowingObject.GrowingObjectGetRange, HandleGrowingObjectGetRange);
            RegisterMessageHandler<GrowingObjectPut>(Protocols.GrowingObject, MessageTypes.GrowingObject.GrowingObjectPut, HandleGrowingObjectPut);
            RegisterMessageHandler<GrowingObjectDelete>(Protocols.GrowingObject, MessageTypes.GrowingObject.GrowingObjectDelete, HandleGrowingObjectDelete);
            RegisterMessageHandler<GrowingObjectDeleteRange>(Protocols.GrowingObject, MessageTypes.GrowingObject.GrowingObjectDeleteRange, HandleGrowingObjectDeleteRange);
        }

        /// <summary>
        /// Sends a single list item as a response for Get and GetRange.
        /// </summary>
        /// <param name="uri">The URI of the parent object.</param>
        /// <param name="contentType">The content type string.</param>
        /// <param name="data">The data.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        public long ObjectFragment(string uri, string contentType, byte[] data, long correlationId, MessageFlags messageFlag = MessageFlags.MultiPartAndFinalPart)
        {
            var header = CreateMessageHeader(Protocols.GrowingObject, MessageTypes.GrowingObject.ObjectFragment, correlationId, messageFlag);

            var message = new ObjectFragment
            {
                Uri = uri,
                ContentType = contentType,
                ContentEncoding = "text/xml",
                Data = data
            };

            return Session.SendMessage(header, message);
        }

        /// <summary>
        /// Handles the GrowingObjectGet event from a customer.
        /// </summary>
        public event ProtocolEventHandler<GrowingObjectGet> OnGrowingObjectGet;

        /// <summary>
        /// Handles the GrowingObjectGetRange event from a customer.
        /// </summary>
        public event ProtocolEventHandler<GrowingObjectGetRange> OnGrowingObjectGetRange;

        /// <summary>
        /// Handles the GrowingObjectPut event from a customer.
        /// </summary>
        public event ProtocolEventHandler<GrowingObjectPut> OnGrowingObjectPut;

        /// <summary>
        /// Handles the GrowingObjectDelete event from a customer.
        /// </summary>
        public event ProtocolEventHandler<GrowingObjectDelete> OnGrowingObjectDelete;

        /// <summary>
        /// Handles the GrowingObjectDeleteRange event from a customer.
        /// </summary>
        public event ProtocolEventHandler<GrowingObjectDeleteRange> OnGrowingObjectDeleteRange;

        /// <summary>
        /// Handles the GrowingObjectGet message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GrowingObjectGet message.</param>
        protected virtual void HandleGrowingObjectGet(IMessageHeader header, GrowingObjectGet message)
        {
            Notify(OnGrowingObjectGet, header, message);
        }

        /// <summary>
        /// Handles the GrowingObjectGetRange message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GrowingObjectGetRange message.</param>
        protected virtual void HandleGrowingObjectGetRange(IMessageHeader header, GrowingObjectGetRange message)
        {
            Notify(OnGrowingObjectGetRange, header, message);
        }

        /// <summary>
        /// Handles the GrowingObjectPut message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GrowingObjectPut message.</param>
        protected virtual void HandleGrowingObjectPut(IMessageHeader header, GrowingObjectPut message)
        {
            Notify(OnGrowingObjectPut, header, message);
        }

        /// <summary>
        /// Handles the GrowingObjectDelete message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GrowingObjectDelete message.</param>
        protected virtual void HandleGrowingObjectDelete(IMessageHeader header, GrowingObjectDelete message)
        {
            Notify(OnGrowingObjectDelete, header, message);
        }

        /// <summary>
        /// Handles the GrowingObjectDeleteRange message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="message">The GrowingObjectDeleteRange message.</param>
        protected virtual void HandleGrowingObjectDeleteRange(IMessageHeader header, GrowingObjectDeleteRange message)
        {
            Notify(OnGrowingObjectDeleteRange, header, message);
        }
    }
}

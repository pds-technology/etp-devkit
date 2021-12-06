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
using System;
using System.Collections.Generic;
using System.Linq;

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
        public GrowingObjectStoreHandler() : base((int)Protocols.GrowingObject, Roles.Store, Roles.Customer)
        {
            RegisterMessageHandler<GrowingObjectGet>(Protocols.GrowingObject, MessageTypes.GrowingObject.GrowingObjectGet, HandleGrowingObjectGet);
            RegisterMessageHandler<GrowingObjectGetRange>(Protocols.GrowingObject, MessageTypes.GrowingObject.GrowingObjectGetRange, HandleGrowingObjectGetRange);
            RegisterMessageHandler<GrowingObjectPut>(Protocols.GrowingObject, MessageTypes.GrowingObject.GrowingObjectPut, HandleGrowingObjectPut);
            RegisterMessageHandler<GrowingObjectDelete>(Protocols.GrowingObject, MessageTypes.GrowingObject.GrowingObjectDelete, HandleGrowingObjectDelete);
            RegisterMessageHandler<GrowingObjectDeleteRange>(Protocols.GrowingObject, MessageTypes.GrowingObject.GrowingObjectDeleteRange, HandleGrowingObjectDeleteRange);
        }

        /// <summary>
        /// Sends a object fragments as a response for Get and GetRange.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="fragment">The fragment.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        public EtpMessage<ObjectFragment> ObjectFragment(IMessageHeader correlatedHeader, ObjectFragment fragment, bool isFinalPart = true)
        {
            var body = fragment;

            return SendResponse(body, correlatedHeader, isMultiPart: true, isFinalPart: isFinalPart);
        }

        /// <summary>
        /// Sends a complete multi-part set of ObjectFragment messages to a customer for the list of <see cref="ObjectFragmentResponse"/> objects.
        /// If there are no fragments in the list, an Acknowledge message is sent with the NoData flag sent.
        /// If there are fragments in the list and acknowledge is requested, an Acknowledge message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="fragments">The fragments.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.  If there are no fragments in the list, a placeholder message with a header matching the sent Acknowledge is returned.</returns>
        public virtual EtpMessage<ObjectFragment> ObjectFragments(IMessageHeader correlatedHeader, IList<ObjectFragment> fragments, bool setFinalPart = true)
        {
            if (fragments == null || fragments.Count == 0)
            {
                var ack = Acknowledge(correlatedHeader, true);
                if (ack == null)
                    return null;

                var header = CreateMessageHeader<ObjectFragment>();
                header.MessageFlags = ack.Header.MessageFlags;
                header.MessageId = ack.Header.MessageId;
                header.CorrelationId = ack.Header.CorrelationId;
                header.Timestamp = ack.Header.Timestamp;
                return new EtpMessage<ObjectFragment>(header, new ObjectFragment());
            }

            EtpMessage<ObjectFragment> message = null;

            for (int i = 0; i < fragments?.Count; i++)
            {
                var ret = ObjectFragment(correlatedHeader, fragments[i], isFinalPart: (i == fragments.Count - 1 && setFinalPart));
                if (ret == null)
                    return null;
                message = message ?? ret;
            }

            return message;
        }

        /// <summary>
        /// Handles the GrowingObjectGet event from a customer.
        /// </summary>
        public event EventHandler<RequestEventArgs<GrowingObjectGet, ObjectFragment>> OnGrowingObjectGet;

        /// <summary>
        /// Handles the GrowingObjectGetRange event from a customer.
        /// </summary>
        public event EventHandler<ListRequestEventArgs<GrowingObjectGetRange, ObjectFragment>> OnGrowingObjectGetRange;

        /// <summary>
        /// Handles the GrowingObjectPut event from a customer.
        /// </summary>
        public event EventHandler<VoidRequestEventArgs<GrowingObjectPut>> OnGrowingObjectPut;

        /// <summary>
        /// Handles the GrowingObjectDelete event from a customer.
        /// </summary>
        public event EventHandler<VoidRequestEventArgs<GrowingObjectDelete>> OnGrowingObjectDelete;

        /// <summary>
        /// Handles the GrowingObjectDeleteRange event from a customer.
        /// </summary>
        public event EventHandler<VoidRequestEventArgs<GrowingObjectDeleteRange>> OnGrowingObjectDeleteRange;

        /// <summary>
        /// Handles the GrowingObjectGet message from a customer.
        /// </summary>
        /// <param name="message">The GrowingObjectGet message.</param>
        protected virtual void HandleGrowingObjectGet(EtpMessage<GrowingObjectGet> message)
        {
            HandleRequestMessage(message, OnGrowingObjectGet, HandleGrowingObjectGet,
                responseMethod: (args) => ObjectFragment(args.Request?.Header, args.Response));
        }

        /// <summary>
        /// Handles the GrowingObjectGet message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="RequestEventArgs{GrowingObjectGet, ObjectFragment}"/> instance containing the event data.</param>
        protected virtual void HandleGrowingObjectGet(RequestEventArgs<GrowingObjectGet, ObjectFragment> args)
        {
        }

        /// <summary>
        /// Handles the GrowingObjectGetRange message from a customer.
        /// </summary>
        /// <param name="message">The GrowingObjectGetRange message.</param>
        protected virtual void HandleGrowingObjectGetRange(EtpMessage<GrowingObjectGetRange> message)
        {
            HandleRequestMessage(message, OnGrowingObjectGetRange, HandleGrowingObjectGetRange,
                responseMethod: (args) => ObjectFragments(args.Request?.Header, args.Responses, setFinalPart: !args.HasErrors));
        }

        /// <summary>
        /// Handles the GrowingObjectGetRange message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="ListRequestEventArgs{GrowingObjectGetRange, ObjectFragment}"/> instance containing the event data.</param>
        protected virtual void HandleGrowingObjectGetRange(ListRequestEventArgs<GrowingObjectGetRange, ObjectFragment> args)
        {
        }

        /// <summary>
        /// Handles the GrowingObjectPut message from a customer.
        /// </summary>
        /// <param name="message">The GrowingObjectPut message.</param>
        protected virtual void HandleGrowingObjectPut(EtpMessage<GrowingObjectPut> message)
        {
            HandleRequestMessage(message, OnGrowingObjectPut, HandleGrowingObjectPut);
        }

        /// <summary>
        /// Handles the GrowingObjectPut message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="VoidRequestEventArgs{GrowingObjectPut}"/> instance containing the event data.</param>
        protected virtual void HandleGrowingObjectPut(VoidRequestEventArgs<GrowingObjectPut> args)
        {
        }

        /// <summary>
        /// Handles the GrowingObjectDelete message from a customer.
        /// </summary>
        /// <param name="message">The GrowingObjectDelete message.</param>
        protected virtual void HandleGrowingObjectDelete(EtpMessage<GrowingObjectDelete> message)
        {
            HandleRequestMessage(message, OnGrowingObjectDelete, HandleGrowingObjectDelete);
        }

        /// <summary>
        /// Handles the GrowingObjectDelete message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="VoidRequestEventArgs{GrowingObjectDelete}"/> instance containing the event data.</param>
        protected virtual void HandleGrowingObjectDelete(VoidRequestEventArgs<GrowingObjectDelete> args)
        {
        }

        /// <summary>
        /// Handles the GrowingObjectDeleteRange message from a customer.
        /// </summary>
        /// <param name="message">The GrowingObjectDeleteRange message.</param>
        protected virtual void HandleGrowingObjectDeleteRange(EtpMessage<GrowingObjectDeleteRange> message)
        {
            HandleRequestMessage(message, OnGrowingObjectDeleteRange, HandleGrowingObjectDeleteRange);
        }

        /// <summary>
        /// Handles the GrowingObjectDeleteRange message from a customer.
        /// </summary>
        /// <param name="args">The <see cref="VoidRequestEventArgs{GrowingObjectDeleteRange}"/> instance containing the event data.</param>
        protected virtual void HandleGrowingObjectDeleteRange(VoidRequestEventArgs<GrowingObjectDeleteRange> args)
        {
        }
    }
}

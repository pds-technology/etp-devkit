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

namespace Energistics.Etp.v11.Protocol.GrowingObject
{
    /// <summary>
    /// Defines the interface that must be implemented by the store role of the growing object protocol.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.IProtocolHandler" />
    [ProtocolRole((int)Protocols.GrowingObject, Roles.Store, Roles.Customer)]
    public interface IGrowingObjectStore : IProtocolHandler
    {
        /// <summary>
        /// Sends a object fragments as a response for Get and GetRange.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the message to send is correlated with.</param>
        /// <param name="fragment">The fragment.</param>
        /// <param name="isFinalPart">Whether or not this is the final part of a multi-part message.</param>
        /// <returns>The sent message on success; <c>null</c> otherwise.</returns>
        EtpMessage<ObjectFragment> ObjectFragment(IMessageHeader correlatedHeader, ObjectFragment fragment, bool isFinalPart = true);

        /// <summary>
        /// Sends a complete multi-part set of ObjectFragment messages to a customer for the list of <see cref="ObjectFragmentResponse"/> objects.
        /// If there are no fragments in the list, an Acknowledge message is sent with the NoData flag sent.
        /// If there are fragments in the list and acknowledge is requested, an Acknowledge message is sent.
        /// </summary>
        /// <param name="correlatedHeader">The message header that the messages to send are correlated with.</param>
        /// <param name="fragments">The fragments.</param>
        /// <param name="setFinalPart">Whether or not the final part flag should be set on the last message.</param>
        /// <returns>The first message sent in the response on success; <c>null</c> otherwise.  If there are no fragments in the list, a placeholder message with a header matching the sent Acknowledge is returned.</returns>
        EtpMessage<ObjectFragment> ObjectFragments(IMessageHeader correlatedHeader, IList<ObjectFragment> fragments, bool setFinalPart = true);

        /// <summary>
        /// Handles the GrowingObjectGet event from a customer.
        /// </summary>
        event EventHandler<RequestEventArgs<GrowingObjectGet, ObjectFragment>> OnGrowingObjectGet;

        /// <summary>
        /// Handles the GrowingObjectGetRange event from a customer.
        /// </summary>
        event EventHandler<ListRequestEventArgs<GrowingObjectGetRange, ObjectFragment>> OnGrowingObjectGetRange;

        /// <summary>
        /// Handles the GrowingObjectPut event from a customer.
        /// </summary>
        event EventHandler<VoidRequestEventArgs<GrowingObjectPut>> OnGrowingObjectPut;

        /// <summary>
        /// Handles the GrowingObjectDelete event from a customer.
        /// </summary>
        event EventHandler<VoidRequestEventArgs<GrowingObjectDelete>> OnGrowingObjectDelete;

        /// <summary>
        /// Handles the GrowingObjectDeleteRange event from a customer.
        /// </summary>
        event EventHandler<VoidRequestEventArgs<GrowingObjectDeleteRange>> OnGrowingObjectDeleteRange;
    }
}

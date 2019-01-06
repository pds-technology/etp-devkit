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
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.StoreNotification
{
    /// <summary>
    /// Defines the interface that must be implemented by the store role of the store notification protocol.
    /// </summary>
    /// <seealso cref="IProtocolHandler" />
    [ProtocolRole((int)Protocols.StoreNotification, "store", "customer")]
    public interface IStoreNotificationStore : IProtocolHandler
    {
        /// <summary>
        /// Sends an ObjectChanged message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="change">The object change.</param>
        /// <returns>The message identifier.</returns>
        long ObjectChanged(IMessageHeader request, ObjectChange change);

        /// <summary>
        /// Sends an ObjectDeleted message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="changeTime">The change time.</param>
        /// <returns>The message identifier.</returns>
        long ObjectDeleted(IMessageHeader request, string uri, long changeTime);

        /// <summary>
        /// Sends an ObjectAccessRevoked message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="changeTime">The change time.</param>
        /// <returns>The message identifier.</returns>
        long ObjectAccessRevoked(IMessageHeader request, string uri, long changeTime);

        /// <summary>
        /// Handles the SubscribeNotification event from a customer.
        /// </summary>
        event ProtocolEventHandler<SubscribeNotification> OnSubscribeNotification;

        /// <summary>
        /// Handles the UnsubscribeNotification event from a customer.
        /// </summary>
        event ProtocolEventHandler<UnsubscribeNotification> OnUnsubscribeNotification;
    }
}

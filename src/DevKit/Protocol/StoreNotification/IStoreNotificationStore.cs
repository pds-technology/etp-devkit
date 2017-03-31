//----------------------------------------------------------------------- 
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

using Energistics.Common;
using Energistics.Datatypes;
using Energistics.Datatypes.Object;

namespace Energistics.Protocol.StoreNotification
{
    /// <summary>
    /// Defines the interface that must be implemented by the store role of the store notification protocol.
    /// </summary>
    /// <seealso cref="Energistics.Common.IProtocolHandler" />
    [ProtocolRole(Protocols.Store, "store", "customer")]
    public interface IStoreNotificationStore : IProtocolHandler
    {
        /// <summary>
        /// Sends a ChangeNotification message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="change">The object change.</param>
        /// <returns>The message identifier.</returns>
        long ChangeNotification(MessageHeader request, ObjectChange change);

        /// <summary>
        /// Sends a NotificationRequestDeleteNotification message to a customer.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="change">The object change.</param>
        /// <returns>The message identifier.</returns>
        long DeleteNotification(MessageHeader request, ObjectChange change);

        /// <summary>
        /// Handles the NotificationRequest event from a customer.
        /// </summary>
        event ProtocolEventHandler<NotificationRequest> OnNotificationRequest;

        /// <summary>
        /// Handles the CancelNotification event from a customer.
        /// </summary>
        event ProtocolEventHandler<CancelNotification> OnCancelNotification;
    }
}
